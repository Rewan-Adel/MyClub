using MyClubLib.Models;
using System.Data.Entity.ModelConfiguration;

public class PersonConfiguration : EntityTypeConfiguration<Person>
{
    public PersonConfiguration()
    {
        // Configure primary key
        HasKey(p => p.PersonId);

        // Configure required properties
        Property(p => p.PersonName).IsRequired().HasMaxLength(100);
        Property(p => p.Gender).IsRequired().HasMaxLength(10);
        Property(p => p.BirthDate).IsRequired();
        Property(p => p.MobileNumber).IsRequired().HasMaxLength(15);
        Property(p => p.HomePhoneNumber).HasMaxLength(15);
        Property(p => p.Email).IsRequired().HasMaxLength(100);
        Property(p => p.Address).HasMaxLength(200);
        Property(p => p.Nationality).HasMaxLength(50);

        // Configure foreign key relationship
        HasOptional(p => p.Member)
            .WithRequired(m => m.Person)
            .WillCascadeOnDelete(false);
    }
}
