namespace Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR
{
    public class Nota
    {
        public string Registro { get; set; }
        public string Operacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal NotaFiscal { get; set; }

    }
}
