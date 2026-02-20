namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public sealed class AlteracaoPedido
    {
        public string Companhia { get; set; }

        public string DataPrevisaoEntrega { get; set; }

        public Embarcador.Pessoas.Pessoa Destinatario { get; set; }

        public string ETA { get; set; }

        public string Navio { get; set; }

        public string Ordem { get; set; }

        public decimal PesoBrutoTotal { get; set; }

        public string PortoChegada { get; set; }
        
        public string PortoSaida { get; set; }

        public int ProtocoloIntegracaoPedido { get; set; }

        public Embarcador.Pessoas.Pessoa Recebedor { get; set; }

        public Embarcador.Pessoas.Pessoa Remetente { get; set; }

        public string Reserva { get; set; }
        
        public string Resumo { get; set; }
        
        public string Temperatura { get; set; }
        
        public string TipoEmbarque { get; set; }

        public string Vendedor { get; set; }
    }
}
