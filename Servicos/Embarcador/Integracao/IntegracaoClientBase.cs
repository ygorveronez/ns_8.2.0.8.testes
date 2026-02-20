using System;

namespace Servicos.Embarcador.Integracao
{
    public abstract class IntegracaoClientBase<TIntegracaoClient, TChannel> : IntegracaoBase
        where TChannel : class
        where TIntegracaoClient : System.ServiceModel.ClientBase<TChannel>, new()
    {
        #region Métodos Públicos Sobrescritos

        public override bool TestarDisponibilidade()
        {
            try
            {
                TIntegracaoClient wsIntegracaoClient = new TIntegracaoClient();

                try
                {
                    string url = wsIntegracaoClient.Endpoint.Address.Uri.ToString();

                    return TestarDisponibilidade(url);
                }
                finally
                {
                    wsIntegracaoClient.Close();
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                return false;
            }
        }

        #endregion
    }
}
