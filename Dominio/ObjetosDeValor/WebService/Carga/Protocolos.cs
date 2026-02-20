namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class Protocolos
    {
        public int protocoloIntegracaoCarga { get; set; }

        public int protocoloIntegracaoPedido { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.Container Container { get; set; }

        public string ParametroIdentificacaoCliente { get; set; }
        public string RastreamentoPedido { get; set; }
        public string RastreamentoMonitoramento { get; set; }
        public string NumeroContainerPedido { get; set; }
        public string TaraContainer { get; set; }
        public string LacreContainerUm { get; set; }
        public string LacreContainerDois { get; set; }
        public string LacreContainerTres { get; set; }
        public string IdentificadorRota { get; set; }
        public string protocoloIntegracaoFilial { get; set; }
    }
}
