namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class PedidoProduto
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public string RazaoRemetente { get; set; }
        public double CNPJCPFRemetente { get; set; }
        public string TipoRemetente { get; set; }
        public string EstadoOrigem { get; set; }
        public string RazaoDestinatario { get; set; }
        public double CNPJCPFDestinatario { get; set; }
        public string TipoDestinatario { get; set; }
        public string EstadoDestino { get; set; }
        public string CNPJCPFRemetenteFormatado
        {
            get
            {
                if (TipoRemetente == null)
                    return "";
                else if (TipoRemetente == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoRemetente == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJCPFRemetente) : string.Format(@"{0:000\.000\.000\-00}", CNPJCPFRemetente);
            }
        }

        public string CNPJCPFDestinatarioFormatado
        {
            get
            {
                if (TipoDestinatario == null)
                    return "";
                else if (TipoDestinatario == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoDestinatario == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJCPFDestinatario) : string.Format(@"{0:000\.000\.000\-00}", CNPJCPFDestinatario);
            }
        }
    }
}
