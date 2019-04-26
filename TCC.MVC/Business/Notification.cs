using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace TCC.MVC.Business
{
    public class Notification
    {
        public async System.Threading.Tasks.Task SendMailAsync(string email, string assunto, string mensagem)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("app.pareaqui18@gmail.com", "Pare Aqui");
            var subject = assunto;
            var to = new EmailAddress(email);
            var plainTextContent = "";
            var htmlContent = mensagem;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        public void SendSMS(string celular, string mensagem)
        {
            HttpClient HttpClientInstance = new HttpClient();
            HttpClientInstance.DefaultRequestHeaders.ConnectionClose = false;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://painel.smsalfa.com.br/send?key=9VWPRY3XJKL43J3R&msg=" + mensagem + "&type=9&number=" + celular);

            HttpResponseMessage response = HttpClientInstance.SendAsync(request).Result;
        }
    }
}