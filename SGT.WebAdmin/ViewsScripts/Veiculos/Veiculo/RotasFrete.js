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
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="Veiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRotasFrete;

//*******EVENTOS*******

function LoadRotasFrete() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirRotasFreteClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Veiculos.Veiculo.RotaDeFrete, width: "70%" },
    ];

    _gridRotasFrete = new BasicDataTable(_veiculo.GridRotasFretes.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridRotasFrete();
}

function RecarregarGridRotasFrete() {
    var data = new Array();
    if (_veiculo.RotasFrete.val() != "" && _veiculo.RotasFrete.val().length > 0) {
        $.each(_veiculo.RotasFrete.val(), function (i, rotaFrete) {
            var rotasFreteGrid = new Object();

            rotasFreteGrid.Codigo = rotaFrete.Codigo;
            rotasFreteGrid.Descricao = rotaFrete.Descricao;

            data.push(rotasFreteGrid);
        });
    }

    _gridRotasFrete.CarregarGrid(data);
}

function ExcluirRotasFreteClick(data) {
    var tiposCargasGrid = _gridRotasFrete.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridRotasFrete.CarregarGrid(tiposCargasGrid);
}

function LimparCamposRotasFrete() {
    LimparCampo(_veiculo.RotaFrete);
    _gridRotasFrete.CarregarGrid(new Array());
}