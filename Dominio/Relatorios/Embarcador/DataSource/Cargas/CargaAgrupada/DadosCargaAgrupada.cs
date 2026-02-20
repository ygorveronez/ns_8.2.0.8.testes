namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada
{
    public class DadosCargaAgrupada
    {
        #region Propriedades
        public int CodigoCarga { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public decimal Volumes { get; set; }
        public decimal Peso { get; set; }
        public decimal ValorTotal { get; set; }
        public string DataCarregamento { get; set; }
        public string PlacaVeiculo { get; set; }
        public string Transportador { get; set; }

        #endregion
    }
}
