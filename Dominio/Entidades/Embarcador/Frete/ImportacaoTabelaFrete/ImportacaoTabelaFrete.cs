using System;

namespace Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_TABELA_FRETE", EntityName = "ImportacaoTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete", NameType = typeof(ImportacaoTabelaFrete))]
    public class ImportacaoTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ITF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_NOME_ARQUIVO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_QUANTIDADE_LINHAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeLinhas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_DATA_INICIO_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_DATA_FIM_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimProcessamento { get; set; }

        #region Configurações para importação

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "VigenciaTabelaFrete", Column = "TFV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_ORIGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_ESTADO_ORIGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaEstadoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_REMETENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_CEP_ORIGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaCEPOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_ROTA_ORIGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaRotaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_REGIAO_ORIGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaRegiaoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_DESTINO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_ESTADO_DESTINO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaEstadoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_DESTINATARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_CEP_DESTINO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaCEPDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_ROTA_DESTINO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaRotaDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_REGIAO_DESTINO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaRegiaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_PRAZO_CEP_DESTINO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaPrazoCEPDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_TOMADOR_DESTINO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaTomadorDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_PARAMETRO_BASE", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaParametroBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_LINHA_INICIO_DADOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaLinhaInicioDados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_VALIDO_PARA_QUALQUER_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidoParaQualquerOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_VALIDO_PARA_QUALQUER_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidoParaQualquerDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_NAO_ATUALIZAR_VALORES_ZERADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAtualizarValoresZerados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_NAO_VALIDAR_TABELAS_EXISTENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarTabelasExistentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_TOKEN_ARQUIVO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string TokenArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_FRONTEIRA", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaFronteira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_PRIORIDADE_USO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaPrioridadeUso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_CANAL_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaCanalEntrega { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_TIPO_OPERACAO", TypeType = typeof(int), NotNull = false)]
		public virtual int? ColunaTipoOperacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_TIPO_DE_CARGA", TypeType = typeof(int), NotNull = false)]
		public virtual int? ColunaTipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_LEAD_TIME_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaLeadTimeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_MOEDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_TIPO_PAGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao? TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ITF_COLUNA_KMSISTEMA", TypeType = typeof(int), NotNull = false)]
        public virtual int? ColunaKMSistema { get; set; }
        #endregion

        public virtual TimeSpan? Tempo()
        {
            if (DataInicioProcessamento.HasValue && DataFimProcessamento.HasValue)
                return DataFimProcessamento.Value - DataInicioProcessamento.Value;
            else
                return null;
        }

        public virtual string Descricao
        {
            get
            {
                return NomeArquivo;
            }
        }
    }
}
