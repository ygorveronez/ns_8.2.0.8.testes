namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_GESTAO_DEVOLUCAO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoGestaoDevolucao", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoGestaoDevolucao", NameType = typeof(ConfiguracaoTipoOperacaoGestaoDevolucao))]
    public class ConfiguracaoTipoOperacaoGestaoDevolucao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGD_USAR_COMO_PADRAO_QUANDO_CARGA_FOR_DEVOLUCAO_DO_TIPO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações Tipo Operação para fluxo de Gestão de Devolução."; }
        }
    }
}