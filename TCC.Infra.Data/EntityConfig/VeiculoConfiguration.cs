using System.Data.Entity.ModelConfiguration;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class VeiculoConfiguration : EntityTypeConfiguration<Veiculo>
    {
        public VeiculoConfiguration()
        {
            HasKey(v => v.VeiculoId);

            Property(v => v.Placa)
                .IsRequired()
                .HasMaxLength(7);

            Property(v => v.IdMarcaFipe)
                .IsRequired();

            Property(v => v.IdModeloFipe)
                .IsRequired();

            Property(v => v.IdAnoFipe)
                .IsRequired();

            Property(v => v.Cor)
                .IsRequired();
            
            Property(v => v.Ativo)
                .IsRequired();
                        
            HasRequired(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.UsuarioId);
        }
    }
}
