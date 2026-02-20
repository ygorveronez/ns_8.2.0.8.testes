using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class DadosIntegracaoEDISumarizados
    {
        public string NumeroDT { get; set; }
        public List<string> NumerosDT { get; set; }
        public string IDDOC { get; set; }
        public string Filial { get; set; }
        public string MeioTransporte { get; set; }
        public string ModeloVeicular { get; set; }
        public string Placa { get; set; }
        public int QuantidadeNfs { get; set; }
        public int Empresa { get; set; }
        public string Roteiro { get; set; }
        
    }
}
