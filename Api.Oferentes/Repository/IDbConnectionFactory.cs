using System.Data;

namespace Api.Oferentes.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}