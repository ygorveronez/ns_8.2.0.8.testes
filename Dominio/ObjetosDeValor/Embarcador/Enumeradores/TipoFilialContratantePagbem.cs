namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFilialContratantePagbem
    {
        Empresa = 0,
        GrupoPessoas = 1
    }

    public static class TipoFilialContratantePagbemHelper
    {
        public static string ObterDescricao(this TipoFilialContratantePagbem tipoFilialContratantePagbem)
        {
            switch (tipoFilialContratantePagbem)
            {
                case TipoFilialContratantePagbem.Empresa: return "Empresa";
                case TipoFilialContratantePagbem.GrupoPessoas: return "Grupo de Pessoas";
                default: return string.Empty;
            }
        }
    }
}
