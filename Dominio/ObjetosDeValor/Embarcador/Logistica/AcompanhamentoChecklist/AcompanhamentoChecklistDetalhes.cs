using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class AcompanhamentoChecklistDetalhes
    {
        #region Propriedades

        public EnumRegimeLimpeza RegimeLimpezaCarga { get; set; }

        public OrdemCargaChecklist OrdemChecklist { get; set; }

        public string Placa { get; set; }

        public string Produto { get; set; }

        #endregion

        #region Propriedades com Regras

        public string OrdemChecklistFormatado
        {
            get { return OrdemChecklist.ObterDescricao(); }
        }

        public string RegimeLimpezaCargaFormatado
        {
            get { return RegimeLimpezaCarga.ObterDescricao(); }
        }

        #endregion
    }
}
