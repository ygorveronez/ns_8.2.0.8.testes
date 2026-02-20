using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class ViagemEvento
	{
		public int? IdViagemEvento { get; set; }
		public int TipoEvento { get; set; }
		public decimal ValorPagamento { get; set; }
		public int Status { get; set; }
		public string DataValidade { get; set; }
		public decimal IRRPF { get; set; }
		public decimal INSS { get; set; }
		public decimal SESTSENAT { get; set; }
		public string Instrucao { get; set; }
		public decimal ValorBruto { get; set; }
		public int? IdMotivo { get; set; }
		public bool HabilitarPagamentoCartao { get; set; }
		public string NumeroControle { get; set; }
		public List<ViagemDocumento> ViagemDocumentos { get; set; }
		public List<ViagemOutroDesconto> ViagemOutrosDescontos { get; set; }
		public List<ViagemOutroAcrescimo> ViagemOutrosAcrescimos { get; set; }
		public DadosAbastecimento DadosAbastecimento { get; set; }
	}
}
