namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class ONUPrestacaoServico
    {
        public string Observacao { get; set; }
        public string NumeroONU { get; set; }
        public string ClasseRisco { get; set; }
        public string RiscoSubsiriario { get; set; }
        public string NumeroRisco { get; set; }
        public string GrupoEmbarcado { get; set; }
        public string ProvisoesEspeciais { get; set; }
        public decimal LimiteKG { get; set; }
        public decimal LimiteLT { get; set; }
        public string InstrucaoEmbalagem { get; set; }
        public string ProvisoesEmbalagem { get; set; }
        public string InstrucaoTanque { get; set; }
        public string ProvisoesTanque { get; set; }
        public int CodigoPedido { get; set; }
    }
}
