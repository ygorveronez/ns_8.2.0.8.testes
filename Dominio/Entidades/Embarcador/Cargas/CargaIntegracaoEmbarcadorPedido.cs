namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INTEGRACAO_EMBARCADOR_PEDIDO", EntityName = "CargaIntegracaoEmbarcadorPedido", Name = "Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedido", NameType = typeof(CargaIntegracaoEmbarcadorPedido))]
    public class CargaIntegracaoEmbarcadorPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CIP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_PROTOCOLO", TypeType = typeof(int), NotNull = true)]
        public virtual int Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_NUMERO_PEDIDO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_TIPO_TOMADOR", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_TIPO_PAGAMENTO", TypeType = typeof(Dominio.Enumeradores.TipoPagamento), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaIntegracaoEmbarcador", Column = "CIE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador CargaIntegracaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_TIPO_RATEIO_DOCUMENTOS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos TipoRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_POSSUI_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_POSSUI_NFS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIP_POSSUI_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFSManual { get; set; }
    }
}
