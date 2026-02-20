using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Italac;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Italac
{
    public class IntegracaoItalac
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoItalac(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Globais 

        public void IntegrarLoteLiberacaoComercialPedidoPendenteIntegracao(Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao integracaoPendente)
        {

            Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoItalac repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoItalac(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string mensagemErro = "Integrado com sucesso";

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalac configuracaoIntegracao = repositorioIntegracao.Buscar();

                if (configuracaoIntegracao == null)
                    throw new ServicoException("Configuração em REST da Italac não está completa, favor atualizar o cadastro da integração");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Login login = ObterLogin(configuracaoIntegracao.URLItalac, configuracaoIntegracao.UsuarioItalac, configuracaoIntegracao.SenhaItalac);

                string token = ObterToken(login);

                HttpClient requisicao = CriarRequisicao(token);

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas> requisicoes = PreencherRequisicao(integracaoPendente);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas pedido in requisicoes)
                {
                    respostaHttp = ExecutarRequisicao(pedido, requisicao, configuracaoIntegracao.URLItalac + "/AOMS141/LiberaPedidoVendas");

                    DadosRespostaItalac respostaItalac = JsonConvert.DeserializeObject<DadosRespostaItalac>(respostaHttp.conteudoResposta);

                    if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                    {
                        integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        integracaoPendente.LoteLiberacaoComercialPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteLiberacaoComercialPedido.Finalizado;
                        integracaoPendente.ProblemaIntegracao = respostaItalac.Mensagem;

                    }
                    else if (string.IsNullOrWhiteSpace(respostaItalac.Mensagem))
                    {
                        mensagemErro = "Houve um erro interno no servidor requisitado.";
                        servicoArquivoTransacao.Adicionar(integracaoPendente, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json", mensagemErro);
                        throw new ServicoException(mensagemErro);
                    }
                    else
                    {
                        mensagemErro = $"Pedido: {pedido.NumeroPedidoEmbarcador}, {respostaItalac.Mensagem}";
                        servicoArquivoTransacao.Adicionar(integracaoPendente, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json", mensagemErro);
                        throw new ServicoException(mensagemErro);
                    }

                    servicoArquivoTransacao.Adicionar(integracaoPendente, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json", mensagemErro);
                }
            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.LoteLiberacaoComercialPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteLiberacaoComercialPedido.FalhaNaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoItalac");

                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.LoteLiberacaoComercialPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteLiberacaoComercialPedido.FalhaNaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com Italac.";
            }

            repositorioLoteLiberacaoComercialPedidoIntegracao.Atualizar(integracaoPendente);
        }

        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoItalacFatura repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoItalacFatura(_unitOfWork);
            Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(_unitOfWork);
            faturaIntegracao.Tentativas++;
            faturaIntegracao.DataEnvio = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalacFatura configuracaoIntegracao = repositorioIntegracao.Buscar();

                if (configuracaoIntegracao == null)
                    throw new ServicoException("Configuração em REST da ItalacFaturas não está completa, favor atualizar o cadastro da integração");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Login login = ObterLogin(configuracaoIntegracao.URLItalacFatura, configuracaoIntegracao.UsuarioItalacFatura, configuracaoIntegracao.SenhaItalacFatura);

                string token = ObterToken(login);

                HttpClient requisicao = CriarRequisicao(token);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Fatura fatura = ObterFatura(faturaIntegracao.Fatura);

                respostaHttp = ExecutarRequisicao(fatura, requisicao, configuracaoIntegracao.URLItalacFatura + "/AOMS148/Fatura");

                if (respostaHttp.httpStatusCode != HttpStatusCode.Created)
                    throw new ServicoException("Problema ao obter a resposta da API Italac.");

                faturaIntegracao.MensagemRetorno = "Sucesso ao comunicar com API Italac.";
                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            }
            catch (ServicoException ex)
            {
                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "IntegracaoItalac");
                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Problema ao integrar com API Italac";
            }

            servicoFatura.SalvarArquivosIntegracaoFatura(faturaIntegracao, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta);

            repFaturaIntegracao.Atualizar(faturaIntegracao);
        }

        #endregion

        #region Métodos de Comunicação

        private string ObterToken(Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Login login)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)12288;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoItalac));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.Timeout = TimeSpan.FromMinutes(3);
            requisicao.DefaultRequestHeaders.Add("grant_type", "password");
            requisicao.DefaultRequestHeaders.Add("username", login.Usuario);
            requisicao.DefaultRequestHeaders.Add("password", login.Senha);

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync($"{login.URL}/api/oauth2/v1/token", null).Result;
            string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode == HttpStatusCode.Created)
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                return obj?.access_token;
            }
            else
            {
                throw new ServicoException("Não foi possivel gerar o Token.");
            }

        }

        private HttpClient CriarRequisicao(string token)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)12288;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoItalac));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.Timeout = TimeSpan.FromMinutes(3);
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return requisicao;
        }

        private HttpRequisicaoResposta ExecutarRequisicao<T>(T dadosRequisicao, HttpClient client, string url)
        {
            string jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented);

            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(url, content).Result;
            HttpRequisicaoResposta httpRequisicaoResposta = ObterHttRequisicaoResposta(jsonRequest, result);

            return httpRequisicaoResposta;
        }

        private static HttpRequisicaoResposta ObterHttRequisicaoResposta(string jsonRequest, HttpResponseMessage result)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = jsonRequest,
                extensaoRequisicao = "json",
                conteudoResposta = result.Content.ReadAsStringAsync().Result,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty,
                httpStatusCode = result.StatusCode
            };

            return httpRequisicaoResposta;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Login ObterLogin(string url, string usuario, string senha)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Login()
            {
                URL = url,
                Senha = senha,
                Usuario = usuario,
            };
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas> PreencherRequisicao(Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado repositorioLoteLiberacaoComercialPedidoBloqueado = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);


            List<int> codigosPedidos = repositorioLoteLiberacaoComercialPedidoBloqueado.BuscarLoteBloqueadoPedidosPorCodigo(integracaoPendente.LoteLiberacaoComercialPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCodigos(codigosPedidos);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas> pedidoBloqueados = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas>();


            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                pedidoBloqueados.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas
                {
                    CnpjRemetentePedido = pedido.Remetente.CPF_CNPJ_SemFormato,
                    NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador,
                    CpfUsuario = integracaoPendente.LoteLiberacaoComercialPedido.Usuario?.CPF

                });
            }

            return pedidoBloqueados;
        }

        Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Fatura ObterFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Fatura
            {
                NumeroFatura = fatura.Numero,
                Filial = fatura.Filial?.Codigo.ToString() ?? string.Empty,
                DataFatura = fatura.DataFatura.ToDateTimeString(showSeconds: true),
                ValorFatura = fatura.Total,
                ValorAcrescimo = fatura.Acrescimos,
                ValorDesconto = fatura.Descontos,
                Observacao = fatura.Observacao,
                Transportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Transportador()
                {
                    CPFCNPJ = fatura.Transportador?.CNPJ ?? string.Empty
                },
                Documentos = ObterDocumentosFatura(fatura)
            };
        }

        List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.DocumentoFatura> ObterDocumentosFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.DocumentoFatura> objetoIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.DocumentoFatura>();
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repositorioCTe.BuscarCTePorFatura(fatura.Codigo);

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                Dominio.Entidades.ModeloDocumentoFiscal modelo = cte.ModeloDocumentoFiscal;

                objetoIntegracao.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.DocumentoFatura
                {
                    TipoDocumento = modelo.Abreviacao ?? string.Empty,
                    NumeroDocumento = cte.Numero,
                    SerieDocumento = cte.Serie?.Descricao ?? string.Empty,
                    ValorTotalDocumento = cte.ValorAReceber,
                    ChaveCTe = cte.ChaveAcesso
                });
            }
            return objetoIntegracao;
        }

        #endregion
    }
}
