using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Domain.Entities
{
    public class Transferencia
    {
        public int TransferenciaId { get; set; }

        public decimal Valor { get; set; }

        public DateTime? DataEfetivado { get; set; }

        public DateTime DataPagamento { get; set; }

        public int DadosBancarioId { get; set; }

        public bool? Aprovado { get; set; }

        public string CodigoTransferencia { get; set; }

        public virtual DadosBancario DadosBancario { get; set; }

        public int LocacaoId { get; set; }

        public virtual Locacao Locacao { get; set; }
    }
}
