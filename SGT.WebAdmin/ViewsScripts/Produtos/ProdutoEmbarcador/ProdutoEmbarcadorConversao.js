//#region Variaiveis Globais
var _gridProdutoConversaoUnidades;
var _produtoConversaoDeUnidades;
//#endregion

//#region Funções constructoras
function ProdutoConversaoUnidade() {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });
    this.QuantidadeDe = PropertyEntity({ text: "Quantidade (de):", getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, maxlength: 14 });
    this.QuantidadePara = PropertyEntity({ text: "*Quantidade (para):", getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, maxlength: 14, required: true });

    this.TipoConversao = PropertyEntity({ text: "*Tipo Conversão:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid(), val: ko.observable(""), required: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarConversaoGrid, type: types.event, text: "Adicionar" });
}

function LoadProdutosConversoes() {
    _produtoConversaoDeUnidades = new ProdutoConversaoUnidade();
    KoBindings(_produtoConversaoDeUnidades, "knockoutConversao");

    new BuscarTipoConversaoUnidade(_produtoConversaoDeUnidades.TipoConversao, retornoTipoConversao);

    LoadGridProdutoConversao();
}
//#endregion

//#region Funções Auxiliares

function LoadGridProdutoConversao() {
    const headerGrid = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoConversao", visible: false },
        { data: "QteDe", title: "Quantidade (de)" },
        { data: "TipoConversao", title: "Conversão" },
        { data: "QtePara", title: "Quantidade (para)" }
    ]

    const excluirConversao = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: RemoverItemGrid, tamanho: "15", icone: ""
    };

    const menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [excluirConversao]
    };

    _gridProdutoConversaoUnidades = new BasicDataTable(_produtoConversaoDeUnidades.Grid.idGrid, headerGrid, menuOpcoes);
    CarregarGridProdutoConversao(new Array());
}

function CarregarGridProdutoConversao(lista) {
    _gridProdutoConversaoUnidades.CarregarGrid(lista);
}

function AdicionarConversaoGrid() {
    if (ValidarCamposObrigatorios(_produtoConversaoDeUnidades) === false) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    const listaRegistrosGrid = ObterRegistrosGridProductoConversao();

    const novaTipoConversaoGrid = {
        Codigo: 0,
        CodigoTipoConversao: _produtoConversaoDeUnidades.TipoConversao.codEntity(),
        TipoConversao: _produtoConversaoDeUnidades.TipoConversao.val(),
        QteDe: _produtoConversaoDeUnidades.QuantidadeDe.val(),
        QtePara: _produtoConversaoDeUnidades.QuantidadePara.val()
    }
    listaRegistrosGrid.push(novaTipoConversaoGrid);
    CarregarGridProdutoConversao(listaRegistrosGrid);
    LimparCamposConversao();
}
function ObterRegistrosGridProductoConversao() {
    return _gridProdutoConversaoUnidades.BuscarRegistros();
}

function retornoTipoConversao(selecionada) {
    _produtoConversaoDeUnidades.TipoConversao.val(selecionada.ConversaoDe + " ↔ " + selecionada.ConversaoPara);
    _produtoConversaoDeUnidades.TipoConversao.codEntity(selecionada.Codigo);
}

function RemoverItemGrid(e) {
    const listaRegistros = ObterRegistrosGridProductoConversao();
    for (var i = 0; i < listaRegistros.length; i++) {
        if (listaRegistros[i].Codigo == e.Codigo && e.CodigTipoConversao == listaRegistros[i].CodigTipoConversao) {
            listaRegistros.splice(i, 1);
            break;
        }
    }
    CarregarGridProdutoConversao(listaRegistros);
}

function LimparCamposConversao() {
    LimparCampo(_produtoConversaoDeUnidades.TipoConversao);
    LimparCampo(_produtoConversaoDeUnidades.QuantidadePara);
    LimparCampo(_produtoConversaoDeUnidades.QuantidadeDe);
}

function ObterDadosDaGridConversaoFormatados() {
    const listRegistroGrid = ObterRegistrosGridProductoConversao();
    const listFormatada = new Array();

    $.each(listRegistroGrid, (_, item) => {
        let data = {
            Codigo: item.Codigo,
            CodigoTipoConversao: item.CodigoTipoConversao,
            QuantidadePara: item.QtePara,
            QuantidadeDe: item.QteDe
        }
        listFormatada.push(data);
    })
    _produtoEmbarcador.TabelaConversao.val(JSON.stringify(listFormatada));

}

function LimparTodosOsCampos() {
    LimparCampos(_produtoConversaoDeUnidades);
    CarregarGridProdutoConversao(new Array());
}
//#endregion