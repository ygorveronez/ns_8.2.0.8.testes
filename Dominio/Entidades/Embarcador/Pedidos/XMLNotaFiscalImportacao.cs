using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_XML_NOTA_FISCAL_IMPORTACAO", EntityName = "XMLNotaFiscalImportacao", Name = "Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao", NameType = typeof(XMLNotaFiscalImportacao))]
    public class XMLNotaFiscalImportacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFI_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFI_QUANTIDADE_REGISTROS_IMPORTADOS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeRegistrosImportados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFI_QUANTIDADE_REGISTROS_TOTAL", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeRegistrosTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFI_NOME_ARQUIVO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoNotaFiscal", Column = "IMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal.ImportacaoNotaFiscal ImportacaoNotaFiscal { get; set; }
    }
}
