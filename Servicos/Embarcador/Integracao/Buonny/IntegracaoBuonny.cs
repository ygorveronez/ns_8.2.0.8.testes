using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Buonny
{
    public class IntegracaoBuonny
    {
        #region Atributos privados

        private static IntegracaoBuonny Instance;
        private ServicoBuonny.PosicaoEmViagemPortClient client;
        private Servicos.InspectorBehavior inspector;

        private string url;
        private string login;
        private string senha;

        #endregion

        private IntegracaoBuonny() { }

        public static IntegracaoBuonny GetInstance()
        {
            if (Instance == null) Instance = new IntegracaoBuonny();
            return Instance;
        }


        public void DefinirConfiguracoes(string url, string login, string senha)
        {
            this.url = url;
            this.login = login;
            this.senha = senha;
        }

        public List<ServicoBuonny.posicoesPosicao_em_viagem> BuscarUltimasPosicoes(Repositorio.UnitOfWork unitOfWork, ServicoBuonny.posicaoEmViagemRequest request)
        {
            VerificarPreparar(unitOfWork);
            ServicoBuonny.posicaoEmViagemResponse ultimasPosicoesResult = this.client.posicaoEmViagem(request);
            List<ServicoBuonny.posicoesPosicao_em_viagem> ultimasPosicoes = ultimasPosicoesResult?.posicoes?.ToList() ?? new List<ServicoBuonny.posicoesPosicao_em_viagem>();
            return ultimasPosicoes;
        }


        #region Métodos privados

        private void VerificarPreparar(Repositorio.UnitOfWork unitOfWork)
        {
            VerificarConfiguracoes();
            PrepararWSClient(unitOfWork);
        }

        private void PrepararWSClient(Repositorio.UnitOfWork unitOfWork)
        {
            if (this.client == null)
            {
                this.client = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoBuonny.PosicaoEmViagemPortClient, ServicoBuonny.PosicaoEmViagemPort>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Buonny_Posicoes, url);
            }
        }

        private void VerificarConfiguracoes()
        {
            if (string.IsNullOrWhiteSpace(this.url)) throw new ServicoException("URL Buonny não definida");
        }

        #endregion

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoDistanciaAlvo DistanciaPlacaAlvo(string placa, string codigoAlvo, ref string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoDistanciaAlvo retorno = null;

                if (integracao == null)
                {
                    mensagemErro = "Sem integração configurada.";
                    return retorno;
                }

                if (empresa == null)
                {
                    mensagemErro = "Não localizada empresa pai.";
                    return retorno;
                }

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && string.IsNullOrWhiteSpace(integracao.URLRestProducaoBuonny))
                {
                    mensagemErro = "Integração sem URL Rest configurada para ambiente de produção.";
                    return retorno;
                }

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && string.IsNullOrWhiteSpace(integracao.URLRestHomologacaoBuonny))
                {
                    mensagemErro = "Integração sem URL Rest configurada para ambiente de homologação.";
                    return retorno;
                }

                string urlAPI = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? integracao.URLRestProducaoBuonny : integracao.URLRestHomologacaoBuonny;

                urlAPI += "distancia_placa_alvo?";
                urlAPI += "token=" + integracao.TokenBuonny;
                urlAPI += "&cnpj=" + integracao.CNPJClienteBuonny;
                urlAPI += "&placa=" + placa;
                urlAPI += "&alvo=" + codigoAlvo;

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBuonny));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result = client.GetAsync(urlAPI).Result;

                if (result.IsSuccessStatusCode)
                {
                    string arquivoRetorno = result.Content.ReadAsStringAsync().Result;
                    Servicos.Log.TratarErro(urlAPI + " retorno: " + arquivoRetorno, "Buonny");

                    if (arquivoRetorno.Contains("erro"))
                        mensagemErro = arquivoRetorno;
                    else
                        retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoDistanciaAlvo>(result.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Servicos.Log.TratarErro("DistanciaPlacaAlvo erro: " + result.ReasonPhrase, "Buonny");
                    mensagemErro = "SERVIÇO DA BUONNY INDISPONÍVEL";//result.ReasonPhrase;
                }

                return retorno;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("DistanciaPlacaAlvo exceção: " + excecao, "Buonny");
                mensagemErro = "SERVIÇO DA BUONNY INDISPONÍVEL";// result.ReasonPhrase;
                return null;
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo ViagemVeiculo(string placa, string pedido, ref string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo retorno = null;

                if (integracao == null)
                {
                    mensagemErro = "Sem integração configurada.";
                    return retorno;
                }

                if (empresa == null)
                {
                    mensagemErro = "Não localizada empresa pai.";
                    return retorno;
                }

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && string.IsNullOrWhiteSpace(integracao.URLRestProducaoBuonny))
                {
                    mensagemErro = "Integração sem URL Rest configurada para ambiente de produção.";
                    return retorno;
                }

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && string.IsNullOrWhiteSpace(integracao.URLRestHomologacaoBuonny))
                {
                    mensagemErro = "Integração sem URL Rest configurada para ambiente de homologação.";
                    return retorno;
                }

                string urlAPI = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? integracao.URLRestProducaoBuonny : integracao.URLRestHomologacaoBuonny;

                urlAPI += "viagem_veiculo?";
                urlAPI += "token=" + integracao.TokenBuonny;
                urlAPI += "&cnpj=" + integracao.CNPJClienteBuonny;
                urlAPI += "&placa=" + placa;
                if (!string.IsNullOrWhiteSpace(pedido))
                    urlAPI += "&pedido=" + pedido;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBuonny));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage result = client.GetAsync(urlAPI).Result;

                if (result.IsSuccessStatusCode)
                {
                    string arquivoRetorno = result.Content.ReadAsStringAsync().Result;
                    Servicos.Log.TratarErro(urlAPI + " retorno: " + arquivoRetorno, "Buonny");

                    if (arquivoRetorno.Contains("erro"))
                    {
                        mensagemErro = arquivoRetorno;
                    }
                    else
                        retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo>(result.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Servicos.Log.TratarErro("ViagemVeiculo erro: " + result.ReasonPhrase, "Buonny");
                    mensagemErro = "SERVIÇO DA BUONNY INDISPONÍVEL";
                }

                return retorno;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("ViagemVeiculo exceção: " + excecao, "Buonny");
                mensagemErro = "SERVIÇO DA BUONNY INDISPONÍVEL";
                return null;
            }
        }

        public async System.Threading.Tasks.Task<Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo> ViagemVeiculoAsync(string placa, string pedido, Repositorio.UnitOfWork unitOfWork, System.Threading.CancellationToken cancellationToken = default)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.Buscar();
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarEmpresaPai();

                if (integracao == null)
                    throw new InvalidOperationException("Sem integração configurada.");

                if (empresa == null)
                    throw new InvalidOperationException("Não localizada empresa pai.");

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && string.IsNullOrWhiteSpace(integracao.URLRestProducaoBuonny))
                    throw new InvalidOperationException("Integração sem URL Rest configurada para ambiente de produção.");

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && string.IsNullOrWhiteSpace(integracao.URLRestHomologacaoBuonny))
                    throw new InvalidOperationException("Integração sem URL Rest configurada para ambiente de homologação.");

                string urlAPI = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? integracao.URLRestProducaoBuonny : integracao.URLRestHomologacaoBuonny;

                urlAPI += "viagem_veiculo?";
                urlAPI += "token=" + integracao.TokenBuonny;
                urlAPI += "&cnpj=" + integracao.CNPJClienteBuonny;
                urlAPI += "&placa=" + placa;
                if (!string.IsNullOrWhiteSpace(pedido))
                    urlAPI += "&pedido=" + pedido;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBuonny));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage result = await client.GetAsync(urlAPI, cancellationToken);

                if (!result.IsSuccessStatusCode)
                {
                    string erro = $"ViagemVeiculo erro: {result.ReasonPhrase}";
                    Servicos.Log.TratarErro(erro, "Buonny");
                    throw new InvalidOperationException("SERVIÇO DA BUONNY INDISPONÍVEL");
                }

                string arquivoRetorno = await result.Content.ReadAsStringAsync();
                Servicos.Log.TratarErro(urlAPI + " retorno: " + arquivoRetorno, "Buonny");

                if (arquivoRetorno.Contains("erro"))
                    throw new InvalidOperationException(arquivoRetorno);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo>(arquivoRetorno);
                return retorno;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("ViagemVeiculo exceção: " + excecao, "Buonny");
                throw new InvalidOperationException("SERVIÇO DA BUONNY INDISPONÍVEL", excecao);
            }
        }

        public async System.Threading.Tasks.Task<Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo> ViagemVeiculoAsync(string placa, string pedido, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoIntegracaoBuonny configuracao, System.Threading.CancellationToken cancellationToken = default)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                if (configuracao.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && string.IsNullOrWhiteSpace(configuracao.URLRestProducao))
                    throw new InvalidOperationException("Integração sem URL Rest configurada para ambiente de produção.");

                if (configuracao.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && string.IsNullOrWhiteSpace(configuracao.URLRestHomologacao))
                    throw new InvalidOperationException("Integração sem URL Rest configurada para ambiente de homologação.");

                string urlAPI = configuracao.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? configuracao.URLRestProducao : configuracao.URLRestHomologacao;

                urlAPI += "viagem_veiculo?";
                urlAPI += "token=" + configuracao.Token;
                urlAPI += "&cnpj=" + configuracao.CNPJCliente;
                urlAPI += "&placa=" + placa;
                if (!string.IsNullOrWhiteSpace(pedido))
                    urlAPI += "&pedido=" + pedido;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBuonny));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage result = await client.GetAsync(urlAPI, cancellationToken);

                if (!result.IsSuccessStatusCode)
                {
                    string erro = $"ViagemVeiculo erro: {result.ReasonPhrase}";
                    Servicos.Log.TratarErro(erro, "Buonny");
                    throw new InvalidOperationException("SERVIÇO DA BUONNY INDISPONÍVEL");
                }

                string arquivoRetorno = await result.Content.ReadAsStringAsync();
                Servicos.Log.TratarErro(urlAPI + " retorno: " + arquivoRetorno, "Buonny");

                if (arquivoRetorno.Contains("erro"))
                    throw new InvalidOperationException(arquivoRetorno);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo>(arquivoRetorno);
                return retorno;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("ViagemVeiculo exceção: " + excecao, "Buonny");
                throw new InvalidOperationException("SERVIÇO DA BUONNY INDISPONÍVEL", excecao);
            }
        }

        public static void StatusMotorista(ref Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracaoMotorista.TipoIntegracao.IntegrarComPlataformaNstech)
            {
                Nstech.IntegracaoMotorista svcIntegracaoSMNstech = new Nstech.IntegracaoMotorista(tipoServicoMultisoftware, unitOfWork);
                svcIntegracaoSMNstech.IntegrarMotorista(ref integracaoMotorista);
            }
            else
                IntegrarMotoristaBuonny(ref integracaoMotorista, unitOfWork);
        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusChecklist StatusChecklist(string placa, ref string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusChecklist retorno = null;

                if (!configuracao.RealizarIntegracaoGerenciadoraEmHomologacao && empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                {
                    retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusChecklist();
                    retorno.status = "S";
                    return retorno;
                }

                if (integracao == null)
                {
                    mensagemErro = "Sem integração configurada.";
                    return retorno;
                }

                if (empresa == null)
                {
                    mensagemErro = "Não localizada empresa pai.";
                    return retorno;
                }

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && string.IsNullOrWhiteSpace(integracao.URLRestProducaoBuonny))
                {
                    mensagemErro = "Integração sem URL Rest configurada para ambiente de produção.";
                    return retorno;
                }

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && string.IsNullOrWhiteSpace(integracao.URLRestHomologacaoBuonny))
                {
                    mensagemErro = "Integração sem URL Rest configurada para ambiente de homologação.";
                    return retorno;
                }

                string urlAPI = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? integracao.URLRestProducaoBuonny : integracao.URLRestHomologacaoBuonny;

                urlAPI += "status_checklist?";
                urlAPI += "token=" + integracao.TokenBuonny;
                urlAPI += "&cnpj=" + integracao.CNPJClienteBuonny;
                urlAPI += "&placa=" + placa;

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBuonny));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result = client.GetAsync(urlAPI).Result;

                if (result.IsSuccessStatusCode)
                {
                    string arquivoRetorno = result.Content.ReadAsStringAsync().Result;
                    Servicos.Log.TratarErro(urlAPI + " retorno: " + arquivoRetorno, "Buonny");

                    if (arquivoRetorno.Contains("erro"))
                    {
                        mensagemErro = mensagemErro = "Veículo sem cadastro/checklist na Buonny."; //arquivoRetorno;
                    }
                    else
                        retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusChecklist>(result.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Servicos.Log.TratarErro("StatusChecklist erro: " + result.ReasonPhrase, "Buonny");
                    mensagemErro = "SERVIÇO DA BUONNY INDISPONÍVEL";//result.ReasonPhrase;
                }

                return retorno;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("StatusChecklist exceção: " + excecao, "Buonny");
                mensagemErro = "SERVIÇO DA BUONNY INDISPONÍVEL";// result.ReasonPhrase;
                return null;
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusRNTRC StatusRNTRC(string placa, ref string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusRNTRC retorno = null;

                if (!configuracao.RealizarIntegracaoGerenciadoraEmHomologacao && empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                {
                    retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusRNTRC();
                    retorno.validado = "S";
                    return retorno;
                }

                if (integracao == null)
                {
                    mensagemErro = "Sem integração configurada.";
                    return retorno;
                }

                if (empresa == null)
                {
                    mensagemErro = "Não localizada empresa pai.";
                    return retorno;
                }

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && string.IsNullOrWhiteSpace(integracao.URLRestProducaoBuonny))
                {
                    mensagemErro = "Integração sem URL Rest configurada para ambiente de produção.";
                    return retorno;
                }

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && string.IsNullOrWhiteSpace(integracao.URLRestHomologacaoBuonny))
                {
                    mensagemErro = "Integração sem URL Rest configurada para ambiente de homologação.";
                    return retorno;
                }

                string urlAPI = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? integracao.URLRestProducaoBuonny : integracao.URLRestHomologacaoBuonny;

                urlAPI += "status_rntrc?";
                urlAPI += "token=" + integracao.TokenBuonny;
                urlAPI += "&cnpj=" + integracao.CNPJClienteBuonny;
                urlAPI += "&placa=" + placa;

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBuonny));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result = client.GetAsync(urlAPI).Result;

                if (result.IsSuccessStatusCode)
                {
                    string arquivoRetorno = result.Content.ReadAsStringAsync().Result;
                    Servicos.Log.TratarErro(urlAPI + " retorno: " + arquivoRetorno, "Buonny");

                    if (arquivoRetorno.Contains("erro"))
                    {
                        mensagemErro = "Veículo sem cadastro/RNTRC na Buonny."; //arquivoRetorno;
                    }
                    else
                        retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusRNTRC>(result.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Servicos.Log.TratarErro("StatusRNTRC erro: " + result.ReasonPhrase, "Buonny");
                    mensagemErro = "SERVIÇO DA BUONNY INDISPONÍVEL";// result.ReasonPhrase;
                }

                return retorno;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("StatusRNTRC exceção: " + excecao, "Buonny");
                mensagemErro = "SERVIÇO DA BUONNY INDISPONÍVEL";// result.ReasonPhrase;
                return null;
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoSolicitacaoAnaliseCadastral RetornarObjetoRequisicaoCadastroMotorista(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            Dominio.Entidades.Embarcador.Cargas.Carga ultimaCargaMotorista = repCarga.BuscarUltimaCargaPorMotorista(motorista.Codigo);

            Dominio.Entidades.Veiculo veiculoMotorista = repVeiculo.BuscarPorMotorista(motorista.Codigo);

            if (veiculoMotorista == null && ultimaCargaMotorista != null && ultimaCargaMotorista.Veiculo != null)
                veiculoMotorista = ultimaCargaMotorista.Veiculo;

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenBuonny) || string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJClienteBuonny))
                throw new ServicoException("Não encontrado configuração com Buonny");

            if (veiculoMotorista == null)
                throw new ServicoException("Motorista não possuí veiculo vinculado");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoSolicitacaoAnaliseCadastral retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoSolicitacaoAnaliseCadastral();
            retorno.cliente_cnpj = configuracaoIntegracao.CNPJClienteBuonny;
            retorno.fornecedor_id = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.Buonny;
            retorno.token = configuracaoIntegracao.TokenBuonny;
            retorno.tipo_pesquisa = "Normal";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Profissional profissional = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Profissional();
            if (motorista.LocalidadeNascimento == null)
                throw new ServicoException("Motorista sem localidade nascimento informada");

            if (!motorista.DataEmissaoRG.HasValue)
                throw new ServicoException("Motorista sem data Emissao da RG informado");

            profissional.pais_origem = 76;
            profissional.rg = motorista.RG;
            profissional.data_emissao_RG = motorista.DataEmissaoRG.HasValue ? motorista.DataEmissaoRG.Value.ToString("dd/MM/yyyy") : "";
            profissional.cpf = motorista.CPF;
            profissional.nome = motorista.Nome;
            profissional.nome_mae = motorista.Nome;
            profissional.nome_pai = motorista.Nome;
            profissional.data_nascimento = motorista.DataNascimento.HasValue ? motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : "";
            profissional.codigo_cidade_nascimento_ibge = motorista.LocalidadeNascimento.CodigoIBGE;
            profissional.codigo_profissao = 1;
            profissional.nome_rua = motorista.Endereco;
            profissional.numero_casa = motorista.NumeroEndereco;
            profissional.vinculo = "Frota";
            profissional.ddd = motorista.Telefone;
            profissional.telefone = motorista.Telefone;
            profissional.uf_rg = motorista.Localidade.Estado.Sigla;
            profissional.codigo_profissao = 0;
            profissional.orgao_rg = "25";
            profissional.endereco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Endereco();
            profissional.endereco.cep = motorista.Localidade.CEP;
            profissional.endereco.nome_bairro = motorista.Bairro;
            profissional.endereco.nome_rua = motorista.Endereco;
            profissional.endereco.numero_casa = motorista.NumeroEndereco;
            profissional.endereco.codigo_cidade_ibge = motorista.Localidade.CodigoIBGE;

            profissional.contatos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Contato>();

            if (motorista.Contatos == null || motorista.Contatos.Count <= 0)
                throw new ServicoException("Necessário ao menos um contato cadastrado ao motorista");

            foreach (var cont in motorista.Contatos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Contato contato1 = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Contato();

                contato1.tipo_contato = 1;
                contato1.nome_contato = cont.Nome ?? "";
                contato1.ddd_contato = cont.Telefone ?? "";
                contato1.telefone_contato = cont.Telefone ?? "";
                contato1.descricao = cont.Nome;

                profissional.contatos.Add(contato1);
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Cnh cnh = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Cnh();
            if (string.IsNullOrEmpty(motorista.NumeroHabilitacao))
                throw new ServicoException("Motorista sem Numero CNH informada");
            cnh.numero_cnh = motorista.NumeroHabilitacao;

            if (motorista.LocalidadeMunicipioEstadoCNH == null)
                cnh.cidade = motorista.Localidade.CodigoIBGE;
            else
                cnh.cidade = motorista.LocalidadeMunicipioEstadoCNH.CodigoIBGE;

            if (!motorista.DataVencimentoHabilitacao.HasValue)
                throw new ServicoException("Motorista sem Data Vencimento Habilitacao informada");
            cnh.data_vencimento = motorista.DataVencimentoHabilitacao.HasValue ? motorista.DataVencimentoHabilitacao.Value.ToString("dd/MM/yyyy") : "";

            if (string.IsNullOrEmpty(motorista.Categoria))
                throw new ServicoException("Motorista sem categoria CNH informada");
            cnh.categoria = motorista.Categoria;

            cnh.data_emissao_cnh = motorista.DataPrimeiraHabilitacao.HasValue ? motorista.DataPrimeiraHabilitacao.Value.ToString("dd/MM/yyyy") : "";
            cnh.numero_registro = motorista.NumeroRegistroHabilitacao;
            cnh.sigla_estado = motorista.EstadoCTPS?.Sigla ?? "";
            cnh.codigo_seguranca = "0";

            if (string.IsNullOrEmpty(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" })))
                throw new ServicoException("Obrigatório foto motorista (foto documento)");

            cnh.foto_documento = !string.IsNullOrEmpty(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" })) ? ObterFotoMotoristaBase64(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" })) : "";
            profissional.cnh = cnh;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Endereco endereco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Endereco();
            endereco.cep = motorista.CEP;
            endereco.codigo_cidade_ibge = motorista.Localidade.CodigoIBGE;
            endereco.nome_bairro = motorista.Bairro;
            endereco.nome_rua = motorista.Endereco;
            endereco.numero_casa = motorista.NumeroEndereco == "S/N" ? "0" : motorista.NumeroEndereco;
            endereco.uf = motorista.Localidade.Estado.Sigla;
            profissional.endereco = endereco;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Contato contato = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Contato();
            contato.tipo_contato = 1;
            contato.nome_contato = motorista.Contatos.Count > 0 ? motorista.Contatos?[0].Nome : "";
            contato.telefone_contato = motorista.Contatos.Count > 0 ? motorista.Contatos?[0].Telefone : "";
            contato.ddd_contato = motorista.Contatos.Count > 0 ? motorista.Contatos?[0].Telefone : "";

            profissional.contatos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Contato>();
            profissional.contatos.Add(contato);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.DadosComplementares dadosComplementares = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.DadosComplementares();
            dadosComplementares.vitima_roubo = "N";
            dadosComplementares.acidentes_sofridos = "N";
            dadosComplementares.viagens_realizadas_unidade_medida = 6;
            dadosComplementares.viagens_realizadas_quantidade = 10;

            profissional.dados_complementares = dadosComplementares;

            retorno.profissional = profissional;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Veiculo veiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Veiculo();
            veiculo.placa = veiculoMotorista.Placa;
            veiculo.numero_chassi = veiculoMotorista.Chassi;
            veiculo.modelo_veiculo = veiculoMotorista.Modelo?.Descricao ?? "";
            veiculo.cor_veiculo = veiculoMotorista?.CorVeiculo?.Descricao;
            veiculo.ano_fabricacao = veiculoMotorista.AnoFabricacao;
            veiculo.ano_modelo = veiculoMotorista.AnoModelo;
            veiculo.acessorio = "[]";
            veiculo.pais_origem = 76;
            veiculo.tipo_veiculo = "";
            veiculo.tipo_carroceria = "";
            veiculo.cor_veiculo = "1";
            veiculo.tipo_combustivel = "Diesel";
            veiculo.fabricante_veiculo = veiculoMotorista.Marca?.Descricao ?? "";
            veiculo.sigla_cidade = motorista.Localidade?.Descricao ?? "";
            veiculo.sigla_estado = veiculoMotorista.Estado?.Sigla ?? "";
            veiculo.numero_renavam = veiculoMotorista.Renavam;
            veiculo.proprietario = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Proprietario();

            if (veiculoMotorista.TipoProprietario == Dominio.Enumeradores.TipoProprietarioVeiculo.TACAgregado)
            {
                veiculo.proprietario.nome = profissional?.nome;
                veiculo.proprietario.documento = motorista?.CPF ?? "";
                veiculo.proprietario.numero_rg = motorista?.RG ?? "";
                veiculo.proprietario.endereco_cep = motorista?.CEP ?? "";
                veiculo.proprietario.endereco_codigo_cidade_ibge = motorista?.Localidade?.CodigoIBGE ?? 0;
                veiculo.proprietario.nome_mae = motorista.Nome;
                veiculo.proprietario.nome_pai = "";
                veiculo.proprietario.nome_rua = motorista?.Endereco ?? "";
                veiculo.proprietario.nome_bairro = motorista?.Bairro ?? "";
                veiculo.proprietario.telefone = motorista?.Telefone ?? "";
                veiculo.proprietario.pais_origem = 76;
                veiculo.proprietario.estado_origem = motorista?.Localidade?.Estado?.CodigoIBGE ?? 42;
                veiculo.proprietario.cidade_nascimento_ibge = motorista?.Localidade?.CodigoIBGE ?? 0;
                veiculo.proprietario.endereco_codigo_cidade_ibge = motorista?.Localidade?.CodigoIBGE ?? 0;
            }
            else
            {
                if (veiculoMotorista.Proprietario == null)
                    throw new ServicoException("Obrigatório informações sobre o proprietário do veiculo " + veiculoMotorista.Placa + " que esta vinculado ao motorista");

                veiculo.proprietario.nome = veiculoMotorista.Proprietario?.Nome ?? "";
                veiculo.proprietario.documento = veiculoMotorista.Proprietario?.CPF_CNPJ_SemFormato ?? "";
                veiculo.proprietario.numero_rg = veiculoMotorista.Proprietario?.RG_Passaporte ?? "";
                veiculo.proprietario.endereco_cep = veiculoMotorista.Proprietario?.CEP ?? "";
                veiculo.proprietario.endereco_codigo_cidade_ibge = veiculoMotorista.Proprietario?.Localidade?.CodigoIBGE ?? 0;
                veiculo.proprietario.nome_mae = veiculoMotorista.Proprietario?.Nome ?? "";
                veiculo.proprietario.nome_pai = veiculoMotorista.Proprietario?.Nome ?? "";
                veiculo.proprietario.nome_bairro = veiculoMotorista.Proprietario?.Bairro ?? "";
                veiculo.proprietario.numero_rua = veiculoMotorista.Proprietario?.Numero ?? "";
                veiculo.proprietario.telefone = veiculoMotorista.Proprietario?.Telefone1 ?? "";
                veiculo.proprietario.pais_origem = 76;
                veiculo.proprietario.estado_origem = veiculoMotorista.Proprietario?.Localidade?.Estado?.CodigoIBGE ?? 42;
                veiculo.proprietario.cidade_nascimento_ibge = veiculoMotorista.Proprietario?.Localidade?.CodigoIBGE ?? 0;
                veiculo.proprietario.endereco_codigo_cidade_ibge = veiculoMotorista.Proprietario?.Localidade?.CodigoIBGE ?? 0;
            }



            veiculo.rastreador = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Rastreador>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Rastreador rastreador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Rastreador();
            rastreador.empresa_rastreador = veiculoMotorista.TecnologiaRastreador?.Descricao ?? "Rastreador";
            rastreador.nome_rastreador = veiculoMotorista.NumeroEquipamentoRastreador ?? "Equip";
            veiculo.rastreador.Add(rastreador);

            retorno.veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Veiculo>();
            retorno.veiculos.Add(veiculo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Viagem viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.Viagem();

            if (ultimaCargaMotorista != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(ultimaCargaMotorista.Codigo);
                if (cargaPedido != null)
                {
                    viagem.cnpj_embarcador = configuracaoIntegracao.CNPJClienteBuonny;
                    viagem.cnpj_transportador = ultimaCargaMotorista.Empresa.CNPJ;
                    viagem.produto = "2";
                    viagem.cidade_origem = cargaPedido.Pedido?.Remetente?.Localidade?.CodigoIBGE ?? 0;
                    viagem.cidade_destino = cargaPedido.Pedido?.Destinatario?.Localidade?.CodigoIBGE ?? 0;
                    viagem.pais_destino = 76;
                    viagem.pais_origem = 76;
                    viagem.carga_tipo = "1";
                    viagem.carga_valor = (int)ultimaCargaMotorista.DadosSumarizados?.ValorTotalMercadoriaPedidos;
                    viagem.carga_nome = ultimaCargaMotorista.CodigoCargaEmbarcador;

                    retorno.viagem = viagem;
                }
            }

            return retorno;

        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoConsultaAnaliseCadastro RetornarObjetoConsultaCadastroMotorista(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Veiculo repVeiculoMotorista = new Repositorio.Veiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            Dominio.Entidades.Veiculo veiculoMotorista = repVeiculoMotorista.BuscarPorMotorista(motorista.Codigo);

            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaMotorista = repCargaMotorista.BuscarUltimaCargaMotorista(motorista.CPF);

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenBuonny) || string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJClienteBuonny) || string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJGerenciadoraDeRiscoBuonny))
                throw new ServicoException("Não encontrado configuração com Buonny");

            if (veiculoMotorista == null)
                throw new ServicoException("Motorista não possuí veiculo vinculado");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoConsultaAnaliseCadastro retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral.RequisicaoConsultaAnaliseCadastro();
            retorno.cliente_cnpj = configuracaoIntegracao.CNPJClienteBuonny;
            retorno.fornecedor_id = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.Buonny;
            retorno.token = configuracaoIntegracao.TokenBuonny;

            retorno.profissional_cpf = motorista.CPF_CNPJ_Formatado;
            retorno.veiculo_placa = veiculoMotorista.Placa;
            retorno.carreteiro = "S";
            retorno.carga_tipo = 3;
            retorno.carga_valor = cargaMotorista?.DadosSumarizados?.ValorTotalProdutos ?? 0;
            //retorno.carga_pais_destino 
            //        carga_pais_origem": 55,
            //carga_uf_origem": 11,
            //"carga_cidade_origem": 11,
            //"carga_pais_destino": 55
            return retorno;

        }


        private static void IntegrarMotoristaBuonny(ref Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();

            Repositorio.Veiculo repveiculo = new Repositorio.Veiculo(unitOfWork);
            Dominio.Entidades.Veiculo veiculo = repveiculo.BuscarPorMotorista(integracaoMotorista.Motorista.Codigo);

            string placa = veiculo?.Placa ?? "";
            string cpf = integracaoMotorista.Motorista.CPF;
            string msg = string.Empty;

            if (!configuracao.RealizarIntegracaoGerenciadoraEmHomologacao && empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
            {
                integracaoMotorista.ProblemaIntegracao = "";
                integracaoMotorista.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                return;
            }

            if (integracao == null)
                msg = "Sem integração configurada.";

            if (empresa == null)
                msg = "Não localizada empresa pai.";

            if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && string.IsNullOrWhiteSpace(integracao.URLRestProducaoBuonny))
                msg = "Integração sem URL Rest configurada para ambiente de produção.";

            if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && string.IsNullOrWhiteSpace(integracao.URLRestHomologacaoBuonny))
                msg = "Integração sem URL Rest configurada para ambiente de homologação.";

            if (string.IsNullOrWhiteSpace(placa))
                msg = "Obrigatório informar a placa na consulta";

            if (!string.IsNullOrWhiteSpace(msg))
            {
                integracaoMotorista.ProblemaIntegracao = msg;
                integracaoMotorista.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                CriarArquivoIntegracao(ref integracaoMotorista, null, unitOfWork);

                return;
            }

            string urlAPI = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? integracao.URLRestProducaoBuonny : integracao.URLRestHomologacaoBuonny;

            urlAPI += "status_motorista?";
            urlAPI += "token=" + integracao.TokenBuonny;
            urlAPI += "&cnpj=" + integracao.CNPJClienteBuonny;
            urlAPI += "&placa=" + placa;
            urlAPI += "&cpf=" + cpf;

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBuonny));
            client.BaseAddress = new Uri(urlAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = client.GetAsync(urlAPI).Result;

            if (result.IsSuccessStatusCode)
            {
                string arquivoRetorno = result.Content.ReadAsStringAsync().Result;
                Servicos.Log.TratarErro(urlAPI + " retorno: " + arquivoRetorno, "Buonny");

                if (arquivoRetorno.Contains("erro"))
                {
                    integracaoMotorista.ProblemaIntegracao = "Motorista sem cadastro/ficha na Buonny.";//arquivoRetorno;
                    integracaoMotorista.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    CriarArquivoIntegracao(ref integracaoMotorista, null, unitOfWork);
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusMotorista retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusMotorista>(result.Content.ReadAsStringAsync().Result);

                    if (!string.IsNullOrWhiteSpace(retorno.valido) && retorno.valido.ToUpper() == "S")
                    {
                        integracaoMotorista.ProblemaIntegracao = "Motorista valido.";
                        integracaoMotorista.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                        CriarArquivoIntegracao(ref integracaoMotorista, retorno, unitOfWork);
                    }
                    else
                    {
                        integracaoMotorista.ProblemaIntegracao = "Motorista não esta valido na Buonny.";
                        integracaoMotorista.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        CriarArquivoIntegracao(ref integracaoMotorista, retorno, unitOfWork);
                    }

                }
            }
            else
            {
                Servicos.Log.TratarErro("StatusMotorista erro: " + result.ReasonPhrase, "Buonny");
                integracaoMotorista.ProblemaIntegracao = "SERVIÇO DA BUONNY INDISPONÍVEL";//result.ReasonPhrase;
                integracaoMotorista.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                CriarArquivoIntegracao(ref integracaoMotorista, null, unitOfWork);
            }
        }

        private static void CriarArquivoIntegracao(ref Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotorista, Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusMotorista retorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string stringRetorno = string.Empty;
            if (retorno != null)
                stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracaoMotorista.ProblemaIntegracao;
            integracaoArquivo.Data = DateTime.Now;
            integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

            if (integracaoMotorista.ArquivosTransacao == null)
                integracaoMotorista.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            integracaoMotorista.ArquivosTransacao.Add(integracaoArquivo);
        }


        private static string ObterFotoMotoristaBase64(string caminho)
        {
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        #region Consulta SM Buonny

        public async Task ConsultarSMBuonnyAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken = default)
        {
            (Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoIntegracaoBuonny configuracaoBuonny) = await ValidarEObterConfiguracoesAsync(unitOfWork, cancellationToken);
            if (configuracaoTMS == null || configuracaoBuonny == null)
                return;

            const int maximoRequisicoesConcorrentes = 5;
            using (SemaphoreSlim semaphore = new SemaphoreSlim(maximoRequisicoesConcorrentes, maximoRequisicoesConcorrentes))
            {
                await ProcessarFluxosPorEtapaAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem, configuracaoTMS, configuracaoBuonny, unitOfWork, semaphore, cancellationToken);

                await ProcessarFluxosPorEtapaAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimViagem, configuracaoTMS, configuracaoBuonny, unitOfWork, semaphore, cancellationToken);
            }
        }

        private async Task<(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoIntegracaoBuonny)> ValidarEObterConfiguracoesAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork, cancellationToken);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = await repositorioIntegracao.BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny);
            Dominio.Entidades.Empresa empresa = await repositorioEmpresa.BuscarEmpresaPaiAsync();

            if (configuracaoIntegracao == null || !configuracaoIntegracao.ConsultarSMBuonny || tipoIntegracao == null || empresa == null)
                return (null, null);

            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoIntegracaoBuonny configuracaoBuonny = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoIntegracaoBuonny
            {
                URLRestProducao = configuracaoIntegracao.URLRestProducaoBuonny,
                URLRestHomologacao = configuracaoIntegracao.URLRestHomologacaoBuonny,
                Token = configuracaoIntegracao.TokenBuonny,
                CNPJCliente = configuracaoIntegracao.CNPJClienteBuonny,
                TipoAmbiente = empresa.TipoAmbiente
            };

            return (configuracaoTMS, configuracaoBuonny);
        }

        private async Task ProcessarFluxosPorEtapaAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoIntegracaoBuonny configuracaoBuonny, Repositorio.UnitOfWork unitOfWork, SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, cancellationToken);
            List<int> codigosFluxo = await repositorioFluxoGestaoPatio.BuscarCodigoFluxoPorEtapaAsync(etapa, cancellationToken);

            List<Task> tarefas = codigosFluxo
                .TakeWhile(_ => !cancellationToken.IsCancellationRequested)
                .Select(codigoFluxo => ProcessarFluxoViagemAsync(codigoFluxo, etapa, configuracaoTMS, configuracaoBuonny, unitOfWork.StringConexao, semaphore, cancellationToken))
                .ToList();

            await Task.WhenAll(tarefas);
        }

        private async Task ProcessarFluxoViagemAsync(int codigoFluxo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoIntegracaoBuonny configuracaoBuonny, string stringConexao, SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, cancellationToken);

                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxo = await repositorioFluxoGestaoPatio.BuscarPorCodigoComDadosIntegracaoAsync(codigoFluxo, cancellationToken);

                    Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ResultadoConsultaViagem retornoViagem = await ConsultarViagemVeiculoInternoAsync(fluxo, configuracaoBuonny, cancellationToken);

                    if (!retornoViagem.Sucesso)
                    {
                        RegistrarErroConsulta(retornoViagem, etapa);
                        return;
                    }

                    DateTime? dataViagem = ObterDataViagemPorEtapa(retornoViagem.Retorno, etapa);
                    if (!dataViagem.HasValue)
                    {
                        RegistrarAusenciaData(retornoViagem.NumeroSM, etapa);
                        return;
                    }

                    RegistrarSucessoConsulta(retornoViagem.NumeroSM, dataViagem.Value, etapa);
                    await ProcessarEtapaViagemAsync(fluxo, etapa, dataViagem.Value, configuracaoTMS, unitOfWork, cancellationToken);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro($"ConsultarSMBuonny {etapa} {excecao}", "Buonny");
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ResultadoConsultaViagem> ConsultarViagemVeiculoInternoAsync(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoIntegracaoBuonny configuracaoBuonny, CancellationToken cancellationToken)
        {
            string numeroSM = fluxoPatio.Filial.CodigoFilialEmbarcador + fluxoPatio.Carga.CodigoCargaEmbarcador;
            string mensagemErro = string.Empty;
            Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo retorno = null;

            try
            {
                retorno = await ViagemVeiculoAsync(fluxoPatio.Carga.Veiculo?.Placa ?? string.Empty, numeroSM, configuracaoBuonny, cancellationToken);
            }
            catch (Exception excecao)
            {
                mensagemErro = excecao.Message;
            }

            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ResultadoConsultaViagem
            {
                NumeroSM = numeroSM,
                Retorno = retorno,
                MensagemErro = mensagemErro,
                Sucesso = string.IsNullOrWhiteSpace(mensagemErro) && retorno != null
            };
        }

        private DateTime? ObterDataViagemPorEtapa(Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoViagemVeiculo retorno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapa)
        {
            string dataString = etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem
                ? retorno.data_inicio
                : retorno.data_fim;

            if (string.IsNullOrWhiteSpace(dataString))
                return null;

            DateTime.TryParseExact(dataString, "yyyyMMdd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataRetorno);
            return dataRetorno;
        }

        private async Task ProcessarEtapaViagemAsync(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapa, DateTime dataViagem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoPatio, etapa, dataViagem);

            if (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem)
                await ProcessarRetencaoInicioViagemAsync(fluxoPatio, dataViagem, configuracaoTMS, unitOfWork, cancellationToken);
            else
                await ProcessarFinalizacaoCargaAsync(fluxoPatio, configuracaoTMS, unitOfWork, cancellationToken);
        }

        private async Task ProcessarRetencaoInicioViagemAsync(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio, DateTime dataInicioViagem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (configuracaoTMS.MotivoChamadoRetencaoInicioViagem == null)
                return;

            DateTime dataFaturamento = await ObterDataFaturamentoAsync(fluxoPatio, unitOfWork, cancellationToken);
            double horasRetencao = (dataInicioViagem - dataFaturamento).TotalHours;

            if (horasRetencao > 5)
                await CriarChamadoRetencaoAsync(fluxoPatio, dataFaturamento, dataInicioViagem, configuracaoTMS, unitOfWork, cancellationToken);
        }

        private async Task<DateTime> ObterDataFaturamentoAsync(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = await repositorioCargaCTe.BuscarPorCargaAsync(fluxoPatio.Carga.Codigo);
            return cargasCTe?.FirstOrDefault()?.CTe.DataEmissao ?? fluxoPatio.Carga.DataCriacaoCarga;
        }

        private async Task CriarChamadoRetencaoAsync(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio, DateTime dataFaturamento, DateTime dataInicioViagem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado
            {
                Empresa = fluxoPatio.Carga.Empresa,
                MotivoChamado = configuracaoTMS.MotivoChamadoRetencaoInicioViagem,
                Carga = fluxoPatio.Carga,
                TipoCliente = Dominio.Enumeradores.TipoTomador.Remetente,
                RetencaoBau = false,
                DataRetencaoInicio = dataFaturamento,
                DataRetencaoFim = dataInicioViagem,
                Observacao = "Atendimento gerado pela retenção para inicio da viagem"
            };

            await unitOfWork.StartAsync(cancellationToken);

            await new Servicos.Embarcador.Chamado.Chamado(unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null).AbrirChamadoAsync(objetoChamado, fluxoPatio.Carga.Motoristas?.FirstOrDefault());

            await unitOfWork.CommitChangesAsync(cancellationToken);
        }

        private async Task ProcessarFinalizacaoCargaAsync(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!configuracaoTMS.EncerrarCargaQuandoFinalizarGestaoPatio || fluxoPatio.Carga == null)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(fluxoPatio.Carga.Codigo);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            servicoCarga.LiberarSituacaoDeCargaFinalizada(carga, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null, null);
        }

        private void RegistrarErroConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ResultadoConsultaViagem resultado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapa)
        {
            string mensagem = !string.IsNullOrWhiteSpace(resultado.MensagemErro)
                ? $"ViagemVeiculo: {resultado.MensagemErro}"
                : "ViagemVeiculo: Integração não teve retorno.";

            Servicos.Log.TratarErro(mensagem, "Buonny");
        }

        private void RegistrarAusenciaData(string numeroSM, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapa)
        {
            string tipoData = etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem
                ? "início"
                : "fim";

            Servicos.Log.TratarErro($"ViagemVeiculo: {numeroSM} não retornou data {tipoData}.", "Buonny");
        }

        private void RegistrarSucessoConsulta(string numeroSM, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapa)
        {
            string tipoData = etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem
                ? "início"
                : "fim";

            Servicos.Log.TratarErro($"ViagemVeiculo: {numeroSM} retornou data {tipoData} {data:yyyyMMdd HH:mm:ss}", "Buonny");
        }

        #endregion Consulta SM Buonny
    }
}
