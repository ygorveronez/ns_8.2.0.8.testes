namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SEPARACAO_MERCADORIA_RESPONSAVEL", EntityName = "SeparacaoMercadoriaResponsavel", Name = "Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel", NameType = typeof(SeparacaoMercadoriaResponsavel))]
    public class SeparacaoMercadoriaResponsavel : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RSM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SeparacaoMercadoria", Column = "SMR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SeparacaoMercadoria SeparacaoMercadoria { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Responsavel { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "RSM_CAPACIDADE_SEPARACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int CapacidadeSeparacao { get; set; }
    }
}