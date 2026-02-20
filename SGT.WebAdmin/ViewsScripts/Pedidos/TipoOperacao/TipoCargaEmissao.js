//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoCargaEmissao;
var _tipoCargaEmissao;

var TipoCargaEmissao = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.TipoCarga = PropertyEntity({ type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.AdicionarTipoDeCarga, idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTipoCargaEmissao() {
    _tipoCargaEmissao = new TipoCargaEmissao();
    KoBindings(_tipoCargaEmissao, "knockoutTipoCargaEmissao");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTipoCargaEmissaoClick(data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%" },
    ];

    _gridTipoCargaEmissao = new BasicDataTable(_tipoCargaEmissao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_tipoCargaEmissao.TipoCarga, null, null, _gridTipoCargaEmissao);

    RecarregarGridTipoCargaEmissao();
}

function RecarregarGridTipoCargaEmissao() {

    let data = new Array();

    if (_tipoOperacao.TiposCargasEmissao.val() != "" && _tipoOperacao.TiposCargasEmissao.val().length > 0) {
        $.each(_tipoOperacao.TiposCargasEmissao.val(), function (i, tipoCarga) {
            let tipoCargaEmissaoGrid = new Object();

            tipoCargaEmissaoGrid.Codigo = tipoCarga.Codigo;
            tipoCargaEmissaoGrid.Descricao = tipoCarga.Descricao;

            data.push(tipoCargaEmissaoGrid);
        });
    }

    _gridTipoCargaEmissao.CarregarGrid(data);
}

function ExcluirTipoCargaEmissaoClick(data) {
    let tiposCargasEmissaoGrid = _gridTipoCargaEmissao.BuscarRegistros();

    for (let i = 0; i < tiposCargasEmissaoGrid.length; i++) {
        if (data.Codigo == tiposCargasEmissaoGrid[i].Codigo) {
            tiposCargasEmissaoGrid.splice(i, 1);
            break;
        }
    }

    _gridTipoCargaEmissao.CarregarGrid(tiposCargasEmissaoGrid);
}

function LimparCamposTipoCargaEmissao() {
    LimparCampos(_tipoCargaEmissao);
    _gridTipoCargaEmissao.CarregarGrid([]);
}