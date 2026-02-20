using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Protheus
{
    public class IntegracaoProtheus
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoProtheus(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoProtheus repositorioProtheus = new Repositorio.Embarcador.Configuracoes.IntegracaoProtheus(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus configuracaoProtheus = repositorioProtheus.BuscarPrimeiroRegistro();

            faturaIntegracao.Tentativas += 1;
            faturaIntegracao.DataEnvio = DateTime.Now;

            if (!PossuiIntegracaoProtheus(configuracaoProtheus))
            {
                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Não existe configuração de integração disponível para o Protheus.";

                repositorioFaturaIntegracao.Atualizar(faturaIntegracao);

                return;
            }

            HttpClient client = ObterClient(configuracaoProtheus);

            if (IntegrarFaturaIntegracao(faturaIntegracao, client, configuracaoProtheus, configuracaoTMS))
            {
                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                faturaIntegracao.Fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado;
                faturaIntegracao.MensagemRetorno = "Viagem inserida com sucesso.";
            }

            if (faturaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                faturaIntegracao.Fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.ProblemaIntegracao;

            repositorioFaturaIntegracao.Atualizar(faturaIntegracao);
        }

        #endregion

        #region Métodos Privados

        private bool IntegrarFaturaIntegracao(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus integracaoProtheus, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = true;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus.Fatura fatura = PreencherRequisicao(faturaIntegracao);

                jsonRequest = JsonConvert.SerializeObject(fatura, Formatting.Indented);

                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage result = client.PostAsync(ObterURL(integracaoProtheus.URLAutenticacao, ""), content).Result;

                jsonResponse = result.Content?.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                //
                //
                //
                //CLIENTE NÃO MONTOU RETORNO ATÉ O MOMENTO DESTE COMMIT, TAREFA ORIGINAL #48447, FAVOR NÃO EXCLUIR SEÇÃO ABAIXO COMENTADA!
                //CLIENTE NÃO MONTOU RETORNO ATÉ O MOMENTO DESTE COMMIT, TAREFA ORIGINAL #48447, FAVOR NÃO EXCLUIR SEÇÃO ABAIXO COMENTADA!
                //CLIENTE NÃO MONTOU RETORNO ATÉ O MOMENTO DESTE COMMIT, TAREFA ORIGINAL #48447, FAVOR NÃO EXCLUIR SEÇÃO ABAIXO COMENTADA!

                if ((!string.IsNullOrWhiteSpace((string)retorno.Status) && (string)retorno.Status == "ERRO") || !result.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrWhiteSpace((string)retorno.message))
                    {
                        faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        faturaIntegracao.MensagemRetorno = "Retorno Servidor Protheus: " + (string)retorno.message;
                        sucesso = false;
                    }
                    else
                    {
                        faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        faturaIntegracao.MensagemRetorno = "Não foi possível concluir a operação, verifique os arquivos de integração.";
                        sucesso = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                faturaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Ocorreu uma falha ao inserir a viagem.";
                sucesso = false;
            }

            SalvarArquivosIntegracao(faturaIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        #endregion

        #region Métodos Privados - Configurações

        private HttpClient ObterClient(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string tokenLogin = $"{configuracaoIntegracao.Usuario}:{configuracaoIntegracao.Senha}";
            string tokenLoginConvertido = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tokenLogin));

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoProtheus));

            client.Timeout = TimeSpan.FromMinutes(2);
            client.BaseAddress = new Uri(configuracaoIntegracao.URLAutenticacao);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", tokenLoginConvertido);

            return client;
        }

        private string ObterURL(string urlBase, string servico)
        {
            if (string.IsNullOrWhiteSpace(urlBase))
                return urlBase;

            if (!urlBase.EndsWith("/"))
                urlBase += "/";

            urlBase += servico + "/";

            return urlBase;
        }

        private bool PossuiIntegracaoProtheus(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus configuracaoProtheus)
        {
            return !(configuracaoProtheus == null || !configuracaoProtheus.PossuiIntegracaoProtheus || string.IsNullOrWhiteSpace(configuracaoProtheus.Usuario) || string.IsNullOrWhiteSpace(configuracaoProtheus.Senha) || string.IsNullOrWhiteSpace(configuracaoProtheus.URLAutenticacao));
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus.Fatura PreencherRequisicao(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> DocumentosCTes = faturaIntegracao.Fatura?.Documentos?.Select(o => o.Documento.CTe).ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus.FaturaCTe> faturaCte = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus.FaturaCTe>();
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico documentoCTE in DocumentosCTes)
            {
                faturaCte.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus.FaturaCTe
                {
                    NumeroCTe = documentoCTE?.Numero ?? 0,
                    SerieCTe = documentoCTE?.Serie?.Numero ?? 0,
                    DataEmissaoCTe = documentoCTE?.DataEmissao ?? DateTime.MinValue,
                    ValorFrete = documentoCTE?.ValorAReceber ?? 0
                });
            }

            DateTime dataVencimento = faturaIntegracao?.Fatura?.Parcelas?.Max(o => o.DataVencimento) ?? DateTime.MinValue;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus.Fatura()
            {
                CNPJPagador = faturaIntegracao?.Fatura?.ClienteTomadorFatura?.CPF_CNPJ_SemFormato ?? "",
                DataInicialFatura = faturaIntegracao.Fatura.DataInicial.HasValue ? faturaIntegracao.Fatura.DataInicial.Value : DateTime.MinValue,
                DataFinalFatura = faturaIntegracao.Fatura.DataFinal.HasValue ? faturaIntegracao.Fatura.DataFinal.Value : DateTime.MinValue,
                FaturaDados = new Dominio.ObjetosDeValor.Embarcador.Integracao.Protheus.FaturaDados()
                {
                    CNPJTransportadora = faturaIntegracao.Fatura?.Transportador?.CNPJ ?? "0",
                    DataEmissao = faturaIntegracao.Fatura.DataFatura,
                    DataVencimento = dataVencimento,
                    DataVencimentoComDesconto = dataVencimento,
                    NumeroFatura = faturaIntegracao.Fatura.Numero.ToString(),
                    ValorFatura = faturaIntegracao.Fatura.Total,
                    FaturaCTe = faturaCte.ToArray(),
                }
            };
        }
        #endregion

        #region Métodos Privados - Salvar Arquivos

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = AdicionarArquivoTransacaoSituacao(faturaIntegracao, jsonRequisicao, jsonRetorno, faturaIntegracao.MensagemRetorno);

            if (faturaIntegracaoArquivo == null)
                return;
        }

        private Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo AdicionarArquivoTransacaoSituacao(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string jsonRequisicao, string jsonRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repositorioFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioFaturaIntegracaoArquivo.Inserir(faturaIntegracaoArquivo);

            faturaIntegracao.ArquivosIntegracao.Add(faturaIntegracaoArquivo);

            repositorioFaturaIntegracao.Atualizar(faturaIntegracao);

            return faturaIntegracaoArquivo;
        }

        #endregion
    }
}