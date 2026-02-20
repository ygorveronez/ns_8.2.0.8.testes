using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class ProprietarioIntegrar
	{
		public string CNPJAplicacao { get; set; }
		public string Token { get; set; }
		public string CNPJEmpresa { get; set; }
		public string CnpjCpf { get; set; }
		public string RazaoSocial { get; set; }
		public string NomeFantasia { get; set; }
		public string RG { get; set; }
		public string RGOrgaoExpedidor { get; set; }
		public int RNTRC { get; set; }
		public int IE { get; set; }
		public int TipoContrato { get; set; }
		public string DataNascimento { get; set; }
		public string INSS { get; set; }
		public string Referencia1 { get; set; }
		public string Referencia2 { get; set; }
		public bool HabilitarContratoCiotAgregado { get; set; }
		public List<Contato> Contatos { get; set; }
		public List<Enderecos> Enderecos { get; set; }
	}
}
