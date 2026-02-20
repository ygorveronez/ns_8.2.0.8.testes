using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.LoggiFaturas
{
    public class IntegracaoLoggiFaturas
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoLoggiFaturas(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoLoggiFaturas repositorioIntegracaoLoggiFaturas = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggiFaturas(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggiFaturas configuracaoIntegracaoLoggiFaturas = repositorioIntegracaoLoggiFaturas.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            faturaIntegracao.DataEnvio = DateTime.Now;
            faturaIntegracao.Tentativas++;

            try
            {
                if (!(configuracaoIntegracaoLoggiFaturas?.PossuiIntegracao ?? false))
                    throw new ServicoException("Não possui configuração para Loggi Faturas.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas.LoggiFatura objetoLoggiFatura = ObterFatura(faturaIntegracao, configuracaoIntegracaoLoggiFaturas.NumeroMaterial);

                string url = configuracaoIntegracaoLoggiFaturas.URL;
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracaoLoggiFaturas.Usuario, configuracaoIntegracaoLoggiFaturas.Senha);

                jsonRequisicao = JsonConvert.SerializeObject(objetoLoggiFatura, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.Created)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas.RetornoLoggiFatura retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas.RetornoLoggiFatura>(jsonRetorno);

                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    faturaIntegracao.MensagemRetorno = "Fatura integrada com sucesso";
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest)//Retornos tratados
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas.RetornoErro retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas.RetornoErro>(jsonRetorno);

                    if (retorno.RetornoErroLoggiFaturas.Codigo.Equals("Z_TMS/012"))//Mensagem de "Fatura duplicada"
                    {
                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        faturaIntegracao.MensagemRetorno = retorno.RetornoErroLoggiFaturas.Mensagem;
                    }
                    else
                        throw new ServicoException($"{retorno.RetornoErroLoggiFaturas.Codigo} - {retorno.RetornoErroLoggiFaturas.Mensagem}");
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException($"Falha na autenticação com a Loggi Faturas");
                else
                    throw new ServicoException($"Falha ao conectar no WS Loggi Faturas: {retornoRequisicao.StatusCode}");
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

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas.LoggiFatura ObterFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string numeroMaterial)
        {
            if (faturaIntegracao.Fatura?.Tomador == null)
                throw new ServicoException($"Tomador não informado na primeira etapa da fatura. Regra do embarcador");

            if (faturaIntegracao.Fatura?.Tomador.Tipo != "J")
                throw new ServicoException($"Tomador precisa ser pessoa jurídica. Regra do embarcador");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.LoggiFaturas.LoggiFatura()
            {
                CompanyCode = "LL4B", //Manter fixo
                NumeroFatura = faturaIntegracao.Fatura?.Numero.ToString() ?? "",
                CNPJTransportador = faturaIntegracao.Fatura?.Transportador?.CNPJ ?? "",
                Quantidade = "1", //Manter fixo
                GrupoCompra = "101", //Manter fixo
                NumeroMaterial = numeroMaterial ?? "",
                TipoDocumento = "Cte", //Manter fixo
                DataDocumento = faturaIntegracao.Fatura?.DataFatura.ToString("dd.MM.yyyy") ?? "",
                Valor = faturaIntegracao.Fatura?.Total.ToString().Replace(',', '.') ?? "",
                CentroCusto = faturaIntegracao.Fatura?.CentroResultado?.PlanoContabilidade ?? "",
                CNPJTomador = faturaIntegracao.Fatura?.Tomador?.CPF_CNPJ_SemFormato ?? ""
            };
        }

        private HttpClient CriarRequisicao(string url, string usuario, string senha)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLoggiFaturas));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(usuario, senha);

            return requisicao;
        }

        private void SalvarArquivosIntegracaoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string arquivoRequisicao, string arquivoRetorno)
        {
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = AdicionarArquivoTransacaoFatura(faturaIntegracao, arquivoRequisicao, arquivoRetorno, faturaIntegracao.MensagemRetorno);

            if (faturaIntegracaoArquivo == null)
                return;

            if (faturaIntegracao.ArquivosIntegracao == null)
                faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();

            faturaIntegracao.ArquivosIntegracao.Add(faturaIntegracaoArquivo);
        }

        private Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo AdicionarArquivoTransacaoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string arquivoRequisicao, string arquivoRetorno, string mensagem)
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

        #endregion Métodos Privados
    }
}
