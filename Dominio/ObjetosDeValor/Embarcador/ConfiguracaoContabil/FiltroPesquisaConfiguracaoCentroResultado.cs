namespace Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil
{
    public sealed class FiltroPesquisaConfiguracaoCentroResultado
    {
        public int Remetente { get; set; }
        public int Destinatario { get; set; }
        public int Tomador { get; set; }
        public int Transportador { get; set; }
        public int TipoOperacao { get; set; }
        public int TipoOcorrencia { get; set; }
        public int GrupoProduto { get; set; }
        public int RotaFrete { get; set; }
        public bool? Situacao { get; set; }
        public int CentroResultadoContabilizacao { get; set; }
        public int CentroResultadoEscrituracao { get; set; }
        public int CentroResultadoICMS { get; set; }
        public int CentroResultadoPIS { get; set; }
        public int CentroResultadoCOFINS { get; set; }
    }
}
