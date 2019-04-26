using System.Data.Entity.ModelConfiguration;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class VagaConfiguration : EntityTypeConfiguration<Vaga>
    {
        public VagaConfiguration()
        {
            HasKey(v => v.VagaId);
                        
            Property(v => v.Observacao)
                .IsOptional()
                .HasMaxLength(250);

            Property(v => v.PrecoAvulso)
                .IsOptional();

            Property(v => v.PrecoMensal)
                .IsOptional();

            Property(v => v.PrecoDiaria)
                .IsOptional();

            Property(v => v.AposPrimeiraHora)
                .IsOptional();

            Property(v => v.Ativo)
                .IsRequired();

            Property(v => v.Desativa)
                .IsOptional();

            Property(v => v.Aceita)
                .IsOptional();

            Property(v => v.MotivoReprovada)
                .IsOptional();

            Property(v => v.AceitaSugestaoDePreco)
                .IsRequired();
                        
            Property(v => v.Avulso)
                .IsRequired();

            Property(v => v.Mensal)
                .IsRequired();

            Property(v => v.Diaria)
                .IsRequired();

            Property(v => v.PortaoAutomatico)
                .IsRequired();

            Property(v => v.Coberta)
                .IsRequired();

            Property(v => v.NumeroVaga)
                .IsOptional();

            HasRequired(v => v.Garagem)
                .WithMany()
                .HasForeignKey(v => v.GaragemId);

            HasRequired(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.UsuarioId);
        }
    }
}
