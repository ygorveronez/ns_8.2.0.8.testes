namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public sealed class VeiculoQrCode
    {
        public byte[] QRCode { get; set; }

        public string ModeloVeicularCarga { get; set; }

        public string Placa { get; set; }

        public string Transportador { get; set; }
    }
}
