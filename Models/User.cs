using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]
public class User
{
    [Key]
    public int user_id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}