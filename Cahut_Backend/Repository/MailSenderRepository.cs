using Cahut_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cahut_Backend.Repository
{
    public class MailSenderRepository : BaseRepository
    {
        public MailSenderRepository(AppDbContext context) : base(context)
        {
        }
        //string sql = "Update [Order] set Status = 0 where OrderId = @OrderId";
        //return dbContext.Database.ExecuteSqlRaw(sql, new SqlParameter("@OrderId", orderId));

        //string sql = "Select * from Product";
        //return dbContext.Product.FromSqlRaw<Product>(sql).ToList();
        public EmailSender GetMailSender()
        {
            string sql = "Select * from EmailSender";
            EmailSender sender = context.EmailSender.FromSqlRaw<EmailSender>(sql).SingleOrDefault();
            return sender;
        }

        public int increaseMailSent(string account)
        {
            EmailSender sender = context.EmailSender.Where(p => p.usr == account).SingleOrDefault();
            sender.EmailSended += 1;
            return context.SaveChanges();
        }
    }
}
