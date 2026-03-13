using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOP.AccesoDatos.Data.Configurations
{
    public class DetallePedidoConfig : IEntityTypeConfiguration<DetallePedido>
    {
        public void Configure(EntityTypeBuilder<DetallePedido> builder)
        {
            builder.ToTable("DetallePedidos");

            builder.Property(x => x.PrecioUnitario)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Ignore(x => x.Subtotal);

            builder.HasIndex(x => new { x.PedidoId, x.ProductoId })
                .IsUnique();

            builder.HasOne(x => x.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(x => x.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Producto)
                .WithMany(p => p.DetallePedidos)
                .HasForeignKey(x => x.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
