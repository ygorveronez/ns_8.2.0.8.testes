using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ExtratoValePedagio
{
    public class FiltroPesquisaExtratosValePedagio
	{
		public string CodigoCargaEmbarcador { get; set; }
		public DateTime? DataCargaInicial { get; set; }
		public DateTime? DataCargaFinal { get; set; }
		public long NumeroValePedagio { get; set; }
		public Enumeradores.SituacaoExtratoValePedagio? SituacaoExtrato { get; set; }
	}
}
