using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ProcessarPosicao
    {
        Pendente = 0,
        Processando = 1,
        Processado = 2
    }

    public static class ProcessarPosicaoHelper
    {
        #region Métodos públicos
        public static string ObterDescricao(this ProcessarPosicao processar)
        {
            switch (processar)
            {
                case ProcessarPosicao.Pendente: return "Pendente";
                case ProcessarPosicao.Processando: return "Processando";
                case ProcessarPosicao.Processado: return "Processado";
                default: return string.Empty;
            }
        }

        public static List<dynamic> ObterTodos()
        {
            List<dynamic> list = new List<dynamic>();
            list.Add(ProcessarPosicao.Pendente.ToObject());
            list.Add(ProcessarPosicao.Processando.ToObject());
            list.Add(ProcessarPosicao.Processado.ToObject());
            return list;
        }
        #endregion

        #region Métodos privados
        private static dynamic ToObject(this ProcessarPosicao processar)
        {
            return new
            {
                Codigo = processar,
                Descricao = processar.ObterDescricao()
            };
        }
        #endregion

    }
}