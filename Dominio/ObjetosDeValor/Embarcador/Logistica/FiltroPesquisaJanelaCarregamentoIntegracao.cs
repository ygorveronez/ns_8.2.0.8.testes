using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaJanelaCarregamentoIntegracao
	{
		public string NumeroCargaEmbarcador { get; set; }
		public DateTime? DataCargaInicial { get; set; }
		public DateTime? DataCargaFinal { get; set; }
		public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }
		public int CodigoFilial { get; set; }
		public int CodigoCentroCarregamento { get; set; }
		public string NumeroViagem { get; set; }
	}
}
