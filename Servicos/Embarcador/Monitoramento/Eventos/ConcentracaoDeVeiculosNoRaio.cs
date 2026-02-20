using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class ConcentracaoDeVeiculosNoRaio : AbstractEvento
    {
        #region Atributos públicos
        public List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> LocaisRaioProximidade { get; set; }
        #endregion

        #region Métodos públicos

        public ConcentracaoDeVeiculosNoRaio(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ConcentracaoDeVeiculosNoRaio)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo();
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {
            if (monitoramento == null || monitoramento.Carga == null || this.MonitoramentoEvento.Gatilho?.RaioProximidade == null) return;

            if (!this.LocaisRaioProximidade.Contains(this.MonitoramentoEvento.Gatilho.RaioProximidade)) return;
            if (this.MonitoramentoEvento.Gatilho.Quantidade == 0) return;

            int maximoVeiculosNoRaio = this.MonitoramentoEvento.Gatilho.Quantidade;
            int veiculosNoRaio = 0;

            Dominio.Entidades.Embarcador.Logistica.RaioProximidade raioProximidadeVeiculo = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarRaioVeiculoEmAreaRaioProximidade(
                this.LocaisRaioProximidade.ToArray(),
                monitoramentoProcessarEvento.LatitudePosicao ?? 0,
                monitoramentoProcessarEvento.LongitudePosicao ?? 0);

            if (raioProximidadeVeiculo == null)
                return;

            Repositorio.Embarcador.Logistica.Posicao repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            DateTime dataInicial = (monitoramentoProcessarEvento.DataVeiculoPosicao ?? DateTime.Now).AddMinutes(-raioProximidadeVeiculo.Tempo);
            DateTime dataFinal = (monitoramentoProcessarEvento.DataVeiculoPosicao ?? DateTime.Now).AddMinutes(raioProximidadeVeiculo.Tempo);

            //Busca os veículos e suas posições entre a faixa de tempo de permanência, configurado no Raio de Proximidade.
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            List<int> codigosVeiculos = repositorioVeiculo.BuscarCodigosVeiculosAtivos();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculos = repositorioPosicaoAtual.BuscarPosicoesVeiculoPorData(codigosVeiculos, dataInicial, dataFinal);

            //Filtra veículos que estão dentro do Raio Proximidade.
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculosNoRaio = posicoesVeiculos.Where(veiculo => Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.EmRaioProximidadeLocal(raioProximidadeVeiculo, veiculo.Latitude, veiculo.Longitude)).ToList();

            //Valida quantidade de veículos no raio, durante a faixa de tempo.
            veiculosNoRaio = posicoesVeiculosNoRaio.GroupBy(veiculo => veiculo.CodigoVeiculo).ToList().Count;
            if (veiculosNoRaio > maximoVeiculosNoRaio)
            {
                string texto = $"Concentração de veículos no raio: {veiculosNoRaio}";
                base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, null, texto, false);
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}