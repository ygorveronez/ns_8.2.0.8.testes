namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum VerificarTipoDeCarga
    {
        Todos = 0,
        NaoVerificar = 1,
        Algum = 2,
        Nenhum = 3
    }

    public static class VerificarTipoDeCargaHelper
    {
        public static string ObterDescricao(this VerificarTipoDeCarga verificarTipoDeCarga)
        {
            switch (verificarTipoDeCarga)
            {
                case VerificarTipoDeCarga.Todos: return "Todos";
                case VerificarTipoDeCarga.NaoVerificar: return "Não verificar";
                case VerificarTipoDeCarga.Algum: return "Algum dos tipos";
                case VerificarTipoDeCarga.Nenhum: return "Nenhum dos tipos";
                default: return "";
            }
        }
    }
}
