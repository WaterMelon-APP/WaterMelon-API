using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterMelon_API.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace WaterMelon_API.Services
{
    public interface IEmailService
    {
        void Send(string from, string to, string subject, string html);
    }

    public class EmailService
    {
        public struct EmailSettings
        {
            public string SmtpHost;
            public int SmtpPort;
            public MailKit.Security.SecureSocketOptions SmtpSecure;
            public string FontColor;
            public string Font2Color;
        }
        private EmailSettings _emailSettings;

        public EmailService()
        {
            this._emailSettings.SmtpHost = "smtp.gmail.com";
            this._emailSettings.SmtpPort = 587;
            this._emailSettings.SmtpSecure = SecureSocketOptions.StartTls;
            this._emailSettings.FontColor = "projectwatermeloneip@gmail.com";
            this._emailSettings.Font2Color = "WaterMelon??!!2020";
        }

        public string CreateMailBody(string eventCreator, string eventName)
        {
            return ("WaterMelon : " + eventCreator + " vous à invité à " + eventName); // Remplacer par template mails
        }

        public string CreateMailSubject(string eventCreator, string eventName)
        {
            return ("WaterMelon : " + eventCreator + " vous à invité à " + eventName);
        }

        public void Send(string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.FontColor));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.FontColor, _emailSettings.Font2Color);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}