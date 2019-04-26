using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TCC.Domain.Entities;

namespace TCC.MVC.ViewModels
{
    public class GaragemLocacaoViewModel
    {
        public Garagem Garagem { get; set; }
        public IEnumerable<VagaLocacaoViewModel> VagaETaxa { get; set; }
    }
}