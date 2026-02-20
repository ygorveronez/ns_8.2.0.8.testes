namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public class ValePallet
    {
        public int Codigo { get; set; }
        public string NomeArquivo { get; set; }
        public byte[] PDF { get; set; }
        public bool Processado { get; set; }
        public string Mensagem { get; set; }

    }
}
