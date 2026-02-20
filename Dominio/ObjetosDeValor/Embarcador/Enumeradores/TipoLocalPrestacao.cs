namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoLocalPrestacao
    {
        todos = 0,
        interMunicipal = 1,
        intraMunicipal = 2
    }

    public static class TipoLocalPrestacaoHelper
    {
        public static string ObterDescricao(this TipoLocalPrestacao tipoLocalPrestacao)
        {
            switch (tipoLocalPrestacao)
            {
                case TipoLocalPrestacao.interMunicipal: return "Intermunicipal";
                case TipoLocalPrestacao.intraMunicipal: return "Municipal";
                default: return string.Empty;
            }
        }
    }
}
