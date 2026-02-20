namespace Servicos.Embarcador.Hubs
{
	public class Fatura : HubBase<Fatura>
	{
		public void InformarQuantidadeDocumentosProcessadosCancelamento(int codigoFatura, int quantidadeTotal, int quantidadeProcessada)
		{
			var retorno = new
			{
				CodigoFatura = codigoFatura,
				QuantidadeProcessada = quantidadeProcessada,
				QuantidadeTotal = quantidadeTotal
			};

			SendToAll("informarQuantidadeDocumentosProcessadosCancelamento", retorno);
		}

		public void InformarQuantidadeDocumentosProcessadosFechamento(int codigoFatura, int quantidadeTotal, int quantidadeProcessada)
		{
			var retorno = new
			{
				CodigoFatura = codigoFatura,
				QuantidadeProcessada = quantidadeProcessada,
				QuantidadeTotal = quantidadeTotal
			};

			SendToAll("informarQuantidadeDocumentosProcessadosFechamento", retorno);
		}

		public void InformarFaturaAtualizada(int codigoFatura)
		{
			var retorno = new
			{
				CodigoFatura = codigoFatura
			};

			SendToAll("informarFaturaAtualizada", retorno);
		}
	}
}
