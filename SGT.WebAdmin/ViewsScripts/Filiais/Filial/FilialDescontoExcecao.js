/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _listaFilialDescontoExcecao;
var _gridFilialDescontoExcecao = null;
var _FilialDescontoExcecao;

var ListaFilialDescontoExcecao = function () {
    this.AdicionarDescontoExcecao = PropertyEntity({ eventClick: exibirDescontoModalExcecaoClick, type: types.event, text: ko.observable(Localization.Resources.Filiais.Filial.AdicionarExcecaoDesconto), visible: true });
    this.Grid = PropertyEntity({ type: types.local });
}

var FilialDescontoExcecao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Filiais.Filial.Transportador.getFieldDescription()), required: false, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Filiais.Filial.Produto.getFieldDescription()), required: false, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Filiais.Filial.ModeloVeicularCarga.getFieldDescription()), issue: 44, required: false, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.HoraInicio = PropertyEntity({ text: Localization.Resources.Filiais.Filial.HoraInicio.getFieldDescription(), getType: typesKnockout.time, required: false });
    this.HoraFim = PropertyEntity({ text: Localization.Resources.Filiais.Filial.HoraFim.getFieldDescription(), getType: typesKnockout.time, required: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDescontoExcecaoApoliceSeguroClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
}

//*******EVENTOS*******
function criarGridFilialDescontoExcecao() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirFilialDescontoExcecaoClick(_FilialDescontoExcecao.Desconto, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTransportador", visible: false },
        { data: "DescricaoTransportador", title: Localization.Resources.Filiais.Filial.Transportador, width: "16%" },
        { data: "CodigoProduto", visible: false },
        { data: "DescricaoProduto", title: Localization.Resources.Filiais.Filial.Produto, width: "16%" },
        { data: "CodigoModeloVeicular", visible: false },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Filiais.Filial.ModeloVeicularCarga, width: "16%" },
        { data: "HoraInicio", title: Localization.Resources.Filiais.Filial.HoraInicio, width: "5%" },
        { data: "HoraFim", title: Localization.Resources.Filiais.Filial.HoraFim, width: "5%" }
    ];

    _gridFilialDescontoExcecao = new BasicDataTable(_listaFilialDescontoExcecao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function loadFilialDescontoExcecao() {
    _listaFilialDescontoExcecao = new ListaFilialDescontoExcecao();
    KoBindings(_listaFilialDescontoExcecao, "knockoutFilialDescontoExcecao");

    _FilialDescontoExcecao = new FilialDescontoExcecao();
    KoBindings(_FilialDescontoExcecao, "knockoutCadastroFilialDescontoExcecao");

    new BuscarEmpresa(_FilialDescontoExcecao.Transportador);
    new BuscarProdutos(_FilialDescontoExcecao.Produto);
    new BuscarModelosVeicularesCarga(_FilialDescontoExcecao.ModeloVeicularCarga);

    criarGridFilialDescontoExcecao();
}

function ExcluirFilialDescontoExcecaoClick(knoutFilialDescontoExcecao, data) {

    var descontosGrid = _gridFilialDescontoExcecao.BuscarRegistros();

    for (var i = 0; i < descontosGrid.length; i++) {
        if (data.Codigo == descontosGrid[i].Codigo) {
            descontosGrid.splice(i, 1);
            break;
        }
    }

    _gridFilialDescontoExcecao.CarregarGrid(descontosGrid);
}


function fecharDescontoModalExcecao() {
    Global.fecharModal('divModalDescontoExcecao');
}

function exibirDescontoModalExcecao() {
    Global.abrirModal('divModalDescontoExcecao');
    $("#divModalDescontoExcecao").one('hidden.bs.modal', function () {
        LimparCampos(_FilialDescontoExcecao);
    });
}


function exibirDescontoModalExcecaoClick(e, sender) {
    exibirDescontoModalExcecao()
}


function obterFilialDescontoExcecaoSalvar() {
    return {
        Codigo: _FilialDescontoExcecao.Codigo.val(),
        CodigoModeloVeicular: _FilialDescontoExcecao.ModeloVeicularCarga.codEntity(),
        DescricaoModeloVeicular: _FilialDescontoExcecao.ModeloVeicularCarga.val(),
        CodigoTransportador: _FilialDescontoExcecao.Transportador.codEntity(),
        DescricaoTransportador: _FilialDescontoExcecao.Transportador.val(),
        CodigoProduto: _FilialDescontoExcecao.Produto.codEntity(),
        DescricaoProduto: _FilialDescontoExcecao.Produto.val(),
        HoraInicio: _FilialDescontoExcecao.HoraInicio.val(),
        HoraFim: _FilialDescontoExcecao.HoraFim.val()
    };
}

function adicionarDescontoExcecaoApoliceSeguroClick() {
    if (validarCamposDescontoExcecao(_FilialDescontoExcecao)) {

        var apoliceSeguroDescontoExcecao = _gridFilialDescontoExcecao.BuscarRegistros();

        apoliceSeguroDescontoExcecao.push(obterFilialDescontoExcecaoSalvar());

        _gridFilialDescontoExcecao.CarregarGrid(apoliceSeguroDescontoExcecao);

        fecharDescontoModalExcecao();
    }
}

function validarCamposDescontoExcecao(e) {
    if ((e.Transportador.val() == "" || e.Transportador.codEntity() == 0 || e.Transportador.val() == undefined || e.Transportador.codEntity() == undefined) &&
        (e.Produto.val() == "" || e.Produto.codEntity() == 0 || e.Produto.val() == undefined || e.Produto.codEntity() == undefined) &&
        (e.ModeloVeicularCarga.val() == "" || e.ModeloVeicularCarga.codEntity() == 0 || e.ModeloVeicularCarga.val() == undefined || e.ModeloVeicularCarga.codEntity() == undefined)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Filiais.Filial.AvisoInformarTransportadoraProdutoModeloVeicular);
        return false;
    }
    else if (((e.HoraInicio.val() == "" || e.HoraInicio.val() == undefined) && (e.HoraFim.val() != "" || e.HoraFim.val() != undefined )) ||
        ((e.HoraInicio.val() != "" || e.HoraInicio.val() != undefined) &&(e.HoraFim.val() == "" || e.HoraFim.val() == undefined)) ) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Filiais.Filial.AvisoInformarHoraInicioHoraFimQuandoInformado);
        return false;
    }
    else return true;
}
