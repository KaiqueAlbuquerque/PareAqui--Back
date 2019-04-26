using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class DadosBancarioConfiguration : EntityTypeConfiguration<DadosBancario>
    {
        public DadosBancarioConfiguration()
        {
            HasKey(d => d.DadosBancarioId);

            Property(d => d.Ativo)
                .IsRequired();

            Property(d => d.Cpf)
                .IsRequired()
                .HasMaxLength(11); 

            Property(d => d.NomeBanco)
                .IsRequired();

            Property(d => d.Agencia)
                .IsRequired();

            Property(d => d.NumeroConta)
                .IsRequired();

            Property(d => d.NomeDonoConta)
                .IsRequired();

            HasRequired(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.UsuarioId);
        }
    }
}
