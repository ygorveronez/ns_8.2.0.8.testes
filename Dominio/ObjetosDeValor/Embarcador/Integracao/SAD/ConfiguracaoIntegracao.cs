namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAD
{
    public sealed class ConfiguracaoIntegracao
    {
        public string UrlIntegracaoBuscarSenha { get; set; }
        public string UrlIntegracaoFinalizarAgenda { get; set; }
        public string UrlIntegracaoCancelarAgenda { get; set; }
        public string Token { get; set; }
    }
}
