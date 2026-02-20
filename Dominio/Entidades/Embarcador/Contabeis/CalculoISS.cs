namespace Dominio.Entidades.Embarcador.Contabeis
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CALCULO_ISS", EntityName = "CalculoISS", Name = "Dominio.Entidades.Embarcador.Contabeis.CalculoISS", NameType = typeof(CalculoISS))]
    public class CalculoISS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoServico", Column = "CIS_CODIGO_SERVICO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aliquota", Column = "CIS_ALIQUOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Aliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRetencao", Column = "CIS_PERCENTUAL_RETENCAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualRetencao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        public virtual string Descricao
        {
            get { return Localidade?.Descricao + " - " + CodigoServico; }
        }
    }
}
