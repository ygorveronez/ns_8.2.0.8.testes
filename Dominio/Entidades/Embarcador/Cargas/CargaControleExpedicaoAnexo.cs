namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CONTROLE_EXPEDICAO_ANEXO", EntityName = "CargaControleExpedicaoAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicaoAnexo", NameType = typeof(CargaControleExpedicaoAnexo))]
    public class CargaControleExpedicaoAnexo : Anexo.Anexo<CargaControleExpedicao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaControleExpedicao", Column = "CCX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CargaControleExpedicao EntidadeAnexo { get; set; }

        #endregion
    }
}