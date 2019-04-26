namespace TCC.MVC.ViewModels
{
    public class LocacaoViewModel
    {
        public string LocacaoId { get; set; }

        public string DiaHoraInicio { get; set; }

        public string DiaHoraFim { get; set; }

        public string DiaHoraSaida { get; set; }

        public string Multa { get; set; }

        public string ValorLocacaoLocador { get; set; }

        public string ValorLocacaoLocatario { get; set; }

        public string ModalidadeLocacao { get; set; }
                
        public string VeiculoId { get; set; }

        public virtual VeiculoViewModel Veiculo { get; set; }

        public string VagaId { get; set; }

        public virtual VagaViewModel Vaga { get; set; }

        public bool Ativo { get; set; }

        public bool? Aprovada { get; set; }

        public bool Desistencia { get; set; }

        public string Observacao { get; set; }

        public bool Cancelada { get; set; }

        public string QuemCancelou { get; set; }

        public string MensagemResumoLocador { get; set; }

        public string MensagemResumoLocatario { get; set; }

        public string MensagemResumoMulta { get; set; }

        public bool ColocaBotaoAvaliacao { get; set; }

        public bool ColocaBotaoFinalizar { get; set; }

        public bool ColocaBotaoCancelar { get; set; }

        public bool ColocaBotaoAprovar { get; set; }

        public bool ColocaBotaoReprovar { get; set; }

        public bool ColocaBotaoDesistir { get; set; }

        public string TaxaLucro { get; set; }

        public double NotaDonoVaga { get; set; }

        public double NotaDonoVeiculo { get; set; }
    }
}