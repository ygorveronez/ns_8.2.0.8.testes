/// <reference path="../../Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDestinatarioRotaFrete;
var _destinatarioRotaFrete;

var DestinatarioRotaFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Destinatario = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarDestinatario, idBtnSearch: guid(), issue: 0 });
    this.AdicionarOutroEndereco = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarOutroEndereco, idBtnSearch: guid(), issue: 0 });

    this.ValidarParaQualquerDestinatarioInformado = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.ValidarDestinatarioInformado, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });

    this.ValidarParaQualquerDestinatarioInformado.val.subscribe(function () {
        ControleVisibilidadeAbaRoteirizacao();
    });
};

//*******EVENTOS*******

function LoadDestinatarioRotaFrete() {

    _destinatarioRotaFrete = new DestinatarioRotaFrete();
    KoBindings(_destinatarioRotaFrete, "knockoutDestinatarioRotaFrete");

    _rotaFrete.ValidarParaQualquerDestinatarioInformado = _destinatarioRotaFrete.ValidarParaQualquerDestinatarioInformado;

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirDestinatarioRotaFreteClick(_destinatarioRotaFrete.Destinatario, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Latitude", visible: false },
        { data: "Longitude", visible: false },
        { data: "Endereco", visible: false },
        { data: "Numero", visible: false },
        { data: "CEP", visible: false },
        { data: "Nome", visible: false },
        { data: "CodigoIBGE", visible: false },
        { data: "CodigoOutroEndereco", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "50%" },
        { data: "Localidade", title: Localization.Resources.Logistica.RotaFrete.Localidades, width: "20%" },
        { data: "DescricaoOutroEndereco", title: "Outro Endereço", width: "30%" },
        { data: "Ordem", visible: false }
    ];

    _gridDestinatarioRotaFrete = new BasicDataTable(_destinatarioRotaFrete.Grid.id, header, menuOpcoes, { column: 8, dir: orderDir.asc });

    new BuscarClientes(_destinatarioRotaFrete.Destinatario, null, null, null, null, _gridDestinatarioRotaFrete);
    new BuscarClienteOutroEndereco(_destinatarioRotaFrete.AdicionarOutroEndereco, callbackDestinatarioOutroEndereco, null, true);

    _destinatarioRotaFrete.Destinatario.basicTable = _gridDestinatarioRotaFrete;

    RecarregarGridDestinatarioRotaFrete();
}

function RecarregarGridDestinatarioRotaFrete() {
    _gridDestinatarioRotaFrete.CarregarGrid(_rotaFrete.Destinatarios.val());
}

function ExcluirDestinatarioRotaFreteClick(knoutDestinatario, data) {

    var destinatariosGrid = knoutDestinatario.basicTable.BuscarRegistros();

    for (var i = 0; i < destinatariosGrid.length; i++) {
        if (data.Codigo == destinatariosGrid[i].Codigo) {
            destinatariosGrid.splice(i, 1);
            break;
        }
    }

    knoutDestinatario.basicTable.CarregarGrid(destinatariosGrid);
}

function LimparCamposDestinatarioRotaFrete() {
    LimparCampos(_destinatarioRotaFrete);
    _gridDestinatarioRotaFrete.CarregarGrid(new Array());
}

function callbackDestinatarioOutroEndereco(dados) {


    var destinatariosGrid = _destinatarioRotaFrete.Destinatario.basicTable.BuscarRegistros();

    var destinatario = {
        Codigo: dados.CodigoCliente,
        Descricao: dados.DescricaoCliente,
        Nome: dados.DescricaoCliente,
        CEP: dados.CEP,
        CodigoIBGE: dados.Endereco,
        Endereco: dados.Endereco,
        Latitude: dados.Latitude,
        Localidade: dados.Cidade,
        Longitude: dados.Longitude,
        Numero: dados.Numero,
        Ordem: destinatariosGrid.length,
        CodigoOutroEndereco: dados.Codigo,
        DescricaoOutroEndereco: dados.Descricao,
    };

    if (destinatariosGrid.length == 0)
        destinatariosGrid = new Array();

    destinatariosGrid.push(destinatario);

    _destinatarioRotaFrete.Destinatario.basicTable.CarregarGrid(destinatariosGrid);

}
