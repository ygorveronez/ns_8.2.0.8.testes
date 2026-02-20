using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;


namespace Servicos.Embarcador.Integracao.ATSLog
{
    public class IntegracaoATSLog
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoATSLog(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaCargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoATSLog repConfiguracaoATSLog = new Repositorio.Embarcador.Configuracoes.IntegracaoATSLog(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSLog configuracaoIntegracaoATSLog = repConfiguracaoATSLog.BuscarPrimeiroRegistro();

            cargaCargaDadosTransporteIntegracao.NumeroTentativas += 1;
            cargaCargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoATSLog?.URL))
                    throw new ServicoException("Não há URL configurada para a integração");

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string endPoint = $"{configuracaoIntegracaoATSLog.URL}";

                HttpClient client = ObterClient(configuracaoIntegracaoATSLog);

                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.GestaoSolicitacaoMonitoramentoIntegracao objEnvio = ObterObjetoSM(cargaCargaDadosTransporteIntegracao.Carga, configuracaoIntegracaoATSLog);

                jsonRequest = JsonConvert.SerializeObject(objEnvio, Formatting.Indented);
                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                if (result.IsSuccessStatusCode)
                {
                    mensagemErro = "Integrado com sucesso";

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaDadosTransporteIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaDadosTransporteIntegracao.ProblemaIntegracao = mensagemErro;
                    cargaCargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    repCargaDadosTransporteIntegracao.Atualizar(cargaCargaDadosTransporteIntegracao);

                    Servicos.Log.TratarErro($"Integrado com sucesso: {cargaCargaDadosTransporteIntegracao.Codigo}", "IntegracaoATSLog");
                }
                else
                {
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else if (retorno.Retorno != null)
                    {
                        mensagemErro = retorno.Retorno;
                        dynamic erro = JsonConvert.DeserializeObject<dynamic>(mensagemErro);
                        if (erro != null && erro.Message != null)
                            mensagemErro = erro.Message;
                    }
                    else
                        mensagemErro = retorno.message;

                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a ATSLog.";
                    else
                        mensagemErro = "Retorno ATSLog: " + mensagemErro;

                    Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoATSLog");
                    Servicos.Log.TratarErro("IntegracaoATSLogCarga", "IntegracaoATSLog");
                    Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoATSLog");
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoATSLog");

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaDadosTransporteIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCargaDadosTransporteIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaDadosTransporteIntegracao.Atualizar(cargaCargaDadosTransporteIntegracao);
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoATSLog");

                mensagemErro = excecao.Message;

                cargaCargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaDadosTransporteIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaDadosTransporteIntegracao.Atualizar(cargaCargaDadosTransporteIntegracao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + configuracaoIntegracaoATSLog.URL);
                Servicos.Log.TratarErro(excecao, "IntegracaoATSLog");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoATSLog");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoATSLog");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da ATSLog.";

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaDadosTransporteIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaCargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaDadosTransporteIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaDadosTransporteIntegracao.Atualizar(cargaCargaDadosTransporteIntegracao);
            }
        }

        #endregion

        #region Métodos Privados
        private HttpClient ObterClient(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSLog configuracaoIntegracaoATSLog)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoATSLog));

            requisicao.BaseAddress = new Uri(configuracaoIntegracaoATSLog.URL);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string token = ObterJWT(configuracaoIntegracaoATSLog);
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            return requisicao;
        }

        private string ObterJWT(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSLog configuracaoIntegracaoATSLog)
        {
            var claims = new[] {
                    new Claim("Email", configuracaoIntegracaoATSLog.Usuario),
                    new Claim("Senha", configuracaoIntegracaoATSLog.Senha)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracaoIntegracaoATSLog.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.GestaoSolicitacaoMonitoramentoIntegracao ObterObjetoSM(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSLog configuracaoIntegracaoATSLog)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.GestaoSolicitacaoMonitoramentoIntegracao objetoSM = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.GestaoSolicitacaoMonitoramentoIntegracao();

            objetoSM.Cliente = ObterDadosCliente(carga, configuracaoIntegracaoATSLog);
            objetoSM.Condutor = ObterDadosCondutor(carga);
            objetoSM.VeiculoTracao = ObterDadosVeiculoTracao(carga);
            objetoSM.PontosControle = ObterDadosPontosControle(carga);

            objetoSM.CodigoExterno = carga.CodigoCargaEmbarcador + "-3"; //Concatenado suffixo para tratar conflitos (Solicitação ATSLog) - Devops #4568
            objetoSM.Tipo = TipoGestaoSolicitacaoMonitoramentoIntegracao.Agendamento;
            objetoSM.DataHoraPrevisaoInicioViagem = carga.DataInicioViagem ?? carga.DataInicioViagemReprogramada ?? carga.DataInicioViagemPrevista ?? carga.DataCarregamentoCarga ?? carga.DataCriacaoCarga;

            DateTime dataUltimaEntrega = objetoSM.PontosControle.Max(o => o.DataHoraPrevisaoFim);
            DateTime dataFimViagem = carga.DataFimViagemPrevista ?? dataUltimaEntrega;
            if (dataFimViagem < dataUltimaEntrega) dataFimViagem = dataUltimaEntrega;
            objetoSM.DataHoraPrevisaoFimViagem = dataFimViagem;

            objetoSM.VinculoCondutor = 1;
            objetoSM.VinculoVeiculoTracao = 1;
            objetoSM.CodigoExternoOperacao = carga.TipoOperacao?.Descricao ?? string.Empty;

            return objetoSM;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.Condutor ObterDadosCondutor(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.Condutor DadosCondutor = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.Condutor();

            Dominio.Entidades.Usuario Motorista = carga.Veiculo?.Motoristas?.FirstOrDefault()?.Motorista ?? null;

            if (Motorista == null)
                Motorista = carga.VeiculosVinculados.FirstOrDefault()?.Motoristas?.FirstOrDefault()?.Motorista ?? null;

            if (Motorista == null)
                Motorista = carga.Motoristas.FirstOrDefault();

            if (Motorista != null && Motorista.Empresa != null)
            {
                DadosCondutor.Nome = Motorista.Nome;
                DadosCondutor.CPF_CNPJ = Motorista.CPF_CNPJ_Formatado;
                DadosCondutor.condutor = true;
                DadosCondutor.CodigoExterno = Motorista.Codigo.ToString();
                DadosCondutor.Cidade = Motorista.Localidade?.Descricao ?? string.Empty;
                DadosCondutor.UF = ObtemCodigoDeUF(Motorista.Localidade?.Estado?.Sigla ?? "");

                DadosCondutor.Empresa.CodigoExterno = Motorista.Empresa.Codigo.ToString();
                DadosCondutor.Empresa.Nome = Motorista.Empresa.NomeFantasia;
                DadosCondutor.Empresa.Cidade = Motorista.Empresa.Localidade?.Descricao ?? string.Empty;
                DadosCondutor.Empresa.UF = ObtemCodigoDeUF(Motorista.Empresa.Localidade?.Estado?.Sigla ?? "");
            }
            else
                throw new ServicoException("Carga sem motorista/empresa definido!");

            return DadosCondutor;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.Cliente ObterDadosCliente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSLog configuracaoIntegracaoATSLog)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.Cliente DadosCliente = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.Cliente();

            if (string.IsNullOrEmpty(configuracaoIntegracaoATSLog.CNPJCompany)) throw new ServicoException("Integração sem CNPJ definido!");
            if (carga.Filial == null) throw new ServicoException("Carga sem filial definida!");

            DadosCliente.Nome = configuracaoIntegracaoATSLog.NomeCompany;
            DadosCliente.CodigoExterno = configuracaoIntegracaoATSLog.CNPJCompany.ObterSomenteNumeros();
            DadosCliente.Condutor = false;
            DadosCliente.Cidade = configuracaoIntegracaoATSLog.Localidade?.Descricao ?? string.Empty;
            DadosCliente.UF = ObtemCodigoDeUF(configuracaoIntegracaoATSLog.Localidade?.Estado?.Sigla ?? string.Empty);

            return DadosCliente;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.VeiculoTracao ObterDadosVeiculoTracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.VeiculoTracao DadosVeiculoTracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.VeiculoTracao();

            Dominio.Entidades.Veiculo Veiculo = carga.Veiculo;
            if (Veiculo == null)
                Veiculo = carga.VeiculosVinculados.FirstOrDefault();

            if (Veiculo != null)
            {
                DadosVeiculoTracao.Placa = Veiculo.Placa;

                if (Veiculo.Tipo == "P") //Próprio
                {
                    DadosVeiculoTracao.Proprietario.Nome = Veiculo.Empresa?.Descricao ?? string.Empty;
                    DadosVeiculoTracao.Proprietario.Condutor = false;
                    DadosVeiculoTracao.Proprietario.CodigoExterno = Veiculo.Empresa?.Codigo.ToString() ?? "0";
                    DadosVeiculoTracao.Proprietario.Cidade = Veiculo.Empresa?.Localidade?.Descricao ?? string.Empty;
                    DadosVeiculoTracao.Proprietario.UF = ObtemCodigoDeUF(Veiculo.Empresa?.Localidade?.Estado?.Sigla ?? "");
                }
                else //Terceiro
                {
                    DadosVeiculoTracao.Proprietario.Nome = Veiculo.Proprietario.Nome;
                    DadosVeiculoTracao.Proprietario.Condutor = false;
                    DadosVeiculoTracao.Proprietario.CodigoExterno = Veiculo.Proprietario.Codigo.ToString();
                    DadosVeiculoTracao.Proprietario.Cidade = Veiculo.Proprietario.Localidade.Descricao;
                    DadosVeiculoTracao.Proprietario.UF = ObtemCodigoDeUF(Veiculo.Proprietario?.Localidade?.Estado?.Sigla ?? "");
                }

                DadosVeiculoTracao.Tipo.Nome = Veiculo.DescricaoTipo;
                DadosVeiculoTracao.Tipo.Sigla = Veiculo.DescricaoTipoRodado;
                DadosVeiculoTracao.Tipo.Tracao = Veiculo.TipoVeiculo == "0";
            }
            else
                throw new ServicoException("Carga sem veículo definido!");

            return DadosVeiculoTracao;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.PontosControle> ObterDadosPontosControle(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioTMS.BuscarPrimeiroRegistro();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.PontosControle> DadosPontosControle = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.PontosControle>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repositorioCargaEntrega.BuscarPorCarga(carga.Codigo);

            if (cargaEntregas.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();
                //DateTime DataHoraPrevisaoFim = carga.Entregas.FirstOrDefault()?.DataPrevista.Value ?? DateTime.Now;
                DateTime DataHoraPrevisaoFim = cargaEntregas.FirstOrDefault()?.DataPrevista.Value ?? DateTime.Now;

                //Se não existe Coleta, envia Expedidor/Remetente como Origem.
                if (!cargaEntregas.Exists(ce => ce.Coleta))
                {
                    if (cargaPedido == null) throw new ServicoException("Carga sem Coleta e sem Pedido.");

                    //bool utilizarExpedidor = cargaPedido.Pedido.Expedidor != null;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.PontosControle ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.PontosControle();

                    //Dominio.Entidades.Cliente cliente = utilizarExpedidor ? cargaPedido.Pedido?.Expedidor : cargaPedido.Pedido?.Remetente;
                    Dominio.Entidades.Cliente cliente = cargaPedido.Pedido?.Expedidor ?? cargaPedido.Pedido?.Remetente;

                    ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;

                    ponto.PontoControle.Pessoa.Nome = cliente?.Nome ?? string.Empty;
                    ponto.PontoControle.Pessoa.CodigoExterno = (cliente?.Codigo ?? 0).ToString();
                    ponto.PontoControle.Pessoa.Condutor = false;
                    ponto.PontoControle.Pessoa.Cidade = cliente?.Localidade?.Descricao ?? string.Empty;
                    ponto.PontoControle.Pessoa.UF = ObtemCodigoDeUF(cliente?.Localidade?.Estado?.Sigla ?? "");

                    ponto.PontoControle.Nome = ponto.PontoControle.Pessoa.Nome;

                    ponto.DataHoraPrevisaoInicio = (carga.DataInicioViagem ?? carga.DataInicioViagemReprogramada ?? carga.DataInicioViagemPrevista).Value.AddMinutes(5);
                    ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio.AddMinutes(60);
                    
                    DataHoraPrevisaoFim = ponto.DataHoraPrevisaoFim;

                    DadosPontosControle.Add(ponto);
                }


                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.PontosControle ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSLog.PontosControle();

                    Dominio.Entidades.Cliente cliente = cargaEntrega.Cliente;

                    //Se for a primeira Coleta, manda como Origem.
                    if (DadosPontosControle.Count == 0)
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;

                    //Se for a ultima entrega, manda como Destino.
                    else if (cargaEntrega == cargaEntregas[cargaEntregas.Count-1])
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Destino;

                    //Coleta.
                    else if (cargaEntrega.Coleta)
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Coleta;

                    //Entrega.
                    else if (!cargaEntrega.Coleta)
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Entrega;

                    ponto.PontoControle.Pessoa.Nome = cliente?.Nome ?? cargaEntrega.Localidade?.Descricao ?? string.Empty;
                    ponto.PontoControle.Pessoa.CodigoExterno = (cliente?.Codigo ?? cargaEntrega.Localidade?.Codigo ?? 0).ToString();
                    ponto.PontoControle.Pessoa.Condutor = false;
                    ponto.PontoControle.Pessoa.Cidade = cliente?.Localidade?.Descricao ?? cargaEntrega.Localidade?.Descricao ?? string.Empty;
                    ponto.PontoControle.Pessoa.UF = ObtemCodigoDeUF(cliente?.Localidade?.Estado?.Sigla ?? cargaEntrega.Localidade?.Estado?.Sigla ?? "");

                    ponto.PontoControle.Nome = ponto.PontoControle.Pessoa.Nome;

                    int minutosPadrao = 60;
                    if (cargaEntrega.Coleta && configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao > 0)
                        minutosPadrao = configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao;
                    else if (!cargaEntrega.Coleta && configuracaoTMS.TempoPadraoDeEntregaParaCalcularPrevisao > 0)
                        minutosPadrao = configuracaoTMS.TempoPadraoDeEntregaParaCalcularPrevisao;

                    ponto.DataHoraPrevisaoInicio = cargaEntrega.DataPrevista.Value;
                    ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio.AddMinutes(minutosPadrao);

                    while (ponto.DataHoraPrevisaoInicio <= DataHoraPrevisaoFim)
                    {
                        ponto.DataHoraPrevisaoInicio = ponto.DataHoraPrevisaoInicio.AddMinutes(minutosPadrao);
                        ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio.AddMinutes(minutosPadrao);
                    }

                    DataHoraPrevisaoFim = ponto.DataHoraPrevisaoFim;

                    DadosPontosControle.Add(ponto);
                }
            }
            return DadosPontosControle;
        }
        #endregion

        private int ObtemCodigoDeUF(string siglaUF)
        {
            switch (siglaUF)
            {
                case "AC": return 1;
                case "AL": return 2;
                case "AP": return 3;
                case "AM": return 4;
                case "BA": return 5;
                case "CE": return 6;
                case "DF": return 7;
                case "ES": return 8;
                case "GO": return 9;
                case "MA": return 10;
                case "MT": return 11;
                case "MS": return 12;
                case "MG": return 13;
                case "PA": return 14;
                case "PB": return 15;
                case "PR": return 16;
                case "PE": return 17;
                case "PI": return 18;
                case "RJ": return 19;
                case "RN": return 20;
                case "RS": return 21;
                case "RO": return 22;
                case "RR": return 23;
                case "SC": return 24;
                case "SP": return 25;
                case "SE": return 26;
                case "TO": return 27;
                case "EX": return 28;
                default: return 0;
            };
        }
    }
}