using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public class HistoricoVeiculoVinculo
    {
        #region Propriedades
        public int Codigo { get; set; }
        public int CodigoVinculoVeiculo { get; set; }
        public string Veiculo { get; set; }
        public string NumeroFrota { get; set; }
        public string Marca { get; set; }
        public string TipoPropriedade { get; set; }
        public string Reboques { get; set; }
        public string Equipamentos { get; set; }
        public string Motorista { get; set; }
        public int QtdDiasVinculo { get; set; }
        public int KMRodadoVinculo { get; set; }
        public int KMVeiculoRealizarVinculo { get; set; }
        public string Usuario { get; set; }
        public string CentroResultado { get; set; }
        public DateTime DataHoraVinculo { get; set; }
        public DateTime DataHoraCentroResultado { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataHoraVinculoFormatada
        {
            get { return DataHoraVinculo != DateTime.MinValue ? DataHoraVinculo.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataHoraCentroResultadoFormatada
        {
            get { return DataHoraCentroResultado != DateTime.MinValue ? DataHoraCentroResultado.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion
    }
}