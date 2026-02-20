/// <reference path="RotaFrete.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Localidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPostoFiscal;
var _postoFiscal;
var _adicionarPostoFiscal;

var PostoFiscal = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.PostosFiscais = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarPostoFiscalOuPontoApoio, idBtnSearch: guid(), eventClick: adicionarPostoFiscalModalClick, issue: 0 });
};

var AdicionarPostoFiscal = function () {
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.Cliente.getFieldDescription(), required: true, idBtnSearch: guid() });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Cidade.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(false) });

    this.TempoEstimadoPermanenciaPostoFiscal = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.TempoEstimadoPermanencia.getRequiredFieldDescription(), getType: typesKnockout.mask, mask: '000:00', maskParams: { selectOnFocus: true, reverse: true }, required: true });
    this.LocalDeParqueamento = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.LocalDeParqueamento, getType: typesKnockout.bool, val: ko.observable(false) });

    this.Confirmar = PropertyEntity({ type: types.event, eventClick: confirmarAdicionarPostoFiscal, text: "Confirmar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: cancelarAdicionarPostoFiscal, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadPostoFiscal() {

    _postoFiscal = new PostoFiscal();
    KoBindings(_postoFiscal, "knockoutPostoFiscal");

    _adicionarPostoFiscal = new AdicionarPostoFiscal();
    KoBindings(_adicionarPostoFiscal, "divModalAdicionarPostoFiscal");

    new BuscarClientes(_adicionarPostoFiscal.Cliente, RetornoClientePostoFiscal);

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirPostoFiscalClick(_postoFiscal.PostosFiscais, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Latitude", visible: false },
        { data: "Longitude", visible: false },
        { data: "CodigoCliente", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%" },
        { data: "TempoEstimadoPermanencia", title: Localization.Resources.Logistica.RotaFrete.TempoEstimadoPermanencia, width: "20%" },
    ];

    _gridPostoFiscal = new BasicDataTable(_postoFiscal.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _postoFiscal.PostosFiscais.basicTable = _gridPostoFiscal;
    RecarregarGridPostoFiscal();
}

function RecarregarGridPostoFiscal() {
    _gridPostoFiscal.CarregarGrid(_rotaFrete.PostosFiscais.val());
}

function ExcluirPostoFiscalClick(knoutPostosFiscais, data) {

    var pontosPassagemPreDefinidosGrid = knoutPostosFiscais.basicTable.BuscarRegistros();

    for (var i = 0; i < pontosPassagemPreDefinidosGrid.length; i++) {
        if (data.Codigo == pontosPassagemPreDefinidosGrid[i].Codigo) {
            pontosPassagemPreDefinidosGrid.splice(i, 1);
            break;
        }
    }

    knoutPostosFiscais.basicTable.CarregarGrid(pontosPassagemPreDefinidosGrid);
}

function LimparCamposPostoFiscal() {
    LimparCampos(_postoFiscal);
    _gridPostoFiscal.CarregarGrid(new Array());
}

function adicionarPostoFiscalModalClick() {
    ExibirModalAdicionarPostoFiscal();
}

function ExibirModalAdicionarPostoFiscal() {

    Global.abrirModal('divModalAdicionarPostoFiscal');
    $("#divModalAdicionarPostoFiscal").one('hidden.bs.modal', function () {
        limparCamposAdicionarPostoFiscal();
    });
}

function confirmarAdicionarPostoFiscal() {

    if (!ValidarCamposObrigatorios(_adicionarPostoFiscal))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);

    var pontopassagem = {
        Codigo: guid(),
        CodigoCliente: Boolean(_adicionarPostoFiscal.Cliente.Codigo) ? _adicionarPostoFiscal.Cliente.Codigo : 0,
        Descricao: Boolean(_adicionarPostoFiscal.Cliente.Descricao) ? _adicionarPostoFiscal.Cliente.Descricao : _adicionarPostoFiscal.Localidade.Descricao,
        TempoEstimadoPermanencia: _adicionarPostoFiscal.TempoEstimadoPermanenciaPostoFiscal.val(),
        Latitude: _adicionarPostoFiscal.Cliente.Latitude,
        Longitude: _adicionarPostoFiscal.Cliente.Longitude
    };

    var listaPontos = _postoFiscal.PostosFiscais.basicTable.BuscarRegistros();

    if (listaPontos == "")
        listaPontos = [];

    listaPontos.push(pontopassagem);

    _rotaFrete.PostosFiscais.val(listaPontos);

    RecarregarGridPostoFiscal();

    cancelarAdicionarPostoFiscal();

    limparCamposAdicionarPostoFiscal();
}

function RetornoClientePostoFiscal(data) {
    _adicionarPostoFiscal.Cliente.codEntity(data.Codigo);
    _adicionarPostoFiscal.Cliente.val(data.Descricao);
    _adicionarPostoFiscal.Cliente.Codigo = data.Codigo;
    _adicionarPostoFiscal.Cliente.Descricao = data.Descricao;
    _adicionarPostoFiscal.Cliente.Latitude = data.Latitude;
    _adicionarPostoFiscal.Cliente.Longitude = data.Longitude;
}


function limparCamposAdicionarPostoFiscal() {
    LimparCampos(_adicionarPostoFiscal);
    Global.fecharModal("divModalAdicionarPostoFiscal");
}

function cancelarAdicionarPostoFiscal() {
    Global.fecharModal("divModalAdicionarPostoFiscal");
}
