namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class ClienteIntegrar
	{
		public string CNPJAplicacao { get; set; }
		public string Token { get; set; }
		public string CNPJEmpresa { get; set; }
		public int? IdCliente { get; set; }
		public int? BACENPais { get; set; }
		public int? IBGEEstado { get; set; }
		public int? IBGECidade { get; set; }
		public string RazaoSocial { get; set; }
		public string NomeFantasia { get; set; }
		public int TipoPessoa { get; set; }
		public string CNPJCPF { get; set; }
		public string RG { get; set; }
		public string OrgaoExpedidorRG { get; set; }
		public int IE { get; set; }
		public int Celular { get; set; }
		public string Email { get; set; }
		public string CEP { get; set; }
		public string Endereco { get; set; }
		public string Complemento { get; set; }
		public int Numero { get; set; }
		public string Bairro { get; set; }
		public decimal Latitude { get; set; }
		public decimal Longitude { get; set; }
		public decimal RaioLocalizacao { get; set; }
		public bool ObrigarCPFReceber { get; set; }
		public bool PermitirAlterarData { get; set; }
		public bool EnviarSMSConfirmacao { get; set; }
		public bool EnviarEmailConfirmacao { get; set; }
		public bool AutenticarCodigoBarraNF { get; set; }
		public int ModeloImpressao { get; set; }
	}
}
