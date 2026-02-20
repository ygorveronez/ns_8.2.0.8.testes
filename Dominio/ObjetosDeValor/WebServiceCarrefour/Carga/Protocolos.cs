namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Carga
{
    public sealed class Protocolos
    {
        public int protocoloIntegracaoCarga { get; set; }

        public int protocoloIntegracaoPedido { get; set; }

        public Pessoas.Pessoa Remetente { get; set; }

        public Pessoas.Pessoa Destinatario { get; set; }

        public string ParametroIdentificacaoCliente { get; set; }
    }
}
