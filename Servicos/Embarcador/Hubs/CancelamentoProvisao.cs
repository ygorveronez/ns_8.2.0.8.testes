namespace Servicos.Embarcador.Hubs
{
	public class CancelamentoProvisao : HubBase<CancelamentoProvisao>
	{
		public void InformarCancelamentoProvisaoAtualizada(int codigoCancelamentoProvisao)
		{
			var retorno = new
			{
				CodigoCancelamentoProvisao = codigoCancelamentoProvisao
			};

			SendToAll("informarCancelamentoProvisaoAtualizada", retorno);
		}

		public void InformarQuantidadeDocumentosProcessadosFechamentoCancelamentoProvisao(int codigoCancelamentoProvisao, int quantidadeTotal, int quantidadeProcessada)
		{
			var retorno = new
			{
				CodigoCancelamentoProvisao = codigoCancelamentoProvisao,
				QuantidadeProcessada = quantidadeProcessada,
				QuantidadeTotal = quantidadeTotal
			};

			SendToAll("informarQuantidadeDocumentosProcessadosFechamentoCancelamentoProvisao", retorno);
		}
	}
}
