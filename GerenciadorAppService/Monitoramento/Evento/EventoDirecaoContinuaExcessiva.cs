using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoDirecaoContinuaExcessiva : IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametroInicioEntrega;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;


        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlertaVeiculo;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.DirecaoContinuaExcessiva;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.DirecaoContinuaExcessiva);
            parametroInicioEntrega = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioEntrega);
        }
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unidadeDeTrabalho);
            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);

            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            ultimoAlertaVeiculo = repAlerta.BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.DirecaoContinuaExcessiva);


            if (parametro.Posicao == 0)
                AtualizarParametroPosicao(repPosicao.BuscarUltimaPosicao()?.Codigo??0);

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
                Servicos.Log.TratarErro("Erro ao gravar EventoDirecaoContinuaExcessiva: " + ex);
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
                var dataControleDirecaoContinua = cargaMonitorada.DataInicio ?? DateTime.Now;

                if (cargaMonitorada.Carga?.Veiculo == null)
                    continue;

                var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo?.Codigo && pos?.DataVeiculo > cargaMonitorada?.DataInicio).OrderBy(o => o.DataVeiculo);
                DateTime dataParada = cargaMonitorada?.PosicaoDirecaoExcessiva?.DataVeiculo ?? DateTime.MinValue;
                foreach (var posicaoVeiculo in posicoesVeiculo)
                {
                    if ((cargaMonitorada == null) || (cargaMonitorada.ParadaProgramada == true) || (posicaoVeiculo.DataVeiculo < cargaMonitorada.DataInicio) || (posicaoVeiculo.DataVeiculo <= dataParada))
                        continue;

                    var ultimoAlertaVeiculo = (from ale in this.ultimoAlertaVeiculo
                                               where ale.Veiculo == posicaoVeiculo.Veiculo.Codigo
                                               orderby ale.Data descending
                                               select ale).FirstOrDefault();


                    var tempoMinimoAlerta = false;
                    var validarDirecaoExessiva = false;

                    var temRegistro = (cargaMonitorada?.PosicaoDirecaoExcessiva != null);

                    if (!temRegistro)
                    {
                        cargaMonitorada.PosicaoDirecaoExcessiva = posicaoVeiculo;
                        repMonitoramento.Atualizar(cargaMonitorada);
                    }


                    if (temRegistro)
                    {
                        double distancia = 1000;
                        if ((cargaMonitorada.PosicaoDirecaoExcessiva != null) && (cargaMonitorada.PosicaoDirecaoExcessiva.DataVeiculo < posicaoVeiculo.DataVeiculo))
                            distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, cargaMonitorada.PosicaoDirecaoExcessiva.Latitude, cargaMonitorada.PosicaoDirecaoExcessiva.Longitude);

                        var veiculoParado = distancia < 0.1;
                        var veiculoEmMovimento = !veiculoParado;

                        if (veiculoParado)
                        {
                            var parametroParado = 45;
                            var dataparado = cargaMonitorada.PosicaoDirecaoExcessiva.Data.AddMinutes(parametroParado);
                            var validarIntervaloDescanso = posicaoVeiculo.Data >= dataparado;

                            if (validarIntervaloDescanso)
                                cargaMonitorada.DataFimDescanso = posicaoVeiculo.DataVeiculo;
                        }

                        if (veiculoEmMovimento)
                        {
                            dataControleDirecaoContinua = posicaoVeiculo.DataVeiculo;
                            cargaMonitorada.PosicaoDirecaoExcessiva = posicaoVeiculo;

                            repMonitoramento.Atualizar(cargaMonitorada);
                        }

                        TimeSpan diferencaData = posicaoVeiculo.DataVeiculo - (cargaMonitorada.DataFimDescanso ?? DateTime.Now);

                        var parametroDirecaoExcessiva = parametro.TempoEvento; //4 horas

                        if ((veiculoEmMovimento) && (diferencaData.TotalMinutes > parametroDirecaoExcessiva))
                        {
                            DateTime data = ultimoAlertaVeiculo != null ? ultimoAlertaVeiculo.Data : DateTime.MinValue;

                            data = data.AddMinutes(parametro.Tempo);
                            tempoMinimoAlerta = posicaoVeiculo.DataVeiculo > data;
                            validarDirecaoExessiva = true;
                        }
                    }

                    if (tempoMinimoAlerta && validarDirecaoExessiva)
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