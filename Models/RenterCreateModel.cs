public class RenterCreateModel
{
    public string renter_fname { get; set; } // Renter's first name
    public string renter_mname { get; set; } // Renter's middle name
    public string renter_lname { get; set; } // Renter's last name
    public DateOnly renter_birthday { get; set; } // Renter's date of birth (DateOnly stores date without time)
    public string renter_contact_num { get; set; } // Renter's contact number 
    public string renter_email { get; set; } // Renter's email address
    public string renter_emergency_contact { get; set; } // Emergency contact number for the renter
    public string renter_address { get; set; } // Renter's physical address
    public IFormFile renter_id_photo_1 { get; set; } // Use IFormFile for file uploads
}