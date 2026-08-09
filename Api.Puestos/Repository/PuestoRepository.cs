using Dapper;
using Api.Puestos.Entities;

namespace Api.Puestos.Repository
{
    public class PuestoRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public PuestoRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Puesto>> GetActivosAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "SELECT Codigo, Nombre FROM Puesto";
                return await connection.QueryAsync<Puesto>(sql);
            }
        }
        public async Task<IEnumerable<Puesto>> GetDisponiblesAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "SELECT Codigo, Nombre FROM Puesto";
                return await connection.QueryAsync<Puesto>(sql);
            }
        }
    }
}