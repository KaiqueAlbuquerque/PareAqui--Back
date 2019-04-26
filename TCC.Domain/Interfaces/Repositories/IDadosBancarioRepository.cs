using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface IDadosBancarioRepository : IRepositoryBase<DadosBancario>
    {
        List<DadosBancario> GetByIdUser(int idUsuario);

        void DisableAccount(int idDadosBancario);
    }
}
