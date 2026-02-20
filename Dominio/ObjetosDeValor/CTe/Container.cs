using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.CTe
{
    public class Container
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Numero { get; set; }
        public DateTime? DataPrevista { get; set; }
        public string Lacre1 { get; set; }
        public string Lacre2 { get; set; }
        public string Lacre3 { get; set; }
        public int CodigoContainer { get; set; }
        public List<Documento> Documentos { get; set; }
    }
}
