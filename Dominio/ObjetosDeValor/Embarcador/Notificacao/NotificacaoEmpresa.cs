using AdminMultisoftware.Dominio.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    public sealed class NotificacaoEmpresa
    {
        public string AssuntoEmail { get; set; }

        public string CabecalhoMensagem { get; set; }

        public int CodigoOrigemNotificacao { get; set; }

        public Entidades.Empresa Empresa { get; set; }

        public string Mensagem { get; set; }

        public bool NotificarSomenteEmailPrincipal { get; set; }

        public TipoServicoMultisoftware TipoServicoMultisoftware { get; set; } = TipoServicoMultisoftware.MultiEmbarcador;

        public List<System.Net.Mail.Attachment> Anexos { get; set; } = null;
    }
}
