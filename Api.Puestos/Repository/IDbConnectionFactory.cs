using System.Data;

namespace Api.Puestos.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}