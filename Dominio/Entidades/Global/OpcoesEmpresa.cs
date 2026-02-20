namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPCOES_EMPRESA", EntityName = "OpcoesEmpresa", Name = "Dominio.Entidades.OpcoesEmpresa", NameType = typeof(OpcoesEmpresa))]
    public class OpcoesEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "OPC_NOME", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "OPC_VALOR", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Valor { get; set; }
    }
}
