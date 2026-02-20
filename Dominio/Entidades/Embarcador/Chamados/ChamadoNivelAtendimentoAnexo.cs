using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_NIVEL_ATENDIMENTO_ANEXO", EntityName = "ChamadoNivelAtendimentoAnexo", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoNivelAtendimentoAnexo", NameType = typeof(ChamadoNivelAtendimentoAnexo))]
    public class ChamadoNivelAtendimentoAnexo : Anexo.Anexo<ChamadoAnalise>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoAnalise", Column = "ANC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ChamadoAnalise EntidadeAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNA_NIVEL", TypeType = typeof(EscalationList), NotNull = false)]
        public virtual EscalationList Nivel { get; set; }
        #endregion
    }
}
