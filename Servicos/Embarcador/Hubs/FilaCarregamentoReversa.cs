namespace Servicos.Embarcador.Hubs
{
    public class FilaCarregamentoReversa : HubBase<FilaCarregamentoReversa>
    {
        public void NotificarTodosFilaAlterada(Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoReversaAlteracao alteracao)
        {
            SendToAll("informarFilaCarregamentoReversaAlterada", alteracao);
        }
    }
}
