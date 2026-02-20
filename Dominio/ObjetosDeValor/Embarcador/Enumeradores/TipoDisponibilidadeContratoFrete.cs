namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDisponibilidadeContratoFrete
    {
        TracaoComOuSemReboque = 0,
        TodosVeiculos = 1,
        Tracao = 2,
        Reboque = 3,
        TracaoComCarroceria = 4
    }

    public static class TipoDisponibilidadeContratoFreteHelper
    {
        public static string ObterDescricao(this TipoDisponibilidadeContratoFrete situacao)
        {
            switch (situacao)
            {
                case TipoDisponibilidadeContratoFrete.TracaoComOuSemReboque: return "Tração com/sem Reboque";
                case TipoDisponibilidadeContratoFrete.TodosVeiculos: return "Todos Veículos";
                case TipoDisponibilidadeContratoFrete.Tracao: return "Tração";
                case TipoDisponibilidadeContratoFrete.Reboque: return "Reboque";
                case TipoDisponibilidadeContratoFrete.TracaoComCarroceria: return "Tração com Carroceria";
                default: return string.Empty;
            }
        }
    }
}
