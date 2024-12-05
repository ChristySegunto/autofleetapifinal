using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]
public class User
{
    [Key]
    public int user_id { get; set; } // Unique identifier for the user
    public string Email { get; set; } // User's email address
    public string Password { get; set; } // User's password
    public string Role { get; set; } // Role of the user to determine access

    public Renter? Renter { get; set; } // Navigation property to the associated Renter

}