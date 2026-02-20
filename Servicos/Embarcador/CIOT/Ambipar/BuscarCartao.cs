using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        public bool BuscarCartao(string numeroCartao, Repositorio.UnitOfWork unitOfWork, out int? idCartao, out string mensagemErro)
        {
            mensagemErro = null;
            bool sucesso = false;
            idCartao = null;

            try
            {
                this.ObterToken(out mensagemErro);
                if (string.IsNullOrWhiteSpace(this.token))
                    return false;

                #region Buscar Cartão

                string urlConsulta = $"{this.urlWebService}mso-cargo-gestaocartao-solicitacao/api/Cartao/ConsultarPorCartao?NumeroCartao={numeroCartao}";
                string jsonRetornoConsulta = "";

                HttpClient requisicaoConsulta = CriarRequisicao(urlConsulta);
                HttpResponseMessage retornoRequisicaoConsulta = requisicaoConsulta.GetAsync(urlConsulta).Result;
                jsonRetornoConsulta = retornoRequisicaoConsulta.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicaoConsulta.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(jsonRetornoConsulta))
                        throw new ServicoException($"Ocorreu uma falha ao consultar o cartão {numeroCartao}: {retornoRequisicaoConsulta.StatusCode}");
                    else
                        throw new ServicoException($"Ocorreu uma falha ao consultar o cartão {numeroCartao}: {jsonRetornoConsulta}");
                }

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retCartao retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retCartao>(jsonRetornoConsulta);
                idCartao = retornoConsulta?.id;

                if (idCartao != null)
                    sucesso = true;

                #endregion
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;
                sucesso = false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = $"Ocorreu uma falha ao buscar o cartão {numeroCartao}.";
                sucesso = false;
            }

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}