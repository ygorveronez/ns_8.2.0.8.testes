/// <reference path="RotaFrete.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Localidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPontosPassagemPreDefinidoRotaFrete;
var _pontosPassagemPreDefinidoRotaFrete;
var _crudMapaPontosDePassagem;

var PontosPassagemPreDefinidoRotaFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.PontosPassagemPreDefinido = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarPontosPassagem, idBtnSearch: guid(), eventClick: adicionarPontoPassagemRotaFreteModalClick, issue: 0 });
};

var CRUDMapaPontosDePassagem = function () {
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.Cliente.getFieldDescription(), required: false, idBtnSearch: guid() });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Cidade.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(false) });

    this.TempoEstimadoPermanencia = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.TempoEstimadoPermanencia.getRequiredFieldDescription(), getType: typesKnockout.mask, mask: '000:00', required: true });
    this.LocalDeParqueamento = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.LocalDeParqueamento, getType: typesKnockout.bool, val: ko.observable(false) });

    this.Confirmar = PropertyEntity({ type: types.event, eventClick: confirmarCRUDMapaPontosDePassagem, text: "Confirmar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: cancelarCRUDMapaPontosDePassagem, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadPontosPassagemPreDefinidoRotaFrete() {

    _pontosPassagemPreDefinidoRotaFrete = new PontosPassagemPreDefinidoRotaFrete();
    KoBindings(_pontosPassagemPreDefinidoRotaFrete, "knockoutPontosPassagemPreDefinidoRotaFrete");

    _crudMapaPontosDePassagem = new CRUDMapaPontosDePassagem();
    KoBindings(_crudMapaPontosDePassagem, "knockoutCRUDMapaPontosDePassagem");

    new BuscarClientes(_crudMapaPontosDePassagem.Cliente, RetornoCliente);
    new BuscarLocalidades(_crudMapaPontosDePassagem.Localidade, null, null, RetornoLocalidade);

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirPontosPassagemPreDefinidoRotaFreteClick(_pontosPassagemPreDefinidoRotaFrete.PontosPassagemPreDefinido, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCliente", visible: false },
        { data: "CodigoLocalidade", visible: false },
        { data: "Latitude", visible: false },
        { data: "Longitude", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%" },
        { data: "TempoEstimadoPermanencia", title: Localization.Resources.Logistica.RotaFrete.TempoEstimadoPermanencia, width: "20%" },
        { data: "LocalDeParqueamento", title: "Local de Parqueamento", width: "20%" },
        { data: "TempoEstimadoPermanenciaMinutos", visible: false },
    ];

    _gridPontosPassagemPreDefinidoRotaFrete = new BasicDataTable(_pontosPassagemPreDefinidoRotaFrete.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.asc });
    _pontosPassagemPreDefinidoRotaFrete.PontosPassagemPreDefinido.basicTable = _gridPontosPassagemPreDefinidoRotaFrete;
    RecarregarGridPontosPassagemPreDefinidoRotaFrete();
}

function RecarregarGridPontosPassagemPreDefinidoRotaFrete() {
    _gridPontosPassagemPreDefinidoRotaFrete.CarregarGrid(_rotaFrete.PontosPassagemPreDefinido.val());
}

function ExcluirPontosPassagemPreDefinidoRotaFreteClick(knoutPontosPassagemPreDefinido, data) {

    var pontosPassagemPreDefinidosGrid = knoutPontosPassagemPreDefinido.basicTable.BuscarRegistros();

    for (var i = 0; i < pontosPassagemPreDefinidosGrid.length; i++) {
        if (data.Codigo == pontosPassagemPreDefinidosGrid[i].Codigo) {
            pontosPassagemPreDefinidosGrid.splice(i, 1);
            break;
        }
    }

    knoutPontosPassagemPreDefinido.basicTable.CarregarGrid(pontosPassagemPreDefinidosGrid);
}

function LimparCamposPontosPassagemPreDefinidoRotaFrete() {
    LimparCampos(_pontosPassagemPreDefinidoRotaFrete);
    _gridPontosPassagemPreDefinidoRotaFrete.CarregarGrid(new Array());
}

function adicionarPontoPassagemRotaFreteModalClick() {
    ExibirModalMapaPontosPassagem();
}

function ExibirModalMapaPontosPassagem() {

    Global.abrirModal('divModalMapaPontosPassagemRotaFrete');
    $("#divModalMapaPontosPassagemRotaFrete").one('hidden.bs.modal', function () {
        limparCamposCRUDMapaPontosDePassagem();
    });
}

function confirmarCRUDMapaPontosDePassagem() {

    if (!ValidarCamposObrigatorios(_crudMapaPontosDePassagem))
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);

    if (_crudMapaPontosDePassagem.Cliente.codEntity() == 0 && _crudMapaPontosDePassagem.Localidade.codEntity() == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.NecessarioSelecionarClienteOuCidade);

    if (_crudMapaPontosDePassagem.Cliente.codEntity() > 0 && _crudMapaPontosDePassagem.Localidade.codEntity() > 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.EPossivelSelecionarClienteOuCidade);

    var pontopassagem = {
        Codigo: guid(),
        CodigoCliente: Boolean(_crudMapaPontosDePassagem.Cliente.Codigo) ? _crudMapaPontosDePassagem.Cliente.Codigo : 0,
        CodigoLocalidade: Boolean(_crudMapaPontosDePassagem.Localidade.Codigo) ? _crudMapaPontosDePassagem.Localidade.Codigo : 0,
        Descricao: Boolean(_crudMapaPontosDePassagem.Cliente.Descricao) ? _crudMapaPontosDePassagem.Cliente.Descricao : _crudMapaPontosDePassagem.Localidade.Descricao,
        Latitude: Boolean(_crudMapaPontosDePassagem.Cliente.Latitude) ? _crudMapaPontosDePassagem.Cliente.Latitude : _crudMapaPontosDePassagem.Localidade.Latitude,
        Longitude: Boolean(_crudMapaPontosDePassagem.Cliente.Longitude) ? _crudMapaPontosDePassagem.Cliente.Longitude : _crudMapaPontosDePassagem.Localidade.Longitude,
        TempoEstimadoPermanencia: _crudMapaPontosDePassagem.TempoEstimadoPermanencia.val(),
        TempoEstimadoPermanenciaMinutos: converterHoraFormatadaEmMinutos(_crudMapaPontosDePassagem.TempoEstimadoPermanencia.val()),
        LocalDeParqueamento: _crudMapaPontosDePassagem.LocalDeParqueamento.val() ? "Sim" : "Não"
    };

    var listaPontos = _pontosPassagemPreDefinidoRotaFrete.PontosPassagemPreDefinido.basicTable.BuscarRegistros();

    if (listaPontos == "")
        listaPontos = [];

    listaPontos.push(pontopassagem);

    _rotaFrete.PontosPassagemPreDefinido.val(listaPontos);

    RecarregarGridPontosPassagemPreDefinidoRotaFrete();

    cancelarCRUDMapaPontosDePassagem();

    limparCamposCRUDMapaPontosDePassagem();
}

