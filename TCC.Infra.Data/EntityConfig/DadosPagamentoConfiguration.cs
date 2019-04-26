using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class DadosPagamentoConfiguration : EntityTypeConfiguration<DadosPagamento>
    {
        public DadosPagamentoConfiguration()
        {
            HasKey(p => p.DadosPagamentoId);

            Property(p => p.Ativo)
                .IsRequired();

            Property(p => p.Bandeira)
                .IsRequired();

            Property(p => p.NumeroCartao)
                .IsRequired()
                .HasMaxLength(16);

            Property(p => p.Cvv)
                .IsRequired();

            Property(p => p.NomeNoCartao)
                .IsRequired();

            Property(p => p.Nascimento)
                .IsRequired();

            Property(p => p.Cpf)
                .IsRequired()
                .HasMaxLength(11);

            HasRequired(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId);
        }
    }
}
