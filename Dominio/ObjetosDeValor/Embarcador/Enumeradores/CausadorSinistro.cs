namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CausadorSinistro
    {
        Todos = 0,
        VeiculoProprio = 1,
        VeiculoTerceiro = 2,
        NaoIdentificado = 3,
        Outros = 4
    }
    public static class CausadorSinistroHelper
    {
        public static string ObterDescricao(this CausadorSinistro causadorSinistro)
        {
            switch (causadorSinistro)
            {
                case CausadorSinistro.Todos: return "Todos";
                case CausadorSinistro.VeiculoProprio: return "Veículo Próprio";
                case CausadorSinistro.VeiculoTerceiro: return "Veículo Terceiro";
                case CausadorSinistro.NaoIdentificado: return "Não Identificado";
                case CausadorSinistro.Outros: return "Outros";
                default: return string.Empty;
            }
        }
    }
}
