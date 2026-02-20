using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class HistoricoVinculo
    {
        #region Propriedades 
        public int Codigo { get; set; }

        public string VeiculoTracao { get; set; }
        public string VeiculoReboque { get; set; }
        public string Motorista { get; set; }
        public string Carga { get; set; }
        public string Pedido { get; set; }
        public string FilaCarregamento { get; set; }
        public string Observacao { get; set; }
        public LocalVinculo LocalVinculo { get; set; }
        public DateTime DataVinculo { get; set; }
        public DateTime DataDesvinculo { get; set; }
        #endregion

        #region Propriedades com Regras
        public string DataVinculoFormatada
        {
            get { return FormataData(DataVinculo); }
        }
        public string DataDesvinculoFormatada
        {
            get { return FormataData(DataDesvinculo); }
        }

        public string LocalVinculoFormatada
        {
            get { return LocalVinculo.ObterDescricao(); }
        }
        #endregion

        #region Métdos Privados
        public string FormataData(DateTime dataParametro)
        {
            return dataParametro != DateTime.MinValue ? dataParametro.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
        }
        #endregion

    }
}