using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio
{
    public sealed class CheckListPergunta
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.TipoOpcaoCheckList Tipo { get; set; }

        public int Ordem { get; set; }

        public bool Obrigatorio { get; set; }

        public bool TipoData { get; set; }

        public bool TipoHora { get; set; }

        public bool PermiteNaoAplica { get; set; }

        public dynamic Resposta { get; set; }

        public bool? RespostaSimNao { get; set; }

        public bool? RespostaNaoSeAplica { get; set; }

        public List<CheckListPerguntaAlternativa> Alternativas { get; set; }
    }
}
