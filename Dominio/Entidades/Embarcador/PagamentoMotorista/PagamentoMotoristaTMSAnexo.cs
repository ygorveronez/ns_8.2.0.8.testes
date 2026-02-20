namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_MOTORISTA_TMS_ANEXO", EntityName = "PagamentoMotoristaTMSAnexo", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo", NameType = typeof(PagamentoMotoristaTMSAnexo))]
    public class PagamentoMotoristaTMSAnexo : Anexo.Anexo<PagamentoMotoristaTMS>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PagamentoMotoristaTMS EntidadeAnexo { get; set; }

        #endregion
    }
}
