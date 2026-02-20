namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class OrdemColetaRemessa
	{
		public int Codigo { get; set; }
		public string NumeroCarga { get; set; }
		public string NumeroPedido { get; set; }
		public string Cliente { get; set; }
		public string PesoBruto { get; set; }
		public string UnidadeMedida { get; set; }
		public string ObservacaoExpedicao { get; set; }
		public string CidadeCliente { get; set; }
		public string EstadoCliente { get; set; }
	}
}
