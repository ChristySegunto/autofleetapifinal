public class RenterCreateModel
{
    public string renter_fname { get; set; }
    public string renter_mname { get; set; }
    public string renter_lname { get; set; }
    public DateOnly renter_birthday { get; set; }
    public string renter_contact_num { get; set; }
    public string renter_email { get; set; }
    public string renter_emergency_contact { get; set; }
    public string renter_address { get; set; }
    public IFormFile renter_id_photo_1 { get; set; } // Use IFormFile for file uploads
}