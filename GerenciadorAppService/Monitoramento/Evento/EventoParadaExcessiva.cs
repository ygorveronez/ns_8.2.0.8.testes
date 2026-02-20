using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoParadaExessiva : IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametroInicioEntrega;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlertaVeiculo;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaExcessiva;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.ParadaExcessiva);
            parametroInicioEntrega = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioEntrega);
        }
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unidadeDeTrabalho);
            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);

            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            ultimoAlertaVeiculo = repAlerta.BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaExcessiva);


            if (parametro.Posicao == 0)
                AtualizarParametroPosicao(repPosicao.BuscarUltimaPosicao()?.Codigo ?? 0);

            Int64 posicao = parametro.Posicao < parametroInicioEntrega.Posicao || parametroInicioEntrega.Posicao == 0 ? parametro.Posicao : parametroInicioEntrega.Posicao;

            posicoes = repPosicao.BuscarPorCodigoMaior(posicao);
        }
        private void InserirAlerta(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            _unidadeDeTrabalho.Start();
            try
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

                repAlerta.Inserir(novoAlerta);
                MonitoramentoUtils.AtualizarUltimoAlerta(ultimoAlertaVeiculo, novoAlerta);

                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar EventoParadaExcessiva: " + ex);
                throw;
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

            foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
            {
                if (cargaMonitorada.Carga?.Veiculo == null)
                    continue;

                var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo.Codigo && pos?.DataVeiculo > cargaMonitorada?.DataInicio).OrderBy(o => o.DataVeiculo);
                DateTime dataControleParado = cargaMonitorada?.PosicaoParadaExcessiva?.DataVeiculo ?? cargaMonitorada?.DataInicio??DateTime.MinValue;
                foreach (var posicaoVeiculo in posicoesVeiculo)
                {
                    if ((cargaMonitorada == null) || (cargaMonitorada.ParadaProgramada == true) || (posicaoVeiculo.DataVeiculo < cargaMonitorada.DataInicio) || (posicaoVeiculo.DataVeiculo <= dataControleParado))
                        continue;

                    var ultimoAlertaVeiculo = (from ale in this.ultimoAlertaVeiculo
                                               where ale.Veiculo == posicaoVeiculo.Veiculo.Codigo
                                               orderby ale.Data descending
                                               select ale).FirstOrDefault();


                    var tempoMinimoAlerta = false;
                    var validarVeiculoParado = false;

                    var temRegistro = (cargaMonitorada?.PosicaoParadaExcessiva != null);

                    if (!temRegistro)
                    {
                        cargaMonitorada.PosicaoParadaExcessiva = posicaoVeiculo;
                        repMonitoramento.Atualizar(cargaMonitorada);
                    }


                    if (temRegistro)
                    {
                        double distancia = 1000;
                        if ((cargaMonitorada.PosicaoParadaExcessiva != null) && (cargaMonitorada.PosicaoParadaExcessiva.DataVeiculo < posicaoVeiculo.DataVeiculo))
                            distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, cargaMonitorada.PosicaoParadaExcessiva.Latitude, cargaMonitorada.PosicaoParadaExcessiva.Longitude);

                        var atualizarVeiculoParado = true;
                        if (distancia < 0.1)
                        {
                            atualizarVeiculoParado = false;
                            
                            var dataValidarEvento = cargaMonitorada.PosicaoParadaExcessiva.Data.AddMinutes(parametro.Tempo);
                            validarVeiculoParado = posicaoVeiculo.Data > dataValidarEvento;

                            var data = ultimoAlertaVeiculo != null ? ultimoAlertaVeiculo.Data : dataValidarEvento;
                            data = data.AddMinutes(parametro.Tempo);
                            tempoMinimoAlerta = posicaoVeiculo.DataVeiculo > data;

                        }

                        if (distancia > 0.1)
                            dataControleParado = posicaoVeiculo.DataVeiculo;

                        if (atualizarVeiculoParado)
                        {
                            cargaMonitorada.PosicaoParadaExcessiva = posicaoVeiculo;

                            repMonitoramento.Atualizar(cargaMonitorada);
                        }
                    }


                    if (tempoMinimoAlerta && validarVeiculoParado)
                        InserirAlerta(posicaoVeiculo, cargaMonitorada.Carga);
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