namespace Dominio.ObjetosDeValor.Relatorios
{
    public class Relatorio
    {
        public Relatorio(byte[] arquivo, string mimeType, string fileNameExtension)
        {
            this.Arquivo = arquivo;
            this.FileNameExtension = fileNameExtension;
            this.MimeType = mimeType;
        }

        public byte[] Arquivo { get; set; }

        public string MimeType { get; set; }

        public string FileNameExtension { get; set; }
    }
}
