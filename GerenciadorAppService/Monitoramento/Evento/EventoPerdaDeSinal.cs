using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoPerdaDeSinal : IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlerta;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PerdaDeSinal;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private void InserirAlerta(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            var novoAlerta = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitor
            {
                DataCadastro = DateTime.Now,
                Data = posicao.DataVeiculo,
                TipoAlerta = tipoAlerta,
                Veiculo = posicao.Veiculo,
                Posicao = posicao,
                Carga = carga
            };

            _unidadeDeTrabalho.Start();
            try
            {
                repAlerta.Inserir(novoAlerta);

                MonitoramentoUtils.AtualizarUltimoAlerta(ultimoAlerta, novoAlerta);
                _unidadeDeTrabalho.CommitChanges();
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Inserindo alerta {novoAlerta.Codigo} ", this.GetType().Name);
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar alterta de sem sinal: " + ex);
                throw;
            }
        }
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.PerdaDeSinal);
        }
        private void AtualizarParametroPosicao(Int64 codigoPosicao)
        {
            parametro.Posicao = codigoPosicao;
            repMonitoramentoEvento.Atualizar(parametro);
        }
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.PerdaDeSinal);

            repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unidadeDeTrabalho);
            ultimoAlerta = repAlerta.BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PerdaDeSinal);

            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);

            if (parametro.Posicao == 0)
                AtualizarParametroPosicao(repPosicao.BuscarUltimaPosicao()?.Codigo ?? 0);

            posicoes = repPosicao.BuscarPorCodigoMaior(parametro.Posicao);
        }
        private void Processar()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Processando cargas", this.GetType().Name);

            if (posicoes.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Processando carga {cargaMonitorada?.Codigo}", this.GetType().Name);
                if (cargaMonitorada?.Carga?.Veiculo == null)
                    continue;

                //Busca somente posições do veículo
                var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo.Codigo && pos?.DataVeiculo > cargaMonitorada?.DataInicio).OrderBy(o => o.DataVeiculo);

                
                Dominio.Entidades.Embarcador.Logistica.Posicao posicaoAnterior = repPosicao.BuscarPosicaoAnteriorVeiculo(posicoesVeiculo?.FirstOrDefault()?.Codigo ?? 0, cargaMonitorada.Carga.Veiculo.Codigo);
                DateTime dataAnterior = posicaoAnterior?.DataVeiculo > cargaMonitorada?.Carga?.DataCriacaoCarga ? posicaoAnterior.DataVeiculo  : cargaMonitorada?.Carga?.DataCriacaoCarga ?? DateTime.MinValue;

                //Percorre todas as posições do veículo
                foreach (var posicaoVeiculo in posicoesVeiculo)
                {
                    var ultimoAlertaVeiculo = (from ale in ultimoAlerta
                                               where ale.Veiculo == cargaMonitorada.Carga.Veiculo.Codigo && ale.Data > cargaMonitorada.DataInicio
                                               orderby ale.Data descending
                                               select ale).FirstOrDefault();


                    DateTime data = ultimoAlertaVeiculo != null ? ultimoAlertaVeiculo.Data : dataAnterior;

                    data = data.AddMinutes(parametro.Tempo);

                    var tempoMinimo = posicaoVeiculo.DataVeiculo > data;

                    dataAnterior = posicaoVeiculo.DataVeiculo;

                    if (tempoMinimo)
                        InserirAlerta(posicaoVeiculo, cargaMonitorada.Carga);
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