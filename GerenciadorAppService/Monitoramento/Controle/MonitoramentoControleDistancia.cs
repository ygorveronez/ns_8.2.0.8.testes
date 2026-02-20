using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento.Controle
{
    public class MonitoramentoControleDistancia : IMonitoramento
    {
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao;
        private Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private Servicos.Embarcador.Logistica.Roteirizacao rota;
        
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();
            var cargasFinalizadas = repMonitoramento.BuscarMonitoramentoFinalizadoPorDataFinalMaior(DateTime.Now.AddHours(-3));
            cargasMonitoradas.AddRange(cargasFinalizadas);

            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);

            repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unidadeDeTrabalho);
            configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);
        }
        private void Processar()
        {
            foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
            {
                if (cargaMonitorada?.Carga?.Veiculo == null)
                    continue;

                DateTime dataInicio = cargaMonitorada.DataInicio ?? DateTime.Now;
                DateTime dataFim = cargaMonitorada.DataFim ?? DateTime.Now;

                var posicoesVeiculo = repPosicao.BuscarPorVeiculoDataInicialeFinal(cargaMonitorada.Carga.Veiculo.Codigo, dataInicio, dataFim);

                var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar { AteOrigem = false, Ordenar = false, PontosNaRota = true };

                var listaPontos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();

                double LatAnterior = -200;
                double LngAnterior = -200;

                foreach (var posicaoVeiculo in posicoesVeiculo)
                {
                    //Adicionar posicoes a cada 20 metros
                    var distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(LatAnterior, LngAnterior, posicaoVeiculo.Latitude, posicaoVeiculo.Longitude);
                    var distanciaMinima = 20;
                    if (distancia > distanciaMinima)
                    {
                        listaPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint { Lat = posicaoVeiculo.Latitude, Lng = posicaoVeiculo.Longitude });
                        LatAnterior = posicaoVeiculo.Latitude;
                        LngAnterior = posicaoVeiculo.Longitude;
                    }
                
                }

                try
                {
                    rota.Clear();

                    rota.Add(listaPontos);
                    var resposta = rota.Roteirizar(opcoes);

                    if ((resposta.Status == "OK") && (resposta.Distancia > 0))
                    {
                        cargaMonitorada.DistanciaRealizada = resposta.Distancia;
                        cargaMonitorada.PolilinhaRealizada = resposta.Polilinha;
                        repMonitoramento.Atualizar(cargaMonitorada);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }
        public void Iniciar(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            _unidadeDeTrabalho = unidadeDeTrabalho;

            Inicializar();

            Processar();
        }

    }
}