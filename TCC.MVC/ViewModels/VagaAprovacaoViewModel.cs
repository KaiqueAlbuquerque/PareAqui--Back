using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCC.MVC.ViewModels
{
    public class VagaAprovacaoViewModel
    {
        public VagaViewModel Vaga { get; set; }

        public List<string> Imagens { get; set; }

        public string QtdeVagas { get; set; }
    }
}