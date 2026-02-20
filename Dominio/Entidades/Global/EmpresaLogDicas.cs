using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_LOG_DICAS", EntityName = "EmpresaLogDicas", Name = "Dominio.Entidades.EmpresaLogDicas", NameType = typeof(EmpresaLogDicas))]
    public class EmpresaLogDicas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ELD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DicasEmissaoCTe", Column = "ELD_DICAS", Type = "StringClob", NotNull = false)]
        public virtual string DicasEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "ELD_FUNCIONARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ELD_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }
    }
}
