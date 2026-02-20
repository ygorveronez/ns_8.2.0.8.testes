using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoTemperatura
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlerta;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
       
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.TemperaturaForaDaFaixa;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        
        private void AtualizarParametroPosicao(Int64 codigoPosicao)
        {
            parametro.Posicao = codigoPosicao;
            repMonitoramentoEvento.Atualizar(parametro);
        }

        private void InserirAlertas(List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> listaAlertas) {
            
            _unidadeDeTrabalho.Start();
            try
            {
                foreach (var alerta in listaAlertas)
                {
                    repAlerta.Inserir(alerta);
                }

                AtualizarParametroPosicao(posicoes.LastOrDefault().Codigo);

                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar EventoTemperatura: " + ex);
                throw;
            }
        }
       
        private Dominio.Entidades.Embarcador.Logistica.AlertaMonitor CriarAlerta(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Cargas.Carga carga)
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

            MonitoramentoUtils.AtualizarUltimoAlerta(ultimoAlerta, novoAlerta);

            return novoAlerta;
        }
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.Temperatura);
        }
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.Temperatura);

            repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unidadeDeTrabalho);
            ultimoAlerta = repAlerta.BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.TemperaturaForaDaFaixa);

            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
            posicoes = repPosicao.BuscarPorCodigoMaior(parametro.Posicao);

            if (parametro.Posicao == 0)
                AtualizarParametroPosicao(repPosicao.BuscarUltimaPosicao()?.Codigo ?? 0);
        }
        private void Processar()
        {
            if (posicoes.Count == 0)
                return;

            Int64 codigoPosicao = 0;
            var listaAlertas = new List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
            {
                if ( (cargaMonitorada.Carga?.TipoDeCarga?.ControlaTemperatura??false) || (cargaMonitorada.Carga?.TipoDeCarga?.FaixaDeTemperatura == null) || (cargaMonitorada?.Carga?.Veiculo == null))
                    continue;

                //Busca somente posições do veículo
                var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo.Codigo && pos?.DataVeiculo > cargaMonitorada?.DataInicio).OrderBy(o => o.DataVeiculo);

                //Percorre todas as posições do veículo
                foreach (var posicaoVeiculo in posicoesVeiculo)
                {
                    codigoPosicao = posicaoVeiculo.Codigo;

                    var ultimoAlertaVeiculo = (from ale in ultimoAlerta
                                               where ale.Veiculo == cargaMonitorada.Carga.Veiculo.Codigo
                                               orderby ale.Data descending
                                               select ale).FirstOrDefault();
                    var tempoMinimo = true;
                    var temperaturaForaFaixa = !MonitoramentoUtils.InRange(cargaMonitorada.Carga.TipoDeCarga.FaixaDeTemperatura.FaixaInicial, cargaMonitorada.Carga.TipoDeCarga.FaixaDeTemperatura.FaixaFinal, posicaoVeiculo.Temperatura??-200);

                    if (temperaturaForaFaixa)
                    {
                        if (ultimoAlertaVeiculo != null)
                        {
                            var data = ultimoAlertaVeiculo.Data.AddMinutes(parametro.Tempo);
                            tempoMinimo = posicaoVeiculo.Data > data;
                        }
                    }

                    if (tempoMinimo && temperaturaForaFaixa)
                    {
                        var alerta = CriarAlerta(posicaoVeiculo, cargaMonitorada.Carga);
                        listaAlertas.Add(alerta);
                    }
                        
                }
            }

            InserirAlertas(listaAlertas);
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