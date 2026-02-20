namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class SimuladorFreteResult
    {
        public int Codigo { get; set; }
        public string Cliente { get; set; }
        public string Transportador { get; set; }
        public string TipoDeCarga { get; set; }
        public bool CargaGerada { get; set; }
        public string Expedicao { get; set; }
        public string NumeroCarregamento { get; set; }
        public decimal GrossSales { get; set; }
        public decimal ValorMinimoCargaCliente { get; set; }
        public string Destino { get; set; }
        public string Estado { get; set; }
        public string Regiao { get; set; }
        public string ExigeIsca { get; set; }
        public string ModeloVeicular { get; set; }
        public string TipoOperacao { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal MetroCubicoTotal { get; set; }
        public decimal QuantidadePalletTotal { get; set; }
        public int VolumesTotal { get; set; }
        public int Quantidade { get; set; }
        public int Ranking { get; set; }
        public string DataCarregamento { get; set; }
        public string ValorTotal { get; set; }
        public string ValorTotalSimulacao { get; set; }
        public string Situacao { get; set; }
        public string Vencedor { get; set; }
        public string Observacao { get; set; }
        public int LeadTime { get; set; }
        public decimal ValorLimiteNaCargaTipoOperacao { get; set; }
        public string Limite { get; set; }
        public bool DT_Enable { get; set; }
        public int DT_RowId { get; set; }
        public string DT_RowColor { get; set; }
    }
}
