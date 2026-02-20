using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Net;

namespace Servicos.Embarcador.Integracao
{
    public abstract class IntegracaoBase
    {
        #region Construtores

        public static IntegracaoBase CriarIntegracaoTesteDisponibilidade(TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork) {
            switch (tipoIntegracao)
            {
                case TipoIntegracao.AngelLira:
                    return new AngelLira.IntegrarCargaAngelLira();

                case TipoIntegracao.SemParar:
                    return new SemParar.PracasPedagio();

                case TipoIntegracao.Carrefour:
                    return new Carrefour.IntegracaoCarrefour(unitOfWork);
            }

            return null;
        }

        #endregion

        #region Métodos Protegidos

        protected bool TestarDisponibilidade(string url)
        {
            SecurityProtocolType protocoloAnterior = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                return false;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = protocoloAnterior;
            }
        }

        #endregion

        #region Métodos Públicos Abstratos

        public abstract bool TestarDisponibilidade();

        #endregion
    }
}