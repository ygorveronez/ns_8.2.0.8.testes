using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public class HistoricoMotoristaCentro
    {
        #region Propriedades
        public int Codigo { get; set; }
        public int CodigoVinculoMotorista { get; set; }
        public string Motorista { get; set; }
        public int QtdDiasVinculo { get; set; }
        public string Usuario { get; set; }
        public string CentroResultado { get; set; }
        private DateTime DataHoraVinculo { get; set; }
        private DateTime DataHoraCentroResultado { get; set; }

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