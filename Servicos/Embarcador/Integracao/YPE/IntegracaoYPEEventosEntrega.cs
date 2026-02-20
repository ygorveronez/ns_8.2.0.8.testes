using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.YPE
{
    public class IntegracaoYPEEventosEntrega
    {
        #region Atributos
        private readonly Repositorio.UnitOfWork _unitOfWork;
        #endregion

        #region Constructores
        public IntegracaoYPEEventosEntrega(Repositorio.UnitOfWork unitOfWork) : base() { _unitOfWork = unitOfWork; }
        #endregion

        #region Metodos Publicos

        public HttpRequisicaoResposta IntegrarEventoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            Repositorio.Embarcador.Configuracoes.IntegracaoYPE repositorioIntegracaoYpe = new Repositorio.Embarcador.Configuracoes.IntegracaoYPE(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE configuracaoIntegracaoYPE = repositorioIntegracaoYpe.BuscarPrimeiroRegistro();

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoYPE?.URLIntegracaoOcorrencia))
                    throw new ServicoException("A integração de Ocorrência YPE não está configurada.");

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracaoYPE);

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repXmlNotaFiscal.BuscarPorCargaEntrega(cargaEntregaEvento.CargaEntrega.Codigo);

                if (notasFiscais.Count == 0)
                    throw new ServicoException($"Entrega {cargaEntregaEvento.CargaEntrega.Codigo} não possui nota fiscal para integrar.");

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in notasFiscais)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.YPE.EnvioOcorrenciaEventoEntrega evento = ObterDadosEnvioOcorrencia(xmlNotaFiscal, cargaEntregaEvento);
                    string jsonRequisicao = JsonConvert.SerializeObject(evento, Formatting.Indented);

                    StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracaoYPE.URLIntegracaoOcorrencia, conteudoRequisicao).Result;
                    string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                    httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;
                    httpRequisicaoResposta.conteudoResposta = jsonRetorno;

                    Log.TratarErro($"jsonRequisicao: {jsonRequisicao}", "IntegracaoYPEEventosEntrega");
                    Log.TratarErro($"jsonRetorno: {jsonRetorno}", "IntegracaoYPEEventosEntrega");
                    Log.TratarErro($"configuracaoIntegracaoYPE.URLIntegracaoOcorrencia: {configuracaoIntegracaoYPE.URLIntegracaoOcorrencia}", "IntegracaoYPEEventosEntrega");
                    Log.TratarErro($"retornoRequisicao.StatusCode: {retornoRequisicao.StatusCode}", "IntegracaoYPEEventosEntrega");

                    if (RetornoSucesso(retornoRequisicao))
                    {
                        httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                        httpRequisicaoResposta.sucesso = true;
                    }
                    else if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                        throw new ServicoException("Integração não autorizada, verifique o usuário e senha!");
                    else if (string.IsNullOrWhiteSpace(jsonRetorno))
                        throw new ServicoException("Retorno integração: " + retornoRequisicao.StatusCode);
                    else
                        throw new ServicoException("Retorno integração: " + jsonRetorno);
                }
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com SAP.";
            }

            return httpRequisicaoResposta;
        }

        #endregion

        #region Metodos Privados

        private HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE integracaoYPE)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoYPEEventosEntrega));

            requisicao.BaseAddress = new Uri(integracaoYPE.URLIntegracaoOcorrencia);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(integracaoYPE.Usuario, integracaoYPE.Senha);

            return requisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.YPE.EnvioOcorrenciaEventoEntrega ObterDadosEnvioOcorrencia(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.YPE.EnvioOcorrenciaEventoEntrega envioOcorrenciaEventoEntrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.YPE.EnvioOcorrenciaEventoEntrega();

            envioOcorrenciaEventoEntrega.zPorto.danfe = xmlNotaFiscal.Chave;
            envioOcorrenciaEventoEntrega.zPorto.cnpjemissaonf = xmlNotaFiscal.Emitente.CPF_CNPJ.ToString();
            envioOcorrenciaEventoEntrega.zPorto.serienf = xmlNotaFiscal.Serie;
            envioOcorrenciaEventoEntrega.zPorto.numeronf = xmlNotaFiscal.Numero.ToString();
            envioOcorrenciaEventoEntrega.zPorto.etapas = ObterEtapas(cargaEntregaEvento);

            return envioOcorrenciaEventoEntrega;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.YPE.Etapa> ObterEtapas(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento repositorioCargaEntregaEvento = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento> cargaEntregaEventosAnteriores = repositorioCargaEntregaEvento.BuscarPorCargaEntrega(cargaEntregaEvento.CargaEntrega.Codigo);

            if (cargaEntregaEvento.Codigo != cargaEntregaEventosAnteriores.LastOrDefault()?.Codigo)
                throw new ServicoException("Não é possível reenviar este evento, pois ele não é o último evento gerado. Por favor, verifique e tente novamente com o evento mais recente.");

            ValidarSeTodosEventosPossuemCodigoIntegracao(cargaEntregaEventosAnteriores);

            var eventosAgrupados = cargaEntregaEventosAnteriores
                .GroupBy(e => e.TipoDeOcorrencia.CodigoIntegracao)
                .Select(grupo => grupo.OrderByDescending(e => e.DataOcorrencia).First())
                .ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.YPE.Etapa> etapas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.YPE.Etapa>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento eventoFiltrado in eventosAgrupados)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.YPE.Etapa()
                {
                    CodigoEntrega = eventoFiltrado.TipoDeOcorrencia.CodigoIntegracao.ToString(),
                    TipoOcorrencia = eventoFiltrado.TipoDeOcorrencia.CodigoIntegracaoAuxiliar.ToString(),
                    DescricaoOcorrencia = eventoFiltrado.TipoDeOcorrencia.Descricao.ToString(),
                    Data = eventoFiltrado.DataOcorrencia.ToString("yyyy-MM-dd"),
                    Hora = eventoFiltrado.DataOcorrencia.ToString("HH:mm:ss"),
                    CodigoCarga = eventoFiltrado.Carga.Codigo.ToString(),
                    CodigoCD = eventoFiltrado.Carga.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                    NomePorto = eventoFiltrado.Carga.TipoTrecho?.Descricao ?? "Origem",
                    Status = eventoFiltrado.Codigo == cargaEntregaEvento.Codigo ? "ATIVO" : "CONCLUIDO"
                });
            }

            return etapas;
        }

        private bool RetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return retornoRequisicao.StatusCode == HttpStatusCode.Created || retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Accepted;
        }

        private void ValidarSeTodosEventosPossuemCodigoIntegracao(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento> cargaEntregaEventosAnteriores)
        {
            List<string> descricoesTipoOcorrencia = cargaEntregaEventosAnteriores
                .Where(e => string.IsNullOrEmpty(e.TipoDeOcorrencia.CodigoIntegracao) || string.IsNullOrEmpty(e.TipoDeOcorrencia.CodigoIntegracaoAuxiliar))
                .Select(e => e.TipoDeOcorrencia.Descricao)
                .Distinct()
                .ToList();

            if (descricoesTipoOcorrencia.Any())
                throw new ServicoException($"O(s) Tipo(s) de Ocorrência: {string.Join("; ", descricoesTipoOcorrencia)}, não possui(em) código de integração ou código de integração auxiliar preenchido (dados da etapa). Favor verificar!");

        }

        #endregion
    }
}
