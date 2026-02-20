using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.Nstech
{
    public class IntegracaoSM : IntegracaoNSTech
    {
        #region Propriedades Privadas

        private static string _caminhoArquivosIntegracao;
        private static AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private static Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        //string endPointAutenticacao = "https://core.apipass.com.br/api/da228c56-6c11-4b5a-8c8d-90f37008263b/dev/autenticacao-grs";
        //string endPointSM = "https://core.apipass.com.br/api/da228c56-6c11-4b5a-8c8d-90f37008263b/dev/solicitacao_monitoramento-grs";

        //string idAutenticacao = "integration-squad";
        //string senhaAutenticacao = "659726b5-b99e-4c8f-9625-4502fcfc3cd7";

        public IntegracaoSM(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            _caminhoArquivosIntegracao = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Globais

        public void IntegrarSM(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoNSTech repIntegracaoNstech = new Repositorio.Embarcador.Configuracoes.IntegracaoNSTech(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech configuracaoIntegracao = repIntegracaoNstech.Buscar();

            try
            {
                if (!(ValidarConfiguracaoIntegracaoNSTech(cargaIntegracao, configuracaoIntegracao) && cargaIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado))
                    return;

                InspectorBehavior inspector = new InspectorBehavior();
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM ObjetorequisicaoSM = ObterObjetoRequisicaoSM(cargaIntegracao);
                ObjetorequisicaoSM.solicitante_id = configuracaoIntegracao.IDAutenticacao;
                ObjetorequisicaoSM.solicitante_token = base.ObterTokenIntegracaoNSTech(configuracaoIntegracao);

                HttpClient client = base.ObterClient(configuracaoIntegracao.UrlIntegracaoSM);
                string jsonRequest = JsonConvert.SerializeObject(ObjetorequisicaoSM, Formatting.Indented);

                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var jsonresult = client.PostAsync(configuracaoIntegracao.UrlIntegracaoSM, content).Result;

                string response = jsonresult.Content.ReadAsStringAsync().Result;

                if ((int)jsonresult.StatusCode == 308)
                {
                    string redirectUrl = jsonresult.Headers.Location.ToString();
                    content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                    HttpClient clientRedirect = ObterClient(redirectUrl);
                    jsonresult = clientRedirect.PostAsync(redirectUrl, content).Result;
                    response = jsonresult.Content.ReadAsStringAsync().Result;
                }

                string msgErro = "";
                if (jsonresult.IsSuccessStatusCode)
                {
                    dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(response);

                    if (cargaIntegracao.TipoIntegracao.Tipo == TipoIntegracao.LogRisk)
                    {
                        if (objetoRetorno?.body?.Body?.sgrGerarAEv13Response?.sgrGerarAEv13Result?.ReturnDataset?.diffgram?.sgrDS?.sgrTB?.CDVIAG != null)
                        {
                            cargaIntegracao.Protocolo = objetoRetorno.body.Body.sgrGerarAEv13Response.sgrGerarAEv13Result.ReturnDataset.diffgram.sgrDS.sgrTB.CDVIAG.Value;
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaIntegracao.ProblemaIntegracao = "Monitoramento inserido com sucesso - Protocolo " + cargaIntegracao.Protocolo + ".";
                            msgErro = "Monitoramento inserido com sucesso - Protocolo " + cargaIntegracao.Protocolo + ".";
                        }
                        else
                        {
                            if (objetoRetorno?.body?.Body?.sgrGerarAEv13Response?.sgrGerarAEv13Result?.ReturnDataset?.diffgram?.DSerros?.TBerros != null ?? false)
                            {
                                msgErro = "Retorno LogRisk: ";
                                msgErro += objetoRetorno?.body.Body.sgrGerarAEv13Response?.sgrGerarAEv13Result?.ReturnDescription?.Value ?? "";
                                dynamic TBerros = objetoRetorno?.body?.Body?.sgrGerarAEv13Response?.sgrGerarAEv13Result?.ReturnDataset?.diffgram?.DSerros?.TBerros ?? null;
                                if (TBerros != null)
                                {
                                    if (TBerros.GetType().IsSerializable)
                                    {
                                        foreach (dynamic erro in TBerros)
                                            msgErro += " - " + erro?.DSERRO ?? "Retorno vazio";
                                    }
                                    else
                                        msgErro += " - " + TBerros?.DSERRO ?? "Retorno vazio";
                                }
                            }

                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaIntegracao.ProblemaIntegracao = msgErro;
                        }
                    }
                    else
                    {
                        if (objetoRetorno?.body?.MonitoramentoId != null && objetoRetorno.body.MonitoramentoId.Value != "0")
                        {
                            cargaIntegracao.Protocolo = objetoRetorno.body.MonitoramentoId.Value;
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaIntegracao.ProblemaIntegracao = "Monitoramento inserido com sucesso - Protocolo " + cargaIntegracao.Protocolo + ".";
                            msgErro = "Monitoramento inserido com sucesso - Protocolo " + cargaIntegracao.Protocolo + ".";
                        }
                        else
                        {
                            if (objetoRetorno?.body?.Mensagem != null)
                                msgErro = objetoRetorno.body.Mensagem.Value;
                            else if (objetoRetorno?.msg != null)
                                msgErro = objetoRetorno.msg.Value;
                            else if (objetoRetorno?.body?.retorno != null && objetoRetorno?.body?.retorno[0]?.Aviso != null)
                                msgErro = objetoRetorno.body.retorno[0].Aviso.Value;
                            else if (objetoRetorno?.body?.Erro != null)
                                msgErro = objetoRetorno.body.Erro.Value;
                            else if (objetoRetorno?.body?.error != null)
                                msgErro = objetoRetorno.body.errormsg.msg.Value;

                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaIntegracao.ProblemaIntegracao = msgErro.Replace("\n", "").Replace("|", ";");
                        }
                    }
                }
                else
                {
                    msgErro = "Não foi possível obter o retorno: StatusCode " + jsonresult.StatusCode.ToString();

                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Não foi possível obter o retorno: StatusCode " + jsonresult.StatusCode.ToString();
                }

                AdicionarArquivosIntegracao(ref cargaIntegracao, response, jsonRequest, msgErro, _unitOfWork);
                repCargaIntegracao.Atualizar(cargaIntegracao);

            }
            catch (ServicoException ex)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = ex.Message;
                repCargaIntegracao.Atualizar(cargaIntegracao);

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao enviar a integração";
                repCargaIntegracao.Atualizar(cargaIntegracao);
            }
        }

        #endregion

        #region Métodos Privados

        private bool ValidarConfiguracaoIntegracaoNSTech(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech configuracaoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            if (configuracaoIntegracao == null || string.IsNullOrEmpty(configuracaoIntegracao?.UrlAutenticacao) || string.IsNullOrEmpty(configuracaoIntegracao?.UrlIntegracaoSM))
            {
                cargaIntegracao.NumeroTentativas++;
                cargaIntegracao.DataIntegracao = DateTime.Now;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível com a NSTech.";

                repCargaIntegracao.Atualizar(cargaIntegracao);

                return false;
            }

            return true;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM ObterObjetoRequisicaoSM(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM requisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM();

            //Para cada tipo integracao completar o objeto correspondente seguindo manual de/para da NSTech (https://plataformanstech.notion.site/SMON-Solicita-o-de-Monitoramento-73f0a6a70f8945d5914a529f998af442);

            switch (cargaIntegracao.TipoIntegracao.Tipo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk:
                    requisicao = Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.RetornarObjetoRequisicaoSM(cargaIntegracao, _unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LogRisk:
                    requisicao = Servicos.Embarcador.Integracao.LogRisk.IntegracaoLogRisk.RetornarObjetoRequisicaoSM(cargaIntegracao, _unitOfWork, _tipoServicoMultisoftware);
                    break;
                default:
                    break;

            }

            return requisicao;
        }

        private void AdicionarArquivosIntegracao(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string jsonRequest, string jsonResponse, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork, _caminhoArquivosIntegracao),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork, _caminhoArquivosIntegracao),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaIntegracao.ArquivosTransacao == null)
                cargaIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion
    }
}
