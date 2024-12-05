using Microsoft.EntityFrameworkCore;

public class AutoFleetDbContext : DbContext
{
    // Constructor for AutoFleetDbContext, accepts DbContextOptions and passes it to the base class
    public AutoFleetDbContext(DbContextOptions<AutoFleetDbContext> options) : base(options) { }

    // DbSet properties for the various entities (tables) in the database
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Renter> Renters { get; set; }
    public DbSet<RentedVehicle> RentedVehicles { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<CarUpdate> CarUpdates { get; set; }
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<Report> Reports { get; set; }

    // OnModelCreating method to configure model relationships, table names, and column settings
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    //USER
        modelBuilder.Entity<User>()
            .HasKey(u => u.user_id); // Set UserId as the primary key
    
        modelBuilder.Entity<Renter>()
            .HasOne(r => r.User)  // Renter has one User
            .WithOne(u => u.Renter)  // User has one Renter
            .HasForeignKey<Renter>(r => r.user_id);  // Explicitly specify the foreign key in Renter

    //ADMIN
        modelBuilder.Entity<Admin>()
            .ToTable("admin") // Explicitly set the table name for Admin
            .HasKey(a => a.admin_id); // Define admin_id as the primary key for Admin

        modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)  // Admin has one User
            .WithOne()            // One User corresponds to one Admin (assuming one-to-one relationship)
            .HasForeignKey<Admin>(a => a.user_id); // Admin's user_id is the foreign key referencing User's user_id


    //VEHICLE
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.distance_traveled)
            .HasPrecision(18, 2) // 18 digits total, 2 after decimal
            .HasDefaultValue(0m); // Default value of 0 for distance_traveled

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.total_fuel_consumption)
            .HasPrecision(18, 2) // 18 digits total, 2 after decimal
            .HasDefaultValue(0m); // Default value of 0 for distance_traveled


        modelBuilder.Entity<Vehicle>()
            .Property(v => v.total_mileage)
            .HasPrecision(18, 2) // 18 digits total, 2 after decimal
            .HasDefaultValue(0m); // Default value of 0 for distance_traveled

    //RENTER
        modelBuilder.Entity<Renter>()
            .ToTable("renter") // Explicitly set the table name for Renter
            .HasKey(r => r.renter_id); // Define rented_vehicle_id as the primary key for RentedVehicle

      

    // RENTED VEHICLE
    modelBuilder.Entity<RentedVehicle>()
        .ToTable("rented_vehicle") // Explicitly set the table name for RentedVehicle
        .HasKey(rv => rv.rented_vehicle_id); // Define rented_vehicle_id as the primary key for RentedVehicle


    modelBuilder.Entity<RentedVehicle>()
        .HasOne<Vehicle>()
        .WithMany() 
        .HasForeignKey(rv => rv.vehicle_id) // Foreign key linking to Vehicle
        .OnDelete(DeleteBehavior.Restrict);  // Prevent deletion of a vehicle that has been rented

    modelBuilder.Entity<RentedVehicle>()
        .HasOne<Renter>()
        .WithMany() 
        .HasForeignKey(rv => rv.renter_id) // Foreign key linking to Renter
        .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of a renter who has rented a vehicle
                
    //CAR UPDATE
    modelBuilder.Entity<CarUpdate>()
        .ToTable("realtime_carupdate") // Explicitly set the table name for CarUpdate
        .HasKey(cu => cu.carupdate_id); // Define carupdate_id as the primary key for CarUpdate

    // Set the column type for location and speed properties in CarUpdate
    modelBuilder.Entity<CarUpdate>()
        .Property(c => c.location_latitude)
        .HasColumnType("decimal(18,6)"); // Set latitude precision to 18 digits, 6 after decimal

    modelBuilder.Entity<CarUpdate>()
        .Property(c => c.location_longitude)
        .HasColumnType("decimal(18,6)"); // Set longitude precision to 18 digits, 6 after decimal

    modelBuilder.Entity<CarUpdate>()
        .Property(c => c.speed)
        .HasColumnType("decimal(18,6)"); // Set speed precision to 18 digits, 6 after decimal

    modelBuilder.Entity<CarUpdate>()
        .Property(c => c.total_fuel_consumption)
        .HasColumnType("decimal(18,6)"); // Set fuel consumption precision to 18 digits, 6 after decimal

    modelBuilder.Entity<CarUpdate>()
        .Property(c => c.total_distance_travelled)
        .HasColumnType("decimal(18,6)"); // Set total distance traveled precision to 18 digits, 6 after decimal
    

    }


}
