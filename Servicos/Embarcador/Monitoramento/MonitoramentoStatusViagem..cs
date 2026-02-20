using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento
{

    public class MonitoramentoStatusViagem
    {

        #region Métodos públicos

        /**
         * Identifica automaticamente o status de viagem do monitoramento e insere no histórico
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem IdentificaStatusViagemRegistraHistorico(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Logistica.Posicao posicao = null, bool forcarTrocaStatus = false)
        {

            // Identifica o possivel novo status
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem novoStatusViagem = IdentificarStatusViagem(unitOfWork, monitoramento, posicao);

            // Se mudou, registra o histórico do status de viagem
            if (monitoramento.Veiculo != null && novoStatusViagem != null && (monitoramento.StatusViagem == null || novoStatusViagem.Codigo != monitoramento.StatusViagem.Codigo))
            {
                RegistraHistorico(unitOfWork, monitoramento, novoStatusViagem, posicao, forcarTrocaStatus);
            }

            return novoStatusViagem;
        }

        /**
         * Alteração explícita do status de viagem do monitoramento
         */
        public static void AlterarStatusViagem(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem novoStatusViagem, DateTime dataInicialNovoStatus, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega)
        {
            if (monitoramento != null && novoStatusViagem != null && (monitoramento.StatusViagem == null || monitoramento.StatusViagem.Codigo != novoStatusViagem.Codigo))
            {

                Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem ultimoHistoricoStatusViagem = repMonitoramentoHistoricoStatusViagem.BuscarUltimoHistoricoDoMonitoramento(monitoramento);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                if (ultimoHistoricoStatusViagem != null && ((dataInicialNovoStatus < ultimoHistoricoStatusViagem.DataInicio) || (ultimoHistoricoStatusViagem.DataFim != null && dataInicialNovoStatus < ultimoHistoricoStatusViagem.DataFim)))
                    throw new Exception("Não pode ser definido um início do novo status anterior ao status anterior, \"" + ultimoHistoricoStatusViagem.StatusViagem.Descricao + "\".");

                if (!configuracaoMonitoramento?.IgnorarStatusViagemMonitoramentoAnterioresTransito ?? false)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicoStatusViagem = repMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramento(monitoramento);
                    switch (novoStatusViagem.TipoRegra)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga:
                            if (!EsteveEmTransito(historicoStatusViagem, dataInicialNovoStatus))
                                throw new Exception("Não pode alterar para \"" + novoStatusViagem.Descricao + "\" pois não esteve em trânsito.");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito:
                            if (!EsteveEmCarregamento(historicoStatusViagem, dataInicialNovoStatus))
                                throw new Exception("Não pode alterar para \"" + novoStatusViagem.Descricao + "\" pois não passou pelo carregamento.");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida:
                            if (!EsteveEmDescarga(historicoStatusViagem, dataInicialNovoStatus))
                                throw new Exception("Não pode alterar para \"" + novoStatusViagem.Descricao + "\" pois não passou pela descarga.");
                            break;
                    }
                }

                // Altera o status do monitoramento
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Servicos.Embarcador.Pedido.ColetaContainer servColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);

                monitoramento.Initialize();
                monitoramento.StatusViagem = novoStatusViagem;
                repMonitoramento.Atualizar(monitoramento);

                // Registra o histórido da alteração do status do monitoramento
                RegistraHistorico(unitOfWork, monitoramento, novoStatusViagem, monitoramento.Veiculo, dataInicialNovoStatus);
                Carga.AtualizarCargaPeloMonitoramento(monitoramento, unitOfWork);

                string mensagemAuditoria = "Alterou o status de viagem do monitoramento manualmente";
                //bool gerarAuditoria = true;
                if (monitoramento.Carga != null)
                {

                    // Quando houver alteração para algum dos status relacionados a origem registra o início da entrega e entrada no raio do controle de entrega
                    if (StatusEstaRelacionadoAOrigem(novoStatusViagem.TipoRegra))
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarColetaNaOrigemPorCarga(monitoramento.Carga.Codigo);
                        Entrega.IniciarEntrega(cargaEntrega, monitoramento, dataInicialNovoStatus, 0, 0, configuracao, tipoServicoMultisoftware, clienteMultisoftware, configuracaoControleEntrega, unitOfWork);

                        Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repColetaContainer.BuscarPorCargaAtual(monitoramento.Carga.Codigo);

                        if (cargaEntrega != null && (cargaEntrega.DataInicio == null || dataInicialNovoStatus < cargaEntrega.DataInicio))
                        {
                            cargaEntrega.DataInicio = dataInicialNovoStatus;
                            cargaEntrega.DataEntradaRaio = dataInicialNovoStatus;
                            repCargaEntrega.Atualizar(cargaEntrega);
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
                        }

                        if (coletaContainer != null && coletaContainer.Container != null)
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer> statusAnterioresAoEmCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer>()
                                            {
                                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.AgColeta,
                                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.EmAreaEsperaVazio,
                                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.EmDeslocamentoCarregamento,
                                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.EmDeslocamentoCarregado
                                            };

                            if (statusAnterioresAoEmCarregamento.Contains(coletaContainer.Status))
                            {

                                Dominio.Entidades.Cliente clienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramento.Carga.Codigo);

                                Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
                                parametrosColetaContainer.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.EmCarregamento;
                                parametrosColetaContainer.DataAtualizacao = DateTime.Now;
                                parametrosColetaContainer.coletaContainer = coletaContainer;
                                parametrosColetaContainer.Usuario = auditado?.Usuario;
                                parametrosColetaContainer.LocalAtual = clienteOrigem;

                                parametrosColetaContainer.OrigemMonimentacaoContainer = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMovimentacaoContainer.UsuarioInterno;
                                parametrosColetaContainer.InformacaoOrigemMonimentacaoContainer = Dominio.ObjetosDeValor.Embarcador.Enumeradores.InformacaoOrigemMovimentacaoContainer.AlterarStatusMonitoramento;

                                servColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);
                            }
                        }

                    }
                    else if (novoStatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida)
                    {
                        //gerarAuditoria = false;
                        Carga.FinalizarViagem(monitoramento, dataInicialNovoStatus, auditado, mensagemAuditoria, tipoServicoMultisoftware, clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.MonitoramentoAutomaticamente, unitOfWork);

                        string msg = "Status de viagem concluída";
                        Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(monitoramento, dataInicialNovoStatus, configuracao, auditado, msg, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoFinalizacaoMonitoramento.StatusViagemConcluida);
                    }
                    else if (novoStatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito)
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarColetaNaOrigemPorCarga(monitoramento.Carga.Codigo);
                        if (cargaEntrega != null && cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente)
                        {
                            Entrega.Finalizar(cargaEntrega, monitoramento, dataInicialNovoStatus, 0, 0, 0, configuracao, tipoServicoMultisoftware, unitOfWork, configuracaoControleEntrega);
                        }

                        if (configuracao.QuandoIniciarViagemViaMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarViagemViaMonitoramento.NoStatusViagemTransito)
                        {
                            Carga.IniciarViagem(monitoramento, dataInicialNovoStatus, 0, 0, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
                        }
                    }
                }

                //if (gerarAuditoria)
                //{
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = monitoramento.GetChanges();
                alteracoes.Add(new Dominio.Entidades.Auditoria.HistoricoPropriedade("DataNovoStatus", null, dataInicialNovoStatus.ToString()));
                Servicos.Auditoria.Auditoria.Auditar(auditado, monitoramento, alteracoes, mensagemAuditoria, unitOfWork);
                //}

            }
        }

        public static void ExcluirHistoricoStatusViagem(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem historicoStatusViagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (monitoramento == null)
                throw new Exception("Monitoramento não encontrado.");

            if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado)
                throw new Exception("Monitoramento já finalizado.");

            if (StatusEhFinalizado(monitoramento.StatusViagem))
                throw new Exception("Status de viagem do monitoramento já concluído.");

            if (historicoStatusViagem == null || StatusEhFinalizado(historicoStatusViagem.StatusViagem))
                throw new Exception("Histórico de status de viagem não encontrado ou exclusão não permitida.");

            if (monitoramento.StatusViagem != null && monitoramento.StatusViagem.Codigo != historicoStatusViagem.StatusViagem.Codigo)
                throw new Exception("É possível excluir apenas o último status de viagem do monitoramento.");

            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);

            // Status anterior
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem historicoStatusViagemAnterior = repMonitoramentoHistoricoStatusViagem.BuscarHistoricoAnteriorDoMonitoramento(historicoStatusViagem, monitoramento);

            // Exclui o histórico
            repMonitoramentoHistoricoStatusViagem.Deletar(historicoStatusViagem);

            // Remove o fim do status anterior para se tornar o status atual
            List<Dominio.Entidades.Auditoria.HistoricoPropriedade> changesHistoricoStatusViagemAnterior = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();
            if (historicoStatusViagemAnterior != null)
            {
                historicoStatusViagemAnterior.Initialize();
                historicoStatusViagemAnterior.DataFim = null;
                historicoStatusViagemAnterior.TempoSegundos = null;
                repMonitoramentoHistoricoStatusViagem.Atualizar(historicoStatusViagemAnterior);
                changesHistoricoStatusViagemAnterior = historicoStatusViagemAnterior.GetChanges();
                foreach (var change in changesHistoricoStatusViagemAnterior)
                {
                    change.Propriedade = "StatusHistoricoAnterior_" + change.Propriedade;
                }

                if (StatusEstaRelacionadoAOrigem(historicoStatusViagemAnterior.StatusViagem.TipoRegra))
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega = repCargaEntrega.BuscarColetaNaOrigemPorCarga(monitoramento.Carga.Codigo);
                    if (entrega != null && (entrega.DataInicio == null || historicoStatusViagemAnterior.DataInicio < entrega.DataInicio))
                    {
                        entrega.DataInicio = historicoStatusViagemAnterior.DataInicio;
                        entrega.DataEntradaRaio = historicoStatusViagemAnterior.DataInicio;
                        repCargaEntrega.Atualizar(entrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(entrega, repCargaEntrega, unitOfWork);
                    }
                }

            }

            // Atualiza o status do monitoramento para o status anterior
            monitoramento.Initialize();
            monitoramento.StatusViagem = (historicoStatusViagemAnterior != null) ? historicoStatusViagemAnterior.StatusViagem : null;
            repMonitoramento.Atualizar(monitoramento);

            Servicos.Auditoria.Auditoria.Auditar(auditado, monitoramento, monitoramento.GetChanges().Concat(changesHistoricoStatusViagemAnterior).ToList(), "Excluiu o histórico o status de viagem \"" + historicoStatusViagem.StatusViagem.Descricao + "\".", unitOfWork);

        }

        public static void AlterarHistoricoStatusViagem(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem historicoStatusViagem, DateTime dataInicial, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (monitoramento == null)
                throw new Exception("Monitoramento não encontrado.");

            if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado)
                throw new Exception("Monitoramento já foi finalizado.");

            if (StatusEhFinalizado(monitoramento.StatusViagem))
                throw new Exception("Status de viagem do monitoramento já finalizado.");

            if (monitoramento.StatusViagem != null && monitoramento.StatusViagem.Codigo != historicoStatusViagem.StatusViagem.Codigo)
                throw new Exception("É possível alterar apenas o último status de viagem do monitoramento.");

            if (dataInicial > DateTime.Now)
                throw new Exception("Não é possível definir uma data maior que a data atual.");

            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            historicoStatusViagem.Initialize();
            DateTime dataInicioAntiga = historicoStatusViagem.DataInicio;
            historicoStatusViagem.DataInicio = dataInicial;
            repMonitoramentoHistoricoStatusViagem.Atualizar(historicoStatusViagem);

            // Status anterior para ajustar a data final
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem historicoStatusViagemAnterior = repMonitoramentoHistoricoStatusViagem.BuscarHistoricoAnteriorDoMonitoramento(historicoStatusViagem, monitoramento);
            if (historicoStatusViagemAnterior != null)
            {
                if (dataInicial < historicoStatusViagemAnterior.DataInicio)
                {
                    throw new Exception("Não é possível alterar a data para uma data anterior ao início do status anterior \"" + historicoStatusViagemAnterior.StatusViagem.Descricao + "\".");
                }
                historicoStatusViagemAnterior.DataFim = dataInicial;
                historicoStatusViagemAnterior.TempoSegundos = (int)(historicoStatusViagemAnterior.DataFim.Value - historicoStatusViagemAnterior.DataInicio).TotalSeconds;
                repMonitoramentoHistoricoStatusViagem.Atualizar(historicoStatusViagemAnterior);
            }

            // Altera a data de entrada do controle de entrega
            if (StatusEstaRelacionadoAOrigem(historicoStatusViagem.StatusViagem.TipoRegra))
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega = repCargaEntrega.BuscarColetaNaOrigemPorCarga(monitoramento.Carga.Codigo);
                if (entrega != null && (entrega.DataInicio == null || dataInicial < entrega.DataInicio))
                {
                    entrega.DataInicio = dataInicial;
                    entrega.DataEntradaRaio = dataInicial;
                    repCargaEntrega.Atualizar(entrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(entrega, repCargaEntrega, unitOfWork);
                }
            }

            Servicos.Auditoria.Auditoria.Auditar(auditado, monitoramento, historicoStatusViagem.GetChanges(), "Alterado o histórico do status de viagem \"" + historicoStatusViagem.StatusViagem.Descricao + "\".", unitOfWork);

        }

        /**
         * Insere um novo registro no histórico dos status de viagem do monitoramento
         */
        public static void RegistraHistorico(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem, Dominio.Entidades.Embarcador.Logistica.Posicao posicao = null, bool forcarTrocaStatus = false)
        {
            if (monitoramento.Veiculo != null && statusViagem != null)
            {

                // Localiza a última posição do veículo
                if (posicao == null)
                {
                    Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                    posicao = repPosicao.BuscarUltimaPosicaoDataVeiculo(monitoramento.Veiculo.Codigo);
                }
                if (posicao != null)
                {
                    RegistraHistorico(unitOfWork, monitoramento, statusViagem, posicao.Veiculo, posicao.DataVeiculo, posicao.Latitude, posicao.Longitude);
                }
                else if (forcarTrocaStatus)
                {
                    RegistraHistorico(unitOfWork, monitoramento, statusViagem, monitoramento.Veiculo, DateTime.Now, null, null);
                }
            }
        }

        /**
         * Insere um novo registro no histórico dos status de viagem do monitoramento
         */
        public static void RegistraHistorico(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem, Dominio.Entidades.Veiculo veiculo, DateTime dataInicio, double? latitude = null, double? longitude = null)
        {
            if (veiculo != null && statusViagem != null)
            {
                int? tempoSegundos;
                DateTime? dataFinal;

                Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);

                // Finaliza o status aberto
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem monitoramentoHistoricoStatusViagemAberto = repMonitoramentoHistoricoStatusViagem.BuscarAbertoPorMonitoramento(monitoramento);
                if (monitoramentoHistoricoStatusViagemAberto != null)
                {
                    dataFinal = dataInicio;
                    if (dataFinal > monitoramentoHistoricoStatusViagemAberto.DataInicio)
                    {
                        monitoramentoHistoricoStatusViagemAberto.DataFim = dataFinal;
                    }
                    else
                    {
                        monitoramentoHistoricoStatusViagemAberto.DataFim = monitoramentoHistoricoStatusViagemAberto.DataInicio;
                    }
                    tempoSegundos = (int)(monitoramentoHistoricoStatusViagemAberto.DataFim.Value - monitoramentoHistoricoStatusViagemAberto.DataInicio).TotalSeconds;
                    monitoramentoHistoricoStatusViagemAberto.TempoSegundos = tempoSegundos;
                    repMonitoramentoHistoricoStatusViagem.Atualizar(monitoramentoHistoricoStatusViagemAberto);
                }

                if (statusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida || statusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Cancelada)
                {
                    dataFinal = dataInicio;
                    tempoSegundos = 0;
                }
                else
                {
                    dataFinal = null;
                    tempoSegundos = null;
                }

                // Insere o novo status no histórico
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem historioStatusViagem = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem()
                {
                    Monitoramento = monitoramento,
                    StatusViagem = statusViagem,
                    Veiculo = veiculo,
                    DataInicio = dataInicio,
                    DataFim = dataFinal,
                    TempoSegundos = tempoSegundos,
                    Latitude = latitude,
                    Longitude = longitude
                };

                // Registra o historico dos status de viagem do monitoramento
                repMonitoramentoHistoricoStatusViagem.Inserir(historioStatusViagem);

            }
        }

        /**
         * Identifica automaticamente o status de viagem do monitoramento
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem IdentificarStatusViagem(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Logistica.Posicao posicao = null)
        {
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens = ObterStatusAtivos(unitOfWork);
            return IdentificarStatusViagem(monitoramentoStatusViagens, monitoramento, posicao: posicao == null ? new Dominio.Entidades.Embarcador.Logistica.Posicao { Codigo = 0 } : posicao,
                pontosDePassagem: new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>(), historicosStatusViagem: new List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>());
        }

        /**
         * Identifica qual o status de viagem do monitoramento a partir do monitoramento e de todas as iformações necessárias
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem IdentificarStatusViagem(
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens,
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao = null,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos = null,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas = null,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes = null,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas = null,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem = null,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes = null,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = null,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem = null,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo = null,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = null,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = null,
            Dominio.Entidades.Cliente clienteOrigem = null,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte = null,
            string codigoIntegracaoViaTransporte = null,
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = null,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento = null,
            bool regrasTransito = true
        )
        {

            //vamos filtrar apenas os statusViagem sequenciais para o monitoramento
            //monitoramentoStatusViagens = FiltrarStatusViagemPossiveisMonitoramento(monitoramento, monitoramentoStatusViagens);

            // Percorre cada um dos status de viagem cadastrados verificando a respectiva regra
            int total = monitoramentoStatusViagens?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (
                     VerificarStatusViagemTipoRegra(
                         monitoramentoStatusViagens[i].TipoRegra,
                         monitoramentoStatusViagens[i],
                         monitoramentoStatusViagens,
                         monitoramento,
                         posicao,
                         posicaoAlvos,
                         posicaoAlvoSubareas,
                         permanenciasClientes,
                         permanenciasSubareas,
                         pontosDePassagem,
                         subareasClientes,
                         cargaEntregas,
                         historicosStatusViagem,
                         posicoesVeiculo,
                         configuracao,
                         configuracaoMonitoramento,
                         clienteOrigem,
                         viaTransporte,
                         codigoIntegracaoViaTransporte,
                         cargaJanelaCarregamento,
                         cargaJanelasDescarregamento,
                         regrasTransito
                     )
                 )
                {
                    return monitoramentoStatusViagens[i];
                }

            }

            // Não encontrou nenhum status ...
            return null;
        }

        /**
         * Validar se a sequencia do proximo status esta de acordo. (por enquanto apenas aplicado para transito)
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem ValidarSequenciaStatusViagem(
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens,
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicoStatusViagens,
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao = null
           )
        {
            if (statusViagem != null)
            {
                switch (statusViagem.TipoRegra)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito:
                        if (!EsteveEmCarregamento(historicoStatusViagens, posicao.DataVeiculo))
                        {
                            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem EmCarregamento = monitoramentoStatusViagens.Where(x => x.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmCarregamento).FirstOrDefault();
                            if (EmCarregamento != null)
                                return EmCarregamento;
                            else
                                return statusViagem;
                        }
                        else
                            return statusViagem;
                    default:
                        break;
                }
            }

            return statusViagem;
        }


        /**
         * Identifica se o tipo regra representa status de viagem do monitoramento
         */
        public static bool VerificarStatusViagemTipoRegra(
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra,
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens,
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporte,
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento,
            bool regrastransito = true
        )
        {
            switch (tipoRegra)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.SemViagem:
                    return VerificarStatusViagemTipoRegra_SemViagem(monitoramento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmViagem:
                    return VerificarStatusViagemTipoRegra_EmViagem(monitoramento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando:
                    return VerificarStatusViagemTipoRegra_Retornando(monitoramento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida:
                    return VerificarStatusViagemTipoRegra_Concluida(monitoramento, posicao, subareasClientes, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, historicosStatusViagem, configuracao, configuracaoMonitoramento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Cancelada:
                    return VerificarStatusViagemTipoRegra_Cancelada(monitoramento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta:
                    return VerificarStatusViagemTipoRegra_DeslocamentoParaPlanta(monitoramento, monitoramentoStatusViagens, posicao, posicaoAlvos, permanenciasClientes, pontosDePassagem, cargaEntregas, historicosStatusViagem, posicoesVeiculo, configuracao, configuracaoMonitoramento, clienteOrigem, viaTransporte, codigoIntegracaoViaTransporte);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento:
                    return VerificarStatusViagemTipoRegra_AguardandoHorarioCarregamento(monitoramento, posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasClientes, permanenciasSubareas, pontosDePassagem, subareasClientes, cargaEntregas, historicosStatusViagem, clienteOrigem, cargaJanelaCarregamento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento:
                    return VerificarStatusViagemTipoRegra_AguardandoCarregamento(monitoramento, posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasClientes, permanenciasSubareas, pontosDePassagem, subareasClientes, cargaEntregas, historicosStatusViagem, clienteOrigem, cargaJanelaCarregamento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmCarregamento:
                    return VerificarStatusViagemTipoRegra_EmCarregamento(monitoramento, posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasClientes, permanenciasSubareas, pontosDePassagem, subareasClientes, historicosStatusViagem, clienteOrigem);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmLiberacao:
                    return VerificarStatusViagemTipoRegra_EmLiberacao(monitoramento, posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasClientes, permanenciasSubareas, pontosDePassagem, subareasClientes, historicosStatusViagem, clienteOrigem);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito:
                    return VerificarStatusViagemTipoRegra_Transito(monitoramento, posicao, posicaoAlvos, permanenciasClientes, pontosDePassagem, cargaEntregas, historicosStatusViagem, posicoesVeiculo, configuracao, configuracaoMonitoramento, clienteOrigem, viaTransporte, codigoIntegracaoViaTransporte, regrastransito, monitoramentoStatusViagem);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga:
                    return VerificarStatusViagemTipoRegra_AguardandoHorarioDescarga(monitoramento, posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasClientes, permanenciasSubareas, pontosDePassagem, subareasClientes, cargaEntregas, historicosStatusViagem, clienteOrigem, cargaJanelasDescarregamento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga:
                    return VerificarStatusViagemTipoRegra_AguardandoDescarga(monitoramento, posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasClientes, permanenciasSubareas, pontosDePassagem, subareasClientes, cargaEntregas, historicosStatusViagem, clienteOrigem, cargaJanelasDescarregamento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga:
                    return VerificarStatusViagemTipoRegra_Descarga(monitoramento, posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasClientes, permanenciasSubareas, pontosDePassagem, cargaEntregas, historicosStatusViagem, subareasClientes, clienteOrigem, viaTransporte, codigoIntegracaoViaTransporte, cargaJanelasDescarregamento);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DescargaFinalizada:
                    return VerificarStatusViagemTipoRegra_DescargaFinalizada(monitoramento, posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasClientes, permanenciasSubareas, pontosDePassagem, subareasClientes, historicosStatusViagem, clienteOrigem, viaTransporte, codigoIntegracaoViaTransporte);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento:
                    return VerificarStatusViagemTipoRegra_DeslocamentoParaColetarEquipamento(monitoramento, posicao, posicaoAlvos, permanenciasClientes, pontosDePassagem, cargaEntregas, historicosStatusViagem, posicoesVeiculo, configuracao, clienteOrigem, viaTransporte, codigoIntegracaoViaTransporte);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta:
                    return VerificarStatusViagemTipoRegra_DeslocamentoComEquipamentoParaPlanta(monitoramento, posicao, posicaoAlvos, permanenciasClientes, pontosDePassagem, cargaEntregas, historicosStatusViagem, posicoesVeiculo, configuracao, clienteOrigem, viaTransporte, codigoIntegracaoViaTransporte);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoECargaParaEntrega:
                    return VerificarStatusViagemTipoRegra_DeslocamentoComEquipamentoECargaParaEntrega(monitoramento, posicao, posicaoAlvos, permanenciasClientes, pontosDePassagem, cargaEntregas, historicosStatusViagem, posicoesVeiculo, configuracao, clienteOrigem, viaTransporte, codigoIntegracaoViaTransporte);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmColeta:
                    return VerificarStatusViagemTipoRegra_EmColeta(monitoramento, posicao, posicaoAlvos, permanenciasClientes, pontosDePassagem, cargaEntregas, historicosStatusViagem, subareasClientes, clienteOrigem, viaTransporte, codigoIntegracaoViaTransporte);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmParqueamento:
                    return VerificarStatusViagemTipoRegra_EmParqueamento(monitoramento, posicao, posicaoAlvos, permanenciasClientes, pontosDePassagem, cargaEntregas, historicosStatusViagem, subareasClientes);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmFronteira:
                    return VerificarStatusViagemTipoRegra_EmFronteira(monitoramento, posicao, posicaoAlvos, permanenciasClientes, pontosDePassagem, cargaEntregas, historicosStatusViagem, subareasClientes, clienteOrigem);
                default:
                    break;
            }

            // Regra não implementada
            return false;
        }

        public static DateTime IdentificarDataInicialStatusViagem(
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> listMonitoramentosStatusViagem
        )
        {
            DateTime dataInicial = posicao.DataVeiculo;
            Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaCliente;
            Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubarea;
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem ultimoMonitoramentoStatusViagem = null;
            if (listMonitoramentosStatusViagem != null && listMonitoramentosStatusViagem.Count > 0)
                ultimoMonitoramentoStatusViagem = listMonitoramentosStatusViagem[listMonitoramentosStatusViagem.Count - 1];

            if (statusViagem != null)
            {
                if ((posicao.EmAlvo ?? false) == true && posicaoAlvos != null && posicaoAlvos.Count > 0 && permanenciasClientes != null && permanenciasClientes.Count > 0)
                {
                    switch (statusViagem.TipoRegra)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmColeta:
                            permanenciaCliente = BuscarPermancenciaClienteEmAberto(permanenciasClientes);
                            if (permanenciaCliente != null)
                            {
                                dataInicial = permanenciaCliente.DataInicio;
                            }
                            break;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmCarregamento:
                            permanenciaSubarea = BuscarPermancenciaSubareaEmAberto(permanenciasSubareas);
                            if (permanenciaSubarea != null)
                            {
                                dataInicial = permanenciaSubarea.DataInicio;
                            }
                            else
                            {
                                permanenciaCliente = BuscarPermancenciaClienteEmAberto(permanenciasClientes);
                                if (permanenciaCliente != null)
                                {
                                    dataInicial = permanenciaCliente.DataInicio;
                                }
                            }
                            break;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga:
                            permanenciaSubarea = BuscarPermancenciaSubareaEmAberto(permanenciasSubareas);
                            if (permanenciaSubarea != null)
                            {
                                dataInicial = permanenciaSubarea.DataInicio;
                            }
                            else
                            {
                                permanenciaCliente = BuscarPermancenciaClienteEmAberto(permanenciasClientes);
                                if (permanenciaCliente != null)
                                {
                                    dataInicial = permanenciaCliente.DataInicio;
                                }
                            }
                            break;
                    }
                }

                switch (statusViagem.TipoRegra)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito:
                        permanenciaCliente = BuscarUltimaPermanenciaClienteEncerrada(permanenciasClientes);
                        if (permanenciaCliente != null)
                        {
                            dataInicial = permanenciaCliente.DataFim.Value;
                        }
                        break;
                }
            }

            if (ultimoMonitoramentoStatusViagem != null && (ultimoMonitoramentoStatusViagem.DataFim == null || dataInicial >= ultimoMonitoramentoStatusViagem.DataFim))
                return dataInicial;

            return DateTime.Now;
        }

        /**
         * Busca o status de viagem específico da uma regra do retorno de viagem
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem OterStatusViagemPorTipoRegra(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem = repMonitoramentoStatusViagem.BuscarAtivoPorTipoRegra(tipoRegra);
            return monitoramentoStatusViagem;
        }
        public static async Task<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> ObterStatusViagemPorTipoRegraAsync(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem =
                await repMonitoramentoStatusViagem.BuscarAtivoPorTipoRegraAsync(tipoRegra);
            return monitoramentoStatusViagem;
        }

        /**
         * Busca o status de viagem específico da regra "Sem viagem"
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem OterStatusViagemSemViagem(Repositorio.UnitOfWork unitOfWork)
        {
            return OterStatusViagemPorTipoRegra(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.SemViagem);
        }
        public static async Task<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> ObterStatusViagemSemViagemAsync(Repositorio.UnitOfWork unitOfWork)
        {
            return await ObterStatusViagemPorTipoRegraAsync(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.SemViagem);
        }

        /**
         * Busca o status de viagem específico da regra "Em viagem"
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem OterStatusViagemEmViagem(Repositorio.UnitOfWork unitOfWork)
        {
            return OterStatusViagemPorTipoRegra(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmViagem);
        }
        public static async Task<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> ObterStatusViagemEmViagemAsync(Repositorio.UnitOfWork unitOfWork)
        {
            return await ObterStatusViagemPorTipoRegraAsync(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmViagem);
        }

        /**
         * Busca o status de viagem específico da regra "Retornando"
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem OterStatusViagemRetornando(Repositorio.UnitOfWork unitOfWork)
        {
            return OterStatusViagemPorTipoRegra(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando);
        }

        /**
         * Busca o status de viagem específico da regra "Concluída"
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem OterStatusViagemConcluida(Repositorio.UnitOfWork unitOfWork)
        {
            return OterStatusViagemPorTipoRegra(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida);
        }

        /**
         * Busca o status de viagem específico da regra "Cancelada"
         */
        public static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem OterStatusViagemCancelada(Repositorio.UnitOfWork unitOfWork)
        {
            return OterStatusViagemPorTipoRegra(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Cancelada);
        }

        /**
         * Verifica se, entre os status da lista, algum deles necessitam dos pontos de passagem
         */
        public static bool VerificaStatusNecessitamDePontosDePassagem(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmCarregamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmLiberacao ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se, entre os status da lista, algum deles necessitam do horário de carregamento para ser identificado
         */
        public static bool VerificaStatusNecessitamDeHorariosCarregamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se, entre os status da lista, algum deles necessitam do histórico dos status
         */
        public static bool VerificaStatusNecessitamDeHistoricoStatusViagem(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmLiberacao ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga
                )
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se, entre os status da lista, algum deles necessitam do horário de descarregamento para ser identificado
         */
        public static bool VerificaStatusNecessitamDeHorariosDescarregamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se, entre os status da lista, algum deles necessitam das entregas da carga
         */
        public static bool VerificaStatusNecessitamDeCargaEntrega(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoECargaParaEntrega
                )
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se, entre os status da lista, algum deles necessitam das via de transporte
         */
        public static bool VerificaStatusNecessitamDeViaTransporte(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens.Count;
            for (int i = 0; i < total; i++)
            {
                if (
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoECargaParaEntrega
                )
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se, entre os status da lista, algum deles necessitam da janela de carregamento
         */
        public static bool VerificaStatusNecessitamDeJanelaCarregamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens.Count;
            for (int i = 0; i < total; i++)
            {
                if (
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento
                )
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se, entre os status da lista, algum deles necessitam da janela de descarregamento
         */
        public static bool VerificaStatusNecessitamDeJanelaDescarregamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens.Count;
            for (int i = 0; i < total; i++)
            {
                if (
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga
                )
                {
                    return true;
                }
            }
            return false;
        }

        public static List<int> BuscarTiposSubareasRelacionadosAoStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra)
        {
            List<int> tipos = new List<int>();
            switch (tipoRegra)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmLiberacao:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DescargaFinalizada:
                    tipos.Add((int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Balanca);
                    tipos.Add((int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Estacionamento);
                    tipos.Add((int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Patio);
                    tipos.Add((int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Portaria);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmCarregamento:
                    tipos.Add((int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Carregamento);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga:
                    tipos.Add((int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Descarregamento);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Todos:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.SemViagem:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmViagem:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoECargaParaEntrega:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmColeta:
                default:
                    break;
            }
            return tipos;
        }

        public static bool StatusEstaRelacionadoAOrigem(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra)
        {
            switch (tipoRegra)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmCarregamento:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmLiberacao:
                    return true;
            }
            return false;
        }

        public static bool StatusEstaRelacionadoAoDestino(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra)
        {
            switch (tipoRegra)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DescargaFinalizada:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga:
                    return true;
            }
            return false;
        }

        public static bool StatusEhFinalizado(Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem)
        {
            if (statusViagem != null)
            {
                switch (statusViagem.TipoRegra)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Cancelada:
                        return true;
                }
            }
            return false;
        }

        public static bool StatusRelacionadoAPlanta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra)
        {
            switch (tipoRegra)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento:
                    return true;
            }
            return false;
        }

        #endregion

        #region Métodos privados


        //private static List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> FiltrarStatusViagemPossiveisMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        //{
        //    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> listaStatusMonitoramentoSequencia = new List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem>();

        //    switch (monitoramento.StatusViagem.TipoRegra)
        //    {
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.SemViagem:
        //            return monitoramentoStatusViagens;

        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmViagem:


        //            return listaStatusMonitoramentoSequencia;

        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Cancelada:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmCarregamento:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmLiberacao:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DescargaFinalizada:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento:

        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoECargaParaEntrega:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmColeta:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmParqueamento:


        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmFronteira:

        //        default:
        //            break;

        //    }

        //    // retorna lista de status possiveis
        //    return listaStatusMonitoramentoSequencia;
        //}

        /**
         * Verifica se o monitoramento possui o status "Sem viagem"
         * - O monitoramento não deve ter sido iniciado
         */
        private static bool VerificarStatusViagemTipoRegra_SemViagem(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            return monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando;
        }

        /**
         * Verifica se o monitoramento possui o status "Em viagem"
         * - O monitoramento deve ter sido iniciado
         */
        private static bool VerificarStatusViagemTipoRegra_EmViagem(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            return monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado
                && (monitoramento.StatusViagem == null || monitoramento.StatusViagem.TipoRegra != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando);
        }

        /**
         * Verifica se o monitoramento possui o status "Retornando"
         * - O monitoramento está com o status de viagem "Retornando"
         */
        private static bool VerificarStatusViagemTipoRegra_Retornando(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            return monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && monitoramento.StatusViagem != null
                && monitoramento.StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando;
        }

        /**
         * Verifica se o monitoramento possui o status "Concluida"
         * - O monitoramento deve ter sido finalizado, ou
         * - A carga já encerrou a viagem ou
         * - cliente possui subArea Descarregamento e o tempo de permanencia na subarea
         */
        private static bool VerificarStatusViagemTipoRegra_Concluida(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            return historicosStatusViagem != null &&
                EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo) &&
                ((configuracaoMonitoramento?.IgnorarStatusViagemMonitoramentoAnterioresTransito ?? false) ||
                ((EsteveAguardandoHorarioCarregamento(historicosStatusViagem, posicao.DataVeiculo) || EsteveAguardandoCarregamento(historicosStatusViagem, posicao.DataVeiculo) || EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo)) &&
                 (EsteveAguardandoHorarioDescarga(historicosStatusViagem, posicao.DataVeiculo) || EsteveAguardandoDescarga(historicosStatusViagem, posicao.DataVeiculo) || EsteveEmDescarga(historicosStatusViagem, posicao.DataVeiculo)))) &&
                (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado
                || (monitoramento.Carga != null && (monitoramento.Carga.DataFimViagem.HasValue || monitoramento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada))
                || (monitoramento.StatusViagem != null && monitoramento.StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida)
                || (ClientePossuiSubareaDescarregamento(posicao, posicaoAlvos, subareasClientes) && EstaNaSubareaDescarregamentoNoTempoMinimo(posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, subareasClientes, configuracao))
                );
        }

        /**
         * Verifica se o monitoramento possui o status "Cancelada"
         * - O monitoramento deve ter sido cancelado
         */
        private static bool VerificarStatusViagemTipoRegra_Cancelada(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            return monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado
                || (monitoramento.StatusViagem != null && monitoramento.StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Cancelada);
        }

        /**
         * Verifica se o veículo está em deslocamento para a planta
         */
        private static bool VerificarStatusViagemTipoRegra_DeslocamentoParaPlanta(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporte
        )
        {
            return (
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                !EstaNaOrigem(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem) &&
                !EsteveAguardandoHorarioCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmLiberacao(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoHorarioDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveDescargaFinalizada(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo) &&
                !PossuiStatusColetaEquipamento(monitoramentoStatusViagens) &&
                !EstaSeDeslocandoEmDirecaoAoDestino(clienteOrigem, posicao, pontosDePassagem, posicoesVeiculo, configuracao, configuracaoMonitoramento) &&
                !EstaEmTransito(monitoramento)
            );
        }

        private static bool EstaSeDeslocandoEmDirecaoAoDestino(Dominio.Entidades.Cliente clienteOrigem, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            //empresas com a fla marcada  vamos validar
            if ((configuracaoMonitoramento?.IgnorarStatusViagemMonitoramentoAnterioresTransito ?? false) && VerificarConfiguracaoDirecaoAtivada(configuracao))
            {
                return VerificarSePosicoesEstaoEmDirecaoAoDestino(clienteOrigem, posicao, posicoesVeiculo, pontosDePassagem, configuracao);
            }
            else
            {
                return false;
            }
        }


        /**
         * Verifica se o veículo está aguardando horário de carregamento
         */
        private static bool VerificarStatusViagemTipoRegra_AguardandoHorarioCarregamento(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                subareasClientes != null &&
                cargaEntregas != null &&
                cargaJanelaCarregamento != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaNaOrigem(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem) &&
                !EstaNaSubareaCarregamento(posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, subareasClientes) &&
                ChegouAntesDoHorarioPrevisto(posicao, cargaJanelaCarregamento) &&
                !EsteveAguardandoCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmLiberacao(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoHorarioDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveDescargaFinalizada(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo) &&
                !EstaEmTransito(monitoramento)
            );
        }

        /**
         * Verifica se o veículo está aguardando carregamento
         */
        private static bool VerificarStatusViagemTipoRegra_AguardandoCarregamento(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                subareasClientes != null &&
                cargaEntregas != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaNaOrigem(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem) &&
                ClientePossuiSubareaCarregamento(posicao, posicaoAlvos, subareasClientes) &&
                !EstaNaSubareaCarregamento(posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, subareasClientes) &&
                !EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmLiberacao(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoHorarioDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveDescargaFinalizada(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo) &&
                !EstaEmTransito(monitoramento) &&
                (
                    cargaJanelaCarregamento == null ||
                    ChegouDepoisDoHorarioPrevisto(posicao, cargaJanelaCarregamento)
                )
            );
        }

        /**
         * Verifica se o veículo está em carregamento
         */
        private static bool VerificarStatusViagemTipoRegra_EmCarregamento(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Cliente clienteOrigem
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                subareasClientes != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaNaOrigem(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem) &&
                (subareasClientes.Length <= 0 || subareasClientes.Length > 0 || EstaNaSubareaCarregamento(posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, subareasClientes) || EsteveAguardandoCarregamento(historicosStatusViagem, posicao.DataVeiculo) || EsteveAguardandoHorarioCarregamento(historicosStatusViagem, posicao.DataVeiculo)) &&
                !EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoHorarioDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveDescargaFinalizada(historicosStatusViagem, posicao.DataVeiculo) &&
                !EstaEmTransito(monitoramento)
            );
        }

        /**
         * Verifica se o veículo está em liberação
         */
        private static bool VerificarStatusViagemTipoRegra_EmLiberacao(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Cliente clienteOrigem
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                subareasClientes != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaNaOrigem(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem) &&
                !EstaNaSubareaCarregamento(posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, subareasClientes) &&
                EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoHorarioDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmDescarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveDescargaFinalizada(historicosStatusViagem, posicao.DataVeiculo) &&
                !EstaEmTransito(monitoramento)
            );
        }

        /**
         * Verifica se o veículo está em trânsito
         */
        private static bool VerificarStatusViagemTipoRegra_Transito(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporte,
            bool regrasTransito,
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                !EstaFisicamenteNaOrigemOuNoDestino(posicao, posicaoAlvos, clienteOrigem, pontosDePassagem) &&
                !EstaNaOrigemOuNoDestino(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem, pontosDePassagem) &&
                !EstaEmParqueamento(posicao, posicaoAlvos, permanenciasClientes, cargaEntregas) &&
                !EstaFisicamenteEmFronteira(posicao, posicaoAlvos, clienteOrigem, pontosDePassagem) &&
                VerificarRegraTransito(posicao.DataVeiculo, historicosStatusViagem, regrasTransito, clienteOrigem, posicao, posicoesVeiculo, pontosDePassagem, configuracao, configuracaoMonitoramento, monitoramento.Carga, monitoramentoStatusViagem)
            );
        }

        /**
         * Verifica se o veículo está aguardando horário de descarga
         */
        private static bool VerificarStatusViagemTipoRegra_AguardandoHorarioDescarga(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                subareasClientes != null &&
                cargaEntregas != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaNoDestino(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem, pontosDePassagem) &&
                ClientePossuiSubareaDescarregamento(posicao, posicaoAlvos, subareasClientes) &&
                !EstaNaSubareaDescarregamento(posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, subareasClientes) &&
                !EsteveEmDescargaNoCliente(historicosStatusViagem, posicao.DataVeiculo, clienteOrigem, pontosDePassagem, posicao, posicaoAlvos, permanenciasClientes) &&
                CargaPossuiJanelaDescarregamentoNoCliente(posicao, posicaoAlvos, cargaEntregas, cargaJanelasDescarregamento) &&
                ChegouAntesDoHorarioPrevistoNaJanela(posicao, posicaoAlvos, cargaEntregas, cargaJanelasDescarregamento) &&
                (EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) || EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo)) &&
                !EsteveAguardandoDescargaNoCliente(historicosStatusViagem, posicao.DataVeiculo, clienteOrigem, pontosDePassagem) &&
                !EsteveDescargaFinalizadaNoCliente(historicosStatusViagem, posicao.DataVeiculo, clienteOrigem, pontosDePassagem)
            );
        }

        /**
         * Verifica se o veículo está aguardando descarga
         */
        private static bool VerificarStatusViagemTipoRegra_AguardandoDescarga(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento
        )
        {
            bool possuiJanelaDescarregamento = CargaPossuiJanelaDescarregamentoNoCliente(posicao, posicaoAlvos, cargaEntregas, cargaJanelasDescarregamento);
            return (
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                subareasClientes != null &&
                cargaEntregas != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaNoDestino(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem, pontosDePassagem) &&
                ClientePossuiSubareaDescarregamento(posicao, posicaoAlvos, subareasClientes) &&
                !EstaNaSubareaDescarregamento(posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, subareasClientes) &&
                !EsteveEmDescargaNoCliente(historicosStatusViagem, posicao.DataVeiculo, clienteOrigem, pontosDePassagem, posicao, posicaoAlvos, permanenciasClientes) &&
                (EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) || EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo)) &&
                !EsteveDescargaFinalizadaNoCliente(historicosStatusViagem, posicao.DataVeiculo, clienteOrigem, pontosDePassagem) &&
                (
                    (
                        !possuiJanelaDescarregamento
                    )
                    ||
                    (
                        possuiJanelaDescarregamento &&
                        ChegouDepoisDoHorarioPrevistoNaJanela(posicao, posicaoAlvos, cargaEntregas, cargaJanelasDescarregamento)
                    )
                )
            );
        }

        /**
         * Verifica se o veículo está em descarga
         */
        private static bool VerificarStatusViagemTipoRegra_Descarga(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporte,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                subareasClientes != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaNoDestino(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem, pontosDePassagem) &&
                !EstaFisicamenteEmFronteira(posicao, posicaoAlvos, clienteOrigem, pontosDePassagem) &&
                (EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) || EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo)) &&
                (
                    !ClientePossuiSubareaDescarregamento(posicao, posicaoAlvos, subareasClientes) ||
                    EstaNaSubareaDescarregamento(posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, subareasClientes)
                )
            );
        }

        /**
         * Verifica se o veículo finalizou a descarga
         */
        private static bool VerificarStatusViagemTipoRegra_DescargaFinalizada(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporte
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                subareasClientes != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaNoDestino(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem, pontosDePassagem) &&
                ClientePossuiSubareaDescarregamento(posicao, posicaoAlvos, subareasClientes) &&
                !EstaNaSubareaDescarregamento(posicao, posicaoAlvos, posicaoAlvoSubareas, permanenciasSubareas, subareasClientes) &&
                EsteveEmDescargaNoCliente(historicosStatusViagem, posicao.DataVeiculo, clienteOrigem, pontosDePassagem, posicao, posicaoAlvos, permanenciasClientes)
            );
        }

        /**
         * Verifica se o veículo está em deslocamento para coletar o equipamento
         */
        private static bool VerificarStatusViagemTipoRegra_DeslocamentoParaColetarEquipamento(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporte
        )
        {
            if (monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                VerificarCargaExportacaoMaritima(monitoramento, cargaEntregas, viaTransporte, codigoIntegracaoViaTransporte) &&
                !EstaFisicamenteNaOrigemOuNoDestino(posicao, posicaoAlvos, clienteOrigem, pontosDePassagem) &&
                !EsteveAguardandoHorarioCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveDeslocamentoComContainterParaPlantaColetarCarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveDeslocamentoComCargaEmContainerParaEntrega(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmColeta(historicosStatusViagem, posicao.DataVeiculo)
            )
            {
                if (EsteveDeslocamentoParaPortoColetarContainer(historicosStatusViagem, posicao.DataVeiculo))
                {
                    return true;
                }
                else if (VerificarConfiguracaoDirecaoAtivada(configuracao))
                {
                    return VerificarSePosicoesEstaoEmDirecaoAoDestino(clienteOrigem, posicao, posicoesVeiculo, pontosDePassagem, configuracao);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se o veículo está em deslocamento com o equipamento para a planta
         */
        private static bool VerificarStatusViagemTipoRegra_DeslocamentoComEquipamentoParaPlanta(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporte
        )
        {
            if (monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                VerificarCargaExportacaoMaritima(monitoramento, cargaEntregas, viaTransporte, codigoIntegracaoViaTransporte) &&
                !EstaFisicamenteNaOrigemOuNoDestino(posicao, posicaoAlvos, clienteOrigem, pontosDePassagem) &&
                !EsteveAguardandoHorarioCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveAguardandoCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveDeslocamentoComCargaEmContainerParaEntrega(historicosStatusViagem, posicao.DataVeiculo)
            )
            {
                if (EsteveDeslocamentoComContainterParaPlantaColetarCarga(historicosStatusViagem, posicao.DataVeiculo) ||
                    EsteveEmColeta(historicosStatusViagem, posicao.DataVeiculo))
                {
                    return true;
                }
                else if (VerificarConfiguracaoDirecaoAtivada(configuracao))
                {
                    return VerificarSePosicoesEstaoEmDirecaoAOrigem(clienteOrigem, posicao, posicoesVeiculo, configuracao);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se o veículo está em deslocamento com equipamento e carga para a entrega
         */
        private static bool VerificarStatusViagemTipoRegra_DeslocamentoComEquipamentoECargaParaEntrega(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporte
        )
        {
            if (monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                posicao != null &&
                pontosDePassagem != null &&
                historicosStatusViagem != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                VerificarCargaExportacaoMaritima(monitoramento, cargaEntregas, viaTransporte, codigoIntegracaoViaTransporte) &&
                !EstaFisicamenteNaOrigemOuNoDestino(posicao, posicaoAlvos, clienteOrigem, pontosDePassagem)
            )
            {
                if (EsteveDeslocamentoComCargaEmContainerParaEntrega(historicosStatusViagem, posicao.DataVeiculo) ||
                    EsteveAguardandoHorarioCarregamento(historicosStatusViagem, posicao.DataVeiculo) ||
                    EsteveAguardandoCarregamento(historicosStatusViagem, posicao.DataVeiculo) ||
                    EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo)
                )
                {
                    return true;
                }
                else if (VerificarConfiguracaoDirecaoAtivada(configuracao))
                {
                    return VerificarSePosicoesEstaoEmDirecaoAoDestino(clienteOrigem, posicao, posicoesVeiculo, pontosDePassagem, configuracao);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se o veículo está em deslocamento com a carga no container para entrega no porto
         */
        private static bool VerificarStatusViagemTipoRegra_EmColeta(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporte
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                subareasClientes != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                VerificarCargaExportacaoMaritima(monitoramento, cargaEntregas, viaTransporte, codigoIntegracaoViaTransporte) &&
                EstaNoDestino(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem, pontosDePassagem) &&
                EstaNoDestinoColeta(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem, cargaEntregas) &&
                !EsteveDeslocamentoComContainterParaPlantaColetarCarga(historicosStatusViagem, posicao.DataVeiculo) &&
                !EsteveDeslocamentoComCargaEmContainerParaEntrega(historicosStatusViagem, posicao.DataVeiculo) &&
                !ClientePossuiSubareaDescarregamento(posicao, posicaoAlvos, subareasClientes) &&
                !ClientePossuiSubareaCarregamento(posicao, posicaoAlvos, subareasClientes)
            );
        }

        /**
        * Verifica se o veículo está em local de parqueamento
        */
        private static bool VerificarStatusViagemTipoRegra_EmParqueamento(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                subareasClientes != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaEmParqueamento(posicao, posicaoAlvos, permanenciasClientes, cargaEntregas) &&
                (EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) || EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo)) &&
                !EsteveEmDescarga(historicosStatusViagem, posicao.DataVeiculo)
            );
        }

        /**
      * Verifica se o veículo está em local de parqueamento
      */
        private static bool VerificarStatusViagemTipoRegra_EmFronteira(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
             Dominio.Entidades.Cliente clienteOrigem
        )
        {
            return (
                posicao != null &&
                pontosDePassagem != null &&
                subareasClientes != null &&
                monitoramento.Veiculo != null &&
                monitoramento.Carga != null &&
                monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                EstaFisicamenteEmFronteira(posicao, posicaoAlvos, clienteOrigem, pontosDePassagem) &&
                (EsteveEmCarregamento(historicosStatusViagem, posicao.DataVeiculo) || EsteveEmTransito(historicosStatusViagem, posicao.DataVeiculo)) &&
                !EsteveEmDescarga(historicosStatusViagem, posicao.DataVeiculo)
            );
        }

        /**
         * Identifica qual status de viagem é considerado como viagem em andamento
         */
        private static Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem BuscarStatusViagemEmAndamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmViagem ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito)
                {
                    return monitoramentoStatusViagens[i];
                }
            }
            return null;
        }

        /**
         * Identifica qual status de viagem é considerado como viagem em andamento
         */
        private static bool VerificarSePosicoesEstaoEmDirecaoAoDestino(
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao
        )
        {
            if (posicao != null && posicoesVeiculo != null && pontosDePassagem != null && configuracao != null && configuracao.IdentificarMonitoramentoStatusViagemEmTransito && clienteOrigem != null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota = ExtraiWayPointsPorUmPeriodo(posicao, posicoesVeiculo, configuracao.IdentificarMonitoramentoStatusViagemEmTransitoMinutos);
                int total = wayPointsRota.Count;
                if (total > 1)
                {

                    // Confirma ter percorrida a distância mínima
                    if (Servicos.Embarcador.Logistica.Polilinha.VerificaSePercorreuDistanciaMinima(wayPointsRota, configuracao.IdentificarMonitoramentoStatusViagemEmTransitoKM * 1000))
                    {
                        double taxaDeCertezaAceita = 0.8, taxa;

                        // Coordenadas da origem da carga
                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointClienteOrigem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(clienteOrigem.Latitude, clienteOrigem.Longitude);

                        // Extrai os pontos de passagem de destino, sem a origem
                        List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemDestino = ExtraiDosPontosDePassagemDestino(clienteOrigem, pontosDePassagem);
                        total = pontosDePassagemDestino.Count;
                        for (int i = 0; i < total; i++)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDestino = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(pontosDePassagemDestino[i].Cliente.Latitude, pontosDePassagemDestino[i].Cliente.Longitude);
                            taxa = Servicos.Embarcador.Logistica.Polilinha.CalculaTaxaDeAproximacaoContinua(wayPointDestino, wayPointsRota);
                            if (taxa >= taxaDeCertezaAceita)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /**
         * Identifica qual status de viagem é considerado como viagem em andamento
         */
        private static bool VerificarSePosicoesEstaoEmDirecaoAOrigem(
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao
        )
        {
            if (posicao != null && posicoesVeiculo != null && configuracao != null && configuracao.IdentificarMonitoramentoStatusViagemEmTransito)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota = ExtraiWayPointsPorUmPeriodo(posicao, posicoesVeiculo, configuracao.IdentificarMonitoramentoStatusViagemEmTransitoMinutos);
                int total = wayPointsRota.Count;
                if (total > 1)
                {

                    // Confirma ter percorrida a distância mínima
                    if (Servicos.Embarcador.Logistica.Polilinha.VerificaSePercorreuDistanciaMinima(wayPointsRota, configuracao.IdentificarMonitoramentoStatusViagemEmTransitoKM * 1000))
                    {
                        double taxaDeCertezaAceita = 0.8;

                        // Coordenadas da origem da carga
                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint WayPointClienteOrigem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(clienteOrigem.Latitude, clienteOrigem.Longitude);

                        // Confirma que as posições indicam que o veículo está se APROXIMANDO da origem da carga
                        double taxa = Servicos.Embarcador.Logistica.Polilinha.CalculaTaxaDeAproximacaoContinua(WayPointClienteOrigem, wayPointsRota);
                        if (taxa >= taxaDeCertezaAceita)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /**
         * Identifica se o comportamento das posições indica que está se afastando da origem em direção ao destino
         */
        private static bool VerificarSePosicoesAfastaDaOrigemEmDirecaoAoDestino(
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao
        )
        {
            if (posicao != null && posicoesVeiculo != null && pontosDePassagem != null && configuracao != null && configuracao.IdentificarMonitoramentoStatusViagemEmTransito)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota = ExtraiWayPointsPorUmPeriodo(posicao, posicoesVeiculo, configuracao.IdentificarMonitoramentoStatusViagemEmTransitoMinutos);
                int total = wayPointsRota.Count;
                if (total > 1)
                {

                    // Confirma ter percorrida a distância mínima
                    if (Servicos.Embarcador.Logistica.Polilinha.VerificaSePercorreuDistanciaMinima(wayPointsRota, configuracao.IdentificarMonitoramentoStatusViagemEmTransitoKM * 1000))
                    {
                        double taxaDeCertezaAceita = 0.8;

                        // Coordenadas da origem da carga
                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointClienteOrigem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(clienteOrigem.Latitude, clienteOrigem.Longitude);

                        // Confirma que as posições indicam que o veículo está se AFASTANDO da origem da carga
                        double taxa = Servicos.Embarcador.Logistica.Polilinha.CalculaTaxaDeAfastamentoContinuo(wayPointClienteOrigem, wayPointsRota);
                        if (taxa >= taxaDeCertezaAceita)
                        {

                            // Extrai os pontos de passagem de destino, sem a origem
                            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemDestino = ExtraiDosPontosDePassagemDestino(clienteOrigem, pontosDePassagem);
                            total = pontosDePassagemDestino.Count;
                            for (int i = 0; i < total; i++)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDestino = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(pontosDePassagemDestino[i].Cliente.Latitude, pontosDePassagemDestino[i].Cliente.Longitude);
                                taxa = Servicos.Embarcador.Logistica.Polilinha.CalculaTaxaDeAproximacaoContinua(wayPointDestino, wayPointsRota);
                                if (taxa >= taxaDeCertezaAceita)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static bool VerificarRegraTransito(
            DateTime dataVeiculo,
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem,
            bool regrasTransito,
            Dominio.Entidades.Cliente clienteOrigem,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento,
            Dominio.Entidades.Embarcador.Cargas.Carga carga,
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem
        )
        {

            if ((configuracaoMonitoramento?.IgnorarStatusViagemMonitoramentoAnterioresTransito ?? false) && VerificarConfiguracaoDirecaoAtivada(configuracao))
            {
                bool estaEmDirecaoDestino = VerificarSePosicoesEstaoEmDirecaoAoDestino(clienteOrigem, posicao, posicoesVeiculo, pontosDePassagem, configuracao);

                if (estaEmDirecaoDestino)
                    return estaEmDirecaoDestino;
                else
                    return EsteveEmCarregamento(historicosStatusViagem, dataVeiculo) || EsteveAguardandoCarregamento(historicosStatusViagem, dataVeiculo) || EsteveAguardandoHorarioCarregamento(historicosStatusViagem, dataVeiculo) || EsteveEmParqueamento(historicosStatusViagem, dataVeiculo) || EsteveEmFronteira(historicosStatusViagem, dataVeiculo);
            }
            else
            {
                if (!StatusCargaPermiteAlteracao(carga, monitoramentoStatusViagem)) return false;

                if (regrasTransito)
                {
                    return EsteveEmCarregamento(historicosStatusViagem, dataVeiculo) || EsteveAguardandoCarregamento(historicosStatusViagem, dataVeiculo) || EsteveAguardandoHorarioCarregamento(historicosStatusViagem, dataVeiculo) || EsteveEmParqueamento(historicosStatusViagem, dataVeiculo) || EsteveEmFronteira(historicosStatusViagem, dataVeiculo);
                }
                else
                {
                    return EsteveEmCarregamento(historicosStatusViagem, dataVeiculo) || EsteveEmParqueamento(historicosStatusViagem, dataVeiculo) || EsteveEmFronteira(historicosStatusViagem, dataVeiculo);
                }
            }

        }

        private static List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> ObterStatusAtivos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens = repMonitoramentoStatusViagem.BuscarAtivos();
            return monitoramentoStatusViagens;
        }

        /**
         * Verifica se a posição atual está em um dos pontos de passagem de origem
         */
        private static bool EstaNaOrigem(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            Dominio.Entidades.Cliente clienteOrigem
        )
        {
            return EstaNoCliente(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem);
        }

        /**
         * Verifica se a posição atual está em um dos pontos de passagem de destino
         */
        private static bool EstaNoDestino(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem
        )
        {
            if (clienteOrigem != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemDestino = ExtraiDosPontosDePassagemDestino(clienteOrigem, pontosDePassagem);
                int total = pontosDePassagemDestino.Count;
                for (int i = 0; i < total; i++)
                {
                    if (EstaNoCliente(posicao, posicaoAlvos, permanenciasClientes, pontosDePassagemDestino[i].Cliente))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Verifica se a posição atual está na oritem ou em um dos pontos de passagem de destino
         */
        private static bool EstaNaOrigemOuNoDestino(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem
        )
        {
            if (EstaNaOrigem(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem))
            {
                return true;
            }
            return EstaNoDestino(posicao, posicaoAlvos, permanenciasClientes, clienteOrigem, pontosDePassagem);
        }

        /**
         * Verifica se a posição atual está na origem e em nenhum dos pontos de passagem de destino, fisicamente, ou seja, observa a permanência e o alvo da posição
         */
        private static bool EstaFisicamenteNaOrigemOuNoDestino(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem
        )
        {
            if ((posicao?.EmAlvo ?? false) && posicaoAlvos != null && posicaoAlvos.Count > 0)
            {
                // Está fisicamente na origem?
                if (EstaFisicamenteNoCliente(posicao, posicaoAlvos, clienteOrigem))
                {
                    return true;
                }

                // .. ou verifica se está em algum dos destinos
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemDestino = ExtraiDosPontosDePassagemDestino(clienteOrigem, pontosDePassagem);
                int total = pontosDePassagemDestino.Count;
                for (int i = 0; i < total; i++)
                {
                    if (EstaFisicamenteNoCliente(posicao, posicaoAlvos, pontosDePassagemDestino[i].Cliente))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Verifica se a posição atual está em algum destino marcado como coleta
         **/
        private static bool EstaNoDestinoColeta(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas
        )
        {
            if (cargaEntregas != null)
            {
                int total = cargaEntregas.Count;
                for (int i = 0; i < total; i++)
                {
                    if (cargaEntregas[i].Cliente?.Codigo != clienteOrigem.Codigo && cargaEntregas[i].Coleta == true && EstaNoCliente(posicao, posicaoAlvos, permanenciasClientes, cargaEntregas[i].Cliente))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Verifica se a posição atual está no cliente
         */
        private static bool EstaNoCliente(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            Dominio.Entidades.Cliente cliente
        )
        {
            Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaCliente;
            if ((posicao?.EmAlvo ?? false) && posicaoAlvos != null && posicaoAlvos.Count > 0 && cliente != null)
            {
                int total = posicaoAlvos.Count;
                for (int i = 0; i < total; i++)
                {
                    if (posicaoAlvos[i] != null && posicaoAlvos[i].Cliente != null && posicaoAlvos[i]?.Cliente?.CPF_CNPJ == cliente.CPF_CNPJ)
                    {
                        permanenciaCliente = BuscarPermancenciaClienteEmAberto(permanenciasClientes, cliente);
                        return (permanenciaCliente != null);
                    }
                }
            }
            permanenciaCliente = BuscarPermancenciaClienteEmAberto(permanenciasClientes, cliente);
            return (permanenciaCliente != null);
        }

        /**
         * Verifica se a posição atual está no cliente fisicamente, ou seja, não observa a permanência nos clientes, mas o alvo da posição
         */
        private static bool EstaFisicamenteNoCliente(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            Dominio.Entidades.Cliente cliente
        )
        {
            if ((posicao?.EmAlvo ?? false) && posicaoAlvos != null && posicaoAlvos.Count > 0 && cliente != null)
            {
                int total = posicaoAlvos.Count;
                for (int i = 0; i < total; i++)
                {
                    if (posicaoAlvos[i].Cliente != null && posicaoAlvos[i].Cliente.CPF_CNPJ == cliente.CPF_CNPJ)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Verifica se a posição corresponde a uma subárea de carregamento do cliente
         */
        private static bool EstaNaSubareaCarregamento(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes
        )
        {
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareas = FiltraSubareasDoCliente(posicaoAlvos, subareasClientes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Carregamento);
            return EstaNaSubarea(posicao, posicaoAlvoSubareas, permanenciasSubareas, subareas);
        }

        /**
         * Verifica se a posição corresponde a uma subárea de carregamento do cliente
         */
        private static bool EstaNaSubareaDescarregamento(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes
        )
        {
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareas = FiltraSubareasDoCliente(posicaoAlvos, subareasClientes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Descarregamento);
            return EstaNaSubarea(posicao, posicaoAlvoSubareas, permanenciasSubareas, subareas);
        }

        private static bool EstaNaSubareaDescarregamentoNoTempoMinimo(
           Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
           List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
           List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
           List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
           Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
           Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS
       )
        {
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareas = FiltraSubareasDoCliente(posicaoAlvos, subareasClientes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Descarregamento);
            return EstaNaSubareaTempoMinimo(posicao, posicaoAlvoSubareas, permanenciasSubareas, subareas, configuracaoTMS);
        }

        /**
         * Verifica se a posição corresponde a uma das subáreas
         */
        private static bool EstaNaSubarea(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes
        )
        {
            int total = subareasClientes?.Length ?? 0;
            if ((posicao?.EmAlvo ?? false) && total > 0)
            {
                int totalAlvoSubareas = posicaoAlvoSubareas.Count;
                for (int i = 0; i < total; i++)
                {
                    for (int j = 0; j < totalAlvoSubareas; j++)
                    {
                        if (subareasClientes[i].Codigo == posicaoAlvoSubareas[j].SubareaCliente.Codigo)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubarea = BuscarPermancenciaSubareaEmAberto(permanenciasSubareas, subareasClientes[i]);
                            if (permanenciaSubarea != null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        private static bool EstaNaSubareaTempoMinimo(
     Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
     List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvoSubareas,
     List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
     Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
     Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS
 )
        {
            int total = subareasClientes?.Length ?? 0;
            if ((posicao?.EmAlvo ?? false) && total > 0)
            {
                int totalAlvoSubareas = posicaoAlvoSubareas.Count;
                for (int i = 0; i < total; i++)
                {
                    for (int j = 0; j < totalAlvoSubareas; j++)
                    {
                        if (subareasClientes[i].Codigo == posicaoAlvoSubareas[j].SubareaCliente.Codigo)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubarea = BuscarPermancenciaSubareaEmAberto(permanenciasSubareas, subareasClientes[i]);
                            if (permanenciaSubarea != null && permanenciaSubarea.Tempo.TotalMinutes >= configuracaoTMS.TempoMinutosPermanenciaSubareaCliente)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        /**
         * Verifica se o cliente alvo da posição possui alguma subárea de descarregamento
         */
        private static bool ClientePossuiSubareaDescarregamento(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes
        )
        {
            if ((posicao?.EmAlvo ?? false) && posicaoAlvos != null && posicaoAlvos.Count > 0)
            {
                Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareas = FiltraSubareasDoCliente(posicaoAlvos, subareasClientes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Descarregamento);
                return (subareas?.Length ?? 0) > 0;
            }
            return false;
        }

        /**
         * Verifica se o cliente alvo da posição possui alguma subárea de carregamento
         */
        private static bool ClientePossuiSubareaCarregamento(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes
        )
        {
            if ((posicao?.EmAlvo ?? false) && posicaoAlvos != null && posicaoAlvos.Count > 0)
            {
                Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareas = FiltraSubareasDoCliente(posicaoAlvos, subareasClientes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente.Carregamento);
                return (subareas?.Length ?? 0) > 0;
            }
            return false;
        }

        /**
         * Filtra um tipo específico de subárea do cliente
         */
        private static Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] FiltraSubareasDoCliente(
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareasClientes,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente tipoSubarea
        )
        {
            List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> subareas = new List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente>();
            int total = posicaoAlvos.Count;
            int totalSubareas = subareasClientes?.Length ?? 0;
            for (int i = 0; i < total; i++)
            {
                for (int j = 0; j < totalSubareas; j++)
                {
                    // É uma das subáreas do cliente e do tipo desejado
                    if (posicaoAlvos[i].Cliente.Codigo == subareasClientes[j].Cliente?.Codigo && (tipoSubarea == 0 || subareasClientes[j].TipoSubarea.Tipo == tipoSubarea))
                    {
                        subareas.Add(subareasClientes[j]);
                    }
                }
            }
            return subareas.ToArray();
        }

        /**
         * Verifica se o monitoramento passou, em aglum momento, por algum status do tipo de regra
         */
        private static bool VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra,
            DateTime dataReferencia,
            double codigoCliente = 0,
            double codigoSubarea = 0
        )
        {
            int total = hitoricoStatusViagem.Count;
            for (int i = 0; i < total; i++)
            {
                if (
                    hitoricoStatusViagem[i].StatusViagem != null && hitoricoStatusViagem[i].StatusViagem.TipoRegra == tipoRegra && hitoricoStatusViagem[i].DataInicio < dataReferencia &&
                    (codigoCliente == 0 || (hitoricoStatusViagem[i].Cliente != null && hitoricoStatusViagem[i].Cliente.CPF_CNPJ == codigoCliente)) &&
                    (codigoSubarea == 0 || (hitoricoStatusViagem[i].SubareaCliente != null && hitoricoStatusViagem[i].SubareaCliente.Codigo == codigoSubarea))
                )
                {
                    return true;
                }
            }
            return false;
        }

        private static bool EstaEmTransito(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            return (monitoramento != null && monitoramento.StatusViagem != null && monitoramento.StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito);
        }


        /**
        * Verifica se a posição atual está em algum destino marcado como coleta
        **/
        private static bool EstaEmParqueamento(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas
        )
        {
            if (cargaEntregas != null)
            {
                int total = cargaEntregas.Count;
                for (int i = 0; i < total; i++)
                {
                    if (cargaEntregas[i].Parqueamento == true && EstaNoCliente(posicao, posicaoAlvos, permanenciasClientes, cargaEntregas[i].Cliente))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private static bool EstaFisicamenteEmFronteira(
           Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
           List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
           Dominio.Entidades.Cliente clienteOrigem,
           List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem
       )
        {
            if ((posicao?.EmAlvo ?? false) && posicaoAlvos != null && posicaoAlvos.Count > 0)
            {
                // Está fisicamente na origem?
                if (EstaEmFronteira(posicao, posicaoAlvos, clienteOrigem))
                {
                    return true;
                }

                // .. ou verifica se está em algum dos destinos
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemDestino = ExtraiDosPontosDePassagemDestino(clienteOrigem, pontosDePassagem);
                int total = pontosDePassagemDestino.Count;
                for (int i = 0; i < total; i++)
                {
                    if (EstaEmFronteira(posicao, posicaoAlvos, pontosDePassagemDestino[i].Cliente))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
        * Verifica se a posição atual está no cliente Fronteira, ou seja, não observa a permanência nos clientes, mas o alvo da posição
        */
        private static bool EstaEmFronteira(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            Dominio.Entidades.Cliente cliente
        )
        {
            if ((posicao?.EmAlvo ?? false) && posicaoAlvos != null && posicaoAlvos.Count > 0 && cliente != null)
            {
                int total = posicaoAlvos.Count;
                for (int i = 0; i < total; i++)
                {
                    if (posicaoAlvos[i].Cliente != null && posicaoAlvos[i].Cliente.FronteiraAlfandega && posicaoAlvos[i].Cliente.CPF_CNPJ == cliente.CPF_CNPJ)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /**
         * Verifica se há algum registro de algum status do tipo de regra "Deslocamento para planta"
         */
        private static bool EsteveDeslocamentoParaPlanta(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Em trânsito"
         */
        private static bool EsteveEmTransito(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Aguardando horário de carregamento"
         */
        private static bool EsteveAguardandoHorarioCarregamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Aguardando de carregamento"
         */
        private static bool EsteveAguardandoCarregamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Em carregamento"
         */
        private static bool EsteveEmCarregamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmCarregamento,
                data
            );
        }

        /**
     * Verifica se há algum registro de algum status do tipo de regra "Em Parqueamento"
     */
        private static bool EsteveEmParqueamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmParqueamento,
                data
            );
        }

        /**
  * Verifica se há algum registro de algum status do tipo de regra "Em Fronteira"
  */
        private static bool EsteveEmFronteira(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmFronteira,
                data
            );
        }



        /**
         * Verifica se há algum registro de algum status do tipo de regra "Em liberação"
         */
        private static bool EsteveEmLiberacao(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmLiberacao,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Descarga"
         */
        private static bool EsteveEmDescarga(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Descarga" neste cliente
         */
        private static bool EsteveEmDescargaNoCliente(
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem,
            DateTime data,
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem,
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes
        )
        {
            if (clienteOrigem != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemDestino = ExtraiDosPontosDePassagemDestino(clienteOrigem, pontosDePassagem);
                int total = pontosDePassagemDestino?.Count ?? 0;
                for (int i = 0; i < total; i++)
                {
                    if (
                        EstaNoCliente(posicao, posicaoAlvos, permanenciasClientes, pontosDePassagemDestino[i].Cliente)
                        &&
                        VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                            hitoricoStatusViagem,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Descarga,
                            data,
                            pontosDePassagemDestino[i].Cliente.Codigo
                        )
                    )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Descarga finalizada"
         */
        private static bool EsteveDescargaFinalizada(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DescargaFinalizada,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Descarga finalizada" neste cliente
         */
        private static bool EsteveDescargaFinalizadaNoCliente(
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem,
            DateTime data,
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem
        )
        {
            if (clienteOrigem != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemDestino = ExtraiDosPontosDePassagemDestino(clienteOrigem, pontosDePassagem);
                int total = pontosDePassagemDestino?.Count ?? 0;
                for (int i = 0; i < total; i++)
                {
                    if (
                        VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                            hitoricoStatusViagem,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DescargaFinalizada,
                            data,
                            pontosDePassagemDestino[i].Cliente.Codigo
                        )
                    )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Aguardando descarga"
         */
        private static bool EsteveAguardandoDescarga(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Aguardando descarga" neste cliente
         */
        private static bool EsteveAguardandoDescargaNoCliente(
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem,
            DateTime data,
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem
        )
        {
            if (clienteOrigem != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemDestino = ExtraiDosPontosDePassagemDestino(clienteOrigem, pontosDePassagem);
                int total = pontosDePassagemDestino?.Count ?? 0;
                for (int i = 0; i < total; i++)
                {
                    if (
                        VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                            hitoricoStatusViagem,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoDescarga,
                            data,
                            pontosDePassagemDestino[i].Cliente.Codigo
                        )
                    )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Aguardando horário de descarga"
         */
        private static bool EsteveAguardandoHorarioDescarga(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioDescarga,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Deslocamento para porto coletar container"
         */
        private static bool EsteveDeslocamentoParaPortoColetarContainer(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Deslocamento com containter para planta coletar carga"
         */
        private static bool EsteveDeslocamentoComContainterParaPlantaColetarCarga(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Deslocamento com carga em container para entrega"
         */
        private static bool EsteveDeslocamentoComCargaEmContainerParaEntrega(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoECargaParaEntrega,
                data
            );
        }

        /**
         * Verifica se há algum registro de algum status do tipo de regra "Em coleta"
         */
        private static bool EsteveEmColeta(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> hitoricoStatusViagem, DateTime data)
        {
            return VerificaSeMonitoramentoPassouPeloStatusViagemTipoRegra(
                hitoricoStatusViagem,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmColeta,
                data
            );
        }

        /**
         * Verifica se a posição representa uma chegada antes do horário previsto
         */
        private static bool ChegouAntesDoHorarioPrevisto(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento
        )
        {
            return cargaJanelaCarregamento != null && posicao.DataVeiculo < cargaJanelaCarregamento.InicioCarregamento;
        }

        /**
         * Verifica se a posição representa uma chegada depois do horário previsto
         */
        private static bool ChegouDepoisDoHorarioPrevisto(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento
        )
        {
            return cargaJanelaCarregamento != null && posicao.DataVeiculo >= cargaJanelaCarregamento.InicioCarregamento;
        }

        /**
         * Verifica se a posição representa uma chegada antes do horário previsto de carregamento
         */
        private static bool ChegouAntesDoHorarioPrevistoNaJanela(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento
        )
        {
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = BuscarJanelaDescarregamentoDoCliente(posicao, posicaoAlvos, cargaEntregas, cargaJanelasDescarregamento);
            return cargaJanelaDescarregamento != null && posicao.DataVeiculo < cargaJanelaDescarregamento.InicioDescarregamento;
        }

        /**
         * Verifica se a posição representa uma chegada depois do horário previsto na janela de descarregamento
         */
        private static bool ChegouDepoisDoHorarioPrevistoNaJanela(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento
        )
        {
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = BuscarJanelaDescarregamentoDoCliente(posicao, posicaoAlvos, cargaEntregas, cargaJanelasDescarregamento);
            return cargaJanelaDescarregamento != null && posicao.DataVeiculo >= cargaJanelaDescarregamento.InicioDescarregamento;
        }

        /**
         * Verifica se a carga possui janela de descarregamento para o cliente que está  no alvo
         */
        private static bool CargaPossuiJanelaDescarregamentoNoCliente(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento
        )
        {
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = BuscarJanelaDescarregamentoDoCliente(posicao, posicaoAlvos, cargaEntregas, cargaJanelasDescarregamento);
            return cargaJanelaDescarregamento != null;
        }

        /**
         * Verifica se a carga possui janela de descarregamento para o cliente que está  no alvo
         */
        private static Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento BuscarJanelaDescarregamentoDoCliente(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento
        )
        {
            if ((posicao?.EmAlvo ?? false) == true && posicaoAlvos != null && cargaEntregas != null && cargaJanelasDescarregamento != null)
            {
                int totalPosicaoAlvos = posicaoAlvos.Count;
                int totalCargaEntregas = cargaEntregas.Count;
                int totalCargaJanelasDescarregamento = cargaJanelasDescarregamento.Count;
                for (int i = 0; i < totalPosicaoAlvos; i++)
                {
                    for (int j = 0; j < totalCargaEntregas; j++)
                    {
                        if (posicaoAlvos[i].Cliente != null && cargaEntregas[j].Cliente != null && posicaoAlvos[i].Cliente.Codigo == cargaEntregas[j].Cliente.Codigo && cargaEntregas[j].Coleta == false)
                        {
                            for (int k = 0; k < totalCargaJanelasDescarregamento; k++)
                            {
                                if (cargaJanelasDescarregamento[k].CentroDescarregamento != null &&
                                    cargaJanelasDescarregamento[k].CentroDescarregamento.Destinatario != null &&
                                    cargaEntregas[j].Cliente.Codigo == cargaJanelasDescarregamento[k].CentroDescarregamento.Destinatario.Codigo)
                                {
                                    return cargaJanelasDescarregamento[k];
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /**
         * Converte o dia da semana de uma data (0 a 6)
         * para o dia da semana do enumerador (1 a 7)
         */
        private static Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana ConverterDiaDaSemana(DateTime date)
        {
            DayOfWeek dia = date.DayOfWeek;
            return (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)((int)dia) + 1;
        }

        /**
         * Extrai os pontos de passagem de destino
         */
        private static List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> ExtraiDosPontosDePassagemDestino(
            Dominio.Entidades.Cliente clienteOrigem,
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem
        )
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemDestino = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            if (clienteOrigem != null && pontosDePassagem != null)
            {
                int total = pontosDePassagem.Count;
                for (int i = 0; i < total; i++)
                {
                    if (pontosDePassagem[i].Cliente != null && pontosDePassagem[i].Cliente.CPF_CNPJ != clienteOrigem.CPF_CNPJ &&
                        (pontosDePassagem[i].TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta || pontosDePassagem[i].TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega || pontosDePassagem[i].TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira)
                    )
                    {
                        pontosDePassagemDestino.Add(pontosDePassagem[i]);
                    }
                }
            }
            return pontosDePassagemDestino;
        }

        /**
         * Extrai WayPoints das posições se existir o tempo mínimo de histórico desejado
         */
        private static List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> ExtraiWayPointsPorUmPeriodo(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo,
            int periodoEmMinutos
        )
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
            int total = posicoesVeiculo.Count;
            if (total > 1)
            {
                // O conjunto de posições ordenadas contém o período?
                double diferencaMinutos = (posicao.DataVeiculo - posicoesVeiculo[0].DataVeiculo).TotalMinutes;
                if (diferencaMinutos >= periodoEmMinutos)
                {
                    DateTime dataLimite = posicao.DataVeiculo.AddMinutes(-periodoEmMinutos);

                    // Identifica o limiar de separação do período considerando um conjunto fechado
                    int indiceInicial = 0;
                    for (int i = 0; i < total; i++)
                    {
                        if (posicoesVeiculo[i].DataVeiculo >= dataLimite && posicoesVeiculo[i].DataVeiculo <= posicao.DataVeiculo)
                        {
                            break;
                        }
                        indiceInicial = i;
                    }

                    for (int i = indiceInicial; i < total; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wp = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(posicoesVeiculo[i].Latitude, posicoesVeiculo[i].Longitude);
                        wayPointsRota.Add(wp);
                    }
                }
            }
            return wayPointsRota;
        }

        private static bool VerificarConfiguracaoDirecaoAtivada(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            return configuracao.IdentificarMonitoramentoStatusViagemEmTransito && configuracao.IdentificarMonitoramentoStatusViagemEmTransitoKM > 0 && configuracao.IdentificarMonitoramentoStatusViagemEmTransitoMinutos > 0;
        }

        /**
         * Cargas de exportação marítima devem:
         * - Possuir obrigatoriamente 3 ou mais entregas;
         * - Os dois primeiros destinos são coletas;
         * - O primeiro e o último destino devem ser iguais; 
         * - A via de transporte deve ser a configurada como marítima
         */
        private static bool VerificarCargaExportacaoMaritima(
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas,
            Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte,
            string codigoIntegracaoViaTransporteMaritima
        )
        {
            return (
                monitoramento != null &&
                monitoramento.Carga != null &&
                cargaEntregas != null &&
                cargaEntregas.Count >= 3 &&
                cargaEntregas[0].Coleta == true &&
                cargaEntregas[0].ColetaEquipamento == true &&
                cargaEntregas[1].Coleta == true &&
                cargaEntregas[1].ColetaEquipamento == false &&
                cargaEntregas[0].Cliente != null &&
                cargaEntregas[cargaEntregas.Count - 1].Cliente != null &&
                cargaEntregas[0].Cliente.CPF_CNPJ == cargaEntregas[cargaEntregas.Count - 1].Cliente.CPF_CNPJ &&
                VerificaViaTransporteCodigoIntegracao(viaTransporte, codigoIntegracaoViaTransporteMaritima)
            );
        }

        /**
         * Verifica se a via de transporte trata-se do código de integração
         */
        private static bool VerificaViaTransporteCodigoIntegracao(Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte, string codigoIntegracaoViaTransporte)
        {
            return (
                viaTransporte != null &&
                !string.IsNullOrWhiteSpace(codigoIntegracaoViaTransporte) &&
                !string.IsNullOrWhiteSpace(viaTransporte.CodigoIntegracao) &&
                viaTransporte.CodigoIntegracao == codigoIntegracaoViaTransporte
            );
        }

        /**
         * Verifica se a posição representa uma chegada antes do horário previsto no controle de entrega
         */
        private static bool ChegouAntesDoHorarioPrevisto(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas
        )
        {
            if (posicao?.EmAlvo ?? false && posicaoAlvos != null && cargaEntregas != null)
            {
                int totalEntregas = cargaEntregas.Count;
                int totalAlvos = posicaoAlvos.Count;
                for (int i = 0; i < totalEntregas; i++)
                {
                    for (int j = 0; j < totalAlvos; j++)
                    {
                        if (cargaEntregas[i].Cliente?.CPF_CNPJ == posicaoAlvos[j].Cliente.CPF_CNPJ && cargaEntregas[i].DataPrevista != null && posicao.DataVeiculo < cargaEntregas[i].DataPrevista)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /**
         * Verifica se a posição representa uma chegada depois do horário previsto no controle de entrega
         */
        private static bool ChegouDepoisDoHorarioPrevisto(
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao,
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas
        )
        {
            if (posicao?.EmAlvo ?? false && posicaoAlvos != null && cargaEntregas != null)
            {
                int totalEntregas = cargaEntregas.Count;
                int totalAlvos = posicaoAlvos.Count;
                for (int i = 0; i < totalEntregas; i++)
                {
                    for (int j = 0; j < totalAlvos; j++)
                    {
                        if (cargaEntregas[i].Cliente?.CPF_CNPJ == posicaoAlvos[j].Cliente.CPF_CNPJ && cargaEntregas[i].DataPrevista != null && posicao.DataVeiculo >= cargaEntregas[i].DataPrevista)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /**
         * Busca todas as permanências no cliente
         */
        private static List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> BuscarPermancenciasCliente(
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            Dominio.Entidades.Cliente cliente
        )
        {
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanencias = new List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            if (cliente != null && permanenciasClientes != null)
            {
                int total = permanenciasClientes.Count;
                for (int i = 0; i < total; i++)
                {
                    if (permanenciasClientes[i].Cliente.Codigo == cliente.Codigo)
                    {
                        permanencias.Add(permanenciasClientes[i]);
                    }
                }
            }
            return permanencias;
        }

        /**
         * Busca a permanência no cliente em aberto, ou seja, que indica que ainda está no cliente
         */
        private static Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente BuscarPermancenciaClienteEmAberto(
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
            Dominio.Entidades.Cliente cliente
        )
        {
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasNoCliente = BuscarPermancenciasCliente(permanenciasClientes, cliente);
            return BuscarPermancenciaClienteEmAberto(permanenciasNoCliente);
        }

        /**
         * Busca a permanência no cliente em aberto, ou seja, que indica que ainda está no cliente
         */
        private static Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente BuscarPermancenciaClienteEmAberto(
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes
        )
        {
            int total = permanenciasClientes?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (permanenciasClientes[i].DataFim == null)
                {
                    return permanenciasClientes[i];
                }
            }
            return null;
        }

        /**
         * Busca a última permanência no cliente encerrada
         */
        private static Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente BuscarUltimaPermanenciaClienteEncerrada(
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes
        )
        {
            Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente ultimaPermanenciaEncerrada = null;
            int total = permanenciasClientes?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (permanenciasClientes[i].DataFim != null && (ultimaPermanenciaEncerrada == null || ultimaPermanenciaEncerrada.DataFim < permanenciasClientes[i].DataFim))
                {
                    ultimaPermanenciaEncerrada = permanenciasClientes[i];
                }
            }
            return ultimaPermanenciaEncerrada;
        }

        /**
            * Busca as permanências no cliente encerradas, ou seja, entrou e saiu do cliente
            */
        private static List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> BuscarPermanenciasClienteEncerradas(
        List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes,
        Dominio.Entidades.Cliente cliente
        )
        {
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasEncerradas = new List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasNoCliente = BuscarPermancenciasCliente(permanenciasClientes, cliente);
            int total = permanenciasNoCliente.Count;
            for (int i = 0; i < total; i++)
            {
                if (permanenciasNoCliente[i].DataFim != null)
                {
                    permanenciasEncerradas.Add(permanenciasNoCliente[i]);
                }
            }
            return permanenciasEncerradas;
        }


        /**
         * Busca todas as permanências na subárea cliente
         */
        private static List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPermancenciasSubarea(
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente subarea
        )
        {
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanencias = new List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            if (subarea != null && permanenciasSubareas != null)
            {
                int total = permanenciasSubareas.Count;
                for (int i = 0; i < total; i++)
                {
                    if (permanenciasSubareas[i].Subarea.Codigo == subarea.Codigo)
                    {
                        permanencias.Add(permanenciasSubareas[i]);
                    }
                }
            }
            return permanencias;
        }

        /**
         * Busca a permanência na subárea cliente em aberto, ou seja, que indica que ainda está na subarea
         */
        private static Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea BuscarPermancenciaSubareaEmAberto(
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente subarea
        )
        {
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasNaSubarea = BuscarPermancenciasSubarea(permanenciasSubareas, subarea);
            return BuscarPermancenciaSubareaEmAberto(permanenciasNaSubarea);
        }

        /**
         * Busca a permanência na subárea cliente em aberto, ou seja, que indica que ainda está na subarea
         */
        private static Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea BuscarPermancenciaSubareaEmAberto(
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas
        )
        {
            int total = permanenciasSubareas?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (permanenciasSubareas[i].DataFim == null)
                {
                    return permanenciasSubareas[i];
                }
            }
            return null;
        }

        /**
         * Busca as permanências na subárea do cliente encerradas, ou seja, entrou e saiu da subárea
         */
        private static List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPermancenciasSubareaEncerradas(
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas,
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente subarea
        )
        {
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasEncerradas = new List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasNaSubarea = BuscarPermancenciasSubarea(permanenciasSubareas, subarea);
            int total = permanenciasNaSubarea.Count;
            for (int i = 0; i < total; i++)
            {
                if (permanenciasNaSubarea[i].DataFim != null)
                {
                    permanenciasEncerradas.Add(permanenciasNaSubarea[i]);
                }
            }
            return permanenciasEncerradas;
        }

        private static bool PossuiStatusColetaEquipamento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens)
        {
            int total = monitoramentoStatusViagens?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta ||
                    monitoramentoStatusViagens[i].TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoECargaParaEntrega
                )
                {
                    return true;
                }
            }
            return false;
        }

        private static bool PossuiEntregaEmAberto(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas)
        {
            int total = cargaEntregas?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (!cargaEntregas[i].Coleta && cargaEntregas[i].DataConfirmacao == null)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Verifica se a Situação da Carga permite alteração para o Status da Viagem.
         */
        private static bool StatusCargaPermiteAlteracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem monitoramentoStatusViagem)
        {
            return !(monitoramentoStatusViagem?.ValidarStatusCargaAoTrocarStatusViagem ?? false) ||
                    monitoramentoStatusViagem.StatusCargaAoTrocarStatusViagem == (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaValidacaoStatusViagemMonitoramento)carga?.SituacaoCarga;
        }

        #endregion

    }

}
