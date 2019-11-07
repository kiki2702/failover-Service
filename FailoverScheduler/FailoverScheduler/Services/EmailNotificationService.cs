using FailoverScheduler.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
//using System.Net.Mail;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Authentication;
using EASendMail; 

namespace FailoverScheduler.Services
{
    public class EmailNotificationService : IEmailNotificationService 
    {
        private readonly ILogger _logger;
        public EmailNotificationService(ILogger<EmailNotificationService> logger)
        {
            _logger = logger;
        }
       

        public void  SendEmail()
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("noreply", "noreply@unitedtractors.com"));
            message.To.Add(new MailboxAddress("cst_dev101", "cst_dev101@unitedtractors.com"));
            message.Subject = "How you doin'?";

            message.Body = new TextPart("plain")
            {
                Text = @"Hey Chandler,I just wanted to let you know that Monica and I were going to go play some paintball, you in?-- Joey"
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                try
                {
                    client.Connect("mail3.unitedtractors.com", 587, false);
                }
                catch (SmtpCommandException e)
                {
                    _logger.LogError(e, e.Message);
                    throw new ApplicationException(e.Message);
                }

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                try
                {
                    client.Authenticate("apidev@unitedtractors.com", "connect,123");
                }
                catch (AuthenticationException e)
                {
                    _logger.LogError(e, e.Message);
                    throw new ApplicationException(e.Message);
                }
                
                try
                {
                    client.Send(message);
                }
                catch (SmtpCommandException e)
                {
                    _logger.LogError(e, e.Message);
                }
                
                client.Disconnect(true);

            }

            //try
            //{
            //    SmtpMail oMail = new SmtpMail("TryIt");

            //    // Your gmail email address
            //    oMail.From = "dwikyfany@gmail.com";
            //    // Set recipient email address
            //    oMail.To = "super.dwiky27@gmail.com";

            //    // Set email subject
            //    oMail.Subject = "test email from gmail account";
            //    // Set email body
            //    oMail.TextBody = "this is a test email sent from c# project with gmail.";

            //    // Gmail SMTP server address
            //    SmtpServer oServer = new SmtpServer("smtp.gmail.com");

            //    // Gmail user authentication
            //    // For example: your email is "gmailid@gmail.com", then the user should be the same
            //    oServer.User = "dwikyfany";
            //    oServer.Password = "19970227DW";

            //    // Set 465 port
            //    oServer.Port = 465;

            //    // detect SSL/TLS automatically
            //    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

            //    Console.WriteLine("start to send email over SSL ...");

            //    EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();
            //    oSmtp.SendMail(oServer, oMail);

            //    Console.WriteLine("email was sent successfully!");
            //}
            //catch (Exception ep)
            //{
            //    Console.WriteLine("failed to send email with the following error:");
            //    Console.WriteLine(ep.Message);
            //}

        }
    }

}
