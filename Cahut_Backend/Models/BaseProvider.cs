using Cahut_Backend.Repository;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Cahut_Backend.Models
{
    public class BaseProvider : IDisposable
    {
        IDbConnection connection;
        protected AppDbContext dbContext;
        protected IConfiguration configuration;

        public BaseProvider(IConfiguration configuration, AppDbContext context)
        {
            this.dbContext = context;
            this.configuration = configuration;
        }

        protected IDbConnection Connection
        {
            get
            {
                if (connection is null)
                {
                    connection = new SqlConnection(configuration.GetConnectionString("Cahut"));
                }
                return connection;
            }
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
            }
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
        }
    }
}