function RetornoCliente(data) {
    _crudMapaPontosDePassagem.Cliente.codEntity(data.Codigo);
    _crudMapaPontosDePassagem.Cliente.val(data.Descricao);
    _crudMapaPontosDePassagem.Cliente.Codigo = data.Codigo;
    _crudMapaPontosDePassagem.Cliente.Descricao = data.Descricao;
    _crudMapaPontosDePassagem.Cliente.Latitude = data.Latitude;
    _crudMapaPontosDePassagem.Cliente.Longitude = data.Longitude;
}

function RetornoLocalidade(data) {
    _crudMapaPontosDePassagem.Localidade.codEntity(data.Codigo);
    _crudMapaPontosDePassagem.Localidade.val(data.Descricao);
    _crudMapaPontosDePassagem.Localidade.Codigo = data.Codigo;
    _crudMapaPontosDePassagem.Localidade.Descricao = data.Descricao;
    _crudMapaPontosDePassagem.Localidade.Latitude = data.Latitude;
    _crudMapaPontosDePassagem.Localidade.Longitude = data.Longitude;
}

function limparCamposCRUDMapaPontosDePassagem() {
    LimparCampos(_crudMapaPontosDePassagem);

    _crudMapaPontosDePassagem.Cliente.Codigo = 0;
    _crudMapaPontosDePassagem.Cliente.Descricao = "";
    _crudMapaPontosDePassagem.Cliente.Latitude = 0;
    _crudMapaPontosDePassagem.Cliente.Longitude = 0;
    _crudMapaPontosDePassagem.Localidade.Codigo = 0;
    _crudMapaPontosDePassagem.Localidade.Descricao = "";
    _crudMapaPontosDePassagem.Localidade.Latitude = 0;
    _crudMapaPontosDePassagem.Localidade.Longitude = 0;
}

function cancelarCRUDMapaPontosDePassagem() {
    Global.fecharModal("divModalMapaPontosPassagemRotaFrete");
}

function converterHoraFormatadaEmMinutos(horaFormadata) {
    if (!horaFormadata) {
        return 0;
    }

    var [horas, minutos] = horaFormadata.split(":");
    try {
        return parseInt(horas) * 60 + parseInt(minutos);
    } catch {
        return 0;
    }
}
