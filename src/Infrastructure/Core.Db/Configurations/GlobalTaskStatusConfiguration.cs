using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class GlobalTaskStatusConfiguration : IEntityTypeConfiguration<GlobalTaskStatusEntity>
{
    public void Configure(EntityTypeBuilder<GlobalTaskStatusEntity> entity)
    {
        entity.HasNoKey();
        entity.ToTable("global_task_status");
        
        entity.Property(e => e.TaskKey)
            .HasColumnType("smallint")
            .HasColumnName("task_key");
        entity.Property(e => e.Status)
            .HasColumnType("smallint")
            .HasColumnName("status");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.User).WithMany()
            .HasForeignKey(d => d.UserId);
    }
} 