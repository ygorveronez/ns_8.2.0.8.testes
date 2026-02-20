using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALORES_POR_DOCUMENTOS", EntityName = "ValoresPorDocumentos", Name = "Dominio.Entidades.ValoresPorDocumentos", NameType = typeof(ValoresPorDocumentos))]
    public class ValoresPorDocumentos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoEmissaoCTe", Column = "PEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoEmissaoCTe Plano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VPD_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieDiferente", Column = "VPD_SERIE_DIFERENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SerieDiferente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Series", Column = "VPD_SERIES", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Series { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "VPD_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual Decimal Valor { get; set; }
    }
}
