using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_FATURAMENTO_OUTRO", EntityName = "FaturamentoCIOTOutro", Name = "Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOTOutro", NameType = typeof(FaturamentoCIOTOutro))]
    public class FaturamentoCIOTOutro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturamentoCIOT", Column = "CFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaturamentoCIOT FaturamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFO_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFO_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFO_TIPO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFO_TIPO_LANCAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TipoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFO_DOCUMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFO_DETALHES", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Detalhes { get; set; }
    }
}
