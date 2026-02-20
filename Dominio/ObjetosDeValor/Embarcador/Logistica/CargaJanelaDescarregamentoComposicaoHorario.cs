using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class CargaJanelaDescarregamentoComposicaoHorario
    {
        #region Propriedades

        public List<string> Detalhes { get; set; }

        #endregion Propriedades

        #region Construtores

        public CargaJanelaDescarregamentoComposicaoHorario()
        {
            Detalhes = new List<string>();
        }

        #endregion Construtores
    }
}
