using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Domain.Entities
{
    public class DadosPagamento
    {
        public int DadosPagamentoId { get; set; }

        public bool Ativo { get; set; }

        public string Bandeira { get; set; }

        public string NumeroCartao { get; set; }

        public string Cvv { get; set; }

        public string NomeNoCartao { get; set; }

        public string Cpf { get; set; }

        public string Nascimento { get; set; }

        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual IEnumerable<Pagamento> Pagamentos { get; set; }
    }
}
