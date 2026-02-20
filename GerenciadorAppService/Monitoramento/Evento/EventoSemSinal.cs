using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public class EventoSemSinal: IMonitoramento
    {
        private DateTime dataZero = Convert.ToDateTime("01/01/1900");
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta;
        private Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual;
        private Repositorio.Embarcador.Logistica.Posicao repPosicao;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlerta;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> posicoesAtuais;
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemSinal;
        private Repositorio.Embarcador.Cargas.Carga repCarga;
        private List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas;
        private void InserirAlerta(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            var novoAlerta = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitor
            {
                DataCadastro = DateTime.Now,
                Data = DateTime.Now,
                TipoAlerta = tipoAlerta,
                Veiculo = carga.Veiculo,
                Posicao = posicao,
                Carga = carga
            };

            repAlerta.Inserir(novoAlerta);
            MonitoramentoUtils.AtualizarUltimoAlerta(ultimoAlerta, novoAlerta);

            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Inserindo alerta {novoAlerta.Codigo} ", this.GetType().Name);
        }
        private void InicializarParametro()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.SemSinal);
        }
        private void Inicializar()
        {
            repCarga = new Repositorio.Embarcador.Cargas.Carga(_unidadeDeTrabalho);
            cargas = repCarga.BuscarCargasSemMonitoramento();

            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);
            parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.SemSinal);

            repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unidadeDeTrabalho);
            ultimoAlerta = repAlerta.BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemSinal);

            repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unidadeDeTrabalho);
            posicoesAtuais = repPosicaoAtual.BuscarTodos();

            repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
        }
        private Dominio.Entidades.Embarcador.Logistica.PosicaoAtual InserirPosicaoAtual(Dominio.Entidades.Veiculo veiculo)
        {
            var posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao
            {
                Veiculo = veiculo,
                Descricao = "Sem posição",
                Data = dataZero,
                DataVeiculo = dataZero,
                DataCadastro = DateTime.Now,
                IDEquipamento = 0,
                Latitude = 0,
                Longitude = 0,
                Velocidade = 0,
                Ignicao = 0,
                Temperatura = 0
            };

            var posAtual = new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual
            {
                Veiculo = veiculo,
                Descricao = "Sem posição",
                Data = dataZero,
                DataVeiculo = dataZero,
                DataCadastro = DateTime.Now,
                IDEquipamento = 0,
                Latitude = 0,
                Longitude = 0,
                Velocidade = 0,
                Ignicao = 0,
                Temperatura = 0,
                Posicao = posicao
            };

            _unidadeDeTrabalho.Start();
            try
            {
                repPosicao.Inserir(posicao);
                repPosicaoAtual.Inserir(posAtual);

                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception ex)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro("Erro ao gravar alterta de sem sinal: " + ex);
                throw;
            }

            return posAtual;
        }
        private void Processar()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Processando cargas", this.GetType().Name);
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Processando carga {carga?.Codigo}", this.GetType().Name);
                if (carga.Veiculo == null)
                    continue;

                //Busca somente posições do veículo
                var posicaoAtualVeiculo = posicoesAtuais.Where(pos => pos.Veiculo?.Codigo == carga?.Veiculo.Codigo).OrderBy(o => o.DataVeiculo).FirstOrDefault();

                if (posicaoAtualVeiculo == null)
                {
                    posicaoAtualVeiculo = InserirPosicaoAtual(carga.Veiculo);
                    posicoesAtuais.Add(posicaoAtualVeiculo);
                }

                DateTime dataPosicaoVeiculo = posicaoAtualVeiculo?.DataVeiculo > dataZero? posicaoAtualVeiculo.DataVeiculo : DateTime.Now;

                
                var ultimoAlertaVeiculo = (from ale in ultimoAlerta
                                           where ale.Veiculo == carga.Veiculo.Codigo && ale.Data > carga.DataCriacaoCarga
                                           orderby ale.Data descending
                                           select ale).FirstOrDefault();

                DateTime data = ultimoAlertaVeiculo != null ? ultimoAlertaVeiculo.Data : DateTime.MinValue;

                data = data.AddMinutes(parametro.Tempo);

                var tempoMinimo = dataPosicaoVeiculo > data;

                if (tempoMinimo)  
                    InserirAlerta(posicaoAtualVeiculo.Posicao, carga);
            }
        }
        public void Iniciar(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Inicio", this.GetType().Name);
            _unidadeDeTrabalho = unidadeDeTrabalho;

            InicializarParametro();

            if (parametro?.MonitoramentoEvento?.Ativo == true)
            {
                Inicializar();

                Processar();
            }
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Fim\r\n", this.GetType().Name);
        }

    }
}