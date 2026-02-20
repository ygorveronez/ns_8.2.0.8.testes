using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class TipoCargaEmbarcador
    {
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public List<string> CNPJsDoTipoCargaNoEmbarcador { get; set; }

        public string ClasseONU { get; set; }
        public string SequenciaONU { get; set; }
        public string CodigoPSNONU { get; set; }
        public string ObservacaoONU { get; set; }
        public int Codigo { get; set; }

    }
}
