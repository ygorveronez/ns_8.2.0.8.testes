using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA", EntityName = "Fatura", Name = "Dominio.Entidades.Embarcador.Fatura.Fatura", NameType = typeof(Fatura))]
    public class Fatura : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.Fatura>, Interfaces.Embarcador.Entidade.IEntidade
    {
        public Fatura()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_ALIQUOTA_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 4, NotNull = false)]
        public virtual decimal? AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "FAT_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        /// <summary>
        /// Campo para controle da numeração da fatura (chave no banco de dados)
        /// Quando inserir a fatura deve setar esta numeração, quando setar o número da fatura deve zerar essa numeração
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_CONTROLE_NUMERACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ControleNumeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPreFatura", Column = "FAT_NUMERO_PRE_FATURA", TypeType = typeof(long), NotNull = false)]
        public virtual long NumeroPreFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_NUMERO_FATURA_ORIGINAL", TypeType = typeof(long), NotNull = false)]
        public virtual long NumeroFaturaOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_GERAR_DOCUMENTOS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarDocumentosAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFatura", Column = "FAT_DATA_FATURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "FAT_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "FAT_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FAT_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "FAT_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_SITUACAO_NO_CANCELAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura? SituacaoNoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Etapa", Column = "FAT_ETAPA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "FAT_TIPO_PESSOA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_TOTAL_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Total", Column = "FAT_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Total { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "FAT_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Acrescimo", Column = "FAT_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Acrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFatura", Column = "FAT_OBSERVACAO_FATURA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimeObservacaoFatura", Column = "FAT_IMPRIME_OBSERVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimeObservacaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_DESCONTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_ACRESCIMO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteTomadorFatura { get; set; }

        /// <summary>
        /// TRANSPORTADORA DA FATURA, APENAS EMBARCADOR
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_TRANSPORTADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_TRANSPORTADORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportadora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoPagamentoRecebimento FormaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoUso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoReversao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO_DESCONTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoUsoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_DESCONTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoReversaoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO_ACRESCIMO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoUsoAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_ACRESCIMO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoReversaoAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "FAT_BANCO_AGENCIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "FAT_BANCO_DIGITO_AGENCIA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "FAT_BANCO_NUMERO_CONTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaBanco", Column = "FAT_BANCO_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? TipoContaBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_TIPO_ARREDONDAMENTO_PARCELAS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento TipoArredondamentoParcelas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamentoFatura", Column = "FAT_DATA_CANCELAMENTO_FATURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamentoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_NOVO_MODELO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NovoModelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_NOME_CLIENTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_CODIGO_DEPOSITO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoDeposito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_TIPO_FRETE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TipoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_CODIGO_TRANSPORTADORA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_MODALIDADE_FRETE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ModalidadeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_DATA_FECHAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_REVERTEU_ACRESCIMO_DESCONTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReverteuAcrescimoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_NUMERO_BOOKING", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_GERAR_POR_FATURA_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoPorFaturaLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_FATURA_PROPOSTA_FATURAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturaPropostaFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pais PaisOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoPropostaMultimodal", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_TIPO_PROPOSTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "FAT_TIPO_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal), NotNull = true)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TipoPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOSConvertidos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_TIPO_OS_CONVERTIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "FAT_TIPO_OS_CONVERTIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido> TiposOSConvertidos { get; set; }

        //Opção do agrupamento da fatura
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.Container Container { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_NUMERO_CONTROLE_CLIENTE", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string NumeroControleCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_IE_TOMADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IETomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_NUMERO_REFERENCIA_EDI", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string NumeroReferenciaEDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "COM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "FAT_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "FAT_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "FAT_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalMoedaEstrangeira", Column = "FAT_TOTAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalMoedaEstrangeira { get; set; }

        /// <summary>
        /// Utilizará apenas o Real como padrão para estas faturas, pois, mesmo que os documentos sejam emitidos em Dólar, em alguns casos o recebimento é em Real.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_NAO_UTILIZAR_MOEDA_ESTRANGEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_GERAR_DOCUMENTOS_APENAS_CANHOTOS_APROVADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarDocumentosApenasCanhotosAprovados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaCarga", Column = "FAC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Fatura.FaturaCarga> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FaturaCargaDocumentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_CARGA_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaCargaDocumento", Column = "FCD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> FaturaCargaDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Logs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_LOG")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaLog", Column = "FTL_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Fatura.FaturaLog> Logs { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Parcelas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_PARCELA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaParcela", Column = "FAP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> Parcelas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaDocumento", Column = "FDO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FaturaLoteCTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_LOTE_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaLoteCTe", Column = "FLC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe> FaturaLoteCTes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_NOTIFICADO_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificadoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_FATURAMENTO_EXCLUSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTe", Column = "FAT_TIPO_CTE", TypeType = typeof(Dominio.Enumeradores.TipoCTE), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCTE? TipoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFaturaIntegracao", Column = "FAT_NUMERO_FATURA_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroFaturaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_FATURA_GERADA_PELA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturaGeradaPelaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloIntegracao", Column = "FAT_PROTOCOLO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int ProtocoloIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturaRecebidaDeIntegracao", Column = "FAT_FATURA_RECEBIDA_DE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturaRecebidaDeIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturaIntegracaComSucesso", Column = "FAT_FATURA_INTEGRADA_COM_SUCESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturaIntegracaComSucesso { get; set; }

        /// <summary>
        /// Indica se a fatura será duplicada no cancelamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "FAT_DUPLICAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Duplicar { get; set; }

        /// <summary>
        /// Fatura cancelada, que foi duplicada e originou esta fatura.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO_ORIGINAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura FaturaOriginal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JustificativaCancelamentoFinanceiro", Column = "JCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro JustificativaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RolagemCarga", Column = "FAT_ROLAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RolagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracaoFatura", Column = "FAT_DATA_GERACAO_FATURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoPDF", Column = "FAT_CAMINHO_PDF", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CaminhoPDF { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descontos", Formula = @"(SELECT ISNULL(SUM(AD.FAD_VALOR), 0)
                                                                                        FROM T_FATURA_ACRESCIMO_DESCONTO AD
                                                                                        JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = AD.JUS_CODIGO
                                                                                        WHERE J.JUS_TIPO = 1 AND AD.FAT_CODIGO = FAT_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal Descontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Acrescimos", Formula = @"(SELECT ISNULL(SUM(AD.FAD_VALOR), 0)
                                                                                        FROM T_FATURA_ACRESCIMO_DESCONTO AD
                                                                                        JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = AD.JUS_CODIGO
                                                                                        WHERE J.JUS_TIPO = 2 AND AD.FAT_CODIGO = FAT_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal Acrescimos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontosMoeda", Formula = @"(select ISNULL(SUM(Documento.FDO_VALOR_DESCONTO_MOEDA), 0) from T_FATURA_DOCUMENTO Documento
                                                                                            where Documento.FAT_CODIGO = FAT_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal DescontosMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescimosMoeada", Formula = @"(select ISNULL(SUM(Documento.FDO_VALOR_ACRESCIMO_MOEDA), 0) from T_FATURA_DOCUMENTO Documento
                                                                                            where Documento.FAT_CODIGO = FAT_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal AcrescimosMoeada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoVencimento", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + convert(varchar(10), FP.FAP_DATA_VENCIMENTO , 103) 
                                                                                        FROM T_FATURA_PARCELA FP
                                                                                        WHERE FP.FAT_CODIGO = FAT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string PeriodoVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoEmissao", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + convert(varchar(10), FP.FAP_DATA_EMISSAO , 103) 
                                                                                        FROM T_FATURA_PARCELA FP
                                                                                        WHERE FP.FAT_CODIGO = FAT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string PeriodoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTitulos", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(T.TIT_CODIGO AS VARCHAR(20))
                                                                                        FROM T_FATURA_PARCELA FP
                                                                                        JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO
                                                                                        WHERE FP.FAT_CODIGO = FAT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroTitulos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBoletos", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + ISNULL(T.TIT_NOSSO_NUMERO, '')
                                                                                        FROM T_FATURA_PARCELA FP
                                                                                        JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO
                                                                                        WHERE FP.FAT_CODIGO = FAT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroBoletos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoTitulos", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN T.TIT_STATUS = 1 THEN 'Aberto' WHEN T.TIT_STATUS = 3 THEN 'Quitado' WHEN T.TIT_STATUS = 4 THEN 'Cancelado' ELSE 'Em Negociação' END 
                                                                                        FROM T_FATURA_PARCELA FP
                                                                                        JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO
                                                                                        WHERE FP.FAT_CODIGO = FAT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string SituacaoTitulos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosFiscais", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CON_NUM AS VARCHAR(20)) from T_FATURA_DOCUMENTO F
                                                                                        JOIN T_DOCUMENTO_FATURAMENTO D ON D.DFA_CODIGO = F.DFA_CODIGO
                                                                                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                                                                                        WHERE F.FAT_CODIGO = FAT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosControle", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + C.CON_NUMERO_CONTROLE from T_FATURA_DOCUMENTO F
                                                                                        JOIN T_DOCUMENTO_FATURAMENTO D ON D.DFA_CODIGO = F.DFA_CODIGO
                                                                                        JOIN T_CTE C ON C.CON_CODIGO = D.CON_CODIGO
                                                                                        WHERE F.FAT_CODIGO = FAT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosControle { get; set; }

        public virtual string DescricaoEtapa
        {
            get
            {
                switch (this.Etapa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Carga:
                        return "Carga";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fatura:
                        return "Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento:
                        return "Fechamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Integracao:
                        return "Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas:
                        return "Lançando Cargas";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento:
                        return "Em Andamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmFechamento:
                        return "Em Fechamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmCancelamento:
                        return "Em Cancelamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado:
                        return "Fechado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Liquidado:
                        return "Liquidado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.SemRegraAprovacao:
                        return "Sem Regra Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.AguardandoAprovacao:
                        return "Aguardando Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.AprovacaoRejeitada:
                        return "Aprovação Rejeitada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.ProblemaIntegracao:
                        return "Problema com a Integração";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoPeriodo
        {
            get
            {
                return this.DataInicial.HasValue && this.DataFinal.HasValue ? this.DataInicial.Value.ToString("dd/MM/yyyy") + " até " + this.DataFinal.Value.ToString("dd/MM/yyyy") : "";
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual bool Equals(Fatura other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Clonar()
        {
            return (Dominio.Entidades.Embarcador.Fatura.Fatura)this.MemberwiseClone();
        }

        #endregion
    }
}
