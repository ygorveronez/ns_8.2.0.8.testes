using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public sealed class CargaEntregaComposicaoPrevisao
    {
        #region Propriedades

        public List<string> DetalhesDataFimEntregaPrevista { get; set; }

        public List<string> DetalhesDataInicioEntregaPrevista { get; set; }

        #endregion Propriedades

        #region Construtores

        public CargaEntregaComposicaoPrevisao()
        {
            DetalhesDataInicioEntregaPrevista = new List<string>();
            DetalhesDataFimEntregaPrevista = new List<string>();
        }

        #endregion Construtores
    }
}
