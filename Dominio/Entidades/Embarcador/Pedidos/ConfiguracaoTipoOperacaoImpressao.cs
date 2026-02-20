namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_IMPRESSAO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoImpressao", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoImpressao", NameType = typeof(ConfiguracaoTipoOperacaoImpressao))]
    public class ConfiguracaoTipoOperacaoImpressao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_PERMITE_BAIXAR_COMPROVANTE_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteBaixarComprovanteColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_ENVIAR_PLANO_VIAGEM_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPlanoViagemTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_OCULTAR_QUANTIDADE_VALORES_ORDEM_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarQuantidadeValoresOrdemColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_IMPRIMIR_MINUTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirMinuta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_ALTERAR_LAYOUT_DA_FATURA_INCLUIR_TIPO_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterarLayoutDaFaturaIncluirTipoServico  { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Impressão"; }
        }
    }
}
