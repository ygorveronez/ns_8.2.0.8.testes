using System.Web.Optimization;

namespace EmissaoCTe.WebApp.App_Start
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts

            bundles.Add(new ScriptBundle("~/bundle/scripts/jquery").Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/jqueryUI").Include("~/Scripts/jquery-ui-{version}.custom.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/bootstrap").Include("~/Scripts/bootstrap.js").Include("~/Scripts/bootstrap.custom.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/bootbox").Include("~/Scripts/bootbox.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/datepicker").Include("~/Scripts/bootstrap-datepicker.js"));
            bundles.Add(new ScriptBundle("~/bundle/scripts/datetimepicker").Include("~/Scripts/moment.js", "~/Scripts/moment.pt-BR.js", "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/ajax").Include("~/Scripts/CTe.Ajax.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/baseConsultas").Include("~/Scripts/CTe.Base.Consultas.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/consulta").Include("~/Scripts/CTe.Consulta.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/gridview").Include("~/Scripts/CTe.GridView.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/mensagens").Include("~/Scripts/CTe.Mensagens.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/datatables").Include("~/Scripts/jquery.datatables.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/globalize").Include("~/Scripts/jquery.globalize.js",
                                                                        "~/Scripts/jquery.globalize.pt-BR.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/blockui").Include("~/Scripts/jquery.blockui.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/maskedinput").Include("~/Scripts/jquery.maskedinput.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/priceformat").Include("~/Scripts/jquery.priceformat.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/json").Include("~/Scripts/json2.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/validaCampos").Include("~/Scripts/validaCampos.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/fileDownload").Include("~/Scripts/jquery.filedownload.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/ckeditor").Include("~/Scripts/ckeditor/ckeditor.js"));
            bundles.Add(new ScriptBundle("~/bundle/scripts/ckeditoradapters").Include("~/Scripts/ckeditor/adapters/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/plupload").IncludeDirectory("~/Scripts/plupload", "*.js").Include("~/Scripts/CTe.Upload.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/cookie").Include("~/Scripts/jquery.cookie.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/smartMenu").Include("~/Scripts/jquery.smartmenus.js", "~/Scripts/jquery.smartmenus.bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/twbsPagination").Include("~/Scripts/jquery.twbsPagination.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/statecreator").Include("~/Scripts/stateCreator.js"));




            bundles.Add(new ScriptBundle("~/bundle/scripts/importacaoArquivos").IncludeDirectory("~/Scripts/ImportacaoArquivos", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/exportacaoDados").IncludeDirectory("~/Scripts/ExportacaoDados", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/configuracoesProposta").IncludeDirectory("~/Scripts/ConfiguracoesProposta", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/proposta").IncludeDirectory("~/Scripts/Proposta", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/importacaoctes").IncludeDirectory("~/Scripts/ImportacaoCTes", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/coletas").IncludeDirectory("~/Scripts/Coletas", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/minutaDevolucaoContainer").IncludeDirectory("~/Scripts/MinutaDevolucaoContainer", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/ordemDeCompra").IncludeDirectory("~/Scripts/OrdemDeCompra", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/cadastroArquivo").IncludeDirectory("~/Scripts/CadastroArquivo", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/ordemDeCompraMateriais").IncludeDirectory("~/Scripts/OrdemDeCompraMateriais", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/emissaoCTe").IncludeDirectory("~/Scripts/EmissaoCTe", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/apolicesdeseguros").IncludeDirectory("~/Scripts/ApolicesDeSeguros", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/configuracoesempresasemissoras").IncludeDirectory("~/Scripts/ConfiguracoesEmpresasEmissoras", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/emissaoMDFe").IncludeDirectory("~/Scripts/EmissaoMDFe", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/encerramentoMDFe").IncludeDirectory("~/Scripts/EncerramentoMDFe", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/emissaoNFSe").IncludeDirectory("~/Scripts/EmissaoNFSe", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/documentoEntrada").IncludeDirectory("~/Scripts/DocumentoEntrada", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/manutencaoPneusVeiculo").IncludeDirectory("~/Scripts/ManutencaoPneusVeiculo", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/motoristas").IncludeDirectory("~/Scripts/Motoristas", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/veiculos").IncludeDirectory("~/Scripts/Veiculos", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/duplicatas").IncludeDirectory("~/Scripts/Duplicatas", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/baixadeparcelasduplicatas").IncludeDirectory("~/Scripts/BaixaDeParcelasDuplicatas", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/freteSubcontratado").IncludeDirectory("~/Scripts/FreteSubcontratado", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/freteSubcontratadoFechamento").IncludeDirectory("~/Scripts/FreteSubcontratadoFechamento", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/produtoFornecedor").IncludeDirectory("~/Scripts/ProdutoFornecedor", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/relacaoCTesEntregues").IncludeDirectory("~/Scripts/RelacaoCTesEntregues", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/integracaoSigaFacil").IncludeDirectory("~/Scripts/IntegracaoSigaFacil", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/acertosDeViagens").IncludeDirectory("~/Scripts/AcertosDeViagens", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/LSTranslogIntegracoes").IncludeDirectory("~/Scripts/LSTranslogIntegracoes", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/destinadosCTes").IncludeDirectory("~/Scripts/DestinadosCTes", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/auditoria").IncludeDirectory("~/Scripts/Auditoria", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/calculoRelacaoCTesEntregues").IncludeDirectory("~/Scripts/CalculoRelacaoCTesEntregues", "*.js"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/pagamentoMotorista").IncludeDirectory("~/Scripts/PagamentoMotorista", "*.js"));

            #endregion

            #region Styles

            bundles.Add(new StyleBundle("~/styles/bootstrap").Include("~/Content/bootstrap.css"));
            bundles.Add(new StyleBundle("~/styles/site").Include("~/Styles/site.css"));
            bundles.Add(new StyleBundle("~/bundle/styles/multiplaSelecao").Include("~/Styles/multiplaSelecao.css"));
            bundles.Add(new StyleBundle("~/bundle/styles/datepicker").Include("~/Styles/bootstrap-datepicker.css"));
            bundles.Add(new StyleBundle("~/bundle/styles/datetimepicker").Include("~/Styles/bootstrap-datetimepicker.css"));
            bundles.Add(new StyleBundle("~/bundle/styles/plupload").Include("~/Styles/plupload/jquery.plupload.queue.css"));
            bundles.Add(new StyleBundle("~/bundle/styles/smartMenu").Include("~/Styles/jquery.smartmenus.bootstrap.css"));

            #endregion
        }
    }
}