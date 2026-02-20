using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumContatosInformacoesEntregaTrizy
    {
        TelefoneTorre = 1,
        TelefoneDoCliente = 2,
    }

    public static class EnumContatosInformacoesEntregaTrizyHelper
    {
        public static string ObterDescricao(this EnumContatosInformacoesEntregaTrizy enumContatosInformacoesEntregaTrizy)
        {
            switch (enumContatosInformacoesEntregaTrizy)
            {
                case EnumContatosInformacoesEntregaTrizy.TelefoneTorre: return Localization.Resources.Pedidos.TipoOperacao.TelefoneTorre;
                case EnumContatosInformacoesEntregaTrizy.TelefoneDoCliente: return Localization.Resources.Pedidos.TipoOperacao.TelefoneDoCliente;
                default: return string.Empty;
            }
        }
    }
}
