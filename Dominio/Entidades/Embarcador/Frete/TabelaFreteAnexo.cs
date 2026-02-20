namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_ANEXO", EntityName = "TabelaFreteAnexo", Name = "Dominio.Entidades.Embarcador.Frete.PedidoAnexo", NameType = typeof(TabelaFreteAnexo))]
    public class TabelaFreteAnexo : Anexo.Anexo<TabelaFrete>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TabelaFrete EntidadeAnexo { get; set; }

        #endregion
    }
}
