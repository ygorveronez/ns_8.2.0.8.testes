using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.Obramax
{
    public class IntegracaoObramax
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly string _urlAcessoCliente;

        #endregion Atributos

        #region Construtores

        public IntegracaoObramax(Repositorio.UnitOfWork unitOfWork, string clienteURL)
        {
            _unitOfWork = unitOfWork;
            _urlAcessoCliente = clienteURL;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaDadosTransporteIntegracao.DataIntegracao = System.DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoObramax repIntegracaoObramax = new Repositorio.Embarcador.Configuracoes.IntegracaoObramax(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax configuracaoIntegracao = repIntegracaoObramax.Buscar();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.CargaDadosTransporteIntegracao
                    dadosRequisicao = PreencherRequisicaoCargaDadosTransporte(cargaDadosTransporteIntegracao);

                HttpClient requisicaoTransportador = CriarRequisicao(configuracaoIntegracao.Token, configuracaoIntegracao.Endpoint);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracaoIntegracao.Endpoint, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;


                if (RetornoSucesso(retornoRequisicao))
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                    cargaDadosTransporteIntegracao.Protocolo = cargaDadosTransporteIntegracao.Carga.Codigo.ToString();
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else
                    throw new ServicoException($"Problema ao integrar com Obramax: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Obramax";
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, ExtensaoArquivo.JSON.ToString().ToLower());
            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        //Este método não tem conexão com a entidade Provisao/DocumentoProvisao
        public void IntegrarProvisao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            cargaCargaIntegracao.NumeroTentativas++;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxProvisao integracaoObramaxProvisao = ObterConfiguracaoIntegracaoProvisao();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.LoteProvisao loteProvisao = ObterProvisao(cargaCargaIntegracao.Carga.Pedidos);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key key = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key()
                {
                    Tipo = "JSON",
                    Dados = "Message id"
                };

                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RequestIntegracaoGenerico<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.LoteProvisao> dadosRequisicao = PreencherObjetoGenerico(loteProvisao, key);

                HttpClient requisicao = CriarRequisicao(integracaoObramaxProvisao.TokenObramaxProvisao, integracaoObramaxProvisao.EndpointObramaxProvisao);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(integracaoObramaxProvisao.EndpointObramaxProvisao, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Response resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Response>(jsonRetorno);

                if (!retornoRequisicao.IsSuccessStatusCode || resposta.ErrorCode == 400)
                    throw new ServicoException("Retorno API Obramax: " + resposta.Mensagem);

                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequisicao, jsonRetorno, ExtensaoArquivo.JSON.ToString().ToLower(), "Integrado com sucesso");

                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";

            }
            catch (ServicoException excecao)
            {
                cargaCargaIntegracao.ProblemaIntegracao = excecao.Message;
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
            }

            if (cargaCargaIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequisicao, jsonRetorno, ExtensaoArquivo.JSON.ToString().ToLower());

            repCargaDadosTransporteIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public void IntegrarCTeCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            IntegrarCTeCarga(cargaCargaIntegracao, cargaCargaIntegracao.Carga, cargaCargaIntegracao.Codigo, Dominio.Enumeradores.TipoDocumento.CTe);

            repositorioCargaCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public void IntegrarCTeCargaEnvioProgramado(Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);

            IntegrarCTeCarga(integracaoEnvioProgramado, integracaoEnvioProgramado.Carga, integracaoEnvioProgramado.Codigo, Dominio.Enumeradores.TipoDocumento.CTe);

            repositorioIntegracaoEnvioProgramado.Atualizar(integracaoEnvioProgramado);
        }

        public void IntegrarNFSeCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            IntegrarNFSeCarga(cargaCargaIntegracao, cargaCargaIntegracao.Carga, cargaCargaIntegracao.Codigo, Dominio.Enumeradores.TipoDocumento.NFSe);

            repositorioCargaCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public void IntegrarNFSeCargaEnvioProgramado(Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);

            IntegrarNFSeCarga(integracaoEnvioProgramado, integracaoEnvioProgramado.Carga, integracaoEnvioProgramado.Codigo, Dominio.Enumeradores.TipoDocumento.NFSe);

            repositorioIntegracaoEnvioProgramado.Atualizar(integracaoEnvioProgramado);
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrenciaPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(_unitOfWork);

            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = ExtensaoArquivo.JSON.ToString().ToLower(),
                conteudoResposta = string.Empty,
                extensaoResposta = ExtensaoArquivo.JSON.ToString().ToLower(),
                sucesso = false,
                mensagem = string.Empty
            };

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoObramax repIntegracaoObramax = new Repositorio.Embarcador.Configuracoes.IntegracaoObramax(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax configuracaoIntegracao = repIntegracaoObramax.Buscar();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.PedidoOcorrenciaIntegracao dadosRequisicao = PreencherRequisicaoPedidoOcorrencia(integracao);

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao.Token, configuracaoIntegracao.EndpointPedidoOcorrencia);
                httpRequisicaoResposta.conteudoRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(httpRequisicaoResposta.conteudoRequisicao.ToString(), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.EndpointPedidoOcorrencia, conteudoRequisicao).Result;
                httpRequisicaoResposta.conteudoResposta = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RetornoRequisicao jsonDeserializedResponse = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RetornoRequisicao>(httpRequisicaoResposta.conteudoResposta);
                if (string.IsNullOrEmpty(jsonDeserializedResponse.Offset))
                {
                    httpRequisicaoResposta.sucesso = false;
                    httpRequisicaoResposta.mensagem = "Falha na integração, retorno sem o campo 'offset'";
                }
                else if (RetornoSucesso(retornoRequisicao))
                {
                    httpRequisicaoResposta.sucesso = true;
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso";
                }
                else
                    throw new ServicoException($"Problema ao integrar com Obramax (Status: {retornoRequisicao.StatusCode})");
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                httpRequisicaoResposta.mensagem = "Problema ao tentar integrar com Obramax.";
            }

            repPedidoOcorrenciaIntegracao.Atualizar(integracao);
            return httpRequisicaoResposta;
        }

        public void IntegrarCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork);

            canhotoIntegracao.DataIntegracao = DateTime.Now;
            canhotoIntegracao.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoObramax repIntegracaoObramax = new Repositorio.Embarcador.Configuracoes.IntegracaoObramax(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax configuracaoIntegracao = repIntegracaoObramax.Buscar();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.PedidoOcorrenciaIntegracao dadosRequisicao = PreencherRequisicaoIntegracaoCanhoto(canhotoIntegracao, configuracaoIntegracao);

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao.Token, configuracaoIntegracao.EndpointPedidoOcorrencia);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings());
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.EndpointPedidoOcorrencia, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!RetornoSucesso(retornoRequisicao))
                    throw new ServicoException($"Problema ao integrar com Obramax (Status: {retornoRequisicao.StatusCode})");

                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                canhotoIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = "Problema ao tentar integrar com Obramax.";
            }
            finally
            {
                servicoArquivoTransacao.Adicionar(canhotoIntegracao, jsonRequisicao, jsonRetorno, ExtensaoArquivo.JSON.ToString().ToLower());
                repCanhotoIntegracao.Atualizar(canhotoIntegracao);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool RetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return (retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created);
        }

        private HttpClient CriarRequisicao(string apiKey, string endpoint)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoObramax));

            requisicao.BaseAddress = new Uri(endpoint);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Add("Content", "application/json");
            requisicao.DefaultRequestHeaders.Add("x-gateway-apikey", apiKey);

            return requisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.CargaDadosTransporteIntegracao PreencherRequisicaoCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Remessa> objetoRemessas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Remessa>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaIntegracao.Carga.Pedidos)
            {

                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Remessa remessa = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Remessa()
                {
                    RemessaCarga = cargaPedido.Pedido.NumeroPedidoEmbarcador ?? "",
                    Protocolo = cargaPedido.Pedido.Protocolo.ToString() ?? "",
                    ChaveNota = cargaPedido.Pedido.NotasFiscais?.FirstOrDefault()?.Chave ?? "",
                    NumeroPedido = cargaPedido.Pedido.CodigoPedidoCliente ?? ""
                };

                objetoRemessas.Add(remessa);
            }

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Transportadora> transportadoras = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Transportadora>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Transportadora transportadora = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Transportadora()
            {
                CodigoIntegracao = cargaIntegracao.Carga.Empresa?.CodigoIntegracao ?? "",
                RazaoSocial = cargaIntegracao.Carga.Empresa?.NomeFantasia ?? "",
            };

            transportadoras.Add(transportadora);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.DadosIntegracao dadosIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.DadosIntegracao()
            {
                OperacaoCarga = cargaIntegracao.Carga.TipoOperacao?.Descricao ?? "",
                NomeMotorista = cargaIntegracao.Carga.Motoristas?.FirstOrDefault()?.Nome ?? "",
                NumeroCarga = cargaIntegracao.Carga.CodigoCargaEmbarcador ?? "",
                Placa = cargaIntegracao.Carga.Veiculo?.Placa ?? "",
                ProtocoloCarga = cargaIntegracao.Carga.Protocolo.ToString() ?? "",
                Operation = string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo) ? "I" : "U",
                Remessa = objetoRemessas,
                Tipotransporte = cargaIntegracao.Carga.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? "",
                Tipoveiculo = cargaIntegracao.Carga.ModeloVeicularCarga?.Descricao ?? "",
                Transportadora = transportadoras
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key chave = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key()
            {
                Tipo = "JSON",
                Dados = "Message id"
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Value value = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Value()
            {
                Tipo = "JSON",
                Dados = dadosIntegracao
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.CargaDadosTransporteIntegracao()
            {
                Chave = chave,
                ValoresIntegracao = value
            };

            return cargaDadosTransporteIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.PedidoOcorrenciaIntegracao PreencherRequisicaoPedidoOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RemessaPedido remessaPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RemessaPedido()
            {
                Remessa = integracao.PedidoOcorrenciaColetaEntrega.Pedido?.NumeroPedidoEmbarcador ?? "",
                Protocolo = integracao.PedidoOcorrenciaColetaEntrega.Pedido?.Protocolo.ToString() ?? "",
                ChaveNota = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais?.FirstOrDefault()?.Chave ?? "",
                CpfCnpjCliente = integracao.PedidoOcorrenciaColetaEntrega.Pedido.Destinatario?.CPF_CNPJ_SemFormato ?? "",
                Pedido = integracao.PedidoOcorrenciaColetaEntrega.Pedido.CodigoPedidoCliente ?? "",
                NumeroNFe = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.FirstOrDefault()?.Numero.ToString() ?? "",
                Tracking = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.CodigoRastreamento, _urlAcessoCliente) ?? "",
                Filial = integracao.PedidoOcorrenciaColetaEntrega.Pedido.Filial?.CodigoFilialEmbarcador ?? ""
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.DadosIntegracaoPedido dadosIntegracaoPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.DadosIntegracaoPedido()
            {
                CodigoEvento = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.CodigoProceda?.ToInt() ?? 0,
                DataEvento = integracao.DataIntegracao.ToString("dd/MM/yyyy") ?? "",
                DescricaoOcorrencia = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.Descricao ?? "",
                Evento = integracao.PedidoOcorrenciaColetaEntrega?.Descricao ?? "",
                Hora = integracao.DataIntegracao.ToString("HH:mm"),
                ImagemCanhoto = string.Empty,//ObterImagemCanhoto(integracao.PedidoOcorrenciaColetaEntrega?.Pedido?.NotasFiscais?.FirstOrDefault()?.Canhoto, _unitOfWork) ?? "",
                ImagemEvento = string.Empty,//ObterImagemEvento(integracao) ?? "",
                Remessa = remessaPedido
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key key = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key()
            {
                Tipo = "JSON",
                Dados = "Message id"
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ValoresPedido valoresPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ValoresPedido()
            {
                Tipo = "JSON",
                DadosIntegracao = dadosIntegracaoPedido
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.PedidoOcorrenciaIntegracao pedidoOcorrenciaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.PedidoOcorrenciaIntegracao()
            {
                Chave = key,
                ValoresIntegracao = valoresPedido
            };

            return pedidoOcorrenciaIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.PedidoOcorrenciaIntegracao PreencherRequisicaoIntegracaoCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao integracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax configuracaoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RemessaPedido remessaPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RemessaPedido()
            {
                Remessa = integracao.Canhoto.Pedido?.NumeroPedidoEmbarcador ?? "",
                Protocolo = integracao.Canhoto.Pedido?.Protocolo.ToString() ?? "",
                ChaveNota = integracao.Canhoto.Pedido.NotasFiscais?.FirstOrDefault()?.Chave ?? "",
                Pedido = integracao.Canhoto.Pedido.CodigoPedidoCliente ?? "",
                NumeroNFe = integracao.Canhoto.Pedido.NotasFiscais.FirstOrDefault()?.Numero.ToString() ?? "",
                Tracking = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(integracao.Canhoto.Pedido.CodigoRastreamento, _urlAcessoCliente) ?? "",
                Filial = integracao.Canhoto.Pedido.Filial?.CodigoFilialEmbarcador ?? ""
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.DadosIntegracaoPedido dadosIntegracaoPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.DadosIntegracaoPedido()
            {
                CodigoEvento = configuracaoIntegracao.CodigoEventoCanhoto,
                DataEvento = integracao.DataIntegracao.ToString("dd/MM/yyyy") ?? "",
                DescricaoOcorrencia = "CANHOTO DIGITALIZADO",
                Evento = "Ocorrência de canhoto digitalizado",
                Hora = integracao.DataIntegracao.ToString("HH:mm"),
                ImagemCanhoto = null,
                ImagemEvento = ObterImagemEvento(integracao) ?? "",
                Remessa = remessaPedido
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key key = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key()
            {
                Tipo = "JSON",
                Dados = "Message id"
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ValoresPedido valoresPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ValoresPedido()
            {
                Tipo = "JSON",
                DadosIntegracao = dadosIntegracaoPedido
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.PedidoOcorrenciaIntegracao pedidoOcorrenciaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.PedidoOcorrenciaIntegracao()
            {
                Chave = key,
                ValoresIntegracao = valoresPedido
            };

            return pedidoOcorrenciaIntegracao;
        }

        private string ObterImagemCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {

            if (canhoto == null)
                return "";

            string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
            string extensao = Path.GetExtension(canhoto.NomeArquivo).ToLower();
            if (string.IsNullOrEmpty(extensao))
                return "";

            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                return "";

            byte[] bufferCanhoto = null;

            bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);

            string base64ImagemCanhoto = Convert.ToBase64String(bufferCanhoto);

            string imagemCanhoto = Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo) ? base64ImagemCanhoto : "";

            return imagemCanhoto;
        }

        private string ObterImagemEvento(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repOcorrenciaColetaEntregaAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(_unitOfWork);

            int codigoCargaEntrega = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Entregas?.FirstOrDefault()?.Codigo ?? 0;
            if (codigoCargaEntrega == 0)
                return "";

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrenciaColetaEntrega = repOcorrenciaColetaEntrega.BuscarPorCargaEntrega(codigoCargaEntrega);
            if (ocorrenciaColetaEntrega == null)
                return "";

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo> anexos = repOcorrenciaColetaEntregaAnexo.BuscarPorOcorrencia(ocorrenciaColetaEntrega.Codigo);
            if (anexos.Count == 0)
                return "";

            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencias" }), $"{anexos.FirstOrDefault().GuidArquivo}.{anexos.FirstOrDefault().ExtensaoArquivo}");
            byte[] bufferArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

            string base64ImagemEvento = Convert.ToBase64String(bufferArquivo);

            string imagemEvento = Utilidades.IO.FileStorageService.Storage.Exists(fileLocation) ? base64ImagemEvento : "";

            return imagemEvento;
        }

        private string ObterImagemEvento(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao integracao)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repOcorrenciaColetaEntregaAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(_unitOfWork);

            int codigoCargaEntrega = integracao.Canhoto.Carga?.Entregas?.FirstOrDefault()?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrenciaColetaEntrega = repOcorrenciaColetaEntrega.BuscarPorCargaEntrega(codigoCargaEntrega);

            if (ocorrenciaColetaEntrega == null)
                return "";

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo> anexos = repOcorrenciaColetaEntregaAnexo.BuscarPorOcorrencia(ocorrenciaColetaEntrega.Codigo);
            byte[] bufferArquivo = null;

            if (anexos.Count == 0)
                return "";

            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencias" }), $"{anexos.FirstOrDefault().GuidArquivo}.{anexos.FirstOrDefault().ExtensaoArquivo}");
            bufferArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

            string base64ImagemEvento = Convert.ToBase64String(bufferArquivo);

            string imagemEvento = Utilidades.IO.FileStorageService.Storage.Exists(fileLocation) ? base64ImagemEvento : "";

            return imagemEvento;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxCTE ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoObramaxCTE repositorioIntegracaoObramaxCTE = new Repositorio.Embarcador.Configuracoes.IntegracaoObramaxCTE(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxCTE configuracaoIntegracaoObramaxCTE = repositorioIntegracaoObramaxCTE.Buscar();

            if ((configuracaoIntegracaoObramaxCTE == null) || !configuracaoIntegracaoObramaxCTE.PossuiIntegracaoObramaxCTE)
                throw new ServicoException("Não existe configuração de integração disponível para a Obramax CTEs.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoObramaxCTE.EndpointObramaxCTE))
                throw new ServicoException("O Endpoint deve estar preenchido corretamente na configuração de integração da Obramax CTEs.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoObramaxCTE.TokenObramaxCTE))
                throw new ServicoException("Token deve estar preenchidos corretamente na configuração de integração da Obramax CTEs.");

            return configuracaoIntegracaoObramaxCTE;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxNFE ObterConfiguracaoIntegracaoNFSe()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoObramaxNFE repositorioIntegracaoObramaxNFE = new Repositorio.Embarcador.Configuracoes.IntegracaoObramaxNFE(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxNFE configuracaoIntegracaoObramaxNFE = repositorioIntegracaoObramaxNFE.Buscar();

            if ((configuracaoIntegracaoObramaxNFE == null) || !configuracaoIntegracaoObramaxNFE.PossuiIntegracaoObramaxNFE)
                throw new ServicoException("Não existe configuração de integração disponível para a Obramax NFes.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoObramaxNFE.EndpointObramaxNFE))
                throw new ServicoException("O Endpoint deve estar preenchido corretamente na configuração de integração da Obramax NFes.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoObramaxNFE.TokenObramaxNFE))
                throw new ServicoException("Token deve estar preenchidos corretamente na configuração de integração da Obramax NFes.");

            return configuracaoIntegracaoObramaxNFE;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxProvisao ObterConfiguracaoIntegracaoProvisao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoObramaxProvisao repositorioIntegracaoObramaxProvisao = new Repositorio.Embarcador.Configuracoes.IntegracaoObramaxProvisao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxProvisao configuracaoIntegracaoObramaxProvisao = repositorioIntegracaoObramaxProvisao.Buscar();

            if ((configuracaoIntegracaoObramaxProvisao == null) || !configuracaoIntegracaoObramaxProvisao.PossuiIntegracaoObramaxProvisao)
                throw new ServicoException("Não existe configuração de integração disponível para a Obramax NFes.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoObramaxProvisao.EndpointObramaxProvisao))
                throw new ServicoException("O Endpoint deve estar preenchido corretamente na configuração de integração da Obramax NFes.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoObramaxProvisao.TokenObramaxProvisao))
                throw new ServicoException("Token deve estar preenchidos corretamente na configuração de integração da Obramax NFes.");

            return configuracaoIntegracaoObramaxProvisao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.CTe ObterCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedidoCte.BuscarCargaPedidoPorCargaCTe(cargaCTe?.Codigo ?? 0);

            Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

            string xml = servicoCTe.ObterStringXMLAutorizacao(cargaCTe.CTe, _unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.CTe data = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.CTe()
            {
                ProtocoloIntegracaoCarga = cargaCTe.Carga?.Protocolo ?? 0,
                ProtocoloIntegracaoPedido = ObterProtocoloPedidosConcatenados(cargaPedidos),
                ProtocoloIntegracaoCTEs = cargaCTe.CTe?.Codigo.ToString() ?? string.Empty,
                ChaveCTe = cargaCTe.CTe?.Chave ?? string.Empty,
                XmlCTe = !string.IsNullOrWhiteSpace(xml) ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(xml)) : string.Empty,
            };

            return data;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ObramaxNFSe.NFSe ObterNFSe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaNFSe)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.XMLCTe repositorioXMLCTe = new Repositorio.XMLCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedidoCte.BuscarCargaPedidoPorCargaCTe(cargaNFSe.Codigo);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaNFSe.CTe;

            string xmlAutorizacao = repositorioXMLCTe.BuscarXMLPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ObramaxNFSe.NFSe nfse = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ObramaxNFSe.NFSe()
            {
                ProtocoloIntegracaoCarga = cargaNFSe.Carga.Protocolo,
                ProtocoloIntegracaoPedido = ObterProtocoloPedidosConcatenados(cargaPedidos),
                ProtocoloIntegracaoNFSe = cte.Codigo.ToString(),
                ChaveNFs = cte.Chave,
                XmlNFs = !string.IsNullOrWhiteSpace(xmlAutorizacao) ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(xmlAutorizacao)) : string.Empty,
                CnpjTransportador = cte.Empresa.CNPJ,
                NumeroNFS = cte.Numero.ToString(),
                SerieNFS = cte.RPS?.Serie.ToString() ?? cte.Serie.Numero.ToString(),
                ValorNFS = cte.ValorAReceber.ToString("F2", CultureInfo.InvariantCulture),
                DigitoVerificadorNFS = cte.Protocolo,
                DataEmissaoNFS = (cte.DataIntegracao ?? cte.DataEmissao)?.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            return nfse;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RequestIntegracaoGenerico<T> PreencherObjetoGenerico<T>(T objeto, Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key key) where T : class
        {
            var valoresIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ValueGenerico<T>()
            {
                Dados = objeto,
                Tipo = "JSON"
            };

            var request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RequestIntegracaoGenerico<T>()
            {
                ValoresIntegracao = valoresIntegracao,
                Chave = key
            };

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.LoteProvisao ObterProvisao(IEnumerable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.LoteProvisao loteProvisao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.LoteProvisao
            {
                ProtocoloCarga = cargaPedidos.FirstOrDefault().Carga?.CodigoCargaEmbarcador ?? string.Empty,
                Provisoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Provisao>()
            };

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {

                loteProvisao.Provisoes.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Provisao
                {
                    Remessa = cargaPedido.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                    ValorCalculado = cargaPedido.ValorFreteAPagar,
                    KMCalculado = cargaPedido.Pedido?.Distancia ?? 0
                });

            }

            return loteProvisao;
        }

        private void IntegrarNFSeCarga<T>(T integracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoIntegracao, Dominio.Enumeradores.TipoDocumento tipoDocumento) where T : Dominio.Entidades.Embarcador.Integracao.Integracao
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                integracao.NumeroTentativas++;
                integracao.DataIntegracao = DateTime.Now;

                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaNFe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxNFE integracaoObramaxNFE = ObterConfiguracaoIntegracaoNFSe();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasNFSe = repositorioCargaNFe.BuscarPorCargaEModeloDocumentoFiscal(carga.Codigo, tipoDocumento);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key key = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key()
                {
                    Tipo = "JSON",
                    Dados = codigoIntegracao.ToString()
                };

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaNFse in cargasNFSe)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ObramaxNFSe.NFSe nfse = ObterNFSe(cargaNFse);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RequestIntegracaoGenerico<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.ObramaxNFSe.NFSe> dadosRequisicao = PreencherObjetoGenerico(nfse, key);

                    HttpClient requisicao = CriarRequisicao(integracaoObramaxNFE.TokenObramaxNFE, integracaoObramaxNFE.EndpointObramaxNFE);
                    jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                    HttpResponseMessage retornoRequisicao = requisicao.PostAsync(integracaoObramaxNFE.EndpointObramaxNFE, conteudoRequisicao).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Response resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Response>(jsonRetorno);

                    if (!retornoRequisicao.IsSuccessStatusCode || resposta.ErrorCode == 400)
                        throw new ServicoException("Retorno API Obramax: " + resposta.Mensagem);

                    servicoArquivoTransacao.Adicionar((Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>)integracao, jsonRequisicao, jsonRetorno, "json", "Integrado com sucesso");
                }

                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                integracao.ProblemaIntegracao = excecao.Message;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Problema ao tentar integrar.";
            }

            if (integracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                servicoArquivoTransacao.Adicionar((Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>)integracao, jsonRequisicao, jsonRetorno, "json");
        }

        private void IntegrarCTeCarga<T>(T integracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoIntegracao, Dominio.Enumeradores.TipoDocumento tipoDocumento) where T : Dominio.Entidades.Embarcador.Integracao.Integracao
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                integracao.NumeroTentativas++;
                integracao.DataIntegracao = DateTime.Now;

                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxCTE integracaoObramaxCTE = ObterConfiguracaoIntegracao();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repositorioCargaCTe.BuscarPorCargaEModeloDocumentoFiscal(carga.Codigo, tipoDocumento);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key key = new Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Key()
                {
                    Tipo = "JSON",
                    Dados = codigoIntegracao.ToString()
                };

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.CTe cte = ObterCTe(cargaCTe);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.RequestIntegracaoGenerico<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.CTe> dadosRequisicao = PreencherObjetoGenerico(cte, key);

                    HttpClient requisicao = CriarRequisicao(integracaoObramaxCTE.TokenObramaxCTE, integracaoObramaxCTE.EndpointObramaxCTE);
                    jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                    HttpResponseMessage retornoRequisicao = requisicao.PostAsync(integracaoObramaxCTE.EndpointObramaxCTE, conteudoRequisicao).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Response resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax.Response>(jsonRetorno);

                    if (!retornoRequisicao.IsSuccessStatusCode || resposta.ErrorCode == 400)
                        throw new ServicoException("Retorno API Obramax: " + resposta.Mensagem);

                    servicoArquivoTransacao.Adicionar((Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>)integracao, jsonRequisicao, jsonRetorno, "json", "Integrado com sucesso");
                }

                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                integracao.ProblemaIntegracao = excecao.Message;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Problema ao tentar integrar.";
            }

            if (integracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                servicoArquivoTransacao.Adicionar((Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>)integracao, jsonRequisicao, jsonRetorno, "json");
        }
        private string ObterProtocoloPedidosConcatenados(ICollection<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            if (cargaPedidos == null)
                return string.Empty;

            return string.Join(",", cargaPedidos.Select(obj => obj.Pedido.Protocolo));
        }

        #endregion Métodos Privados
    }
}
