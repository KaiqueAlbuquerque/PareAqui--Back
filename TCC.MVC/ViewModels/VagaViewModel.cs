using System.Collections.Generic;

namespace TCC.MVC.ViewModels
{
    public class VagaViewModel
    {
        public string VagaId { get; set; }

        public string PrecoAvulso { get; set; }

        public string AposPrimeiraHora { get; set; }

        public string PrecoMensal { get; set; }

        public string PrecoDiaria { get; set; }

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

        public string GaragemId { get; set; }

        public virtual GaragemViewModel Garagem { get; set; }

        public string UsuarioId { get; set; }

        public virtual UsuarioViewModel Usuario { get; set; }

        //public virtual ICollection<EventoViewModel> Eventos { get; set; }

        //public virtual IEnumerable<LocacaoViewModel> Locacoes { get; set; }
    }
}