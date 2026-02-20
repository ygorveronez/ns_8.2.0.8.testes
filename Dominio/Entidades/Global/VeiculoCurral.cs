namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_VEICULO_CURRAL", EntityName = "VeiculoCurral", Name = "Dominio.Entidades.VeiculoCurral", NameType = typeof(VeiculoCurral))]
    public class VeiculoCurral : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VCU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Largura", Column = "VCU_LARGURA", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Largura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Comprimento", Column = "VCU_COMPRIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Comprimento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCurral", Column = "VCU_NUMERO_CURRAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroCurral { get; set; }
    }
}
