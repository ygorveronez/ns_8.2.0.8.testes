namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INFRACAO_ANEXOS", EntityName = "InfracaoAnexo", Name = "Dominio.Entidades.Embarcador.Frete.InfracaoAnexo", NameType = typeof(InfracaoAnexo))]
    public class InfracaoAnexo : Anexo.Anexo<Infracao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Infracao", Column = "INF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Infracao EntidadeAnexo { get; set; }

        #endregion
    }
}
