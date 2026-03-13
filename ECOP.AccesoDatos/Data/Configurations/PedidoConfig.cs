using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOP.AccesoDatos.Data.Configurations
{
    public class PedidoConfig : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");

            builder.Property(x => x.NumeroPedido)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.TotalMonto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Observaciones)
                .HasMaxLength(500);

            builder.HasIndex(x => x.NumeroPedido)
                .IsUnique();

            builder.HasOne(x => x.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Estado)
                .WithMany(s => s.Pedidos)
                .HasForeignKey(x => x.EstadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
