namespace eShop.Ordering.Infrastructure.EntityConfigurations;

class OrderReturnEntityTypeConfiguration
    : IEntityTypeConfiguration<OrderReturn>
{
    public void Configure(EntityTypeBuilder<OrderReturn> orderReturnConfiguration)
    {
        orderReturnConfiguration.ToTable("orderReturns");

        orderReturnConfiguration.Ignore(b => b.DomainEvents);

        orderReturnConfiguration.Property(o => o.Id)
            .UseHiLo("orderreturnseq");

        orderReturnConfiguration.Property<int>("OrderId");

        orderReturnConfiguration
            .Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        orderReturnConfiguration
            .Property(o => o.Reason)
            .HasMaxLength(500);
    }
}
