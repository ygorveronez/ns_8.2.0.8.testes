namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao
{
    public sealed class Notificacao
    {
        public string Assunto { get; set; }

        public int CodigoCentroCarregamento { get; set; }

        public string Data { get; set; }

        public string DescricaoCentroCarregamento { get; set; }

        public string Mensagem { get; set; }
    }
}
