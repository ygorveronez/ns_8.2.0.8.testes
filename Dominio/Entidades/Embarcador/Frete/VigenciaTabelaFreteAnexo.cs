namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_VIGENCIA_ANEXOS", EntityName = "VigenciaTabelaFreteAnexo", Name = "Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFreteAnexo", NameType = typeof(VigenciaTabelaFreteAnexo))]
    public class VigenciaTabelaFreteAnexo : Anexo.Anexo<VigenciaTabelaFrete>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "VigenciaTabelaFrete", Column = "TFV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override VigenciaTabelaFrete EntidadeAnexo { get; set; }

        #endregion
    }
}
