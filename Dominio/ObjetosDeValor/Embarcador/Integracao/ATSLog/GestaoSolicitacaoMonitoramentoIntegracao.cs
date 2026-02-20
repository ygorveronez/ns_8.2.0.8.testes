using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog
{
	public class GestaoSolicitacaoMonitoramentoIntegracao
	{
        public string CodigoExterno { get; set; }
        public TipoGestaoSolicitacaoMonitoramentoIntegracao Tipo { get; set; }
        public DateTime DataHoraPrevisaoInicioViagem { get; set; }
		public DateTime DataHoraPrevisaoFimViagem { get; set; }
        public int VinculoCondutor { get; set; }
		public int VinculoVeiculoTracao { get; set; }
		public string CodigoExternoOperacao { get; set; }

		public Cliente Cliente { get; set; } = new Cliente();
        public Condutor Condutor { get; set; } = new Condutor();
		public VeiculoTracao VeiculoTracao { get; set; } = new VeiculoTracao();
		public List<PontosControle> PontosControle { get; set; } = new List<PontosControle>();
    }
	public class Cliente
	{
		public string Nome { get; set; }
		public string CodigoExterno { get; set; }
		public bool Condutor { get; set; }
		public string Cidade { get; set; }
		public int UF { get; set; }
	}
	public class Condutor
	{
		public string Nome { get; set;}
		public string CPF_CNPJ { get; set; }
		public string CodigoExterno { get; set; }
		public string Cidade { get; set;}
		public int UF { get; set; }
		public Empresa Empresa { get; set; } = new Empresa();

        [JsonProperty(PropertyName = "Condutor", Required = Required.Default)]
        public bool condutor { get; set; }
    }
	public class Empresa
	{
		public string Nome { get; set; }
		public string Cidade { get; set; }
		public string CodigoExterno { get; set; }
		public bool Condutor { get; set; }
		public int UF { get; set; }
	}

	public class VeiculoTracao
	{
		public string Placa { get; set;}
		public Proprietario Proprietario { get; set; } = new Proprietario();
		public TipoVeiculoTracao Tipo { get; set; } = new TipoVeiculoTracao();
	}
	public class Proprietario
	{
		public string Nome { get; set; }
		public string CodigoExterno { get; set; }
		public bool Condutor { get; set; }
		public string Cidade { get; set;}
		public int UF { get; set; }
	}
	public class TipoVeiculoTracao
	{
		public string Nome { get; set;}
		public string Sigla { get; set; }
		public bool Tracao { get; set; }
	}

	public class PontosControle
	{
		public PontoControle PontoControle { get; set; } = new PontoControle();
        public TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle Tipo { get; set; }
        public DateTime DataHoraPrevisaoInicio { get; set; }
        public DateTime DataHoraPrevisaoFim { get; set; }
		public List<Produtos> Produtos { get; set; } = new List<Produtos>();
    }
	public class PontoControle
    {
        public string Nome { get; set; }
		public Pessoa Pessoa { get; set; } = new Pessoa();
    }
	public class Pessoa
	{
		public string Nome { get; set; }
		public string CodigoExterno { get; set; }
		public bool Condutor { get; set; }
		public string Cidade { get; set; }
		public int UF { get; set; }
    }
    public class Produtos
    {
        public Produto Produto { get; set; } = new Produto();
    }
    public class Produto
	{
		public string Nome { get; set; }
		public string CodigoExterno { get; set; }
    }
}