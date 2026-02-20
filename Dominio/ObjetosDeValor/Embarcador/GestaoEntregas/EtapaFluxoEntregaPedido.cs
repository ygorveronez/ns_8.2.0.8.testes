namespace Dominio.ObjetosDeValor.Embarcador.GestaoEntregas
{
    public class EtapaFluxoEntregaPedido
    {
        public int Codigo { get; set; }

        public string Numero { get; set; }

        public Enumeradores.SituacaoEntregaPedido? Situacao { get; set; }
    }
}
