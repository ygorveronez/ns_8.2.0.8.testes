using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoVelocidade : IMonitoramento
    {
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlerta;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes;
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.VelocidadeExcedida;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        
        private void InserirNovoAlerta(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            var novoAlerta = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitor
            {
                DataCadastro = DateTime.Now,
                Data = posicao.DataVeiculo,
                TipoAlerta =  tipoAlerta,
                Veiculo = posicao.Veiculo,
                Posicao = posicao,
                Carga = carga
            };

            _unidadeDeTrabalho.Start();
            try
            {
                repAlerta.Inserir(novoAlerta);
                

                MonitoramentoUtils.AtualizarUltimoAlerta(ultimoAlerta, novoAlerta);
                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar EventoVelocidade: " + ex);
                throw;
            }
        }
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.Velocidade);
        }
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unidadeDeTrabalho);
            ultimoAlerta = repAlerta.BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.VelocidadeExcedida);

            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);

            if (parametro.Posicao == 0)
                AtualizarParametroPosicao(repPosicao.BuscarUltimaPosicao()?.Codigo ?? 0);

            posicoes = repPosicao.BuscarPorCodigoMaiorEVelociadade(parametro.Posicao, parametro.Velocidade);
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
                if (cargaMonitorada?.Carga?.Veiculo == null)
                    continue;

                //Busca somente posições do veículo
                var posicoesVeiculo = posicoes.Where(pos => pos.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo.Codigo && pos?.DataVeiculo > cargaMonitorada?.DataInicio).OrderBy(o => o.DataVeiculo);

                //Percorre todas as posições do veículo
                foreach (var posicaoVeiculo in posicoesVeiculo)
                {
                    var ultimoAlertaVeiculo = (from ale in ultimoAlerta
                                               where ale.Veiculo == cargaMonitorada.Carga.Veiculo.Codigo && ale.Data > cargaMonitorada.DataInicio
                                               orderby ale.Data descending
                                               select ale).FirstOrDefault();

                    //Verificar velocidade excedida
                    var velocidade = parametro.Velocidade;//80
                    var velocidadeExcedida = posicaoVeiculo.Velocidade > velocidade;

                    var tempoMinimo = true;
                    var distanciaMinima = true;
                    if (ultimoAlertaVeiculo != null)
                    {
                        //Verificar tempo
                        var data = ultimoAlertaVeiculo.Data.AddMinutes(parametro.Tempo);
                        tempoMinimo = posicaoVeiculo.Data > data;

                        //verificar distância
                        var parametroDistancia = 5;
                        double distancia = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(posicaoVeiculo.Latitude, posicaoVeiculo.Longitude, ultimoAlertaVeiculo.Latitude, ultimoAlertaVeiculo.Longitude);
                        distanciaMinima = distancia > parametroDistancia;
                    }

                    if (tempoMinimo && velocidadeExcedida && distanciaMinima)
                    {
                        var cargaVeiculo = cargasMonitoradas.Where(car => car?.Carga?.Veiculo == posicaoVeiculo.Veiculo).OrderByDescending(o => o.Codigo).FirstOrDefault();

                        if (cargaVeiculo?.Carga != null)
                            InserirNovoAlerta(posicaoVeiculo, cargaVeiculo?.Carga);
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