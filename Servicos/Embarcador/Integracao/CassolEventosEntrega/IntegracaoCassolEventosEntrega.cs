using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
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

namespace Servicos.Embarcador.Integracao.Cassol
{
    public class IntegracaoCassolEventosEntrega
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributo

        #region Construtores

        public IntegracaoCassolEventosEntrega(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public HttpRequisicaoResposta IntegrarEventoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao cargaEntregaEventoIntegracao)
        {
            List<Tuple<string, string>> integracoes = new();
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoCassol repConfiguracaoIntegracao = new(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCassol configuracaoIntegracao = repConfiguracaoIntegracao.BuscarPrimeiroRegistro();

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = cargaEntregaEventoIntegracao.CargaEntregaEvento;

                if (cargaEntregaEvento.CargaEntrega == null)
                    throw new ServicoException("Integração sem Carga ou Entrega vinculada.");

                if (cargaEntregaEvento.CargaEntrega.Pedidos == null || !cargaEntregaEvento.CargaEntrega.Pedidos.Any())
                    throw new ServicoException("Carga sem Pedido vinculados.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaEntregaEvento.CargaEntrega.Pedidos.Select(entregaPedido => entregaPedido.CargaPedido).ToList();


                httpRequisicaoResposta = EnviarIntegracaoEventosEntrega(ref integracoes, httpRequisicaoResposta, cargaEntregaEventoIntegracao.CargaEntregaEvento, cargaPedidos, configuracaoIntegracao);
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "IntegracaoCassolEventosEntrega");
                httpRequisicaoResposta.mensagem = excecao.Message;
                if (!string.IsNullOrEmpty(httpRequisicaoResposta.conteudoRequisicao))
                    integracoes.Add(new Tuple<string, string>(httpRequisicaoResposta.conteudoRequisicao, httpRequisicaoResposta.conteudoResposta));
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoCassolEventosEntrega");
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a Cassol.";
                if (!string.IsNullOrEmpty(httpRequisicaoResposta.conteudoRequisicao))
                    integracoes.Add(new Tuple<string, string>(httpRequisicaoResposta.conteudoRequisicao, httpRequisicaoResposta.conteudoResposta));
            }

            if (integracoes.Count > 0) integracoes.RemoveAt(integracoes.Count - 1);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            foreach (Tuple<string, string> integracao in integracoes)
                servicoArquivoTransacao.Adicionar(cargaEntregaEventoIntegracao, integracao.Item1, integracao.Item2, "json");

            return httpRequisicaoResposta;
        }

