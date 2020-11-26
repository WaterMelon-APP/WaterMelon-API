﻿using System;
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

namespace WaterMelon_API.Services
{
    public interface IEmailService
    {
        void Send(string from, string to, string subject, string html);
    }

    public class EmailService
    {

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
            this._emailSettings.SmtpPort = 587;
            this._emailSettings.SmtpSecure = SecureSocketOptions.StartTls;
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
            bodyReturn += _eventService.GetFromEventId(eventId) + "\" " + last;
            return (bodyReturn);
        }
        public string CreateModifyMailBody(string eventName, string eventId)
        {
            string first = File.ReadAllText("MailTemplate/EmailTemplateFirst.html");
            string last = File.ReadAllText("MailTemplate/EmailTemplateLast.html");


            string bodyReturn = first + "<h5 style=\"font-size:18px;border-radius:12px;color:white;background-color: #FD5757;padding:60px;margin:0px;margin-top:-4px;\">Modification d'événement !</h5></div><div class=\"card-body\"><br><br><img src=\"watermelon_logo.jpg\" height=\"160px\"/><br><br><p>";
            bodyReturn += "L'événement auquel tu participes</p><h3>" + eventName;
            bodyReturn += "</h3><p>a été modifié !<br></p><a href =\"";
            bodyReturn += _eventService.GetFromEventId(eventId) + "\" " + last;
            return (bodyReturn);
        }

        public string CreateInvitationMailSubject(string eventCreator, string eventName)
        {
            return ("WaterMelon : " + eventCreator + " t'as invité à " + eventName);
        }
        public string CreateModifyMailSubject(string eventName)
        {
            return ("WaterMelon : L'évnément " + eventName + "à été modifié !");
        }


        public void Send(string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.FontColor));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            //email.Body = new TextPart(TextFormat.Html) { Text = html };
            var builder = new BodyBuilder();
            builder.HtmlBody = html;
            var image = builder.LinkedResources.Add(@"H:\WaterMelon-API\WaterMelon-API\MailTemplate\watermelon_logo.jpg");
            image.ContentId = MimeUtils.GenerateMessageId();

            email.Body = builder.ToMessageBody();
            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.FontColor, _emailSettings.Font2Color);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}