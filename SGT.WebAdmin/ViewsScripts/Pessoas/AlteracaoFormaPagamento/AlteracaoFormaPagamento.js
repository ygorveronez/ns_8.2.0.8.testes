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
/// <reference path="Cliente.js" />
/// <reference path="../../Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _alteracaoFormaPagamento, _gridGrupoPessoa = null;

var AlteracaoFormaPagamento = function () {
    this.TipoPagamentoRecebimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Forma de pagamento:", idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.map, required: false, text: "Informar Grupo de Pessoas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.ListaGrupoPessoa = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), required: false });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
}

//*******EVENTOS*******
function loadAlteracaoFormaPagamento() {
    HeaderAuditoria("AlteracaoFormaPagamento", _alteracaoFormaPagamento);
    _alteracaoFormaPagamento = new AlteracaoFormaPagamento();
    KoBindings(_alteracaoFormaPagamento, "knockoutAlteracaoFormaPagamento");
    new BuscarTipoPagamentoRecebimento(_alteracaoFormaPagamento.TipoPagamentoRecebimento);
    LoadGridGrupoPessoa();
}

function atualizarClick(e, sender) {

    if (_alteracaoFormaPagamento.GrupoPessoa.basicTable.BuscarRegistros().length == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Pelo menos um grupo de pessoas deve ser adicionado");
        return;
    }

    preencherListaGrupoPessoa();
    Salvar(e, "AlteracaoFormaPagamento/AlterarFormaPagamento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridGrupoPessoa.CarregarGrid([]);
                LimparCampos(_alteracaoFormaPagamento);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function LoadGridGrupoPessoa() {
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverGrupoPessoaClick(_alteracaoFormaPagamento.GrupoPessoa, data);
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", title: "Código", width: "20%", className: "text-align-left" },
        { data: "Descricao", title: "Descrição", width: "80%", className: "text-align-left" }
    ];

    _gridGrupoPessoa = new BasicDataTable(_alteracaoFormaPagamento.GrupoPessoa.idGrid, header, menuOpcoes);
    _alteracaoFormaPagamento.GrupoPessoa.basicTable = _gridGrupoPessoa;
    new BuscarGruposPessoas(_alteracaoFormaPagamento.GrupoPessoa, RetornoInserirGrupoPessoa, false, _gridGrupoPessoa);
    RecarregarListaGrupoPessoa();
}

function RemoverGrupoPessoaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o grupo de pessoas " + sender.Descricao + "?", function () {
        var grupoPessoaGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < grupoPessoaGrid.length; i++) {
            if (sender.Codigo == grupoPessoaGrid[i].Codigo) {
                grupoPessoaGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(grupoPessoaGrid);
    });
}

function RetornoInserirGrupoPessoa(data) {
    if (data == null) {
        return;
    }

    var dataGrid = _gridGrupoPessoa.BuscarRegistros();

    var gruposNaoRepetidos = [];
    for (var grupo of data) {
        let grupoJaAdicionado = dataGrid.find((g => g.Codigo == grupo.Codigo));
        if (!grupoJaAdicionado) {
            gruposNaoRepetidos.push(grupo);
        }
    }

    dataGrid = [].concat(dataGrid, gruposNaoRepetidos);
    _gridGrupoPessoa.CarregarGrid(dataGrid);
}

function RecarregarListaGrupoPessoa() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_alteracaoFormaPagamento.ListaGrupoPessoa.val())) {

        $.each(_alteracaoFormaPagamento.ListaGrupoPessoa.val(), function (i, grupoPessoa) {
            var obj = new Object();

            obj.Codigo = grupoPessoa.Codigo;
            obj.Descricao = grupoPessoa.Descricao;

            data.push(obj);
        });
    }

    _gridGrupoPessoa.CarregarGrid(data);
}

function preencherListaGrupoPessoa() {
    _alteracaoFormaPagamento.ListaGrupoPessoa.list = new Array();

    var grupos = new Array();

    $.each(_alteracaoFormaPagamento.GrupoPessoa.basicTable.BuscarRegistros(), function (i, grupoPessoa) {
        grupos.push(grupoPessoa.Codigo);
    });

    _alteracaoFormaPagamento.ListaGrupoPessoa.val(JSON.stringify(grupos));
}