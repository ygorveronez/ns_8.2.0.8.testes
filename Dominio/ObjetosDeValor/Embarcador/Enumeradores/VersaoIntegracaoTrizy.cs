using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum VersaoIntegracaoTrizy
    {
        Versao1 = 1,
        Versao3 = 3,
    }

    public static class VersaoIntegracaoTrizyHelper
    {
        public static string ObterDescricao(this VersaoIntegracaoTrizy data)
        {
            switch (data)
            {
                case VersaoIntegracaoTrizy.Versao1: return "Versão 1";
                case VersaoIntegracaoTrizy.Versao3: return "Versão 3";
                default: return "";
            }
        }
        public static string ObterDescricaoRota(this VersaoIntegracaoTrizy data)
        {
            switch (data)
            {
                case VersaoIntegracaoTrizy.Versao1: return "v1";
                case VersaoIntegracaoTrizy.Versao3: return "v3";
                default: return "";
            }
        }
    }
}
