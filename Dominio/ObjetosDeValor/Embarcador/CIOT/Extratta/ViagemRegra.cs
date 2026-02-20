namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class ViagemRegra
	{
		public int? IdViagemRegra { get; set; }
		public decimal TaxaAntecipacao { get; set; }
		public decimal ToleranciaPeso { get; set; }
		public decimal TarifaTonelada { get; set; }
		public int TipoQuebraMercadoria { get; set; }
	}
}
