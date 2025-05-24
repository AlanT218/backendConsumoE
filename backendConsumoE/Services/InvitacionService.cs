using backendConsumoE.Dtos;
using backendConsumoE.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace backendConsumoE.Services
{
    public class InvitacionService
    {
        private readonly InvitacionRepository _repo;
        public InvitacionService(InvitacionRepository repo) => _repo = repo;

        public async Task<int> ObtenerIdUsuarioPorCorreoAsync(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("El correo no puede estar vacío.");

            var usuario = await _repo.ObtenerUsuarioPorCorreoAsync(correo);
            if (usuario == null)
                throw new KeyNotFoundException("No existe ningún usuario con ese correo.");

            return usuario.IdUsuario;
        }

        public async Task<string> EnviarInvitacionAsync(int idInvitador, int idInvitado, int idHogar, int idRol)
        {
            if (idInvitador <= 0) throw new ArgumentException("Invitador inválido.");
            if (idInvitado <= 0) throw new ArgumentException("Invitado inválido.");
            if (idHogar <= 0) throw new ArgumentException("Hogar inválido.");
            if (idRol <= 0) throw new ArgumentException("Rol inválido.");

            // Doble-check en código: si ya existe pendiente corto
            if (await _repo.InvitacionPendienteExisteAsync(idInvitado, idHogar))
                return "Ya existe una invitación pendiente.";

            try
            {
                bool ok = await _repo.EnviarInvitacionAsync(idInvitador, idInvitado, idHogar, idRol);
                return ok
                    ? "Invitación enviada exitosamente."
                    : "No se pudo enviar la invitación.";
            }
            catch (InvalidOperationException ex)
            {
                // Mensaje de RAISERROR: usuario y SP
                return ex.Message;
            }
        }

        public async Task<string> AceptarInvitacionAsync(int idInvitacion)
        {
            if (idInvitacion <= 0)
                throw new ArgumentException("Id de invitación inválido.");

            bool ok = await _repo.AceptarInvitacionAsync(idInvitacion);
            return ok
                ? "Invitación aceptada."
                : "No hay invitación pendiente para aceptar.";
        }

        public async Task<string> RechazarInvitacionAsync(int idInvitacion)
        {
            if (idInvitacion <= 0)
                throw new ArgumentException("Id de invitación inválido.");

            bool ok = await _repo.RechazarInvitacionAsync(idInvitacion);
            return ok
                ? "Invitación rechazada."
                : "No hay invitación pendiente para rechazar.";
        }
        public async Task<IEnumerable<InvitacionPendienteDto>> ListarPendientesPorInvitadoAsync(int idInvitado)
        {
            if (idInvitado <= 0)
                throw new ArgumentException("Id de invitado inválido.");

            return await _repo.ListarPendientesPorInvitadoAsync(idInvitado);
        }

        public async Task ExpirarInvitacionesAsync()
        {
            await _repo.ExpirarInvitacionesAsync();
        }

    }
}