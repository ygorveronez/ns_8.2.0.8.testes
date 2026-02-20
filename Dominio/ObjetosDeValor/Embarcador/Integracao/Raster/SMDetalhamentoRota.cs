using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class SMDetalhamentoRota
    {
        public int CodRota { get; set; }
        public List<SMDetalhamentoRotaLocalParada> LocaisParada { get; set; }
        public List<SMDetalhamentoRotaPontoPassagem> PontosPassagem { get; set; }
    }
}
