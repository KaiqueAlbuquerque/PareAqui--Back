using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class DadosBancarioRepository : RepositoryBase<DadosBancario>, IDadosBancarioRepository
    {
        public List<DadosBancario> GetByIdUser(int idUsuario)
        {
            var dados = Db.DadosBancarios.Where(d => d.UsuarioId == idUsuario && d.Ativo == true).ToList();
            return dados;
        }

        public void DisableAccount(int idDadosBancario)
        {
            var dadoBancario = Db.DadosBancarios.Where(d => d.DadosBancarioId == idDadosBancario).FirstOrDefault();
            dadoBancario.Ativo = false;
            Db.SaveChanges();
        }
    }
}
