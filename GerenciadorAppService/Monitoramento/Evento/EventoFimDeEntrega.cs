using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoFimDeEntrega : IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametroInicioEntrega;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.FimEntrega);
            parametroInicioEntrega = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioEntrega);
        }
        private void Inicializar()
        {
            var posicao = parametro.Posicao < parametroInicioEntrega.Posicao || parametroInicioEntrega.Posicao == 0? parametro.Posicao : parametroInicioEntrega.Posicao;

            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
            posicoes = repPosicao.BuscarPorCodigoMaior(posicao);

            repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unidadeDeTrabalho);
        }
        private void ConfirmarPedido(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido,DateTime DataSaida)
        {
            var CodigoPedido = CargaPedido.Pedido?.Codigo ?? 0;

            if (CargaPedido?.DataSaida != null || CodigoPedido == 0)
                return;

            Repositorio.Embarcador.GestaoEntregas.EntregaPedido repEntregaPedido = new Repositorio.Embarcador.GestaoEntregas.EntregaPedido(_unidadeDeTrabalho);
            Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega repFluxoGestaoEntrega = new Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega(_unidadeDeTrabalho);


            Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido entregaPedido = repEntregaPedido.BuscarPorPedido(CodigoPedido);

            // Valida
            if ((entregaPedido == null) || (entregaPedido.Etapa.FluxoGestaoEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoEntrega.Aguardando))
                return;


            Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega fluxoGestaoEntrega = repFluxoGestaoEntrega.BuscarPorEntregaPedido(entregaPedido.Codigo);

            _unidadeDeTrabalho.Start();
            try
            {
                entregaPedido.Initialize();

                entregaPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaPedido.Entregue;
                entregaPedido.DataEntrega = DataSaida;
                Servicos.Embarcador.GestaoEntregas.EmailEntrega.EnviarEmailPedidoEntregue(entregaPedido, _unidadeDeTrabalho);

                CargaPedido.DataSaida = DataSaida;
                repCargaPedido.Atualizar(CargaPedido);

                Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarEntrega(cargaMonitorada, _unidadeDeTrabalho);

                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar fim de entrega " + ex);
                throw;
            }
        }
        private void ConfirmarCargaEntrega(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime DataSaida)
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Confirmando entrega da carga entrega {cargaEntrega?.Codigo}", this.GetType().Name);
            if (cargaEntrega?.DataEntrega != null) 
                return;
            
            _unidadeDeTrabalho.Start();
            try
            {
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntrega, DataSaida, _unidadeDeTrabalho);
                Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarEntrega(cargaMonitorada, _unidadeDeTrabalho);

                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar fim de entrega " + ex);
                throw;
            }
        }
        private void ProcessarCargaPedido()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Processando carga pedido", this.GetType().Name);
            foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Processando carga {cargaMonitorada?.Carga?.Codigo}", this.GetType().Name);

                if (cargaMonitorada?.Carga?.Veiculo == null)
                    continue;

                //Busca somente posições do veículo
                var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo.Codigo && pos?.DataVeiculo > cargaMonitorada?.DataInicio).OrderBy(o => o.DataVeiculo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargapedidos = repCargaPedido.BuscarPorCarga(cargaMonitorada?.Carga?.Codigo ?? 0);
                cargapedidos = cargapedidos.Where(ped => ped.DataChegada != null && ped.DataSaida == null).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in cargapedidos)
                {

                    if ((pedido.DataChegada == null) || (pedido.DataSaida != null) || (pedido.ClienteEntrega == null))
                        continue;

                    var lat = double.Parse(pedido.ClienteEntrega.Latitude, System.Globalization.CultureInfo.InvariantCulture);
                    var lng = double.Parse(pedido.ClienteEntrega.Longitude, System.Globalization.CultureInfo.InvariantCulture);

                    //Percorre todas as posições do veículo
                    foreach (var posicaoVeiculo in posicoesVeiculo)
                    {
                        if (posicaoVeiculo.DataVeiculo >= pedido.DataChegada)
                        {
                            double distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, lat, lng);

                            var saidaRaio = distancia > parametro.Raio;
                            if (saidaRaio)
                            {
                                ConfirmarPedido(cargaMonitorada, pedido, posicaoVeiculo.Data);
                                break;
                            }
                        }
                    }
                }

            }
        }
        private void ProcessarCargaEntrega()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Processando carga entrega", this.GetType().Name);
            foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Processando carga {cargaMonitorada?.Carga?.Codigo}", this.GetType().Name);
                if (cargaMonitorada?.Carga?.Veiculo == null)
                    continue;

                //Busca somente posições do veículo
                var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo.Codigo && pos?.DataVeiculo > cargaMonitorada?.DataInicio).OrderBy(o => o.DataVeiculo);

                var cargaEntregas = cargaMonitorada?.Carga?.Entregas.Where(ent => ent.DataInicioEntrega != null && ent.DataEntrega == null);

                foreach (var cargaEntrega in cargaEntregas)
                {

                    if ((cargaEntrega.DataInicioEntrega == null) || (cargaEntrega.DataEntrega != null) || (cargaEntrega.Cliente == null))
                        continue;

                    var lat = double.Parse(cargaEntrega.Cliente.Latitude.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                    var lng = double.Parse(cargaEntrega.Cliente.Longitude.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);

                    //Percorre todas as posições do veículo
                    foreach (var posicaoVeiculo in posicoesVeiculo)
                    {
                        if (posicaoVeiculo.DataVeiculo >= cargaEntrega.DataInicioEntrega)
                        {
                            double distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, lat, lng);

                            var saidaRaio = distancia > parametro.Raio;
                            if (saidaRaio)
                            {
                                ConfirmarCargaEntrega(cargaMonitorada, cargaEntrega, posicaoVeiculo.Data);
                                break;
                            }
                        }
                    }
                }

            }
        }
        private void AtualizarParametroPosicao(Int64 codigoPosicao)
        {
            parametro.Posicao = codigoPosicao;
            repMonitoramentoEvento.Atualizar(parametro);
        }
        private void Processar()
        {
            if (posicoes.Count == 0)
                return;

            //Antigo remover
            ProcessarCargaPedido();

            ProcessarCargaEntrega();


            AtualizarParametroPosicao(posicoes.LastOrDefault().Codigo);
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