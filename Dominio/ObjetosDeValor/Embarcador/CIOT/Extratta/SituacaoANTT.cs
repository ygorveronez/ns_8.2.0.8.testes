using System;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class SituacaoANTT
	{
		public int? IdProprietario { get; set; }
		public bool RNTRCAtivo { get; set; }
		public DateTime DataValidadeRNTRC { get; set; }
		public string TipoTransportador { get; set; }
		public bool EquiparadoTAC { get; set; }
	}
}
