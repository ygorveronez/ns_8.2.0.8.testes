namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CargaJanelaCarregamentoGuaritaDadosPesagem
    {
        public string Pedido { get; set; }
        public decimal PesagemInicial { get; set; }
        public decimal PesagemFinal { get; set; }
        public decimal PorcentagemPerda { get; set; }
        public decimal Pressao { get; set; }
        public string LoteInterno { get; set; }
        public string NumeroLacre { get; set; }
        public int QuantidadeCaixas { get; set; }
        public bool? ProdutorRural { get; set; }
        public int CodigoGuarita { get; set; }
        public Dominio.Entidades.Usuario Usuario { get; set; }
    }
}
