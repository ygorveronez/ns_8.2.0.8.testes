using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class CargaAlertaTendenciaEntregaColeta : AbstractAlertaCarga
    {
        #region Métodos públicos

        public CargaAlertaTendenciaEntregaColeta(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.AtrasoColetaDescarga) { }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            return;
        }

        //metodo chamado por uma thread desvinculada a thread de alertas da carga. (thread ControleTendenciaEntrega)
        public void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> EntregasGerarAlerta, bool coleta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega tendenciaEntrega)
        {
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(base.unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga>();
            tiposAlerta.Add(base.TipoAlertaCarga);

            if (EstaAtivo())
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas = repCargaEvento.BuscarUltimoAlertaPendenteObjetoDeValorPorTiposDeAlertaeCarga(tiposAlerta);

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega cargaAlertar in EntregasGerarAlerta)
                {
                    if (!base.ExisteAlertaAbertoOuFechadoHaPouco(cargaAlertar.CodCarga, alertas))
                    {
                        string texto = $"Carga " + cargaAlertar.CodCargaEmbarcador + (coleta ? " Coleta: " + cargaAlertar.CPF_CNPJ : " Entrega: " + cargaAlertar.CPF_CNPJ) + " Tendência: " + Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(tendenciaEntrega)
                            + " Data prevista: " + cargaAlertar.DataEntregaPrevista.ToString("dd/MM/yyyy HH:mm:ss") + " Data reprogramada: " + cargaAlertar.DataEntregaReprogramada.ToString("dd/MM/yyyy HH:mm:ss");
                        CriarAlerta(alertas, cargaAlertar.CodCarga, cargaAlertar.CodCargaEntrega,0, texto);
                    }
                }
            }
        }

        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ConfiguracaoAlertaCarga.Tempo > 0;
        }

        #endregion


        #region Metodos Privados

        #endregion
    }
}
