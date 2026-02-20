namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain
{
    public sealed class ConfiguracaoIntegracao
    {
        public string Senha { get; set; }

        public string Url { get; set; }

        public string Usuario { get; set; }

        //USADAS PARA BUSCA DOS DADOS DO PEDIDO PORTAL DO FORNECEDOR
        public string APIKey { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string UrlConsultaPedido { get; set; }
        public string UrlValidaToken { get; set; }
        public string UrlConsultaUsuario { get; set; }

    }
}
