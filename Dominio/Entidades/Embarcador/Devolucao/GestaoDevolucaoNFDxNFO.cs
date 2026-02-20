namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_NFD_X_NFO", EntityName = "GestaoDevolucaoNFDxNFO", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO", NameType = typeof(GestaoDevolucaoNFDxNFO))]

    public class GestaoDevolucaoNFDxNFO : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "DNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }
        #endregion

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO_NFD", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NFD { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO_NFO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NFO { get; set; }

        #region Atributos Virtuais
        public virtual string Descricao
        {
            get
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
