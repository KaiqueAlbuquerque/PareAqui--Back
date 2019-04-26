using System;
using System.Collections.Generic;

namespace TCC.Domain.Entities
{
    public class Locacao
    {
        public int LocacaoId { get; set; }

        public DateTime DiaHoraInicio { get; set; }

        public DateTime? DiaHoraFim { get; set; }

        public DateTime? DiaHoraSaida { get; set; }

        public decimal? Multa { get; set; }

        public decimal ValorLocacaoLocador { get; set; }

        public decimal ValorLocacaoLocatario { get; set; }

        public int ModalidadeLocacao { get; set; }
                
        public int VeiculoId { get; set; }

        public bool? Aprovada { get; set; }

        public bool Cancelada { get; set; }

        public bool Desistencia { get; set; }

        public int? QuemCancelou { get; set; }
                
        public string Observacao { get; set; }

        public virtual Veiculo Veiculo { get; set; }

        public int VagaId { get; set; }

        public virtual Vaga Vaga { get; set; }

        public bool Ativo { get; set; }

        public int TaxaLucro { get; set; }

        public string MensagemResumoLocador { get; set; }

        public string MensagemResumoLocatario { get; set; }

        public string MensagemResumoMulta { get; set; }

        public virtual IEnumerable<Pagamento> Pagamento { get; set; }

        public virtual IEnumerable<Transferencia> Transferencia { get; set; }
    }
}