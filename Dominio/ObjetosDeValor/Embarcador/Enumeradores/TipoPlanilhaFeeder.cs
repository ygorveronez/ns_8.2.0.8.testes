
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{ 
    public enum TipoPlanilhaFeeder
    {
        Todas = 0,
        Feeder = 1,
        Subcontratacao = 2,
        SubcontratacaoTipoUm = 3,
    }

    public static class TipoPlanilhaFeederHelper
    {
        public static string ObterDescricao(this TipoPlanilhaFeeder situacao)
        {
            switch (situacao)
            {
                case TipoPlanilhaFeeder.Todas:
                    return "Todas";
                case TipoPlanilhaFeeder.Feeder:
                    return "Feeder";
                case TipoPlanilhaFeeder.Subcontratacao:
                    return "Subcontratacao";
                case TipoPlanilhaFeeder.SubcontratacaoTipoUm:
                    return "Subcontratacao";
                default:
                    return string.Empty;
            }
        }
    }
}
