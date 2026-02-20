using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    [XmlRoot(ElementName = "nfse", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class TemplateDocumento
    {
        [XmlElement(ElementName = "identificacao")]
        public Identificacao Identificacao { get; set; }

        [XmlElement(ElementName = "emitente")]
        public Emitente Emitente { get; set; }

        [XmlElement(ElementName = "destinatario")]
        public Destinatario Destinatario { get; set; }

        [XmlElement(ElementName = "detalhe")]
        public List<DetalheTemplate> Detalhes { get; set; }

        [XmlElement(ElementName = "total")]
        public Total Total { get; set; }

        [XmlElement(ElementName = "faturas", IsNullable = true)]
        public Faturas Faturas { get; set; }

        [XmlElement(ElementName = "informacao_adicional", IsNullable = true)]
        public InformacaoAdicional InformacaoAdicional { get; set; }

        [XmlElement(ElementName = "eventos", IsNullable = true)]
        public Eventos Eventos { get; set; }
        
    }
    public class Identificacao
    {
        [XmlElement(ElementName = "chave")]
        public string Chave { get; set; }

        [XmlElement(ElementName = "cod_modelo")]
        public string CodModelo { get; set; }

        [XmlElement(ElementName = "num_nota")]
        public string NumNota { get; set; }

        [XmlElement(ElementName = "serie")]
        public string Serie { get; set; }

        [XmlElement(ElementName = "subserie", IsNullable = true)]
        public string SubSerie { get; set; }

        [XmlElement(ElementName = "data_emissao")]
        public DateTime DataEmissao { get; set; }

        [XmlElement(ElementName = "ambiente")]
        public int Ambiente { get; set; }
    }

    public class Emitente
    {
        [XmlElement(ElementName = "cnpj", IsNullable = true)]
        public string Cnpj { get; set; }

        [XmlElement(ElementName = "cpf", IsNullable = true)]
        public string Cpf { get; set; }

        [XmlElement(ElementName = "razao_social")]
        public string RazaoSocial { get; set; }

        [XmlElement(ElementName = "endereco")]
        public string Endereco { get; set; }

        [XmlElement(ElementName = "end_numero")]
        public string EndNumero { get; set; }

        [XmlElement(ElementName = "bairro")]
        public string Bairro { get; set; }

        [XmlElement(ElementName = "municipio")]
        public string Municipio { get; set; }

        [XmlElement(ElementName = "municipio_cod_ibge", IsNullable = true)]
        public string MunicipioCodIBGE { get; set; }

        [XmlElement(ElementName = "uf")]
        public string Uf { get; set; }

        [XmlElement(ElementName = "cep")]
        public string Cep { get; set; }

        [XmlElement(ElementName = "telefone", IsNullable = true)]
        public ulong? Telefone { get; set; }

        [XmlElement(ElementName = "inscricao_estadual")]
        public string InscricaoEstadual { get; set; }
    }
    public class Destinatario
    {
        [XmlElement(ElementName = "cnpj", IsNullable = true)]
        public string Cnpj { get; set; }

        [XmlElement(ElementName = "cpf", IsNullable = true)]
        public string Cpf { get; set; }

        [XmlElement(ElementName = "razao_social")]
        public string RazaoSocial { get; set; }

        [XmlElement(ElementName = "endereco")]
        public string Endereco { get; set; }

        [XmlElement(ElementName = "end_numero")]
        public string EndNumero { get; set; }

        [XmlElement(ElementName = "bairro")]
        public string Bairro { get; set; }

        [XmlElement(ElementName = "municipio")]
        public string Municipio { get; set; }

        [XmlElement(ElementName = "municipio_cod_ibge", IsNullable = true)]
        public string MunicipioCodIBGE { get; set; }

        [XmlElement(ElementName = "uf")]
        public string Uf { get; set; }

        [XmlElement(ElementName = "cep")]
        public string Cep { get; set; }

        [XmlElement(ElementName = "telefone", IsNullable = true)]
        public ulong? Telefone { get; set; }

        [XmlElement(ElementName = "inscricao_estadual")]
        public string InscricaoEstadual { get; set; }
    }

    [XmlRoot(ElementName = "Detalhe")]
    public class DetalheTemplate
    {
        [XmlElement(ElementName = "produto")]
        public List<Produto> Produtos { get; set; }
    }

    public class Produto
    {
        [XmlElement(ElementName = "num_item")]
        public int NumItem { get; set; }

        [XmlElement(ElementName = "cfop")]
        public int Cfop { get; set; }

        [XmlElement(ElementName = "cod_produto")]
        public string CodProduto { get; set; }

        [XmlElement(ElementName = "descricao")]
        public string Descricao { get; set; }

        [XmlElement(ElementName = "unidade")]
        public string Unidade { get; set; }

        [XmlElement(ElementName = "quantidade")]
        public decimal Quantidade { get; set; }

        [XmlElement(ElementName = "valor_unitario")]
        public decimal ValorUnitario { get; set; }

        [XmlElement(ElementName = "valor")]
        public decimal Valor { get; set; }

        [XmlElement(ElementName = "impostos", IsNullable = true)]
        public Impostos Impostos { get; set; }
    }

    public class Impostos
    {
        [XmlElement(ElementName = "iss_aliquota", IsNullable = true)]
        public decimal? IssAliquota { get; set; }

        [XmlElement(ElementName = "iss_retido", IsNullable = true)]
        public int? IssRetido { get; set; }

        [XmlElement(ElementName = "iss_base_calc", IsNullable = true)]
        public decimal? IssBaseCalc { get; set; }

        [XmlElement(ElementName = "iss_valor", IsNullable = true)]
        public decimal? IssValor { get; set; }

        [XmlElement(ElementName = "pis_aliquota", IsNullable = true)]
        public decimal? PisAliquota { get; set; }

        [XmlElement(ElementName = "pis_retido", IsNullable = true)]
        public int? PisRetido { get; set; }

        [XmlElement(ElementName = "pis_base_calc", IsNullable = true)]
        public decimal? PisBaseCalc { get; set; }

        [XmlElement(ElementName = "pis_valor", IsNullable = true)]
        public decimal? PisValor { get; set; }

        [XmlElement(ElementName = "cofins_aliquota", IsNullable = true)]
        public decimal? CofinsAliquota { get; set; }

        [XmlElement(ElementName = "cofins_retido", IsNullable = true)]
        public int? CofinsRetido { get; set; }

        [XmlElement(ElementName = "cofins_base_calc", IsNullable = true)]
        public decimal? CofinsBaseCalc { get; set; }

        [XmlElement(ElementName = "cofins_valor", IsNullable = true)]
        public decimal? CofinsValor { get; set; }

        [XmlElement(ElementName = "inss_aliquota", IsNullable = true)]
        public decimal? InssAliquota { get; set; }

        [XmlElement(ElementName = "inss_retido", IsNullable = true)]
        public int? InssRetido { get; set; }

        [XmlElement(ElementName = "inss_base_calc", IsNullable = true)]
        public decimal? InssBaseCalc { get; set; }

        [XmlElement(ElementName = "inss_valor", IsNullable = true)]
        public decimal? InssValor { get; set; }

        [XmlElement(ElementName = "ir_aliquota", IsNullable = true)]
        public decimal? IrAliquota { get; set; }

        [XmlElement(ElementName = "ir_retido", IsNullable = true)]
        public int? IrRetido { get; set; }

        [XmlElement(ElementName = "ir_base_calc", IsNullable = true)]
        public decimal? IrBaseCalc { get; set; }

        [XmlElement(ElementName = "ir_valor", IsNullable = true)]
        public decimal? IrValor { get; set; }

        [XmlElement(ElementName = "csll_aliquota", IsNullable = true)]
        public decimal? CsllAliquota { get; set; }

        [XmlElement(ElementName = "csll_retido", IsNullable = true)]
        public int? CsllRetido { get; set; }

        [XmlElement(ElementName = "csll_base_calc", IsNullable = true)]
        public decimal? CsllBaseCalc { get; set; }

        [XmlElement(ElementName = "csll_valor", IsNullable = true)]
        public decimal? CsllValor { get; set; }
    }

    public class Total
    {
        [XmlElement(ElementName = "valor_total")]
        public decimal ValorTotal { get; set; }
    }

    public class Faturas
    {
        [XmlElement(ElementName = "fat")]
        public List<Fatura> FaturaList { get; set; }
    }

    public class Fatura
    {
        [XmlElement(ElementName = "num_item")]
        public int NumItem { get; set; }

        [XmlElement(ElementName = "numero_fatura")]
        public string NumeroFatura { get; set; }

        [XmlElement(ElementName = "data_vencimento")]
        public DateTime DataVencimento { get; set; }

        [XmlElement(ElementName = "valor_fatura")]
        public decimal ValorFatura { get; set; }
    }

    public class InformacaoAdicional
    {
        [XmlElement(ElementName = "observacao_manual", IsNullable = true)]
        public string ObservacaoManual { get; set; }
    }

    public class Eventos
    {
        [XmlElement(ElementName = "motivo_cancelamento", IsNullable = true)]
        public string MotivoCancelamento { get; set; }

        [XmlElement(ElementName = "data_cancelamento", IsNullable = true)]
        public DateTime? DataCancelamento { get; set; }

        [XmlElement(ElementName = "cancelada", IsNullable = true)]
        public string Cancelada { get; set; }

        [XmlElement(ElementName = "substitui_nf_registrada", IsNullable = true)]
        public ushort? SubstituiNfRegistrada { get; set; }
    }
}
