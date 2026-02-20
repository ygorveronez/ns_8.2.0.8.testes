using System;
using System.Net;

namespace Servicos.Embarcador.Hubs
{
    public class Manobra : HubBase<Manobra>
    {
        #region Métodos Privados

        private void CriarNotificaoAlteracaoOutroAmbiente(string url)
        {
            try
            {
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

        #endregion

        #region Métodos Públicos

        public void CriarNotificaoManobraAlteradaOutroAmbiente(int codigoManobra, string urlBaseOrigemRequisicao)
        {
            if (!string.IsNullOrWhiteSpace(urlBaseOrigemRequisicao))
            {
                string url = $"{urlBaseOrigemRequisicao}/Manobra/DispararNotificacao?Manobra={codigoManobra}";

                CriarNotificaoAlteracaoOutroAmbiente(url);
            }
        }

        public void CriarNotificaoManobraTracaoAlteradaOutroAmbiente(int codigoManobra, string urlBaseOrigemRequisicao)
        {
            if (!string.IsNullOrWhiteSpace(urlBaseOrigemRequisicao))
            {
                string url = $"{urlBaseOrigemRequisicao}/ManobraTracao/DispararNotificacaoAlteracao?ManobraTracao={codigoManobra}";

                CriarNotificaoAlteracaoOutroAmbiente(url);
            }
        }

        public void CriarNotificaoManobraTracaoRemovidaOutroAmbiente(int codigoManobra, string urlBaseOrigemRequisicao)
        {
            if (!string.IsNullOrWhiteSpace(urlBaseOrigemRequisicao))
            {
                string url = $"{urlBaseOrigemRequisicao}/ManobraTracao/DispararNotificacaoRemocao?ManobraTracao={codigoManobra}";

                CriarNotificaoAlteracaoOutroAmbiente(url);
            }
        }

        public void NotificarTodosManobraAlterada(Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAlteracao alteracao)
        {
            SendToAll("informarManobraAlterada", alteracao);
		}

        public void NotificarTodosManobraTracaoAlterada(Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraTracaoDados manobraTracaoDados)
        {
			SendToAll("informarManobraTracaoAlterada", manobraTracaoDados);
        }

        public void NotificarTodosManobraTracaoRemovida(int codigoManobraTracao)
        {
			SendToAll("informarManobraTracaoRemovida", codigoManobraTracao);
        }

        #endregion
    }
}
