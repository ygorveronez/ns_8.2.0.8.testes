namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ServicoVeiculoExecutado
    {
        NaoDefinido = 0,
        Executado = 1,
        NaoExecutado = 2
    }

    public static class ServicoVeiculoExecutadoHelper
    {
        public static string ObterDescricao(this ServicoVeiculoExecutado servicoVeiculoExecutado)
        {
            switch (servicoVeiculoExecutado)
            {
                case ServicoVeiculoExecutado.Executado: return "Executado";
                case ServicoVeiculoExecutado.NaoExecutado: return "Não Executado";
                default: return string.Empty;
            }
        }
    }
}
