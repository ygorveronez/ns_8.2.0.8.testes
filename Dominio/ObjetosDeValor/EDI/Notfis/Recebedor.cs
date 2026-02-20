namespace Dominio.ObjetosDeValor.EDI.Notfis
{
    public class Recebedor
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public string NumeroPedido { get; set; }
        public string AreaFrete { get; set; }
        public string MeioTransporte { get; set; }
        public string Filler { get; set; }

    }
}
