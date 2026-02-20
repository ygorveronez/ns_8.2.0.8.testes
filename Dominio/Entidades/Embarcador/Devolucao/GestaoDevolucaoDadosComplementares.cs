using System;

namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_DADOS_COMPLEMENTARES", EntityName = "GestaoDevolucaoDadosComplementares", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares", NameType = typeof(GestaoDevolucaoDadosComplementares))]

    public class GestaoDevolucaoDadosComplementares : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucao", Column = "GDV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDevolucao GestaoDevolucao { get; set; }

        #region ETAPA: Centro de Custo e Conta Contábil
        [NHibernate.Mapping.Attributes.Property(0, Name = "CentroCusto", Column = "GDC_CENTRO_CUSTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CentroCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContaContabil", Column = "GDC_CONTA_CONTABIL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailEnviado", Column = "GDC_EMAIL_ENVIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailEnviado { get; set; }
        #endregion

        #region ETAPA: Agendamento
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDaSolicitacao", Column = "GDC_DATA_DA_SOLICITACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDaSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAnaliseAgendamento", Column = "GDC_DATA_ANALISE_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAnaliseAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamento", Column = "GDC_DATA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDescarregamento", Column = "GDC_DATA_DESCARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE_AGENDAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente RemetenteAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO_AGENDAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente DestinatarioAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_DESTINO_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestinoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAgendamento", Column = "GDC_OBSERVACAO_AGENDAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAnaliseAgendamento", Column = "GDC_OBSERVACAO_ANALISE_AGENDAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoAnaliseAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PeriodoDescarregamento", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento PeriodoDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }
        #endregion

        #region ETAPA: Aprovação
        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAprovacao", Column = "GDC_OBSERVACAO_APROVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "GDC_USUARIO_APROVACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioAprovacao { get; set; }
        #endregion

        #region ETAPA: OrdemeRemessa
        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "GDC_ORDEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Remessa", Column = "GDC_REMESSA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Remessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoDocumentacaoEntradaFiscal", Column = "GDC_OBSERVACAO_DOCUMENTACAO_ENTRADA_FISCAL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoDocumentacaoEntradaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarNFesTransferenciaPallet", Column = "GDC_ADICIONAR_NFES_TRANSFERENCIA_PALLET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarNFesTransferenciaPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControleFinalizacaoDevolucao", Column = "GDV_CONTROLE_FINALIZACAO_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControleFinalizacaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "GDV_TIPO_TOMADOR", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoTomador? TipoTomador { get; set; }
        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCancelamento", Column = "GDC_OBSERVACAO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoCancelamento { get; set; }

        #endregion

        #region Atributos Virtuais
        #endregion
    }
}
