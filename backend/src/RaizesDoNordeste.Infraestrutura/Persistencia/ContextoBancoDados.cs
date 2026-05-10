using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.Dominio.Entidades;

namespace RaizesDoNordeste.Infraestrutura.Persistencia;

public class ContextoBancoDados : DbContext
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Franquia> Franquias { get; set; }

    public ContextoBancoDados(DbContextOptions<ContextoBancoDados> opcoes) : base(opcoes) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entidade =>
        {
            entidade.HasKey(c => c.Id);
            entidade.Property(c => c.Email).IsRequired().HasMaxLength(200);
            entidade.HasIndex(c => c.Email).IsUnique();
            entidade.Property(c => c.Nome).IsRequired().HasMaxLength(150);
            entidade.Property(c => c.SenhaHash).HasMaxLength(255);
        });

        modelBuilder.Entity<Pedido>(entidade =>
        {
            entidade.HasKey(p => p.Id);
            entidade.OwnsMany(p => p.Itens, item =>
            {
                item.WithOwner();
                item.HasKey(i => i.Id);
            });
        });

        modelBuilder.Entity<Produto>(entidade =>
        {
            entidade.HasKey(p => p.Id);
            entidade.Property(p => p.Nome).IsRequired().HasMaxLength(100);
            entidade.Property(p => p.Preco).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Franquia>(entidade =>
        {
            entidade.HasKey(f => f.Id);
            entidade.Property(f => f.Nome).IsRequired().HasMaxLength(150);
        });
    }
}
