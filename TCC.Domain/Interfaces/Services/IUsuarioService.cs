using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Services
{
    public interface IUsuarioService : IServiceBase<Usuario>
    {
        Usuario GetByEmail(string email);

        string ConsultaUsuarioSemRastrear(int idUsuario);

        void DisableUser(int idUsuario);

        int GetQtdUsersActive();

        int GetQtdUsersActivePerMonth(int mes, int ano);

        string GetTokenPush(int idUsuario);
    }
}
