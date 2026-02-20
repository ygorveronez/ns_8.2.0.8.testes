namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SINISTRO_INDICACAO_PAGADOR_ANEXO", EntityName = "SinistroIndicacaoPagadorAnexo", Name = "Dominio.Entidades.Embarcador.Frota.SinistroIndicacaoPagadorAnexo", NameType = typeof(SinistroIndicacaoPagadorAnexo))]
    public class SinistroIndicacaoPagadorAnexo : Anexo.Anexo<SinistroDados>
    {
        #region Propriedades Sobrescritas
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SinistroDados", Column = "SDS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override SinistroDados EntidadeAnexo { get; set; }

        #endregion
    }
}
