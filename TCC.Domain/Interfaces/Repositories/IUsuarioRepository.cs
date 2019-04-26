using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepositoryBase<Usuario>
    {
        //Como não tem mais nenhum método especifico, não faço mais nada aqui, pois tudo que tem em IRepositoryBase é o suficiente. Se quiser 
        //um método além do CRUD, fazer aqui
        Usuario GetByEmail(string email);

        string ConsultaUsuarioSemRastrear(int idUsuario);

        void DisableUser(int idUsuario);

        int GetQtdUsersActive();

        int GetQtdUsersActivePerMonth(int mes, int ano);

        string GetTokenPush(int idUsuario);
    }
}
