namespace Dominio.ObjetosDeValor
{
    public class RetornoVerificacaoCliente
    {
        public bool Status { get; set; }

        public Dominio.Entidades.Cliente cliente { get; set; }
        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco { get; set; }

        public string Mensagem { get; set; }

        public bool UsarOutroEndereco { get; set; }

        public string CodigoOutroEndereco { get; set; }
        public string CodigoDocumentoAnterior { get; set; }
        public string NovoCodigoDocumento { get; set; }
    }
}
