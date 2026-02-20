using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.TrizyEventos
{
    public class IntegracaoTrizyEventos
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizyEventos _configuracaoIntegracaoTrizyEventos;

        #endregion Atributos Privados

        #region Construtores

        public IntegracaoTrizyEventos(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                HttpClient requisicao = CriarRequisicao();
                Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.TrizyEventos corpoRequisicao = PreencherCorpoRequisicaoTrizyEventos(integracaoPendente.Carga);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracaoTrizyEventos.URLIntegracao + "/DocumentosDisponiveis", conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Trizy Eventos.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.ResponseTrizyEventos retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.ResponseTrizyEventos>(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoPendente.ProblemaIntegracao = retornoIntegracao.Mensagem;
                }
                else if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException(retornoIntegracao.Mensagem);
                else
                    throw new ServicoException("Retorno de status não tratado, verificar a comunicação com a Trizy Eventos!");
            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoTrizyEventos");

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com Trizy Eventos.";
            }

            servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json");
            repositorioCargaCargaIntegracao.Atualizar(integracaoPendente);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient CriarRequisicao()
        {
            ObterConfiguracaoIntegracaoTrizyEventos();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizyEventos));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", "Bearer " + _configuracaoIntegracaoTrizyEventos.Token);

            return requisicao;
        }

        private void ObterConfiguracaoIntegracaoTrizyEventos()
        {
            if (_configuracaoIntegracaoTrizyEventos != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrizyEventos repositorioIntegracaoTrizyEventos = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizyEventos(_unitOfWork);
            _configuracaoIntegracaoTrizyEventos = repositorioIntegracaoTrizyEventos.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracaoTrizyEventos == null || !_configuracaoIntegracaoTrizyEventos.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para Trizy Eventos.");
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.TrizyEventos PreencherCorpoRequisicaoTrizyEventos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.Protocolo <= 0)
                throw new ServicoException("Protocolo não encontrado ou não existe.");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.TrizyEventos
            {
                Protocolo = carga.Protocolo,
            };
        }

        #endregion Métodos Privados
    }
}
