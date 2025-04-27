using backendConsumoE.Dtos;
using backendConsumoE.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backendConsumoE.Repositories
{
    public class DuenioCasaRepository
    {
        private readonly DbContextUtility _dbContextUtility;

        public DuenioCasaRepository(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
        }

        public async Task<List<HogarDto>> ObtenerHogaresPorUsuario(int idUsuario)
        {
            var hogares = new List<HogarDto>();

            const string sql = @"
        SELECT H.id_hogar, H.nombre, H.categoria, UH.id_usuario
        FROM HOGAR H
        INNER JOIN USU_HOGAR UH ON UH.id_hogar = H.id_hogar
        WHERE UH.id_usuario = @idUsuario";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@idUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    hogares.Add(new HogarDto
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Categoria = reader.GetString(2),
                        IdUsuario = reader.GetInt32(3)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los hogares: " + ex.Message);
            }

            return hogares;
        }

        public async Task<List<ZonaDto>> ObtenerZonasAsync()
        {
            var zonas = new List<ZonaDto>();

            const string sql = @"
        SELECT id_zona, nombre
        FROM [bdGestion].[dbo].[ZONA]";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    zonas.Add(new ZonaDto
                    {
                        IdZona = reader.GetInt32(0),
                        Nombre = reader.GetString(1)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las zonas: " + ex.Message);
            }

            return zonas;
        }
        public async Task<List<ElectrodomesticoDto>> ObtenerElectrodomesticosAsync()
        {
            var electrodomesticos = new List<ElectrodomesticoDto>();

            const string sql = @"
        SELECT id_electro, nombre
        FROM [bdGestion].[dbo].[ELECTRODOMESTICO]";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    electrodomesticos.Add(new ElectrodomesticoDto
                    {
                        IdElectro = reader.GetInt32(0),
                        Nombre = reader.GetString(1)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los electrodomésticos: " + ex.Message);
            }

            return electrodomesticos;
        }

        public async Task InsertarZonaElectroAsync(ZonaElectDto nuevaZonaElect)
        {
            const string sql = @"
        INSERT INTO ZONA_ELECT (id_zona, id_electro, id_hogar, consumo, estado)
        VALUES (@IdZona, @IdElectro, @IdHogar, @Consumo, @Estado)";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@IdZona", nuevaZonaElect.IdZona);
                command.Parameters.AddWithValue("@IdElectro", nuevaZonaElect.IdElectro);
                command.Parameters.AddWithValue("@IdHogar", nuevaZonaElect.IdHogar);
                command.Parameters.AddWithValue("@Consumo", nuevaZonaElect.Consumo);
                command.Parameters.AddWithValue("@Estado", nuevaZonaElect.Estado);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar en la tabla ZONA_ELECT: " + ex.Message);
            }
        }


    }
}
