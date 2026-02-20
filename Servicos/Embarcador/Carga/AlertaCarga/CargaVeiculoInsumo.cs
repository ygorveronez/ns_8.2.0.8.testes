using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class CargaVeiculoInsumo : AbstractAlertaCarga
    {
        #region Metodos Privados

        public CargaVeiculoInsumo(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.VeiculoComInsumos)
        {

        }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            return;
        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
      */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo();
        }

        #endregion
    }
}
