using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio
{
    public sealed class CheckList
    {
        public int Codigo { get; set; }

        public string Assunto { get; set; }

        public List<CheckListPergunta> Perguntas { get; set; }
    }
}
