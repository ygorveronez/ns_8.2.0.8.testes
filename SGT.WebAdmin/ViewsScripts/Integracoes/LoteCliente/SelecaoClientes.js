/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="LoteCliente.js" />
/// <reference path="../../Consultas/Localidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridSelecaoClientes;
var _selecaoClientes;

var SelecaoClientes = function () {
    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final:  ", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.RazaoSocial = PropertyEntity({ text: ko.observable("Razão Social:"), enable: ko.observable(true), maxlength: 2000 });
    this.CNPJ = PropertyEntity({ getType: typesKnockout.cnpj, text: "CNPJ: ", maxlength: 20, enable: ko.observable(true) });
    this.InscricaoEstadual = PropertyEntity({ text: ko.observable("Inscrição Estadual:"), maxlength: 50, enable: ko.observable(true) });
    this.Endereco = PropertyEntity({ text: "Endereço:", enable: ko.observable(true), maxlength: 500 });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(true), text: "Selecionar Todos" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSelecaoClientes.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Criar = PropertyEntity({ eventClick: CriarClick, type: types.event, text: "Criar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadSelecaoClientes() {
    _selecaoClientes = new SelecaoClientes();
    KoBindings(_selecaoClientes, "knockoutSelecaoClientes");

    GridSelecaoClientes();

    new BuscarLocalidades(_selecaoClientes.Localidade);

    $('#knockoutSelecaoClientes').keypress(function (e) {
        var keyCode = e.keyCode || e.which;

        if (keyCode == 13)
            $('#' + _selecaoClientes.Pesquisar.id).trigger('click');
    });
}

//*******MÉTODOS*******

function CriarClick(e, sender) {
    if (ValidaClientesSelecionados()) {
        if (ValidarCamposObrigatorios(e)) {
            exibirConfirmacao("Criar Lote de Cliente", "Você tem certeza que deseja criar o lote para os clientes selecionados?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoClientes);

                dados.SelecionarTodos = _selecaoClientes.SelecionarTodos.val();

                if (!dados.SelecionarTodos)
                    dados.ClientesSelecionados = JSON.stringify(ObterCodigosRegistrosMultiplaSelecao(_gridSelecaoClientes.ObterMultiplosSelecionados()));
                else
                    dados.ClientesSelecionados = JSON.stringify(ObterCodigosRegistrosMultiplaSelecao(_gridSelecaoClientes.ObterMultiplosNaoSelecionados()));

                executarReST("LoteCliente/Adicionar", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote de clientes criado com sucesso!");

                            _loteCliente.Situacao.val(EnumSituacaoLoteCliente.AgIntegracao);

                            BuscarLoteClientePorCodigo(arg.Data.Codigo);
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Por favor, informe os campos obrigatórios");
        }
    }
}

function ObterCodigosRegistrosMultiplaSelecao(objetos) {
    var codigos = [];

    for (var i = 0; i < objetos.length; i++)
        codigos.push(objetos[i].Codigo);

    return codigos;
}

function GridSelecaoClientes() {
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoClientes.SelecionarTodos,
        somenteLeitura: false
    };

    if (_loteCliente.Situacao.val() != EnumSituacaoLoteCliente.EmCriacao)
        multiplaescolha = null;

    _gridSelecaoClientes = new GridView(_selecaoClientes.Pesquisar.idGrid, "LoteCliente/PesquisaCliente", _selecaoClientes, null, { column: 1, dir: orderDir.asc }, 25, null, null, null, multiplaescolha);
    _gridSelecaoClientes.SetPermitirRedimencionarColunas(true);
    _gridSelecaoClientes.CarregarGrid();
}

function ValidaClientesSelecionados() {
    var valido = true;

    var itens = _gridSelecaoClientes.ObterMultiplosSelecionados();

    if (itens.length == 0 && !_selecaoClientes.SelecionarTodos.val()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Clientes Selecionados", "Nenhum cliente selecionado.");
    }

    return valido;
}

function EditarSelecaoClientes(data) {
    _selecaoClientes.SelecionarTodos.visible(false);
    _selecaoClientes.DataInicio.enable(false);
    _selecaoClientes.DataFim.enable(false);
    _selecaoClientes.Criar.visible(false);

    _selecaoClientes.Codigo.val(data.Codigo);
    _selecaoClientes.DataInicio.val(data.DataInicial);
    _selecaoClientes.DataFim.val(data.DataFinal);

    GridSelecaoClientes();
}

function LimparCamposSelecaoClientes() {
    _selecaoClientes.DataInicio.enable(true);
    _selecaoClientes.DataFim.enable(true);
    _selecaoClientes.Criar.visible(true);
    _selecaoClientes.SelecionarTodos.visible(true);
    _selecaoClientes.SelecionarTodos.val(false);

    LimparCampos(_selecaoClientes);
}
