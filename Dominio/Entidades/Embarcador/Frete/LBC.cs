namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LBC", EntityName = "LBC", Name = "Dominio.Entidades.Embarcador.Frete.LBC", NameType = typeof(LBC))]
    public class LBC : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LBC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
    }
}
