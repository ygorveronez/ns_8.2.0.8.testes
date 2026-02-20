using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento
{
    public class Monitoramento
    {
        #region Métodos públicos estáticos 
        public static Dominio.Entidades.Embarcador.Logistica.PosicaoAtual AtualizarDadosPosicaoAtual(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao status, bool ProcessarPosicoesDemaisPlacas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PosicaoAtual repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorVeiculo(posicao.Veiculo.Codigo);

            // Ainda não existe posição atual para o veículo.
            if (posicaoAtual == null)
            {
                posicaoAtual = new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual();
                NovaPosicaoAtualApartirDaPosicao(posicaoAtual, posicao, posicao.Veiculo.Codigo);
                posicaoAtual.Status = status;
                repositorioPosicaoAtual.Inserir(posicaoAtual);
            }

            // ... considera a posição recebida como posição atual apenas se for mais nova que a posição atual já existente (Há casos de posições recebidas fora de ordem)
            else if (posicao.DataVeiculo > posicaoAtual.DataVeiculo)
            {
                NovaPosicaoAtualApartirDaPosicao(posicaoAtual, posicao, posicao.Veiculo.Codigo);
                posicaoAtual.Status = status;
                repositorioPosicaoAtual.Atualizar(posicaoAtual);
            }

            if (ProcessarPosicoesDemaisPlacas)
                ValidarPosicaoAtualDemaisVeiculosDeMesmaPlaca(posicaoAtual, posicao, status, unitOfWork);

            return posicaoAtual;
        }

        public static void ExcluirMonitoriaPorCargas(List<int> cargas, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoTMS.PossuiMonitoramento)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                repMonitoramento.ExcluirTodosPorCargas(cargas);
            }
        }

        public static void ExcluirMonitoriaPorCarga(int carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoTMS.PossuiMonitoramento)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                repMonitoramento.ExcluirTodosPorCarga(carga);
            }
        }

        public static void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, string mensagemAuditoria, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!configuracao.PossuiMonitoramento)
                return;

            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarUltimoPorCarga(cargaAtual.Codigo);

            if (monitoramento != null)
            {
                mensagemAuditoria += $". De {cargaAtual.CodigoCargaEmbarcador} para {cargaNova.CodigoCargaEmbarcador}";

                AtualizarMonitoramento(monitoramento, cargaNova, DateTime.Now, null, mensagemAuditoria, configuracao, unitOfWork);
                AlertaMonitor.AlterarCarga(monitoramento.Carga, cargaNova, unitOfWork);
            }
            else
            {
                CriarMonitoramento(cargaNova, DateTime.Now, configuracao, null, mensagemAuditoria, unitOfWork);
            }
        }

        public static void GerarMonitoramentoEIniciar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork, bool estaIniciandoPelaDataCarregamento = false)
        {
            List<SituacaoCarga> situacoesCargaNaoPermitida = new List<SituacaoCarga>() { SituacaoCarga.Encerrada, SituacaoCarga.Cancelada, SituacaoCarga.Anulada };

            if (configuracao.PossuiMonitoramento && carga != null && carga.Veiculo != null && !situacoesCargaNaoPermitida.Contains(carga.SituacaoCarga))
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = GerarMonitoramento(carga, configuracao, auditoria, mensagemAuditoria, unitOfWork);

                Servicos.Log.TratarErro($"Monitoramento Gerado: {monitoramento?.Codigo}", "Monitoramento");

                if (configuracao.UtilizaAppTrizy && (carga.Filial?.HabilitarPreViagemTrizy ?? false))
                {
                    DateTime? dataInicioPreTripCargaCancelada = BuscarDataPreTripCargaCancelada(carga, repositorioCarga);
                    if (dataInicioPreTripCargaCancelada != null && dataInicioPreTripCargaCancelada != DateTime.MinValue)
                    {
                        carga.DataPreViagemInicio = dataInicioPreTripCargaCancelada;
                        repositorioCarga.Atualizar(carga);

                        Servicos.Log.TratarErro($"Monitoramento Possui Pre-trip de Carga anterior Cancelada: carga: {carga.Codigo}", "Monitoramento");
                    }
                }

                if (monitoramento != null && configuracao.QuandoIniciarMonitoramento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.NaoIniciar)
                {
                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                        return;

                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoIniciarViagem && !carga.DataInicioViagem.HasValue)
                        return;

                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.EstouIndoAoIniciarViagem && !carga.DataPreViagemInicio.HasValue && !carga.DataInicioViagem.HasValue)
                        return;

                    IniciarMonitoramento(monitoramento, DateTime.Now, configuracao, auditoria, unitOfWork, estaIniciandoPelaDataCarregamento);
                }
            }
        }
        //Async Method
        public static async Task GerarMonitoramentoEIniciarAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork, bool estaIniciandoPelaDataCarregamento = false)
        {
            if (configuracao.PossuiMonitoramento && carga != null && carga.Veiculo != null && carga.SituacaoCarga != SituacaoCarga.Cancelada)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = GerarMonitoramento(carga, configuracao, auditoria, mensagemAuditoria, unitOfWork);

                Servicos.Log.TratarErro($"Monitoramento Gerado: {monitoramento?.Codigo}", "Monitoramento");

                if (configuracao.UtilizaAppTrizy && (carga.Filial?.HabilitarPreViagemTrizy ?? false))
                {
                    DateTime? dataInicioPreTripCargaCancelada = BuscarDataPreTripCargaCancelada(carga, repositorioCarga);
                    if (dataInicioPreTripCargaCancelada != null && dataInicioPreTripCargaCancelada != DateTime.MinValue)
                    {
                        carga.DataPreViagemInicio = dataInicioPreTripCargaCancelada;
                        repositorioCarga.Atualizar(carga);

                        Servicos.Log.TratarErro($"Monitoramento Possui Pre-trip de Carga anterior Cancelada: carga: {carga.Codigo}", "Monitoramento");
                    }
                }

                if (monitoramento != null && configuracao.QuandoIniciarMonitoramento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.NaoIniciar)
                {
                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                        return;

                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoIniciarViagem && !carga.DataInicioViagem.HasValue)
                        return;

                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.EstouIndoAoIniciarViagem && !carga.DataPreViagemInicio.HasValue && !carga.DataInicioViagem.HasValue)
                        return;

                    IniciarMonitoramento(monitoramento, DateTime.Now, configuracao, auditoria, unitOfWork, estaIniciandoPelaDataCarregamento);
                }
            }
        }
        public static void GerarMonitoriaPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.PossuiMonitoramento && carga != null && carga.Veiculo != null && carga.SituacaoCarga != SituacaoCarga.Cancelada)
            {
                GerarMonitoramento(carga, configuracao, auditoria, mensagemAuditoria, unitOfWork);
            }
        }

        public static void CancelarMonitoramentoAoDisponibilizarTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.PossuiMonitoramento && carga != null)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarUltimoPorCarga(carga.Codigo);
                if (monitoramento != null && monitoramento.Veiculo != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega = repCargaEntrega.BuscarColetaNaOrigemPorCarga(monitoramento.Carga.Codigo);
                    if (entrega != null && (entrega.DataEntradaRaio.HasValue))
                    {
                        entrega.DataEntradaRaio = null;
                        repCargaEntrega.Atualizar(entrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(entrega, repCargaEntrega, unitOfWork);
                    }

                    Dominio.Entidades.Veiculo veiculoAntigo = monitoramento.Veiculo;
                    //monitoramento.Veiculo = null;
                    monitoramento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado;

                    Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem = MonitoramentoStatusViagem.OterStatusViagemPorTipoRegra(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Cancelada);
                    if (statusViagem != null)
                        monitoramento.StatusViagem = statusViagem;

                    FinalizaMonitoramentoVeiculo(unitOfWork, monitoramento, veiculoAntigo, DateTime.Now);
                    Auditoria.AuditarMonitoramento(monitoramento, mensagemAuditoria, auditoria, unitOfWork);

                    repMonitoramento.Atualizar(monitoramento);
                }
            }
        }

        public static void IniciarMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime? dataInicio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.PossuiMonitoramento && carga != null)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCargaAguardando(carga.Codigo);
                if (monitoramento != null)
                {
                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte && monitoramento.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                        return;

                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoIniciarViagem && !monitoramento.Carga.DataInicioViagem.HasValue)
                        return;

                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.EstouIndoAoIniciarViagem && !carga.DataPreViagemInicio.HasValue && !carga.DataInicioViagem.HasValue)
                        return;

                    Servicos.Log.TratarErro($"Iniciando Monitoramento {monitoramento.Codigo}");
                    IniciarMonitoramento(monitoramento, dataInicio ?? DateTime.Now, configuracao, auditoriaCarga, unitOfWork);
                }
            }
        }

        public static void FinalizarMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime? dataFim, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork, MotivoFinalizacaoMonitoramento motivoFinalizacaoMonitoramento)
        {
            if (configuracao.PossuiMonitoramento && carga != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCargaNaoFinalizado(carga.Codigo);
                if (monitoramento != null)
                {
                    FinalizarMonitoramento(monitoramento, dataFim ?? DateTime.Now, configuracao, auditoriaBase, mensagemAuditoria, unitOfWork, motivoFinalizacaoMonitoramento);
                    IniciarMonitoramentoRetorno(monitoramento, configuracao, auditoriaBase, unitOfWork);

                    if (configuracaoMonitoramento?.FinalizarAutomaticamenteAlertasDoMonitoramentoAoFinalizarViagem ?? false)
                        FinalizarAlertasPorMonitoramento(monitoramento, dataFim ?? DateTime.Now, "Finalizado automaticamente pelo fluxo de Finalizaçao de Viagem!", auditoriaBase, unitOfWork);
                }
            }
        }

        public static void FinalizarMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataFim, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork, MotivoFinalizacaoMonitoramento motivoFinalizacaoMonitoramento)
        {
            FinalizarUltimoMonitoramentoHistoricoStatusViagemDescarga(monitoramento, dataFim, unitOfWork);
            FinalizarMonitoramento(monitoramento, dataFim, unitOfWork, motivoFinalizacaoMonitoramento, auditoriaBase);
            Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento finalizado. " + mensagemAuditoria, auditoriaBase, unitOfWork);
            if ((monitoramento.Carga?.TipoOperacao?.ConfiguracaoControleEntrega?.FinalizarControleEntregaAoFinalizarMonitoramentoCarga ?? false) && monitoramento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
            {
                FinalizarControleEntrega(monitoramento, auditoriaBase, unitOfWork);
            }
            IniciarProximoMonitoramentoAgendadoDoVeiculo(monitoramento.Veiculo, dataFim, auditoriaBase, configuracao, unitOfWork);
        }

        public static void CancelarMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, bool iniciarProximoMonitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.PossuiMonitoramento && carga != null)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarPorCargaNaoFinalizado(carga.Codigo);
                if (monitoramento != null)
                {
                    DateTime dataFim = DateTime.Now;
                    CancelarMonitoramento(monitoramento, dataFim, unitOfWork, MotivoFinalizacaoMonitoramento.FinalizadoAoCancelarMonitoramento, auditoriaBase);
                    Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento cancelado. " + mensagemAuditoria, auditoriaBase, unitOfWork);

                    if (iniciarProximoMonitoramento)
                        IniciarProximoMonitoramentoAgendadoDoVeiculo(monitoramento.Veiculo, dataFim, auditoriaBase, configuracao, unitOfWork);
                }
            }
        }

        public static DateTime ObterMenorDataDePosicaoDoMonitoramento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataFinal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            DateTime menorData = dataFinal;
            if (configuracaoEmbarcador.PossuiMonitoramento)
            {
                // Busca a data da primeira posição após o início do monitoramento
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarUltimoPorCarga(carga.Codigo);
                if (monitoramento != null)
                {
                    Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes = repPosicao.BuscarPorVeiculoDataInicialeFinal(monitoramento.Veiculo.Codigo, monitoramento.DataInicio ?? monitoramento.DataCriacao ?? carga.DataCriacaoCarga, dataFinal);
                    if (posicoes.Count > 0)
                        menorData = posicoes.FirstOrDefault().DataVeiculo;
                }
            }
            return menorData;
        }

        public static Dominio.Entidades.Cliente BuscarClienteOrigemDaCargaPeloPedido(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga != null)
            {
                return BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, carga.Codigo);
            }
            return null;
        }

        public static Dominio.Entidades.Cliente BuscarClienteOrigemDaCargaPeloPedido(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Cliente clienteOrigem = repCargaPedido.BuscarClienteDaPrimeiraPorCarga(codigoCarga);
                return clienteOrigem;
            }
            return null;
        }

        public static List<Tuple<int, Dominio.Entidades.Cliente>> BuscarClienteOrigemDasCargasPeloPedido(Repositorio.UnitOfWork unitOfWork, List<int> codigosCarga)
        {
            List<Tuple<int, Dominio.Entidades.Cliente>> clientesOrigem = new();
            if (codigosCarga.Count > 0)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarCargasPedidoPorCodigosCargaFetch(codigosCarga);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    int index = clientesOrigem.FindIndex(c => c.Item1 == cargaPedido.Carga.Codigo);
                    if (index < 0)
                        clientesOrigem.Add(new Tuple<int, Dominio.Entidades.Cliente>(cargaPedido.Carga.Codigo, cargaPedido.Expedidor != null ? cargaPedido.Expedidor : cargaPedido.Pedido.Remetente));
                }
                return clientesOrigem;
            }
            return null;
        }

        public static double? BuscarCodigoClienteOrigemDaCargaPeloPedido(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                return repCargaPedido.BuscarCodigoClienteDaPrimeiraPorCarga(codigoCarga);
            }
            return null;
        }

        public static bool DeveProcessarTrocaDeAlvo(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return (!configuracao.NaoProcessarTrocaAlvoViaMonitoramento && (tipoOperacao == null || !tipoOperacao.NaoProcessarTrocaAlvoViaMonitoramento));
        }

        public static bool DeveProcessarTrocaDeAlvo(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao)
        {
            return (!configuracao.NaoProcessarTrocaAlvoViaMonitoramento && !naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao);
        }

        /// <summary>
        /// Atualizacao de N monitoramentos, Quando chamado por WEBSevices e projetos secundarios
        /// </summary>
        /// <param name="ListaCodigosCarga"></param>
        /// <param name="codigoClienteMultisoftware"></param>
        /// <param name="unitOfWork"></param>
        public void InformarAtualizacaoListaMonitoramentosMSMQ(List<int> ListaCodigosCarga, int codigoClienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork).BuscarConfiguracaoPadrao();

            if (!configuracaoMonitoramento.TelaMonitoramentoAtualizarGridAoReceberAtualizacoesOnTime) return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

            dynamic ListaMonitoramentos = repositorioMonitoramento.ConsultarOnTime(ListaCodigosCarga, configuracao);

            dynamic objeto = new System.Dynamic.ExpandoObject();
            objeto.objetoMonitoramento = ListaMonitoramentos;

            Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(objeto, codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.Monitoramento, Servicos.SignalR.Hubs.Monitoramento.GetHub(Servicos.SignalR.Hubs.MonitoramentoHubs.ListaMonitoramentosAtualizados));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
        }

        public void RegistrarPosicaoEventosRelevantesTrizy(int codigoCarga, DateTime dataPosicao, double? latitude, double? longitude, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoRelevanteMonitoramento evento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao repMonitoramentoVeiculoPosicao = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);

            try
            {
                //Validação de Latitude e Longitude.
                if (!latitude.HasValue || !longitude.HasValue || (double)latitude == 0 || (double)longitude == 0)
                    return;

                //Validação de Monitoramento com veículo definido.
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigoCarga(codigoCarga);
                if (monitoramento == null || monitoramento?.Veiculo == null)
                    return;

                Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo monitoramentoVeiculo = repMonitoramentoVeiculo.BuscarAbertoPorMonitoramento(monitoramento.Codigo);
                if (monitoramentoVeiculo == null)
                    return;

                //Vamos criar uma posicao com a lat e log do evento.
                Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao = new Dominio.Entidades.Embarcador.Logistica.Posicao()
                {
                    Data = dataPosicao,
                    DataVeiculo = dataPosicao,
                    DataCadastro = DateTime.Now,
                    IDEquipamento = monitoramento.Veiculo.Codigo.ToString(),
                    Descricao = $"{latitude.ToString()}, {longitude.ToString()} ({evento.ObterDescricao()})",
                    Veiculo = monitoramento.Veiculo,
                    Latitude = (double)latitude,
                    Longitude = (double)longitude,
                    Processar = ProcessarPosicao.Processado,
                    Rastreador = EnumTecnologiaRastreador.Mobile
                };

                repPosicao.Inserir(novaPosicao);

                Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao monitoramentoVeiculoPosicao = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao
                {
                    MonitoramentoVeiculo = monitoramentoVeiculo,
                    Posicao = novaPosicao
                };

                repMonitoramentoVeiculoPosicao.Inserir(monitoramentoVeiculoPosicao);
            }
            catch (ServicoException)
            {
                //enterrar a excecao pois o processo precisa continuar..
            }
            catch (Exception ex)
            {
                //enterrar a excecao pois o processo precisa continuar..
                Servicos.Log.TratarErro(ex, "IntegracaoTrizy");
            }
        }

        public static void FinalizarAlertasPorMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataFim, string observacao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramento == null) return;

            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertasEmAberto = repAlertaMonitor.BuscarAlertasEmAbertoPorCarga(monitoramento.Carga?.Codigo ?? 0);
            new Servicos.Embarcador.Monitoramento.AlertaMonitor().FinalizarAlertas(alertasEmAberto, DateTime.Now, "Finalizado pelo fluxo de Finalizaçao de Viagem!", unitOfWork, auditoriaBase?.Usuario, false, null, null, true);
        }

        #endregion

        #region Métodos privados

        private static Dominio.Entidades.Embarcador.Logistica.Monitoramento GerarMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga == null || carga.Veiculo == null || (carga.TipoOperacao != null && carga.TipoOperacao.RetornoVazio && !configuracao.GerarMonitoramentoParaCargaRetornoVazio))
                return null;

            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.NaoGerarMonitoramento)
                return null;

            DateTime dataAtual = DateTime.Now;
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarUltimoPorCarga(carga.Codigo);

            Servicos.Log.TratarErro($"Gerando Monitoramento Carga: {carga.Codigo}", "Monitoramento");

            if (monitoramento == null || monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado || monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado)
                return CriarMonitoramento(carga, dataAtual, configuracao, auditoriaBase, mensagemAuditoria, unitOfWork);

            switch (monitoramento.Status)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado:
                    AtualizarMonitoramento(monitoramento, carga, dataAtual, auditoriaBase, mensagemAuditoria, configuracao, unitOfWork);
                    break;
            }

            return monitoramento;
        }
        private static async Task<Dominio.Entidades.Embarcador.Logistica.Monitoramento> GerarMonitoramentoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga == null || carga.Veiculo == null || (carga.TipoOperacao != null && carga.TipoOperacao.RetornoVazio && !configuracao.GerarMonitoramentoParaCargaRetornoVazio))
                return null;

            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.NaoGerarMonitoramento)
                return null;

            DateTime dataAtual = DateTime.Now;
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = await repositorioMonitoramento.BuscarUltimoPorCargaAsync(carga.Codigo);

            Servicos.Log.TratarErro($"Gerando Monitoramento Carga: {carga.Codigo}", "Monitoramento");

            if (monitoramento == null || monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado || monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado)
                return CriarMonitoramento(carga, dataAtual, configuracao, auditoriaBase, mensagemAuditoria, unitOfWork);

            switch (monitoramento.Status)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado:
                    AtualizarMonitoramento(monitoramento, carga, dataAtual, auditoriaBase, mensagemAuditoria, configuracao, unitOfWork);
                    break;
            }

            return monitoramento;
        }

        private static DateTime? BuscarDataPreTripCargaCancelada(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.Embarcador.Cargas.Carga repositorioCarga)
        {
            DateTime? dataInicioPreTripCargaCancelada = repositorioCarga.BuscarDataPreTripCargaCanceladaPorCodigoEmbarcadorEVeiculo(carga.CodigoCargaEmbarcador, carga.Veiculo.Codigo);

            return dataInicioPreTripCargaCancelada;
        }

        public static void AtualizarMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime data, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria, string mensagemAuditoria, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repositorioCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repositorioCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Veiculo veiculoAntigo = monitoramento.Veiculo;

            monitoramento.Initialize();
            monitoramento.Carga = carga;
            monitoramento.Veiculo = carga.Veiculo;
            monitoramento.Critico = VerificarSeOMonitoramentoECritico(monitoramento, unitOfWork);

            if (cargaRotaFrete != null)
            {
                monitoramento.DistanciaPrevista = Rota.BuscarDistanciaRotaMonitoramento(cargaRotaFrete.Codigo, carga, configuracao, unitOfWork);
                monitoramento.PontosPrevistos = Servicos.Embarcador.Carga.RotaFrete.ObterPontosPassagemCargaRotaFreteSerializada(cargaRotaFrete, unitOfWork);
                monitoramento.PolilinhaPrevista = cargaRotaFrete.PolilinhaRota;
            }

            repositorioMonitoramento.Atualizar(monitoramento);
            Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento atualizado. " + mensagemAuditoria, auditoria, unitOfWork);

            // Removeu o veículo da carga
            if (veiculoAntigo != null && monitoramento.Veiculo == null)
            {
                FinalizaMonitoramentoVeiculo(unitOfWork, monitoramento, veiculoAntigo, data);
            }
            // ... alterou ou informou o veículo para a carga
            else if (monitoramento.Veiculo != null && (veiculoAntigo == null || veiculoAntigo.Codigo != monitoramento.Veiculo.Codigo))
            {
                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosEmAberto = (configuracao.LimitarApenasUmMonitoramentoPorPlaca) ?
                    repositorioMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlaca(monitoramento.Veiculo.Placa) : repositorioMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculo(monitoramento.Veiculo);

                if (monitoramentosEmAberto != null && monitoramentosEmAberto.Count > 0)
                {
                    if (!monitoramento.Carga.CargaDePreCarga && configuracao.FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar)
                    {

                        //cargas que estao em agrupamento nao podem finalizar monitoramentos
                        if ((monitoramento.Carga.CargaAgrupamento != null) ||
                            (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte) ||
                            (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoIniciarViagem) ||
                            (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.EstouIndoAoIniciarViagem))
                        {
                            SubstituirVeiculo(monitoramento, data, unitOfWork);
                            return;
                        }

                        string msg = "Monitoramento finalizado para iniciar o monitoramento " + monitoramento.Codigo + ", carga " + monitoramento.Carga.CodigoCargaEmbarcador + " após substituir o veículo.";
                        monitoramentosEmAberto = monitoramentosEmAberto.Where(x => x.Codigo != monitoramento.Codigo).ToList();
                        FinalizarMonitoramentos(monitoramentosEmAberto, data, auditoria, msg, unitOfWork);
                    }
                    else if (monitoramento.Carga.CargaDePreCarga || (!configuracao.FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar && configuracao.LimitarApenasUmMonitoramentoPorPlaca))
                    {
                        monitoramentosEmAberto = monitoramentosEmAberto.Where(x => x.Codigo != monitoramento.Codigo).ToList();
                        foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramentoaberto in monitoramentosEmAberto)
                        {
                            //possui outro monitoramento iniciado, deve voltar esse para aguardando.
                            if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado)
                            {
                                monitoramento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando;
                                repositorioMonitoramento.Atualizar(monitoramento);
                                Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento atualizado para aguardando após substituir o veículo, pois já possuí monitoramento iniciado para a placa" + mensagemAuditoria, auditoria, unitOfWork);
                                break;
                            }
                        }
                    }
                }

                SubstituirVeiculo(monitoramento, data, unitOfWork);
                IniciarProximoMonitoramentoAgendadoDoVeiculo(veiculoAntigo, data, null, configuracao, unitOfWork);
            }
        }

        private static Dominio.Entidades.Embarcador.Logistica.Monitoramento CriarMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataCriacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga != null && carga.Veiculo != null)
            {
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
                if (cargaRotaFrete != null)
                {

                    Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento;

                    // Já existe um monitoramento de retorno para o veículo?
                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosVeiculo = repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculo(carga.Veiculo);
                    if (monitoramentosVeiculo != null && monitoramentosVeiculo.Count > 0 && monitoramentosVeiculo.First().StatusViagem != null && monitoramentosVeiculo.First().StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando)
                    {
                        monitoramento = monitoramentosVeiculo.First();
                        monitoramento.Carga = carga;
                        monitoramento.Critico = carga.CargaCritica ?? false;
                        if (monitoramento.StatusViagem == null) monitoramento.StatusViagem = MonitoramentoStatusViagem.OterStatusViagemEmViagem(unitOfWork);
                        repMonitoramento.Atualizar(monitoramento);
                    }
                    else
                    {
                        if (carga?.TipoOperacao?.NaoGerarControleColetaEntrega ?? false)
                            return null;

                        // Novo monitoramento
                        Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem = MonitoramentoStatusViagem.OterStatusViagemSemViagem(unitOfWork);
                        decimal distancia = Rota.BuscarDistanciaRotaMonitoramento(cargaRotaFrete.Codigo, carga, configuracao, unitOfWork);
                        string pontosRota = Servicos.Embarcador.Carga.RotaFrete.ObterPontosPassagemCargaRotaFreteSerializada(cargaRotaFrete, unitOfWork);
                        monitoramento = InserirMonitoramento(unitOfWork, auditoriaBase, mensagemAuditoria, carga, carga.Veiculo, dataCriacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando, statusViagem, pontosRota, cargaRotaFrete.PolilinhaRota, distancia);

                        if (monitoramento == null)
                            return null;
                    }

                    // Histórico do monitoramento
                    MonitoramentoStatusViagem.RegistraHistorico(unitOfWork, monitoramento, monitoramento.StatusViagem);
                    Carga.AtualizarCargaPeloMonitoramento(monitoramento, unitOfWork);

                    return monitoramento;
                }
            }
            return null;
        }
        //Async Method
        private static async Task<Dominio.Entidades.Embarcador.Logistica.Monitoramento> CriarMonitoramentoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataCriacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga != null && carga.Veiculo != null)
            {
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = await repCargaRotaFrete.BuscarPorCargaAsync(carga.Codigo);
                if (cargaRotaFrete != null)
                {

                    Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento;

                    // Já existe um monitoramento de retorno para o veículo?
                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosVeiculo =
                        await repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoAsync(carga.Veiculo);
                    if (monitoramentosVeiculo != null && monitoramentosVeiculo.Count > 0 && monitoramentosVeiculo.First().StatusViagem != null && monitoramentosVeiculo.First().StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando)
                    {
                        monitoramento = monitoramentosVeiculo.First();
                        monitoramento.Carga = carga;
                        monitoramento.Critico = carga.CargaCritica ?? false;
                        if (monitoramento.StatusViagem == null) monitoramento.StatusViagem = await MonitoramentoStatusViagem.ObterStatusViagemEmViagemAsync(unitOfWork);
                        await repMonitoramento.AtualizarAsync(monitoramento);
                    }
                    else
                    {
                        if (carga?.TipoOperacao?.NaoGerarControleColetaEntrega ?? false)
                            return null;

                        // Novo monitoramento
                        Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem = await MonitoramentoStatusViagem.ObterStatusViagemSemViagemAsync(unitOfWork);
                        decimal distancia = Rota.BuscarDistanciaRotaMonitoramento(cargaRotaFrete.Codigo, carga, configuracao, unitOfWork);
                        string pontosRota = await Servicos.Embarcador.Carga.RotaFrete.ObterPontosPassagemCargaRotaFreteSerializadaAsync(cargaRotaFrete, unitOfWork);
                        monitoramento = InserirMonitoramento(unitOfWork, auditoriaBase, mensagemAuditoria, carga, carga.Veiculo, dataCriacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando, statusViagem, pontosRota, cargaRotaFrete.PolilinhaRota, distancia);

                        if (monitoramento == null)
                            return null;
                    }

                    // Histórico do monitoramento
                    MonitoramentoStatusViagem.RegistraHistorico(unitOfWork, monitoramento, monitoramento.StatusViagem);
                    Carga.AtualizarCargaPeloMonitoramento(monitoramento, unitOfWork);

                    return monitoramento;
                }
            }
            return null;
        }

        private static void CriarMonitoramentoVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataInicio, DateTime? dataFim = null)
        {
            if (monitoramento != null && monitoramento.Veiculo != null)
            {
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo monitoramentoVeiculo = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo();
                monitoramentoVeiculo.Monitoramento = monitoramento;
                monitoramentoVeiculo.Veiculo = monitoramento.Veiculo;
                monitoramentoVeiculo.DataInicio = dataInicio;
                monitoramentoVeiculo.DataFim = dataFim;

                Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
                repMonitoramentoVeiculo.Inserir(monitoramentoVeiculo);
            }
        }

        private static void FinalizaMonitoramentoVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Veiculo veiculo, DateTime data)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> monitoramentoVeiculos = repMonitoramentoVeiculo.BuscarTodosPorMonitoramentoVeiculo(monitoramento.Codigo, veiculo.Codigo);
            FinalizaMonitoramentoVeiculo(unitOfWork, monitoramentoVeiculos, data);
        }

        private static void ValidarPerdaSinalMonitoramento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento repPerdaSinalMonitoramento = new Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento monioramentoPerdaSinalAberto = repPerdaSinalMonitoramento.BuscarUltimoPorMonitoramentoEmAberto(monitoramento.Codigo);

            if (monioramentoPerdaSinalAberto != null)
            {
                monioramentoPerdaSinalAberto.MonitoramentoFinalizadoComPerdaSinalAberto = true;

                repPerdaSinalMonitoramento.Atualizar(monioramentoPerdaSinalAberto);
            }
            //FinalizaMonitoramentoVeiculo(unitOfWork, monitoramentoVeiculos, data);
        }

        private static void FinalizaMonitoramentoVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime data)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> monitoramentoVeiculos = repMonitoramentoVeiculo.BuscarAbertosPorMonitoramento(monitoramento.Codigo);
            FinalizaMonitoramentoVeiculo(unitOfWork, monitoramentoVeiculos, data);
        }

        private static void FinalizaMonitoramentoVeiculo(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> monitoramentoVeiculos, DateTime data)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
            int total = monitoramentoVeiculos?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentoVeiculos[i].DataFim == null)
                {
                    monitoramentoVeiculos[i].DataFim = data;
                    repMonitoramentoVeiculo.Atualizar(monitoramentoVeiculos[i]);
                }
            }
        }

        private static void SubstituirVeiculo(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            FinalizaMonitoramentoVeiculo(unitOfWork, monitoramento, data);
            CriarMonitoramentoVeiculo(unitOfWork, monitoramento, data);
        }

        private static void SubstituirVeiculoGerandoNovoMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime data, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            // Cancela o monitoramento atual
            monitoramento.Initialize();
            CancelarMonitoramento(monitoramento, data, unitOfWork, MotivoFinalizacaoMonitoramento.FinalizadoPorSubstituicao, auditoriaBase);
            Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento cancelado por substituição de veículo", auditoriaBase, unitOfWork);

            // Gera um novo monitoramento para a mesma carga mas os novos dados da carga
            Dominio.Entidades.Embarcador.Logistica.Monitoramento novoMonitoramento = CriarMonitoramento(carga, data, configuracao, auditoriaBase, "Veículo substituído", unitOfWork);
            IniciarMonitoramentoAguardando(novoMonitoramento, data, configuracao, auditoriaBase, unitOfWork);
            Auditoria.AuditarMonitoramento(novoMonitoramento, "Monitoramento criado e iniciado por substituição de veículo", auditoriaBase, unitOfWork);
            Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento criado e iniciado por substituição de veículo", auditoriaBase, unitOfWork);
        }

        private static void CriarMonitoramentoRetornoAutomatico(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramentoAnterior, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime dataAtual = DateTime.Now;

            // Identificar o alvo que o veículo se encontrava antes de encerrar o monitoramento
            DateTime inicio = monitoramentoAnterior?.DataInicio ?? DateTime.MinValue;
            DateTime fim = monitoramentoAnterior?.DataFim ?? DateTime.MinValue;
            if (inicio == DateTime.MinValue || fim == DateTime.MinValue) return;

            // Confirma que o monitoramento anterior não era um monitoramento de retorno
            if (monitoramentoAnterior.Carga.TipoOperacao != null && monitoramentoAnterior.Carga.TipoOperacao.RetornoVazio) return;

            // Confirma que o veículo possui contrato de frete
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarContratosPorVeiculo(dataAtual, monitoramentoAnterior.Carga.Veiculo.Codigo);
            if (contrato == null) return;

            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            List<Dominio.Entidades.Cliente> clientesFinais = repPosicao.BuscarUltimosAlvosPorVeiculoDataInicialeFinal(monitoramentoAnterior.Carga.Veiculo.Codigo, inicio, fim);
            if (clientesFinais == null || clientesFinais.Count == 0) return;

            // Buca a origem da viagem
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(monitoramentoAnterior.Carga.Codigo);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem reCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = reCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);
            Dominio.Entidades.Cliente clienteInicial = pontosPassagem.First().Cliente;

            // Encontrar a rota entre o último alvo e a origem da viagem
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorRemetentesDestinatario(clientesFinais, clienteInicial.CPF_CNPJ);
            string polilinha = null;
            decimal distancia = 0;
            if (rotaFrete != null)
            {
                polilinha = rotaFrete.PolilinhaRota;
                distancia = rotaFrete.Quilometros;
            }
            else
            {
                // Se não encontrar uma rota pré-definida, gera uma rota
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao response = Rota.Roteirizar(clientesFinais.First(), clienteInicial, unitOfWork);
                if (response.IsSuccess())
                {
                    polilinha = response.Polilinha;
                    distancia = response.Distancia;
                }

            }

            // Gerar o monitoramento de retorno
            if (polilinha != null && distancia > 0)
            {

                // Pontos previstos
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>();
                pontosDaRota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota(clientesFinais.First()));
                pontosDaRota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota(clienteInicial));
                string pontosRota = Newtonsoft.Json.JsonConvert.SerializeObject(pontosDaRota);

                // Monitoramento
                Dominio.Entidades.Embarcador.Logistica.Monitoramento novoMonitoramento = InserirMonitoramento(unitOfWork, auditoriaBase, "Retorno automático.", null, monitoramentoAnterior.Veiculo, dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado, MonitoramentoStatusViagem.OterStatusViagemRetornando(unitOfWork), pontosRota, polilinha, distancia, dataAtual);

                if (novoMonitoramento == null)
                    return;

                // Registra o histórico do status de viagem
                Dominio.Entidades.Embarcador.Logistica.Posicao posicao = repPosicao.BuscarUltimaPosicaoDataVeiculo(novoMonitoramento.Veiculo.Codigo);
                MonitoramentoStatusViagem.RegistraHistorico(unitOfWork, novoMonitoramento, novoMonitoramento.StatusViagem, posicao);

            }

        }

        private static Dominio.Entidades.Embarcador.Logistica.Monitoramento InserirMonitoramento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo, DateTime dataCriacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus status, Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem, string pontosPrevistos, string polilinhaPrevista, decimal? distanciaPrevista = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracao = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
            if (configuracao?.NaoGerarNovoMonitoramentoCarga ?? false)
            {
                //validar se carga ja possui monitoramento finalizado.
                if (repMonitoramento.validarExisteMonitoramentoFinalizadoPorCarga(carga.Codigo))
                    return null;
            }


            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = new Dominio.Entidades.Embarcador.Logistica.Monitoramento();
            monitoramento.Initialize();
            monitoramento.DataCriacao = dataCriacao;
            monitoramento.Status = status;
            monitoramento.StatusViagem = statusViagem;
            monitoramento.Carga = carga;
            monitoramento.Veiculo = veiculo;
            monitoramento.PontosPrevistos = pontosPrevistos;
            monitoramento.PolilinhaPrevista = polilinhaPrevista;
            monitoramento.DistanciaPrevista = distanciaPrevista;
            monitoramento.DataInicio = dataInicio;
            monitoramento.DataFim = dataFim;
            monitoramento.Critico = VerificarSeOMonitoramentoECritico(monitoramento, unitOfWork);

            repMonitoramento.Inserir(monitoramento);

            CriarMonitoramentoVeiculo(unitOfWork, monitoramento, dataCriacao);

            Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento criado. " + mensagemAuditoria, auditoriaBase, unitOfWork);
            return monitoramento;
        }
        //Async Method
        private static async Task<Dominio.Entidades.Embarcador.Logistica.Monitoramento> InserirMonitoramentoAsync(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo, DateTime dataCriacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus status, Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem, string pontosPrevistos, string polilinhaPrevista, decimal? distanciaPrevista = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracao = await repConfiguracaoMonitoramento.BuscarConfiguracaoPadraoAsync();
            if (configuracao?.NaoGerarNovoMonitoramentoCarga ?? false)
            {
                //validar se carga ja possui monitoramento finalizado.
                if (await repMonitoramento.validarExisteMonitoramentoFinalizadoPorCargaAsync(carga.Codigo))
                    return null;
            }


            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = new Dominio.Entidades.Embarcador.Logistica.Monitoramento();
            monitoramento.Initialize();
            monitoramento.DataCriacao = dataCriacao;
            monitoramento.Status = status;
            monitoramento.StatusViagem = statusViagem;
            monitoramento.Carga = carga;
            monitoramento.Veiculo = veiculo;
            monitoramento.PontosPrevistos = pontosPrevistos;
            monitoramento.PolilinhaPrevista = polilinhaPrevista;
            monitoramento.DistanciaPrevista = distanciaPrevista;
            monitoramento.DataInicio = dataInicio;
            monitoramento.DataFim = dataFim;
            monitoramento.Critico = VerificarSeOMonitoramentoECritico(monitoramento, unitOfWork);

            repMonitoramento.Inserir(monitoramento);

            CriarMonitoramentoVeiculo(unitOfWork, monitoramento, dataCriacao);

            Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento criado. " + mensagemAuditoria, auditoriaBase, unitOfWork);
            return monitoramento;
        }

        private static bool VerificarSeOMonitoramentoECritico(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramento.Carga.CargaCritica ?? false)
                return true;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioNFe = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

            if (configuracaoMonitoramento == null || configuracaoMonitoramento.ValorMinimoMonitoramentoCritico == 0)
                return false;

            decimal valorTotalNotasCargas = repositorioNFe.BuscarTotalPorCarga(monitoramento.Carga.Codigo);

            return valorTotalNotasCargas > configuracaoMonitoramento.ValorMinimoMonitoramentoCritico;
        }
        private static async Task<bool> VerificarSeOMonitoramentoECriticoAsync(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramento.Carga.CargaCritica ?? false)
                return true;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioNFe = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = await repConfiguracaoMonitoramento.BuscarConfiguracaoPadraoAsync();

            if (configuracaoMonitoramento == null || configuracaoMonitoramento.ValorMinimoMonitoramentoCritico == 0)
                return false;

            decimal valorTotalNotasCargas = await repositorioNFe.BuscarTotalPorCargaAsync(monitoramento.Carga.Codigo);

            return valorTotalNotasCargas > configuracaoMonitoramento.ValorMinimoMonitoramentoCritico;
        }

        private static void IniciarMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataInicio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Repositorio.UnitOfWork unitOfWork, bool estaIniciandoPelaDataCarregamento = false)
        {
            if (monitoramento != null && monitoramento.Carga != null && monitoramento.Veiculo != null && monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando)
            {
                if (monitoramento.Carga.TipoOperacao != null && monitoramento.Carga.TipoOperacao.IniciarMonitoramentoAutomaticamenteDataCarregamento && !estaIniciandoPelaDataCarregamento)//inicia pela thread MonitorarMonitoramentos apenas na data do carregamento!
                    return;

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosEmAberto = (configuracao.LimitarApenasUmMonitoramentoPorPlaca) ?
                    repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlaca(monitoramento.Veiculo.Placa) : repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculo(monitoramento.Veiculo);

                if (monitoramentosEmAberto.Count == 0)
                {
                    // Deve iniciar o monitoramento pois não há nenhum outro monitoramento em aberto para o veículo
                    IniciarMonitoramentoAguardando(monitoramento, dataInicio, configuracao, auditoriaBase, unitOfWork);
                    Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento iniciado", auditoriaBase, unitOfWork);
                }
                else
                {
                    // Não deve iniciar se a carga a ser iniciada for de retorno vazio havendo outra carga para o veículo com monitoramento em aberto
                    if (monitoramento.Carga.TipoOperacao != null && monitoramento.Carga.TipoOperacao.RetornoVazio)
                        return;

                    //cargas que estao em agrupamento nao devem alterar outros monitoramentos
                    if (monitoramento.Carga.CargaAgrupamento != null)
                        return;

                    // Se a carga a ser iniciada não for pré-carga deve iniciar o monitoramento (encerrando outras em andamento)
                    else if (!monitoramento.Carga.CargaDePreCarga && configuracao.FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar)
                    {
                        string msg = "Monitoramento finalizado para iniciar o monitoramento " + monitoramento.Codigo + ", carga " + monitoramento.Carga.CodigoCargaEmbarcador;

                        FinalizarMonitoramentos(monitoramentosEmAberto, dataInicio, auditoriaBase, msg, unitOfWork);

                        List<int> codigosMonitoramentos = (from row in monitoramentosEmAberto select row.Codigo).ToList();
                        IniciarMonitoramentoAguardando(monitoramento, dataInicio, configuracao, auditoriaBase, unitOfWork);
                        Auditoria.AuditarMonitoramento(monitoramento, "Monitoramento iniciado. Finalizado(s) o(s) monitoramento(s) " + String.Join(",", codigosMonitoramentos) + " iniciados.", auditoriaBase, unitOfWork);
                    }
                }
            }
        }

        private static void IniciarMonitoramentoAguardando(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataInicio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosEmAberto = (configuracao.LimitarApenasUmMonitoramentoPorPlaca) ?
                    repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlaca(monitoramento.Veiculo.Placa) : repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculo(monitoramento.Veiculo);
                if (!monitoramento.Carga.CargaDePreCarga && configuracao.FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar)
                {
                    //cargas que estao em agrupamento nao devem alterar outros monitoramentos
                    if (monitoramento.Carga.CargaAgrupamento != null)
                        return;

                    FinalizarMonitoramentos(monitoramentosEmAberto, dataInicio, auditoriaBase, "Monitoramento finalizado para manter apenas um monitoramento em andamento por veículo.", unitOfWork);
                }

                Servicos.Log.TratarErro($"Iniciando Monitoramento: {monitoramento.Codigo} placa {monitoramento.Veiculo?.Placa}", "Monitoramento");

                monitoramento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado;
                monitoramento.DataInicio = dataInicio < monitoramento.DataCriacao ? DateTime.Now : dataInicio;
                monitoramento.PercentualViagem = 0;

                if (monitoramento.StatusViagem == null) monitoramento.StatusViagem = MonitoramentoStatusViagem.IdentificaStatusViagemRegistraHistorico(unitOfWork, monitoramento);

                repMonitoramento.Atualizar(monitoramento);

                Carga.AtualizarCargaPeloMonitoramento(monitoramento, unitOfWork);
                ImportarInformacoesMonitoramentoAnterior(monitoramento, unitOfWork);
            }
        }

        private static void IniciarMonitoramentoRetorno(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramentoAnterior, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramentoAnterior.Veiculo != null && configuracao.PossuiMonitoramento && monitoramentoAnterior.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado && monitoramentoAnterior.DataInicio != null && monitoramentoAnterior.DataFim != null)
            {
                // Verifica se já não existe um monitoramento aberto para o veículo
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosEmAberto = repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculo(monitoramentoAnterior.Veiculo);
                if (monitoramentosEmAberto.Count == 0)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosRetornoAguardando = repMonitoramento.BuscarMonitoramentosRetornoVazioAguardandoInicioPorVeiculo(monitoramentoAnterior.Veiculo, monitoramentoAnterior.DataCriacao ?? monitoramentoAnterior.Carga.DataCriacaoCarga);
                    if (monitoramentosRetornoAguardando.Count > 0)
                    {
                        if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte && monitoramentosRetornoAguardando.First().Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                            return;

                        IniciarMonitoramentoAguardando(monitoramentosRetornoAguardando.First(), monitoramentoAnterior.DataFim ?? DateTime.Now, configuracao, auditoriaBase, unitOfWork);
                    }
                    else if (configuracao.GerarMonitoramentoParaCargaRetornoVazio)
                    {
                        CriarMonitoramentoRetornoAutomatico(monitoramentoAnterior, auditoriaBase, unitOfWork);
                    }
                }
            }
        }

        private static void IniciarProximoMonitoramentoAgendadoDoVeiculo(Dominio.Entidades.Veiculo veiculo, DateTime dataInicio, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (veiculo != null && configuracao.AcaoAoFinalizarMonitoramento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoAoFinalizarMonitoramento.Nenhuma)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosAgendados = null;
                if (configuracao.AcaoAoFinalizarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoAoFinalizarMonitoramento.IniciarProximoMonitoramentoAgendadoPorDataCriacao)
                {
                    monitoramentosAgendados = (configuracao.LimitarApenasUmMonitoramentoPorPlaca) ? repMonitoramento.BuscarMonitoramentosAgendadosPorPlacaVeiculo(veiculo.Placa) : repMonitoramento.BuscarMonitoramentosAgendadosPorVeiculo(veiculo.Codigo);
                }
                else if (configuracao.AcaoAoFinalizarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoAoFinalizarMonitoramento.IniciarProximoMonitoramentoAgendadoPorDataCarregamentoCarga)
                {
                    monitoramentosAgendados = (configuracao.LimitarApenasUmMonitoramentoPorPlaca) ? repMonitoramento.BuscarMonitoramentosAgendadosPorPlacaVeiculoDataCarregamentoCarga(veiculo.Placa) : repMonitoramento.BuscarMonitoramentosAgendadosPorVeiculoDataCarregamentoCarga(veiculo.Codigo);
                }

                Servicos.Log.TratarErro($"Validação IniciarProximoMonitoramentoAgendadoDoVeiculo", "Monitoramento");

                Servicos.Log.TratarErro($"Monitoramentos Agendados: {monitoramentosAgendados?.Count}", "Monitoramento");

                Servicos.Log.TratarErro($"Buscando Monitoramentos abertos placa: {veiculo.Placa}", "Monitoramento");

                bool existeMonitoramentoIniciadoPlaca = configuracao.LimitarApenasUmMonitoramentoPorPlaca && repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlaca(veiculo.Placa).Count > 0;

                if (monitoramentosAgendados?.Count > 0 && !existeMonitoramentoIniciadoPlaca)
                {

                    Servicos.Log.TratarErro($"Nenhum Monitoramento aberto encontrado placa: {veiculo.Placa}", "Monitoramento");

                    monitoramentosAgendados.First().Initialize();

                    if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte && monitoramentosAgendados.First().Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                        return;

                    Servicos.Log.TratarErro($"Iniciando Monitoramento: {monitoramentosAgendados.First().Codigo} placa {monitoramentosAgendados.First().Veiculo?.Placa}", "Monitoramento");

                    IniciarMonitoramentoAguardando(monitoramentosAgendados.First(), dataInicio, configuracao, auditoriaBase, unitOfWork);
                    Auditoria.AuditarMonitoramento(monitoramentosAgendados.First(), "Monitoramento iniciado pois era o primeiro agendado do veículo", auditoriaBase, unitOfWork);
                }
            }
        }

        private static void FinalizarUltimoMonitoramentoHistoricoStatusViagemDescarga(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataFim, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem ultimoHistoricoStatusViagem = repMonitoramentoHistoricoStatusViagem.BuscarUltimoHistoricoDoMonitoramento(monitoramento);
            if (ultimoHistoricoStatusViagem != null &&
                (ultimoHistoricoStatusViagem.StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga || ultimoHistoricoStatusViagem.StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga) &&
                !ultimoHistoricoStatusViagem.DataFim.HasValue)
            {
                ultimoHistoricoStatusViagem.DataFim = dataFim;
                ultimoHistoricoStatusViagem.TempoSegundos = (int)(ultimoHistoricoStatusViagem.DataFim.Value - ultimoHistoricoStatusViagem.DataInicio).TotalSeconds;
                repMonitoramentoHistoricoStatusViagem.Atualizar(ultimoHistoricoStatusViagem);
            }
        }

        private static void FinalizarMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataFim, Repositorio.UnitOfWork unitOfWork, MotivoFinalizacaoMonitoramento motivoFinalizacaoMonitoramento, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase)
        {
            EncerrarMonitoramento(monitoramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida, dataFim, unitOfWork, motivoFinalizacaoMonitoramento, auditoriaBase);
        }

        private static void CancelarMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataFim, Repositorio.UnitOfWork unitOfWork, MotivoFinalizacaoMonitoramento motivoFinalizacaoMonitoramento, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase)
        {
            EncerrarMonitoramento(monitoramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Cancelada, dataFim, unitOfWork, motivoFinalizacaoMonitoramento, auditoriaBase);
        }

        private static void EncerrarMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra statusViagemTipoRegra, DateTime dataFim, Repositorio.UnitOfWork unitOfWork, MotivoFinalizacaoMonitoramento motivoFinalizacaoMonitoramento, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
            Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();

            DateTime dataFimMonitoramento = dataFim;
            if ((configuracaoMonitoramento?.ConsiderarDataAuditoriaComoDataFimDoMonitoramento ?? false) || (dataFim < monitoramento.DataInicio))
                dataFimMonitoramento = DateTime.Now;

            monitoramento.Status = status;
            monitoramento.DataFim = dataFimMonitoramento;
            monitoramento.DataInicio = monitoramento.DataInicio ?? monitoramento.DataFim;
            monitoramento.PercentualViagem = 100;
            monitoramento.PolilinhaAteOrigem = string.Empty;
            monitoramento.DistanciaAteOrigem = 0;
            monitoramento.PolilinhaAteDestino = string.Empty;
            monitoramento.DistanciaAteDestino = 0;
            monitoramento.Processar = ProcessarPosicao.Processado;
            monitoramento.MotivoFinalizacao = motivoFinalizacaoMonitoramento;

            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem antigoStatusViagem = monitoramento.StatusViagem;
            monitoramento.StatusViagem = MonitoramentoStatusViagem.OterStatusViagemPorTipoRegra(unitOfWork, statusViagemTipoRegra);

            if (monitoramento.Veiculo != null)
            {
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                monitoramento.UltimaPosicao = repPosicao.BuscarUltimaPosicaoVeiculo(monitoramento.Veiculo.Codigo, monitoramento.DataInicio.Value, monitoramento.DataFim.Value);
            }

            FinalizarUltimoMonitoramentoHistoricoStatusViagemDescarga(monitoramento, (DateTime)monitoramento.DataFim, unitOfWork);

            FinalizaMonitoramentoVeiculo(unitOfWork, monitoramento, dataFim);

            ValidarPerdaSinalMonitoramento(unitOfWork, monitoramento);

            if (monitoramento.StatusViagem != null && (antigoStatusViagem == null || antigoStatusViagem.TipoRegra != monitoramento.StatusViagem.TipoRegra))
            {
                Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.RegistraHistorico(unitOfWork, monitoramento, monitoramento.StatusViagem, monitoramento.Veiculo, dataFim);
            }

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado)
            {
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem = MonitoramentoStatusViagem.OterStatusViagemPorTipoRegra(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Cancelada);
                if (statusViagem != null)
                    monitoramento.StatusViagem = statusViagem;
            }

            repMonitoramento.Atualizar(monitoramento);

            if (configuracaoTMS?.UtilizaAppTrizy ?? false)
                servMonitoramento.RegistrarPosicaoEventosRelevantesTrizy(monitoramento.Carga.Codigo, monitoramento.DataFim ?? DateTime.Now, monitoramento.UltimaPosicao?.Latitude, monitoramento.UltimaPosicao?.Longitude, EventoRelevanteMonitoramento.FinalizacaoMonitoramento, unitOfWork);

            Carga.AtualizarCargaPeloMonitoramento(monitoramento, unitOfWork);

            if (configuracaoMonitoramento?.GerarDadosSumarizadosDasParadasAoFinalizarOMonitoramento ?? false)
                new ParadasMonitoramentosFinalizados().GerarParadasMonitoramentosFinalizados(monitoramento, configuracaoTMS, unitOfWork);

            GerarDadosSumarizadosMonitoramento(monitoramento, null, unitOfWork);

            if (configuracaoMonitoramento?.FinalizarAutomaticamenteAlertasDoMonitoramentoAoFinalizarViagem ?? false)
                FinalizarAlertasPorMonitoramento(monitoramento, dataFimMonitoramento, "Finalizado automaticamente pelo fluxo de Finalizaçao de Viagem!", auditoriaBase, unitOfWork);
        }

        private static void FinalizarMonitoramentos(List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork)
        {
            int total = monitoramentos?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                FinalizarMonitoramento(monitoramentos[i], dataFim, unitOfWork, MotivoFinalizacaoMonitoramento.FinalizadoPorSubstituicao, auditoriaBase);

                if ((monitoramentos[i].Carga?.TipoOperacao?.ConfiguracaoControleEntrega?.FinalizarControleEntregaAoFinalizarMonitoramentoCarga ?? false) && monitoramentos[i].Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                {
                    FinalizarControleEntrega(monitoramentos[i], auditoriaBase, unitOfWork);
                }
                Auditoria.AuditarMonitoramento(monitoramentos[i], mensagemAuditoria, auditoriaBase, unitOfWork);
            }
        }

        private static void CancelarMonitoramentos(List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, string mensagemAuditoria, Repositorio.UnitOfWork unitOfWork)
        {
            int total = monitoramentos?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                CancelarMonitoramento(monitoramentos[i], dataFim, unitOfWork, MotivoFinalizacaoMonitoramento.FinalizadoAoCancelarMonitoramento, auditoriaBase);
                Auditoria.AuditarMonitoramento(monitoramentos[i], mensagemAuditoria, auditoriaBase, unitOfWork);
            }
        }

        private static void FinalizarControleEntrega(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();

            if (monitoramento.Carga.DataInicioViagem == DateTime.MinValue || monitoramento.Carga.DataInicioViagem == null)
            {

                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(monitoramento.Carga.Codigo, DateTime.Now, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, configuracao, TipoServicoMultisoftware.MultiEmbarcador, null, auditoriaBase, unitOfWork))
                {
                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();

                    if (auditoriaBase == null)
                    {
                        auditoria.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                        auditoria.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
                    }
                    else
                    {
                        auditoria.TipoAuditado = auditoriaBase.TipoAuditado;
                        auditoria.OrigemAuditado = auditoriaBase.OrigemAuditado;
                        auditoria.Usuario = auditoriaBase.Usuario;
                    }

                    Servicos.Auditoria.Auditoria.Auditar(auditoria, monitoramento.Carga, "Início de viagem informado automáticamente ao finalizar o monitoramento", unitOfWork);
                    monitoramento.Carga.ViagemIniciadaViaFinalizacaoMonitoramento = true;
                }

            }

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido>();

            OrigemSituacaoEntrega origemSituacaoEntrega = OrigemSituacaoEntrega.UsuarioMultiEmbarcador;

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(monitoramento.Carga.TipoOperacao?.Codigo ?? 0);
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in monitoramento.Carga.Entregas)
            {
                if (cargaEntrega.Situacao == SituacaoEntrega.Entregue || cargaEntrega.Situacao == SituacaoEntrega.EmFinalizacao)
                    continue;

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
                {
                    cargaEntrega = cargaEntrega,
                    dataInicioEntrega = cargaEntrega?.DataInicio ?? DateTime.Now,
                    dataTerminoEntrega = DateTime.Now,
                    dataConfirmacao = DateTime.Now,
                    dataSaidaRaio = null,
                    wayPoint = null,
                    wayPointDescarga = null,
                    pedidos = pedidos,
                    motivoRetificacao = 0,
                    justificativaEntregaForaRaio = "",
                    motivoFalhaGTA = 0,
                    configuracaoEmbarcador = configuracao,
                    tipoServicoMultisoftware = TipoServicoMultisoftware.MultiEmbarcador,
                    sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                    dadosRecebedor = null,
                    OrigemSituacaoEntrega = origemSituacaoEntrega,
                    ClienteAreaRedex = 0,
                    Container = 0,
                    DataColetaContainer = null,
                    auditado = auditoriaBase,
                    configuracaoControleEntrega = configuracaoControleEntrega,
                    tipoOperacaoParametro = tipoOperacaoParametro,
                    TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
                };

                if (!(cargaEntrega?.Cliente?.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente ?? false))
                    servicoControleEntrega.SetarCanhotosComoPendente(cargaEntrega, auditoriaBase, unitOfWork);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametros, unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();

                if (auditoriaBase == null)
                {
                    auditoria.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                    auditoria.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
                }
                else
                {
                    auditoria.TipoAuditado = auditoriaBase.TipoAuditado;
                    auditoria.OrigemAuditado = auditoriaBase.OrigemAuditado;
                    auditoria.Usuario = auditoriaBase.Usuario;
                }

                Servicos.Auditoria.Auditoria.Auditar(auditoria, cargaEntrega, $"Coleta/entrega finalizada porque o monitoramento foi finalizado", unitOfWork);

                cargaEntrega.EntregaFinalizadaViaFinalizacaoMonitoramento = true;

                if (monitoramento.Carga.DataFimViagem != null && monitoramento.Carga.DataFimViagem != DateTime.MinValue)
                    monitoramento.Carga.ViagemFinalizadaViaFinalizacaoMonitoramento = true;

            }

        }

        private static void ImportarInformacoesMonitoramentoAnterior(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Logistica.PermanenciaSubarea repPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
            Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
                if (configuracaoMonitoramento?.IdentificarCarregamentoAoIniciarOuFinalizarMonitoramentosConsecutivos ?? false)
                {
                    //Buscar último monitoramento finalizado.
                    Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo ultimoMonitoramentoVeiculo = repMonitoramentoVeiculo.BuscarUltimoPorVeiculoSimples(monitoramento.Veiculo.Codigo);
                    if (ultimoMonitoramentoVeiculo != null)
                    {
                        //Buscar última CargaEntrega (ENTREGA) da carga do monitoramento anterior.
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaCargaEntregaCargaAnterior = repCargaEntrega.BuscarUltimaCargaEntregaEntregaRealizada(ultimoMonitoramentoVeiculo.Carga);

                        //Buscar primeira CargaEntrega (COLETA) da carga do monitoramenot atual.
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraCargaEntregaCargaAtual = repCargaEntrega.BuscarPrimeiraCargaEntregaColetaPorCarga(monitoramento.Carga.Codigo);

                        //Se o Destino da Ultima Entrega é Origem da Primeira Coleta.
                        if (ultimaCargaEntregaCargaAnterior != null && primeiraCargaEntregaCargaAtual != null &&
                            ultimaCargaEntregaCargaAnterior.Cliente?.CPF_CNPJ == primeiraCargaEntregaCargaAtual.Cliente?.CPF_CNPJ)
                        {
                            DateTime dataEntrada = (DateTime)(ultimaCargaEntregaCargaAnterior.DataEntradaRaio ?? ultimaCargaEntregaCargaAnterior.DataInicio);
                            DateTime dataSaida = (DateTime)(ultimaCargaEntregaCargaAnterior.DataSaidaRaio ?? ultimaCargaEntregaCargaAnterior.DataFim);

                            //Replicar a entrada e a saída do alvo para a CargaEntrega (coleta) do monitoramento iniciado.
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarEntradaNoRaio(primeiraCargaEntregaCargaAtual, dataEntrada, unitOfWork);
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarSaidaDoRaio(primeiraCargaEntregaCargaAtual, dataSaida, unitOfWork);

                            //Criar movimentação de histórico de status do monitoramento com tipo de "Em Carregamento".
                            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem = MonitoramentoStatusViagem.OterStatusViagemPorTipoRegra(unitOfWork, MonitoramentoStatusViagemTipoRegra.EmCarregamento);
                            Dominio.Entidades.Embarcador.Logistica.Posicao ultimaPosicao = monitoramento.UltimaPosicao;

                            new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork).Inserir(new Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem()
                            {
                                Monitoramento = monitoramento,
                                StatusViagem = statusViagem,
                                Veiculo = monitoramento.Veiculo,
                                DataInicio = dataEntrada,
                                DataFim = dataSaida,
                                TempoSegundos = (int)(dataSaida - dataEntrada).TotalSeconds,
                                Latitude = ultimaCargaEntregaCargaAnterior.LatitudeFinalizada != null ? (double)ultimaCargaEntregaCargaAnterior.LatitudeFinalizada : (ultimaPosicao?.Latitude ?? 0d),
                                Longitude = ultimaCargaEntregaCargaAnterior.LongitudeFinalizada != null ? (double)ultimaCargaEntregaCargaAnterior.LongitudeFinalizada : (ultimaPosicao?.Longitude ?? 0d),
                            });

                            //Replicar permanencias:
                            //... Cliente.
                            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes = repPermanenciaCliente.BuscarPorClienteECargaEntrega(ultimaCargaEntregaCargaAnterior.Cliente?.CPF_CNPJ ?? 0, ultimaCargaEntregaCargaAnterior.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaCliente in permanenciasClientes)
                            {
                                repPermanenciaCliente.Inserir(new Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente()
                                {
                                    CargaEntrega = primeiraCargaEntregaCargaAtual,
                                    Cliente = primeiraCargaEntregaCargaAtual.Cliente,
                                    DataInicio = permanenciaCliente.DataInicio,
                                    DataFim = permanenciaCliente.DataFim,
                                    TempoSegundos = permanenciaCliente.TempoSegundos
                                });
                            }
                            //... Subareas.
                            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas = repPermanenciaSubarea.BuscarPorCargaEntrega(ultimaCargaEntregaCargaAnterior);
                            foreach (Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubarea in permanenciasSubareas)
                            {
                                repPermanenciaSubarea.Inserir(new Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea()
                                {
                                    CargaEntrega = primeiraCargaEntregaCargaAtual,
                                    Subarea = permanenciaSubarea.Subarea,
                                    DataInicio = permanenciaSubarea.DataInicio,
                                    DataFim = permanenciaSubarea.DataFim,
                                    TempoSegundos = permanenciaSubarea.TempoSegundos
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //O baile tem que continuar...
                Servicos.Log.TratarErro(ex);
            }
        }

        private static void NovaPosicaoAtualApartirDaPosicao(Dominio.Entidades.Embarcador.Logistica.PosicaoAtual novaPosicaoAtual, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, int codigoVeiculo)
        {
            novaPosicaoAtual.Data = posicao.Data;
            novaPosicaoAtual.DataVeiculo = posicao.DataVeiculo;
            novaPosicaoAtual.DataCadastro = posicao.DataCadastro;
            novaPosicaoAtual.Latitude = posicao.Latitude;
            novaPosicaoAtual.Longitude = posicao.Longitude;
            novaPosicaoAtual.Descricao = posicao.Descricao;
            novaPosicaoAtual.IDEquipamento = posicao.IDEquipamento;
            novaPosicaoAtual.Velocidade = posicao.Velocidade;
            novaPosicaoAtual.Temperatura = posicao.Temperatura;
            novaPosicaoAtual.NivelBateria = posicao.NivelBateria;
            novaPosicaoAtual.NivelSinalGPS = posicao.NivelSinalGPS;
            novaPosicaoAtual.Ignicao = posicao.Ignicao;
            novaPosicaoAtual.Veiculo = new Dominio.Entidades.Veiculo { Codigo = codigoVeiculo };
            novaPosicaoAtual.SensorTemperatura = posicao.SensorTemperatura;
            novaPosicaoAtual.EmAlvo = posicao.EmAlvo;
            novaPosicaoAtual.EmLocal = posicao.EmLocal;
            novaPosicaoAtual.Posicao = posicao;
        }

        private static void ValidarPosicaoAtualDemaisVeiculosDeMesmaPlaca(Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao status, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            List<int> veiculosMesmaPlaca = repVeiculo.BuscarCodigoVeiculoPorPlaca(posicaoAtual.Veiculo.Placa);

            foreach (int veiculo in veiculosMesmaPlaca)
            {
                if (veiculo != posicaoAtual.Veiculo.Codigo)
                {
                    Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtualVeiculoMesmaPlaca = repPosicaoAtual.BuscarPorVeiculo(veiculo);

                    if (posicaoAtualVeiculoMesmaPlaca == null)
                    {
                        posicaoAtualVeiculoMesmaPlaca = new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual();
                        NovaPosicaoAtualApartirDaPosicao(posicaoAtualVeiculoMesmaPlaca, posicao, veiculo);
                        posicaoAtualVeiculoMesmaPlaca.Veiculo = new Dominio.Entidades.Veiculo { Codigo = veiculo };
                        posicaoAtualVeiculoMesmaPlaca.Status = status;
                        repPosicaoAtual.Inserir(posicaoAtualVeiculoMesmaPlaca);
                    }
                    // ... considera a posição recebida como posição atual apenas se for mais nova que a posição atual já existente (Há casos de posições recebidas fora de ordem)
                    else if (posicao.DataVeiculo > posicaoAtualVeiculoMesmaPlaca.DataVeiculo)
                    {
                        NovaPosicaoAtualApartirDaPosicao(posicaoAtualVeiculoMesmaPlaca, posicao, veiculo);
                        posicaoAtualVeiculoMesmaPlaca.Status = status;
                        posicaoAtualVeiculoMesmaPlaca.Veiculo = new Dominio.Entidades.Veiculo { Codigo = veiculo };
                        repPosicaoAtual.Atualizar(posicaoAtualVeiculoMesmaPlaca);
                    }
                }
            }
        }

        public static void GerarDadosSumarizadosMonitoramento(int codigoMonitoramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraQualidadeMonitoramento? tipoRegra, Repositorio.UnitOfWork unitOfWork)
        {
            GerarDadosSumarizadosMonitoramento(new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork).BuscarPorCodigo(codigoMonitoramento), tipoRegra, unitOfWork);
        }

        public static void GerarDadosSumarizadosMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegraQualidadeMonitoramento? tipoRegra, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento repositorioRegraQualidadeMonitoramento = new Repositorio.Embarcador.TorreControle.RegraQualidadeMonitoramento(unitOfWork);
            List<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento> regras = null;
            if (!tipoRegra.HasValue)
                regras = repositorioRegraQualidadeMonitoramento.BuscarRegrasAtivas();
            else
                regras = repositorioRegraQualidadeMonitoramento.BuscarRegrasAtivasPorTipoRegra(tipoRegra.Value);

            if (regras == null || regras.Count == 0 || monitoramento.DataInicio == null || monitoramento.DataFim == null) return;

            Repositorio.Embarcador.Logistica.MonitoramentoDadosSumarizados repositorioMonitoramentoDadosSumarizados = new Repositorio.Embarcador.Logistica.MonitoramentoDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Monitoramento.MonitoramentoDadosSumarizados servicoMonitoramentoDadosSumarizados = new Servicos.Embarcador.Monitoramento.MonitoramentoDadosSumarizados(unitOfWork);
            servicoMonitoramentoDadosSumarizados.PrepararDadosMonitoramento(monitoramento);

            Dictionary<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento, Dominio.ObjetosDeValor.Embarcador.Monitoramento.MonitoramentoDadosSumarizados> dadosSumarizados = new Dictionary<Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento, Dominio.ObjetosDeValor.Embarcador.Monitoramento.MonitoramentoDadosSumarizados>();

            foreach (Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento regra in regras)
                dadosSumarizados[regra] = servicoMonitoramentoDadosSumarizados.SumarizarPorRegra(regra.TipoRegraQualidadeMonitoramento);

            monitoramento.Initialize();
            foreach (Dominio.Entidades.Embarcador.TorreControle.RegraQualidadeMonitoramento regra in regras)
            {
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados dadoSumarizado = repositorioMonitoramentoDadosSumarizados.BuscarPorMonitoramentoeRegra(monitoramento.Codigo, regra.Codigo) ?? new Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados();
                dadoSumarizado.Initialize();
                dadoSumarizado.Monitoramento = monitoramento;
                dadoSumarizado.RegraQualidadeMonitoramento = regra;
                dadoSumarizado.Resultado = dadosSumarizados[regra].Resultado;
                dadoSumarizado.QuantidadePosicoesSumarizadas = dadosSumarizados[regra].Posicoes?.Count ?? 0;
                dadoSumarizado.Posicoes = dadosSumarizados[regra].Posicoes?.Select(pos => new Dominio.Entidades.Embarcador.Logistica.Posicao() { Codigo = pos.ID }).ToList() ?? null;
                if (dadoSumarizado.Codigo == 0)
                    repositorioMonitoramentoDadosSumarizados.Inserir(dadoSumarizado);
                else
                    repositorioMonitoramentoDadosSumarizados.Atualizar(dadoSumarizado);
                monitoramento.SetExternalChanges(dadoSumarizado.GetCurrentChanges());
                unitOfWork.Flush();
            }

            if (!tipoRegra.HasValue)
                Auditoria.AuditarMonitoramento(monitoramento, $"Geração dos Dados Sumarizados por Regras de Qualidade de Monitoramento", unitOfWork);
            else
                Auditoria.AuditarMonitoramento(monitoramento, $"Reprocessada regra de qualidade de monitoramento {tipoRegra}", unitOfWork);
        }

        #endregion
    }
}
