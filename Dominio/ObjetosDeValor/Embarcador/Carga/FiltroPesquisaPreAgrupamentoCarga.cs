namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaPreAgrupamentoCarga
    {
        public int CodigoAgrupamento { get; set; }
        public string CodigoViagem { get; set; }
        public string NumeroNota { get; set; }
        public string Emitente { get; set; }
        public string Expedidor { get; set; }
        public string Recebedor { get; set; }
        public System.DateTime? DataFinal { get; set; }
        public System.DateTime? DataInicial { get; set; }
        public Enumeradores.SituacaoPreAgrupamentoCarga? Situacao { get; set; }
        public System.DateTime? DataPrevisaoEntregaInicial { get; set; }
        public System.DateTime? DataPrevisaoEntregaFinal { get; set; }
    }
}
