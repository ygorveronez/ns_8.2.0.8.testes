using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar
{
    public class RetornoObterExtratoCredito
	{
		public long Numero { get; set; }

		public DateTime? DataOperacao { get; set; }

		public DateTime? DataCompra { get; set; }

		public string Acao { get; set; }

		public string Descricao { get; set; }

		public decimal? ValorOperacao { get; set; }

		public long NumeroViagem { get; set; }

		public DateTime? DataInicioVigencia { get; set; }

		public DateTime? DataFimVigencia { get; set; }

		public DateTime? DataPassagem { get; set; }

		public string CNPJCPFTransp { get; set; }

		public string NomeTransp { get; set; }

		public string Tag { get; set; }

		public string Placa { get; set; }

		public string NomeRota { get; set; }

		public string ItemFinanceiro1 { get; set; }

		public string ItemFinanceiro2 { get; set; }

		public string ItemFinanceiro3 { get; set; }

		public decimal? SaldoDia { get; set; }

		public string NomePraca { get; set; }

		public string NomeRodovia { get; set; }

		public string NomeConcessionaria { get; set; }

		public int? Fatura { get; set; }

		public DateTime? DataFatura { get; set; }

		public int TipoVP { get; set; }

		public int Status { get; set; }
	}
}
