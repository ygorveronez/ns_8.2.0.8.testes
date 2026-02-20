namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CondicaoLicenca
    {
        E = 1,
        Ou = 2
    }

    public static class CondicaoLicencaHelper
    {
        public static string ObterDescricao(this CondicaoLicenca situacao)
        {
            switch (situacao)
            {
                case CondicaoLicenca.E: return "E";
                case CondicaoLicenca.Ou: return "Ou";
                default: return null;
            }
        }
    }
}