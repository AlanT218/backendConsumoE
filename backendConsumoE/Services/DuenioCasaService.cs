using backendConsumoE.Dtos;
using backendConsumoE.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace backendConsumoE.Services
{
    public class DuenioCasaService
    {
        private readonly DuenioCasaRepository _duenioCasaRepository;

        public DuenioCasaService(DuenioCasaRepository duenioCasaRepository)
        {
            _duenioCasaRepository = duenioCasaRepository;
        }
        public async Task<List<HogarDto>> ObtenerTiposHogar()
        {
            return await _duenioCasaRepository.ObtenerTiposHogar();
        }

        public async Task<bool> RegistrarHogar(HogarDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del hogar no puede estar vacío.");

            if (dto.IdTipo <= 0)
                throw new ArgumentException("Debe especificar un tipo válido de hogar.");

            if (dto.IdUsuario <= 0)
                throw new ArgumentException("Debe especificar un usuario válido.");

            return await _duenioCasaRepository.RegistrarHogar(dto);
        }

        public async Task<List<HogarDto>> ObtenerHogaresPorUsuario(int idUsuario)
        {
            return await _duenioCasaRepository.ObtenerHogaresPorUsuario(idUsuario);
        }

        public async Task<List<SelectListItem>> ObtenerZonasAsync()
        {
            var zonas = await _duenioCasaRepository.ObtenerZonasAsync();
            return zonas.Select(z => new SelectListItem
            {
                Value = z.IdZona.ToString(),
                Text = z.Nombre
            }).ToList();
        }

        public async Task<List<SelectListItem>> ObtenerElectrodomesticosAsync()
        {
            var electrodomesticos = await _duenioCasaRepository.ObtenerElectrodomesticosAsync();
            return electrodomesticos.Select(e => new SelectListItem
            {
                Value = e.IdElectro.ToString(),
                Text = e.Nombre
            }).ToList();
        }

        public async Task AgregarZonaElectroAsync(ZonaElectDto nuevaZonaElect)
        {
            try
            {
                await _duenioCasaRepository.InsertarZonaElectroAsync(nuevaZonaElect);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar el electrodoméstico a la zona: " + ex.Message);
            }
        }

        public async Task<List<ZonaElectroVistaDto>> ObtenerZonaElectPorHogarAsync(int idHogar)
        {
            try
            {
                return await _duenioCasaRepository.ObtenerZonaElectPorHogarAsync(idHogar);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el servicio al obtener electrodomésticos por hogar: " + ex.Message);
            }
        }


        public async Task ActualizarZonaElectroAsync(int idZonaElect, ZonaElectroActualizarDto dto)
        {
            try
            {
                await _duenioCasaRepository.ActualizarZonaElectroAsync(idZonaElect, dto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el servicio al actualizar el electrodoméstico: " + ex.Message);
            }
        }


        // ELIMINAR un electrodoméstico
        public async Task<bool> EliminarZonaElectAsync(int idZonaElect)
        {
            try
            {
                return await _duenioCasaRepository.EliminarZonaElectAsync(idZonaElect);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el electrodoméstico: " + ex.Message);
            }
        }

        public async Task CambiarEstadoElectrodomesticoAsync(CambioEstadoDto dto)
        {
            await _duenioCasaRepository.CambiarEstadoElectrodomesticoAsync(dto);
        }
        public async Task<bool> ObtenerEstadoZonaElectAsync(int idZonaElect)
        {
            return await _duenioCasaRepository.ObtenerEstadoZonaElectAsync(idZonaElect);
        }

        public List<ConsumoReporteDto> ObtenerDatosReporteConsumo(int idHogar)
        {
            return _duenioCasaRepository.ObtenerDatosReporteConsumo(idHogar);
        }
        public async Task<List<RecomendacionDto>> ObtenerTodasRecomendacionesAsync()
        {
            return await _duenioCasaRepository.ObtenerTodasRecomendacionesAsync();
        }
    }
}
