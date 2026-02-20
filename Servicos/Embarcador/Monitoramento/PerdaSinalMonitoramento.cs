using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento
{
    public class PerdaSinalMonitoramento
    {

        #region Atributos

        private Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public PerdaSinalMonitoramento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Metodos Publicos

        public void CriarNovoRegistroPerdaSinal(Dominio.ObjetosDeValor.Embarcador.Logistica.ParametrosPerdaSinalMonitoramento parametros)
        {
            Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento repPerdaSinalMonitoramento = new Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento perdaSinalMonitoramento = repPerdaSinalMonitoramento.BuscarUltimoPorMonitoramentoEmAberto(parametros.codigoMonitoramento);

            if (perdaSinalMonitoramento == null)
            {
                perdaSinalMonitoramento = new Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento();
                perdaSinalMonitoramento.Monitoramento = new Dominio.Entidades.Embarcador.Logistica.Monitoramento { Codigo = parametros.codigoMonitoramento };
                perdaSinalMonitoramento.Veiculo = parametros.CodigoVeiculo > 0 ? new Dominio.Entidades.Veiculo { Codigo = parametros.CodigoVeiculo } : null;
                perdaSinalMonitoramento.DataInicio = parametros.DataInicio;
                perdaSinalMonitoramento.TempoSegundos = 0;
                perdaSinalMonitoramento.LatitudeInicio = parametros.Latitude;
                perdaSinalMonitoramento.LongitudeInicio = parametros.Longitude;
                perdaSinalMonitoramento.AlertaMonitor = parametros.AlertaMonitor;
                repPerdaSinalMonitoramento.Inserir(perdaSinalMonitoramento);
            }
        }


        public void FinalizarRegistroPerdaSinal(Dominio.ObjetosDeValor.Embarcador.Logistica.ParametrosPerdaSinalMonitoramento parametros)
        {
            Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento repPerdaSinalMonitoramento = new Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento(_unitOfWork);
            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento perdaSinalMonitoramento = repPerdaSinalMonitoramento.BuscarUltimoPorMonitoramentoEmAberto(parametros.codigoMonitoramento);

            if (perdaSinalMonitoramento != null)
            {
                if (perdaSinalMonitoramento.Veiculo == null && parametros.CodigoVeiculo > 0)
                    perdaSinalMonitoramento.Veiculo = new Dominio.Entidades.Veiculo { Codigo = parametros.CodigoVeiculo };

                perdaSinalMonitoramento.DataFim = parametros.DataFim;
                perdaSinalMonitoramento.LatitudeFim = parametros.Latitude;
                perdaSinalMonitoramento.LongitudeFim = parametros.Longitude;

                perdaSinalMonitoramento.TempoSegundos = (int)(perdaSinalMonitoramento.DataFim.Value - perdaSinalMonitoramento.DataInicio).TotalSeconds;

                perdaSinalMonitoramento.AlertaPossuiPosicaoRetroativa = VerificarRegistrosPerdaSinalPosicaoRetroativa(perdaSinalMonitoramento.DataInicio, parametros.DataFim, parametros.CodigoVeiculo);
                if (perdaSinalMonitoramento.AlertaMonitor != null)
                    perdaSinalMonitoramento.AlertaMonitor.AlertaPossuiPosicaoRetroativa = perdaSinalMonitoramento.AlertaPossuiPosicaoRetroativa;

                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDestino = null;
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointOrigem = null;

                if (perdaSinalMonitoramento.LatitudeFim > 0 && perdaSinalMonitoramento.LongitudeFim > 0)
                    wayPointDestino = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(perdaSinalMonitoramento.LatitudeFim, perdaSinalMonitoramento.LongitudeFim);

                if (perdaSinalMonitoramento.LatitudeInicio > 0 && perdaSinalMonitoramento.LongitudeInicio > 0)
                    wayPointOrigem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(perdaSinalMonitoramento.LatitudeInicio, perdaSinalMonitoramento.LongitudeInicio);

                double metros = 0;
                if (wayPointOrigem != null && wayPointDestino != null)
                {
                    metros = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(wayPointOrigem, wayPointDestino);
                    perdaSinalMonitoramento.DistanciaPerdaSinal = metros;
                }

                repPerdaSinalMonitoramento.Atualizar(perdaSinalMonitoramento);

                if (perdaSinalMonitoramento.AlertaMonitor != null)
                    repAlertaMonitor.Atualizar(perdaSinalMonitoramento.AlertaMonitor);

                //Redmine: #77397 - DevOps: 442
                TratarEventoAutomaticamente(perdaSinalMonitoramento.Monitoramento?.Carga ?? null, TratativaAutomaticaMonitoramentoEvento.RetornoSinal, _unitOfWork);
            }
        }

        public bool VerificarRegistrosPerdaSinalPosicaoRetroativa(DateTime dataInicio, DateTime dataFim, int veiculo)
        {
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork);
            //com a data Inicial e final da perda de sinal, é preciso varrer as posições e validar se ainda dentro deste periodo existe um intervalo maior ou igual a 60 minutos
            bool tevePosicoesRetroativas = false;

            List<Dominio.Entidades.Embarcador.Logistica.Posicao> listaPosicao = repPosicao.BuscarPorVeiculoDataInicialeFinal(veiculo, dataInicio, dataFim);
            DateTime ultimaData = dataInicio;

            if (listaPosicao?.Count > 0)
                tevePosicoesRetroativas = true;

            foreach (Dominio.Entidades.Embarcador.Logistica.Posicao posicao in listaPosicao)
            {
                TimeSpan diferenca = posicao.DataVeiculo - ultimaData;
                if (diferenca.TotalMinutes >= 60)
                {
                    tevePosicoesRetroativas = false;
                    break;
                }

                if (posicao.DataVeiculo > ultimaData)
                    ultimaData = posicao.DataVeiculo;
            }

            return tevePosicoesRetroativas;
        }

        #endregion

        #region Metodos Privados

        private static void TratarEventoAutomaticamente(Dominio.Entidades.Embarcador.Cargas.Carga carga, TratativaAutomaticaMonitoramentoEvento tipoTratativa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Monitoramento.MonitoramentoEventoTratativaAutomatica serMonitoramentoEventoTratativaAutomatica = new Servicos.Embarcador.Monitoramento.MonitoramentoEventoTratativaAutomatica(unitOfWork);
            serMonitoramentoEventoTratativaAutomatica.TratarEventoAutomaticamente(carga, tipoTratativa);
        }

        #endregion
    }
}
