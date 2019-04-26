using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class DadosPagamentoRepository : RepositoryBase<DadosPagamento>, IDadosPagamentoRepository
    {
        public List<DadosPagamento> GetByIdUser(int idUsuario)
        {
            var dados = Db.DadosPagamentos.Where(d => d.UsuarioId == idUsuario && d.Ativo == true).ToList();
            return dados;
        }

        public void DisableCard(int idDadosPagamento)
        {
            var dadoPagamento = Db.DadosPagamentos.Where(d => d.DadosPagamentoId == idDadosPagamento).FirstOrDefault();
            dadoPagamento.Ativo = false;
            Db.SaveChanges();
        }
    }
}
