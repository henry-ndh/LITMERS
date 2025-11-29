using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TFU.Services
{
	// This class is used by the application to send email for account confirmation and password reset.
	// For more details see https://go.microsoft.com/fwlink/?LinkID=532713
	public class EmailSender : IEmailSender
	{
		private readonly IConfiguration _configuration;

		public EmailSender(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public Task SendEmailAsync(string email, string subject, string content)
		{
			var emailSection = _configuration.GetSection("DefaultEmailConfig");
			var host = emailSection["Host"];
			var port = emailSection["Post"];
			var username = emailSection["Username"];
			var password = emailSection["Password"];

			try
			{
				// Command-line argument must be the SMTP host.
				SmtpClient client = new SmtpClient(host, int.Parse(port));
				client.EnableSsl = true;
				client.Credentials = new NetworkCredential(username, password);
				MailAddress from = new MailAddress(username);
				// Set destinations for the email message.
				MailAddress to = new MailAddress(email);
				// Specify the message content.
				MailMessage message = new MailMessage(from, to);
				message.Body = content;
				message.BodyEncoding = System.Text.Encoding.UTF8;
				message.IsBodyHtml = true;
				message.Subject = subject;
				message.SubjectEncoding = System.Text.Encoding.UTF8;
				// Set the method that is called back when the send operation ends.
				client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
				// The userState can be any object that allows your callback 
				// method to identify this send operation.
				// For this example, the userToken is a string constant.
				string userState = Guid.NewGuid().ToString();
				client.SendAsync(message, userState);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public Task SendEmailAffiliateAsync(string email, string subject, string content)
		{
			var emailSection = _configuration.GetSection("AffiliateEmailConfig");
			var host = emailSection["Host"];
			var port = emailSection["Post"];
			var username = emailSection["Username"];
			var password = emailSection["Password"];

			try
			{
				// Command-line argument must be the SMTP host.
				SmtpClient client = new SmtpClient(host, int.Parse(port));
				client.EnableSsl = true;
				client.Credentials = new NetworkCredential(username, password);
				MailAddress from = new MailAddress(username);
				// Set destinations for the email message.
				MailAddress to = new MailAddress(email);
				// Specify the message content.
				MailMessage message = new MailMessage(from, to);
				message.Body = content;
				message.BodyEncoding = System.Text.Encoding.UTF8;
				message.IsBodyHtml = true;
				message.Subject = subject;
				message.SubjectEncoding = System.Text.Encoding.UTF8;
				// Set the method that is called back when the send operation ends.
				client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
				// The userState can be any object that allows your callback 
				// method to identify this send operation.
				// For this example, the userToken is a string constant.
				string userState = Guid.NewGuid().ToString();
				client.SendAsync(message, userState);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		/// <summary>
		/// This method to determined when this email successful sent.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="AsyncCompletedEventArgs"/> instance containing the event data.</param>
		private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
		{
			// Get the unique identifier for this asynchronous operation.
			String token = (string)e.UserState;

			if (e.Cancelled)
			{
				Console.WriteLine("[{0}] Send canceled.", token);
			}
			if (e.Error != null)
			{
				Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
			}
			else
			{
				Console.WriteLine("Message sent.");
			}
		}

	}
}
