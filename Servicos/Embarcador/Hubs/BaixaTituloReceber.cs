namespace Servicos.Embarcador.Hubs
{
	public class BaixaTituloReceber : HubBase<BaixaTituloReceber>
	{
		public void InformarQuantidadeTitulosGerados(int codigoTituloBaixa, int quantidadeTotal, int quantidadeProcessada)
		{
			var retorno = new
			{
				CodigoTituloBaixa = codigoTituloBaixa,
				QuantidadeProcessada = quantidadeProcessada,
				QuantidadeTotal = quantidadeTotal
			};

			SendToAll("informarQuantidadeTitulosGerados", retorno);
		}

		public void InformarQuantidadeTitulosFinalizados(int codigoTituloBaixa, int quantidadeTotal, int quantidadeProcessada)
		{
			var retorno = new
			{
				CodigoTituloBaixa = codigoTituloBaixa,
				QuantidadeProcessada = quantidadeProcessada,
				QuantidadeTotal = quantidadeTotal
			};

			SendToAll("informarQuantidadeTitulosFinalizados", retorno);
		}

		public void InformarBaixaAtualizada(int codigoTituloBaixa)
		{
			var retorno = new
			{
				CodigoTituloBaixa = codigoTituloBaixa
			};

			SendToAll("informarBaixaAtualizada", retorno);
		}
	}
}
