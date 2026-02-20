using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoServidorEmail
    {
        Outlook = 0,
        Gmail = 1,
        Outro = 2
    }

    public static class TipoServidorEmailHelper
    {
        public static string ObterDescricao(this TipoServidorEmail tipo)
        {
            switch (tipo)
            {
                case TipoServidorEmail.Outlook: return "Outlook";
                case TipoServidorEmail.Gmail: return "Gmail";
                case TipoServidorEmail.Outro: return "Outro";
                default: return string.Empty;
            }
        }
    }
}
