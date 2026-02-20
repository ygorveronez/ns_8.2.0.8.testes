using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Efesus
{
    public class IntegracaoEfesus
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoEfesus(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            faturaIntegracao.DataEnvio = DateTime.Now;
            faturaIntegracao.Tentativas += 1;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEfesus configuracaoIntegracao = PossuiIntegracaoEfesus();
                string url = configuracaoIntegracao.URLIntegracao;
                HttpClient requisicao = ObterClient(configuracaoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.Fatura fatura = PreencherRequisicao(faturaIntegracao);
                jsonRequisicao = JsonConvert.SerializeObject(fatura, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.RetornoIntegracaoFatura retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.RetornoIntegracaoFatura>(jsonRetorno);

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
                {
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    faturaIntegracao.Fatura.Situacao = SituacaoFatura.Fechado;
                    faturaIntegracao.MensagemRetorno = "Integração realizada com sucesso";
                }
                else
                {
                    faturaIntegracao.MensagemRetorno = (retorno == null || retorno.faturaId == 0) ? $"Ocorreu uma falha ao realizar a integração com a Efesus. Retorno: {(retorno.message)}" : $"{(string.IsNullOrWhiteSpace(retorno.message) ? "" : $"{retorno.message} ")}{(string.IsNullOrWhiteSpace(retorno.message) ? "" : retorno.faturaId)}";
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                SalvarArquivosIntegracao(faturaIntegracao, jsonRequisicao, jsonRetorno);
            }
            catch (ServicoException excecao)
            {
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Ocorreu uma falha ao realizar a integração com a Efesus";

                SalvarArquivosIntegracao(faturaIntegracao, jsonRequisicao, jsonRetorno);
            }

            repositorioCargaIntegracao.Atualizar(faturaIntegracao);
        }

        #endregion

        #region Métodos Privados - Configurações

        private HttpClient ObterClient(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEfesus configuracaoEfesus)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string tokenLogin = $"{configuracaoEfesus.Usuario}:{configuracaoEfesus.Senha}";
            string tokenLoginConvertido = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tokenLogin));

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoEfesus));

            client.BaseAddress = new Uri(configuracaoEfesus.URLIntegracao);
            client.Timeout = TimeSpan.FromMinutes(2);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", tokenLoginConvertido);

            return client;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEfesus PossuiIntegracaoEfesus()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoEfesus repositorioEfesus = new Repositorio.Embarcador.Configuracoes.IntegracaoEfesus(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEfesus configuracaoEfesus = repositorioEfesus.BuscarPrimeiroRegistro();

            if (configuracaoEfesus == null || !configuracaoEfesus.PossuiIntegracao ||
                string.IsNullOrWhiteSpace(configuracaoEfesus.Usuario) ||
                string.IsNullOrWhiteSpace(configuracaoEfesus.Senha) ||
                string.IsNullOrWhiteSpace(configuracaoEfesus.URLIntegracao))
            {
                throw new ServicoException("Não existe configuração de integração disponível para o Efesus");
            }

            return configuracaoEfesus;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.Fatura PreencherRequisicao(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.DocumentoCTeFaturaIntegracao> documentoCTesFaturaIntegracao = repositorioCTe.BuscarDocumentosCTesFaturaIntegracao(faturaIntegracao.Fatura?.Codigo ?? 0);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.FaturaCTe> faturaCte = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.FaturaCTe>();
            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.DocumentoCTeFaturaIntegracao documentoCTeFaturaIntegracao in documentoCTesFaturaIntegracao)
            {
                faturaCte.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.FaturaCTe
                {
                    NumeroCTe = documentoCTeFaturaIntegracao.NumeroCTe,
                    SerieCTe = documentoCTeFaturaIntegracao.SerieCTe,
                    ChaveCTe = documentoCTeFaturaIntegracao.ChaveCTe,
                    DataEmissaoCTe = documentoCTeFaturaIntegracao.DataEmissaoCTe.Value,
                    ValorFrete = documentoCTeFaturaIntegracao.ValorFrete
                });
            }

            DateTime dataVencimento = faturaIntegracao.Fatura?.Parcelas?.Max(o => o.DataVencimento) ?? DateTime.MinValue;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.Fatura()
            {
                CNPJPagador = faturaIntegracao.Fatura?.ClienteTomadorFatura?.CPF_CNPJ_SemFormato ?? "",
                DataInicialFatura = faturaIntegracao.Fatura.DataInicial.HasValue ? faturaIntegracao.Fatura.DataInicial.Value : DateTime.MinValue,
                DataFinalFatura = faturaIntegracao.Fatura.DataFinal.HasValue ? faturaIntegracao.Fatura.DataFinal.Value : DateTime.MinValue,
                FaturaDados = new Dominio.ObjetosDeValor.Embarcador.Integracao.Efesus.FaturaDados()
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