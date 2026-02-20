using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class CargaVeiculoNaoMonitorado : AbstractAlertaCarga
    {
        #region Metodos Publicos

        public CargaVeiculoNaoMonitorado(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.VeiculoNaoMonitorado) { }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            return;
        }


        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ConfiguracaoAlertaCarga.Tempo > 0;
        }


        #endregion

    }
}
