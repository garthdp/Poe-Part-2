using BCryptNet = BCrypt.Net.BCrypt;
namespace Poe_Part_2.Data
{
    //Class which hashes passwords
    public class EncryptionClass
    {
        // hashs passwords
        public string HashPassword(string password)
        {
            string salt = BCryptNet.GenerateSalt(12);
            string hashPassword = BCryptNet.HashPassword(password, salt);

            return hashPassword;
        }
        // verifies passwords
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCryptNet.Verify(password, hashedPassword);
        }
    }
}
