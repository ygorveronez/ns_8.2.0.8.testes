namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class ConfiguracaoIntegracao
    {
        public int ShortCode { get; set; }
        public string Token { get; set; }
        public string URLWebService { get; set; }
        public string EndPoint { get; set; }
        public string EndPointToken { get; set; }
        public enumTipoWS Method { get; set; }

    }
}