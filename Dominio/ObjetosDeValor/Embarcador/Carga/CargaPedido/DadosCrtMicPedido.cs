namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class DadosCrtMicPedido
    {
        public int CodigoPedido { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumIncotermPedido? Incoterm { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TransitoAduaneiro? TransitoAduaneiro { get; set; }
        public Dominio.Entidades.Cliente NotificacaoCRT { get; set; }
        public string DtaRotaPrazoTransporte { get; set; }
        public Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem TipoEmbalagem { get; set; }
        public string DetalheMercadoria { get; set; }
    }
}