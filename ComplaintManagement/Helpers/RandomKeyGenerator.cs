using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ComplaintManagement.Helpers
{

    public class RandomKeyGeneratorManagement
    {
        public enum PasswordScore
        {
            Blank = 0,
            VeryWeak = 1,
            Weak = 2,
            Medium = 3,
            Strong = 4,
            VeryStrong = 5
        }

        public string GenerateRandomAlphaNumericString(int size)
        {
            Random random = new Random();
            string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public string GenerateRandomNumericString(int size)
        {
            int randomValue = 0;
            try
            {
                Random generator = new Random();

                string dd = "";

                int ss = Convert.ToInt32(DateTime.Now.Second + DateTime.Now.Minute);

                if (ss.ToString().Trim().Length < size)
                {
                    dd = ss + dd.PadRight(size - (ss.ToString().Trim().Length), '0');
                    ss = Convert.ToInt16(dd);
                }

                randomValue = generator.Next(1000, ss);
                //Debug.Print(randomValue)



            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }

            return Convert.ToString(randomValue);

        }

        public PasswordScore CheckStrength(string password)
        {
            int score = 0;

            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 4)
                return PasswordScore.VeryWeak;
            if (password.Length == 5)
                return PasswordScore.Weak;
            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;
            if (Regex.Match(password, @"\d+", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"[a-z]", RegexOptions.ECMAScript).Success &&
              Regex.Match(password, @"[A-Z]", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @".[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript).Success)
                score++;

            return (PasswordScore)score;
        }

    }
}