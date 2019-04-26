using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TCC.Domain.Entities;

namespace TCC.MVC.ViewModels
{
    public class VagaLocacaoViewModel
    {
        public Vaga Vaga { get; set; }

        public int TaxaLucro { get; set; }

        public double Media { get; set; }
    }
}