namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INFRACAO_HISTORICO_ANEXOS", EntityName = "InfracaoHistoricoAnexo", Name = "Dominio.Entidades.Embarcador.Frota.InfracaoHistoricoAnexo", NameType = typeof(InfracaoHistoricoAnexo))]
    public class InfracaoHistoricoAnexo : Anexo.Anexo<InfracaoHistorico>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "InfracaoHistorico", Column = "IFH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override InfracaoHistorico EntidadeAnexo { get; set; }

        #endregion
    }
}
