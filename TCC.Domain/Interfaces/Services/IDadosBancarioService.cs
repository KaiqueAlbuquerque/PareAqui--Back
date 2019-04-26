using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Services
{
    public interface IDadosBancarioService : IServiceBase<DadosBancario>
    {
        List<DadosBancario> GetByIdUser(int idUsuario);

        void DisableAccount(int idDadosBancario);
    }
}
