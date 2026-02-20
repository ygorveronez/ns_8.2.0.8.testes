namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaJanelaCarregamentoIntegracao
	{
		public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento JanelaCarregamento { get; set; }
		public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem JanelaCarregamentoViagem { get; set; }
		public Enumeradores.TipoRetornoRecebimento? TipoRetornoRecebimento { get; set; }
		public Enumeradores.TipoEventoIntegracaoJanelaCarregamento? TipoEvento { get; set; }
	}
}
