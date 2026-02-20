namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEntidadeProgramacao
    {
        Todos = 0,
        Veiculo = 1,
        Motorista = 2
    }

    public static class TipoEntidadeProgramacaoPedidoHelper
    {
        public static string Descricao(this TipoEntidadeProgramacao tipoEntidadeProgramacao)
        {
            switch (tipoEntidadeProgramacao)
            {
                case TipoEntidadeProgramacao.Todos:
                    return "Todos";
                case TipoEntidadeProgramacao.Veiculo:
                    return "Veículo";
                case TipoEntidadeProgramacao.Motorista:
                    return "Motorista";
                default:
                    return string.Empty;
            }
        }
    }
}
