using System.Collections.Generic;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Services
{
    public interface IChatService : IServiceBase<Chat>
    {
        List<Usuario> GetUsersChat(int idUsuario);

        bool CheckIfNotRegistered(int usuarioLocadorId, int usuarioLocatarioId);
    }
}
