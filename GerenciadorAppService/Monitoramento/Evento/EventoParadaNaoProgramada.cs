using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoParadaNaoProgramada : IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametroInicioEntrega;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private Repositorio.Embarcador.Logistica.Locais repLocais;
        private List<Dominio.Entidades.Embarcador.Logistica.Locais> locais;

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlertaVeiculo;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaNaoProgramada;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.ParadaNaoProgramada);
            parametroInicioEntrega = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioEntrega);
        }
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unidadeDeTrabalho);
            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
            repLocais = new Repositorio.Embarcador.Logistica.Locais(_unidadeDeTrabalho);

            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            ultimoAlertaVeiculo = repAlerta.BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaNaoProgramada);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal> listaLocais = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal>
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.AreaDeRisco,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.Pernoite
            };


            locais = repLocais.BuscarPorTiposDeLocais(listaLocais);

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
                var dataControleParado = cargaMonitorada.DataInicio ?? DateTime.Now;

                if (cargaMonitorada.Carga?.Veiculo == null)
                    continue;

                var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo.Codigo && pos?.DataVeiculo > cargaMonitorada?.DataInicio).OrderBy(o => o.DataVeiculo);
                DateTime dataParada = cargaMonitorada?.PosicaoParadaNaoProgramada?.DataVeiculo ?? DateTime.MinValue;
                foreach (var posicaoVeiculo in posicoesVeiculo)
                {
                    if ((cargaMonitorada == null) || (cargaMonitorada.ParadaProgramada == true) || (posicaoVeiculo.DataVeiculo < cargaMonitorada.DataInicio) || (posicaoVeiculo.DataVeiculo <= dataParada))
                        continue;

                    var ultimoAlertaVeiculo = (from ale in this.ultimoAlertaVeiculo
                                               where ale.Veiculo == posicaoVeiculo.Veiculo.Codigo
                                               orderby ale.Data descending
                                               select ale).FirstOrDefault();


                    var tempoMinimoAlerta = false;
                    var validarVeiculoParado = false;

                    var temRegistro = (cargaMonitorada?.PosicaoParadaNaoProgramada != null);

                    if (!temRegistro)
                    {
                        cargaMonitorada.PosicaoParadaNaoProgramada = posicaoVeiculo;
                        repMonitoramento.Atualizar(cargaMonitorada);
                    }


                    if (temRegistro)
                    {
                        double distancia = 1000;
                        if ((cargaMonitorada.PosicaoParadaNaoProgramada != null) && (cargaMonitorada.PosicaoParadaNaoProgramada.DataVeiculo < posicaoVeiculo.DataVeiculo))
                            distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, cargaMonitorada.PosicaoParadaNaoProgramada.Latitude, cargaMonitorada.PosicaoParadaNaoProgramada.Longitude);

                        var atualizarVeiculoParado = true;
                        if (distancia < 0.1)
                        {
                            atualizarVeiculoParado = false;

                            var dataparado = cargaMonitorada.PosicaoParadaNaoProgramada.Data.AddMinutes(parametro.Tempo);
                            validarVeiculoParado = posicaoVeiculo.Data > dataparado;

                            //Verificar tempo do alerta
                            DateTime data = ultimoAlertaVeiculo != null ? ultimoAlertaVeiculo.Data : dataparado;

                            data = data.AddMinutes(parametro.Tempo);
                            tempoMinimoAlerta = posicaoVeiculo.DataVeiculo > data;
                        }

                        if (atualizarVeiculoParado)
                        {
                            dataControleParado = posicaoVeiculo.DataVeiculo;
                            cargaMonitorada.PosicaoParadaNaoProgramada = posicaoVeiculo;

                            repMonitoramento.Atualizar(cargaMonitorada);
                        }
                    }


                    if (tempoMinimoAlerta && validarVeiculoParado)
                    {
                        bool areaDeRisco = SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.VerificarLocais(locais, posicaoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.AreaDeRisco);
                        bool pernoite = SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.VerificarLocais(locais, posicaoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.Pernoite);
                        if (!areaDeRisco && !pernoite)
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