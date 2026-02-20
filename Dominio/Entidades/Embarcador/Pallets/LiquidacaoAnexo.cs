
namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LIQUIDACAO_VALE_PALLET_ANEXOS", EntityName = "LiquidacaoAnexo", Name = "Dominio.Entidades.Embarcador.Pallets.LiquidacaoAnexo", NameType = typeof(LiquidacaoAnexo))]
    public class LiquidacaoAnexo : Anexo.Anexo<ValePallet>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValePallet", Column = "VLP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ValePallet EntidadeAnexo { get; set; }

        #endregion
    }
}
