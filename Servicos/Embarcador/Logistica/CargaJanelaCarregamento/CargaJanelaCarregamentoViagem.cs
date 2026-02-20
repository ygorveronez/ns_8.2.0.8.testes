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

namespace Servicos.Embarcador.Logistica
{
    public class CargaJanelaCarregamentoViagem
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Contrutores

        public CargaJanelaCarregamentoViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) : this(unitOfWork, tipoServicoMultisoftware, auditado: null) { }

        public CargaJanelaCarregamentoViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public void ControlarLiberacaoTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem janelaCarregamentoViagem, AcaoLiberacaoTransportadores acao)
        {
            for (int i = 0; i < janelaCarregamentoViagem.CargasJanelaCarregamento.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = janelaCarregamentoViagem.CargasJanelaCarregamento.ElementAt(i);

                switch (acao)
                {
                    case AcaoLiberacaoTransportadores.Liberar:
                        LiberarCargaParaTransportadores(cargaJanelaCarregamento);
                        break;

                    case AcaoLiberacaoTransportadores.Cancelar:
                        CancelarCargaLiberadaParaTransportadores(cargaJanelaCarregamento);
                        break;

                    case AcaoLiberacaoTransportadores.Descartar:
                        DescartarCargaDosTransportadores(cargaJanelaCarregamento);
                        break;

                    default:
                        break;
                }
            }
        }

        public bool ReenviarIntegracaoRetorno(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao, out string mensagemErro)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioCargaJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);

            mensagemErro = string.Empty;

            janelaCarregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

            repositorioCargaJanelaCarregamentoIntegracao.Atualizar(janelaCarregamentoIntegracao);

            return true;
        }

        public void GerarIntegracaoVencedorLeilao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoViagem repositorioJanelaCarregamentoViagem = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoViagem(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            CargaJanelaCarregamentoIntegracao servicoJanelaCarregamentoIntegracao = new CargaJanelaCarregamentoIntegracao(_unitOfWork, _tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem janelaCarregamentoViagem = repositorioJanelaCarregamentoViagem.BuscarPorJanelaCarregamento(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo);

            if (janelaCarregamentoViagem != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento outraCargaJanelaCarregamento = janelaCarregamentoViagem.CargasJanelaCarregamento.Where(o => o.Codigo != cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador outraCargaJanelaCarregamentoTransportador = null;

                decimal valorFrete = 0m;

                if (outraCargaJanelaCarregamento != null)
                    outraCargaJanelaCarregamentoTransportador = repositorioJanelaCarregamentoTransportador.BuscarPorCargaJanelaCarregamentoETransportador(outraCargaJanelaCarregamento.Codigo, cargaJanelaCarregamentoTransportador.Transportador.Codigo);

                valorFrete += cargaJanelaCarregamentoTransportador.ValorFreteTransportador;
                valorFrete += outraCargaJanelaCarregamentoTransportador?.ValorFreteTransportador ?? 0m;

                janelaCarregamentoViagem.Transportador = cargaJanelaCarregamentoTransportador.Transportador;
                janelaCarregamentoViagem.ValorFrete = valorFrete;
                janelaCarregamentoViagem.StatusLeilao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLeilaoIntegracaoJanelaCarregamento.Contratado;

                repositorioJanelaCarregamentoViagem.Atualizar(janelaCarregamentoViagem);

                servicoJanelaCarregamentoIntegracao.IncluirJanelaCarregamentoIntegracao(null, janelaCarregamentoViagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoRecebimento.Retorno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEventoIntegracaoJanelaCarregamento.ResultadoLeilao);

            }
        }

        public void EnviarIntegracaoResultadoLeilao()
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao> cargaJanelaCarregamentoIntegracoes = repositorioJanelaCarregamentoIntegracao.BuscarIntegracoesLeilaoPendentesDeEnvio();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao in cargaJanelaCarregamentoIntegracoes)
                IntegrarResultadoLeilaoOTM(janelaCarregamentoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private void LiberarCargaParaTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.CargaLiberadaCotacao)
                throw new ServicoException("A situação da janela de carregamento não permite que seja disponibilizada para os transportadores.");

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            var configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            CargaJanelaCarregamento servicoCargaJanelaCarregamento = new CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);
            CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new CargaJanelaCarregamentoTransportador(_unitOfWork, configuracaoEmbarcador);
            CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new CargaJanelaCarregamentoTransportadorTerceiro(_unitOfWork, configuracaoEmbarcador);
            CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new CargaJanelaCarregamentoCotacao(_unitOfWork, configuracaoEmbarcador);
            CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);

            if (cargaJanelaCarregamento.Carga.Empresa != null)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

                Auditoria.Auditoria.Auditar(_auditado, cargaJanelaCarregamento.Carga, $"Removido o transportador {cargaJanelaCarregamento.Carga.Empresa.Descricao} ao liberar para os transportadores", _unitOfWork);
                if (cargaJanelaCarregamento.TransportadorOriginal == null)
                {
                    cargaJanelaCarregamento.TransportadorOriginal = cargaJanelaCarregamento.Carga.Empresa;
                    repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                }
                cargaJanelaCarregamento.Carga.Empresa = null;
                repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
            }

            repositorioCargaJanelaCarregamentoTransportador.DeletarPorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo);

            if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                servicoCargaJanelaCarregamentoTransportadorTerceiro.DisponibilizarParaTransportadoresTerceiros(cargaJanelaCarregamento, cargaJanelaCarregamento.CentroCarregamento.TipoTransportadorTerceiro, _tipoServicoMultisoftware);
            else
                servicoCargaJanelaCarregamentoTransportador.DisponibilizarParaTransportadores(cargaJanelaCarregamento, cargaJanelaCarregamento.CentroCarregamento.TipoTransportador, null, _tipoServicoMultisoftware);

            servicoCargaJanelaCarregamentoCotacao.LiberarParaCotacaoAutomaticamente(cargaJanelaCarregamento, _tipoServicoMultisoftware);
            servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento, _tipoServicoMultisoftware);
            servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaLiberadaParaCotacaoParaTranportadores(cargaJanelaCarregamento);
        }

        private void CancelarCargaLiberadaParaTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new CargaJanelaCarregamentoCotacao(_unitOfWork, _tipoServicoMultisoftware);

            servicoCargaJanelaCarregamentoCotacao.CancelarCargaLiberadaParaTransportadores(cargaJanelaCarregamento);
        }

        private void DescartarCargaDosTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new CargaJanelaCarregamentoCotacao(_unitOfWork, _auditado);

            servicoCargaJanelaCarregamentoCotacao.DescartarCotacao(cargaJanelaCarregamento, _tipoServicoMultisoftware);
        }

        private void IntegrarResultadoLeilaoOTM(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoOTM repositorioIntegracaoOTM = new Repositorio.Embarcador.Configuracoes.IntegracaoOTM(_unitOfWork);
            CargaJanelaCarregamentoIntegracao servicoJanelaCarregamentoIntegracao = new CargaJanelaCarregamentoIntegracao(_unitOfWork, _tipoServicoMultisoftware);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOTM configuracaoIntegracaoOTM = repositorioIntegracaoOTM.BuscarPrimeiroRegistro();

            string jsonPost = string.Empty;
            string jsonResult = string.Empty;
            string mensagem = string.Empty;

            if (configuracaoIntegracaoOTM != null && (configuracaoIntegracaoOTM?.PossuiIntegracaoOTM ?? false))
            {
                dynamic objeto = new
                {
                    numeroShipment = janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem.NumeroViagem,
                    transportadoraEmitente = new
                    {
                        codigoIntegracao = janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem.Transportador?.CodigoIntegracao ?? string.Empty
                    },
                    valorFrete = janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem.ValorFrete,
                    statusLeilao = janelaCarregamentoIntegracao.CargaJanelaCarregamentoViagem.StatusLeilao.ObterEnumStatusLeilao()
                };

                string url = configuracaoIntegracaoOTM.URLIntegracaoLeilaoOTM;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(CargaJanelaCarregamentoViagem));

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("client_id", configuracaoIntegracaoOTM.ClientIDOTM);
                client.DefaultRequestHeaders.Add("client_secret", configuracaoIntegracaoOTM.ClientSecretOTM);

                jsonPost = JsonConvert.SerializeObject(objeto, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                Servicos.Log.TratarErro(jsonPost, "IntegracaoResultadoLeilao");

                var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;

                if (result.IsSuccessStatusCode)
                {
                    dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                    jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonResult, "IntegracaoResultadoLeilao");

                    mensagem = "Integrado com sucesso";
                    janelaCarregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }
                else
                {
                    Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "IntegracaoResultadoLeilao");

                    mensagem = result.Content.ReadAsStringAsync().Result;
                    janelaCarregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                janelaCarregamentoIntegracao.Mensagem = mensagem;

                servicoJanelaCarregamentoIntegracao.GravarArquivoIntegracao(janelaCarregamentoIntegracao, jsonPost, jsonResult, mensagem);
            }
            else
            {
                mensagem = "Configuração de integração de leilão não encontrada.";

                Servicos.Log.TratarErro(mensagem, "IntegracaoResultadoLeilao");

                janelaCarregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                janelaCarregamentoIntegracao.Mensagem = mensagem;

                servicoJanelaCarregamentoIntegracao.GravarArquivoIntegracao(janelaCarregamentoIntegracao, jsonPost, jsonResult, mensagem);
            }
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        #endregion
    }
}
