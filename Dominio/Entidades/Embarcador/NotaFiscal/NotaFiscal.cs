using Dominio.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL", EntityName = "NotaFiscal", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal", NameType = typeof(NotaFiscal))]
    public class NotaFiscal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NFI_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "NFI_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "NFI_PROTOCOLO", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissao", Column = "NFI_TIPO_EMISSAO", TypeType = typeof(Dominio.Enumeradores.TipoEmissaoNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoEmissaoNFe TipoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAmbiente", Column = "NFI_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAmbiente), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "NFI_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSaida", Column = "NFI_DATA_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrestacaoServico", Column = "NFI_DATA_PRESTACAO_SERVICO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "NFI_DATA_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Finalidade", Column = "NFI_FINALIDADE", TypeType = typeof(Dominio.Enumeradores.FinalidadeNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.FinalidadeNFe Finalidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorPresenca", Column = "NFI_INDICADOR_PRESENCA", TypeType = typeof(Dominio.Enumeradores.IndicadorPresencaNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.IndicadorPresencaNFe IndicadorPresenca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NFI_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.StatusNFe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCICMS", Column = "NFI_BC_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "NFI_VALOR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ICMSDesonerado", Column = "NFI_ICMS_DESONERADO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ICMSDesonerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorII", Column = "NFI_VALOR_II", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorII { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCICMSST", Column = "NFI_BC_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSST", Column = "NFI_VALOR_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProdutos", Column = "NFI_VALOR_PRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "NFI_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSeguro", Column = "NFI_VALOR_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "NFI_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutrasDespesas", Column = "NFI_VALOR_OUTRAS_DESPESAS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorOutrasDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIPI", Column = "NFI_VALOR_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalNota", Column = "NFI_VALOR_TOTAL_NOTA", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTotalNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorServicos", Column = "NFI_VALOR_SERVICOS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorServicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCISSQN", Column = "NFI_BC_ISSQN", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCISSQN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISSQN", Column = "NFI_VALOR_ISSQN", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorISSQN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCDeducao", Column = "NFI_BC_DEDUCAO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCDeducao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutrasRetencoes", Column = "NFI_VALOR_OUTRAS_RETENCOES", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorOutrasRetencoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoIncondicional", Column = "NFI_VALOR_DESCONTO_INCONDICIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorDescontoIncondicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoCondicional", Column = "NFI_VALOR_DESCONTO_CONDICIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorDescontoCondicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetencaoISS", Column = "NFI_VALOR_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCPIS", Column = "NFI_BC_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "NFI_VALOR_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCCOFINS", Column = "NFI_BC_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BCCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "NFI_VALOR_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFCP", Column = "NFI_VALOR_FCP", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorFCP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSDestino", Column = "NFI_VALOR_ICMS_DESTINO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMSDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSRemetente", Column = "NFI_VALOR_ICMS_REMETENTE", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMSRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoTributaria", Column = "NFI_OBSERVACAO_TRIBUTARIA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoTributaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoNota", Column = "NFI_OBSERVACAO_NOTA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspQuantidade", Column = "NFI_TRANSP_QUANTIDADE", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string TranspQuantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspEspecie", Column = "NFI_TRANSP_ESPECIE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string TranspEspecie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspMarca", Column = "NFI_TRANSP_MARCA", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string TranspMarca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspVolume", Column = "NFI_TRANSP_VOLUME", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string TranspVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspPesoBruto", Column = "NFI_TRANSP_PESO_BRUTO", TypeType = typeof(decimal), Scale = 3, Precision = 15, NotNull = false)]
        public virtual decimal TranspPesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspPesoLiquido", Column = "NFI_TRANSP_PESO_LIQUIDO", TypeType = typeof(decimal), Scale = 3, Precision = 15, NotNull = false)]
        public virtual decimal TranspPesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFrete", Column = "NFI_TRANSP_TIPO_FRETE", TypeType = typeof(Dominio.Enumeradores.ModalidadeFrete), NotNull = false)]
        public virtual Dominio.Enumeradores.ModalidadeFrete TipoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspPlacaVeiculo", Column = "NFI_TRANSP_PLACA_VEICULO", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string TranspPlacaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspUFVeiculo", Column = "NFI_TRANSP_UF_VEICULO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TranspUFVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspANTTVeiculo", Column = "NFI_TRANSP_ANTT_VEICULO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string TranspANTTVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspCNPJCPF", Column = "NFI_TRANSP_CNPJ_CPF_TRANSPORTADORA", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string TranspCNPJCPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspNome", Column = "NFI_TRANSP_NOME_TRANSPORTADORA", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string TranspNome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspIE", Column = "NFI_TRANSP_IE_TRANSPORTADORA", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string TranspIE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspEndereco", Column = "NFI_TRANSP_ENDERECO_TRANSPORTADORA", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string TranspEndereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspMunicipio", Column = "NFI_TRANSP_MUNICIPIO_TRANSPORTADORA", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string TranspMunicipio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_TRANSP_MUNICIPIO_TRANSPORTADORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeTranspMunicipio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspUF", Column = "NFI_TRANSP_UF_TRANSPORTADORA", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TranspUF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TranspEmail", Column = "NFI_TRANSP_EMAIL_TRANSPORTADORA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TranspEmail { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "ESE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie EmpresaSerie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaDaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade Atividade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_PRESTACAO_SERVICO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadePrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TRANSPORTADORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Transportadora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO_TRANSPORTADORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRecibo", Column = "NFI_NUMERO_RECIBO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string NumeroRecibo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoStatus", Column = "NFI_ULTIMO_STATUS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string UltimoStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorImpostoIBPT", Column = "NFI_VALOR_IMPOSTO_OBPT", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorImpostoIBPT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoStatusSEFAZ", Column = "NFI_ULTIMO_STATUS_SEFAZ", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string UltimoStatusSEFAZ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFEmbarque", Column = "NFI_UF_EMBARQUE", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalEmbarque", Column = "NFI_LOCAL_EMBARQUE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string LocalEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacaoCompraNotaEmpenho", Column = "NFI_INFORMACAO_COMPRA_NOTA_EMPENHO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string InformacaoCompraNotaEmpenho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacaoCompraPedido", Column = "NFI_INFORMACAO_COMPRA_PEDIDO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string InformacaoCompraPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacaoCompraContrato", Column = "NFI_INFORMACAO_COMPRA_CONTRATO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string InformacaoCompraContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalDespacho", Column = "NFI_LOCAL_DESPACHO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string LocalDespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloNotaFiscal", Column = "NFI_MODELO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string ModeloNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFCNPJConsumidorFinal", Column = "NFI_CPF_CNPJ_CONSUMIDOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CPFCNPJConsumidorFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeConsumidorFinal", Column = "NFI_NOME_CONSUMIDOR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeConsumidorFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTroco", Column = "NFI_VALOR_TROCO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTroco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoNFe", Column = "NFI_VERSAO_NFE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.VersaoNFe), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.VersaoNFe VersaoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFCPICMS", Column = "NFI_VALOR_FCP_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorFCPICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFCPICMSST", Column = "NFI_VALOR_FCP_ICMSST", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorFCPICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIPIDevolvido", Column = "NFI_VALOR_IPI_DEVOLVIDO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorIPIDevolvido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BCICMSSTRetido", Column = "NFI_BC_ICMS_ST_RETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BCICMSSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSSTRetido", Column = "NFI_VALOR_ICMS_ST_RETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorIntermediador", Column = "NFI_INDICADOR_INTERMEDIADOR", TypeType = typeof(Dominio.Enumeradores.IndicadorIntermediadorNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.IndicadorIntermediadorNFe? IndicadorIntermediador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_INTERMEDIADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Intermediador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaPagamento", Column = "NFI_FORMA_PAGAMENTO", TypeType = typeof(Dominio.Enumeradores.FormaPagamento), NotNull = false)]
        public virtual Dominio.Enumeradores.FormaPagamento? FormaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarEnderecoRetirada", Column = "NFI_UTILIZAR_ENDERECO_RETIRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEnderecoRetirada { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_RETIRADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteRetirada { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_RETIRADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeRetirada { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "RetiradaLogradouro", Column = "NFI_RETIRADA_LOGRADOURO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string RetiradaLogradouro { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "RetiradaNumeroLogradouro", Column = "NFI_RETIRADA_NUMERO_LOGRADOURO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string RetiradaNumeroLogradouro { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "RetiradaComplementoLogradouro", Column = "NFI_RETIRADA_COMPLEMENTO_LOGRADOURO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string RetiradaComplementoLogradouro { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "RetiradaBairro", Column = "NFI_RETIRADA_BAIRRO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string RetiradaBairro { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "RetiradaCEP", Column = "NFI_RETIRADA_CEP", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string RetiradaCEP { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "RetiradaTelefone", Column = "NFI_RETIRADA_TELEFONE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string RetiradaTelefone { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "RetiradaEmail", Column = "NFI_RETIRADA_EMAIL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string RetiradaEmail { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "RetiradaIE", Column = "NFI_RETIRADA_IE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string RetiradaIE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarEnderecoEntrega", Column = "NFI_UTILIZAR_ENDERECO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEnderecoEntrega { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteEntrega { get; set; }
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeEntrega { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaLogradouro", Column = "NFI_ENTREGA_LOGRADOURO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EntregaLogradouro { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaNumeroLogradouro", Column = "NFI_ENTREGA_NUMERO_LOGRADOURO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EntregaNumeroLogradouro { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaComplementoLogradouro", Column = "NFI_ENTREGA_COMPLEMENTO_LOGRADOURO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EntregaComplementoLogradouro { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaBairro", Column = "NFI_ENTREGA_BAIRRO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EntregaBairro { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaCEP", Column = "NFI_ENTREGA_CEP", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EntregaCEP { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaTelefone", Column = "NFI_ENTREGA_TELEFONE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EntregaTelefone { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaEmail", Column = "NFI_ENTREGA_EMAIL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EntregaEmail { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaIE", Column = "NFI_ENTREGA_IE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EntregaIE { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "ItensNFe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NOTA_FISCAL_PRODUTOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NotaFiscalProdutos", Column = "NFP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> ItensNFe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ReferenciaNFe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NOTA_FISCAL_REFERENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NotaFiscalReferencia", Column = "NFR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia> ReferenciaNFe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ParcelasNFe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NOTA_FISCAL_PARCELA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NotaFiscalParcela", Column = "NFP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela> ParcelasNFe { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString() + " - " + (this.EmpresaSerie?.Descricao ?? string.Empty);
            }
        }

        public virtual string DescricaoStatus
        {
            get { return Status.ObterDescricao(); }
        }

        public virtual bool Equals(NotaFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal Clonar()
        {
            return (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal)this.MemberwiseClone();
        }
    }
}
