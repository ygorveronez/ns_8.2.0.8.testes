using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_NOTA_FISCAL_LINHA", EntityName = "ImportacaoNotaFiscalLinha", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha", NameType = typeof(ImportacaoNotaFiscalLinha))]
    public class ImportacaoNotaFiscalLinha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IML_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImportacaoNotaFiscal", Column = "IMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ImportacaoNotaFiscal ImportacaoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IML_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "IML_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "IML_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Colunas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_IMPORTACAO_NOTA_FISCAL_LINHA_COLUNA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IML_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ImportacaoNotaFiscalLinhaColuna", Column = "IMC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna> Colunas { get; set; }


    }
}
