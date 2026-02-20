using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.ImportsysCTe
{
    public class ImportsysCTe
    {
        public List<Conhecimento> Conhecimentos { get; set; }

        public DateTime DataGeracao { get; set; }

        public string NomeArquivo { get; set; }

        public string NomeArquivoSemExtencao { get; set; }
    }
}
