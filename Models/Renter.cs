using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


[Table("renter")]
public class Renter {
    [Key]
    public int renter_id { get; set; }
    public int user_id { get; set; }
    public string renter_fname { get; set; }
    public string renter_mname { get; set; }
    public string renter_lname { get; set; }
    public DateOnly renter_birthday { get; set; }
    public string renter_contact_num { get; set; }
    public string renter_email { get; set; }
    public string renter_emergency_contact { get; set; }
    public string renter_address { get; set; }
    public byte[]? renter_id_photo_1 { get; set; } // Change to byte[]
    public string? renter_id_photo_2 { get; set; }

    [JsonIgnore] 
    public User? User { get; set; }


}