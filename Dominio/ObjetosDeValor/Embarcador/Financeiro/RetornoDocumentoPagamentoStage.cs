namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class RetornoDocumentoPagamentoStage
    {
        public int CodigoStage { get; set; }
        public int CodigoDocumento { get; set; }
        public int CodigoProvisao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao SituacaoProvisao { get; set; }
        public bool StageCancelada { get; set; }
        public string NumeroFolha { get; set; }
    }
}
