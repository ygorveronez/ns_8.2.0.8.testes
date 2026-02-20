namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_VEICULO", EntityName = "IntegracaoVeiculo", Name = "Dominio.Entidades.IntegracaoVeiculo", NameType = typeof(IntegracaoVeiculo))]
    public class IntegracaoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Arquivo", Column = "INV_ARQUIVO", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string Arquivo { get; set; }
    }
}
