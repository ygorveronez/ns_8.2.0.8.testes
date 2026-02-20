namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoInclusaoImpostoComplemento
    {
        ConformeDocumentoAnterior = 0,
        SempreIncluir = 1,
        NuncaIncluir = 2
    }

    public static class TipoInclusaoImpostoComplementoHelper
    {
        public static string ObterDescricao(this TipoInclusaoImpostoComplemento tipo)
        {
            switch (tipo)
            {
                case TipoInclusaoImpostoComplemento.ConformeDocumentoAnterior: return "Conf. Documento Anterior";
                case TipoInclusaoImpostoComplemento.SempreIncluir: return "Sempre Incluir";
                case TipoInclusaoImpostoComplemento.NuncaIncluir: return "Nunca Incluir";
                default: return "Conf.CTe Anterior";
            }
        }
    }
}
