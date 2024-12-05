using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("admin")] // Specifies that this class maps to the "admin" table in the database
public class Admin
{
    [Key] // Specifies that admin_id is the primary key of the "admin" table
    public int admin_id { get; set; }

    public int user_id { get; set; } // Foreign key to the User table (associates an admin with a user)
    public string admin_fname { get; set; } // First name of the admin
    public string admin_mname { get; set; } // Middle name of the admin
    public string admin_lname { get; set; } // Last name of the admin
    public DateTime? admin_birthday { get; set; } // Admin's birthday (nullable in case it's not provided)
    public DateTime created_at { get; set; } // Timestamp for when the admin record was created
    public DateTime updated_at { get; set; } // Timestamp for when the admin record was last updated

    // Navigation property to the associated User
    // This is how the Admin class links to the User class via the user_id foreign key
    [ForeignKey("user_id")]  // Specifies that the "user_id" field in Admin is a foreign key to the User table
    public User User { get; set; } // Navigation property that enables navigation from an Admin to the associated User entity
}