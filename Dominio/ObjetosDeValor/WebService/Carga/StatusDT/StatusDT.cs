namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class StatusDT
    {
        public string dtNumber { get; set; }
        public string dtSAPFlag { get; set; }

        /// <summary>
        /// O Tipo ainda está indefinido, pois passamos somente como null na integração
        /// </summary>
        public dynamic OcorrbLiveFlag { get; set; }
        public decimal? dtPesoLiquido { get; set; }
    }
}
