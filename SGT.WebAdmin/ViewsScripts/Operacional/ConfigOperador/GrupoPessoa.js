/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="ConfigOperador.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoPessoa;
var _grupoPessoa;

var GrupoPessoa = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Grupo = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.AdicionarGrupoPessoas, idBtnSearch: guid() });
    this.PossuiFiltroGrupoPessoas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Operacional.ConfigOperador.EsteOperadorDeveFiltrarCargas });
    this.VisualizaCargasSemGrupoPessoas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Operacional.ConfigOperador.OperadorVisualizaCargasNaoSaoGrupoPessoas });
};

//*******EVENTOS*******

function loadGrupoPessoa() {

    _grupoPessoa = new GrupoPessoa();
    KoBindings(_grupoPessoa, "knockoutGrupoPessoa");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: function (data) {
                excluirGrupoPessoaClick(_grupoPessoa.Grupo, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "80%" }
    ];

    _gridGrupoPessoa = new BasicDataTable(_grupoPessoa.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarGruposPessoas(_grupoPessoa.Grupo, null, null, _gridGrupoPessoa, EnumTipoGrupoPessoas.Ambos, false);
    _grupoPessoa.Grupo.basicTable = _gridGrupoPessoa;

    recarregarGridGrupoPessoa();
}

function recarregarGridGrupoPessoa() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_operador.GrupoPessoas.val())) {
        $.each(_operador.GrupoPessoas.val(), function (i, grupoPessoa) {
            var grupoPessoaGrid = new Object();

            grupoPessoaGrid.Codigo = grupoPessoa.Grupo.Codigo;
            grupoPessoaGrid.Descricao = grupoPessoa.Grupo.Descricao;

            data.push(grupoPessoaGrid);
        });
    }

    _gridGrupoPessoa.CarregarGrid(data);

    _grupoPessoa.VisualizaCargasSemGrupoPessoas.val(_operador.VisualizaCargasSemGrupoPessoas.val());
    _grupoPessoa.PossuiFiltroGrupoPessoas.val(_operador.PossuiFiltroGrupoPessoas.val());
}

function excluirGrupoPessoaClick(knoutGrupo, data) {
    var grupoGrid = knoutGrupo.basicTable.BuscarRegistros();

    for (var i = 0; i < grupoGrid.length; i++) {
        if (data.Codigo == grupoGrid[i].Codigo) {
            grupoGrid.splice(i, 1);
            break;
        }
    }
    knoutGrupo.basicTable.CarregarGrid(grupoGrid);
}

function limparCamposGrupoPessoa() {
    LimparCampos(_grupoPessoa);
}