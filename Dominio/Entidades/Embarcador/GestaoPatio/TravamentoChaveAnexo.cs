namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRAVAMENTO_CHAVE_ANEXO", EntityName = "TravamentoChaveAnexo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAnexo", NameType = typeof(TravamentoChaveAnexo))]
    public class TravamentoChaveAnexo : Anexo.Anexo<TravamentoChave>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TravamentoChave", Column = "TCH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TravamentoChave EntidadeAnexo { get; set; }

        #endregion
    }
}
