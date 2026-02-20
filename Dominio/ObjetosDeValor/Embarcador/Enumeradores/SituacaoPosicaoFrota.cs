using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPosicaoFrota
    {
        EmViagem = 1,
        SemViagem = 2
    }

    public static class SituacaoPosicaoFrotaHelper
    {
        #region Métodos públicos
        public static string ObterCorFonte(this SituacaoPosicaoFrota situacao)
        {
            switch (situacao)
            {
                case SituacaoPosicaoFrota.EmViagem: return "#85de7b";
                case SituacaoPosicaoFrota.SemViagem: return "#ffe699";
                default: return string.Empty;
            }
        }

        public static string ObterCorLinha(this SituacaoPosicaoFrota situacao)
        {
            switch (situacao)
            {
                case SituacaoPosicaoFrota.EmViagem: return "#008e83";
                case SituacaoPosicaoFrota.SemViagem: return "#e6ac00";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoPosicaoFrota situacao)
        {
            switch (situacao)
            {
                case SituacaoPosicaoFrota.EmViagem: return "Em viagem";
                case SituacaoPosicaoFrota.SemViagem: return "Sem viagem";
                default: return string.Empty;
            }
        }

        public static List<dynamic> ObterTodos()
        {
            List<dynamic> list = new List<dynamic>();
            list.Add(SituacaoPosicaoFrota.EmViagem.ToObject());
            list.Add(SituacaoPosicaoFrota.SemViagem.ToObject());
            return list;
        }
        #endregion

        #region Métodos privados
        private static dynamic ToObject(this SituacaoPosicaoFrota situacao)
        {
            return new
            {
                Codigo = situacao,
                Descricao = situacao.ObterDescricao(),
                CorFonte = situacao.ObterCorFonte(),
                CorLinha = situacao.ObterCorLinha()
            };
        }
        #endregion

    }
}
