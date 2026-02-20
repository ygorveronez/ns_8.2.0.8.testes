using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class FimViagem : AbstractAlertaCarga
    {
        #region Métodos públicos

        public FimViagem(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.FimViagem) { }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            return;
        }

        public void ProcessarEvento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return;

            //Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new TorreControle.AlertaAcompanhamentoCarga(this.unitOfWork);
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(base.unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga>();
            tiposAlerta.Add(base.TipoAlertaCarga);

            if (EstaAtivo() && carga.DataFimViagem.HasValue)
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas = repCargaEvento.BuscarUltimoAlertaPendenteObjetoDeValorPorTiposDeAlertaeCarga(tiposAlerta);

                if (!base.ExisteAlertaAbertoOuFechadoHaPouco(carga.Codigo, alertas))
                {
                    string texto = "Carga " + carga.CodigoCargaEmbarcador + " Fim viagem em " + carga.DataFimViagem.Value.ToString("dd/MM/yyyy HH:mm:ss");
                    CriarAlerta(alertas, carga.Codigo, 0, 0, texto);
                }
            }
            //else
            //    servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargaAcompanhamento(carga);
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
