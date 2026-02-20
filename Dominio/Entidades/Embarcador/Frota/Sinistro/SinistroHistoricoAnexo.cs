namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SINISTRO_HISTORICO_ANEXO", EntityName = "SinistroHistoricoAnexo", Name = "Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo", NameType = typeof(SinistroHistoricoAnexo))]
    public class SinistroHistoricoAnexo : Anexo.Anexo<SinistroHistorico>
    {
        #region Propriedades Sobrescritas
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SinistroHistorico", Column = "SHC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override SinistroHistorico EntidadeAnexo { get; set; }

        #endregion
    }
}
