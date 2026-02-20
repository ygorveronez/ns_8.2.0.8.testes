namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_COLETA_ANEXO", EntityName = "AgendamentoColetaAnexo", Name = "Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo", NameType = typeof(AgendamentoColetaAnexo))]
    public class AgendamentoColetaAnexo : Anexo.Anexo<AgendamentoColeta>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AgendamentoColeta", Column = "ACO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override AgendamentoColeta EntidadeAnexo { get; set; }
        
        #endregion
    }
}
