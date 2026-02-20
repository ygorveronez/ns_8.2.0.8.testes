using System;

namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    public sealed class NotificacaoEmail
    {
        public DateTime Data { get; set; }

        public string Email { get; set; }

        public string Nota { get; set; }

        public int QuantidadeNotificacoesNaoVisualizadas { get; set; }

        public string Titulo { get; set; }
    }
}
