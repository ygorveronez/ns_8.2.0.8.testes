using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class AtendimentoIniciado : AbstractAlertaCarga
    {
        #region Métodos públicos

        public AtendimentoIniciado(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.AtendimentoIniciado) { }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            return;
        }

        public void ProcessarEvento(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.Chamados.Chamado Chamado)
        {
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(base.unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga>();
            tiposAlerta.Add(base.TipoAlertaCarga);

            if (EstaAtivo() && cargaEntrega != null)
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas = repCargaEvento.BuscarUltimoAlertaPendenteObjetoDeValorPorTiposDeAlertaeCarga(tiposAlerta);

                if (!base.ExisteAlertaAbertoOuFechadoHaPouco(cargaEntrega.Carga.Codigo, alertas))
                {
                    string texto = "Novo atendimento iniciado ";
                    texto += (cargaEntrega.Coleta ? "Coleta: " : "Entrega: ") + (cargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? "") + " Carga:  " + cargaEntrega.Carga.CodigoCargaEmbarcador + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    CriarAlerta(alertas, cargaEntrega.Carga.Codigo, cargaEntrega.Codigo, Chamado.Codigo, texto);
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
