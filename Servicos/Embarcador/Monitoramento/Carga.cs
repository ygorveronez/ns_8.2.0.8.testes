using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento
{
    public class Carga
    {

        #region Métodos públicos estáticos 

        public static void IniciarViagem(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataInicio, double latitude, double longitude, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramento != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint() { Latitude = latitude, Longitude = longitude };
                IniciarViagem(monitoramento, wayPoint, dataInicio, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
            }
        }

        public static void IniciarViagem(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, DateTime dataInicio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (!monitoramento.Carga.DataInicioViagem.HasValue)
            {
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = TipoAuditado.Sistema,
                    OrigemAuditado = OrigemAuditado.Sistema
                };

                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(monitoramento.Carga.Codigo, dataInicio, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, wayPoint, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, auditado, unitOfWork))
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    monitoramento.Carga.InicioDeViagemNoRaio = true;
                    repCarga.Atualizar(monitoramento.Carga);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, monitoramento.Carga, $"Início de viagem informado automaticamente", unitOfWork);
                }

                monitoramento.Initialize();
                monitoramento.PolilinhaAteOrigem = string.Empty;
                monitoramento.DistanciaAteOrigem = 0;
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                repMonitoramento.Atualizar(monitoramento);

                Auditoria.AuditarMonitoramento(monitoramento, "Viagem iniciada via monitoramento", unitOfWork);
            }
            // ... se já já existe data de início de viagem mas recebeu uma posição fora de ordem que aconteceu antes do registrado anteriormente
            else if (configuracaoEmbarcador.MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem && dataInicio < monitoramento.Carga.DataInicioViagem)
            {
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarInicioViagem(monitoramento.Carga, dataInicio, unitOfWork);
            }
        }

        public static async Task IniciarViagemEPrimeiraColetaAsync(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, DateTime dataInicio, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.UnitOfWork unitOfWorkAsync = new(unitOfWork.StringConexao, TipoSessaoBancoDados.AtualizarAtual);
            IniciarViagemEPrimeiraColeta(monitoramento, wayPoint, dataInicio, cliente, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWorkAsync);
        }

        public static void IniciarViagemEPrimeiraColeta(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, DateTime dataInicio, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramento != null && monitoramento.Carga != null && !monitoramento.Carga.DataInicioViagem.HasValue && Monitoramento.DeveProcessarTrocaDeAlvo(configuracao, monitoramento.Carga.TipoOperacao))
            {
                // Início de viagem
                IniciarViagem(monitoramento, wayPoint, dataInicio, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);

                // Registro do primeiro destino se for uma coleta
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarEntregaOuColetaPorClienteECarga(monitoramento.Carga.Codigo, cliente.CPF_CNPJ);
                if (cargaEntrega != null && cargaEntrega.Ordem == 0 && cargaEntrega.Coleta && cargaEntrega.DataInicio == null)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);

                    var parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
                    {
                        cargaEntrega = cargaEntrega,
                        dataInicioEntrega = dataInicio,
                        dataTerminoEntrega = dataInicio,
                        dataSaidaRaio = null,
                        wayPoint = wayPoint,
                        wayPointDescarga = null,
                        pedidos = null,
                        motivoRetificacao = 0,
                        justificativaEntregaForaRaio = "",
                        motivoFalhaGTA = 0,
                        configuracaoEmbarcador = configuracao,
                        tipoServicoMultisoftware = tipoServicoMultisoftware,
                        sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.GerenciadorApp,
                        dadosRecebedor = null,
                        OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.MonitoramentoAutomaticamente,
                        configuracaoControleEntrega = configuracaoControleEntrega,
                        tipoOperacaoParametro = tipoOperacaoParametro,
                        TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
                    };

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametros, unitOfWork);
                    Auditoria.AuditarMonitoramento(monitoramento, "Coleta " + cargaEntrega.Ordem + ", " + cargaEntrega.Cliente?.CPF_CNPJ_Formatado + "finalizada via monitoramento ao iniciar viagem", unitOfWork);
                }
            }
        }

        public static void AtualizarCargaPeloMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramento.Carga == null) return;
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            monitoramento.Carga.DataAtualizacaoCarga = DateTime.Now;
            repositorioCarga.Atualizar(monitoramento.Carga);
        }

        public static void FinalizarViagem(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime data, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string mensagemAuditoria, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, OrigemSituacaoEntrega origemSituacaoEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(monitoramento.Carga.Codigo, data, auditado, mensagemAuditoria, tipoServicoMultisoftware, clienteMultisoftware, origemSituacaoEntrega, unitOfWork))
                Servicos.Auditoria.Auditoria.Auditar(auditado, monitoramento.Carga, $"fim de viagem informado automaticamente", unitOfWork);
        }

        public static void DesvincularCavaloNaCargaComStatusEmParqueamento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Repositorio.UnitOfWork unitOfWork)
        {

            if (monitoramento.Carga == null) return;

            if (monitoramento.StatusViagem == null) return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repoTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra TipoRegra = repMonitoramentoStatusViagem.BuscarPorCodigo(monitoramento.StatusViagem.Codigo, false).TipoRegra;

            bool exigePlacaTracao = repoTipoOperacao.BuscarPorCodigo(monitoramento.Carga.TipoOperacao.Codigo)?.ExigePlacaTracao ?? false;
            bool estaEmParqueamento = TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmParqueamento;

            if (exigePlacaTracao && estaEmParqueamento)
            {
                monitoramento.Carga.Veiculo = null;
                repositorioCarga.Atualizar(monitoramento.Carga);
            }

        }

        #endregion

    }

}
