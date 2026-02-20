namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class FilialIntegrar
	{
		public string CNPJAplicacao { get; set; }
		public string Token { get; set; }
		public string CNPJEmpresa { get; set; }
		public string CNPJ { get; set; }
		public string RazaoSocial { get; set; }
		public string NomeFantasia { get; set; }
		public string Sigla { get; set; }
		public string CEP { get; set; }
		public string Endereco { get; set; }
		public int Numero { get; set; }
		public string Bairro { get; set; }
		public string Telefone { get; set; }
		public string Email { get; set; }
		public int? codigoIbgeCidade { get; set; }
		public int? codigoIbgeEstado { get; set; }
		public int? CodigoBACENPais { get; set; }
		public decimal Latitude { get; set; }
		public decimal Longitude { get; set; }
	}
}
