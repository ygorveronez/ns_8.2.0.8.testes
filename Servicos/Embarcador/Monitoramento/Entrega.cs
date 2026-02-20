using System;

namespace Servicos.Embarcador.Monitoramento
{
    public class Entrega
    {
        #region Métodos públicos estáticos 

        public static void IniciarEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, int codigoMonitoramento, DateTime dataInicio, double latitude, double longitude, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);

            IniciarEntrega(cargaEntrega, monitoramento, dataInicio, latitude, longitude, configuracao, tipoServicoMultisoftware, clienteMultisoftware, configuracaoControleEntrega, unitOfWork);
        }

        public static void IniciarEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime dataInicio, double latitude, double longitude, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntrega != null && monitoramento != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = null;
                if (latitude != 0 && longitude != 0)
                {
                    wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint() { Latitude = latitude, Longitude = longitude };
                }

                if (cargaEntrega.Ordem == 0 && cargaEntrega.Coleta && configuracao.QuandoIniciarViagemViaMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarViagemViaMonitoramento.AoChegarNaOrigem)
                {
                    Carga.IniciarViagem(monitoramento, wayPoint, dataInicio, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
                }

                if (cargaEntrega.DataInicio == null || !cargaEntrega.DataEntradaRaio.HasValue || (cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado && cargaEntrega.PermitirEntregarMaisTarde))
                {
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarEntrega(cargaEntrega, wayPoint, dataInicio, configuracao, tipoServicoMultisoftware, clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.MonitoramentoAutomaticamente, configuracaoControleEntrega, unitOfWork);
                    Auditoria.AuditarMonitoramento(monitoramento, "Entrega " + cargaEntrega.Ordem + ", " + cargaEntrega.Cliente?.CPF_CNPJ_Formatado + ", iniciada via monitoramento", unitOfWork);
                }
            }
        }

        /// <summary>
        /// Finaliza a entrega quando o tracking detecta que o motorista saiu do alvo da entrega.
        /// </summary>
        public static void Finalizar(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime data, double latitude, double longitude, double codigoCliente, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega)
        {
            if (cargaEntrega.Carga.TipoOperacao != null &&
                cargaEntrega.Carga.TipoOperacao.ConfiguracaoControleEntrega != null &&
                ((cargaEntrega.Coleta && cargaEntrega.Carga.TipoOperacao.ConfiguracaoControleEntrega.NaoFinalizarColetasPorTrackingMonitoramento) ||
                (!cargaEntrega.Coleta && cargaEntrega.Carga.TipoOperacao.ConfiguracaoControleEntrega.NaoFinalizarEntregasPorTrackingMonitoramento)))
            {
                return;
            }

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Cliente PossivelclienteAreaRedex = null;
            if (codigoCliente > 0)
                PossivelclienteAreaRedex = repCliente.BuscarPorCPFCNPJ(codigoCliente);

            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = null;
            if (latitude != 0 && longitude != 0)
                wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint() { Latitude = latitude, Longitude = longitude };

            var parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
            {
                cargaEntrega = cargaEntrega,
                dataInicioEntrega = data,
                dataTerminoEntrega = data,
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
                ClienteAreaRedex = PossivelclienteAreaRedex != null && PossivelclienteAreaRedex.AreaRedex ? codigoCliente : 0,
                OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.MonitoramentoAutomaticamente,
                configuracaoControleEntrega = configuracaoControleEntrega,
                tipoOperacaoParametro = cargaEntrega.Carga.TipoOperacao,
                TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
            };

            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametros, unitOfWork);

            monitoramento.Initialize();
            Auditoria.AuditarMonitoramento(monitoramento, "Entrega " + cargaEntrega.Ordem + ", " + cargaEntrega.Cliente?.CPF_CNPJ_Formatado + ", finalizada via monitoramento", unitOfWork);
        }

        #endregion

    }

}
