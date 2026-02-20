using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENVIO_DOCUMENTACAO_LOTE", EntityName = "EnvioDocumentacaoLote", Name = "Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote", NameType = typeof(EnvioDocumentacaoLote))]

    public class EnvioDocumentacaoLote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "EDL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "EDL_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDL_NUMERO_BOOKING", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDL_NUMERO_OS", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDL_NUMERO_CONTROLE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFiscal", Column = "EDL_NUMERO_FISCAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Container Container { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ProvedorOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FoiAnulado", Column = "EDL_FOI_ANULADO", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNaoPesquisa), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNaoPesquisa FoiAnulado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FoiSubstituido", Column = "EDL_FOI_SUBSTITUIDO", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNaoPesquisa), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNaoPesquisa FoiSubstituido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoEnvioDocumentacao", Column = "EDL_SITUACAO_ENVIO_DOCUMENTACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao SituacaoEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaEnvioDocumentacao", Column = "EDL_FORMA_ENVIO_EMAIL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao FormaEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDL_EMAIL_INFORMADO_MANUALMENTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string EmailInformadoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDL_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Retorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioAutomatico", Column = "EDL_ENVIO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnvioAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificadoOperador", Column = "EDL_NOTIFICADO_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificadoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTentativaEnvio", Column = "EDL_QUANTIDADE_TENTATIVA_ENVIO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeTentativaEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "EDL_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalEnvioDocumentacao", Column = "EDL_MODAL_ENVIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao ModalEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressaoLote", Column = "EDL_TIPO_IMPRESSAO_LOTE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoLote), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoLote TipoImpressaoLote { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposProposta", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ENVIO_DOCUMENTACAO_LOTE_TIPO_PROPOSTAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EDL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "EDL_TIPO_PROPOSTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TiposProposta { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposServicos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ENVIO_DOCUMENTACAO_LOTE_TIPO_SERVICOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EDL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "EDL_TIPO_SERVICO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> TiposServicos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CTes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ENVIO_DOCUMENTACAO_LOTE_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EDL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO")]
        public virtual ICollection<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
