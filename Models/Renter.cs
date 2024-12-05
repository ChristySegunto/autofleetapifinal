using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


[Table("renter")]
public class Renter {
    [Key] // Specifies this property is the primary key
    public int renter_id { get; set; } 
    public int user_id { get; set; } // Foreign key to the User table (relates Renter to User)
    public string renter_fname { get; set; } // First name of the renter
    public string renter_mname { get; set; } // Middle name of the renter
    public string renter_lname { get; set; } // Last name of the renter
    public DateOnly renter_birthday { get; set; } // Date of birth of the renter
    public string renter_contact_num { get; set; } // Renter's contact number
    public string renter_email { get; set; } // Renter's email address
    public string renter_emergency_contact { get; set; } // Emergency contact details of the renter
    public string renter_address { get; set; } // Address of the renter
    public byte[]? renter_id_photo_1 { get; set; } // Renter's ID photo (byte array for storage)
    public string? renter_id_photo_2 { get; set; } // Additional ID photo

    [JsonIgnore] // Prevents circular references during serialization
    public User? User { get; set; } // Navigation property to the associated User


}