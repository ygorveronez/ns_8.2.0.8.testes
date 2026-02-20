using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Newtonsoft.Json;
using Utilidades.Extensions;
using static Google.Apis.Requests.BatchRequest;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {
        private class LogIntegracao
        {
            public string NomeEtapa { get; set; }
            public string JsonEnvio { get; set; }
            public string JsonRetorno { get; set; }
        }
        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            string mensagemErro = string.Empty;
            int idViagem = 0;

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }
            var cargaPedidoPrimeiro = cargaIntegracao.Carga.Pedidos.FirstOrDefault();

            if (cargaPedidoPrimeiro == null)
                return;

            var pedido = cargaPedidoPrimeiro.Pedido;

            Dominio.Entidades.Cliente clienteOrigem;
            Dominio.Entidades.Cliente clienteDestino;
            Dominio.Entidades.Cliente clienteTomador = cargaPedidoPrimeiro.Tomador;

            bool temPontoPartida = cargaPedidoPrimeiro.PontoPartida != null;

            clienteOrigem = temPontoPartida
                ? cargaPedidoPrimeiro.PontoPartida
                : cargaPedidoPrimeiro.Expedidor ?? pedido.Expedidor ?? pedido.Remetente;

            clienteDestino = temPontoPartida
                ? cargaPedidoPrimeiro.Expedidor ?? pedido.Expedidor ?? pedido.Remetente
                : cargaPedidoPrimeiro.Recebedor ?? pedido.Recebedor ?? pedido.Destinatario;

            if (_configuracaoIntegracao.AplicarRegraLocalPalletizacao && pedido.LocalPaletizacao != null)
            {
                clienteDestino = pedido.LocalPaletizacao;
            }

            List<int> idVeiculos = null;
            List<int> idMotoristas = null;
            List<int> idColetasEntregas = null;
            int idClienteOrigem = 0;
            int idClienteOrigemEndereco = 0;
            int idClienteDestino = 0;
            int idClienteDestinoEndereco = 0;
            int idClienteTomador = 0;
            int idClienteTomadorEndereco = 0;
            int idRota = 0;
            var veiculos = new List<Dominio.Entidades.Veiculo>();

            veiculos.Add(cargaIntegracao.Carga.Veiculo);
            veiculos.AddRange(cargaIntegracao.Carga.VeiculosVinculados
                .OrderBy(v => v.TipoVeiculo));


            if (ObterToken(out mensagemErro) &&
                IntegrarVeiculos(veiculos, cargaIntegracao, out idVeiculos, out mensagemErro) &&
                IntegrarMotoristas(cargaIntegracao.Carga.Motoristas.ToList(), cargaIntegracao, out idMotoristas, out mensagemErro) &&
                IntegrarCliente(clienteOrigem, cargaIntegracao, out idClienteOrigem, out idClienteOrigemEndereco, out mensagemErro) &&
                IntegrarCliente(clienteDestino, cargaIntegracao, out idClienteDestino, out idClienteDestinoEndereco, out mensagemErro) &&
                IntegrarCliente(clienteTomador, cargaIntegracao, out idClienteTomador, out idClienteTomadorEndereco, out mensagemErro) &&
                IntegrarColetasEntregas(cargaIntegracao.Carga.CargaCTes.ToList(), cargaIntegracao, out idColetasEntregas, out mensagemErro) &&
                IntegrarRota(cargaIntegracao, cargaIntegracao.Carga.Rota, out idRota, out mensagemErro) &&
                IntegrarViagem(cargaIntegracao, cargaPedidoPrimeiro, idVeiculos, idMotoristas, idClienteOrigem, idClienteOrigemEndereco, idClienteDestino, idClienteDestinoEndereco, idClienteTomador, idClienteTomadorEndereco, idRota, idColetasEntregas, out idViagem, out mensagemErro))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaIntegracao.ProblemaIntegracao = "Viagem inserida com sucesso.";
                cargaIntegracao.Protocolo = idViagem.ToString();
            }
            else
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = mensagemErro;
            }

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas += 1;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }

            string mensagemErro = string.Empty;
            string jsonEnvio = string.Empty;
            string jsonRetorno = string.Empty;
            int idViagem = 0;
            List<int> idVeiculos = null;
            List<int> idMotoristas = null;
            List<int> idColetasEntregas = null;
            int idClienteOrigem = 0;
            int idClienteOrigemEndereco = 0;
            int idClienteDestino = 0;
            int idClienteDestinoEndereco = 0;
            int idClienteTomador = 0;
            int idClienteTomadorEndereco = 0;
            int idRota = 0;
            List<LogIntegracao> logsIntegracao = new();

            var cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao
            {
                Carga = cargaDadosTransporteIntegracao.Carga,
                NumeroTentativas = cargaDadosTransporteIntegracao.NumeroTentativas + 1,
                DataIntegracao = DateTime.Now
            };

            var cargaPedidoPrimeiro = cargaIntegracao.Carga.Pedidos.FirstOrDefault();
            if (cargaPedidoPrimeiro == null)
                return; 

            var pedido = cargaPedidoPrimeiro.Pedido;
            var clienteTomador = cargaPedidoPrimeiro.Tomador;

            bool temPontoPartida = cargaPedidoPrimeiro.PontoPartida != null;

            var clienteOrigem = temPontoPartida
                ? cargaPedidoPrimeiro.PontoPartida
                : cargaPedidoPrimeiro.Expedidor ?? pedido.Expedidor ?? pedido.Remetente;

            var clienteDestino = temPontoPartida
                ? cargaPedidoPrimeiro.Expedidor ?? pedido.Expedidor ?? pedido.Remetente
                : cargaPedidoPrimeiro.Recebedor ?? pedido.Recebedor ?? pedido.Destinatario;

            if (_configuracaoIntegracao.AplicarRegraLocalPalletizacao && pedido.LocalPaletizacao != null)
            {
                clienteDestino = pedido.LocalPaletizacao;
            }

            var veiculos = new List<Dominio.Entidades.Veiculo>();

            veiculos.Add(cargaIntegracao.Carga.Veiculo);
            veiculos.AddRange(cargaIntegracao.Carga.VeiculosVinculados
                .OrderBy(v => v.TipoVeiculo));

            if (ObterToken(out mensagemErro) &&
                IntegrarVeiculos(veiculos, cargaIntegracao, out idVeiculos, out mensagemErro, logsIntegracao) &&
                IntegrarMotoristas(cargaIntegracao.Carga.Motoristas.ToList(), cargaIntegracao, out idMotoristas, out mensagemErro, logsIntegracao) &&
                IntegrarCliente(clienteOrigem, cargaIntegracao, out idClienteOrigem, out idClienteOrigemEndereco, out mensagemErro, logsIntegracao) &&
                IntegrarCliente(clienteDestino, cargaIntegracao, out idClienteDestino, out idClienteDestinoEndereco, out mensagemErro, logsIntegracao) &&
                IntegrarCliente(clienteTomador, cargaIntegracao, out idClienteTomador, out idClienteTomadorEndereco, out mensagemErro, logsIntegracao) &&
                IntegrarColetasEntregas(cargaIntegracao.Carga.CargaCTes.ToList(), cargaIntegracao, out idColetasEntregas, out mensagemErro, logsIntegracao))
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Viagem inserida com sucesso.";
                cargaDadosTransporteIntegracao.Protocolo = idViagem.ToString();
            }
            else
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagemErro;
            }
            string jsonEnvioFinal = JsonConvert.SerializeObject(logsIntegracao.Select(x => new { x.NomeEtapa, x.JsonEnvio }));
            string jsonRetornoFinal = JsonConvert.SerializeObject(logsIntegracao.Select(x => new { x.NomeEtapa, x.JsonRetorno }));

            SalvarArquivosIntegracao(cargaDadosTransporteIntegracao, jsonEnvioFinal, jsonRetornoFinal);

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }
        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarViagem(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrimeiro, List<int> idVeiculos, List<int> idMotoristas, int idClienteOrigem, int idClienteOrigemEndereco, int idClienteDestino, int idClienteDestinoEndereco, int idClienteTomador, int idClienteTomadorEndereco, int idRota, List<int> idColetasEntregas, out int idViagem, out string mensagemErro)
        {
            mensagemErro = null;
            idViagem = 0;

            string jsonEnvio = string.Empty;
            string jsonRetorno = string.Empty;
            string mensagemLog = string.Empty;
            try
            {
                bool sucesso = false;

                object envioWS = ObterViagem(cargaIntegracao, cargaPedidoPrimeiro, idVeiculos, idMotoristas, idClienteOrigem, idClienteOrigemEndereco, idClienteDestino, idClienteDestinoEndereco, idClienteTomador, idClienteTomadorEndereco, idRota, idColetasEntregas);

                //Transmite o arquivo
                retornoWebService retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "viagens", this.tokenAutenticacao);

                jsonEnvio = retornoWS.jsonEnvio;
                jsonRetorno = retornoWS.jsonRetorno;

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    retError retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retError>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de integração de carga A52: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o envio da viagem; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        string mensagem = null;

                        int count = 0;
                        foreach (object message in retorno.message)
                        {
                            count++;

                            if (count == 1)
                                mensagem = message.ToString();
                        }

                        if (string.IsNullOrEmpty(mensagem))
                        {
                            mensagemErro = "Ocorreu uma falha ao efetuar o envio da viagem.";
                            sucesso = false;
                        }
                        else
                        {
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o envio da viagem; RetornoWS {0}.", mensagem);
                            sucesso = false;
                        }
                    }
                }
                else if (retornoWS.erro)
                {
                    mensagemErro = "Ocorreu uma falha ao efetuar o envio da viagem.";
                    sucesso = false;
                }
                else
                {
                    retViagem retViagem = retornoWS.jsonRetorno.ToString().FromJson<retViagem>();
                    idViagem = (int)retViagem.id;
                    mensagemLog = "Viagem: Viagem enviada com sucesso.";
                    sucesso = true;
                }

                SalvarArquivosIntegracao(cargaIntegracao, retornoWS.jsonEnvio, retornoWS.jsonRetorno, mensagemLog);

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do motorista.";
                return false;
            }
        }

        private envViagem ObterViagem(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrimeiro, List<int> idVeiculos, List<int> idMotoristas, int idClienteOrigem, int idClienteOrigemEndereco, int idClienteDestino, int idClienteDestinoEndereco, int idClienteTomador, int idClienteTomadorEndereco, int idRota, List<int> idColetasEntregas)
        {
            envViagem retorno = new envViagem();

            retorno.identificador = null;
            retorno.idOperacaoLogistico = cargaIntegracao.Carga.TipoOperacao.TipoOperacaoA52;

            // 0 - Vazio, 1 - Carregado
            retorno.tipoTransporte = 1;

            foreach (int idVeiculo in idVeiculos)
            {
                if (retorno.idVeiculo == null)
                    retorno.idVeiculo = idVeiculo;
                else if (retorno.idCarreta1 == null)
                    retorno.idCarreta1 = idVeiculo;
                else if (retorno.idCarreta2 == null)
                    retorno.idCarreta2 = idVeiculo;
                else
                    break;
            }

            foreach (int idMotorista in idMotoristas)
            {
                if (retorno.idMotorista == null)
                    retorno.idMotorista = idMotorista;
               
                else
                    break;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = null;

            if (_configuracaoIntegracao.UtilizarDataAgendamentoPedido)
                cargaPedidos = cargaIntegracao.Carga.Pedidos.OrderBy(o => o.Pedido.DataAgendamento).ToList();
            else
                cargaPedidos = cargaIntegracao.Carga.Pedidos.OrderBy(o => o.Pedido.DataPrevisaoSaida).ToList();

            DateTime? dataAgendamento = cargaPedidos.Max(o => o.Pedido.DataAgendamento);
            DateTime? dataPrevisaoInicio = cargaPedidoPrimeiro.Pedido.DataPrevisaoSaida;

            DateTime? dataAgendamentoMais3Horas = dataAgendamento?.AddHours(3);
            DateTime? dataPrevisaoInicioMais3Horas = dataPrevisaoInicio?.AddHours(3);


            retorno.dataPrevisaoInicio = dataPrevisaoInicioMais3Horas != null
             ? dataPrevisaoInicioMais3Horas.Value.ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T")
             : null;

            retorno.dataPrevisaoFim = dataAgendamentoMais3Horas != null
                ? dataAgendamentoMais3Horas.Value.ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T")
                : null;

            retorno.idClienteOrigem = idClienteOrigem;
            retorno.idClienteOrigemEndereco = idClienteOrigemEndereco;
            retorno.idClienteDestino = idClienteDestino;
            retorno.idClienteDestinoEndereco = idClienteDestinoEndereco;
            retorno.idRota = idRota;
            retorno.numeroPedido = cargaPedidoPrimeiro?.Pedido.Numero.ToString();
            retorno.numeroCarga = cargaIntegracao.Carga.CodigoCargaEmbarcador;
            retorno.dataPrevisaoChegadaOrigem = null;
            retorno.dataPrevisaoChegadaDestino = null;
            retorno.coletasEntregas = idColetasEntregas;
            retorno.preferencial = false;

            return retorno;
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string jsonRequisicao, string jsonRetorno, string mensagemLog)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, mensagemLog);

            if (arquivoIntegracao == null)
                return;

            if (cargaIntegracao.ArquivosTransacao == null)
                cargaIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, cargaDadosTransporteIntegracao.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (cargaDadosTransporteIntegracao.ArquivosTransacao == null)
                cargaDadosTransporteIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo AdicionarArquivoTransacao(string jsonRequisicao, string jsonRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        #endregion Métodos Privados
    }
}
