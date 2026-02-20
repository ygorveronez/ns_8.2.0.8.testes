using System;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCustomEventAppTrizy
    {
        Nenhum = 0,
        EstouIndo = 1,
        SolicitacaoDataeHoraCanhoto = 2
    }

    public static class TipoCustomEventAppTrizyHelper
    {
        public static string ObterDescricao(this TipoCustomEventAppTrizy tipo)
        {
            switch (tipo)
            {
                case TipoCustomEventAppTrizy.EstouIndo: return "Estou indo";
                case TipoCustomEventAppTrizy.SolicitacaoDataeHoraCanhoto: return "Solicitação de data e hora do canhoto";
                default: return string.Empty;
            }
        }

        public static TipoCustomEventAppTrizy ObterEnumerador(object customEvent)
        {
            if (customEvent is string descricao)
                return ObterTipoPorDescricao(descricao);

            if (customEvent is TipoCustomEventAppTrizy tipo)
                return tipo;

            if (customEvent is int codigo)
                return (TipoCustomEventAppTrizy)codigo;

            return TipoCustomEventAppTrizy.Nenhum;
        }

        public static TipoCustomEventAppTrizy ObterTipoPorDescricao(string descricao)
        {
            var campos = typeof(TipoCustomEventAppTrizy).GetFields();

            foreach (var campo in campos)
            {
                if (string.Equals(campo.Name, descricao, StringComparison.OrdinalIgnoreCase))
                    return (TipoCustomEventAppTrizy)campo.GetValue(null);
            }

            return TipoCustomEventAppTrizy.Nenhum;
        }
    }
}
