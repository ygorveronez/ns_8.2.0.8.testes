using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete.Consulta
{
    public class HistoricoParametroBaseTabelaFrete
    {
        public long CodigoTabelaFrete { get; set; }
        public DateTime DataHistorico { get; set; }
        public string DescricaoParametroBaseDe { get; set; }
        public string DescricaoParametroBasePara { get; set; }
    }
}
