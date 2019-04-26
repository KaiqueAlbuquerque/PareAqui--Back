using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Domain.Entities
{
    public class Pagamento
    {
        public int PagamentoId { get; set; }

        public bool? Aprovado { get; set; }

        public decimal Valor { get; set; }

        public DateTime DataPagamento { get; set; }

        public string CodigoPagamento { get; set; }

        public string DataEfetivado { get; set; }

        public int LocacaoId { get; set; }

        public virtual Locacao Locacao { get; set; }

        public int DadosPagamentoId { get; set; }

        public virtual DadosPagamento DadosPagamento { get; set; }
    }
}
