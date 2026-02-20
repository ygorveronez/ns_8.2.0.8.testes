namespace Dominio.ObjetosDeValor.WebService.NFe
{
    public class ConsultaSefaz
    {
        public bool ConsultaValida { get; set; }
        public string MensagemSefaz { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal NotaFiscal { get; set; }
    }
}
