namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class AreaVeiculoPosicaoQrCode
    {
        public string AreaVeiculo { get; set; }

        public byte[] QRCode { get; set; }

        public string Posicao { get; set; }
    }
}
