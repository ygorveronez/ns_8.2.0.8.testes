using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Senior
{
    public class IntegracaoSenior
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSenior _configuracaoIntegracao;
        private string _jsonRequisicao = string.Empty;
        private string _jsonRetorno = string.Empty;

        #endregion

        #region Construtores

        public IntegracaoSenior(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public bool ValidarEtapaIntegracao(EtapaFluxoGestaoPatio etapa)
        {
            if (etapa != EtapaFluxoGestaoPatio.SolicitacaoVeiculo)
                return false;

            GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new GestaoPatio.FluxoGestaoPatioConfiguracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();

            if (configuracaoGestaoPatio.SolicitacaoVeiculoTipoIntegracao != TipoIntegracao.Senior)
                return false;

            Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow repositorioIntegracaoBoticarioFreeFlow = new Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow configIntegracaoBoticarioFreeFlow = repositorioIntegracaoBoticarioFreeFlow.Buscar();

            if (!(configIntegracaoBoticarioFreeFlow?.PossuiIntegracao ?? false))
                return false;

            return true;
        }

        public void Integrar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao fluxoPatioIntegrao)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            try
            {
                fluxoPatioIntegrao.NumeroTentativas++;
                fluxoPatioIntegrao.DataIntegracao = DateTime.Now;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao configIntegracaoSenior = ObterConfiguracaoSenior();

                jsonRequest = ObterRequestIntegracao(fluxoPatioIntegrao.Carga);

                HttpClient cliente = ObterHttpClient(configIntegracaoSenior);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = cliente.PostAsync(configIntegracaoSenior.UrlRequisicao, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    string message = $"Falha ao integrar - Status ({retorno.status}) - {retorno?.fault?.faultstring ?? retorno?.message}";
                    fluxoPatioIntegrao.ProblemaIntegracao = message;
                    fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    fluxoPatioIntegrao.ProblemaIntegracao = "Integração realizada com Sucesso";

                }
                servicoArquivoTransacao.Adicionar(fluxoPatioIntegrao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                fluxoPatioIntegrao.ProblemaIntegracao = ex.Message;
                fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                fluxoPatioIntegrao.ProblemaIntegracao = "Problema ao tentar integrar com Senior";
                fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                servicoArquivoTransacao.Adicionar(fluxoPatioIntegrao, jsonRequest, jsonResponse, "json");
            }

            repositorioFluxoPatio.Atualizar(fluxoPatioIntegrao);

        }

        public async Task IntegrarCargaDadosTransporteAsync(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracaoPendente, CancellationToken cancellationToken)
        {
            await IntegrarDadosTransporteAsync(integracaoPendente, cancellationToken);
        }

        #endregion

        #region Métodos Privados

        private string ObterToken(Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient(configuracaoIntegracao.UrlAutenticacao);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", configuracaoIntegracao.ClienteId);
            request.AddParameter("client_secret", configuracaoIntegracao.ClientSecret);
            request.AddParameter("grant_type", "client_credentials");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            if (string.IsNullOrWhiteSpace(retorno.access_token))
                throw new ServicoException("Não foi possível obter o Token");

            return retorno.access_token;
        }

        private HttpClient ObterHttpClient(Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            string token = ObterToken(configuracaoIntegracao);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSenior));

            client.BaseAddress = new Uri(configuracaoIntegracao.UrlRequisicao);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao ObterConfiguracaoSenior()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow repIntegracaoBoticarioFreeFlow = new Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow configIntegracaoBoticarioFreeFlow = repIntegracaoBoticarioFreeFlow.Buscar();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao()
            {
                UrlRequisicao = configIntegracaoBoticarioFreeFlow.URLIntegracao,
                UrlAutenticacao = configIntegracaoBoticarioFreeFlow.URLAutenticacao,
                ClienteId = configIntegracaoBoticarioFreeFlow.ClientId,
                ClientSecret = configIntegracaoBoticarioFreeFlow.ClientSecret
            };
        }

        private string ObterRequestIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio repDadosTransporteFluxoPatio = new Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio dadosTransporteFluxoPatio = repDadosTransporteFluxoPatio.BuscarPorCarga(carga.Codigo);

            string datePattern = "dd/MM/yyyy";
            string timePattern = "HH:mm";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RequestSchedule requestSchedule = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RequestSchedule()
            {
                Key = carga.Protocolo.ToString(),
                DataPrevisao = DateTime.Now.ToString(datePattern),
                HoraPrevisao = DateTime.Now.ToString(timePattern),
                Sequencia = 1,
                TipoVisita = 3,
                CodigoMotivacao = string.Empty,
                DataValidade = DateTime.Now.ToString(datePattern),
                HoraValidade = DateTime.Now.AddHours(2).ToString(timePattern),
            };

            requestSchedule.Company = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.Company()
            {
                Codigo = 1001,
                CodigoPortao = 4,
                Colaborador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.Colaborador() { Tipo = 1, NumeroRegistro = 999999 }
            };

            Dominio.Entidades.Usuario motorista = dadosTransporteFluxoPatio.Motorista;

            requestSchedule.Visitor = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.Visitor()
            {
                Nome = motorista.Nome,
                DataNascimento = motorista.DataNascimento?.ToString(datePattern) ?? "",
                Email = motorista.Email,
                FotoBase64 = ObterFotoBase64(dadosTransporteFluxoPatio.FotoMotorista, _unitOfWork),
                CodigoNacionalidade = 10,
                DDI = "55",
                DDD = motorista.Telefone?.ObterDDD() ?? "",
                NumeroTelefone = motorista.Telefone?.ObterFoneSemDDD() ?? "",
                Anotacao = "",
                Documento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.Documento() { Tipo = 2, Numero = motorista.CPF },
                Empresa = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.Empresa() { Codigo = carga.Empresa?.CNPJ_SemFormato ?? "", Nome = carga.Empresa?.RazaoSocial ?? "" },
                Veiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.Veiculo() { Placa = dadosTransporteFluxoPatio.Veiculo.Placa, EntradaAutorizada = "" }
            };

            return JsonConvert.SerializeObject(requestSchedule, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        private string ObterFotoBase64(string guidArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoArquivoFoto(unitOfWork);

            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{guidArquivo}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                throw new ServicoException("Imagem não encontrada");

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private string ObterCaminhoArquivoFoto(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "DadosTransporteFluxoPatio" });
        }

        private async Task IntegrarDadosTransporteAsync(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracaoPendente, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork, cancellationToken);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            try
            {
                integracaoPendente.NumeroTentativas++;
                integracaoPendente.DataIntegracao = DateTime.Now;

                await ObterConfiguracaoIntegracaoAsync();
                VerificarConfiguracaoIntegracao();
                string token = await ObterTokenSeniorAsync();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RetornoPedidosIntegracaoCarga retornoPedidosIntegracaoCarga = await ObterPedidosIntegracaoCargaAsync(integracaoPendente.Carga, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RetornoIntegracaoSenior retornoIntegracao = await EnviarTransporteAsync(retornoPedidosIntegracaoCarga, token, integracaoPendente.Protocolo);

                bool sucesso = retornoIntegracao.SituacaoValida;

                string mensagem = sucesso ? $"Integração gerada com Sucesso" : retornoIntegracao.Mensagem;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = sucesso ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                integracaoPendente.SituacaoIntegracao = situacao;
                integracaoPendente.ProblemaIntegracao = mensagem;
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSenior");

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSenior");

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com Senior.";
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(_jsonRequisicao) && !string.IsNullOrWhiteSpace(_jsonRetorno))
                    servicoArquivoTransacao.Adicionar(integracaoPendente, _jsonRequisicao, _jsonRetorno, "json");

                await repositorioCargaDadosTransporteIntegracao.AtualizarAsync(integracaoPendente);

                _jsonRequisicao = string.Empty;
                _jsonRetorno = string.Empty;
            }
        }

        private async Task ObterConfiguracaoIntegracaoAsync()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSenior repositorioIntegracaoSenior = new Repositorio.Embarcador.Configuracoes.IntegracaoSenior(_unitOfWork);
            _configuracaoIntegracao = await repositorioIntegracaoSenior.BuscarPrimeiroRegistroAsync();

            if (_configuracaoIntegracao == null || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para Senior.");
        }

        private async Task<string> ObterTokenSeniorAsync()
        {
            string token = string.Empty;

            Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao repositorioTipoIntegracaoAutenticacao = new Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao tipoIntegracaoAutenticacao = await repositorioTipoIntegracaoAutenticacao.BuscarPorTipoAsync(TipoIntegracao.Senior);

            if (tipoIntegracaoAutenticacao == null)
                tipoIntegracaoAutenticacao = new Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao();

            if (!string.IsNullOrWhiteSpace(tipoIntegracaoAutenticacao.Token) && tipoIntegracaoAutenticacao.DataExpiracao > DateTime.Now)
            {
                token = tipoIntegracaoAutenticacao.Token;

                return token;
            }

            HttpClient client = CriarRequisicao(token);

            var response = await client.GetAsync(_configuracaoIntegracao.URLAutenticacao);

            if (!response.IsSuccessStatusCode)
                throw new ServicoException("Não foi possível obter o Token");

            string retornoObjeto = await response.Content.ReadAsStringAsync();

            if (retornoObjeto == null)
                throw new ServicoException("Falha ao desserializar o retorno da autenticação Senior.");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.Retorno>(retornoObjeto);

            if (string.IsNullOrWhiteSpace(retorno.Token))
                throw new ServicoException("Token não retornado pela autenticação Senior.");

            if (retorno.ExpiraEm <= 0)
                throw new ServicoException("Tempo de expiração do token inválido.");

            tipoIntegracaoAutenticacao.Token = retorno.Token;
            tipoIntegracaoAutenticacao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Senior;
            tipoIntegracaoAutenticacao.DataExpiracao = DateTime.Now.AddSeconds(retorno.ExpiraEm);

            if (tipoIntegracaoAutenticacao.Codigo > 0)
                await repositorioTipoIntegracaoAutenticacao.AtualizarAsync(tipoIntegracaoAutenticacao);
            else
                await repositorioTipoIntegracaoAutenticacao.InserirAsync(tipoIntegracaoAutenticacao);

            return tipoIntegracaoAutenticacao.Token;
        }

        private HttpClient CriarRequisicao(string token)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSenior));

            if (string.IsNullOrWhiteSpace(token))
            {
                string basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_configuracaoIntegracao.Usuario}:{_configuracaoIntegracao.Senha}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            else
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }

            return client;
        }

        private void VerificarConfiguracaoIntegracao()
        {
            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLAutenticacao))
                throw new ServicoException("A url de autenticação deve estar preenchida na configuração de integração da Senior");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.Usuario))
                throw new ServicoException("Não existe usuário de integração configurada para Senior");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.Senha))
                throw new ServicoException("A senha deve ser informada na configuração da integração da Senior");
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RetornoPedidosIntegracaoCarga> ObterPedidosIntegracaoCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, CancellationToken cancellationToken)
        {
            CultureInfo culture = new CultureInfo("pt-BR");

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = await repositorioCargaPedido.BuscarPedidosPorCargaAsync(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaProdutos = await repositorioCargaPedidoProduto.BuscarPorPedidosAsync(listaPedidos.Select(x => x.Codigo).ToList());
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporte> listaIntegrarCargaDadosTransporte = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporte>();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidosAdicionais = await repositorioPedidoAdicional.BuscarPorPedidosAsync(listaPedidos.Select(x => x.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in listaPedidos)
            {
                decimal valorTotalNota = 0;

                Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidosAdicional = pedidosAdicionais.FirstOrDefault(x => x.Pedido.Codigo == pedido.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporte integracaoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporte()
                {
                    CodigoInterno = pedido.NumeroPedidoEmbarcador.ToString(),
                    NumeroPedido = pedido.NumeroPedidoEmbarcador.ToString(),
                    CnpjDepositante = pedido.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty,
                    CnpjEmitente = pedido.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty,
                    Tipo = pedidosAdicional?.TipoServico ?? string.Empty,
                    DescricaoOperacao = pedidosAdicional?.CondicaoExpedicao ?? string.Empty,
                    Cfop = pedido.NumeroControle ?? string.Empty,
                    DataEmissao = carga.DataCriacaoCarga,
                    TipoPessoaDestinatario = "J",
                    CodigoDestinatario = pedido.Destinatario?.CodigoIntegracao ?? string.Empty,
                    NomeDestinatario = pedido.Destinatario?.Nome ?? string.Empty,
                    DocumentoDestinatario = pedido.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                    EnderecoDestinatario = pedido.Destinatario.Endereco,
                    BairroDestinatario = pedido.Destinatario.Bairro,
                    CepDestinatario = pedido.Destinatario.CEP,
                    CidadeDestinatario = string.IsNullOrWhiteSpace(pedido?.Destino?.Descricao) ? (pedido.Destinatario?.Localidade?.Descricao ?? string.Empty) : (pedido.Destino?.Descricao ?? string.Empty),
                    EstadoDestinatario = pedido.Destinatario?.Localidade?.Estado.Descricao ?? string.Empty,
                    NomeTransportadora = carga.Empresa?.NomeFantasia ?? string.Empty,
                    CnpjTransportadora = carga.Empresa?.CNPJ ?? string.Empty,
                    ModalidadeFrete = "4",
                    PesoLiquido = pedido.PesoTotal.ToString("0.000", culture),
                    GeraFinanceiro = (pedidosAdicional?.GrupoFreteMaterial ?? "N").ToUpper().Contains("S"),
                    TipoDocumento = pedidosAdicional?.IndicadorPOF ?? string.Empty,
                    TipoCarga = pedido.TipoCarga?.Descricao ?? string.Empty,
                    NumeroItens = int.TryParse(pedidosAdicional?.NumeroOSMae, out var numeroItens) ? numeroItens : 0,
                    TipoNotaFiscal = "P",
                    EstadoDocumento = pedidosAdicional?.RestricaoEntrega ?? string.Empty,
                    CnpjUnidade = pedido.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty,
                    CodigoTipoPedido = pedido.CanalEntrega?.Descricao ?? string.Empty,
                    IdIntegracaoErp = carga.Empresa?.CodigoIntegracao ?? string.Empty,
                    CanalVenda = carga.CodigoCargaEmbarcador,
                    ProtocoloCarga = carga.Protocolo.ToString(),
                    Items = ObterItensProduto(listaProdutos, pedido, ref valorTotalNota),
                    ValorProdutos = valorTotalNota.ToString("0.000", culture),
                    ValorTotal = valorTotalNota.ToString("0.000", culture),
                };

                listaIntegrarCargaDadosTransporte.Add(integracaoCarga);
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RetornoPedidosIntegracaoCarga dadosSaida = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RetornoPedidosIntegracaoCarga();
            dadosSaida.chavelayout = "int_pedido";
            dadosSaida.ListaIntegrarCargaDadosTransporte = listaIntegrarCargaDadosTransporte;

            return dadosSaida;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporteItemTransporte> ObterItensProduto(List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaProdutos, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, ref decimal valorTotalNotal)
        {
            int contador = 1;

            CultureInfo culture = new CultureInfo("pt-BR");

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporteItemTransporte> listaItensProduto = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporteItemTransporte>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in listaProdutos.Where(x => x.Pedido.Codigo == pedido.Codigo))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporteItemTransporte itemTransporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.IntegrarCargaDadosTransporteItemTransporte
                {
                    CodigoInterno = pedido.NumeroPedidoEmbarcador.ToString(),
                    NumeroPedido = pedido.NumeroPedidoEmbarcador.ToString(),
                    CnpjDepositante = pedido.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty,
                    CnpjEmitente = pedido?.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty,
                    Tipo = "S",
                    Sequencia = contador++,
                    DescricaoProduto = produto.Produto.Descricao,
                    CodigoBarras = !string.IsNullOrWhiteSpace(produto.Produto.Observacao) ? produto.Produto.Observacao : produto.Produto?.CodigoEAN ?? string.Empty,
                    Quantidade = produto.Quantidade.ToString("0.00", culture),
                    ValorUnitario = produto.ValorProduto.ToString("0.000", culture),
                    ValorTotal = produto.ValorTotal.ToString("0.000", culture),
                    TotalLiquido = produto.ValorTotal.ToString("0.000", culture),
                    TipoProduto = "P",
                    CodigoIndustria = produto.Produto?.CodigoProdutoEmbarcador ?? string.Empty,
                    DescricaoCompletaProduto = produto.Produto.Descricao
                };

                listaItensProduto.Add(itemTransporte);
                valorTotalNotal += produto.ValorTotal;
            }

            return listaItensProduto;
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RetornoIntegracaoSenior> EnviarTransporteAsync(Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RetornoPedidosIntegracaoCarga listaPedidosIntegracaoCarga, string token, string? protocolo = null)
        {
            string urlBase = _configuracaoIntegracao.URLIntegracao;

            if (!string.IsNullOrWhiteSpace(protocolo))
            {
                Uri baseUri = new Uri(urlBase.EndsWith("/") ? urlBase : $"{urlBase}/");
                urlBase = new Uri(baseUri, protocolo).ToString();
            }

            HttpClient client = CriarRequisicao(token);

            try
            {
                _jsonRequisicao = JsonConvert.SerializeObject(listaPedidosIntegracaoCarga);
                var content = new StringContent(_jsonRequisicao, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(urlBase, content);

                _jsonRetorno = await response.Content.ReadAsStringAsync();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RetornoIntegracaoSenior retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.RetornoIntegracaoSenior>(_jsonRetorno);

                if (retorno == null)
                    Servicos.Log.TratarErro("Não foi possível interpretar a resposta da API.", "IntegracaoSenior");

                if (!retorno.SituacaoValida)
                    Servicos.Log.TratarErro($"Erro retornado pela API: {retorno.Mensagem}", "IntegracaoSenior");

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Exceção ao enviar transporte: {ex.Message}", "IntegracaoSenior");
                return null;
            }

        }
        #endregion
    }
}