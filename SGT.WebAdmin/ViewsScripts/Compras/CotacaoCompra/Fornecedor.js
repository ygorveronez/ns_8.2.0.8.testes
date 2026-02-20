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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />

var _fornecedorCotacaoCompra;
var _gridFornecedores;
var _modalHistoricoCompraCotacaoCompra;

var FornecedorMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.string });
    this.CodigoCotacao = PropertyEntity({ type: types.map, val: "" });
    this.CodigoFornecedor = PropertyEntity({ type: types.map, val: "" });
    this.Fornecedor = PropertyEntity({ type: types.map, val: "" });
}

var FornecedorCotacaoCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoFornecedor = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.FornecedoresCotacao = PropertyEntity({ type: types.local });

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Fornecedor:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.SalvarFornecedorCotacao = PropertyEntity({ type: types.event, eventClick: SalvarFornecedorCotacaoClick, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******
function loadFornecedorCotacaoCompra() {
    _fornecedorCotacaoCompra = new FornecedorCotacaoCompra();
    KoBindings(_fornecedorCotacaoCompra, "knockoutCotacao_Fornecedor");

    new BuscarClientes(_fornecedorCotacaoCompra.Fornecedor);

    CarregarFornecedoresCotacao();

    _modalHistoricoCompraCotacaoCompra = new bootstrap.Modal(document.getElementById("divHistoricoCompra"), { backdrop: 'static', keyboard: true });
}

function SalvarFornecedorCotacaoClick(e, sender) {
    var tudoCerto = true;
    _fornecedorCotacaoCompra.Fornecedor.requiredClass("form-control");

    if (!(_fornecedorCotacaoCompra.Fornecedor.codEntity() > 0)) {
        tudoCerto = false;
        _fornecedorCotacaoCompra.Fornecedor.requiredClass("form-control is-invalid");
    }

    if (tudoCerto) {
        if (_fornecedorCotacaoCompra.CodigoFornecedor.val() == "") {
            //novo
            var fornecedor = new FornecedorMap()
            _fornecedorCotacaoCompra.CodigoFornecedor.val(guid());

            fornecedor.Codigo.val = _fornecedorCotacaoCompra.CodigoFornecedor.val();
            fornecedor.CodigoCotacao.val = _fornecedorCotacaoCompra.Codigo.val();
            fornecedor.CodigoFornecedor.val = _fornecedorCotacaoCompra.Fornecedor.codEntity();
            fornecedor.Fornecedor.val = _fornecedorCotacaoCompra.Fornecedor.val();

            _cotacaoCompra.ListaFornecedor.list.push(fornecedor);
        } else {
            //editando
            $.each(_cotacaoCompra.ListaFornecedor.list, function (i, fornecedor) {
                if (fornecedor.Codigo.val == _fornecedorCotacaoCompra.CodigoFornecedor.val()) {

                    fornecedor.CodigoCotacao.val = _fornecedorCotacaoCompra.Codigo.val();
                    fornecedor.CodigoFornecedor.val = _fornecedorCotacaoCompra.Fornecedor.codEntity();
                    fornecedor.Fornecedor.val = _fornecedorCotacaoCompra.Fornecedor.val();

                    return false;
                }
            });
        }

        RegarregarGridFornecedores();
        RegarregarGridRetornos();

        LimparCampoEntity(_fornecedorCotacaoCompra.Fornecedor);
        _fornecedorCotacaoCompra.CodigoFornecedor.val("");
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function CarregarFornecedoresCotacao() {
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: ExcluirFornecedorCotacaoClick, tamanho: "10", icone: "" };
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarFornecedorCotacaoClick, tamanho: "10", icone: "" };
    var historico = { descricao: "Histórico", id: guid(), evento: "onclick", metodo: HistoricoFornecedorCotacaoClick, tamanho: "10", icone: "" };
    var qualificacao = { descricao: "Qualificação", id: guid(), evento: "onclick", metodo: QualificacaoFornecedorCotacaoClick, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [excluir, editar, historico, qualificacao], descricao: "Opções", tamanho: 10 };

    var header = [{ data: "Codigo", visible: false },
    { data: "CodigoCotacao", visible: false },
    { data: "CodigoFornecedor", visible: false },
    { data: "Fornecedor", title: "Fornecedor", width: "70%" }];

    _gridFornecedores = new BasicDataTable(_fornecedorCotacaoCompra.FornecedoresCotacao.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });
    RegarregarGridFornecedores();
}

function RegarregarGridFornecedores() {
    var data = new Array();

    $.each(_cotacaoCompra.ListaFornecedor.list, function (i, listaFornecedor) {
        var listaFornecedorGrid = new Object();

        listaFornecedorGrid.Codigo = listaFornecedor.Codigo.val;
        listaFornecedorGrid.CodigoCotacao = listaFornecedor.CodigoCotacao.val;
        listaFornecedorGrid.CodigoFornecedor = listaFornecedor.CodigoFornecedor.val;
        listaFornecedorGrid.Fornecedor = listaFornecedor.Fornecedor.val;

        data.push(listaFornecedorGrid);
    });

    _gridFornecedores.CarregarGrid(data);
}


function ExcluirFornecedorCotacaoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o fornecedor selecionado?", function () {
        var listaAtualizada = new Array();
        $.each(_cotacaoCompra.ListaFornecedor.list, function (i, fornecedor) {
            if (fornecedor.Codigo.val != data.Codigo) {
                listaAtualizada.push(fornecedor);
            }
        });
        _cotacaoCompra.ListaFornecedor.list = listaAtualizada;

        RegarregarGridFornecedores();

        LimparCampoEntity(_fornecedorCotacaoCompra.Fornecedor);
        _fornecedorCotacaoCompra.CodigoFornecedor.val("");
    });
}

function EditarFornecedorCotacaoClick(data) {
    _fornecedorCotacaoCompra.Fornecedor.val(data.Fornecedor);
    _fornecedorCotacaoCompra.Fornecedor.codEntity(data.CodigoFornecedor);
    _fornecedorCotacaoCompra.CodigoFornecedor.val(data.Codigo);
}

function HistoricoFornecedorCotacaoClick(data) {
    _modalHistoricoCompraCotacaoCompra.show();

    _historico.Codigo.val(data.CodigoFornecedor);
    _gridHistorico = new GridView(_historico.Historico.id, "Pessoa/HistoricoNotaEntrada", _historico);
    _gridHistorico.CarregarGrid();
}

function QualificacaoFornecedorCotacaoClick(data) {
    _modalHistoricoCompraCotacaoCompra.show();

    _historico.Codigo.val(data.CodigoFornecedor);
    _gridHistorico = new GridView(_historico.Historico.id, "Pessoa/DocumentosFornecedor", _historico);
    _gridHistorico.CarregarGrid();
}