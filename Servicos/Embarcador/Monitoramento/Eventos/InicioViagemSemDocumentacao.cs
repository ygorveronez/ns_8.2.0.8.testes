using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class InicioViagemSemDocumentacao : AbstractEvento
    {


        #region Atributos públicos

        #endregion

        #region Métodos públicos

        public InicioViagemSemDocumentacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.InicioViagemSemDocumentacao)
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
            // Apenas verificar a situacao da carga, caso a carga esta nos status descritos abaixo deve gerar um evento de alerta
            if (monitoramento != null && monitoramento.Carga != null)
            {
                // Esta em uma situacao Sem Documentacao?
                if (EstaComStatusSemDocumentacao(monitoramento.Carga))
                {
                    // Cria o alerta para a carga se não existir algum
                    string texto = "Carga: " + monitoramento.Carga.CodigoCargaEmbarcador + " em trânsito sem documentação emitida";
                    base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, entregas, texto);
                }
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

        #region Métodos privados
        private bool EstaComStatusSemDocumentacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return carga.SituacaoCarga.IsSituacaoCargaSemDocumentacao();
        }

        #endregion


    }
}