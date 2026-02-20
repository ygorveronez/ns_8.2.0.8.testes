namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_TMS_ANEXO_ADIANTAMENTO_MOTORISTA", EntityName = "ChamadoTMSAnexoAdiantamentoMotorista", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista", NameType = typeof(ChamadoTMSAnexoAdiantamentoMotorista))]
    public class ChamadoTMSAnexoAdiantamentoMotorista : Anexo.Anexo<ChamadoTMS>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoTMS", Column = "CHT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ChamadoTMS EntidadeAnexo { get; set; }

        #endregion
    }
}
