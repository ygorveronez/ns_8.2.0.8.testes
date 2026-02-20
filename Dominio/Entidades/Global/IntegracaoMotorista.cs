namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_MOTORISTA", EntityName = "IntegracaoMotorista", Name = "Dominio.Entidades.IntegracaoMotorista", NameType = typeof(IntegracaoMotorista))]
    public class IntegracaoMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Arquivo", Column = "INM_ARQUIVO", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string Arquivo { get; set; }
    }
}
