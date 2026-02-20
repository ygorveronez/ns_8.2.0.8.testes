namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_INSCRICAO_ST", EntityName = "TransportadorInscricaoST", Name = "Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST", NameType = typeof(TransportadorInscricaoST))]
    public class TransportadorInscricaoST : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ENS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoST", Column = "ENS_INSCRICAO_ST", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string InscricaoST { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }
    }
}
