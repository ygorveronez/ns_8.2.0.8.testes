using Dominio.ObjetosDeValor.Embarcador.Pessoas;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class NotaFiscalEletronica
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string Chave { get; set; }
        public string Protocolo { get; set; }
        public Dominio.Enumeradores.TipoEmissaoNFe TipoEmissao { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataSaida { get; set; }
        public DateTime? DataPrestacaoServico { get; set; }
        public DateTime? DataProcessamento { get; set; }
        public Dominio.Enumeradores.FinalidadeNFe Finalidade { get; set; }
        public Dominio.Enumeradores.IndicadorPresencaNFe IndicadorPresenca { get; set; }
        public Dominio.Enumeradores.IndicadorIntermediadorNFe? IndicadorIntermediador { get; set; }
        public Dominio.Enumeradores.StatusNFe Status { get; set; }
        public decimal BCICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ICMSDesonerado { get; set; }
        public decimal ValorII { get; set; }
        public decimal BCICMSST { get; set; }
        public decimal ValorICMSST { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorSeguro { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorOutrasDespesas { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal ValorTotalNota { get; set; }
        public decimal ValorServicos { get; set; }
        public decimal BCISSQN { get; set; }
        public decimal ValorISSQN { get; set; }
        public decimal BCDeducao { get; set; }
        public decimal ValorOutrasRetencoes { get; set; }
        public decimal ValorDescontoIncondicional { get; set; }
        public decimal ValorDescontoCondicional { get; set; }
        public decimal ValorRetencaoISS { get; set; }
        public decimal BCPIS { get; set; }
        public decimal ValorPIS { get; set; }
        public decimal BCCOFINS { get; set; }
        public decimal ValorCOFINS { get; set; }
        public decimal ValorFCP { get; set; }
        public decimal ValorICMSDestino { get; set; }
        public decimal ValorICMSRemetente { get; set; }
        public decimal ValorFCPICMS { get; set; }
        public decimal ValorFCPICMSST { get; set; }
        public decimal ValorIPIDevolvido { get; set; }
        public decimal BCICMSSTRetido { get; set; }
        public decimal ValorICMSSTRetido { get; set; }
        public string ObservacaoTributaria { get; set; }
        public string ObservacaoNota { get; set; }
        public string TranspQuantidade { get; set; }
        public string TranspEspecie { get; set; }
        public string TranspMarca { get; set; }
        public string TranspVolume { get; set; }
        public decimal TranspPesoBruto { get; set; }
        public decimal TranspPesoLiquido { get; set; }
        public Dominio.Enumeradores.ModalidadeFrete TipoFrete { get; set; }
        public string TranspPlacaVeiculo { get; set; }
        public string TranspUFVeiculo { get; set; }
        public string TranspANTTVeiculo { get; set; }
        public string TranspCNPJCPF { get; set; }
        public string TranspNome { get; set; }
        public string TranspIE { get; set; }
        public string TranspEndereco { get; set; }
        public Dominio.ObjetosDeValor.Localidade TranspMunicipio { get; set; }
        public string TranspUF { get; set; }
        public string TranspEmail { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Cliente { get; set; }
        public int Serie { get; set; }
        public NaturezaDaOperacao NaturezaDaOperacao { get; set; }
        public Atividade Atividade { get; set; }
        public ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadePrestacaoServico { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Transportadora { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Empresa { get; set; }
        public List<NotaFiscalProduto> ItensNFe { get; set; }
        public List<NotaFiscalReferencia> ReferenciaNFe { get; set; }
        public List<NotaFiscalParcela> ParcelasNFe { get; set; }
        public int CodigoEmpresa { get; set; }
        public string UFEmbarque { get; set; }
        public string LocalEmbarque { get; set; }
        public string LocalDespacho { get; set; }
        public string InformacaoCompraNotaEmpenho { get; set; }
        public string InformacaoCompraPedido { get; set; }
        public string InformacaoCompraContrato { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Intermediador { get; set; }

        public bool UtilizarEnderecoRetirada { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ClienteRetirada { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadeRetirada{ get; set; }
        public string RetiradaLogradouro { get; set; }
        public string RetiradaNumeroLogradouro { get; set; }
        public string RetiradaComplementoLogradouro { get; set; }
        public string RetiradaBairro { get; set; }
        public string RetiradaCEP { get; set; }
        public string RetiradaTelefone { get; set; }
        public string RetiradaEmail { get; set; }
        public string RetiradaIE { get; set; }

        public bool UtilizarEnderecoEntrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ClienteEntrega { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadeEntrega { get; set; }
        public string EntregaLogradouro { get; set; }
        public string EntregaNumeroLogradouro { get; set; }
        public string EntregaComplementoLogradouro { get; set; }
        public string EntregaBairro { get; set; }
        public string EntregaCEP { get; set; }
        public string EntregaTelefone { get; set; }
        public string EntregaEmail { get; set; }
        public string EntregaIE { get; set; }
    }
}
