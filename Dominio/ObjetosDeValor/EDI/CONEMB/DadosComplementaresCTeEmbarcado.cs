namespace Dominio.ObjetosDeValor.EDI.CONEMB
{
    public class DadosComplementaresCTeEmbarcado
    {
        public string TipoMeioTransporte { get; set; }
        public decimal ValorTotalDespesasExtras { get; set; }
        public decimal ValorTotalISS { get; set; }
        public string FilialEmissora { get; set; }
        public int Serie { get; set; }
        public int Numero { get; set; }
        public string CodigoLoja { get; set; }
        public string NumeroViagem { get; set; }
        public string DocumentoAutorizacaoCarregamento { get; set; }
        public string EspecieUnitizacao { get; set; }
        public string CodigoRota { get; set; }
        public int KMRodado { get; set; }
        public string Filler { get; set; }
    }
}
