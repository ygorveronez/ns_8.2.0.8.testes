namespace Dominio.ObjetosDeValor.WebService.Financeiro
{
    public class StageRetornoProvisao
    {
        public string NumeroStage { get; set; }
        public string NumeroFolha { get; set; }
        public string DataFolha { get; set; }
        public string Calculo { get; set; }
        public string Atribuido { get; set; }
        public string Transferido { get; set; }
        public bool Cancelado { get; set; }
        public bool Inconsistente { get; set; }
        public string MensagemRetornoEtapa { get; set; }
    }
}
