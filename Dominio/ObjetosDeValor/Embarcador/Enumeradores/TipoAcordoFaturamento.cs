namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAcordoFaturamento
    {
        NaoInformado = 1,
        FreteLongoCurso = 2,
        CustoExtra = 3
    }

    public static class TipoAcordoFaturamentoHelper
    {
        public static string ObterDescricao(this TipoAcordoFaturamento etapa)
        {
            switch (etapa)
            {
                case TipoAcordoFaturamento.NaoInformado: return "Não Informado";
                case TipoAcordoFaturamento.FreteLongoCurso: return "Frete Longo Curso";
                case TipoAcordoFaturamento.CustoExtra: return "Custo Extra";
                default: return string.Empty;
            }
        }
    }
}
