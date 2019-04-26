using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCC.MVC.ViewModels
{
    public class DashBoardViewModel
    {
        public int QtdeUsuariosAtivos { get; set; }

        public int QtdeVagasAtivas { get; set; }

        public int QtdeVeiculosAtivos { get; set; }

        public int QteLocacoes { get; set; }

        public int QtdeVagasPendentes { get; set; }

        public int QtdeEventosPendentes { get; set; }

        public string[] ArrMeses { get; set; }

        public int[] ArrValoresUsuarios { get; set; }

        public int[] ArrValoresVagas { get; set; }

        public int[] ArrValoresVeiculos { get; set; }

        public int[] ArrValoresLocacoes { get; set; }
    }
}