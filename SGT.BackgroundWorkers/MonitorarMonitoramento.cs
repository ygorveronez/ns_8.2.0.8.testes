using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class MonitorarMonitoramento : LongRunningProcessBase<MonitorarMonitoramento>
    {
        #region propriedades privadas
        private DateTime dataAtual;
        #endregion

        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            try
            {
                Servicos.Log.TratarErro("-- Ciclo Iniciado --", "MonitorarMonitoramento");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

                if (configuracaoTMS.PossuiMonitoramento)
                {
                    try
                    {
                        Servicos.Log.TratarErro("Iniciou BuscarMonitoramentosIniciarDataCarregamento", "MonitorarMonitoramento");
                        BuscarMonitoramentosIniciarDataCarregamento(unitOfWork);

                        Servicos.Log.TratarErro("Finalizou BuscarMonitoramentosIniciarDataCarregamento e Iniciou FinalizarAlertaDeMonitoramentoPeriodicamente", "MonitorarMonitoramento");
                        FinalizarAlertaDeMonitoramentoPeriodicamente(unitOfWork);

                        Servicos.Log.TratarErro("Finalizou FinalizarAlertaDeMonitoramentoPeriodicamente e Iniciou FinalizarMonitoramentosAutomaticamente", "MonitorarMonitoramento");
                        FinalizarMonitoramentosAutomaticamente(unitOfWork);
                        Servicos.Log.TratarErro("Finalizou FinalizarMonitoramentosAutomaticamente", "MonitorarMonitoramento");

                        if (!_urlHomologacao)
                        {
                            Servicos.Log.TratarErro("Iniciou AlertarTecnlogiaSemSinal e ProcessarAlertaMonitoramentoPosicoesPendentes", "MonitorarMonitoramento");

                            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                            if (configuracaoMonitoramento.EnviarAlertasMonitoramentoEmail)
                            {
                                AlertarTecnlogiaSemSinal(unitOfWork);
                            }

                            ProcessarAlertaMonitoramentoPosicoesPendentes(unitOfWork);
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "MonitorarMonitoramento");
                    }
                }

                //AlertarTecnlogiaSemSinal(unitOfWork); //isso vamos colocar no painel do grafana
                Servicos.Log.TratarErro("Iniciou BuscarECorrigirCargasSemControleEntrega", "MonitorarMonitoramento");
                BuscarECorrigirCargasSemControleEntrega(unitOfWork);
                Servicos.Log.TratarErro("Finalizou BuscarECorrigirCargasSemControleEntrega e Iniciou BuscarECorrigirCargasSemIntegracao", "MonitorarMonitoramento");
                BuscarECorrigirCargasSemIntegracao(unitOfWork);
                Servicos.Log.TratarErro("Finalizou BuscarECorrigirCargasSemIntegracao e Iniciou BuscarECorrigirCargasEntregasSemNotasFiscais", "MonitorarMonitoramento");
                BuscarECorrigirCargasEntregasSemNotasFiscais(unitOfWork);
                Servicos.Log.TratarErro("Finalizou BuscarECorrigirCargasEntregasSemNotasFiscais e Iniciou BuscarECorrigirCargasEntregasSemProdutos", "MonitorarMonitoramento");
                BuscarECorrigirCargasEntregasSemProdutos(unitOfWork);
                Servicos.Log.TratarErro("Finalizou BuscarECorrigirCargasEntregasSemProdutos", "MonitorarMonitoramento");

                Servicos.Log.TratarErro("-- Fim do Ciclo --", "MonitorarMonitoramento");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "MonitorarMonitoramento");
                Servicos.Log.TratarErro("-- Fim do Ciclo Exception --", "MonitorarMonitoramento");
            }
        }

        #endregion

        #region Método Privados

        private void BuscarMonitoramentosIniciarDataCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            this.dataAtual = DateTime.Now;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasIniciarMonitoramento = repCarga.ConsultarCargasDataCarregamentoInicioMonitoramentoAutomatico(this.dataAtual);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasIniciarMonitoramento)
            {
                Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, configuracao, null, "Carga na data de carregamento - Iniciar Monitoramento Automaticamente", unitOfWork, true);
            }
        }

        private void AlertarTecnlogiaSemSinal(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            if (configuracaoTMS.PossuiMonitoramento)
            {
                Servicos.Embarcador.Logistica.Monitoramento.SemSinalAntesDoCarregamento semSinalAntesDoCarregamento = new Servicos.Embarcador.Logistica.Monitoramento.SemSinalAntesDoCarregamento(unitOfWork, configuracaoTMS);
                semSinalAntesDoCarregamento.Iniciar();

                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Posicao ultimaPosicao = repPosicao.BuscarUltimaPosicao();
                if (ultimaPosicao != null)
                {
                    DateTime dataAtual = DateTime.Now;
                    TimeSpan diff = dataAtual - ultimaPosicao.DataCadastro;
                    int totalMinutesLate = 25;
                    if (diff.TotalMinutes >= totalMinutesLate)
                    {
                        string maskDate = "dd/MM/yyyy HH:mm:ss";

                        string cliente = _clienteMultisoftware.RazaoSocial;
                        string subject = "Problemas no monitoramento";
                        string body = $"<h1>Atenção! {subject}</h1>";
                        body += "<p>";
                        body += $"Ambiente: {cliente}<br/>";
                        body += $"Serviço Multisoftware: {_tipoServicoMultisoftware}<br/>";
                        body += $"Data atual: {dataAtual.ToString(maskDate)}<br/>";
                        body += $"Última posição: {ultimaPosicao.DataCadastro.ToString(maskDate)}<br/>";
                        body += $"Atraso (>{totalMinutesLate}min): {diff.ToString(@"d\.hh\:mm\:ss")}";
                        body += "</p>";


                        Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                        List<string> emails = new List<string>();
                        if (!string.IsNullOrWhiteSpace(configuracaoMonitoramento.EmailsAlertaMonitoramento))
                            emails.AddRange(configuracaoMonitoramento.EmailsAlertaMonitoramento.Split(';').ToList());

                        emails = emails.Distinct().ToList();
                        emails.Add("fernando.morh@multisoftware.com.br");
                        emails.Add("rodolfo.trevisol@multisoftware.com.br");
                        emails.Add("guilherme.romanini@multisoftware.com.br");
                        if (emails.Count > 0)
                        {
                            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);
                            svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, cliente + " - " + subject, body, string.Empty, null, string.Empty, true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, emails, false);
                        }
                    }
                }
            }
        }

        private void FinalizarAlertaDeMonitoramentoPeriodicamente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Repositorio.Embarcador.Logistica.AlertaMonitor repositorioAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
            Servicos.Embarcador.Monitoramento.AlertaMonitor serAlertaMonitor = new Servicos.Embarcador.Monitoramento.AlertaMonitor();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
            if (!(configuracaoMonitoramento?.FinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente ?? false) || configuracaoMonitoramento.DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente <= 0) return;

            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertasAbertoPeriodo = repositorioAlerta.BuscarAlertasEmAbertoPorNDias(DateTime.Now.AddDays(-configuracaoMonitoramento.DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente));
            string observacaoFinalizacao = $"Finalizado automaticamente pelo fluxo de Finalizaçao por Período ({configuracaoMonitoramento.DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente} dias)";
            serAlertaMonitor.FinalizarAlertas(alertasAbertoPeriodo, DateTime.Now, observacaoFinalizacao, unitOfWork, _auditado?.Usuario, false, null, null, true);
        }

        private void BuscarECorrigirCargasSemControleEntrega(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                // TODO: ToList não tem AddRange
                List<int> cargasSemControleEntrega = repCarga.BuscarCodigosCargasSemControleEntrega(50).ToList();

                List<int> cargasPedidoSemEntrega = repCarga.BuscarCodigosCargasPedidoSemControleEntrega(50).ToList();

                cargasSemControleEntrega.AddRange(cargasPedidoSemEntrega);

                if (cargasSemControleEntrega.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> allCargas = repCarga.BuscarPorCodigos(cargasSemControleEntrega);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> allCargasRotaFrete = repCargaRotaFrete.BuscarPorCargas(cargasSemControleEntrega);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> allCargasPedidos = repCargaPedido.BuscarPorCargas(cargasSemControleEntrega);
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> allCargaPedidosXMLs = repPedidoXMLNotaFiscal.BuscarPorCargas(cargasSemControleEntrega);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> allPontosPassagens = repCargaRotaFretePontosPassagem.BuscarPorCargas(cargasSemControleEntrega);

                    foreach (int codigoCarga in cargasSemControleEntrega)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = allCargas.Where(x => x.Codigo == codigoCarga).FirstOrDefault();

                        try
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = allCargasRotaFrete.Where(x => x.Carga.Codigo == codigoCarga).FirstOrDefault();
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = allCargasPedidos.Where(x => x.Carga.Codigo == codigoCarga).ToList();
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs = allCargaPedidosXMLs.Where(x => x.CargaPedido.Carga.Codigo == codigoCarga).ToList();
                            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagens = allPontosPassagens.Where(x => x.CargaRotaFrete.Carga.Codigo == codigoCarga).ToList();

                            unitOfWork.Start();

                            if (pontosPassagens.Exists(pontoPassagem => pontoPassagem.Cliente == null && pontoPassagem.ClienteOutroEndereco == null && pontoPassagem.Localidade == null)
                               || cargaRotaFrete == null
                               || pontosPassagens == null
                               || !pontosPassagens.Exists(pontoPassagem => pontoPassagem.TipoPontoPassagem == TipoPontoPassagem.Entrega))
                            {
                                Servicos.Log.TratarErro($"Iniciando GerarIntegracoesRoteirizacaoCarga {carga.Codigo} {carga.CodigoCargaEmbarcador}", "ControleQualidadeTorre");
                                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracao, this._tipoServicoMultisoftware, true);
                                Servicos.Log.TratarErro($"Finalizado GerarIntegracoesRoteirizacaoCarga {carga.Codigo} {carga.CodigoCargaEmbarcador}", "ControleQualidadeTorre");
                            }
                            else
                            {
                                Servicos.Log.TratarErro($"Iniciando GerarEntregasControleQualidade {carga.Codigo} {carga.CodigoCargaEmbarcador}", "ControleQualidadeTorre");
                                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarEntregasControleQualidade(carga, cargaPedidos, cargaPedidosXMLs, cargaRotaFrete, pontosPassagens, true, configuracao, unitOfWork, this._tipoServicoMultisoftware);
                                Servicos.Log.TratarErro($"Finalizado GerarEntregasControleQualidade {carga.Codigo} {carga.CodigoCargaEmbarcador}", "ControleQualidadeTorre");
                            }

                            unitOfWork.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();

                            carga = repCarga.BuscarPorCodigo(codigoCarga);
                            carga.NumeroTentativaGeracaoEntregasControleQualidade += 1;
                            repCarga.Atualizar(carga);

                            Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
            }
        }

        private void BuscarECorrigirCargasSemIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

                if (configuracaoIntegracao == null)
                    return;

                if (configuracaoIntegracao.BuscarCargasSemIntegracoes && (configuracaoIntegracao.PossuiIntegracaoTrizy || configuracaoIntegracao.PossuiIntegracaoBrasilRisk))
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesValidar = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk };

                    IList<int> cargasFinalizadasSemIntegracao = repCarga.BuscarCodigosCargasSemIntegracao(50, integracoesValidar);

                    if (cargasFinalizadasSemIntegracao.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> allCargas = repCarga.BuscarPorCodigos(cargasFinalizadasSemIntegracao);

                        foreach (int codigoCarga in cargasFinalizadasSemIntegracao)
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = allCargas.Where(x => x.Codigo == codigoCarga).FirstOrDefault();

                            try
                            {
                                carga.SituacaoCarga = SituacaoCarga.AgIntegracao;
                                carga.GerandoIntegracoes = true;
                                repCarga.Atualizar(carga);

                                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, null, "Voltou carga para Reenviar Integrações (Auto correção)", unitOfWork);

                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
            }
        }

        public void BuscarECorrigirCargasEntregasSemProdutos(UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade servControleEntregaQualidade = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade(unitOfWork, null);

                IList<int> cargasComEntregasSemProdutos = repCarga.BuscarCodigosCargasEntregasSemProdutos(50);

                if (cargasComEntregasSemProdutos.Count > 0)
                {
                    foreach (int codigoCarga in cargasComEntregasSemProdutos)
                    {
                        servControleEntregaQualidade.AjustarItensNotasFiscaisControleEntrega(codigoCarga, _auditado);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
            }
        }

        private void BuscarECorrigirCargasEntregasSemNotasFiscais(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNota = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade servControleEntregaQualidade = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade(unitOfWork, null);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                IList<int> cargasComEntregasSemNotas = repCarga.BuscarCodigosCargasEntregasSemNotas(50, configuracaoControleEntrega);

                if (cargasComEntregasSemNotas.Count > 0)
                {
                    foreach (int codigoCarga in cargasComEntregasSemNotas)
                    {
                        servControleEntregaQualidade.AjustarNotasFiscaisControleEntrega(codigoCarga, _auditado);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
            }
        }

        private void FinalizarMonitoramentosAutomaticamente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTms = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTms.BuscarConfiguracaoPadrao();

            if ((configuracaoMonitoramento?.FinalizarAutomaticamenteMonitoramentosEmAndamento ?? false) && configuracaoMonitoramento.DiasFinalizarAutomaticamenteMonitoramentoEmAndamento > 0)
            {
                string msg = $"por data criação + {configuracaoMonitoramento.DiasFinalizarAutomaticamenteMonitoramentoEmAndamento} dias ";
                IList<int> codigosMonitoramentosFinalizar = repMonitoramento.BuscarCodigosMonitoramentosFinalizarDataInicio(configuracaoMonitoramento, 50);

                if (codigosMonitoramentosFinalizar.Count > 0)
                {
                    foreach (int codigoMonitoramento in codigosMonitoramentosFinalizar)
                    {
                        try
                        {
                            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);

                            Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(monitoramento, DateTime.Now, configuracaoTMS, _auditado, $"Monitoramento finalizado automaticamente {msg}", unitOfWork, MotivoFinalizacaoMonitoramento.StatusViagemConcluida);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
                        }
                    }
                }
            }

            if ((configuracaoMonitoramento?.FinalizarAutomaticamenteMonitoramentosPrevisaoUltimaEntrega ?? false) && configuracaoMonitoramento.DiasFinalizarMonitoramentoPrevisaoUltimaEntrega > 0)
            {
                string msg = $"por data previsão entrega + {configuracaoMonitoramento.DiasFinalizarMonitoramentoPrevisaoUltimaEntrega} dias";
                List<int> codigosMonitoramentosFinalizar = repMonitoramento.BuscarCodigosMonitoramentosFinalizarDataPrevisao(configuracaoMonitoramento, 50);

                if (codigosMonitoramentosFinalizar.Count > 0)
                {
                    foreach (int codigoMonitoramento in codigosMonitoramentosFinalizar)
                    {
                        try
                        {
                            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);

                            Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(monitoramento, DateTime.Now, configuracaoTMS, _auditado, $"Monitoramento finalizado automaticamente {msg}", unitOfWork, MotivoFinalizacaoMonitoramento.StatusViagemConcluida);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "ControleQualidadeTorre");
                        }
                    }
                }
            }

        }

        private void ProcessarAlertaMonitoramentoPosicoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

            if (!configuracaoTMS.PossuiMonitoramento || configuracaoTMS.NumeroPosicoesPendentesAlerta <= 0)
                return;

            //posicoes pendentes monitoramento
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao> ListaSituacoesPendentes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao>();
            ListaSituacoesPendentes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Pendente);
            ListaSituacoesPendentes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processando);

            int numeroPosicoesPendentesAlerta = repPosicao.BuscarContarProcessar(ListaSituacoesPendentes);

            if (numeroPosicoesPendentesAlerta >= configuracaoTMS.NumeroPosicoesPendentesAlerta)
            {
                string tituloEmail = "Alerta Monitoramento: Número Posições Pendetes em (" + _clienteMultisoftware.RazaoSocial + ")";

                string body = $"<h1>Atenção!</h1>";
                body += "<p>";
                body += $"Cliente: {_clienteMultisoftware.RazaoSocial}<br/>";
                body += $"Data atual: {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}<br/><br/>";
                body += $"<b>Existem posições aguardando o processamento superior ao maximo configurado de : {configuracaoTMS.NumeroPosicoesPendentesAlerta} posições.<br/><br/>";
                body += $"Posições Pendentes neste momento: {numeroPosicoesPendentesAlerta}<br/>";
                body += "</p>";

                string emails = string.Empty;
                if (!string.IsNullOrWhiteSpace(configuracaoMonitoramento.EmailsAlertaMonitoramento))
                    emails += configuracaoMonitoramento.EmailsAlertaMonitoramento;
                emails += "fernando.morh@multisoftware.com.br;rodolfo.trevisol@multisoftware.com.br;willian.dedordi@multisoftware.com.br;";

                Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, emails, "", "", tituloEmail, body, string.Empty, null, "", true, string.Empty, 0, unitOfWork, 0, false, null, false);

                Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic posicoesParadas = new Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic();
                posicoesParadas.ValorAlerta = numeroPosicoesPendentesAlerta;
                posicoesParadas.Cliente = _clienteMultisoftware.RazaoSocial;
                posicoesParadas.TipoServico = _clienteMultisoftware.ClienteConfiguracao.TipoServicoMultisoftware;
                posicoesParadas.DataAtual = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                posicoesParadas.DescricaoAlerta = "Existem posições aguardando o processamento superior ao maximo configurado";
                posicoesParadas.CodigoTipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogElastic.FilasPosicoesProcessar;

                Servicos.Log.TratarErro(posicoesParadas, TipoLogSistema.Info);
            }

        }


        #endregion
    }
}
