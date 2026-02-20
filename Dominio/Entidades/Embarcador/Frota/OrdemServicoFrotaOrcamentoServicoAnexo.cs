using Dominio.Entidades.Embarcador.Frota;

namespace Dominio.Entidades.Embarcador.Anexo
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_SERVICO_FROTA_ORCAMENTO_SERVICO_ANEXO", EntityName = "OrdemServicoFrotaOrcamentoServicoAnexo", Name = "Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo", NameType = typeof(OrdemServicoFrotaOrcamentoServicoAnexo))]
    public class OrdemServicoFrotaOrcamentoServicoAnexo : Anexo.Anexo<OrdemServicoFrotaOrcamentoServico>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaOrcamentoServico", Column = "OOS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override OrdemServicoFrotaOrcamentoServico EntidadeAnexo { get; set; }

        #endregion

    }
}
