namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO_ANEXOS", EntityName = "ContratoFreteAcrescimoDescontoAnexo", Name = "Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAnexo", NameType = typeof(ContratoFreteAcrescimoDescontoAnexo))]
    public class ContratoFreteAcrescimoDescontoAnexo : Anexo.Anexo<ContratoFreteAcrescimoDesconto>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteAcrescimoDesconto", Column = "CAD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ContratoFreteAcrescimoDesconto EntidadeAnexo { get; set; }

        #endregion
    }
}
