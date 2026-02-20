using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class ConfirmacaoColetaEntrega : AbstractAlertaCarga
    {
        #region Métodos públicos

        public ConfirmacaoColetaEntrega(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.ConfirmacaoColetaEntrega) { }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            return;
        }

        public void ProcessarEvento(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            if (cargaEntrega == null)
                return;

            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new TorreControle.AlertaAcompanhamentoCarga(this.unitOfWork);
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(base.unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga>();
            tiposAlerta.Add(base.TipoAlertaCarga);

            if (EstaAtivo() && cargaEntrega.DataConfirmacao.HasValue)
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas = repCargaEvento.BuscarUltimoAlertaPendenteObjetoDeValorPorTiposDeAlertaeCarga(tiposAlerta);

                if (!base.ExisteAlertaAbertoOuFechadoHaPouco(cargaEntrega.Carga.Codigo, alertas))
                {
                    string texto = (cargaEntrega.Coleta ? "Coleta: " : "Entrega: ") + (cargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? "") + " confirmada em " + cargaEntrega.DataConfirmacao.Value.ToString("dd/MM/yyyy HH:mm:ss");
                    CriarAlerta(alertas, cargaEntrega.Carga.Codigo, cargaEntrega.Codigo, 0, texto);
                }
            }
            //else
            //    servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargaAcompanhamento(cargaEntrega.Carga);
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
