namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RetornoCargaTipo
    {
        Carregado = 1,
        Vazio = 2,
        Devolucao = 3,
    }

    public static class RetornoCargaTipoHelper
    {
        public static string ObterDescricao(this RetornoCargaTipo retornoCargaTipo)
        {
            switch (retornoCargaTipo)
            {
                case RetornoCargaTipo.Carregado:
                    return "Carregado";
                case RetornoCargaTipo.Vazio:
                    return "Vazio";
                case RetornoCargaTipo.Devolucao:
                    return "Devolução";
                default:
                    return "";
            }
        }
    }
}
