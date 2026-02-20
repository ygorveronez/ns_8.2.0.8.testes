namespace Dominio.ObjetosDeValor.WebService.Entrega
{
    public class MensagemChat
    {
        public UsuarioMensagemChat Usuario { get; set; }
        public Embarcador.NFe.NotaFiscal NotaFiscal { get; set; }
        public string Mensagem { get; set; }
    }

    public class UsuarioMensagemChat
    {
        public string CodigoIntegracao { get; set; }
        public string CPF { get; set; }
        public string Nome { get; set; }
    }
}
