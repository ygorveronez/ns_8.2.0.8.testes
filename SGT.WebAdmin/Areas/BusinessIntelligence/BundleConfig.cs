using WebOptimizer;

namespace SGT.WebAdmin.Areas.BusinessIntelligence
{
	public static class BundleConfig
	{
		public static void RegisterBundlesBI(this IAssetPipeline pipeline)
		{
			#region Libs

			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/powerbi", "/Areas/BusinessIntelligence/js/powerbi.min.js").UseContentRoot();

			#endregion

			#region Global

			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/businessIntelligenceGlobal", "/Areas/BusinessIntelligence/ViewsScripts/BusinessIntelligence/Global/**/*.js").UseContentRoot();

			#endregion

			#region Cargas

			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/quantidadesCarga", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/Quantidade/**/*.js").UseContentRoot();
			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/direcionamentoOperador", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/DirecionamentoOperador/**/*.js").UseContentRoot();
			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/valorMedioFrete", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/ValorMedioFrete/**/*.js").UseContentRoot();
			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/indiceAtraso", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/IndiceAtraso/**/*.js").UseContentRoot();
			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/faturamentoTransportador", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/FaturamentoTransportador/**/*.js").UseContentRoot();
			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/quantidadePorRota", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/QuantidadePorRota/**/*.js").UseContentRoot();

			#endregion

			#region Chamados

			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/chamado", "/Areas/BusinessIntelligence/ViewsScripts/Chamados/Chamado/**/*.js").UseContentRoot();

			#endregion

			#region Pallets

			pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/quantidadePallets", "/Areas/BusinessIntelligence/ViewsScripts/Pallets/QuantidadePallets/**/*.js").UseContentRoot();

			#endregion
		}
	}
}