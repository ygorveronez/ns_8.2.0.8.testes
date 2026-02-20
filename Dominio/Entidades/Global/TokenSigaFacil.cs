using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SIGA_FACIL_TOKEN", EntityName = "TokenSigaFacil", Name = "Dominio.Entidades.TokenSigaFacil", NameType = typeof(TokenSigaFacil))]
    public class TokenSigaFacil: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SFK_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "SFK_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "SFK_TOKEN", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Token { get; set; }
    }
}
