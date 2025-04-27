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

    }
}
