var EnumTipoOperacaoEmissao = {
    todas: 0,
    vendaNormal : 1,
    EntregaArmazem : 2,
    VendaArmazemCliente : 3,
    VendaComRedespacho : 4
};

function EnumTipoOperacaoEmissaoDescricao(tipoOperacaoEmissao) {

    if (tipoOperacaoEmissao == EnumTipoOperacaoEmissao.todas)
        return "Todas";

    if (tipoOperacaoEmissao == EnumTipoOperacaoEmissao.vendaNormal)
        return "Venda Normal";

    if (tipoOperacaoEmissao == EnumTipoOperacaoEmissao.VendaArmazemCliente)
        return "Venda Armazém Cliente";

    if (tipoOperacaoEmissao == EnumTipoOperacaoEmissao.EntregaArmazem)
        return "Entrega no Armazém";

    if (tipoOperacaoEmissao == EnumTipoOperacaoEmissao.VendaComRedespacho)
        return "Venda com Redespacho";

    return "";
}