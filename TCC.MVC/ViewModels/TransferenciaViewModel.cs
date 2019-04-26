using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCC.MVC.ViewModels
{
    public class TransferenciaViewModel
    {
        public string TransferenciaId { get; set; }

        public string Valor { get; set; }

        public string DataEfetivado { get; set; }

        public string DataPagamento { get; set; }

        public string DadosBancarioId { get; set; }

        public bool? Aprovado { get; set; }

        public string CodigoTransferencia { get; set; }

        public virtual DadosBancarioViewModel DadosBancario { get; set; }

        public string LocacaoId { get; set; }

        public virtual LocacaoViewModel Locacao { get; set; }
    }
}