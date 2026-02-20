using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 180000)]

    public class ControleTendenciaEntrega : LongRunningProcessBase<ControleTendenciaEntrega>
    {
        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

        }

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarEntregasEmRegimeAtraso(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            VerificarCargasParaInicioViagemAutomatico(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            GerarOcorrenciaCargaPedidoEntregaPorOcorrenciasAtrasadas(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
        }

        private void VerificarEntregasEmRegimeAtraso(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repAcompanhamentoEntregaConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendendicas = new Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao();
                configuracaoTempoTendendicas = repAcompanhamentoEntregaConfiguracao.BuscarConfiguracao();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                bool possuiMonitoramento = configuracaoEmbarcador?.PossuiMonitoramento ?? false;

                Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga repConfiguracaoAlertaCarga = new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga ConfigAlertaAtrasoDescarga = repConfiguracaoAlertaCarga.BuscarAtivo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.AtrasoColetaDescarga);


                if (configuracaoTempoTendendicas != null && possuiMonitoramento)
                {
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarTendenciasEntrega(configuracaoTempoTendendicas, ConfigAlertaAtrasoDescarga, unitOfWork);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private void VerificarCargasParaInicioViagemAutomatico(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

            if (configuracaoControleEntrega?.TempoInicioViagemAposEmissaoDoc > 0 || configuracaoControleEntrega?.TempoInicioViagemAposFinalizacaoFluxoPatio > 0)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (configuracaoControleEntrega.TempoInicioViagemAposEmissaoDoc > 0)
                {
                    DateTime dataComparacao = DateTime.Now.AddMinutes(-configuracaoControleEntrega.TempoInicioViagemAposEmissaoDoc);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCodigosPentendesInicioViagemPorEmissao(10, dataComparacao);
                    for (int i = 0; i < cargas.Count; i++)
                    {

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas[i];

                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                        {
                            TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                            OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                        };

                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(carga.Codigo, carga.DataFinalizacaoEmissao.Value, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, configuracao, tipoServicoMultisoftware, _clienteMultisoftware, auditado, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Iniciou viagem automaticamente por tempo limite após emissão dos documentos", unitOfWork);
                    }
                }

                if (configuracaoControleEntrega.TempoInicioViagemAposFinalizacaoFluxoPatio > 0)
                {
                    DateTime dataComparacao = DateTime.Now.AddMinutes(-configuracaoControleEntrega.TempoInicioViagemAposFinalizacaoFluxoPatio);

                    List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxoGestaoPatios = repFluxoGestaoPatio.BuscarCodigosPentendesInicioViagemPorFimFluxoPatio(10, dataComparacao);

                    for (int i = 0; i < fluxoGestaoPatios.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio gestaoPatio = fluxoGestaoPatios[i];
                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                        {
                            TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                            OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                        };

                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(gestaoPatio.Carga.Codigo, gestaoPatio.DataFinalizacaoFluxo.Value, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, configuracao, tipoServicoMultisoftware, _clienteMultisoftware, auditado, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(auditado, gestaoPatio.Carga, "Iniciou viagem automaticamente por tempo limite após finalização do fluxo do pátio", unitOfWork);
                    }
                }
            }
        }

        private void GerarOcorrenciaCargaPedidoEntregaPorOcorrenciasAtrasadas(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            if (repTipoOperacao.ExisteGerarOcorrenciaPedidoEntregueForaPrazo())
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia servOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = repositorioCargaEntrega.BuscarCargaEntregaTipoDevolucaoEmAberto(2);

                if (listaCargaEntrega.Count == 0)
                    return;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaCargaEntrega)
                {
                    servOcorrencia.GerarOcorrenciaCargaPedidoEntregueForaPrazo(cargaEntrega, DateTime.Now, cargaEntrega.Carga.TipoOperacao?.TipoOcorrenciaPedidoEntregueForaPrazo, unitOfWork, tipoServicoMultisoftware, null, configuracaoEmbarcador);
                }
            }
        }
    }
}