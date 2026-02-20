using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto
{
    public class HistoricoMovimentacaoCanhoto
    {
		#region Propriedades
		public int Codigo { get; set; }
		private DateTime DataUpload { get; set; }
		private DateTime DataAprovacao { get; set; }
		private DateTime DataRejeicao { get; set; }
		private DateTime DataReversao { get; set; }
		private DateTime DataRecebimentoFisico { get; set; }
		private DateTime DataConfirmacaoEntrega { get; set; }
		public string Usuario { get; set; }
		public string MotivoRejeicao { get; set; }
		public string NomeEmitente { get; set; }
        public int NumeroCanhoto { get; set; }
        public string SerieCanhoto { get; set; }
        private double CNPJEmitente { get; set; }


        public string DataUploadFormatada
		{
			get { return DataUpload != DateTime.MinValue ? DataUpload.ToString("dd/MM/yyyy") : string.Empty; }
		}
		public string DataAprovacaoFormatada
		{
			get { return DataAprovacao != DateTime.MinValue ? DataAprovacao.ToString("dd/MM/yyyy") : string.Empty; }
		}
		public string DataRejeicaoFormatada
		{
			get { return DataRejeicao != DateTime.MinValue ? DataRejeicao.ToString("dd/MM/yyyy") : string.Empty; }
		}
		public string DataReversaoFormatada
		{
			get { return DataReversao != DateTime.MinValue ? DataReversao.ToString("dd/MM/yyyy") : string.Empty; }
		}
		public string DataRecebimentoFisicoFormatada
		{
			get { return DataRecebimentoFisico != DateTime.MinValue ? DataRecebimentoFisico.ToString("dd/MM/yyyy") : string.Empty; }
		}
		public string DataConfirmacaoEntregaFormatada
		{
			get { return DataConfirmacaoEntrega != DateTime.MinValue ? DataConfirmacaoEntrega.ToString("dd/MM/yyyy") : string.Empty; }
		}

		public string CNPJEmitenteFormatado
        {
            get { return CNPJEmitente.ToString(@"00\.000\.000\/0000\-00"); }
        }

		#endregion
	}
}
