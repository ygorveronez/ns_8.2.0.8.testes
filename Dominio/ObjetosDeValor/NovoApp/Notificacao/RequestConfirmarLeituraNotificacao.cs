using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NovoApp.Notificacao
{
    public class RequestConfirmarLeituraNotificacao
    {
        public List<int> codigosNotificacaoes { get; set; }
        public bool? lida;
    }
}
