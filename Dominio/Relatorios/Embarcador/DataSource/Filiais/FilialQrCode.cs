namespace Dominio.Relatorios.Embarcador.DataSource.Filiais
{
    public sealed class FilialQrCode
    {
        public string Etapa { get; set; }

        public string Filial { get; set; }

        public byte[] QRCode { get; set; }
    }
}
