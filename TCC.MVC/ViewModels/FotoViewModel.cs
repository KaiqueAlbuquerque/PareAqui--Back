using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCC.MVC.ViewModels
{
    public class FotoViewModel
    {
        public string FotoId { get; set; }

        public string Imagem { get; set; }

        public string Tipo { get; set; }

        public string VagaId { get; set; }

        public string EventoId { get; set; }
    }
}