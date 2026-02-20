namespace Servicos.Embarcador.Hubs
{
	public class FluxoPatio : HubBase<FluxoPatio>
	{
		public void InformarPesagemInicialAtualizada(Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem)
		{
			var retorno = new
			{
				CodigoPesagem = pesagem.Codigo,
				PesagemInicial = pesagem.PesoInicial.ToString("n2")
			};

			SendToAll("informarPesagemInicialAtualizada", retorno);
		}

		public void InformarPesagemFinalAtualizada(Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem)
		{
			var retorno = new
			{
				CodigoPesagem = pesagem.Codigo,
				PesagemFinal = pesagem.PesoFinal.ToString("n2")
			};

			SendToAll("informarPesagemFinalAtualizada", retorno);
		}
	}
}
