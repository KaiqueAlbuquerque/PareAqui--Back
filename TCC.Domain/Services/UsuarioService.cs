using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class UsuarioService : ServiceBase<Usuario>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
            : base(usuarioRepository)//O construtor da classe pai esta esperando o repositório
            //com a linha acima eu to passando o que ele esta esperando. Ele esta esperando alguem do tipo IRepositoryBase
            //e o meu IUsuarioRepository implementa repositoryBase então ele satisfaz a exigencia da classe pai
        {
            _usuarioRepository = usuarioRepository;
        }

        public string ConsultaUsuarioSemRastrear(int idUsuario)
        {
            return _usuarioRepository.ConsultaUsuarioSemRastrear(idUsuario);
        }

        public void DisableUser(int idUsuario)
        {
            _usuarioRepository.DisableUser(idUsuario);
        }

        public Usuario GetByEmail(string email)
        {
            return _usuarioRepository.GetByEmail(email);
        }

        public int GetQtdUsersActive()
        {
            return _usuarioRepository.GetQtdUsersActive();
        }

        public int GetQtdUsersActivePerMonth(int mes, int ano)
        {
            return _usuarioRepository.GetQtdUsersActivePerMonth(mes, ano);
        }

        public string GetTokenPush(int idUsuario)
        {
            return _usuarioRepository.GetTokenPush(idUsuario);
        }
    }
}
