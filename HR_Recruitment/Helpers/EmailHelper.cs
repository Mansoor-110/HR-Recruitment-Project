using System.Net;
using System.Net.Mail;

namespace HR_Recruitment.Helpers
{
    public class EmailHelper
    {
        public static void Send(string to, string subject, string body)
        {
            var mail = new MailMessage();

            mail.From = new MailAddress(
                "aliyanahmed833@gmail.com",
                "HR Department | JobFinder Team"
            );

            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            mail.ReplyToList.Add(
                new MailAddress("aliyanahmed833@gmail.com", "HR Recruitment Team")
            );

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    "aliyanahmed833@gmail.com",
                    "ifgo hkhf znkp djab"
                ),
                EnableSsl = true
            };

            smtp.Send(mail);
        }
    }
}
