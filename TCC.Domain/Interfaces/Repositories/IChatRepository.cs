using System.Collections.Generic;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface IChatRepository : IRepositoryBase<Chat>
    {
        List<Usuario> GetUsersChat(int idUsuario);

        bool CheckIfNotRegistered(int usuarioLocadorId, int usuarioLocatarioId);
    }
}
