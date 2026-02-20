namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos
{
    public sealed class MotivoAtendimento
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public Enumeradores.TipoMotivoAtendimento Tipo { get; set; }
        public bool ExigirQrCode { get; set; }
        public bool ExigirFoto { get; set; }
    }
}
