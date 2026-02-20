namespace Servicos.Embarcador.Hubs
{
	public class Provisao : HubBase<Provisao>
	{
		public void InformarProvisaoAtualizada(int codigoProvisao)
		{
			var retorno = new
			{
				CodigoProvisao = codigoProvisao
			};

			SendToAll("informarProvisaoAtualizada", retorno);
		}

		public void InformarQuantidadeDocumentosProcessadosFechamentoProvisao(int codigoProvisao, int quantidadeTotal, int quantidadeProcessada)
		{
			var retorno = new
			{
				CodigoProvisao = codigoProvisao,
				QuantidadeProcessada = quantidadeProcessada,
				QuantidadeTotal = quantidadeTotal
			};

			SendToAll("informarQuantidadeDocumentosProcessadosFechamentoProvisao", retorno);
		}
	}
}
