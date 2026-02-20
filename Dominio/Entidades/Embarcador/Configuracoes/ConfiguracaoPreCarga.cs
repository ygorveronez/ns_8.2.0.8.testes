namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_PRE_CARGA", EntityName = "ConfiguracaoPreCarga", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga", NameType = typeof(ConfiguracaoPreCarga))]
    public class ConfiguracaoPreCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPC_DIAS_TRANSICAO_AUTOMATICA_FILA_CARREGAMENTO_VEICULO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasTransicaoAutomaticaFilaCarregamentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPC_DIAS_PARA_TRANSPORTADOR_ADICIONAR_FILA_CARREGAMENTO_VEICULO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaTransportadorAdicionarFilaCarregamentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPC_TEMPO_AGUARDAR_CONFIRMACAO_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoAguardarConfirmacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VincularFilaCarregamentoVeiculoAutomaticamente", Column = "CPC_VINCULAR_FILA_CARREGAMENTO_VEICULO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularFilaCarregamentoVeiculoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AceitarVinculoFilaCarregamentoVeiculoAutomaticamente", Column = "CPC_ACEITAR_VINCULO_FILA_CARREGAMENTO_VEICULO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AceitarVinculoFilaCarregamentoVeiculoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VincularPrePlanoSemValidarModeloVeicularCarga", Column = "CPC_VINCULAR_PRE_PLANO_SEM_VALIDAR_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularPrePlanoSemValidarModeloVeicularCarga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração de Pré Planejamento";
            }
        }
    }
}
