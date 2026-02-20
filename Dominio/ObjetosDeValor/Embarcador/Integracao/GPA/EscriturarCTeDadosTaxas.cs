namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GPA
{
    public class EscriturarCTeDadosTaxas
    {
        public string tipoImposto { get; set; }
        public decimal montanteBasico { get; set; }
        public decimal taxaImposto { get; set; }
        public decimal valorFiscal { get; set; }
        public string impostoRetido { get; set; }
        public decimal percBaseImposto { get; set; }
        public string impVlLiquido { get; set; }
    }
}
