namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class FiltroConsultaCTeTerceiro
    {
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public Dominio.Enumeradores.TipoCTE? TipoCTe { get; set; }
        public double CPFCNPJEmitente { get; set; }
        public double CPFCNPJRemetente { get; set; }
        public double CPFCNPJDestinatario { get; set; }
        public bool PossuiOcorrenciaGerada { get; set; }
    }
}
