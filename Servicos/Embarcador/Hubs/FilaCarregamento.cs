using System;
using System.Net;

namespace Servicos.Embarcador.Hubs
{
	public class FilaCarregamento : HubBase<FilaCarregamento>
	{
		public void CriarNotificaoFilaAlteradaOutroAmbiente(int codigoHistoricoAlteracao, string urlBaseOrigemRequisicao)
		{
			if (!string.IsNullOrWhiteSpace(urlBaseOrigemRequisicao))
			{
				try
				{
					string url = $"{urlBaseOrigemRequisicao}/FilaCarregamento/DispararNotificacao?FilaCarregamentoVeiculo={codigoHistoricoAlteracao}";
					WebRequest requisicao = WebRequest.Create(url);

					requisicao.Method = "GET";

					WebResponse resposta = requisicao.GetResponse();

					resposta.Close();
				}
				catch (Exception excecao)
				{
					Log.TratarErro(excecao);
				}
			}
		}

		public void NotificarTodosFilaAlterada(Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao alteracao)
		{
			SendToAll("informarFilaAlterada", alteracao);
		}

		public void NotificarTodosSituacaoFilaAlterada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
		{
			var objetoRetornar = new
			{
				filaCarregamentoVeiculo.Codigo,
				CorLinha = filaCarregamentoVeiculo.ObterCorLinha()
			};

			SendToAll("informarSituacaoFilaAlterada", objetoRetornar);
		}
	}
}
