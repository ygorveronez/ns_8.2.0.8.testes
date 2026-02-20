using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRACKING_DOCUMENTACAO_REGISTRO", EntityName = "TrackingDocumentacaoRegistro", Name = "Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro", NameType = typeof(TrackingDocumentacaoRegistro))]
    public class TrackingDocumentacaoRegistro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "TDR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "TDR_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaIMO", Column = "TDR_CARGA_IMO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaIMO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTrackingDocumentacao", Column = "TDO_TIPO_TRACKING_DOCUMENTACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao TipoTrackingDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario OperadorCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TrackingDocumentacao", Column = "TDO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TrackingDocumentacao TrackingDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TRACKING_DOCUMENTACAO_REGISTRO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TDR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TrackingDocumentacaoRegistroCarga", Column = "TRC_CODIGO")]
        public virtual IList<TrackingDocumentacaoRegistroCarga> Cargas { get; set; }

        public virtual string DescricaoCargaIMO
        {
            get
            {
                if (this.CargaIMO)
                    return "SIM";
                else
                    return "NãO";
            }
        }

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
                return this.TrackingDocumentacao.Numero.ToString();
            }
        }
    }
}
