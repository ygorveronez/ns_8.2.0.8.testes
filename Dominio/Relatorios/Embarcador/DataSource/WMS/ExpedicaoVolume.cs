using System;

namespace Dominio.Relatorios.Embarcador.DataSource.WMS
{
    public sealed class ExpedicaoVolume
    {
        #region Propriedades

        public string NumeroNota { get; set; }
        public string SerieNota { get; set; }
        public string CodigoBarras { get; set; }
        public string NumeroSolicitacao { get; set; }
        public decimal Volumes { get; set; }
        public decimal Embarcados { get; set; }
        public decimal Faltantes { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string Carga { get; set; }
        public string Conferente { get; set; }
        private DateTime DataExpedicao { get; set; }
        private DateTime DataEmbarque { get; set; }

        #endregion

        #region  Propriedades com Regras

        public string DataExpedicaoFormatada
        {
            get { return DataExpedicao != DateTime.MinValue ? DataExpedicao.ToString("dd/MM/yyyy") : ""; }
        }
        public string DataEmbarqueFormatada
        {
            get { return DataEmbarque != DateTime.MinValue ? DataEmbarque.ToString("dd/MM/yyyy") : ""; }
        }

        #endregion
    }
}
