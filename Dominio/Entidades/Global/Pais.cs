using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAIS", EntityName = "Pais", Name = "Dominio.Entidades.Pais", NameType = typeof(Pais))]
    public class Pais : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "PAI_NOME", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sigla", Column = "PAI_SIGLA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Sigla { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Abreviacao", Column = "PAI_ABREVIACAO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string Abreviacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTelefonico", Column = "PAI_CODIGO_TELEFONICO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoTelefonico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LicencaTNTI", Column = "PAI_LICENCA_TNTI", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LicencaTNTI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VencimentoLicencaTNTI", Column = "PAI_VENCIMENTO_LICENCA_TNTI", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VencimentoLicencaTNTI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPais", Column = "PAI_CODIGO_PAIS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoPais { get; set; }

        public virtual string Descricao
        {
            get { return Nome; }
        }
    }
}
