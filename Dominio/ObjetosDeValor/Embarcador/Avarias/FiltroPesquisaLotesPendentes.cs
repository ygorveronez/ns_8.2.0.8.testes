using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Avarias
{
	public sealed class FiltroPesquisaLotesPendentes
    {
		public int NumeroLote { get; set; }

		public int CodigoTransportador { get; set; }

		public EtapaLote EtapaLote { get; set; }

		public SituacaoLote SituacaoLote { get; set; }

	}
}
