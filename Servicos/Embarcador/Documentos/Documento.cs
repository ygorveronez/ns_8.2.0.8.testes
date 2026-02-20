namespace Servicos.Embarcador.Documentos
{
    public class Documento : ServicoBase
    {
        public Documento() : base() { }        
        public Documento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public bool ValidarChave(string chaveCompleta)
        {

            int digito = int.Parse(chaveCompleta[43].ToString());
            string chave = chaveCompleta.Remove(43);


            int digitoRetorno;
            int soma = 0;
            int resto = 0;
            int[] peso = { 4, 3, 2, 9, 8, 7, 6, 5 };

            for (int i = 0; i < chave.Length; i++)
            {
                soma += peso[i % 8] * (int.Parse(chave.Substring(i, 1)));
            }

            resto = soma % 11;
            if (resto == 0 || resto == 1)
            {
                digitoRetorno = 0;
            }
            else
            {
                digitoRetorno = 11 - resto;
            }

            if (digito == digitoRetorno)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ExtractJavaScript(string s, string identi, string delimiter)
        {
            string viewStateNameDelimiter = identi;
            string valueDelimiter = delimiter;

            int viewStateNamePosition = s.IndexOf(viewStateNameDelimiter);

            int viewStateStartPosition = viewStateNamePosition +
                                         identi.Length;
            int viewStateEndPosition = s.IndexOf(delimiter, viewStateStartPosition);


            return s.Substring(
                        viewStateStartPosition,
                        viewStateEndPosition - viewStateStartPosition
                     );
        }

        public string ExtractCampo(string s, string identi, string value)
        {
            string viewStateNameDelimiter = identi;
            string valueDelimiter = value + "=\"";

            int viewStateNamePosition = s.IndexOf(viewStateNameDelimiter);
            int viewStateValuePosition = s.IndexOf(
                  valueDelimiter, viewStateNamePosition
               );

            int viewStateStartPosition = viewStateValuePosition +
                                         valueDelimiter.Length;
            int viewStateEndPosition = s.IndexOf("\"", viewStateStartPosition);


            return s.Substring(
                        viewStateStartPosition,
                        viewStateEndPosition - viewStateStartPosition
                     );
        }
    }
}
