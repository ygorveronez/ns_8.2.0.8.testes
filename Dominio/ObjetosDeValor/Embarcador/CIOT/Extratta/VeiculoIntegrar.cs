namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class VeiculoIntegrar
	{
		public string CNPJAplicacao { get; set; }
		public string Token { get; set; }
		public string CPFCNPJProprietario { get; set; }
		public string CPFMotorista { get; set; }
		public string CNPJEmpresa { get; set; }
		public string Placa { get; set; }
		public string Chassi { get; set; }
		public int Renavam { get; set; }
		public int AnoFabricacao { get; set; }
		public int AnoModelo { get; set; }
		public string Marca { get; set; }
		public string Modelo { get; set; }
		public bool ComTracao { get; set; }
		public int? IdTipoCarreta { get; set; }
		public int TipoContrato { get; set; }
		public int QuantidadeEixos { get; set; }
		public int? IdTipoCavalo { get; set; }
		public string TecnologiaRastreamento { get; set; }
		public int NumeroFrota { get; set; }
		public int CodigoDaOperacao { get; set; }
		public string Municipio { get; set; }
		public string CNPJFilial { get; set; }
		public int? IdTecnologia { get; set; }
		public int? IBGECidade { get; set; }
	}
}
