namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_FILA_CARREGAMENTO", EntityName = "ConfiguracaoFilaCarregamento", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento", NameType = typeof(ConfiguracaoFilaCarregamento))]
    public class ConfiguracaoFilaCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_DIAS_FILTRAR_DATA_PROGRAMADA", TypeType = typeof(int), NotNull = true)]
        public virtual int DiasFiltrarDataProgramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_NAO_PERMITIR_ADICIONAR_VEICULO_EM_MAIS_DE_UMA_FILA_CARREGAMENTO_SIMULTANEAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAdicionarVeiculoEmMaisDeUmaFilaCarregamentoSimultaneamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_INFORMAR_AREA_CD_ADICIONAR_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarAreaCDAdicionarVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_PERMITE_AVANCAR_PRIMEIRA_ETAPA_CARGA_AO_ALOCAR_DADOS_TRANSPORTE_PELA_FILA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAvancarPrimeiraEtapaCargaAoAlocarDadosTransportePelaFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_ATUALIZAR_FILA_CARREGAMENTO_AO_ALTERAR_DADOS_TRANSPORTE_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarFilaCarregamentoAoAlterarDadosTransporteNaCarga { get; set; }
    }
}