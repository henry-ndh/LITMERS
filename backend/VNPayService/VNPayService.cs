using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.DAL.Interface;
using App.Entity.Models;
using App.Entity.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using VNPayService.Config;
using VNPayService.DTO;
using VNPayService.VNPlayPackage;

namespace VNPayService
{
    public class VNPayService : IVnPayService
    {
        private readonly IGenericRepository<OrderModel> _orderRepository;
        private readonly VNPayConfig _vnPayConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VNPayService(IGenericRepository<OrderModel> orderRepository, 
            VNPayConfig config, 
            IHttpContextAccessor httpContextAccessor)
        {
            _vnPayConfig = config;
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> CreatePayment(CreateOrder request)
        {
            try
            {
          
                var getOrder = await _orderRepository.GetByIdAsync(request.OrderId);

                if (getOrder == null)
                {
                    return "404";
                }

                var createVnPayOrder = new OrderInfo
                {
                    OrderID = request.OrderId,
                    Amount = getOrder.Amount,
                    CreatedDate = DateTime.Now,
                    Des = request.Des,
                    locale = "VN",
                    Status = 0,
                };

       
                var vnPay = new VNPayLibrary();
                vnPay.AddRequestData("vnp_Version", "2.1.0");
                vnPay.AddRequestData("vnp_Command", "pay");
                vnPay.AddRequestData("vnp_TmnCode", _vnPayConfig.TmnCode);
                vnPay.AddRequestData("vnp_Amount", (createVnPayOrder.Amount * 100).ToString());
                vnPay.AddRequestData("vnp_CreateDate", createVnPayOrder.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnPay.AddRequestData("vnp_CurrCode", "VND");
                vnPay.AddRequestData("vnp_IpAddr", _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
                vnPay.AddRequestData("vnp_Locale", createVnPayOrder.locale);
                vnPay.AddRequestData("vnp_OrderInfo", createVnPayOrder.Des);
                vnPay.AddRequestData("vnp_OrderType", "other");
                vnPay.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
                vnPay.AddRequestData("vnp_TxnRef", createVnPayOrder.OrderID.ToString());

                string paymentUrl = vnPay.CreateRequestUrl(_vnPayConfig.PaymentUrl,_vnPayConfig.SecretKey);
                return paymentUrl;
            }
            catch (Exception ex) { 
            
                throw new Exception(ex.ToString());

            }

        }

        public async Task<string> VnPayReturn()
        {
            var vnPaydata = _httpContextAccessor.HttpContext.Request.Query;
            var vnPay = new VNPayLibrary();
            foreach (var item in vnPaydata.Keys) {
                if (!string.IsNullOrEmpty(item) && item.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(item, vnPaydata[item]);
                }
            }

            string txnRefStr = vnPay.GetResponseData("vnp_TxnRef");
            long orderId = 0;
            if (!long.TryParse(txnRefStr, out orderId))
            {
                return "{\"RspCode\":\"01\",\"Message\":\"Invalid transaction reference\"}";
            }

            string amountStr = vnPay.GetResponseData("vnp_Amount");
            long vnp_Amount = 0;
            if (!long.TryParse(amountStr, out vnp_Amount))
            {
                return "{\"RspCode\":\"01\",\"Message\":\"Invalid amount\"}";
            }
            vnp_Amount /= 100;

            string tranIdStr = vnPay.GetResponseData("vnp_TransactionNo");
            long vnpayTranId = 0;
            if (!long.TryParse(tranIdStr, out vnpayTranId))
            {
                return "{\"RspCode\":\"01\",\"Message\":\"Invalid transaction number\"}";
            }

            string vnp_ResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnPay.GetResponseData("vnp_TransactionStatus");
            string vnp_SecureHash = vnPaydata["vnp_SecureHash"]!;

            bool checkSignature = vnPay.ValidateSignature(vnp_SecureHash, _vnPayConfig.SecretKey);

            if (!checkSignature)    
            {
                return "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
            }

            var order = await _orderRepository.GetByIdAsync(orderId);


            if (order == null)
            {
                return "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
            }

            if (order.Amount != vnp_Amount)
            {
                return "{\"RspCode\":\"04\",\"Message\":\"Invalid amount\"}";
            }

            if (order.PaymentStatus != PaymentStatus.PENDING)
            {
                return "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
            }

            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                order.PaymentStatus = PaymentStatus.SUCCESS;
            }
            else
            {
                order.PaymentStatus = PaymentStatus.CANCELLED;
            }

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            return "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
        }

        public async Task<PaymentResponse> ProcessVnPayReturn(Dictionary<string, string> queryParams)
        {
            try
            {
                var vnp_HashSecret = _vnPayConfig?.SecretKey;
                VNPayLibrary vnpay = new VNPayLibrary();

                foreach (var key in queryParams)
                {
                    if (!string.IsNullOrEmpty(key.Key) && key.Key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key.Key, key.Value);
                    }
                }

                var vnp_SecureHash = queryParams["vnp_SecureHash"];
                var checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (!checkSignature)
                {
                    return new PaymentResponse
                    {
                        IsSuccessful = false,
                        Message = "Chữ ký không hợp lệ."
                    };
                }

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                string bankCode = vnpay.GetResponseData("vnp_BankCode");
                string terminalId = vnpay.GetResponseData("vnp_TmnCode");

                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    return new PaymentResponse
                    {
                        IsSuccessful = true,
                        Message = "Giao dịch thành công.",
                        OrderId = orderId,
                        VnpayTransactionId = vnpayTranId,
                        Amount = vnp_Amount,
                        BankCode = bankCode,
                        TerminalId = terminalId
                    };
                }

                return new PaymentResponse
                {
                    IsSuccessful = false,
                    Message = $"Giao dịch thất bại. Mã lỗi: {vnp_ResponseCode}"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}

