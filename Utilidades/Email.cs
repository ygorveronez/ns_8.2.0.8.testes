using System.Collections.Generic;

namespace Utilidades
{
    public class Email
    {
        public static string ObterEmailsValidos(string strEmail, char separadorDe, string separadorPara)
        {
            if (string.IsNullOrWhiteSpace(strEmail))
                return string.Empty;

            var emails = strEmail.Split(separadorDe);
            List<string> emailsValidos = new List<string>();

            foreach (string email in emails)
                if (!string.IsNullOrEmpty(email) && Utilidades.Validate.ValidarEmail(email.Trim()))
                    emailsValidos.Add(email);

            return string.Join(separadorPara, emailsValidos);
        }

        public static bool ValidarEmails(string emails, char separador)
        {
            if (string.IsNullOrWhiteSpace(emails))
                return false;

            var er = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            string[] lEmails = emails.Split(separador);

            foreach (string email in lEmails)
                if (!er.IsMatch(email))
                    return false;

            return true;
        }

        public static bool ValidarEmails(out string emailsInvalidos, string emails, char separador)
        {
            bool valido = true;
            emailsInvalidos = string.Empty;

            if (string.IsNullOrWhiteSpace(emails))
                return false;

            List<string> lEmailsInvalidos = new List<string>();
            var er = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            string[] lEmails = emails.Split(separador);

            foreach (string email in lEmails)
            {
                if (!er.IsMatch(email.Trim()))
                {
                    valido = false;
                    lEmailsInvalidos.Add(email);
                }
            }

            if (lEmailsInvalidos.Count > 0)
                emailsInvalidos = "Os seguintes e-mails são inválidos: " + string.Join("/", lEmailsInvalidos);

            return valido;
        }
    }
}
