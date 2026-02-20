namespace Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica
{
    public class ParametrosClienteBuscaAutomatica
    {
        public int Codigo { get; set; }

        public double? CPFCNPJRemetente { get; set; }

        public double? CPFCNPJDestinatario { get; set; }

        public int? CodigoOrigem { get; set; }

        public int? CodigoFilial { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.TipoParticipante TipoParticipante { get; set; }

        public double CPFCNPJCliente { get; set; }
    }
}
