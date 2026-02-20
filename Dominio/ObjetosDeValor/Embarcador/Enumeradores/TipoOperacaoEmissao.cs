namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOperacaoEmissao
    {
        todos = 0,
        VendaNormal = 1,
        EntregaArmazem = 2,
        VendaArmazemCliente = 3,
        VendaComRedespacho = 4
        //    ,
        //VendaTriangular = 5
    }

    public static class TipoOperacaoEmissaoDescricao
    {
        public static string RetornarDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao tipoOperacaoEmissao)
        {

            if (tipoOperacaoEmissao == TipoOperacaoEmissao.VendaNormal)
                return "Venda Normal";

            if (tipoOperacaoEmissao == TipoOperacaoEmissao.VendaArmazemCliente)
                return "Venda Armazém Cliente";

            if (tipoOperacaoEmissao == TipoOperacaoEmissao.EntregaArmazem)
                return "Entrega no Armazém";

            if (tipoOperacaoEmissao == TipoOperacaoEmissao.VendaComRedespacho)
                return "Venda com Redespacho";

            //if (tipoOperacaoEmissao == TipoOperacaoEmissao.VendaTriangular)
            //    return "Venda Triangular";

            return "";
        }
    }
}
