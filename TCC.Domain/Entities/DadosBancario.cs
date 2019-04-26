using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Domain.Entities
{
    public class DadosBancario
    {
        public int DadosBancarioId { get; set; }

        public bool Ativo { get; set; }

        public string Cpf { get; set; }

        public string NomeBanco { get; set; }

        public string Agencia { get; set; }

        public string NumeroConta { get; set; }

        public string NomeDonoConta { get; set; }

        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual IEnumerable<Transferencia> Transferencias { get; set; }
    }
}
