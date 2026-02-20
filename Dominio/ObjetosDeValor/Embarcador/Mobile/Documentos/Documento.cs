namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos
{
    public class Documento
    {
        public int CodigoIntegracao { get; set; }
        public int Numero { get; set; }
        public string NumeroNF { get; set; }
        public int Serie { get; set; }
        public string NumeroPedido { get; set; }
        public int OrdemEntrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga Carga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Emitente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }

        public bool Coleta { get; set; }
    }
}
