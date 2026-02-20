using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.GTA
{
    public class Pergunta
    {
        public int CodigoIntegracao { get; set; }
        public string TipoDescricao { get; set; }
        public int TipoCodigo { get; set; }
        public string Descricao { get; set; }
        public string RespostaDescricao { get; set; }
        public List<int> RespostaCodigosIntegracao { get; set; }
    }
}
