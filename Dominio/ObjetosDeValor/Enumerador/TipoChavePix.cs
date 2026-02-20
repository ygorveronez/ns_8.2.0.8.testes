namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum TipoChavePix
    {
        CPFCNPJ = 1,
        Email = 2,
        Celular = 3,
        Aleatoria = 4
    }

    public static class TipoChavePixHelper
    {
        public static string ObterDescricao(this TipoChavePix tipoChavePix)
        {
            switch (tipoChavePix)
            {
                case TipoChavePix.CPFCNPJ: return "CPF/CNPJ";
                case TipoChavePix.Email: return "E-mail";
                case TipoChavePix.Celular: return "Celular";
                case TipoChavePix.Aleatoria: return "Aleatória";
                default: return string.Empty;
            }
        }
    }
}