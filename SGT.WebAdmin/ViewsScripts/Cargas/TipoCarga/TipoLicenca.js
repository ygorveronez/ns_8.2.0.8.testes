/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Licenca.js"/>
/// <reference path="TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*****
var _tiposLicenca;
var _gridTiposLicenca;

var TiposLicenca = function () {
    this.TipoLicenca = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.TipoCarga.AdicionarTipoLicenca, idBtnSearch: guid() });
    this.GridTiposLicenca = PropertyEntity({ type: types.local });
};

//*******EVENTOS*******

function loadTiposLicenca() {
    _tiposLicenca = new TiposLicenca();
    KoBindings(_tiposLicenca, "knockoutTiposLicenca");

    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerTiposLicenca, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "55%", className: "text-align-left" },
        { data: "DescricaoTipo", title: Localization.Resources.Consultas.Licenca.Tipo, width: "20%", className: "text-align-left" }
    ];

    _gridTiposLicenca = new BasicDataTable(_tiposLicenca.GridTiposLicenca.id, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarLicenca(_tiposLicenca.TipoLicenca, null, _gridTiposLicenca);
    _tiposLicenca.TipoLicenca.basicTable = _gridTiposLicenca;

    recarregarGridTiposLicenca();
}

function preencherTiposLicencaParaBackEnd() {
    _tipoCarga.ListaTiposLicenca.list = new Array();
    let registros = _gridTiposLicenca.BuscarRegistros();
    _tipoCarga.ListaTiposLicenca.val(JSON.stringify(registros));
}


function recarregarGridTiposLicenca() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tipoCarga.ListaTiposLicenca.val())) {
        $.each(_tipoCarga.ListaTiposLicenca.val(), function (i, tipoLicenca) {
            var tipoLicencaGrid = new Object();
            
            tipoLicencaGrid.Codigo = tipoLicenca.Codigo;
            tipoLicencaGrid.Descricao = tipoLicenca.Descricao;
            tipoLicencaGrid.DescricaoTipo = tipoLicenca.DescricaoTipo;

            data.push(tipoLicencaGrid);
        });
    }

    _gridTiposLicenca.CarregarGrid(data);
}

function removerTiposLicenca(data) {
    
    var tipoLicencaGrid = _tiposLicenca.TipoLicenca.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoLicencaGrid.length; i++) {
        if (data.Codigo == tipoLicencaGrid[i].Codigo) {
            tipoLicencaGrid.splice(i, 1);
            break;
        }
    }

    _tiposLicenca.TipoLicenca.basicTable.CarregarGrid(tipoLicencaGrid);

}

function limparCamposTiposLicenca() {
    LimparCampos(_tiposLicenca);
}