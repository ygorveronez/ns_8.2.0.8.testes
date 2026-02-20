using Dominio.Entidades.Embarcador.Produtos;
using Dominio.Entidades.Embarcador.Pessoas;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DATA_PREFERENCIAL_DESCARGA_FORNECEDOR_CATEGORIA", EntityName = "DataPreferencialDescargaFornecedorCategoria", Name = "Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria", NameType = typeof(DataPreferencialDescargaFornecedorCategoria))]
    public class DataPreferencialDescargaFornecedorCategoria : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DataPreferencialDescarga", Column = "DPD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DataPreferencialDescarga DataPreferencialDescarga { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoFornecedor { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEmbarcador Categoria { get; set; }
    }
}
