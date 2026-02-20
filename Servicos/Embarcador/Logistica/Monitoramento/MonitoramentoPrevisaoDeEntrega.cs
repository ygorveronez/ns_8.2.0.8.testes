using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Servicos.Embarcador.Logistica.Monitoramento
{
    public class PrevisaoDeEntrega
    {
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao;
        private Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido;
        private Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual;
        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> posicoesAtuais;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private Servicos.Embarcador.Logistica.Roteirizacao roteirizador;
        private void Inicializar()
        {
            repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unidadeDeTrabalho);

            repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unidadeDeTrabalho);
            posicoesAtuais = repPosicaoAtual.BuscarTodos();

            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unidadeDeTrabalho);
            configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            roteirizador = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);
        }
        private DateTime GetPrevisaoInicialEntrega(DateTime InicioViagem, Double Distancia, Double Velocidade, int TempoEntrega)
        {
            if (Velocidade == 0)
                return InicioViagem;

            double tempo = ((Distancia / 1000 / Velocidade) * 60) + TempoEntrega;

            return InicioViagem.AddMinutes(tempo);
        }
        private int GetTempoEntrega(Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual, Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoDaRota, int VelocidadeMedia)
        {
            var distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaKM(posicaoAtual.Latitude, posicaoAtual.Longitude, pontoDaRota.lat, pontoDaRota.lng);
            
            if ((distancia > 0) && (VelocidadeMedia > 0))
                return Convert.ToInt32((distancia * 1.3) / VelocidadeMedia * 60);

            return 0;
        }
        private int CalcularVelocidadeCarregado(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada)
        {
            decimal tempo = (cargaMonitorada?.Carga?.Rota?.TempoDeViagemEmMinutos ?? 0) / Convert.ToDecimal(60);
            decimal velocidaRota = 0;
            if (tempo > 0)
                velocidaRota = (cargaMonitorada?.Carga?.Rota?.Quilometros ?? 0) / tempo * Convert.ToDecimal(0.7); //70% da velocidade do veiculo

            if (velocidaRota < 0 || velocidaRota > 100)
                return 45;

            return cargaMonitorada?.Carga?.Rota?.VelocidadeMediaCarregado != null && cargaMonitorada?.Carga?.Rota?.VelocidadeMediaCarregado != 0 ? cargaMonitorada.Carga.Rota.VelocidadeMediaCarregado : Decimal.ToInt32(velocidaRota);
        }

        private void Processar(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unidadeDeTrabalho);


            List<int> codigoCargas = (from cargaMonitorada in cargasMonitoradas select cargaMonitorada.Codigo).ToList();


            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> cargasRotaFrete = repCargaRotaFrete.BuscarPorCargas(codigoCargas);

            try
            {
                foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
                {
                    if (cargaMonitorada?.Carga?.Veiculo == null)
                        continue;

                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = cargasRotaFrete.Where(o => o.Carga.Codigo == cargaMonitorada.Carga.Codigo).FirstOrDefault();

                    if (cargaRotaFrete == null || !repCargaRotaFretePontosPassagem.PossuiPorCargaRotaFrete(cargaRotaFrete.Codigo))
                        continue;

                    string pontosString = Servicos.Embarcador.Carga.RotaFrete.ObterPontosPassagemCargaRotaFreteSerializada(cargaRotaFrete, unidadeDeTrabalho);
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontosString);

                    var mediaEntrega = 15;
                    var tempoEntrega = 0;

                    var posicaoAtual = posicoesAtuais.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo?.Codigo).FirstOrDefault();

                    if (posicaoAtual != null)
                    {
                        int velocidadeCarregado = CalcularVelocidadeCarregado(cargaMonitorada);

                        foreach (var pontoRota in pontosDaRota)
                        {
                            var pedidoRota = cargaMonitorada.Carga.Pedidos.Where(p => p.ClienteEntrega?.Codigo == pontoRota.codigo).ToList();

                            if (pedidoRota == null || pedidoRota.Count == 0)
                                continue;

                            foreach (var pedido in pedidoRota)
                            {
                                if (pedido.DataSaida != null)
                                    continue;

                                if (pedido.Pedido?.PrevisaoEntrega == null)
                                    pedido.Pedido.PrevisaoEntrega = GetPrevisaoInicialEntrega(cargaMonitorada.DataInicio ?? DateTime.Now, pontoRota.distancia,
                                        velocidadeCarregado, tempoEntrega);

                                var tempoProximaEntrega = GetTempoEntrega(posicaoAtual, pontoRota, velocidadeCarregado);
                                pedido.Pedido.PrevisaoEntregaAtual = DateTime.Now.AddMinutes(tempoProximaEntrega);

                                repCargaPedido.Atualizar(pedido);

                                tempoEntrega = tempoEntrega + mediaEntrega;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unidadeDeTrabalho.FlushAndClear();
            }
        }
      
        public void Iniciar(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            _unidadeDeTrabalho = unidadeDeTrabalho;

            Inicializar();

            Processar(_unidadeDeTrabalho);
        }

    }
}