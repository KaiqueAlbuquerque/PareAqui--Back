using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    //Se quiser algo mais especifico, por exemplo uma busca, eu coloco esse metodo na interface IUsuarioRepository e implemento ele aqui
    public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
    {
        public string ConsultaUsuarioSemRastrear(int idUsuario)
        {
            var senha = (from u in Db.Usuarios.AsNoTracking() where u.UsuarioId == idUsuario select u).First();
            return senha.Senha;
        }

        public string GetTokenPush(int idUsuario)
        {
            var token = (from u in Db.Usuarios.AsNoTracking() where u.UsuarioId == idUsuario select u).First();
            return token.TokenPush;
        }

        public void DisableUser(int idUsuario)
        {
            var usuario = Db.Usuarios.Where(u => u.UsuarioId == idUsuario).FirstOrDefault();
            usuario.Ativo = false;
            Db.SaveChanges();
        }

        public Usuario GetByEmail(string email)
        {
            var usuario = Db.Usuarios.Where(u => u.Email == email && u.Ativo).FirstOrDefault();
            return usuario;
        }

        public int GetQtdUsersActive()
        {
            return Db.Usuarios.Where(u => u.Ativo == true).Count();
        }

        public int GetQtdUsersActivePerMonth(int mes, int ano)
        {
            return Db.Usuarios.Where(u => u.DataCadastro.Month == mes && u.DataCadastro.Year == ano).Count();
        }
    }
}
