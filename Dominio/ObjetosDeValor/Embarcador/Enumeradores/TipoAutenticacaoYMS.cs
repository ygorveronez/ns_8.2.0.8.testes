namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAutenticacaoYMS
    {
        Basic = 1,
        BearerToken = 2
    }

    public static class TipoAutenticacaoYMSHelper
    {
        public static string ObterDescricao(this TipoAutenticacaoYMS tipoAutenticacao)
        {
            switch (tipoAutenticacao)
            {
                case TipoAutenticacaoYMS.Basic: return "Basic";
                case TipoAutenticacaoYMS.BearerToken: return "Bearer Token";
                default: return string.Empty;
            }
        }
    }
}
