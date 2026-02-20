using System;
using System.Collections.Generic;
using System.Linq;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoInicioDeViagem : IMonitoramento
    {
        private Repositorio.Embarcador.Cargas.Carga repCarga;
        private List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaVeiulo> cargas;
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem;
        private Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete;
        private void AtualizarParametroPosicao(Int64 codigoPosicao)
        {
            parametro.Posicao = codigoPosicao;
            repMonitoramentoEvento.Atualizar(parametro);
        }
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioDeViagem);
        }
        private void Inicializar()
        {
            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
            posicoes = repPosicao.BuscarPorCodigoMaior(parametro.Posicao);

            repCarga = new Repositorio.Embarcador.Cargas.Carga(_unidadeDeTrabalho);
            cargas = repCarga.BuscarCargasMonitoramento();

            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);

            repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unidadeDeTrabalho);
            repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unidadeDeTrabalho);
        }
        private Dominio.Entidades.Embarcador.Logistica.Monitoramento InserirMonitoramento(int codigoCarga)
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Gerando monitoramento", this.GetType().Name);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            
            return Servicos.Embarcador.Monitoramento.Monitoramento.GerarEntradaUnidade(carga, _unidadeDeTrabalho);
        }
        private void FinalizarViagensVeiculo(List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas)
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Finalizando viagens não finalizadas", this.GetType().Name);
            foreach (var cargaMonitorada in cargasMonitoradas)
            {
                DateTime dataFim = DateTime.Now;
                Servicos.Embarcador.GestaoEntregas.GestaoEntregas.SetarProximaEtapa(cargaMonitorada.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimViagem,
                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, _unidadeDeTrabalho, dataFim);
                Servicos.Embarcador.Monitoramento.Monitoramento.FimViagem(cargaMonitorada, dataFim, _unidadeDeTrabalho);
            }
        }
        private Dominio.Entidades.Cliente ObterRemetente(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            
            var cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(codigoCarga);
            

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete?.Codigo ?? 0);

            return pontosPassagem?.OrderBy(o => o.Ordem)?.FirstOrDefault()?.Cliente;
        }
        private void IniciarMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, DateTime dataInicio)
        {
            if ((cargaMonitorada != null) && (cargaMonitorada.DataInicio == null) && (cargaMonitorada.ParadaProgramada) && (cargaMonitorada.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando))
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Iniciando viagem para carga {cargaMonitorada?.Carga?.Codigo}", this.GetType().Name);
                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasComMonitoramentoEmAberto = repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculo(cargaMonitorada);

                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unidadeDeTrabalho);
                _unidadeDeTrabalho.Start();
                try
                {
                    FinalizarViagensVeiculo(cargasComMonitoramentoEmAberto);

                    Servicos.Embarcador.GestaoEntregas.GestaoEntregas.SetarProximaEtapa(cargaMonitorada.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, _unidadeDeTrabalho, dataInicio);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(cargaMonitorada.Carga.Codigo, dataInicio, _unidadeDeTrabalho);
                    Servicos.Embarcador.Monitoramento.Monitoramento.IniciarViagem(cargaMonitorada, dataInicio, _unidadeDeTrabalho);

                    _unidadeDeTrabalho.CommitChanges();
                }
                catch (Exception ex)
                {
                    _unidadeDeTrabalho.Rollback();
                    Servicos.Log.TratarErro("Erro ao gravar alterta de inicio de viagem: " + ex);
                    throw;
                }
            }
        }
        private void Processar()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Processando cargas", this.GetType().Name);
            if (posicoes.Count == 0)
                return;

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaVeiulo carga in cargas)
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Processando carga {carga?.CodigoCarga}", this.GetType().Name);
                var cargaMonitorada = repMonitoramento.BuscarPorCarga(carga.CodigoCarga);

                var StatusValido = (cargaMonitorada == null) || (cargaMonitorada?.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando);

                if (!StatusValido)
                    continue;

                var remetente = ObterRemetente(carga.CodigoCarga, _unidadeDeTrabalho);

                if (remetente == null)
                    continue;

                
                var remetenteLatitude = double.Parse(remetente.Latitude, System.Globalization.CultureInfo.InvariantCulture);
                var remetenteLongitude = double.Parse(remetente.Longitude, System.Globalization.CultureInfo.InvariantCulture);

                var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == carga.CodigoVeiculo).OrderBy(o=>o.DataVeiculo);
                
                //Percorre todas as posições do veículo
                foreach (var posicaoVeiculo in posicoesVeiculo)
                {
                    if (cargaMonitorada?.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado)
                        break;

                    double distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, remetenteLatitude, remetenteLongitude);

                    var raio = remetente.RaioEmMetros != null && remetente.RaioEmMetros > 0 ? remetente.RaioEmMetros : parametro.Raio;

                    var noRaio = distancia < raio;

                    if (noRaio)
                    {
                        if (cargaMonitorada == null)
                            cargaMonitorada = InserirMonitoramento(carga.CodigoCarga);
                    }

                    if (!noRaio)
                        IniciarMonitoramento(cargaMonitorada, posicaoVeiculo.DataVeiculo);
                }
            }

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