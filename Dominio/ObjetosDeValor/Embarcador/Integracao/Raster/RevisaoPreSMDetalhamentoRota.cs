using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class RevisaoSMDetalhamentoRota
    {
        public int CodRota { get; set; }
        public List<RevisaoSMDetalhamentoRotaLocalParada> LocaisParada { get; set; }
        public List<RevisaoSMDetalhamentoRotaPontoPassagem> PontosPassagem { get; set; }
    }
}
