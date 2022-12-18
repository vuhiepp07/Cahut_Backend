using Cahut_Backend.Models;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Cahut_Backend
{
    public static class Helper
    {
        //uncomment this when test api intergrate with Front-end, both in localhost and database if offline too
        public static string TestingLink = "http://localhost:3000";

        //uncomment this when test api with post man only
        //public static string TestingLink = "https://localhost:7080";


        //uncomment this when deploy only
        //public static string TestingLink = "https://cahut2.netlify.app/";

        public static string RandomString(int len)
        {
            Random rand = new Random();
            string pattern = "qwertyuiopasdfghjklzxcvbnm1234567890";
            char[] arr = new char[len];
            for (int i = 0; i < len; i++)
            {
                arr[i] = pattern[rand.Next(pattern.Length)];
            }
            return string.Join(string.Empty, arr);

        }

        public static string SendEmails(EmailSender sender, EmailMessage obj, IConfiguration configuration)
        {
            IConfiguration section = configuration.GetSection("Mails:Gmail");
            using (SmtpClient client = new SmtpClient(section.GetValue<string>("Host"), section.GetValue<int>("Port"))
            {
                Credentials = new NetworkCredential(sender.usr, sender.pwd),
                EnableSsl = true
            })
            {
                try
                {
                    MailMessage message = new MailMessage(new MailAddress(sender.usr, "Cahut"), new MailAddress(obj.EmailTo))
                    {
                        IsBodyHtml = true,
                        Subject = obj.Subject,
                        Body = obj.Content
                    };
                    client.Send(message);
                    return "Send mail success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public static byte[] Hash(string plaintext)
        {
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA-512");
            return algorithm.ComputeHash(Encoding.ASCII.GetBytes(plaintext));
        }
    }
}
