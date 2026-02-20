using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb
{
    public class envViagem
    {
        public envPessoaViagem Cliente { get; set; }
        public envMotoristaViagem Condutor { get; set; }
        public envVeiculo VeiculoTracao { get; set; }
        public envVeiculo PrimeiraCarreta { get; set; }
        public envVeiculo SegundaCarreta { get; set; }
        public List<envPontoControleViagem> PontosControle { get; set; }
        public string CodigoExterno { get; set; }
        public TipoGestaoSolicitacaoMonitoramentoIntegracao Tipo { get; set; }
        public DateTime? DataHoraPrevisaoInicioViagem { get; set; }
        public DateTime? DataHoraPrevisaoFimViagem { get; set; }
        public decimal? ValorTotalCarga { get; set; }
        public int? VinculoCondutor { get; set; }
        public int? VinculoVeiculoTracao { get; set; }
        public string CodigoExternoOperacao { get; set; }
        public string CodigoRota { get; set; }
    }

    public class envPessoaViagem
    {
        public string Nome { get; set; }
        public string CPF_CNPJ { get; set; }
        public string CodigoExterno { get; set; }
        public bool? Condutor { get; set; }
        public string Cidade { get; set; }
        public int? UF { get; set; }
        public envComplemento Complemento { get; set; }
        public envEndereco Endereco { get; set; }
        public envFisicaComplemento FisicaComplemento { get; set; }
        public envJuridicaComplemento JuridicaComplemento { get; set; }
    }
    public class envMotoristaViagem
    {
        public string Nome { get; set; }
        public string CPF_CNPJ { get; set; }
        public string CodigoExterno { get; set; }
        public bool? Condutor { get; set; }
        public string Cidade { get; set; }
        public int? UF { get; set; }
        public envComplemento Complemento { get; set; }
        public envEndereco Endereco { get; set; }
        public envFisicaComplemento FisicaComplemento { get; set; }
        public envJuridicaComplemento JuridicaComplemento { get; set; }
        public envParticipante Empresa { get; set; }
    }

    public class envPontoControleViagem
    {
        public envPontoControle PontoControle { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle Tipo { get; set; }
        public DateTime? DataHoraPrevisaoInicio { get; set; }
        public DateTime? DataHoraPrevisaoFim { get; set; }
        public List<envProdutoEntrega> Produtos { get; set; }
    }
    public class envProdutoEntrega
    {
        public envProdutoViagem Produto { get; set; }
        public int? Quantidade { get; set; }
        public string NotaFiscal { get; set; }
    }
    public class envProdutoViagem
    {
        public string CodigoExterno { get; set; }
        public decimal? MetragemCubica { get; set; }
        public string Nome { get; set; }
    }
}
