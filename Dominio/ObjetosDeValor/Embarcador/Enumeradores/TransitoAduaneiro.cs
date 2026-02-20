using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TransitoAduaneiro
    {
        Sim = 0,
        Nao = 1,
        Nenhum = 2
    }

    public static class TransitoAduaneiroHelper
    {
        public static string ObterDescricao(this TransitoAduaneiro tipo)
        {
            switch (tipo)
            {
                case TransitoAduaneiro.Sim: return Localization.Resources.Gerais.Geral.Sim;
                case TransitoAduaneiro.Nao: return Localization.Resources.Gerais.Geral.Nao;
                case TransitoAduaneiro.Nenhum: return Localization.Resources.Gerais.Geral.Nenhum;
                default: return string.Empty;
            }
        }
    }
}
