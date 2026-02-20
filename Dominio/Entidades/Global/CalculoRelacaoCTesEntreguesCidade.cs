namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CALCULO_RELACAO_CTES_ENTREGUES_CIDADE", EntityName = "CalculoRelacaoCTesEntreguesCidade", Name = "Dominio.Entidades.CalculoRelacaoCTesEntreguesCidade", NameType = typeof(CalculoRelacaoCTesEntreguesCidade))]
    public class CalculoRelacaoCTesEntreguesCidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CalculoRelacaoCTesEntregues", Column = "CRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CalculoRelacaoCTesEntregues CalculoRelacaoCTesEntregues { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Cidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCC_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Cidade?.Descricao ?? string.Empty;
            }
        }
    }
}
