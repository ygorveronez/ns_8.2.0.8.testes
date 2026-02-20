using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO", EntityName = "DocumentoEntradaCentroResultado", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultado", NameType = typeof(DocumentoEntradaCentroResultado))]
    public class DocumentoEntradaCentroResultado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntradaTMS DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TDC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "TDC_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "TDC_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroResultado CentroResultado { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