        public HttpRequisicaoResposta IntegrarEventoEntregaChamadoOcorrencia(Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao)
        {

            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta();
            List<Tuple<string, string>> integracoes = new();
            HttpClient requisicao = null;

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento repCargaEntregaEvento = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = repCargaEntregaEvento.BuscarUltimoPorCargaEntrega(chamadoIntegracao.Chamado.CargaEntrega.Codigo);
                if (cargaEntregaEvento == null)
                {
                    throw new ServicoException("Erro: Não foi possível encontrar um evento de entrega correspondente para a carga. " +
                        "A primeira etapa (evento de ocorrência) não foi integrada, impedindo a integração desta etapa.");
                }
                Repositorio.Embarcador.Configuracoes.IntegracaoCassol repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoCassol(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCassol configuracaoIntegracao = repConfiguracaoIntegracao.BuscarPrimeiroRegistro();

                if (cargaEntregaEvento.CargaEntrega == null)
                    throw new ServicoException("Integração sem Carga ou Entrega vinculada.");

                if (cargaEntregaEvento.CargaEntrega.Pedidos == null || !cargaEntregaEvento.CargaEntrega.Pedidos.Any())
                    throw new ServicoException("Carga sem Pedido vinculados.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaEntregaEvento.CargaEntrega.Pedidos.Select(entregaPedido => entregaPedido.CargaPedido).ToList();

                chamadoIntegracao.NumeroTentativas++;
                chamadoIntegracao.DataIntegracao = DateTime.Now;


                httpRequisicaoResposta = EnviarIntegracaoEventosEntrega(ref integracoes, httpRequisicaoResposta, cargaEntregaEvento, cargaPedidos, configuracaoIntegracao, chamadoIntegracao);

                if (!httpRequisicaoResposta.sucesso)
                {
                    chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    chamadoIntegracao.ProblemaIntegracao = httpRequisicaoResposta.mensagem;
                }
                else
                {
                    chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    chamadoIntegracao.ProblemaIntegracao = "";
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "IntegracaoCassolEventosEntrega");
                httpRequisicaoResposta.mensagem = excecao.Message;

                if (!string.IsNullOrEmpty(httpRequisicaoResposta.conteudoRequisicao))
                {
                    integracoes.Add(new Tuple<string, string>(
                        httpRequisicaoResposta.conteudoRequisicao,
                        httpRequisicaoResposta.conteudoResposta));
                }
                chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                chamadoIntegracao.ProblemaIntegracao = httpRequisicaoResposta.mensagem;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoCassolEventosEntrega");
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a Cassol.";

                if (!string.IsNullOrEmpty(httpRequisicaoResposta.conteudoRequisicao))
                {
                    integracoes.Add(new Tuple<string, string>(
                        httpRequisicaoResposta.conteudoRequisicao,
                        httpRequisicaoResposta.conteudoResposta));
                }
                chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                chamadoIntegracao.ProblemaIntegracao = httpRequisicaoResposta.mensagem;
            }
            finally
            {
                requisicao?.Dispose();
            }

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao =
                new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            foreach (Tuple<string, string> integracao in integracoes)
            {
                servicoArquivoTransacao.Adicionar(
                    chamadoIntegracao,
                    integracao.Item1,
                    integracao.Item2,
                    "json");
            }

            return httpRequisicaoResposta;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCassol configuracaoIntegracao, string endPoint)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCassolEventosEntrega));

            requisicao.BaseAddress = new Uri(endPoint);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuracaoIntegracao.Token}");

            return requisicao;
        }

        private dynamic ObterRequisicaoEventoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao? chamadoIntegracao = null)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestEventoEntrega retorno = new();

            retorno.DthrCriacao = cargaEntregaEvento.DataOcorrencia.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            if (chamadoIntegracao != null && chamadoIntegracao.Chamado != null)
            {
                retorno.Descricao = chamadoIntegracao.Chamado.TratativaDevolucao.ObterDescricaoTratativaDevolucao();
                retorno.TpPendencia = chamadoIntegracao.Chamado.TratativaDevolucao switch
                {
                    SituacaoEntrega.Rejeitado => "100",
                    SituacaoEntrega.Revertida => "101",
                    SituacaoEntrega.Reentergue => "102",
                    _ => cargaEntregaEvento.TipoDeOcorrencia.CodigoIntegracao
                };
                retorno.DthrCriacao = chamadoIntegracao.DataIntegracao.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }
            else
            {
                retorno.Descricao = cargaEntregaEvento.TipoDeOcorrencia.Descricao;
                retorno.TpPendencia = cargaEntregaEvento.TipoDeOcorrencia.CodigoIntegracao;
            }
            retorno.PedvenNumPedven = cargaPedido.Pedido.NumeroPedidoEmbarcador.ObterSomenteNumeros().ToLong();
            retorno.IdTmsCarga = cargaEntregaEvento.Carga.CodigoCargaEmbarcador.ObterSomenteNumeros().ToLong();
            retorno.TmsCargaCenterpoint = cargaEntregaEvento.Carga.Redespacho?.TipoRedespacho == TipoRedespacho.Redespacho || (cargaEntregaEvento.Carga.DadosSumarizados?.PossuiRedespacho ?? false);
            retorno.TmsCargaAgrupada = cargaEntregaEvento.Carga.CargaAgrupada;
            retorno.FilialSaida = cargaEntregaEvento.Carga.Filial.CodigoFilialEmbarcador.ToLong();
            retorno.EnderecoCodCli = cargaEntregaEvento.CargaEntrega.Cliente.CodigoIntegracao.ObterSomenteNumeros().ToLong();
            retorno.EnderecoCodEnd = cargaEntregaEvento.CargaEntrega.ClienteOutroEndereco == null ? 0 : cargaEntregaEvento.CargaEntrega.ClienteOutroEndereco.Cliente.CodigoIntegracao.ObterSomenteNumeros().ToLong();
            return retorno;
        }

        private static void ValidarIntegracaoEventosEntrega(HttpRequisicaoResposta httpRequisicaoResposta, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCassol configuracaoIntegracao)
        {
            if (configuracaoIntegracao == null)
                httpRequisicaoResposta.mensagem += "Integração Cassol sem dados de integração configurada. ";

            if (cargaEntregaEvento.CargaEntrega == null || cargaEntregaEvento.Carga == null)
                httpRequisicaoResposta.mensagem += "Integração sem Carga ou Entrega vinculada. ";
            else if (!cargaEntregaEvento.Carga.CodigoCargaEmbarcador.IsSomenteNumeros())
                httpRequisicaoResposta.mensagem += "Código de Integração da Carga não está no padrão aceito pela integração. Favor ajustar para somente números. ";

            if (cargaEntregaEvento.Carga.Filial == null)
                httpRequisicaoResposta.mensagem += "Carga sem Filial. ";
            else if (!cargaEntregaEvento.Carga.Filial.CodigoFilialEmbarcador.IsSomenteNumeros())
                httpRequisicaoResposta.mensagem += "Código de Integração da Filial não está no padrão aceito pela integração. Favor ajustar para somente números. ";

            if (cargaPedidos == null || cargaPedidos.Count == 0)
                httpRequisicaoResposta.mensagem += "Carga sem Pedido vinculados. ";
            else if (cargaPedidos.Any(cp => !cp.Pedido.NumeroPedidoEmbarcador.IsSomenteNumeros()))
                httpRequisicaoResposta.mensagem += "Código de Integração dos Pedidos não está no padrão aceito pela integração. Favor ajustar para somente números. ";

            if (cargaEntregaEvento.CargaEntrega.Cliente == null)
                httpRequisicaoResposta.mensagem += "Entrega sem Cliente definido. ";
            else if (!cargaEntregaEvento.CargaEntrega.Cliente.CodigoIntegracao.IsSomenteNumeros())
                httpRequisicaoResposta.mensagem += "Código de Integração do Cliente não está no padrão aceito pela integração. Favor ajustar para somente números. ";

            if (cargaEntregaEvento.TipoDeOcorrencia == null)
                httpRequisicaoResposta.mensagem += "Integração sem Tipo de Ocorrência vinculado. ";
        }

        private HttpRequisicaoResposta EnviarIntegracaoEventosEntrega(ref List<Tuple<string, string>> integracoes, HttpRequisicaoResposta httpRequisicaoResposta, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCassol configuracaoIntegracao, Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao? chamadoIntegracao = null)
        {
            ValidarIntegracaoEventosEntrega(httpRequisicaoResposta, cargaEntregaEvento, cargaPedidos, configuracaoIntegracao);

            if (!string.IsNullOrEmpty(httpRequisicaoResposta.mensagem))
                return httpRequisicaoResposta;


            string endPoint = $"{configuracaoIntegracao.URLIntegracao}/tms/v1/multi/carga/evento";
            HttpClient requisicao = CriarRequisicao(configuracaoIntegracao, endPoint);

            try
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    httpRequisicaoResposta.sucesso = false;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestEventoEntrega evento = ObterRequisicaoEventoEntrega(cargaEntregaEvento, cargaPedido, chamadoIntegracao);
                    string jsonRequisicao = JsonConvert.SerializeObject(evento, Formatting.Indented);
                    httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;

                    StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                    HttpResponseMessage retornoRequisicao = requisicao.PostAsync(endPoint, conteudoRequisicao).Result;
                    string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                    httpRequisicaoResposta.conteudoResposta = jsonRetorno;

                    if (retornoRequisicao.IsSuccessStatusCode)
                    {
                        httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                        httpRequisicaoResposta.sucesso = true;
                    }
                    else if (string.IsNullOrWhiteSpace(jsonRetorno))
                    {
                        httpRequisicaoResposta.mensagem = "Retorno integração: " + retornoRequisicao.StatusCode;
                    }
                    else if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.ResponseErroInterno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.ResponseErroInterno>(jsonRetorno);
                        httpRequisicaoResposta.mensagem = $"Retorno integração: {retorno.Title} {retorno.Detail}";
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.ResponseErroEventoEntrega retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.ResponseErroEventoEntrega>(jsonRetorno);
                        httpRequisicaoResposta.mensagem = retorno.Error.Mensagem + retorno.Error.Object;
                    }

                    integracoes.Add(new Tuple<string, string>(httpRequisicaoResposta.conteudoRequisicao, httpRequisicaoResposta.conteudoResposta));

                    if (!httpRequisicaoResposta.sucesso)
                        break;
                }
            }
            catch (Exception ex)
            {
                httpRequisicaoResposta.mensagem = "Erro ao enviar integração: " + ex.Message;
                httpRequisicaoResposta.sucesso = false;
            }
            finally
            {
                requisicao.Dispose();
            }

            return httpRequisicaoResposta;
        }

        #endregion Métodos Privados
    }
}
