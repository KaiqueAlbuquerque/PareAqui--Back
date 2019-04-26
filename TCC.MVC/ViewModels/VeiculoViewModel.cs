using System.Collections.Generic;

namespace TCC.MVC.ViewModels
{
    public class VeiculoViewModel
    {
        public string VeiculoId { get; set; }

        public string Placa { get; set; }

        public bool Ativo { get; set; }

        public string IdModeloFipe { get; set; }

        public string IdMarcaFipe { get; set; }

        public string IdAnoFipe { get; set; }

        public string Cor { get; set; }

        public string UsuarioId { get; set; }

        public virtual UsuarioViewModel Usuario { get; set; }

        //public virtual IEnumerable<LocacaoViewModel> Locacoes { get; set; }
    }
}