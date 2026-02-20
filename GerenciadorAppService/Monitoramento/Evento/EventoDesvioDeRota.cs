using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoDesvioDeRota : IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlerta;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.DesvioDeRota;
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
                AtualizarParametroPosicao(posicao.Codigo);
                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar EventoDesvioDeRota: " + ex);
                throw;
            }
        }
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.DesvioDeRota);
        }
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unidadeDeTrabalho);
            ultimoAlerta = repAlerta.BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.DesvioDeRota);

            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
            posicoes = repPosicao.BuscarPorCodigoMaior(parametro.Posicao);
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

            
            foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
            {
                string polilinha = cargaMonitorada.PolilinhaPrevista;

                if ((!string.IsNullOrWhiteSpace(polilinha)) && (cargaMonitorada?.Carga?.Veiculo != null))
                {
                    //Busca somente posições do veículo
                    var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo.Codigo && pos?.DataVeiculo > cargaMonitorada?.DataInicio).OrderBy(o => o.DataVeiculo);

                    //Percorre todas as posições do veículo
                    foreach (var posicaoVeiculo in posicoesVeiculo)
                    {
                        var ultimoAlertaVeiculo = (from ale in ultimoAlerta
                                                   where ale.Veiculo == cargaMonitorada.Carga?.Veiculo.Codigo
                                                   orderby ale.Data descending
                                                   select ale).FirstOrDefault();

                        //Verifica se a posição esta no raio definido
                        var ponto = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                        {
                            Latitude = posicaoVeiculo.Latitude,
                            Longitude = posicaoVeiculo.Longitude,
                        };

                        double raio = parametro.Raio;
                        double menorDistancia = Servicos.Embarcador.Logistica.Polilinha.MenorDistancia(polilinha, ponto);
                        bool distanciaMinima = menorDistancia > raio;

                        //Verifica se a posição está no tempo mínimo
                        var tempoMinimo = true;
                        if (ultimoAlertaVeiculo != null)
                        {
                            var data = ultimoAlertaVeiculo.Data.AddMinutes(parametro.Tempo);
                            tempoMinimo = posicaoVeiculo.Data > data;
                        }

                        if ((distanciaMinima) && (tempoMinimo) && (!cargaMonitorada?.ParadaProgramada??false))
                            InserirAlerta(posicaoVeiculo, cargaMonitorada?.Carga);

                        
                    }

                }
            }

            AtualizarParametroPosicao(posicoes.LastOrDefault().Codigo);
        }

        public void Iniciar(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            _unidadeDeTrabalho = unidadeDeTrabalho;

            InicializarParametro();

            if (parametro?.MonitoramentoEvento?.Ativo == true)
            {
                Inicializar();

                Processar();
            }
         
        }
    }
}