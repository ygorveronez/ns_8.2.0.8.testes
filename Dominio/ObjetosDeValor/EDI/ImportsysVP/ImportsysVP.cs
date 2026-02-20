using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.ImportsysVP
{
    public class ImportsysVP
    {
        public List<ValePedagio> ValePedagios { get; set; }

        public DateTime DataGeracao { get; set; }

        public string NomeArquivo { get; set; }

        public string NomeArquivoSemExtencao { get; set; }
    }
}
