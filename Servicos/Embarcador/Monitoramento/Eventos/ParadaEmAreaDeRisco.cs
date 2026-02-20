using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class ParadaEmAreaDeRisco : AbstractEvento
    {

        #region Atributos públicos

        public Dominio.Entidades.Embarcador.Logistica.Locais[] LocaisAreaDeRisco { get; set; }

        #endregion

        #region Métodos públicos

        public ParadaEmAreaDeRisco(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaEmAreaDeRisco)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Tempo != null);
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {

            // Não está em um raio de cliente e está parado 
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.LatitudePosicao != null && monitoramentoProcessarEvento.LongitudePosicao != null && !EmAlvoEntrega(codigosClientesAlvo, entregas) && monitoramentoProcessarEvento.VelocidadePosicao == 0)
            {

                // Garante que não está em uma área de risco ou área de pernoite
                Dominio.Entidades.Embarcador.Logistica.Locais localAreaDeRisco = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarLocalEmArea(LocaisAreaDeRisco, monitoramentoProcessarEvento.LatitudePosicao.Value, monitoramentoProcessarEvento.LongitudePosicao.Value);
                if (localAreaDeRisco != null)
                {

                    // Cria o alerta para a carga se não existir algum
                    base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento?.Carga ?? null, entregas, localAreaDeRisco.Descricao);

                }

            }

        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}