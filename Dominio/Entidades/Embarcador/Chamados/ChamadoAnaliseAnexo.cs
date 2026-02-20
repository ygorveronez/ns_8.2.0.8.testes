namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_ANALISE_ANEXO", EntityName = "ChamadoAnaliseAnexo", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoAnaliseAnexo", NameType = typeof(ChamadoAnaliseAnexo))]
    public class ChamadoAnaliseAnexo : Anexo.Anexo<ChamadoAnalise>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoAnalise", Column = "ANC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ChamadoAnalise EntidadeAnexo { get; set; }

        #endregion
    }
}
