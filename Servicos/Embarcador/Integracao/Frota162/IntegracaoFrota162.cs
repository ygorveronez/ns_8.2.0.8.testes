using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Frota162
{
    public class IntegracaoFrota162
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoFrota162(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarVeiculo(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoFrota162 repositorioIntegracaoFrota162 = new Repositorio.Embarcador.Configuracoes.IntegracaoFrota162(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162 configuracaoIntegracaoFrota162 = repositorioIntegracaoFrota162.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            veiculoIntegracao.DataIntegracao = DateTime.Now;
            veiculoIntegracao.NumeroTentativas++;

            try
            {
                if (!(configuracaoIntegracaoFrota162?.PossuiIntegracaoFrota162 ?? false))
                    throw new ServicoException("Não possui configuração para Frota 162.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.Veiculo objetoVeiculo = ObterVeiculo(veiculoIntegracao.Veiculo, configuracaoIntegracaoFrota162);

                string url = $"{configuracaoIntegracaoFrota162.URL}/key/create-car";
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracaoFrota162.Token, configuracaoIntegracaoFrota162.SecretKey);

                jsonRequisicao = JsonConvert.SerializeObject(objetoVeiculo, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoVeiculo>(jsonRetorno);

                    if (retorno.Erro)
                        veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    else
                        veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    
                    if (retorno.JaExiste)
                        veiculoIntegracao.ProblemaIntegracao = "Retornou que carro já existe";
                    else
                        veiculoIntegracao.ProblemaIntegracao = retorno.Mensagem;

                    if (!string.IsNullOrWhiteSpace(retorno.Veiculo?.Id ?? retorno.Data?.Id ?? ""))
                    {
                        veiculoIntegracao.Veiculo.CodigoIntegracao = retorno.Veiculo?.Id ?? retorno.Data?.Id ?? "";
                        repVeiculo.Atualizar(veiculoIntegracao.Veiculo);
                    }
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest || retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)//Retornos tratados
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoErro retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoErro>(jsonRetorno);
                    throw new ServicoException($"{retorno.Codigo} - {retorno.Mensagem}");
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Frota 162: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Frota 162";
            }

            servicoArquivoTransacao.Adicionar(veiculoIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioVeiculoIntegracao.Atualizar(veiculoIntegracao);
        }

        public void InativarVeiculo(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoFrota162 repositorioIntegracaoFrota162 = new Repositorio.Embarcador.Configuracoes.IntegracaoFrota162(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162 configuracaoIntegracaoFrota162 = repositorioIntegracaoFrota162.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            veiculoIntegracao.DataIntegracao = DateTime.Now;
            veiculoIntegracao.NumeroTentativas++;

            try
            {
                if (!(configuracaoIntegracaoFrota162?.PossuiIntegracaoFrota162 ?? false))
                    throw new ServicoException("Não possui configuração para Frota 162.");

                if (string.IsNullOrWhiteSpace(veiculoIntegracao.Veiculo?.CodigoIntegracao ?? ""))
                    throw new ServicoException("Veículo não possui código de integração para realizar a inativação.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.Veiculo objetoVeiculo = ObterVeiculo(veiculoIntegracao.Veiculo, configuracaoIntegracaoFrota162);

                string url = $"{configuracaoIntegracaoFrota162.URL}/key/disable-car/" + veiculoIntegracao.Veiculo.CodigoIntegracao;
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracaoFrota162.Token, configuracaoIntegracaoFrota162.SecretKey);
                
                HttpResponseMessage retornoRequisicao = requisicao.DeleteAsync(url).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoVeiculo>(jsonRetorno);
                    if (retorno.Erro)
                    {
                        veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        veiculoIntegracao.ProblemaIntegracao = retorno.Mensagem;
                    }
                    else
                    {
                        veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        veiculoIntegracao.ProblemaIntegracao = retorno.Mensagem;
                    }
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest || retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)//Retornos tratados
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoErro retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoErro>(jsonRetorno);
                    throw new ServicoException($"{retorno.Codigo} - {retorno.Mensagem}");
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Frota 162: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Frota 162";
            }

            servicoArquivoTransacao.Adicionar(veiculoIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioVeiculoIntegracao.Atualizar(veiculoIntegracao);
        }

        public void IntegrarMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao motoristaIntegracao)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoFrota162 repositorioIntegracaoFrota162 = new Repositorio.Embarcador.Configuracoes.IntegracaoFrota162(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162 configuracaoIntegracaoFrota162 = repositorioIntegracaoFrota162.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            motoristaIntegracao.DataIntegracao = DateTime.Now;
            motoristaIntegracao.NumeroTentativas++;

            try
            {
                if (!(configuracaoIntegracaoFrota162?.PossuiIntegracaoFrota162 ?? false))
                    throw new ServicoException("Não possui configuração para Frota 162.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.Motorista objetoMotorista = ObterMotorista(motoristaIntegracao.Motorista, configuracaoIntegracaoFrota162);

                string url = $"{configuracaoIntegracaoFrota162.URL}/key/create-driver";
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracaoFrota162.Token, configuracaoIntegracaoFrota162.SecretKey);

                jsonRequisicao = JsonConvert.SerializeObject(objetoMotorista, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoMotorista retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoMotorista>(jsonRetorno);

                    if (retorno.MotoristaStatus.Erro)
                        throw new ServicoException($"{retorno.MotoristaStatus.Codigo} - {retorno.MotoristaStatus.Mensagem}");

                    motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    motoristaIntegracao.ProblemaIntegracao = retorno.MotoristaStatus.Mensagem;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest || retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)//Retornos tratados
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoErro retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.RetornoErro>(jsonRetorno);

                    if (retorno.Codigo.Equals("fbk_400"))//Mensagem de "Condutor já cadastrado"
                    {
                        motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        motoristaIntegracao.ProblemaIntegracao = retorno.Mensagem;
                    }
                    else
                        throw new ServicoException($"{retorno.Codigo} - {retorno.Mensagem}");
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Frota 162: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                motoristaIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                motoristaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Frota 162";
            }

            servicoArquivoTransacao.Adicionar(motoristaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioMotoristaIntegracao.Atualizar(motoristaIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.Veiculo ObterVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162 configuracaoIntegracaoFrota162)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.Veiculo()
            {
                CompanyID = configuracaoIntegracaoFrota162.CompanyId,
                Placa = veiculo.Placa,
                Chassi = veiculo.Chassi,
                Renavam = veiculo.Renavam,
                PrefixoFrota = veiculo.NumeroFrota
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.Motorista ObterMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162 configuracaoIntegracaoFrota162)
        {
            Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato pai = motorista.Contatos?.Where(o => o.TipoParentesco == TipoParentesco.Pai)?.FirstOrDefault() ?? null;
            Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato mae = motorista.Contatos?.Where(o => o.TipoParentesco == TipoParentesco.Mae)?.FirstOrDefault() ?? null;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Frota162.Motorista()
            {
                CompanyID = configuracaoIntegracaoFrota162.CompanyId,
                CPF = motorista.CPF,
                Nome = motorista.Nome,
                CNH = motorista.NumeroHabilitacao,
                EstadoCNH = motorista.EstadoRG?.Sigla,
                CategoriaCNH = motorista.Categoria,
                NomeMae = mae?.Nome,
                NomePai = pai?.Nome,
                RG = motorista.RG,
                DataNascimento = motorista.DataNascimento?.ToString("yyyy-MM-dd"),
                DataAdmissao = motorista.DataAdmissao?.ToString("yyyy-MM-dd"),
                DataVencimentoCNH = motorista.DataVencimentoHabilitacao?.ToString("yyyy-MM-dd"),
                DataCNH = motorista.DataHabilitacao?.ToString("yyyy-MM-dd"),
                Email = motorista.Email,
                Cidade = motorista.Localidade?.Descricao,
                Estado = motorista.Localidade?.Estado?.Sigla,
                CEP = motorista.CEP,
                Endereco = motorista.Endereco,
                EnderecoNumero = motorista.NumeroEndereco,
                EnderecoComplemento = motorista.Complemento
            };
        }

        private HttpClient CriarRequisicao(string url, string accessToken, string key)
        {

            System.Net.ServicePointManager.Expect100Continue = true;
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;


            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoFrota162));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", accessToken);

            string keyHexHmacSHA256 = Utilidades.Calc.HmacSHA256(key, accessToken);
            requisicao.DefaultRequestHeaders.Add("key", keyHexHmacSHA256);

            return requisicao;
        }

        #endregion Métodos Privados
    }
}
