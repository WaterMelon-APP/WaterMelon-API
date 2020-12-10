using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using WaterMelon_API.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using Microsoft.Extensions.Configuration;
//using System.Net;
//using System.Net.Mail;

namespace WaterMelon_API.Services
{
    public interface IEmailService
    {
        void Send(string from, string to, string subject, string html);
    }

    public class EmailService
    {
        const string c_webAppUrl = "http://localhost:4200/";

        private readonly IConfiguration _configuration;
        private readonly EventService _eventService;

        public struct EmailSettings
        {
            public string SmtpHost;
            public int SmtpPort;
            public MailKit.Security.SecureSocketOptions SmtpSecure;
            public string FontColor;
            public string Font2Color;
        }
        private EmailSettings _emailSettings;

        public EmailService(IConfiguration config, EventService eventService)
        {
            _configuration = config;
            _eventService = eventService;
            this._emailSettings.SmtpHost = "smtp.gmail.com";
            this._emailSettings.SmtpPort = 465;
            //this._emailSettings.SmtpSecure = SecureSocketOptions.StartTls;
            this._emailSettings.FontColor = _configuration["stmplogging:watermelonmaillogin"];
            this._emailSettings.Font2Color = _configuration["stmplogging:watermelonmailpassword"];
        }

        public string CreateInviteMailBody(string eventCreator, string eventName, string eventId)
        {
            string first = File.ReadAllText("MailTemplate/EmailTemplateFirst.html");
            string last = File.ReadAllText("MailTemplate/EmailTemplateLast.html");


            string bodyReturn = first + "<h5 style=\"font-size:18px;border-radius:12px;color:white;background-color: #FD5757;padding:40px;margin:0px;margin-top:-4px;\">Nouvelle invitation !</h5></div><div class=\"card-body\"><br><br><img src=\"watermelon_logo.jpg\" height=\"160px\"/><br><br><p>";
            bodyReturn += "Tu as été invité(e) à l'événement</p><h3>" + eventName;
            bodyReturn += "</h3><p>par</par><h5 style=\"font-size:16px;color:FD5757;\">" + eventCreator + "</h5><a href =\"";
            bodyReturn += c_webAppUrl + "event/" + eventId + "\" " + last;
            return (bodyReturn);
        }
        public string CreateModifyMailBody(string eventName, string eventId)
        {
            string first = File.ReadAllText("MailTemplate/EmailTemplateFirst.html");
            string last = File.ReadAllText("MailTemplate/EmailTemplateLast.html");


            string bodyReturn = first + "<h5 style=\"font-size:18px;border-radius:12px;color:white;background-color: #FD5757;padding:60px;margin:0px;margin-top:-4px;\">Modification d'événement !</h5></div><div class=\"card-body\"><br><br><img src=\"watermelon_logo.jpg\" height=\"160px\"/><br><br><p>";
            bodyReturn += "L'événement auquel tu participes</p><h3>" + eventName;
            bodyReturn += "</h3><p>a été modifié !<br></p><a href =\"";
            bodyReturn += c_webAppUrl + "event/" + eventId + "\" " + last;
            return (bodyReturn);
        }
        public string CreatePasswdRecoveryMailBody(string userId)
        {
            string first = File.ReadAllText("MailTemplate/EmailTemplateFirst.html");
            string last = File.ReadAllText("MailTemplate/EmailTemplateLast.html");


            string bodyReturn = first + "<h5 style=\"font-size:18px;border-radius:12px;color:white;background-color: #FD5757;padding:60px;margin:0px;margin-top:-4px;\">Votre lien de récupération de mot de passe WaterMelon</h5></div><div class=\"card-body\"><br><br><img src=\"watermelon_logo.jpg\" height=\"160px\"/><br><br><p>";
            bodyReturn += "Vous avez demandé un lien de récupération de votre mot de passe</p><h3>";
            bodyReturn += "</h3><p>Clickez sur le bouton ci-dessous pour restorer votre mot de passe !<br></p><a href =\"";
            bodyReturn += c_webAppUrl + "reset/" + userId + "\" " + last;
            return (bodyReturn);
        }
        public string CreatePasswdRecoveryMailSubject()
        {
            return ("WaterMelon : Récupération de mot de passe");
        }
        public string CreateInvitationMailSubject(string eventCreator, string eventName)
        {
            return ("WaterMelon : " + eventCreator + " t'as invité à " + eventName);
        }
        public string CreateModifyMailSubject(string eventName)
        {
            return ("WaterMelon : L'évnément " + eventName + "à été modifié !");
        }


        public void Send(string to, string subjects, string html)
        {
            /*onst string tmp = "";
            var fromAddress = new MailAddress(_emailSettings.FontColor, "WaterMelon");
            var toAddress = new MailAddress(to, to);
            const string fromPassword = tmp;
            const string subject = subjects;
            const string body = "Hey now!!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }*/

            /*MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress(_emailSettings.FontColor);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = html;

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment("MailTemplate/watermelon_logo.jpg");
            mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(_emailSettings.FontColor, _emailSettings.Font2Color);
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);*/


            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.FontColor));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subjects;
            var builder = new BodyBuilder();
            builder.HtmlBody = html;
            var image = builder.LinkedResources.Add("MailTemplate/watermelon_logo.jpg");
            image.ContentId = MimeUtils.GenerateMessageId();

            email.Body = builder.ToMessageBody();
            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Authenticate(_emailSettings.FontColor, _emailSettings.Font2Color);
            smtp.Send(email);
            smtp.Disconnect(true);

        }
    }
}