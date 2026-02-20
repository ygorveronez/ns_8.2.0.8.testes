using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_ENTREGA", EntityName = "EntregaCTe", Name = "Dominio.Entidades.EntregaCTe", NameType = typeof(EntregaCTe))]
    public class EntregaCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ETC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "ETC_VALOR_FRETE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPrestacaoServico", Column = "ETC_VALOR_PREST_SERVICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAReceber", Column = "ETC_VALOR_RECEBER", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_ENTREGA_DOC")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ETC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EntregaCTeDocumento", Column = "ETD_CODIGO")]
        public virtual IList<Dominio.Entidades.EntregaCTeDocumento> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosTransporteAnterior", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_ENTREGA_DOC_TRANSP_ANT")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ETC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EntregaCTeDocumentoTransporteAnterior", Column = "ETA_CODIGO")]
        public virtual IList<Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior> DocumentosTransporteAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ComponentesPrestacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_ENTREGA_COMP_PREST")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ETC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EntregaCTeComponentePrestacao", Column = "ETP_CODIGO")]
        public virtual IList<Dominio.Entidades.EntregaCTeComponentePrestacao> ComponentesPrestacao { get; set; }

        public virtual EntregaCTe Clonar()
        {
            return (EntregaCTe)this.MemberwiseClone();
        }
    }
}
