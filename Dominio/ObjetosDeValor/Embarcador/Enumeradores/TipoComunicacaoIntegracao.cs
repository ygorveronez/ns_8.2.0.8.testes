namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoComunicacaoIntegracao
    {
        WebService = 1,
        WebServiceREST = 2,
        WebServiceSOAP = 3,
        DatabaseMSSQL = 4,
        DatabaseMySQL = 5,
        DatabaseOracle = 6,
        DatabasePostgreSQL = 7,
        ActiveMQ = 8,
    }

    public static class TipoComunicacaoIntegracaoHelper
    {
        public static string ObterDescricao(this TipoComunicacaoIntegracao tipo)
        {
            switch (tipo)
            {
                case TipoComunicacaoIntegracao.WebService: return "WebService";
                case TipoComunicacaoIntegracao.WebServiceREST: return "WebService REST";
                case TipoComunicacaoIntegracao.WebServiceSOAP: return "WebService SOAP";
                case TipoComunicacaoIntegracao.DatabaseMSSQL: return "Database MS SQL Server";
                case TipoComunicacaoIntegracao.DatabaseMySQL: return "Database MySQL";
                case TipoComunicacaoIntegracao.DatabaseOracle: return "Database Oracle";
                case TipoComunicacaoIntegracao.DatabasePostgreSQL: return "Database PostgreSQL";
                case TipoComunicacaoIntegracao.ActiveMQ: return "ActiveMQ";
                default: return string.Empty;
            }
        }
    }
}
