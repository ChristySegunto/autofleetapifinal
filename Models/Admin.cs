using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("admin")]
public class Admin
{
    [Key]
    public int admin_id { get; set; }

    public int user_id { get; set; } // Foreign key to User table
    public string admin_fname { get; set; }
    public string admin_mname { get; set; }
    public string admin_lname { get; set; }
    public DateTime? admin_birthday { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }

    // Navigation property to the associated User
    [ForeignKey("user_id")]
    public User User { get; set; }
}