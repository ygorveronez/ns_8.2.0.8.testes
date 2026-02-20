namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoServicoVeiculo
    {
        Ambos = 0,
        PorKM = 1,
        PorDia = 2,
        PorHorimetro = 3,
        Todos = 4,
        PorHorimetroDia = 5,
        Nenhum = 6
    }

    public static class TipoServicoVeiculoHelper
    {
        public static string ObterDescricao(this TipoServicoVeiculo tipoServicoVeiculo)
        {
            switch (tipoServicoVeiculo)
            {
                case TipoServicoVeiculo.Ambos: return "Por KM e dia";
                case TipoServicoVeiculo.PorKM: return "Por KM";
                case TipoServicoVeiculo.PorDia: return "Por Dia";
                case TipoServicoVeiculo.PorHorimetro: return "Por Horímetro";
                case TipoServicoVeiculo.Todos: return "Todos";
                case TipoServicoVeiculo.PorHorimetroDia: return "Por Horímetro e dia";
                case TipoServicoVeiculo.Nenhum: return "Nenhum";
                default: return string.Empty;
            }
        }
    }
}
