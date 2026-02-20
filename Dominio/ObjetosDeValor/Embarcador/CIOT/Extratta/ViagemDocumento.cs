namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class ViagemDocumento
	{
		public int? IdViagemDocumento { get; set; }
		public int TipoEvento { get; set; }
		public string Descricao { get; set; }
		public int TipoDocumento { get; set; }
		public int NumeroDocumento { get; set; }
		public bool ObrigaAnexo { get; set; }
	}
}
