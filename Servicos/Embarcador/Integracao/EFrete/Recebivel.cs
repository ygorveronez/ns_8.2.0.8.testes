using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.EFrete
{
    public class Recebivel
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _urlAcessoCliente;

        #endregion

        public Recebivel(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURL)
        {
            _unitOfWork = unitOfWork;
            _urlAcessoCliente = clienteURL;
        }

        public void IntegrarCargaCte(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);

            cargaCTeIntegracaoPendente.DataIntegracao = DateTime.Now;
            cargaCTeIntegracaoPendente.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete repIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaointegracaoEFrete = repIntegracaoEFrete.Buscar();

                ValidarConfiguracoesIntegracao(configuracaointegracaoEFrete);

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe> dadosRequisicao = PreencherRequisicaoRecebivel(cargaCTeIntegracaoPendente);

                HttpClient requisicaoTransportador = CriarRequisicao(configuracaointegracaoEFrete.URLRecebivel, ObterToken(configuracaointegracaoEFrete));
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracaointegracaoEFrete.URLRecebivel, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    cargaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCTeIntegracaoPendente.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else
                    throw new ServicoException($"Problema ao integrar com e-Frete: {retornoRequisicao.StatusCode}");

            }
            catch (ServicoException excecao)
            {
                cargaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a e-Frete";
            }

            servicoArquivoTransacao.Adicionar(cargaCTeIntegracaoPendente, jsonRequisicao, jsonRetorno, "json");
            repCargaCte.Atualizar(cargaCTeIntegracaoPendente);
        }

        public void IntegrarCancelamentoCte(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> arquivoIntegracao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                cargaCancelamentoCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCTeIntegracao.NumeroTentativas++;

                Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete repIntegracaoEfrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete integracaoEFrete = repIntegracaoEfrete.Buscar();

                ValidarConfiguracoesIntegracao(integracaoEFrete);

                Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTeCancelamento dadosRequisicao = PreencherRequisicaoCancelamentoRecebivel(cargaCancelamentoCTeIntegracao);

                HttpClient requisicaoCancelamento = CriarRequisicao(integracaoEFrete.URLCancelamentoRecebivel, ObterToken(integracaoEFrete));

                var request = new HttpRequestMessage(HttpMethod.Delete, integracaoEFrete.URLCancelamentoRecebivel);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                request.Content = conteudoRequisicao;

                HttpResponseMessage retornoRequisicao = requisicaoCancelamento.SendAsync(request).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    cargaCancelamentoCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCancelamentoCTeIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NoContent)
                {
                    cargaCancelamentoCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCancelamentoCTeIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }

                else
                    throw new ServicoException($"Problema ao integrar com e-Frete: {retornoRequisicao.StatusCode}");

            }
            catch (ServicoException excecao)
            {
                cargaCancelamentoCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCTeIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCancelamentoCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a e-Frete";
            }

            arquivoIntegracao.Adicionar(cargaCancelamentoCTeIntegracao, jsonRequisicao, jsonRetorno, "json");
            repCargaCTeIntegracaoArquivo.Atualizar(cargaCancelamentoCTeIntegracao);
        }

        public void IntegrarPagamentos(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            pagamentoIntegracao.DataIntegracao = DateTime.Now;
            pagamentoIntegracao.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete repIntegracaoEfrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracoesIntegracaoEFrete = repIntegracaoEfrete.Buscar();
                ValidarConfiguracoesIntegracao(configuracoesIntegracaoEFrete);

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe> dadosRequisicao = PreencherRequisicaoPagamentos(pagamentoIntegracao);

                HttpClient requisicaoTransportador = CriarRequisicao(configuracoesIntegracaoEFrete.URLPagamentoRecebivel, ObterToken(configuracoesIntegracaoEFrete));
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracoesIntegracaoEFrete.URLPagamentoRecebivel, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;


                if (RetornoSucesso(retornoRequisicao))
                {
                    pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    pagamentoIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else
                    throw new ServicoException($"Problema ao integrar com e-Frete: {retornoRequisicao.StatusCode}");

            }
            catch (ServicoException excecao)
            {
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a e-Frete";
            }

            servicoArquivoTransacao.Adicionar(pagamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            repPagamentoIntegracao.Atualizar(pagamentoIntegracao);
        }

        public void IntegrarCargaCteOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);

            ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
            ocorrenciaCTeIntegracao.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete repIntegracaoGeralEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete integracaoGeralEFrete = repIntegracaoGeralEFrete.Buscar();

                ValidarConfiguracoesIntegracao(integracaoGeralEFrete);

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe> dadosRequisicao = PreencherRequisicaoOcorrencia(ocorrenciaCTeIntegracao);

                HttpClient requisicaoOcorrencia = CriarRequisicao(integracaoGeralEFrete.URLRecebivel, ObterToken(integracaoGeralEFrete));
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoOcorrencia.PostAsync(integracaoGeralEFrete.URLRecebivel, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    ocorrenciaCTeIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else
                    throw new ServicoException($"Problema ao integrar com e-Frete: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException excecao)
            {
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a e-Frete";
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracao, jsonRequisicao, jsonRetorno, "json");
            repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        public void IntegrarCancelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao ocorrenciaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo> arquivoIntegracao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                ocorrenciaCancelamentoIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaCancelamentoIntegracao.NumeroTentativas++;

                Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete repIntegracaoEfrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete integracaoEFrete = repIntegracaoEfrete.Buscar();

                ValidarConfiguracoesIntegracao(integracaoEFrete);

                Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTeCancelamento dadosRequisicao = PreencherRequisicaoCancelamentoOcorrencia(ocorrenciaCancelamentoIntegracao);

                HttpClient requisicaoCancelamento = CriarRequisicao(integracaoEFrete.URLCancelamentoRecebivel, ObterToken(integracaoEFrete));

                var request = new HttpRequestMessage(HttpMethod.Delete, integracaoEFrete.URLCancelamentoRecebivel);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                request.Content = conteudoRequisicao;

                HttpResponseMessage retornoRequisicao = requisicaoCancelamento.SendAsync(request).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NoContent)
                {
                    ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else
                    throw new ServicoException($"Problema ao integrar com e-Frete: {retornoRequisicao.StatusCode}");

            }
            catch (ServicoException excecao)
            {
                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a e-Frete";
            }

            arquivoIntegracao.Adicionar(ocorrenciaCancelamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            repCargaCTeIntegracaoArquivo.Atualizar(ocorrenciaCancelamentoIntegracao);
        }

        private bool RetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return (retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created);
        }

        private void ValidarConfiguracoesIntegracao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracao)
        {
            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoRecebivel)
                throw new ServicoException("Não existe configuração de integração de recebível disponível para e-Frete");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLRecebivel) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLRecebivel))
                throw new ServicoException("A URL do recebível não está configurada para a integração com e-Frete");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaRecebivel) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaRecebivel))
                throw new ServicoException("O usuário e senha devem estar preenchidos na configuração de Integração com e-Frete");
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe> PreencherRequisicaoRecebivel(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracaoPendente)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe> requestLista = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe request = new Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe()
            {
                IdentificadorCTe = cargaCTeIntegracaoPendente?.CargaCTe?.CTe?.Chave ?? "",
                CNPJEmpresa = cargaCTeIntegracaoPendente.CargaCTe?.CTe?.Empresa?.CNPJ ?? "",
                Empresa = cargaCTeIntegracaoPendente.CargaCTe.CTe?.Empresa?.NomeFantasia ?? "",
                DataPagamento = cargaCTeIntegracaoPendente.CargaCTe.CTe?.Titulo?.DataProgramacaoPagamento ?? DateTime.Now.AddDays(1),
                ValorTotal = cargaCTeIntegracaoPendente.CargaCTe.CTe.ValorAReceber,
                PercentualDeAntecipacao = 100,
                TomadorCNPJ = cargaCTeIntegracaoPendente.CargaCTe.CTe.TomadorPagador.CPF_CNPJ,
                TomadorNome = cargaCTeIntegracaoPendente.CargaCTe.CTe.Tomador.Nome ?? "",
                NumeroCTe = cargaCTeIntegracaoPendente.CargaCTe.CTe.Numero.ToString() ?? "",
                TipoDocumento = cargaCTeIntegracaoPendente.CargaCTe.CTe.ModeloDocumentoFiscal.Abreviacao,
                ChaveCTe = cargaCTeIntegracaoPendente.CargaCTe.CTe.Chave,
                DataEmissaoDocumento = cargaCTeIntegracaoPendente.CargaCTe.CTe?.DataEmissao,
                URLCliente = _urlAcessoCliente.URLAcesso
            };

            requestLista.Add(request);

            return requestLista;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe> PreencherRequisicaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe> requestLista = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe request = new Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe()
            {
                IdentificadorCTe = ocorrenciaCTeIntegracao?.CargaCTe?.CTe?.Chave ?? "",
                CNPJEmpresa = ocorrenciaCTeIntegracao.CargaCTe?.CTe?.Empresa?.CNPJ ?? "",
                Empresa = ocorrenciaCTeIntegracao.CargaCTe.CTe?.Empresa?.NomeFantasia ?? "",
                DataPagamento = ocorrenciaCTeIntegracao.CargaCTe.CTe?.Titulo?.DataProgramacaoPagamento ?? DateTime.Now.AddDays(1),
                ValorTotal = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorAReceber,
                PercentualDeAntecipacao = 100,
                TomadorCNPJ = ocorrenciaCTeIntegracao.CargaCTe.CTe.TomadorPagador.CPF_CNPJ,
                TomadorNome = ocorrenciaCTeIntegracao.CargaCTe.CTe.Tomador.Nome ?? "",
                NumeroCTe = ocorrenciaCTeIntegracao.CargaCTe.CTe.Numero.ToString() ?? "",
                TipoDocumento = ocorrenciaCTeIntegracao.CargaCTe.CTe.ModeloDocumentoFiscal.Abreviacao,
                ChaveCTe = ocorrenciaCTeIntegracao.CargaCTe.CTe.Chave,
                DataEmissaoDocumento = ocorrenciaCTeIntegracao.CargaCTe.CTe?.DataEmissao,
                URLCliente = _urlAcessoCliente.URLAcesso
            };

            requestLista.Add(request);

            return requestLista;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTeCancelamento PreencherRequisicaoCancelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao cancelamentoOcorrenciaCTeIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTeCancelamento request = new Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTeCancelamento()
            {
                Justificativa = cancelamentoOcorrenciaCTeIntegracao.OcorrenciaCancelamento?.MotivoCancelamento ?? "",
                NumeroCTe = cancelamentoOcorrenciaCTeIntegracao?.OcorrenciaCTeIntegracao?.CargaCTe?.CTe?.Chave ?? ""
            };

            return request;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe> PreencherRequisicaoPagamentos(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe> requestLista = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe request = new Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTe()
            {
                IdentificadorCTe = pagamentoIntegracao.DocumentoFaturamento.CTe.Chave ?? "",
                CNPJEmpresa = pagamentoIntegracao.DocumentoFaturamento?.CTe?.Empresa?.CNPJ ?? "",
                Empresa = pagamentoIntegracao.DocumentoFaturamento.CTe?.Empresa?.NomeFantasia ?? "",
                DataPagamento = pagamentoIntegracao.DocumentoFaturamento.CTe?.Titulo?.DataProgramacaoPagamento ?? DateTime.Now.AddDays(1),
                ValorTotal = pagamentoIntegracao.DocumentoFaturamento.CTe.ValorAReceber,
                PercentualDeAntecipacao = 100,
                TomadorCNPJ = pagamentoIntegracao.DocumentoFaturamento?.CTe?.TomadorPagador?.CPF_CNPJ,
                TomadorNome = pagamentoIntegracao.DocumentoFaturamento.CTe.Tomador.Nome ?? "",
                NumeroCTe = pagamentoIntegracao.DocumentoFaturamento.CTe.Numero.ToString() ?? "",
                TipoDocumento = pagamentoIntegracao.DocumentoFaturamento.CTe.ModeloDocumentoFiscal.Abreviacao,
                ChaveCTe = pagamentoIntegracao.DocumentoFaturamento.CTe.Chave,
                DataEmissaoDocumento = pagamentoIntegracao.DocumentoFaturamento.CTe?.DataEmissao,
                URLCliente = _urlAcessoCliente.URLAcesso
            };

            requestLista.Add(request);

            return requestLista;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTeCancelamento PreencherRequisicaoCancelamentoRecebivel(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCTeIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTeCancelamento request = new Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.CTeCancelamento()
            {
                Justificativa = cargaCancelamentoCTeIntegracao.CargaCancelamento?.MotivoCancelamento ?? "",
                NumeroCTe = cargaCancelamentoCTeIntegracao.CargaCTe.CTe?.Chave ?? ""
            };

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.Login PreencherRequisicaoLogin(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracao, string certificado)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.Login request = new Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.Login()
            {
                Usuario = configuracaoIntegracao.UsuarioRecebivel,
                Senha = Utilidades.Calc.OaepSHA512(configuracaoIntegracao.SenhaRecebivel, certificado)
            };

            return request;
        }

        private string ObterCertificado()
        {

            string directoryCert = Servicos.FS.GetPath("C:\\Cert_Multisoftware.cer");

            if (!Utilidades.IO.FileStorageService.Storage.Exists(directoryCert))
                throw new ServicoException("Caminho do certificado não encontrado");

            return directoryCert;

        }

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracao)
        {

            Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.Login dadosRequisicao = PreencherRequisicaoLogin(configuracaoIntegracao, ObterCertificado());

            HttpClient requisicao = CriarRequisicaoLogin(configuracaoIntegracao, configuracaoIntegracao.APIKey, configuracaoIntegracao.CodigoIntegracaoRecebivel);
            string jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLAutenticacao, conteudoRequisicao).Result;
            string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.Token resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.EFrete.Token>(jsonRetorno);

            return resposta.AcessToken;

        }

        private HttpClient CriarRequisicaoLogin(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete, string apiKey, string integrationIdentifier)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(Recebivel));

            requisicao.BaseAddress = new Uri(configuracaoIntegracaoEFrete.URLAutenticacao);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            requisicao.DefaultRequestHeaders.Add("api-key", apiKey);
            requisicao.DefaultRequestHeaders.Add("integration-identifier", integrationIdentifier);

            return requisicao;
        }

        private HttpClient CriarRequisicao(string url, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(Recebivel));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            return requisicao;
        }
    }
}
