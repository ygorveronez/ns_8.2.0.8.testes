var EnumTipoRegraNaoConformidadeHelper = function () {
    this.Todos = "",
    this.ExisteXmlNota = 1,
    this.CargaCancelada = 2,
    this.ValidarRaiz = 3,
    this.ValidarCNPJ = 4,
    this.StatusSefaz = 5,
    this.SituacaoCadastral = 6,
    this.EstendidoFilial = 7,
    this.Nacionalizacao = 8,
    this.TipoTomador = 11,
    this.Tomador = 12,
    this.RecebedorNotaFiscal = 15,
    this.LocalEntrega = 17,
    this.RecebedorArmazenagem = 18,
    this.Transportador = 19,
    this.NumeroPedido = 20,
    this.QuantidadePedido = 21,
    this.PesoLiquidoTotal = 22,
    this.ProdutoDePara = 23,
    this.ProdutoSituacao = 24,
    this.ProdutoFilial = 25,
    this.ProdutoFilialRecebedor = 26,
    this.ProdutoConversaoUnidade = 27,
    this.CapacidadeExcedida = 28,
    this.Produto = 29
}

EnumTipoRegraNaoConformidadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "Existe XML da nota", value: this.ExisteXmlNota},
            { text: "Carga cancelada", value: this.CargaCancelada },
            { text: "Validar raiz do CNPJ", value: this.ValidarRaiz },
            { text: "Validar CNPJ", value: this.ValidarCNPJ },
            { text: "Status da nota", value: this.StatusSefaz },
            { text: "Situação cadastral", value: this.SituacaoCadastral },
            { text: "Estendido Filial", value: this.EstendidoFilial },
            { text: "Nacionalização", value: this.Nacionalizacao },
            { text: "Tipo do tomador", value: this.TipoTomador },
            { text: "Tomador", value: this.Tomador },
            { text: "Recebedor notas fiscais", value: this.RecebedorNotaFiscal },
            { text: "Local de entrega", value: this.LocalEntrega },
            { text: "Recebedor armazenagem", value: this.RecebedorArmazenagem },
            { text: "Transportador", value: this.Transportador },
            { text: "Número do pedido", value: this.NumeroPedido },
            { text: "Quantidade de pedidos", value: this.QuantidadePedido },
            { text: "Peso líquido total", value: this.PesoLiquidoTotal },
            { text: "Produto De/Para", value: this.ProdutoDePara },
            { text: "Produto situação", value: this.ProdutoSituacao },
            { text: "Produto filial", value: this.ProdutoFilial },
            { text: "Produto filial recebedor", value: this.ProdutoFilialRecebedor },
            { text: "Produto conversão de unidade", value: this.ProdutoConversaoUnidade },
            { text: "Capacidade excedida", value: this.CapacidadeExcedida },
            { text: "Produto", value: this.Produto }
        ];
    },


    obterOpcoesTexto: function (tipoRegra, participante) {

        let descricaoRegra = [
            { text: "Não definida", value: this.NaoDefinido },
            { text: "Valida verificando se existe o XML da nota fiscal, caso possuir, adiciona o mesmo a uma carga sem nota fiscal", value: this.ExisteXmlNota },
            { text: "Não definida.", value: this.CargaCancelada },
            { text: "Valida o CNPJ da Raiz do participante selecionado da nota fiscal tag com o CNPJ do participante constante na remessa (pedido).", value: this.ValidarRaiz },
            { text: "Valida o CNPJ do participante selecionado da nota fiscal com o CNPJ do participante constante na remessa (pedido).", value: this.ValidarCNPJ },
            { text: "Valida o status da nota no Sefaz", value: this.StatusSefaz },
            { text: "Valida se o participante selecionado da nota fiscal está ativo", value: this.SituacaoCadastral },
            { text: "Valida o CNPJ do participante selecionado da nota fiscal com o CNPJ da filial da carga.", value: this.EstendidoFilial },
            { text: "Valida se o participante constante na nota fiscal é do exterior", value: this.Nacionalizacao },
            { text: "Valida o tipo tomador presente na nota fiscal com o tipo tomador constante na remessa (pedido).", value: this.TipoTomador },
            { text: "Valida o CNPJ do tomador presente na nota fiscal com o tomador constante na remessa (pedido).", value: this.Tomador },
            { text: "Valida o Recebedor da nota fiscal com o recebedor do pedido", value: this.RecebedorNotaFiscal },
            { text: "Valida se existe um recebedor presente na nota fiscal", value: this.LocalEntrega },
            { text: "Valida a raiz do CNPJ do recebedor da nota fiscal com o CNPJ do recebedor constante na remessa (pedido)", value: this.RecebedorArmazenagem },
            { text: "Valida o transportador presente na nota fiscal com o transportador constante na Carga.", value: this.Transportador },
            { text: "Valida o número de ordem do pedido presente na nota fiscal com o número do pedido constante na remessa.", value: this.NumeroPedido },
            { text: "Não definida", value: this.QuantidadePedido },
            { text: "Valida o peso líquido que consta na nota fiscal.", value: this.PesoLiquidoTotal },
            { text: "Verifica se os produtos constantes na nota fiscal possuem De/Para", value: this.ProdutoDePara },
            { text: "Verifica se existe produto desativado ou descontinuado para aquela filial.", value: this.ProdutoSituacao },
            { text: "Verifica se existe produto sem cadastro para aquela filial.", value: this.ProdutoFilial },
            { text: "Verifica se os produto das notas tem um relacionamento com os cadastrado no sistema", value: this.ProdutoFilialRecebedor },
            { text: "Não definida", value: this.ProdutoConversaoUnidade },
            { text: "Não definida", value: this.CapacidadeExcedida },
            { text: "Verifica se todos os produtos da notas estão contido no pedido e viseversa", value: this.Produto },
        ];
        let [regra] = descricaoRegra.filter(x => x.value == tipoRegra);
        let [nomeRegra] = this.obterOpcoes().filter(x => x.value == tipoRegra)
        return regra != null ? `* ${nomeRegra.text} - ${regra.text}` : "Não definido"
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRegraNaoConformidade = Object.freeze(new EnumTipoRegraNaoConformidadeHelper());
