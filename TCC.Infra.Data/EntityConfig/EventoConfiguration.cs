using System.Data.Entity.ModelConfiguration;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class EventoConfiguration : EntityTypeConfiguration<Evento>
    {
        public EventoConfiguration()
        {
            HasKey(e => e.EventoId);

            Property(e => e.Cep)
                .IsRequired()
                .HasMaxLength(8);

            Property(e => e.Endereco)
                .IsRequired()
                .HasMaxLength(255);

            Property(e => e.Complemento)
                .IsOptional()
                .HasMaxLength(255);

            Property(e => e.Numero)
                .IsRequired();

            Property(e => e.Bairro)
                .IsRequired();

            Property(e => e.Cidade)
                .IsRequired();

            Property(e => e.Estado)
                .IsRequired();

            Property(e => e.Latitude)
                .IsRequired();

            Property(e => e.Longitude)
                .IsRequired();

            Property(e => e.DataHoraFim)
                .IsRequired();

            Property(e => e.DataHoraInicio)
                .IsRequired();

            Property(e => e.Ativo)
                .IsRequired();

            Property(e => e.NomeEvento)
                .IsRequired();

            Property(e => e.Aprovado)
                .IsOptional();

            Property(e => e.MotivoReprovado)
                .IsOptional();

            Property(e => e.CategoriaEvento)
                .IsRequired();

            Property(e => e.Observacao)
                .IsOptional();
        }
    }
}
