using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ManutencaoPallet
{
	public sealed class FiltroPesquisaManutencaoPallet
	{
		public string Carga { get; set; }

		public int Filial { get; set; }

		public int Transportador { get; set; }

		public long Cliente { get; set; }

		public DateTime DataInicialMovimentacao { get; set; }

		public DateTime DataFinalMovimentacao { get; set; }

		public TipoManutencaoPallet? TipoManutencaoPallet { get; set; }

		public TipoEntradaSaida TipoMovimentacao { get; set; }

	}
}
