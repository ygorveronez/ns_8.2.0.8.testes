using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_PORTAL_MULTI_CLIFOR", EntityName = "ConfiguracaoPortalMultiClifor", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoPortalMultiClifor", NameType = typeof(ConfiguracaoPortalMultiClifor))]
    public class ConfiguracaoPortalMultiClifor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MostrarNoAcompanhamentoDePedidosDeslocamentoVazio", Column = "CPM_MOSTRAR_NO_ACOMPANHAMENTO_DE_PEDIDOS_DESLOCAMENTO_VAZIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MostrarNoAcompanhamentoDePedidosDeslocamentoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiltrarPedidosPorRemetenteRetiradaProduto", Column = "CPM_FILTRAR_PEDIDOS_POR_REMETENTE_RETIRADA_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarPedidosPorRemetenteRetiradaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoReportMenuBI", Column = "CPM_CODIGO_REPORT_MENU_BI", TypeType = typeof(int), NotNull = false)]
        public virtual int? CodigoReportMenuBI { get; set; }

        [Obsolete("Configuração descontinuada")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLScriptPortalMenu", Column = "CPM_URL_SCRIPT_PORTAL_MENU", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLScriptPortalMenu { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteVisualizacaoBI", Column = "CPM_SOMENTE_VISUALIZACAO_BI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomenteVisualizacaoBI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarAcessoTodosClientes", Column = "CPM_HABILITAR_ACESSO_TODOS_CLIENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarAcessoTodosClientes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaPadraoAcessoPortal", Column = "CPM_SENHA_PADRAO_ACESSO_PORTAL", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string SenhaPadraoAcessoPortal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesabilitarIconeNotificacao", Column = "CPM_DESABILITAR_ICONE_NOTIFICACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesabilitarIconeNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesabilitarFiltrosBI", Column = "CPM_DESABILITAR_FILTROS_BI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesabilitarFiltrosBI { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para o Portal MultiClifor"; }
        }
    }
}
