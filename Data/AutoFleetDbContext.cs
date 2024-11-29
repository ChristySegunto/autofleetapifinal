using Microsoft.EntityFrameworkCore;

public class AutoFleetDbContext : DbContext
{
    public AutoFleetDbContext(DbContextOptions<AutoFleetDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Renter> Renters { get; set; }
    public DbSet<RentedVehicle> RentedVehicles { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<CarUpdate> CarUpdates { get; set; }
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    //USER
        modelBuilder.Entity<User>()
            .HasKey(u => u.user_id); // Set UserId as the primary key

    //ADMIN
        modelBuilder.Entity<Admin>()
            .ToTable("admin")
            .HasKey(a => a.admin_id);

        modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)  // Admin has one User
            .WithOne()            // One User corresponds to one Admin (assuming one-to-one relationship)
            .HasForeignKey<Admin>(a => a.user_id); // Admin's user_id is the foreign key referencing User's user_id


    //VEHICLE
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.distance_traveled)
            .HasPrecision(18, 2) // 18 digits total, 2 after decimal
            .HasDefaultValue(0m);

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.total_fuel_consumption)
            .HasPrecision(18, 2) // 18 digits total, 2 after decimal
            .HasDefaultValue(0m);


        modelBuilder.Entity<Vehicle>()
            .Property(v => v.total_mileage)
            .HasPrecision(18, 2) // 18 digits total, 2 after decimal
            .HasDefaultValue(0m);

    //RENTER
        modelBuilder.Entity<Renter>()
            .ToTable("renter")
            .HasKey(r => r.renter_id);

        modelBuilder.Entity<Renter>()
            .HasOne(r => r.User)  // renter has one User
            .WithOne()            // One User corresponds to one renter (assuming one-to-one relationship)
            .HasForeignKey<Renter>(r => r.user_id); // renter's user_id is the foreign key referencing User's user_id
        
        modelBuilder.Entity<Renter>()
            .HasMany(r => r.RentedVehicles) // A renter can have many rented vehicles
            .WithOne(rv => rv.Renter)       // A rented vehicle belongs to one renter
            .HasForeignKey(rv => rv.renter_id);

    //RENTED VEHICLE
        modelBuilder.Entity<RentedVehicle>()
            .ToTable("rented_vehicle")
            .HasKey(rv => rv.rented_vehicle_id);

        modelBuilder.Entity<RentedVehicle>()
            .HasOne(rv => rv.Vehicle) // A rented vehicle corresponds to one Vehicle
            .WithMany()               // A Vehicle can be rented multiple times
            .HasForeignKey(rv => rv.vehicle_id);

        modelBuilder.Entity<RentedVehicle>()
            .HasOne(rv => rv.Renter)
            .WithMany(r => r.RentedVehicles)
            .HasForeignKey(rv => rv.renter_id)
            .OnDelete(DeleteBehavior.Restrict);
            
    //CAR UPDATE
        modelBuilder.Entity<CarUpdate>()
            .ToTable("realtime_carupdate")
            .HasKey(cu => cu.carupdate_id);
        
        modelBuilder.Entity<CarUpdate>()
            .HasOne(cu => cu.Renter)
            .WithMany(r => r.CarUpdates)
            .HasForeignKey(cu => cu.renter_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CarUpdate>()
            .Property(c => c.location_latitude)
            .HasColumnType("decimal(18,6)");

        modelBuilder.Entity<CarUpdate>()
            .Property(c => c.location_longitude)
            .HasColumnType("decimal(18,6)");

        modelBuilder.Entity<CarUpdate>()
            .Property(c => c.speed)
            .HasColumnType("decimal(18,6)");

        modelBuilder.Entity<CarUpdate>()
            .Property(c => c.total_fuel_consumption)
            .HasColumnType("decimal(18,6)");

        modelBuilder.Entity<CarUpdate>()
            .Property(c => c.total_distance_travelled)
            .HasColumnType("decimal(18,6)");
        
        

    //MAINTENANCE
        // modelBuilder.Entity<Maintenance>()
        //     .HasOne(m => m.Vehicle)
        //     .WithMany() // If each vehicle can have multiple maintenance records
        //     .HasForeignKey(m => m.vehicle_id); 
    }


}
