using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento
{
    public class Rota
    {

        #region Métodos públicos estáticos 

        public static int BuscarDistanciaRotaMonitoramento(int codigoRotaFrete, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.RoteirizarPorCidade && carga.Rota != null)
            {
                return (int)carga.Rota.Quilometros;
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
                return repCargaRotaFretePontosPassagem.BuscarDistanciaPorCargaRotaFreteCodigo(codigoRotaFrete) / 1000;
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RotaRestanteAteDestino(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagens, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointAtual, string servidorOSRM)
        {

            // Extrai os pontos de passagem que são entregas da carga e ainda não finalizada
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsPassagem = ExtrairWayPointsDePassagemEntregas(cargaEntregas, pontosDePassagens, false);

            // Adiciona a posição atual como ponto de partida da roteirização
            wayPointsPassagem.Insert(0, wayPointAtual);

            // Rota entre os pontos de passsagem: poisção atual, entregas pendentes até o destino final (que pode ser até a origem)
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao resposta = Servicos.Embarcador.Logistica.Monitoramento.ControleDistancia.ObterRespostaRoteirizacao(wayPointsPassagem, servidorOSRM, false);

            return resposta;
        }

        public static decimal? CalcularDistanciaAteEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagens, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> pontosNaRotaAteDestino)
        {
            decimal distancia = 0;
            int totalPontos = pontosNaRotaAteDestino?.Count() ?? 0;
            int totalPontosDePassagens = pontosDePassagens?.Count() ?? 0;
            if (totalPontos > 0 && totalPontosDePassagens > 0)
            {
                for (int i = 0; i < totalPontosDePassagens; i++)
                {
                    if (PontoDePassagemEhEntrega(pontosDePassagens[i], cargaEntrega, false))
                    {
                        for (int j = 0; j < totalPontos; j++)
                        {
                            distancia += pontosNaRotaAteDestino[j].Distancia;
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointPontoDaRota = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(pontosNaRotaAteDestino[j].Lat, pontosNaRotaAteDestino[j].Lng);
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointPontoDePassagem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(pontosDePassagens[i].Latitude, pontosDePassagens[i].Longitude);
                            if (wayPointPontoDaRota.Equals(wayPointPontoDePassagem))
                            {
                                return distancia / 1000; // Km
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao Roteirizar(Dominio.Entidades.Cliente clienteOrigem, Dominio.Entidades.Cliente clienteDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);
            rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(clienteOrigem.Latitude, clienteOrigem.Longitude));
            rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(clienteDestino.Latitude, clienteDestino.Longitude));
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao response = rota.Roteirizar();
            return response;
        }

        #endregion

        #region Métodos privados

        private static List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> ExtrairWayPointsDePassagemEntregas(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagens, bool finalizada)
        {
            // Extrai os pontos de passagem que são entregas da carga
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsPassagem = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
            int total = pontosDePassagens?.Count ?? 0;
            if (total > 0)
            {
                for (int i = 0; i < total; i++)
                {
                    if (PontoDePassagemEhEntrega(pontosDePassagens[i], cargaEntregas, finalizada))
                    {
                        wayPointsPassagem.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(pontosDePassagens[i].Latitude, pontosDePassagens[i].Longitude));
                    }
                }

                // Último ponto caso seja um retorno
                if (pontosDePassagens.First().Cliente?.Codigo == pontosDePassagens.Last().Cliente?.Codigo &&
                    pontosDePassagens.Last().TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno &&
                    (wayPointsPassagem.Count > 0 && (wayPointsPassagem.Last().Latitude != (double)pontosDePassagens.Last().Latitude || wayPointsPassagem.Last().Longitude != (double)pontosDePassagens.Last().Longitude)))
                {
                    wayPointsPassagem.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(pontosDePassagens.Last().Latitude, pontosDePassagens.Last().Longitude));
                }
            }
            return wayPointsPassagem;

        }

        private static bool PontoDePassagemEhEntrega(Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoDePassagen, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, bool iniciouOuFinalizouEntrega)
        {
            int total = cargaEntregas?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (PontoDePassagemEhEntrega(pontoDePassagen, cargaEntregas[i], iniciouOuFinalizouEntrega))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool PontoDePassagemEhEntrega(Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoDePassagen, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, bool iniciouOuFinalizouEntrega)
        {
            if (pontoDePassagen.Cliente != null && cargaEntrega.Cliente != null && pontoDePassagen.Cliente.Codigo == cargaEntrega.Cliente.Codigo &&
                (
                    (pontoDePassagen.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta && cargaEntrega.Coleta) ||
                    (pontoDePassagen.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega && !cargaEntrega.Coleta)
                ) &&
                (
                    (iniciouOuFinalizouEntrega && (cargaEntrega.DataFim != null || cargaEntrega.DataInicio != null))
                    ||
                    (!iniciouOuFinalizouEntrega && cargaEntrega.DataFim == null && cargaEntrega.DataInicio == null)
                )
            )
            {
                return true;
            }
            return false;
        }

        #endregion

    }

}
