using System;
using System.Collections.Generic;

namespace TCC.Domain.Entities
{
    public class Vaga
    {
        public int VagaId { get; set; }
        
        public decimal? PrecoAvulso { get; set; }

        public decimal? AposPrimeiraHora { get; set; }

        public decimal? PrecoMensal { get; set; }

        public decimal? PrecoDiaria { get; set; }
                
        public string Observacao { get; set; }

        public bool Ativo { get; set; }

        public bool? Desativa { get; set; }

        public bool? Aceita { get; set; }

        public string MotivoReprovada { get; set; }

        public bool AceitaSugestaoDePreco { get; set; }

        public bool Avulso { get; set; }

        public bool Mensal { get; set; }

        public bool Diaria { get; set; }

        public bool PortaoAutomatico { get; set; }

        public bool Coberta { get; set; }

        public string NumeroVaga { get; set; }

        public int GaragemId { get; set; }

        public virtual Garagem Garagem { get; set; }

        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }

        public DateTime DataCadastro { get; set; }

        //public virtual ICollection<Evento> Eventos { get; set; }

        public virtual IEnumerable<Locacao> Locacoes { get; set; }
    }
}