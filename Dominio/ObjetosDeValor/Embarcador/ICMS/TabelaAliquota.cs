namespace Dominio.ObjetosDeValor.Embarcador.ICMS
{
    public class TabelaAliquota
    {
        public Dominio.Entidades.Aliquota Aliquota { get; set; }
        public Dominio.Entidades.Estado UFEmitente { get; set; }
        public Dominio.Entidades.Estado UFInicioPrestacao { get; set; }
        public Dominio.Entidades.Estado UFTerminoPrestacao { get; set; }
        public Dominio.Entidades.Atividade AtividadeTomador { get; set; }
        public Dominio.Entidades.Atividade AtividadeDestinatario { get; set; }
    }
}
