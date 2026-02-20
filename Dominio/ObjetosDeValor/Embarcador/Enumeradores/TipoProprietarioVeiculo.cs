namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoProprietarioVeiculo
    {
        TACAgregado = 0,
        TACIndependente = 1,
        Outros = 2,
        NaoAplicado = 3,
        Todos = 4
    }

    public static class TipoProprietarioVeiculoHelper
    {
        public static string ObterDescricao(this TipoProprietarioVeiculo situacao)
        {
            switch (situacao)
            {
                case TipoProprietarioVeiculo.TACAgregado: return "TAC Agregado";
                case TipoProprietarioVeiculo.TACIndependente: return "TAC Independente";
                case TipoProprietarioVeiculo.Outros: return "Outros";
                case TipoProprietarioVeiculo.NaoAplicado: return "Não Aplicado";
                case TipoProprietarioVeiculo.Todos: return "Todos";

                default: return null;
            }
        }
    }
}