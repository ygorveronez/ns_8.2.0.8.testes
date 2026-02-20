using System;

namespace Servicos.Embarcador.HubsMobile
{
    [Obsolete("Classe utilizada apenas para compatibilidade com a fila de carregamento do GPA")]
    sealed class FilaCarregamentoMobile : HubBaseMobile<FilaCarregamentoMobile>
    {
        #region Métodos Públicos

        public void Notificar(string chaveUsuario, string notificacaoEnviar)
        {
            SendToClient(chaveUsuario, "notificarAlteracao", notificacaoEnviar);
        }

        #endregion
    }
}
