using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRACKING_DOCUMENTACAO", EntityName = "TrackingDocumentacao", Name = "Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao", NameType = typeof(TrackingDocumentacao))]
    public class TrackingDocumentacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "TDO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "TDO_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoPendente", Column = "TDO_INTEGRACAO_PENDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoPendente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "TDO_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIMO", Column = "TDO_TIPO_IMO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO TipoIMO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoTrackingDocumentacao", Column = "TDO_SITUACAO_RECKING_DOCUMENTACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTrackingDocumentacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTrackingDocumentacao SituacaoTrackingDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTrackingDocumentacao", Column = "TDO_TIPO_TRACKING_DOCUMENTACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao TipoTrackingDocumentacao { get; set; }        

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Registros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TRACKING_DOCUMENTACAO_REGISTRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TrackingDocumentacaoRegistro", Column = "TDR_CODIGO")]
        public virtual IList<TrackingDocumentacaoRegistro> Registros { get; set; }

        public virtual string DescricaoTipoTrackingDocumentacao
        {
            get
            {
                switch (TipoTrackingDocumentacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao.Cabotagem:
                        return "Cabotagem";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao.Feeder:
                        return "Feeder";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }
    }
}
