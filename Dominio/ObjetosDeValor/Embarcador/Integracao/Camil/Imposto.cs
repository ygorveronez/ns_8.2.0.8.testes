namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Camil
{
    public class Imposto
    {
        public TipoImposto CodigoImposto { get; set; }
        public string DescricaoImposto { get; set; }
        public decimal AliquotaImposto { get; set; }
        public decimal? ValorBaseImposto { get; set; }
        public decimal ValorImposto { get; set; }
        public decimal PerctReducao { get; set; }
        public string ClassTrib { get; set; }
        public string CST { get; set; }
    }
}
