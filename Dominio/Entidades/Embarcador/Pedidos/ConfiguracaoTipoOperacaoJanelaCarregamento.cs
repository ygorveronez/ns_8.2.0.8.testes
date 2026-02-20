namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_JANELA_CARREGAMENTO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoJanelaCarregamento", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoJanelaCarregamento", NameType = typeof(ConfiguracaoTipoOperacaoJanelaCarregamento))]
    public class ConfiguracaoTipoOperacaoJanelaCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COJ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirRejeitarCargaJanelaCarregamentoTransportador", Column = "COJ_PERMITIR_REJEITAR_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRejeitarCargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores", Column = "COJ_PERMITIR_LIBERAR_CARGA_COM_TIPO_CONDICAO_PAGAMENTO_FOB_PARA_TRANSPORTADORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LayoutImpressaoOrdemColeta", Column = "COJ_LAYOUT_IMPRESSAO_ORDEM_COLETA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.LayoutImpressaoOrdemColeta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.LayoutImpressaoOrdemColeta LayoutImpressaoOrdemColeta { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações da janela de carregamento."; }
        }
    }
}