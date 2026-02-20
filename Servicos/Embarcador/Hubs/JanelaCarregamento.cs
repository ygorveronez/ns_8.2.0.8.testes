using System;
using System.Net;

namespace Servicos.Embarcador.Hubs
{
    public class JanelaCarregamento : HubBase<JanelaCarregamento>
    {
        public void CriarNotificaoJanelaCarregamentoAtualizadaOutroAmbiente(int codigoCargaJanelaCarregamento, string urlBaseOrigemRequisicao)
        {
            if (string.IsNullOrWhiteSpace(urlBaseOrigemRequisicao))
                return;
            
            try
            {
                string url = $"{urlBaseOrigemRequisicao}/JanelaCarregamento/DispararNotificacao?JanelaCargaAtualizada={codigoCargaJanelaCarregamento}";
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

        public void InformarJanelaCarregamentoAtualizada(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            var retorno = new
            {
                Codigo = cargaJanelaCarregamento.Codigo,
                CodigoCentroCarregamento = cargaJanelaCarregamento.CentroCarregamento?.Codigo ?? 0,
                DataCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy")
            };

			SendToAll("informarJanelaCarregamentoAlterada", retorno);
        }
    }
}
