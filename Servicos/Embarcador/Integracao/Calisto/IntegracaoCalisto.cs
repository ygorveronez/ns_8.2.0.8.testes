using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Calisto
{
    public class IntegracaoCalisto
    {

        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributo

        #region Construtores

        public IntegracaoCalisto(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCargasDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoCalisto repIntegracaoCalisto = new Repositorio.Embarcador.Configuracoes.IntegracaoCalisto(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto configuracaoIntegracao = repIntegracaoCalisto.Buscar();
                ValidarConfiguracoesIntegracao(configuracaoIntegracao);
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EnvioCarga> dadosRequisicao = PreencherObjetoEnvioCargaDadosTransporte(cargaDadosTransporteIntegracao);

                HttpClient requisicaoTransportador = CriarRequisicao(configuracaoIntegracao, configuracaoIntegracao.URL);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracaoIntegracao.URL, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                List<Response> responsesRetornoJson = JsonConvert.DeserializeObject<List<Response>>(jsonRetorno);

                foreach (Response response in responsesRetornoJson)
                {
                    if (!response.Sucesso)
                    {

                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = response.Menssagem ?? "";
                    }

                    else if (RetornoSucesso(retornoRequisicao) && response.Sucesso)
                    {
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                    }

                    else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                        throw new ServicoException("Requisição não encontrada");
                    else
                        throw new ServicoException($"Problema ao integrar com Calisto: {retornoRequisicao.StatusCode}");
                }
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar Integrar.";
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            cargaCargaIntegracao.NumeroTentativas++;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoCalisto repIntegracaoCalisto = new Repositorio.Embarcador.Configuracoes.IntegracaoCalisto(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto configuracaoIntegracao = repIntegracaoCalisto.Buscar();
                ValidarConfiguracoesIntegracao(configuracaoIntegracao);
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EnvioCarga> dadosRequisicao = PreencherObjetoEnvioCarga(cargaCargaIntegracao);

                HttpClient requisicaoTransportador = CriarRequisicao(configuracaoIntegracao, configuracaoIntegracao.URL);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracaoIntegracao.URL, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");
                else
                    throw new ServicoException($"Problema ao integrar com Calisto: {retornoRequisicao.StatusCode}");
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
                cargaCargaIntegracao.ProblemaIntegracao = "Problema ao tentar Integrar.";
            }

            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            repCargaDadosTransporteIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoCalisto repositorioIntegracaoCalisto = new Repositorio.Embarcador.Configuracoes.IntegracaoCalisto(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto configuracaoIntegracaoCalisto = repositorioIntegracaoCalisto.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            faturaIntegracao.DataEnvio = DateTime.Now;
            faturaIntegracao.Tentativas++;

            try
            {
                if (!(configuracaoIntegracaoCalisto?.PossuiIntegracao ?? false))
                    throw new ServicoException("Não possui configuração para Calisto.");

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.FaturaDetalhe> objetoCalistoFatura = ObterFatura(faturaIntegracao.Fatura, configuracaoIntegracaoCalisto.Usuario);

                string url = configuracaoIntegracaoCalisto.URLContabilizacao;
                HttpClient requisicao = CriarRequisicao(configuracaoIntegracaoCalisto, url);

                jsonRequisicao = JsonConvert.SerializeObject(objetoCalistoFatura, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                    JToken token = JToken.Parse(retorno.ToString());

                    if (token.Type == JTokenType.Array && Convert.ToBoolean(retorno[0].sucesso))
                    {
                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        faturaIntegracao.MensagemRetorno = "Fatura integrada com sucesso";
                    }
                    else
                    {
                        if (retorno.message != null)
                            throw new ServicoException($"{retorno.message}");
                        if (retorno.erro != null)
                            throw new ServicoException($"{retorno.erro}");
                    }
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException($"Falha na autenticação com a Calisto.");
                else
                    throw new ServicoException($"Falha ao conectar no WS Calisto: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Ocorreu uma falha ao realizar a integração da Loggi Faturas";
            }

            SalvarArquivosIntegracaoFatura(faturaIntegracao, jsonRequisicao, jsonRetorno);

            repositorioFaturaIntegracao.Atualizar(faturaIntegracao);
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo AdicionarArquivoTransacaoFatura(string arquivoRequisicao, string arquivoRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(arquivoRequisicao) && string.IsNullOrWhiteSpace(arquivoRetorno))
                return null;

            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repositorioFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioFaturaIntegracaoArquivo.Inserir(faturaIntegracaoArquivo);

            return faturaIntegracaoArquivo;
        }

        private HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto configuracaoIntegracao, string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCalisto));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

            return requisicao;
        }

        List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.FaturaDetalhe> ObterFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, string login)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.FaturaDetalhe> faturasDetalhe = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.FaturaDetalhe>();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisFatura = fatura.Documentos.Select(obj => obj.Documento.Filial).Distinct().ToList();
            foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in filiaisFatura)
            {
                List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentosPorFilial = fatura.Documentos.Where(obj => obj.Documento.Filial.Codigo == filial.Codigo).ToList();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.FaturaDetalhe faturaDetalhe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.FaturaDetalhe()
                {
                    CodigoBanco = 0,
                    CodigoCobranca = 1,
                    CodigoTipoOrdemCompra = 1,
                    CodigoOperacao = 4,
                    CodigoFilial = filial.CodigoFilialEmbarcador.ToInt(),
                    Login = login,
                    CodigoCompra = 1,
                    Observacao = $"Fatura de Transporte Período de {fatura.DataInicial?.ToString("d")} a {fatura.DataFinal?.ToString("d")}",
                    Encargos = 0,
                    Cep = fatura.Transportador?.CEP ?? "",
                    Bairro = fatura.Transportador?.Bairro ?? "",
                    Complemento = fatura.Transportador?.Complemento ?? "",
                    Contato = fatura.Transportador?.Contato ?? "",
                    CodigoAlternativo = "",
                    DataChegada = fatura.DataFatura.ToString("yyyy-MMM-dd HH:mm:ss.BRT"),
                    ValorDescontoTotal = documentosPorFilial.Sum(obj => obj.ValorDesconto),
                    SaldoFinanceiro = 0,
                    CodigoMoeda = 0,
                    Daf = fatura.DataFatura.ToString("yyyy-MMM-dd HH:mm:ss.BRT"),
                    CodigoMovCtb = documentosPorFilial
                         .FirstOrDefault(obj => obj.Documento?.CTe?.CargaCTes != null && obj.Documento.CTe.CargaCTes.Count > 0)
                         ?.Documento?.CTe?.CargaCTes[0]?.Carga?.TipoOperacao?.CodigoIntegracaoGerenciadoraRisco?.ToInt() ?? 0,
                    ValorBruto = documentosPorFilial.Sum(obj => obj.ValorTotalACobrar),
                    EntidadeCliente = ObterEntidadeCliente(fatura, login, filial),
                    EntidadeFornecedor = ObterEntidadeFornecedor(fatura, login, filial),
                    OrdemCompraItens = ObterOrdemCompraItem(documentosPorFilial, login),
                };
                faturasDetalhe.Add(faturaDetalhe);
            }

            return faturasDetalhe;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EnvioCarga> PreencherObjetoEnvioCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargasDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorCarga(cargasDadosTransporteIntegracao.Carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargasDadosTransporteIntegracao.Carga;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EnvioCarga> request = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EnvioCarga>();


            List<ListaPedidos> objetoPedido = new List<ListaPedidos>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
            {
                ListaPedidos listaPedidos = new ListaPedidos()
                {
                    CodigoPedido = cargaPedido.Pedido?.NumeroPedidoEmbarcador.ObterSomenteNumeros().ToInt() ?? 0
                };

                objetoPedido.Add(listaPedidos);
            }



            request.Add(new EnvioCarga()
            {
                CodigoFilial = cargasDadosTransporteIntegracao.Carga.Filial?.CodigoFilialEmbarcador.ObterSomenteNumeros().ToInt() ?? 0,
                CodigoTransportador = cargasDadosTransporteIntegracao.Carga.Empresa?.CodigoIntegracao.ObterSomenteNumeros().ToInt() ?? 0,
                NumeroCarga = cargasDadosTransporteIntegracao.Carga?.CodigoCargaEmbarcador ?? "",
                ListaPedidos = objetoPedido
            });

            return request;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EnvioCarga> PreencherObjetoEnvioCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCargaIntegracao.Carga;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EnvioCarga> request = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EnvioCarga>();


            List<ListaPedidos> objetoPedido = new List<ListaPedidos>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
            {
                ListaPedidos listaPedidos = new ListaPedidos()
                {
                    CodigoPedido = cargaPedido.Pedido?.NumeroPedidoEmbarcador.ObterSomenteNumeros().ToInt() ?? 0
                };

                objetoPedido.Add(listaPedidos);
            }



            request.Add(new EnvioCarga()
            {
                CodigoFilial = cargaCargaIntegracao.Carga.Filial?.CodigoFilialEmbarcador.ObterSomenteNumeros().ToInt() ?? 0,
                CodigoTransportador = cargaCargaIntegracao.Carga.Empresa?.CodigoIntegracao.ObterSomenteNumeros().ToInt() ?? 0,
                NumeroCarga = cargaCargaIntegracao.Carga?.CodigoCargaEmbarcador ?? "",
                ListaPedidos = objetoPedido
            });

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EntidadeCliente ObterEntidadeCliente(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, string login, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EntidadeCliente()
            {
                CodigoEntidade = fatura.ClienteTomadorFatura.CodigoIntegracao,
                Nome = fatura.ClienteTomadorFatura.Nome,
                TipoPessoa = fatura.ClienteTomadorFatura.Tipo,
                CpfCnpj = fatura.ClienteTomadorFatura.CPF_CNPJ_SemFormato,
                Fone1 = fatura.ClienteTomadorFatura.Telefone1,
                Login = login,
                CdFilial = filial.CodigoFilialEmbarcador.ToInt()
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EntidadeFornecedor ObterEntidadeFornecedor(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, string login, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.EntidadeFornecedor()
            {
                CodigoEntidade = fatura.Transportador.CodigoIntegracao,
                Nome = fatura.Transportador.RazaoSocial,
                TipoPessoa = fatura.Transportador.Tipo,
                CpfCnpj = fatura.Transportador.CNPJ,
                Fone1 = fatura.Transportador.Telefone,
                Login = login,
                CdFilial = filial.CodigoFilialEmbarcador.ToInt()
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.OrdemCompraItem> ObterOrdemCompraItem(List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> DocumentosPorFilial, string login)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            List<OrdemCompraItem> ordemCompraItens = new List<OrdemCompraItem> { };

            foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento in DocumentosPorFilial)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = faturaDocumento.Documento.CTe;

                foreach (string numeroPedido in faturaDocumento.Documento.NumeroPedidoCliente)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorNumeroEmbarcador(numeroPedido);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.OrdemCompraItem ordemCompraItem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.OrdemCompraItem()
                    {
                        CodigoDepartamento = pedido.PedidoCentroCusto?.Codigo ?? 0,
                        DataPrevista = cte.DataEmissao?.ToString("yyyy-MMM-dd HH:mm:ss.BRT") ?? DateTime.MinValue.ToString("yyyy-MMM-dd HH:mm:ss.BRT"),
                        Quantidade = 1,
                        Saldo = 1,
                        DataEntrega = cte.DataEmissao?.ToString("yyyy-MMM-dd HH:mm:ss.BRT") ?? DateTime.MinValue.ToString("yyyy-MMM-dd HH:mm:ss.BRT"),
                        Login = login,
                        CodigoFilial = faturaDocumento.Documento.Filial.CodigoFilialEmbarcador.ToInt(),
                        Espec = $"Pagamento CTE numero {cte.Numero}",
                        CdGrupoCtb = 0,
                        Preco = cte.ValorAReceber,
                        Fcoberturacambial = 1,
                        Produto = ObterProduto(faturaDocumento),
                    };

                    ordemCompraItens.Add(ordemCompraItem);
                }
            }

            return ordemCompraItens;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.Produto ObterProduto(Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento)
        {

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto.Produto()
            {
                CodigoAlternativo = "MAT017",
                Unidade = "1",
                Nome = "FRETES E CARRETOS",
                DataAlteracao = faturaDocumento.Documento.CTe.DataEmissao?.ToString("yyyy-MMM-dd HH:mm:ss.BRT") ?? DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss.BRT"),
                CodigoClasse = "1",
                CodigoGrupo = "1",
                CodigoSubGrupo = "1",
            };
        }

        private void SalvarArquivosIntegracaoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string arquivoRequisicao, string arquivoRetorno)
        {
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = AdicionarArquivoTransacaoFatura(arquivoRequisicao, arquivoRetorno, faturaIntegracao.MensagemRetorno);

            if (faturaIntegracaoArquivo == null)
                return;

            if (faturaIntegracao.ArquivosIntegracao == null)
                faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();

            faturaIntegracao.ArquivosIntegracao.Add(faturaIntegracaoArquivo);
        }

        private bool RetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return (retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created);
        }

        private void ValidarConfiguracoesIntegracao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto configuracaoIntegracao)
        {
            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para Calisto");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URL) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URL))
                throw new ServicoException("A URL não está configurada para a integração com Calisto");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.Usuario) || string.IsNullOrWhiteSpace(configuracaoIntegracao.Senha))
                throw new ServicoException("O usuário e senha devem estar preenchidos na configuração de Integração com Calisto");
        }

        #endregion
    }
}
