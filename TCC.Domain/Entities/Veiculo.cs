using System;
using System.Collections.Generic;

namespace TCC.Domain.Entities
{
    public class Veiculo
    {
        public int VeiculoId { get; set; }

        public string Placa { get; set; }

        public bool Ativo { get; set; }

        public int IdMarcaFipe { get; set; }

        public int IdModeloFipe { get; set; }

        public string IdAnoFipe { get; set; }

        public string Cor { get; set; }
                
        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }

        public DateTime DataCadastro { get; set; }

        public virtual IEnumerable<Locacao> Locacoes { get; set; }
    }
}
