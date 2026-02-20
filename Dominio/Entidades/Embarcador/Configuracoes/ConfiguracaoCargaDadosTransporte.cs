namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_CARGA_DADOS_TRANSPORTE", EntityName = "ConfiguracaoCargaDadosTransporte", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoCargaDadosTransporte", NameType = typeof(ConfiguracaoCargaDadosTransporte))]
    public class ConfiguracaoCargaDadosTransporte : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDT_RETORNAR_CARGA_PENDENTE_CONSULTA_CARREGAMENTO_AO_SALVAR_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDT_EXIGIR_QUE_VEICULO_CAVALO_TENHA_REBOQUE_VINCULADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirQueVeiculoCavaloTenhaReboqueVinculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDT_EXIGIR_PROTOCOLO_LIBERACAO_SEM_INTEGRACAO_GR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirProtocoloLiberacaoSemIntegracaoGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDT_EXIGIR_QUE_APOLICE_PROPRIA_TRANSPORTADOR_ESTEJA_VALIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirQueApolicePropriaTransportadorEstejaValida { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para dados transporte da carga"; }
        }
    }
}
