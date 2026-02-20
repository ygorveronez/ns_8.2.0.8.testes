namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.CargaAgrupada
{
    public class CargaDivisaoCapacidadeModeloVeicular
    {
        #region Propriedades

        public int CodigoDivisao { get; set; }
        public string NomeDivisaoModelo { get; set; }
        public decimal QuantidadeDivisaoModelo { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string ZonaTransporte { get; set; }
        public decimal Volumes { get; set; }
        public decimal Peso { get; set; }

        #endregion
    }
}
