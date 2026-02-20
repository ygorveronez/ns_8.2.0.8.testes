namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODELO_VEICULAR_CARGA_DIVISAO_CAPACIDADE", EntityName = "ModeloVeicularCargaDivisaoCapacidade", Name = "Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade", NameType = typeof(ModeloVeicularCargaDivisaoCapacidade))]
    public class ModeloVeicularCargaDivisaoCapacidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDC_DESCRICAO", TypeType = typeof(string), NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeDeMedida", Column = "UNI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.UnidadeDeMedida UnidadeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDC_QUANTIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDC_PISO", TypeType = typeof(int), NotNull = false)]
        public virtual int? Piso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDC_COLUNA", TypeType = typeof(int), NotNull = false)]
        public virtual int? Coluna { get; set; }
    }
}
