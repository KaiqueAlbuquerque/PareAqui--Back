
using System.Data.Entity.ModelConfiguration;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class GaragemConfiguration : EntityTypeConfiguration<Garagem>
    {
        public GaragemConfiguration()
        {
            HasKey(g => g.GaragemId);
            
            Property(g => g.Cep)
                .IsRequired()
                .HasMaxLength(8);

            Property(g => g.Endereco)
                .IsRequired()
                .HasMaxLength(255);

            Property(g => g.Complemento)
                .IsOptional()
                .HasMaxLength(255);

            Property(g => g.NumeroRua)
                .IsRequired();

            Property(g => g.Latitude)
                .IsRequired();

            Property(g => g.Estado)
                .IsRequired();

            Property(g => g.Cidade)
                .IsRequired();

            Property(g => g.Bairro)
                .IsRequired();

            Property(g => g.Longitude)
                .IsRequired();
            
            Property(g => g.Ativo)
                .IsOptional();

            Property(g => g.Condominio)
                .IsRequired();
        }
    }
}
