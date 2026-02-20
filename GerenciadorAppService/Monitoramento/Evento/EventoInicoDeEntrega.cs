using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoInicioDeEntrega : IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametroInicioDeViagem;
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
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioEntrega);
            parametroInicioDeViagem = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioDeViagem);
        }
        private void Inicializar()
        {
            var posicao = parametro.Posicao < parametroInicioDeViagem.Posicao || parametroInicioDeViagem.Posicao == 0 ? parametro.Posicao : parametroInicioDeViagem.Posicao;
            
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();
            

            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
            posicoes = repPosicao.BuscarPorCodigoMaior(posicao);

            repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unidadeDeTrabalho);
    }
        private void AtualizarParametroPosicao(Int64 codigoPosicao)
        {
            parametro.Posicao = codigoPosicao;
            repMonitoramentoEvento.Atualizar(parametro);
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
                cargapedidos = cargapedidos.Where(ped => ped.DataChegada == null).ToList();


                foreach (var cargapedido in cargapedidos)
                {

                    if (cargapedido.ClienteEntrega == null || cargapedido.DataChegada != null)
                        continue;

                    var lat = double.Parse(cargapedido.ClienteEntrega.Latitude.Replace(',','.'), System.Globalization.CultureInfo.InvariantCulture);
                    var lng = double.Parse(cargapedido.ClienteEntrega.Longitude.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);

                    var raio = cargapedido.ClienteEntrega.RaioEmMetros != null && cargapedido.ClienteEntrega.RaioEmMetros > 0 ? cargapedido.ClienteEntrega.RaioEmMetros : parametro.Raio;

                    //Percorre todas as posições do veículo
                    foreach (var posicaoVeiculo in posicoesVeiculo)
                    {
                        double distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, lat, lng);

                        var noRaio = distancia < raio;
                        if (noRaio)
                        {
                            IniciarEntrega(cargaMonitorada, cargapedido, posicaoVeiculo.Data);
                            break;
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

                var cargaEntregas= cargaMonitorada?.Carga?.Entregas?.Where(ent => ent.DataInicioEntrega == null).ToList();

                foreach (var cargaEntrega in cargaEntregas)
                {

                    if (cargaEntrega.Cliente == null || cargaEntrega.DataInicioEntrega != null)
                        continue;

                    var lat = double.Parse(cargaEntrega.Cliente.Latitude.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                    var lng = double.Parse(cargaEntrega.Cliente.Longitude.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);

                    var raio = cargaEntrega.Cliente.RaioEmMetros ?? parametro.Raio;

                    //Percorre todas as posições do veículo
                    foreach (var posicaoVeiculo in posicoesVeiculo)
                    {
                        double distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, lat, lng);

                        var noRaio = distancia < raio;
                        if (noRaio)
                        {
                            IniciarCargaEntrega(cargaMonitorada, cargaEntrega, posicaoVeiculo.Data);
                            break;
                        }
                    }
                }
            }
        }
        private void IniciarEntrega(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, DateTime data)
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Iniciando entrega da carga pedido {cargaPedido?.Codigo}", this.GetType().Name);
            if (cargaPedido.DataChegada != null)
                return;

            _unidadeDeTrabalho.Start();
            try
            {
                Servicos.Embarcador.Monitoramento.Monitoramento.IniciarEntrega(cargaMonitorada, cargaPedido.ClienteEntrega, _unidadeDeTrabalho);

                cargaPedido.DataChegada = data;
                repCargaPedido.Atualizar(cargaPedido);


                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar alterta de inicio de viagem: " + ex);
                throw;
            }
        }
        private void IniciarCargaEntrega(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime data)
        {
            if (cargaEntrega.DataInicioEntrega != null)
                return;

            _unidadeDeTrabalho.Start();
            try
            {
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarEntrega(cargaEntrega, data , _unidadeDeTrabalho);
                Servicos.Embarcador.Monitoramento.Monitoramento.IniciarEntrega(cargaMonitorada, cargaEntrega.Cliente, _unidadeDeTrabalho);

                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar alterta de inicio de entrega: " + ex);
                throw;
            }
        }
        private void Processar()
        {
            if (posicoes.Count == 0)
                return;

            //Modelo antigo (desabilitar)
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