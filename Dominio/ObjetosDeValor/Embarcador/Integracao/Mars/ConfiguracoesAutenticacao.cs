namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class ConfiguracoesAutenticacao
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string URLAutenticacao { get; set; }

        public ConfiguracoesAutenticacao CarregarConfiguracaoPadrao(Entidades.Embarcador.Configuracoes.IntegracaoMars configuracaoIntegracao)
        {
            ClientID = configuracaoIntegracao.ClientID;
            ClientSecret = configuracaoIntegracao.ClientSecret;
            URLAutenticacao = configuracaoIntegracao.URLAutenticacao;

            return this;
        }

        public ConfiguracoesAutenticacao CarregarConfiguracaoCancelamentosCargas(Entidades.Embarcador.Configuracoes.IntegracaoMars configuracaoIntegracao)
        {
            ClientID = configuracaoIntegracao.ClientIDCancelamentosCargas;
            ClientSecret = configuracaoIntegracao.ClientSecretCancelamentosCargas;
            URLAutenticacao = configuracaoIntegracao.URLAutenticacaoCancelamentosCargas;
            
            return this;
        }
    }
}
