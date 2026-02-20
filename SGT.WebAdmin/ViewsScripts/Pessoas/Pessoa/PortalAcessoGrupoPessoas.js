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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="Pessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoPessoas;
var _grupoPessoas;

var GrupoPessoas = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CadastrarGrupoPessoas = PropertyEntity({ type: types.event, text: "Adicionar Grupo de Pessoas", idBtnSearch: guid(), visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadGrupoPessoas() {
    _grupoPessoas = new GrupoPessoas();
    KoBindings(_grupoPessoas, "knockoutGrupoPessoas");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirGrupoPessoasClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Grupo de Pessoas", width: "70%" },
    ];

    _gridGrupoPessoas = new BasicDataTable(_grupoPessoas.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarGruposPessoas(_grupoPessoas.CadastrarGrupoPessoas, null, null, _gridGrupoPessoas);

    RecarregarGridGrupoPessoas();
}

function RecarregarGridGrupoPessoas() {
    var data = new Array();
    if (_pessoa.GrupoPessoasAcessoPortal.val() != "") {
        $.each(_pessoa.GrupoPessoasAcessoPortal.val(), function (i, grupoPessoa) {
            var grupoPessoasGrid = new Object();

            grupoPessoasGrid.Codigo = grupoPessoa.Codigo;
            grupoPessoasGrid.Descricao = grupoPessoa.Descricao;

            data.push(grupoPessoasGrid);
        });
    }
    _gridGrupoPessoas.CarregarGrid(data);
}

function ExcluirGrupoPessoasClick(data) {
    var grupoPessoasGrid = _gridGrupoPessoas.BuscarRegistros();

    for (var i = 0; i < grupoPessoasGrid.length; i++) {
        if (data.Codigo == grupoPessoasGrid[i].Codigo) {
            grupoPessoasGrid.splice(i, 1);
            break;
        }
    }

    _gridGrupoPessoas.CarregarGrid(grupoPessoasGrid);
}

function LimparCamposGrupoPessoas() {
    LimparCampos(_grupoPessoas);
    _gridGrupoPessoas.CarregarGrid(new Array());
}