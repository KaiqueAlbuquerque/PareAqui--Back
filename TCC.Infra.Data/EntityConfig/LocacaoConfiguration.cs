using System.Data.Entity.ModelConfiguration;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class LocacaoConfiguration : EntityTypeConfiguration<Locacao>
    {
        public LocacaoConfiguration()
        {
            HasKey(l => l.LocacaoId);

            Property(l => l.DiaHoraInicio)
                 .IsRequired();

            Property(l => l.DiaHoraFim)
                 .IsOptional();

            Property(l => l.Multa)
                 .IsOptional();

            Property(l => l.DiaHoraSaida)
                 .IsOptional(); 

            Property(l => l.ModalidadeLocacao)
                 .IsRequired();
                
            Property(l => l.ValorLocacaoLocador)
                 .IsRequired();

            Property(l => l.ValorLocacaoLocatario)
                 .IsRequired();

            Property(l => l.Aprovada)
                .IsOptional();

            Property(l => l.Ativo)
                .IsRequired();

            Property(l => l.Cancelada)
                .IsRequired();

            Property(l => l.Desistencia)
                .IsRequired();

            Property(l => l.QuemCancelou)
                .IsOptional();

            Property(l => l.TaxaLucro)
                .IsRequired();

            Property(l => l.MensagemResumoLocador)
                .IsOptional();

            Property(l => l.MensagemResumoLocatario)
                .IsOptional();

            Property(l => l.MensagemResumoMulta)
                .IsOptional();

            Property(l => l.Observacao)
                .HasMaxLength(250)
                .IsOptional();
                        
            HasRequired(l => l.Vaga)
                .WithMany()
                .HasForeignKey(l => l.VagaId);

            HasRequired(l => l.Veiculo)
                .WithMany()
                .HasForeignKey(l => l.VeiculoId);

            HasRequired(l => l.Veiculo)
                .WithMany()
                .HasForeignKey(l => l.VeiculoId);
        }
    }
}
