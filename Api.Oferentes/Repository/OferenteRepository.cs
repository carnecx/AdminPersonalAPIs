using Dapper;
using Api.Oferentes.Entities;

namespace Api.Oferentes.Repository
{
    public class OferenteRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public OferenteRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        // Listado general de oferentes (HU OFE1)
        public async Task<IEnumerable<Oferente>> GetAllAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = @"SELECT id_oferente AS IdOferente, identificacion AS Identificacion,
                                   tipo_identificacion AS TipoIdentificacion, nombre_completo AS NombreCompleto,
                                   fecha_nacimiento AS FechaNacimiento, ruta_curriculum AS RutaCurriculum
                            FROM oferente";
                return await connection.QueryAsync<Oferente>(sql);
            }
        }

        // Core8: detalle completo de un oferente (datos + correos + teléfonos)
        public async Task<OferenteDetalle?> GetDetalleByIdentificacionAsync(string identificacion)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var oferente = await connection.QueryFirstOrDefaultAsync<Oferente>(
                    @"SELECT id_oferente AS IdOferente, identificacion AS Identificacion,
                             tipo_identificacion AS TipoIdentificacion, nombre_completo AS NombreCompleto,
                             fecha_nacimiento AS FechaNacimiento, ruta_curriculum AS RutaCurriculum
                      FROM oferente WHERE identificacion = @Identificacion",
                    new { Identificacion = identificacion });

                if (oferente is null) return null;

                var correos = await connection.QueryAsync<string>(
                    "SELECT correo FROM oferente_correo WHERE id_oferente = @IdOferente",
                    new { oferente.IdOferente });

                var telefonos = await connection.QueryAsync<string>(
                    "SELECT telefono FROM oferente_telefono WHERE id_oferente = @IdOferente",
                    new { oferente.IdOferente });

                return new OferenteDetalle
                {
                    IdOferente = oferente.IdOferente,
                    Identificacion = oferente.Identificacion,
                    TipoIdentificacion = oferente.TipoIdentificacion,
                    NombreCompleto = oferente.NombreCompleto,
                    FechaNacimiento = oferente.FechaNacimiento,
                    RutaCurriculum = oferente.RutaCurriculum,
                    Correos = correos.ToList(),
                    Telefonos = telefonos.ToList()
                };
            }
        }

        // Core2: oferentes inscritos en concursos de un puesto (según código de puesto)
        // NOTA: la BD no tiene tabla de "requisitos cumplidos por oferente", así que esto
        // devuelve los oferentes que participan en algún concurso ligado a ese puesto.
        public async Task<IEnumerable<OferenteResumen>> GetOferentesPorPuestoAsync(string codigoPuesto)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT DISTINCT o.identificacion AS Identificacion, o.nombre_completo AS NombreCompleto
                    FROM oferente o
                    INNER JOIN oferente_concurso oc ON oc.id_oferente = o.id_oferente
                    INNER JOIN concurso c ON c.id_concurso = oc.id_concurso
                    INNER JOIN puesto p ON p.id_puesto = c.id_puesto
                    WHERE p.codigo = @CodigoPuesto";
                return await connection.QueryAsync<OferenteResumen>(sql, new { CodigoPuesto = codigoPuesto });
            }
        }

        // Aut3: registrar postulación de un oferente a un puesto (vía el concurso vigente de ese puesto)
        public async Task<string> CrearPostulacionAsync(OferentePostulacion datos, string? rutaCurriculum)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    // 1. Busca el concurso vigente para ese puesto
                    var idConcurso = await connection.QueryFirstOrDefaultAsync<int?>(
                        @"SELECT c.id_concurso FROM concurso c
                          INNER JOIN puesto p ON p.id_puesto = c.id_puesto
                          WHERE p.codigo = @CodigoPuesto AND c.estado = 'Vigente'
                          ORDER BY c.fecha_fin DESC LIMIT 1",
                        new { datos.CodigoPuesto }, tx);

                    if (idConcurso is null)
                    {
                        tx.Rollback();
                        return "No hay un concurso vigente para el puesto indicado.";
                    }

                    // 2. Verifica si el oferente ya existe (por identificación)
                    var idOferente = await connection.QueryFirstOrDefaultAsync<int?>(
                        "SELECT id_oferente FROM oferente WHERE identificacion = @Identificacion",
                        new { datos.Identificacion }, tx);

                    if (idOferente is null)
                    {
                        idOferente = await connection.QuerySingleAsync<int>(
                            @"INSERT INTO oferente (identificacion, tipo_identificacion, nombre_completo, fecha_nacimiento, ruta_curriculum)
                              VALUES (@Identificacion, @TipoIdentificacion, @NombreCompleto, @FechaNacimiento, @RutaCurriculum);
                              SELECT LAST_INSERT_ID();",
                            new { datos.Identificacion, datos.TipoIdentificacion, datos.NombreCompleto, datos.FechaNacimiento, RutaCurriculum = rutaCurriculum },
                            tx);

                        foreach (var correo in datos.Correos.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                        {
                            await connection.ExecuteAsync(
                                "INSERT INTO oferente_correo (id_oferente, correo) VALUES (@IdOferente, @Correo)",
                                new { IdOferente = idOferente, Correo = correo }, tx);
                        }

                        foreach (var telefono in datos.Telefonos.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                        {
                            await connection.ExecuteAsync(
                                "INSERT INTO oferente_telefono (id_oferente, telefono) VALUES (@IdOferente, @Telefono)",
                                new { IdOferente = idOferente, Telefono = telefono }, tx);
                        }
                    }

                    // 3. Relaciona el oferente con el concurso (si no está ya inscrito)
                    var yaInscrito = await connection.QueryFirstOrDefaultAsync<int?>(
                        "SELECT 1 FROM oferente_concurso WHERE id_oferente = @IdOferente AND id_concurso = @IdConcurso",
                        new { IdOferente = idOferente, IdConcurso = idConcurso }, tx);

                    if (yaInscrito is null)
                    {
                        await connection.ExecuteAsync(
                            "INSERT INTO oferente_concurso (id_oferente, id_concurso) VALUES (@IdOferente, @IdConcurso)",
                            new { IdOferente = idOferente, IdConcurso = idConcurso }, tx);
                    }

                    tx.Commit();
                    return "OK";
                }
            }
        }
    }
}