
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class CheckList
    {
        public string Descricao { get; set; }

        public List<CheckListPergunta> Perguntas { get; set; }

    }
}
