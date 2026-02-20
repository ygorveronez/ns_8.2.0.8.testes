using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.RegraAutorizacao
{
    public sealed class AprovacaoAlcadaSemPadronizacao
    {
        #region Métodos Públicos

        public bool ValidarAlcadas<TAlcadaSemPadronizacao>(IList<TAlcadaSemPadronizacao> alcadas, object valorComparar) where TAlcadaSemPadronizacao : Dominio.Entidades.Embarcador.RegraAutorizacao.AlcadaSemPadronizacao
        {
            if (alcadas.ToList().All(alcada => alcada.IsCondicaoVerdadeira(valorComparar) && alcada.IsJuncaoTodasVerdadeiras()))
                return true;

            if (alcadas.ToList().Any(alcada => alcada.IsCondicaoVerdadeira(valorComparar) && !alcada.IsJuncaoTodasVerdadeiras()))
                return true;

            return false;
        }

        #endregion
    }
}
