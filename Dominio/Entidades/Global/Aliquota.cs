namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALIQUOTA", EntityName = "Aliquota", Name = "Dominio.Entidades.Aliquota", NameType = typeof(Aliquota))]
    public class Aliquota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString("D");
            }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "ALI_UF_EMPRESA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "ALI_UF_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "ALI_UF_DESTINO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade AtividadeTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade AtividadeDestinatario { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "ALI_ALIQUOTA", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "ALI_CST", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ALI_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaFCP", Column = "ALI_ALIQUOTA_FCP", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaFCP { get; set; }
    }
}
