using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.QualP
{
    public class ValePedagio
    {
        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP ObterConfiguracaoQualP(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool registrarErro = true)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);

            bool possuiIntegracaoQualP = IsPossuiIntegracaoQualP(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP integracaoQualP = null;

            if (possuiIntegracaoQualP)
            {
                integracaoQualP = servicoValePedagio.ObterIntegracaoQualP(cargaConsultaValePedagio.Carga, tipoServicoMultisoftware);

                if (integracaoQualP == null)
                {
                    if (registrarErro)
                    {
                        cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                        cargaConsultaValePedagio.ProblemaIntegracao = "Integração QualP  não está configurado, por favor, entre em contato com a Multisoftware";
                        cargaConsultaValePedagio.NumeroTentativas++;
                        cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        repCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);

                        //setar carga como possui pendencia
                        cargaConsultaValePedagio.Carga.MotivoPendencia = "Falha ao Integrar consulta valor Vale Pedágio. Integração QualP não está configurado, por favor, entre em contato com a Multisoftware";
                        cargaConsultaValePedagio.Carga.PossuiPendencia = true;
                        cargaConsultaValePedagio.Carga.ProblemaIntegracaoValePedagio = true;
                        repCarga.Atualizar(cargaConsultaValePedagio.Carga);
                    }
                }
            }

            return integracaoQualP;
        }

        public decimal ConsultaValorPedagio(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP integracaoQualP, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao consultaCargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repConsultaValorPedagioIntegrecao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga Carga = consultaCargaValePedagio.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            if (cargaRotaFrete != null && !string.IsNullOrEmpty(cargaRotaFrete.PolilinhaRota))
            {
                decimal valorRecebido = 0;
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string polilinhaReduzida = ReduzirPolilinhaPorPontos(cargaRotaFrete.PolilinhaRota, integracaoQualP.DistanciaMinimaQuadrante);

                    string token = integracaoQualP.Token;
                    string endPoint = integracaoQualP.UrlIntegracao;

                    if (string.IsNullOrWhiteSpace(token)) throw new ServicoException("Integracao não possuí Token.");
                    string jsonRequest = string.Empty, jsonResponse = string.Empty;

                    endPoint = CriarUrlConexaoQualP(endPoint, token, polilinhaReduzida, consultaCargaValePedagio.QuantidadeEixos);

                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(ValePedagio));
                    client.BaseAddress = new Uri(endPoint);

                    // Headers
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

                    // Request
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    var result = client.PostAsync(endPoint, content).Result;

                    // Response
                    jsonResponse = result.Content.ReadAsStringAsync().Result;

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
                    cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);
                    cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;

                    if (result.IsSuccessStatusCode)
                    {
                        string retorno = result.Content.ReadAsStringAsync().Result;
                        dynamic retornoJSON = JsonConvert.DeserializeObject<dynamic>(retorno);
                        if (retornoJSON == null)
                        {
                            cargaValePedagioIntegracaoArquivo.Mensagem = "Falha ao Integrar consulta valor Pedágios.";
                            consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                        else
                        {
                            foreach (var pedagio in retornoJSON.pedagios)
                            {
                                dynamic tarifa = JsonConvert.DeserializeObject<dynamic>(pedagio.tarifa.ToString().Replace('"','\"'));

                                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.FromObject(tarifa);

                                // Converter o JObject em um dicionário
                                System.Collections.Generic.Dictionary<string, object> dictionary = jObject.ToObject<System.Collections.Generic.Dictionary<string, object>>();

                                var objValorTarifa = dictionary.ElementAt(0);
                                valorRecebido += decimal.Parse(objValorTarifa.Value?.ToString() ?? "0");
                            }
                            consultaCargaValePedagio.ValorValePedagio = valorRecebido;
                            consultaCargaValePedagio.ProblemaIntegracao = "";
                            consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            carga.ProblemaIntegracaoValePedagio = false;
                        }
                    }
                    else
                    {
                        cargaValePedagioIntegracaoArquivo.Mensagem = "Retorno integração QualP: " + result.StatusCode.ToString();
                        consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    if (string.IsNullOrWhiteSpace(cargaValePedagioIntegracaoArquivo.Mensagem))
                        cargaValePedagioIntegracaoArquivo.Mensagem = "Integrado com sucesso";
                    else
                        consultaCargaValePedagio.ProblemaIntegracao = cargaValePedagioIntegracaoArquivo.Mensagem;

                    cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
                    consultaCargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);

                    return valorRecebido;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "IntegracaoQualP");
                    consultaCargaValePedagio.ProblemaIntegracao = "Falha na consulta dos valores do pedágio da rota com a IntegracaoQualP";
                    consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    //setar carga como possui pendencia
                    carga.MotivoPendencia = "Falha ao Integrar consulta valor Pedágios. " + ex.Message;
                    carga.PossuiPendencia = true;
                    carga.ProblemaIntegracaoValePedagio = true;
                    repCarga.Atualizar(carga);
                    return 0;
                }
            }
            else
            {
                consultaCargaValePedagio.ProblemaIntegracao = "Carga não possui rota frete";
                consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                Carga.IntegrandoValePedagio = false;
                repCarga.Atualizar(Carga);
                return 0;
            }
        }

        #endregion

        #region Métodos Privados

        private string CriarUrlConexaoQualP(string endPoint, string token, string polilinhaReduzida, int quantidadeEixos)
        {
            string url = endPoint;
            url += "?";
            url += $"access-token={Uri.EscapeUriString(token)}&polilinha={Uri.EscapeUriString(polilinhaReduzida)}&precisao-polilinha=5&categoria=caminhao&tabela-frete=nao&eixos={quantidadeEixos}&format=json";
            return url;
        }

        private bool IsPossuiIntegracaoQualP(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            return repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.QualP) != null;
        }

        private string ReduzirPolilinhaPorPontos(string polilinha, int distanciaMinima)
        {
            if (distanciaMinima == 0)
                distanciaMinima = 5000;

            var listapontos = Logistica.Polilinha.ObterPontosPolilinha(polilinha, distanciaMinima);

            while (listapontos.Count > 150)
            {
                distanciaMinima += 1000;
                listapontos = Logistica.Polilinha.ObterPontosPolilinha(polilinha, distanciaMinima);

            }

            return Logistica.Polilinha.Codificar(listapontos);
        }

        #endregion
    }
}
