namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MERCADO_LIVRE_HANDLING_UNIT_DETAIL", EntityName = "MercadoLivreHandlingUnitDetail", Name = "Dominio.Entidades.Embarcardor.Integracao.MercadoLivreHandlingUnitDetail", NameType = typeof(MercadoLivreHandlingUnitDetail))]
    public class MercadoLivreHandlingUnitDetail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MUD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MercadoLivreHandlingUnit", Column = "MHU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit HandlingUnit { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MUD_SHIPMENT_ID", TypeType = typeof(long), NotNull = true)]
        public virtual long ShipmentID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MUD_TRACKING_NUMBER", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TrackingNumber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MUD_WEIGHT", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Weight { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MUD_STATUS", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MUD_REQUEST_DOCUMENT", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string RequestDocument { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MUD_CHAVE_ACESSO", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MUD_TIPO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMercadoLivreHandlingUnit), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMercadoLivreHandlingUnit TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MUD_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MUD_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MercadoLivreHandlingUnitDetail", Column = "MUD_CODIGO_PAI", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail DetailRegistroPai { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }
    }
}
