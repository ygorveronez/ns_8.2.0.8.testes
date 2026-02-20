using System.Web.Optimization;

namespace AdminMultisoftwareApp
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Bibliotecas

            bundles.Add(new ScriptBundle("~/scripts/jquery").Include("~/js/libs/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/scripts/jqueryMbBrowser").Include("~/js/plugin/msie-fix/jquery.mb.browser.js"));
            bundles.Add(new ScriptBundle("~/scripts/jqueryTouch").Include("~/js/plugin/jquery-touch/jquery.ui.touch-punch.js"));
            bundles.Add(new ScriptBundle("~/scripts/jqueryUI").Include("~/js/libs/jquery-ui-{version}.js"));
            bundles.Add(new ScriptBundle("~/scripts/jqueryValidate").Include("~/js/plugin/jquery-validate/jquery.validate.js"));
            bundles.Add(new ScriptBundle("~/scripts/blockUI").Include("~/js/libs/jquery.blockui.js"));
            bundles.Add(new ScriptBundle("~/scripts/JarvisWidget").Include("~/js/smartwidgets/jarvis.widget.js"));
            bundles.Add(new ScriptBundle("~/scripts/fastClick").Include("~/js/plugin/fastclick/fastclick.js"));
            bundles.Add(new ScriptBundle("~/scripts/maskMoney").Include("~/js/libs/jquery.maskMoney.js"));
            bundles.Add(new ScriptBundle("~/scripts/fileDownload").Include("~/js/libs/jquery.filedownload.js"));
            bundles.Add(new ScriptBundle("~/scripts/twbsPagination").Include("~/js/libs/jquery.twbsPagination.js"));
            bundles.Add(new ScriptBundle("~/scripts/globalize").Include("~/js/libs/jquery.globalize.js").Include("~/js/libs/jquery.globalize.pt-BR.js"));
            bundles.Add(new ScriptBundle("~/scripts/dataTables").Include("~/js/plugin/datatables/jquery.dataTables.js").Include("~/js/plugin/datatables/dataTables.colReorder.js")
                                                                .Include("~/js/plugin/datatables/dataTables.bootstrap.js").Include("~/js/plugin/datatable-responsive/datatables.responsive.js"));
            bundles.Add(new ScriptBundle("~/scripts/smartNotification").Include("~/js/notification/SmartNotification.js"));
            bundles.Add(new ScriptBundle("~/scripts/knockout").Include("~/js/knockout/knockout-{version}.js"));
            bundles.Add(new ScriptBundle("~/scripts/app").Include("~/js/app.js"));
            bundles.Add(new ScriptBundle("~/scripts/appConfig").Include("~/js/app.config.js"));
            bundles.Add(new ScriptBundle("~/scripts/myAppConfig").Include("~/js/myapp.config.js"));
            bundles.Add(new ScriptBundle("~/scripts/bootstrap").Include("~/js/bootstrap/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/scripts/maskedInput").Include("~/js/plugin/masked-input/jquery.maskedinput.js"));
            bundles.Add(new ScriptBundle("~/scripts/smartChat").IncludeDirectory("~/js/smart-chat-ui", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/plupload").IncludeDirectory("~/js/plugin/plupload", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/dropzone").IncludeDirectory("~/js/plugin/dropzone", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/datetimepicker").Include("~/js/libs/bootstrap-datetimepicker.js"));
            bundles.Add(new ScriptBundle("~/scripts/moment").Include("~/js/libs/moment.js").Include("~/js/libs/pt-br.js"));
            bundles.Add(new ScriptBundle("~/scripts/signalR").Include("~/js/libs/jquery.signalR-{version}.js"));
            bundles.Add(new ScriptBundle("~/scripts/bootstrapSelect").Include("~/js/libs/bootstrap-select.js"));

            bundles.Add(new ScriptBundle("~/scripts/treeviewLoad").Include("~/js/libs/TreeViewLoad.js"));

            #endregion

            #region Configuracoes

            bundles.Add(new ScriptBundle("~/scripts/instanciaBase").IncludeDirectory("~/ViewsScripts/Configuracoes/InstanciaBase", "*.js"));

            #endregion

            #region Modulos

            bundles.Add(new ScriptBundle("~/scripts/modulo").IncludeDirectory("~/ViewsScripts/Modulos/Modulo", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/formulario").IncludeDirectory("~/ViewsScripts/Modulos/Formulario", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/permissaoPersonalizada").IncludeDirectory("~/ViewsScripts/Modulos/PermissaoPersonalizada", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/clienteModulo").IncludeDirectory("~/ViewsScripts/Modulos/ClienteModulo", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/clienteFormulario").IncludeDirectory("~/ViewsScripts/Modulos/ClienteFormularios", "*.js"));

            #endregion

            #region Notificacoes

            bundles.Add(new ScriptBundle("~/scripts/mensagemAviso").IncludeDirectory("~/ViewsScripts/Notificacoes/MensagemAviso", "*.js"));

            #endregion

            #region Usuarios

            bundles.Add(new ScriptBundle("~/scripts/usuario").Include("~/ViewsScripts/Usuarios/Usuario.js"));

            #endregion

            #region Pessoas

            bundles.Add(new ScriptBundle("~/scripts/clienteURLAcesso").IncludeDirectory("~/ViewsScripts/Pessoas/ClienteURLAcesso", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/cliente").IncludeDirectory("~/ViewsScripts/Pessoas/Cliente", "*.js"));

            #endregion

            #region Globais

            bundles.Add(new ScriptBundle("~/scripts/enumeradores").IncludeDirectory("~/ViewsScripts/Enumeradores", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/consultas").IncludeDirectory("~/ViewsScripts/Consultas", "*.js"));
            bundles.Add(new ScriptBundle("~/scripts/global").IncludeDirectory("~/js/Global", "*.js"));

            #endregion

            #region Styles

#if !DEBUG
            bundles.Add(new StyleBundle("~/css/production").Include("~/css/smartadmin-production.css"));
            bundles.Add(new StyleBundle("~/css/plugins").Include("~/css/smartadmin-production-plugins.css"));
            bundles.Add(new StyleBundle("~/css/skins").Include("~/css/smartadmin-skins.css"));
            bundles.Add(new StyleBundle("~/css/bootstrap").Include("~/css/bootstrap.css"));
            bundles.Add(new StyleBundle("~/css/fontAwesome").Include("~/css/font-awesome.css"));
            bundles.Add(new StyleBundle("~/css/smartAdminRTL").Include("~/css/smartadmin-rtl.css"));
            bundles.Add(new StyleBundle("~/css/bootstrap-datetimepicker").Include("~/css/bootstrap-datetimepicker.css"));
            bundles.Add(new StyleBundle("~/css/yourStyle").Include("~/css/your_style.css"));
            bundles.Add(new StyleBundle("~/css/bootstrapSelect").Include("~/css/bootstrap-select.css"));
#else
            bundles.Add(new StyleBundle("~/css/production").Include("~/css/smartadmin-production.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/plugins").Include("~/css/smartadmin-production-plugins.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/skins").Include("~/css/smartadmin-skins.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/bootstrap").Include("~/css/bootstrap.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/bootstrap-datetimepicker").Include("~/css/bootstrap-datetimepicker.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/fontAwesome").Include("~/css/font-awesome.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/smartAdminRTL").Include("~/css/smartadmin-rtl.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/yourStyle").Include("~/css/your_style.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/css/bootstrapSelect").Include("~/css/bootstrap-select.css", new CssRewriteUrlTransform()));
#endif

            #endregion
        }
    }
}