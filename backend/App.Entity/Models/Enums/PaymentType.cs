using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Entity.Models.Enums
{
    public enum PaymentType
    {
        /// <summary>
        /// Thanh toán để tổ chức một sự kiện.
        /// </summary>
        ORGANIZER_EVENT = 1,

        /// <summary>
        /// Thanh toán từ người tham dự sự kiện.
        /// </summary>
        PARTICIPANT_EVENT = 2,

        /// <summary>
        /// Thanh toán để mua sản phẩm trong sự kiện.
        /// </summary>
        PRODUCT_PURCHASE = 3
    }
}
