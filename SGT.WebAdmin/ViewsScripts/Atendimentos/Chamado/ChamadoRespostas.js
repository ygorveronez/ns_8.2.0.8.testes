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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="Chamado.js" />
/// <reference path="ChamadoEtapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _chamadoRespostas;
var _gridRespostas;

var RespostaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Resposta = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoFuncionario = PropertyEntity({ type: types.map, val: ko.observable(""), getType: typesKnockout.int });
    this.Funcionario = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DataHora = PropertyEntity({ type: types.map, val: ko.observable(""), getType: typesKnockout.dateTime });
}

var ChamadoRespostas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Resposta = PropertyEntity({ text: "*Resposta: ", required: true, maxlength: 5000 });

    this.FuncionarioResposta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.SalvarResposta = PropertyEntity({ eventClick: SalvarRespostaClick, type: types.event, text: "Salvar Resposta", visible: ko.observable(true), enable: ko.observable(true) });

    this.RespostasChamado = PropertyEntity({ type: types.local, id: guid() });
}

//*******EVENTOS*******

function loadChamadoRespostas() {
    _chamadoRespostas = new ChamadoRespostas();
    KoBindings(_chamadoRespostas, "knockoutRespostasChamado");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirRespostaClick, tamanho: 10 }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoChamado", visible: false },
        { data: "Resposta", title: "Resposta", width: "50%", },
        { data: "CodigoFuncionario", visible: false },
        { data: "Funcionario", title: "Funcionário", width: "25%" },
        { data: "DataHora", title: "Data e Hora", width: "15%" }
    ];

    _gridRespostas = new BasicDataTable(_chamadoRespostas.RespostasChamado.id, header, menuOpcoes, { column: 5, dir: orderDir.desc });

    buscarFuncionarioLogado();
    recarregarGridListaRespostas();
}

function ExcluirRespostaClick(data) {
    if (data.CodigoFuncionario != _chamadoRespostas.FuncionarioResposta.codEntity()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível excluir respostas de outro funcionário!");
    } else {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir a resposta: " + data.Resposta + "?", function () {
            $.each(_chamado.ListaRespostas.list, function (i, listaRespostas) {
                if (data.Codigo == listaRespostas.Codigo.val) {
                    _chamado.ListaRespostas.list.splice(i, 1);

                    if (VerificaNovasRespostasLancados(data.Codigo)) {
                        var listaRespostasExcluidas = new RespostaMap();
                        listaRespostasExcluidas.Codigo.val = data.Codigo;
                        _chamado.ListaRespostasExcluidas.list.push(listaRespostasExcluidas);
                    }

                    return false;
                }
            });

            $.each(_chamado.ListaRespostasNovas.list, function (i, listaRespostasNovas) {
                if (data.Codigo == listaRespostasNovas.Codigo.val) {
                    _chamado.ListaRespostasNovas.list.splice(i, 1);

                    return false;
                }
            });

            recarregarGridListaRespostas();
        });
    }
}

function SalvarRespostaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_chamadoRespostas);
    var nomeFuncionario, codigoFuncionario;

    if (valido) {
        var codigo = guid();
        var listaRespostasGrid = new RespostaMap();

        listaRespostasGrid.Codigo.val = codigo;
        listaRespostasGrid.Resposta.val = _chamadoRespostas.Resposta.val();
        listaRespostasGrid.CodigoFuncionario.val = _chamadoRespostas.FuncionarioResposta.codEntity();
        listaRespostasGrid.Funcionario.val = _chamadoRespostas.FuncionarioResposta.val();
        listaRespostasGrid.DataHora.val = moment().format("DD/MM/YYYY HH:mm:ss");
        _chamado.ListaRespostas.list.push(listaRespostasGrid);

        var listaRespostas = new RespostaMap();

        listaRespostas.Codigo.val = codigo;
        listaRespostas.Resposta.val = _chamadoRespostas.Resposta.val();
        listaRespostas.CodigoFuncionario.val = _chamadoRespostas.FuncionarioResposta.codEntity();
        listaRespostas.DataHora.val = moment().format("DD/MM/YYYY HH:mm:ss");

        _chamado.ListaRespostasNovas.list.push(listaRespostas);

        recarregarGridListaRespostas();
        limparCamposChamadoRespostas();
        $("#" + _chamadoRespostas.Resposta.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

//*******MÉTODOS*******

function buscarFuncionarioLogado() {
    executarReST("PedidoVenda/BuscarFuncionarioLogado", null, function (r) {
        if (r.Success) {
            _chamadoRespostas.FuncionarioResposta.codEntity(r.Data.Codigo);
            _chamadoRespostas.FuncionarioResposta.val(r.Data.Nome);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function VerificaNovasRespostasLancados(codigoResposta) {
    var valido = true;
    $.each(_chamado.ListaRespostasNovas.list, function (i, listaRespostas) {
        if (codigoResposta == listaRespostas.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function recarregarGridListaRespostas() {

    var data = new Array();

    $.each(_chamado.ListaRespostas.list, function (i, listaRespostas) {
        var listaRespostasGrid = new Object();

        listaRespostasGrid.Codigo = listaRespostas.Codigo.val;
        listaRespostasGrid.CodigoChamado = _chamado.Codigo.val();
        listaRespostasGrid.Resposta = listaRespostas.Resposta.val;
        listaRespostasGrid.CodigoFuncionario = listaRespostas.CodigoFuncionario.val;
        listaRespostasGrid.Funcionario = listaRespostas.Funcionario.val;
        listaRespostasGrid.DataHora = listaRespostas.DataHora.val;

        data.push(listaRespostasGrid);
    });

    _gridRespostas.CarregarGrid(data);
}

function limparCamposChamadoRespostas() {
    LimparCampos(_chamadoRespostas);
    buscarFuncionarioLogado();
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}