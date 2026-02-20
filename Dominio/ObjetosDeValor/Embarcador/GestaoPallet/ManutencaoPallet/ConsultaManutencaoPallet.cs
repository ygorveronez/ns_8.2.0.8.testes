using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet;

public sealed class ConsultaManutencaoPallet
{
	#region Propriedades

	public int Codigo { get; set; }

	public int NumeroNotaFiscal { get; set; }

	public string Carga { get; set; }

	public int QuantidadePallet { get; set; }

	public DateTime DataCriacao { get; set; }

	public string Filial { get; set; }

	public string FilialCNPJ { get; set; }

	public TipoManutencaoPallet TipoManutencaoPallet { get; set; }

	public TipoEntradaSaida TipoMovimentacao { get; set; }

	public string Observacao { get; set; }

	#endregion

	#region Propriedades com Regras

	public string TipoManutencaoPalletDescricao
	{
		get { return TipoManutencaoPallet.ObterDescricao(); }
	}

	public string TipoMovimentacaoDescricao
	{
		get { return TipoMovimentacao.ObterDescricao(); }
	}

	public string FilialDescricao
	{
		get
		{
			string descricao = string.Empty;

			if (!string.IsNullOrEmpty(Filial))
				descricao = Filial;

			if (!string.IsNullOrEmpty(FilialCNPJ))
				descricao += $" ({FilialCNPJ.ObterCnpjFormatado()})";

			return descricao;

		}
	}
	#endregion Propriedades com Regras
}