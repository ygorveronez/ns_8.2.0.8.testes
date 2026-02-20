namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_AGENDAMENTO_ENTREGA", EntityName = "ConfiguracaoAgendamentoEntrega", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega", NameType = typeof(ConfiguracaoAgendamentoEntrega))]
    public class ConfiguracaoAgendamentoEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "VisualizarTelaDeAgendamentoPorEntrega", Column = "CAE_VISUALIZAR_TELA_DE_AGENDAMENTO_POR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarTelaDeAgendamentoPorEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteInformarDataDeAgendamentoEReagendamentoRetroativamente", Column = "CAE_PERMITE_INFORMAR_DATA_DE_AGENDAMENTO_E_REAGENDAMENTO_RETROATIVAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarDataDeAgendamentoEReagendamentoRetroativamente { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras
        public virtual string Descricao
        {
            get
            {
                return "Configuração Agendamento Entrega";
            }
        }
        #endregion Propriedades com Regras
    }
}