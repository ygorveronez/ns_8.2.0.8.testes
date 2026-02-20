using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Neokohm
{
    public class IntegracaoNeokohm
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributo

        #region Construtores

        public IntegracaoNeokohm(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                bool atualizar = !string.IsNullOrWhiteSpace(cargaDadosTransporteIntegracao.Protocolo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoNeokohm();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.RequestIntegracao dadosRequisicao = PreencherRequisicao(cargaDadosTransporteIntegracao, atualizar);

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao, cargaDadosTransporteIntegracao.Protocolo, atualizar);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.Url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.ResponseNeokohm resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.ResponseNeokohm>(jsonRetorno);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);

                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                    cargaDadosTransporteIntegracao.Protocolo = resposta?.ID?.ToString() ?? "";
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException(resposta.Detail);
                else
                    throw new ServicoException($"Problema ao integrar com Neokohn: {retornoRequisicao.StatusCode} {resposta?.Detail ?? string.Empty}");
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrenciaPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
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

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoNeokohm();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.RequestIntegracao dadosRequisicao = PreencherRequisicaoPedidoOcorrencia(integracao);

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao, string.Empty, false);
                httpRequisicaoResposta.conteudoRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(httpRequisicaoResposta.conteudoRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.Url, conteudoRequisicao).Result;
                httpRequisicaoResposta.conteudoResposta = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.ResponseNeokohm resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.ResponseNeokohm>(httpRequisicaoResposta.conteudoResposta);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);

                    httpRequisicaoResposta.sucesso = true;
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso";
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException(resposta.Detail);
                else
                    throw new ServicoException($"Problema ao integrar com Neokohm (Status: {retornoRequisicao.StatusCode})");
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                httpRequisicaoResposta.mensagem = "Problema ao tentar integrar com Neokohm.";
            }

            return httpRequisicaoResposta;
        }

        public bool ValidarSeDeveGerarIntegracaoNeokohm(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoNeokohm repositorioConfiguracaoIntegracaoNeokohm = new Repositorio.Embarcador.Configuracoes.IntegracaoNeokohm(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm configuracaoIntegracaoNeokohm = repositorioConfiguracaoIntegracaoNeokohm.Buscar();

            if (!(configuracaoIntegracaoNeokohm?.PossuiIntegracaoNeokohm ?? false))
                return false;

            bool possuiEquipamentoAtivo = false;
            if (carga.Veiculo != null && carga.Veiculo.Equipamentos != null && carga.Veiculo.Equipamentos.Count > 0)
                possuiEquipamentoAtivo = carga.Veiculo.Equipamentos.Any(x => x.Neokohm == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim);

            if (!possuiEquipamentoAtivo && carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                possuiEquipamentoAtivo = carga.VeiculosVinculados.Any(x => x.Equipamentos != null && x.Equipamentos.Count > 0 && x.Equipamentos.Any(y => y.Neokohm == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim));

            return possuiEquipamentoAtivo;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient CriarRequisicao(Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.ConfiguracaoIntegracao configuracaoIntegracao, string protocolo, bool atualizar)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoNeokohm));

            requisicao.BaseAddress = atualizar ? new Uri(configuracaoIntegracao.Url + $"/{protocolo.ToInt()}") : new Uri(configuracaoIntegracao.Url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.Token);

            return requisicao;
        }

        private bool IsRetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return (retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created);
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoNeokohm repositorioIntegracaoToken = new Repositorio.Embarcador.Configuracoes.IntegracaoNeokohm(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm configuracaoIntegracaoToken = repositorioIntegracaoToken.Buscar();

            if ((configuracaoIntegracaoToken == null) || !configuracaoIntegracaoToken.PossuiIntegracaoNeokohm)
                throw new ServicoException("Não existe configuração de integração disponível para a Neokohm.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoToken.TokenNeokohm) || string.IsNullOrWhiteSpace(configuracaoIntegracaoToken.URLIntegracaoNeokohm))
                throw new ServicoException("O Token e a URL devem estar preenchidos na configuração de integração da Neokohm.");

            return configuracaoIntegracaoToken;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.ConfiguracaoIntegracao ObterConfiguracaoIntegracaoNeokohm()
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm configuracaoIntegracaoNeokohm = ObterConfiguracaoIntegracao();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoNeokohm.URLIntegracaoNeokohm))
                throw new ServicoException("A URL não está configurada para a integração com a Neokohm");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.ConfiguracaoIntegracao()
            {
                Token = configuracaoIntegracaoNeokohm.TokenNeokohm,
                Url = configuracaoIntegracaoNeokohm.URLIntegracaoNeokohm
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.RequestIntegracao PreencherRequisicao(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, bool atualizar)
        {
            DateTime dataPrevisaoFimViagem = DateTime.Now;

            if (cargaDadosTransporteIntegracao.Carga.Rota.PadraoTempo.HasValue && cargaDadosTransporteIntegracao.Carga?.Rota?.PadraoTempo.Value == PadraoTempoDiasMinutos.Minutos)
                dataPrevisaoFimViagem.AddMinutes(cargaDadosTransporteIntegracao.Carga?.Rota?.TempoDeViagemEmMinutos ?? 0);
            else if (cargaDadosTransporteIntegracao.Carga.Rota.PadraoTempo.HasValue && cargaDadosTransporteIntegracao.Carga?.Rota?.PadraoTempo.Value == PadraoTempoDiasMinutos.Minutos)
                dataPrevisaoFimViagem.AddDays(cargaDadosTransporteIntegracao.Carga?.Rota?.TempoDeViagemEmMinutos ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.RequestIntegracao request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.RequestIntegracao();

            request.DataFinal = dataPrevisaoFimViagem.ToString("dd/MM/yyyy HH:mm");
            request.DataInicial = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            if (atualizar)
            {
                request.InicioCarga = true;
                request.FimCarga = false;
            }
            request.Placa = cargaDadosTransporteIntegracao.Carga.VeiculosVinculados.FirstOrDefault()?.Placa ?? string.Empty;
            request.NumeroCarga = cargaDadosTransporteIntegracao.Carga?.CodigoCargaEmbarcador ?? string.Empty;
            request.TemperaturaMinimaCarga = cargaDadosTransporteIntegracao.Carga.TipoDeCarga?.FaixaDeTemperatura?.FaixaInicial ?? 0;
            request.TemperaturaMaximaCarga = cargaDadosTransporteIntegracao.Carga.TipoDeCarga?.FaixaDeTemperatura?.FaixaFinal ?? 0;
            request.TemperaturaIdeal = null;
            request.Seca = false;
            request.Observacao = null;
            request.Origens = ObterOrigens(cargaDadosTransporteIntegracao);
            request.Destinos = ObterDestinos(cargaDadosTransporteIntegracao);

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.RequestIntegracao PreencherRequisicaoPedidoOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            DateTime dataPrevisaoFimViagem = DateTime.Now;

            if (integracao.PedidoOcorrenciaColetaEntrega.Carga.Rota.PadraoTempo.HasValue && integracao.PedidoOcorrenciaColetaEntrega.Carga?.Rota?.PadraoTempo.Value == PadraoTempoDiasMinutos.Minutos)
                dataPrevisaoFimViagem.AddMinutes(integracao.PedidoOcorrenciaColetaEntrega.Carga?.Rota?.TempoDeViagemEmMinutos ?? 0);
            else if (integracao.PedidoOcorrenciaColetaEntrega.Carga?.Rota?.PadraoTempo.Value == PadraoTempoDiasMinutos.Minutos)
                dataPrevisaoFimViagem.AddDays(integracao.PedidoOcorrenciaColetaEntrega.Carga?.Rota?.TempoDeViagemEmMinutos ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.RequestIntegracao request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.RequestIntegracao();

            request.DataFinal = dataPrevisaoFimViagem.ToString("dd/MM/yyyy HH:mm");
            request.DataInicial = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            if (integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.TipoEnvio.HasValue 
                && integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.TipoEnvio.Value == TipoEnvioIntegracaoNeokohm.InicioViagem)
            {
                request.InicioCarga = true;
                request.FimCarga = false;
            } else if (integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.TipoEnvio.HasValue
                && integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.TipoEnvio.Value == TipoEnvioIntegracaoNeokohm.FimViagem)
            {
                request.InicioCarga = false;
                request.FimCarga = true;
            }
            request.Placa = integracao.PedidoOcorrenciaColetaEntrega.Carga.Veiculo?.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty;
            request.NumeroCarga = integracao.PedidoOcorrenciaColetaEntrega.Carga?.CodigoCargaEmbarcador ?? string.Empty;
            request.TemperaturaMinimaCarga = integracao.PedidoOcorrenciaColetaEntrega.Carga.TipoDeCarga?.FaixaDeTemperatura?.FaixaInicial ?? 0;
            request.TemperaturaMaximaCarga = integracao.PedidoOcorrenciaColetaEntrega.Carga.TipoDeCarga?.FaixaDeTemperatura?.FaixaFinal ?? 0;
            request.TemperaturaIdeal = null;
            request.Seca = false;
            request.Observacao = null;
            request.Origens = ObterOrigensPedidoOcorrencia(integracao.PedidoOcorrenciaColetaEntrega);
            request.Destinos = ObterDestinosPedidoOcorrencia(integracao.PedidoOcorrenciaColetaEntrega);

            return request;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens> ObterOrigens(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens> listaNeokohm = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = cargaDadosTransporteIntegracao.Carga.Pedidos.ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in pedidos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens neokohm = new Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens()
                {
                    IDUnidadeOrigem = pedido.Codigo,
                    LatitudeOrigem = pedido.Origem?.Latitude ?? 0,
                    LongitudeOrigem = pedido.Origem?.Longitude ?? 0,
                    NomeOrigem = pedido.ClienteColeta?.Descricao ?? string.Empty,
                    MunicipioOrigem = pedido.Origem?.LocalidadePolo.Descricao ?? string.Empty,
                    DataInicialOrigem = pedido.Carga.DataInicioViagemPrevista.HasValue ? pedido.Carga.DataInicioViagemPrevista.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    PrevisaoChegadaOrigem = pedido.Carga.DataInicioViagemPrevista.HasValue ? pedido.Carga.DataInicioViagemPrevista.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    OrdemOrigem = pedido.OrdemColeta,
                };

                listaNeokohm.Add(neokohm);
            }

            return listaNeokohm;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos> ObterDestinos(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos> listaNeokohm = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas = cargaDadosTransporteIntegracao.Carga.Entregas.ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in entregas)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos neokohm = new Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos()
                {
                    IDUnidadeDestino = entrega.Codigo,
                    LatitudeDestino = entrega.Cliente?.Latitude.ToDecimal() ?? 0,
                    LongitudeDestino = entrega.Cliente?.Longitude.ToDecimal() ?? 0,
                    NomeDestino = entrega.Cliente?.Descricao ?? string.Empty,
                    MunicipioDestino = entrega.Cliente?.Localidade?.Descricao ?? string.Empty,
                    DataFinalDestino = entrega.DataPrevista.HasValue ? entrega.DataPrevista.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    OrdemDestino = entrega.Ordem,
                };

                listaNeokohm.Add(neokohm);
            }

            return listaNeokohm;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens> ObterOrigensPedidoOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrencia)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens> listaNeokohm = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> coletas = pedidoOcorrencia.Carga.Pedidos.ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido coleta in coletas)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens neokohm = new Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Origens()
                {
                    IDUnidadeOrigem = coleta.Codigo,
                    LatitudeOrigem = coleta.Origem?.Latitude ?? 0,
                    LongitudeOrigem = coleta.Origem?.Longitude ?? 0,
                    NomeOrigem = coleta.ClienteColeta?.Descricao ?? string.Empty,
                    MunicipioOrigem = coleta.Origem?.LocalidadePolo?.Descricao ?? string.Empty,
                    DataInicialOrigem = coleta.Carga.DataInicioViagemPrevista.HasValue ? coleta.Carga.DataInicioViagemPrevista.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    PrevisaoChegadaOrigem = coleta.Carga.DataInicioViagemPrevista.HasValue ? coleta.Carga.DataInicioViagemPrevista.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    OrdemOrigem = coleta.OrdemColeta,
                };

                listaNeokohm.Add(neokohm);
            }

            return listaNeokohm;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos> ObterDestinosPedidoOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrencia)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos> listaNeokohm = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas = pedidoOcorrencia.Carga.Entregas.ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in entregas)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos neokohm = new Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm.Destinos()
                {
                    IDUnidadeDestino = entrega.Codigo,
                    LatitudeDestino = entrega.Cliente?.Latitude.ToDecimal() ?? 0,
                    LongitudeDestino = entrega.Cliente?.Longitude.ToDecimal() ?? 0,
                    NomeDestino = entrega.Cliente?.Descricao ?? string.Empty,
                    MunicipioDestino = entrega.Cliente?.Localidade?.Descricao ?? string.Empty,
                    DataFinalDestino = entrega.DataPrevista.HasValue ? entrega.DataPrevista.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    OrdemDestino = entrega.Ordem,
                };

                listaNeokohm.Add(neokohm);
            }

            return listaNeokohm;
        }

        #endregion Métodos Privados
    }
}
