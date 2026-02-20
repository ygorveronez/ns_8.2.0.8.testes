using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENVIO_DOCUMENTACAO_AFRMM", EntityName = "EnvioDocumentacaoAFRMM", Name = "Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM", NameType = typeof(EnvioDocumentacaoAFRMM))]
    public class EnvioDocumentacaoAFRMM : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "EDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "EDA_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDA_NUMERO_BOOKING", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDA_NUMERO_CONTROLE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroControle { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoEnvioDocumentacao", Column = "EDA_SITUACAO_ENVIO_DOCUMENTACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao SituacaoEnvioDocumentacao { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDA_EMAIL_INFORMADO_MANUALMENTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string EmailInformadoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDA_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Retorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificadoOperador", Column = "EDA_NOTIFICADO_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificadoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioAutomatico", Column = "EDA_ENVIO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnvioAutomatico { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTentativaEnvio", Column = "EDA_QUANTIDADE_TENTATIVA_ENVIO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeTentativaEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "EDA_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescargaPODInicial", Column = "EDA_DATA_DESCARGA_POD_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DescargaPODInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescargaPODFinal", Column = "EDA_DATA_DESCARGA_POD_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DescargaPODFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioDocInicial", Column = "EDA_DATA_ENVIO_DOC_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? EnvioDocInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioDocFinal", Column = "EDA_DATA_ENVIO_DOC_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? EnvioDocFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDA_NUMERO_MANIFESTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroManifesto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDA_NUMERO_MANIFESTO_TRANSBORDO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroManifestoTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EDA_NUMERO_CE_MERCANTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroCEMercante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentacaoAFRMM", Column = "EDA_TIPO_DOCUMENTACAO_AFRMM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoLote), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentacaoAFRMM TipoDocumentacaoAFRMM { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposServicos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ENVIO_DOCUMENTACAO_AFRMM_TIPO_SERVICOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "EDA_TIPO_SERVICO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> TiposServicos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CTes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ENVIO_DOCUMENTACAO_AFRMM_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EDA_CODIGO")]
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
