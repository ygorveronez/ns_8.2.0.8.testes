using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoFimDeViagem :IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametroInicioViagem;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.FimDeViagem);
            parametroInicioViagem = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioEntrega);
        }
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);

            var posicao = parametro.Posicao < parametroInicioViagem.Posicao || parametroInicioViagem.Posicao == 0 ? parametro.Posicao : parametroInicioViagem.Posicao;

            posicoes = repPosicao.BuscarPorCodigoMaior(posicao);

            repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unidadeDeTrabalho);
            
            repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unidadeDeTrabalho);
        }
        private void AtualizarParametroPosicao(Int64 codigoPosicao)
        {
            parametro.Posicao = codigoPosicao;
            repMonitoramentoEvento.Atualizar(parametro);
        }
        private void FinalizarViagem(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, DateTime? dataFim)
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Finalizando viagem da carga {cargaMonitorada?.Carga?.Codigo}", this.GetType().Name);
            var posicaoAtual = repPosicaoAtual.BuscarPorVeiculo(cargaMonitorada.Carga?.Veiculo.Codigo ?? 0);

            _unidadeDeTrabalho.Start();
            try
            {
                if (dataFim == null)
                    dataFim = DateTime.Now;

                if (posicaoAtual != null)
                {
                    posicaoAtual.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.SemViagem;
                    repPosicaoAtual.Atualizar(posicaoAtual);
                }

                Servicos.Embarcador.GestaoEntregas.GestaoEntregas.SetarProximaEtapa(cargaMonitorada.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimViagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, _unidadeDeTrabalho, dataFim);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(cargaMonitorada.Carga.Codigo, dataFim ?? DateTime.Now, _unidadeDeTrabalho);
                Servicos.Embarcador.Monitoramento.Monitoramento.FimViagem(cargaMonitorada,null, _unidadeDeTrabalho);
                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar alterta de fim de viagem: " + ex);
                throw;
            }

        }
        private void Processar()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Processando cargas", this.GetType().Name);
            foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Processando carga {cargaMonitorada?.Carga?.Codigo}", this.GetType().Name);
                if (cargaMonitorada?.Carga?.Veiculo == null)
                    continue;

                var pedidosEmAberto = cargaMonitorada?.Carga?.Pedidos.Where(ped => ped.DataSaida == null).ToList();

                if (pedidosEmAberto.Count == 0) {

                    var ultimoPedido = cargaMonitorada?.Carga?.Pedidos.Where(ped => ped.DataSaida != null).OrderByDescending(o => o.DataSaida).FirstOrDefault();

                    FinalizarViagem(cargaMonitorada, ultimoPedido?.DataSaida);
                }
            }
        }
        public void Iniciar(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Inicio", this.GetType().Name);
            _unidadeDeTrabalho = unidadeDeTrabalho;

            InicializarParametro();

            if (parametro?.MonitoramentoEvento?.Ativo == true)
            {
                Inicializar();

                Processar();
            }
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Fim\r\n", this.GetType().Name);
        }
    }
}