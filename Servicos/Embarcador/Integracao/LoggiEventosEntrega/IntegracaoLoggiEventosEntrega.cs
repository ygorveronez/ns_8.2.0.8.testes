using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Loggi
{
    public class IntegracaoLoggiEventosEntrega
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributo

        #region Construtores

        public IntegracaoLoggiEventosEntrega(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public HttpRequisicaoResposta EnviarRequisicao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            Repositorio.Embarcador.Configuracoes.IntegracaoLoggi repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggi(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            try
            {
                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao);

                dynamic evento = ObterRequisicao(cargaEntregaEvento);

                string jsonRequisicao = JsonConvert.SerializeObject(evento, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PutAsync(configuracaoIntegracao.URLIntegracaoEventoEntrega, conteudoRequisicao).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;
                httpRequisicaoResposta.conteudoResposta = jsonRetorno;

                if (retornoRequisicao.IsSuccessStatusCode)
                {
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                    httpRequisicaoResposta.sucesso = true;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException("Integração não autorizada, verifique o usuário e senha!");
                else if (string.IsNullOrWhiteSpace(jsonRetorno))
                    httpRequisicaoResposta.mensagem = "Retorno integração: " + retornoRequisicao.StatusCode;
                else
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                    httpRequisicaoResposta.mensagem = (string)retorno;
                }
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a Loggi.";
            }

            return httpRequisicaoResposta;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLoggiEventosEntrega));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLIntegracaoEventoEntrega);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", "Bearer " + ObterToken(configuracaoIntegracao.URLAutenticacaoEventoEntrega, configuracaoIntegracao.ClientID, configuracaoIntegracao.ClientSecret, configuracaoIntegracao.Scope));

            return requisicao;
        }

        private string ObterToken(string urlToken, string clienteId, string clienteSecret, string scope)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(urlToken);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", clienteId);
            request.AddParameter("client_secret", clienteSecret);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", scope);

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            return retorno.access_token;
        }

        private dynamic ObterRequisicao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            dynamic retorno = new ExpandoObject();

            Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(_unitOfWork);
            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unitOfWork);
            Repositorio.Embarcador.Logistica.AlertaTratativa repAlertaTratativa = new Repositorio.Embarcador.Logistica.AlertaTratativa(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEntregaEvento.Carga;
            EventoColetaEntrega? evento = cargaEntregaEvento.EventoColetaEntrega;

            if (carga == null) throw new ServicoException("Carga não encontrada");
            if (evento == null) throw new ServicoException("Não foi possível criar a requisição");

            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
            string codigoCargaEmbarcador = carga.CodigoCargaEmbarcador;
            Dominio.Entidades.Empresa empresa = carga.Empresa;
            Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Cargas.Pacote> pacotes = repPacote.BuscarPacotesPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = Servicos.Embarcador.Monitoramento.MonitoramentoEventoParada.ObterAlertasParadas(_unitOfWork, carga.Codigo, carga.Veiculo?.Codigo ?? 0, null, null);
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = repAlertaMonitor.BuscarPorCargaETipoAlerta(carga.Codigo, TipoAlerta.DesvioDeRota, null);
            Dominio.Entidades.Embarcador.Logistica.AlertaMonitor desvioDeRota = alertas.Find(x => x.TipoAlerta == TipoAlerta.DesvioDeRota && x.DataFim.HasValue && x.Observacao.HasValue());
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaEntrega = repCargaEntrega.BuscarUltimaCargaEntrega(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraEntrega = repCargaEntrega.BuscarPrimeiroPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Logistica.AlertaTratativa tratativa = desvioDeRota != null ? repAlertaTratativa.BuscarPorAlerta(desvioDeRota.Codigo) : null;
            List<Parada> dynParadas = paradas != null && paradas.Count > 0 ? ObterParadas(paradas) : new List<Parada>();
            string motivo_intercorrencia = alertas.Find(alerta => alerta.CargaEntregaEvento?.Codigo == cargaEntregaEvento.Codigo)?.Descricao ?? null;
            string hora_fim_programada = ultimaEntrega?.DataFimPrevista?.ToDateTimeStringISO8601() ?? string.Empty;
            string codigo_origem = carga.DadosSumarizados?.ClientesRemetentes?.FirstOrDefault()?.CodigoIntegracao ?? string.Empty;
            string codigo_destino = carga.DadosSumarizados?.ClientesDestinatarios?.FirstOrDefault()?.CodigoIntegracao ?? string.Empty;

            retorno.tipo_evento = ObterTipoEvento(evento.Value, cargaEntregaEvento.CargaEntrega?.Coleta ?? false, cargaEntregaEvento.TipoDeOcorrencia?.DescricaoAuxiliar ?? string.Empty);
            retorno.hora_acao = DateTime.Now.ToDateTimeStringISO8601();
            retorno.carga = ObterCarga(codigoCargaEmbarcador, carga.TipoOperacao?.Descricao ?? string.Empty);
            retorno.transportadora = ObterPessoaJuridica(empresa?.RazaoSocial ?? string.Empty, empresa?.CNPJ ?? string.Empty);
            retorno.motorista = ObterPessoaFisica(motorista?.Nome ?? string.Empty, motorista?.CPF ?? string.Empty);
            retorno.veiculo = ObterVeiculo(veiculo?.Placa ?? string.Empty, veiculo?.ModeloVeicularCarga?.Descricao ?? string.Empty);
            retorno.viagem = ObterViagem(carga.DataInicioViagem?.ToDateTimeStringISO8601() ?? "", carga.DataFimViagem?.ToDateTimeStringISO8601(), motivo_intercorrencia, hora_fim_programada, codigo_origem, codigo_destino, dynParadas);
            retorno.desvio_de_rota = ObterDesvioDeRota(desvioDeRota, tratativa);
            retorno.pacotes = pacotes != null && pacotes.Count > 0 ? pacotes.Select(x => x.LogKey).ToList() : new List<string>();

            return retorno;
        }

        private dynamic ObterDesvioDeRota(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor desvioDeRota, Dominio.Entidades.Embarcador.Logistica.AlertaTratativa tratativa)
        {
            if (desvioDeRota == null)
                return null;
            return new
            {
                hora_inicio = desvioDeRota.Data.ToDateTimeStringISO8601(),
                hora_fim = desvioDeRota.DataFim.Value.ToDateTimeStringISO8601(),
                tratativa = tratativa?.Descricao ?? string.Empty,
            };
        }

        #endregion Métodos Privados

        #region Métodos Privados - Funções Puras

        private string ObterTipoEvento(EventoColetaEntrega evento, bool coleta, string descricaoAuxiliar)
        {
            string tipoEvento = string.Empty;

            if (!string.IsNullOrEmpty(descricaoAuxiliar))
                tipoEvento = descricaoAuxiliar;
            else
            {
                tipoEvento = evento.ObterDescricao();
                //Regra específica para evento.
                switch (evento)
                {
                    case EventoColetaEntrega.IniciaViagem:
                        tipoEvento = "Rota iniciada";
                        break;
                    case EventoColetaEntrega.ChegadaNoAlvo:
                        tipoEvento = "Descarga iniciada";
                        break;
                    case EventoColetaEntrega.Confirma:
                        tipoEvento = coleta ? "Coleta realizada" : "Entrega realizada";
                        break;
                    case EventoColetaEntrega.FimViagem:
                        tipoEvento = "Rota finalizada";
                        break;
                    case EventoColetaEntrega.Intercorrencia:
                        tipoEvento = "Intercorrências com o motorista/viagem";
                        break;
                }
            }

            return tipoEvento;
        }
        private dynamic ObterCarga(string numero, string tipo) => new { numero, tipo };
        private dynamic ObterPessoaJuridica(string nome, string cnpj) => new { nome, cnpj };
        private dynamic ObterPessoaFisica(string nome, string cpf) => new { nome, cpf };
        private dynamic ObterVeiculo(string placa, string tipo) => new { placa, tipo };
        private dynamic ObterViagem(string hora_inicio, string hora_fim, string motivo_intercorrencia, string hora_fim_programada, string codigo_origem, string codigo_destino, List<Parada> paradas) => new { hora_inicio, hora_fim, motivo_intercorrencia, hora_fim_programada, codigo_origem, codigo_destino, paradas };
        private List<Parada> ObterParadas(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas) => (from obj in paradas select new Parada { hora_inicio = obj.DataInicio.ToDateTimeStringISO8601(), hora_fim = obj.DataFim.ToDateTimeStringISO8601(), descricao = obj.Descricao }).ToList();

        #endregion

        #region Objetos
        class Parada
        {
            public string hora_inicio { get; set; }
            public string hora_fim { get; set; }
            public string descricao { get; set; }
        }
        #endregion
    }
}
