using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCC.MVC.ViewModels
{
    public class PagamentoViewModel
    {
        public string PagamentoId { get; set; }

        public bool? Aprovado { get; set; }

        public string Valor { get; set; }

        public string DataPagamento { get; set; }

        public string CodigoPagamento { get; set; }

        public string DataEfetivado { get; set; }

        public string LocacaoId { get; set; }

        public virtual LocacaoViewModel Locacao { get; set; }

        public string DadosPagamentoId { get; set; }

        public virtual DadosPagamentoViewModel DadosPagamento { get; set; }
    }
}