using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class FiltroRelatorioComponenteFreteCTe
    {
        public int Empresa { get; set; }
        public List<int> ModeloDocumento { get; set; }
        public List<string> StatusCTe { get; set; }
        public List<int> GrupoPessoas { get; set; }
        public List<int> ComponenteFrete { get; set; }
        public int Carga { get; set; }
        public int CTe { get; set; }
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public DateTime DataInicialAutorizacao { get; set; }
        public DateTime DataFinalAutorizacao { get; set; }
    }
}
