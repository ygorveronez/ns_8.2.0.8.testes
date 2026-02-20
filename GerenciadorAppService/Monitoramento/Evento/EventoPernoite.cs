using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoPernoite : IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametroFimDeViagem;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private Repositorio.Embarcador.Logistica.Locais repLocais;
        private List<Dominio.Entidades.Embarcador.Logistica.Locais> locais;

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlertaVeiculo;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.Pernoite;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.Pernoite);
            parametroFimDeViagem = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.FimDeViagem);
        }
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unidadeDeTrabalho);
            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
            repLocais = new Repositorio.Embarcador.Logistica.Locais(_unidadeDeTrabalho);

            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            ultimoAlertaVeiculo = repAlerta.BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.Pernoite);

            locais = repLocais.BuscarPorTipoDeLocal(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.Pernoite);

            if (parametro.Posicao == 0)
                AtualizarParametroPosicao(repPosicao.BuscarUltimaPosicao()?.Codigo ?? 0);

            Int64 posicao = parametro.Posicao < parametroFimDeViagem.Posicao || parametroFimDeViagem.Posicao == 0 ? parametro.Posicao : parametroFimDeViagem.Posicao;

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
                Servicos.Log.TratarErro("Erro ao gravar EventoParadaNaoProgramada: " + ex);
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
                DateTime dataContoleParado = cargaMonitorada?.PosicaoPernoite?.DataVeiculo ?? cargaMonitorada?.DataInicio?? DateTime.MinValue;
                foreach (var posicaoVeiculo in posicoesVeiculo)
                {
                    DateTime dataInicioPernoite = posicaoVeiculo.Data.Date.AddHours(-6);//18:00;
                    DateTime dataFimPernoite = posicaoVeiculo.Data.Date.AddHours(8);//8:00;

                    if (posicaoVeiculo.Data > dataFimPernoite)
                    {
                        dataInicioPernoite = dataInicioPernoite.AddDays(1);
                        dataFimPernoite = dataFimPernoite.AddDays(1);
                    }

                    bool periodoPernoite = (posicaoVeiculo.Data >= dataInicioPernoite && posicaoVeiculo.Data < dataFimPernoite);

                    if ((!periodoPernoite) ||(cargaMonitorada == null) || (cargaMonitorada.ParadaProgramada == true) || (posicaoVeiculo.DataVeiculo < cargaMonitorada.DataInicio) || (posicaoVeiculo.DataVeiculo <= dataContoleParado))
                        continue;

                    var ultimoAlertaVeiculo = (from ale in this.ultimoAlertaVeiculo
                                               where ale.Veiculo == posicaoVeiculo.Veiculo.Codigo
                                               orderby ale.Data descending
                                               select ale).FirstOrDefault();

                    var tempoMinimoAlerta = false;
                    var validarVeiculoParado = false;

                    var temRegistro = (cargaMonitorada?.PosicaoPernoite != null);

                    if (!temRegistro)
                    {
                        cargaMonitorada.PosicaoPernoite = posicaoVeiculo;
                        repMonitoramento.Atualizar(cargaMonitorada);
                    }

                    if (temRegistro)
                    {
                        double distancia = 1000;
                        if ((cargaMonitorada.PosicaoPernoite != null) && (cargaMonitorada.PosicaoPernoite.DataVeiculo < posicaoVeiculo.DataVeiculo))
                            distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, cargaMonitorada.PosicaoPernoite.Latitude, cargaMonitorada.PosicaoPernoite.Longitude);

                        var atualizarVeiculoParado = true;
                        if (distancia < 0.1)
                        {
                            atualizarVeiculoParado = false;
                            
                            var dataparado = cargaMonitorada.PosicaoPernoite.Data.AddMinutes(parametro.Tempo);
                            validarVeiculoParado = posicaoVeiculo.Data > dataparado;
                            
                            DateTime data = ultimoAlertaVeiculo != null? ultimoAlertaVeiculo.Data : dataparado;
                           
                            data = data.AddMinutes(parametro.Tempo);
                            tempoMinimoAlerta = posicaoVeiculo.DataVeiculo > data;
                        }

                        if (distancia > 0.1)
                            dataContoleParado = posicaoVeiculo.DataVeiculo;

                        if (atualizarVeiculoParado)
                        {
                            cargaMonitorada.PosicaoPernoite = posicaoVeiculo;
                            repMonitoramento.Atualizar(cargaMonitorada);
                        }
                    }

                    var dataAlerta = ultimoAlertaVeiculo != null ? ultimoAlertaVeiculo.Data : DateTime.MinValue;

                    bool gerarAlertaPernoite = !(dataAlerta >= dataInicioPernoite) && (dataAlerta < dataFimPernoite);

                    if (tempoMinimoAlerta && validarVeiculoParado && gerarAlertaPernoite)
                    {
                        bool pernoite = SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.VerificarLocais(locais, posicaoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.Pernoite);
                        if (pernoite)
                            InserirAlerta(posicaoVeiculo, cargaMonitorada.Carga);
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