namespace EmissaoCTe.Integracao
{
    public class RetornoConsultaXML
    {
        public int Protocolo { get; set; }

        public string Chave { get; set; }

        public TipoXML TipoXML { get; set; }

        public string XML { get; set; }
    }

    public enum TipoXML
    {
        Autorizacao = 0,
        Cancelamento = 1,
        Encerramento = 2
    }
}