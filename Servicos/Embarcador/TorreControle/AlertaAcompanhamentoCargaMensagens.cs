namespace Servicos.Embarcador.TorreControle
{
    public class AlertaAcompanhamentoCargaMensagens
    {
        public AlertaAcompanhamentoCargaMensagens() { }

        public void informarAtualizacaoMensagensCard()
        {
            Servicos.SignalR.Hubs.AcompanhamentoCarga hubAcompanhamentoCarga = new SignalR.Hubs.AcompanhamentoCarga();
            hubAcompanhamentoCarga.InformarMensagensAtualizadas();

            // Servicos.Log.TratarErro("Informar atualizacao Mensagens Card", "MensagemChat");
        }
    }
}
