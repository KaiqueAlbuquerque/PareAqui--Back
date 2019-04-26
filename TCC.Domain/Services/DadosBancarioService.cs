using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class DadosBancarioService : ServiceBase<DadosBancario>, IDadosBancarioService
    {
        private readonly IDadosBancarioRepository _dadosBancarioRepository;

        public DadosBancarioService(IDadosBancarioRepository dadosBancarioRepository)
            : base(dadosBancarioRepository)
        {
            _dadosBancarioRepository = dadosBancarioRepository;
        }

        public void DisableAccount(int idDadosBancario)
        {
            _dadosBancarioRepository.DisableAccount(idDadosBancario);
        }

        public List<DadosBancario> GetByIdUser(int idUsuario)
        {
            return _dadosBancarioRepository.GetByIdUser(idUsuario);
        }
    }
}
