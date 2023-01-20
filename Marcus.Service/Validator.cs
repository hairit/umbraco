namespace Marcus.Service
{
    public class Validator
    {
         public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidName(string name)
        {
            if(String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
            {
                return false;
            }else
            {
                return true;
            }
        }

        public static bool IsValidAge(int age)
        {
            return (age > 0);
        }
    }
}