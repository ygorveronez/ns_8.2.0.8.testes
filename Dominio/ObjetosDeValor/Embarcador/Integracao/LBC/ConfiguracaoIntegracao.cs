namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LBC
{
    public sealed class ConfiguracaoIntegracao
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Url { get; set; }

        public bool UtilizarValorPadraoParaCamposNulos { get; set; }
    }
}
