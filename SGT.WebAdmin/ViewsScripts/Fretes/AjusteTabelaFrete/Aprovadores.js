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
/// <reference path="AjusteTabelaFrete.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _detalhesAjuste;
var _gridAutorizacoes;
var _detalheAutorizacao;

var DetalheAjuste = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DetalhesAjuste = PropertyEntity({ visible: ko.observable(true) });

    this.Numero = PropertyEntity({ text: "Número:" });
    this.Situacao = PropertyEntity({ text: "Situação:" });
    this.Criador = PropertyEntity({ text: "Criador:" });
    this.DataCriacao = PropertyEntity({ text: "Data Criação:" });
    this.NumeroAprovadores = PropertyEntity({ text: "Número Aprovadores:" });
    this.Aprovacoes = PropertyEntity({ text: "Aprovações:" });
    this.Reprovacoes = PropertyEntity({ text: "Reprovações:" });

    this.UsuariosAutorizadores = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });
    this.Reprocessar = PropertyEntity({ eventClick: reprocessarClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });

    // Mensagem da etapa
    this.MensagemEtapaSemRegra = PropertyEntity({ type: types.local, visible: ko.observable(false), title: "Autorização Pendente", text: '<b class="margin-bottom-10" style="display:block">Nenhuma regra encontrada.</b><b>O reajuste permanecerá aguardando autorização.</b>' });
}

var DetalheAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Justificativa = PropertyEntity({ text: "Justificativa:", val: ko.observable(""), visible: ko.observable(true) });
    this.MotivoRejeicao = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: "Observação:", val: ko.observable(""), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadAutorizadores() {
    _detalhesAjuste = new DetalheAjuste();
    KoBindings(_detalhesAjuste, "knockoutAjusteAprovacao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    // Grid so aprovadores
    GridAprovadores();
}

function reprocessarClick() {
    BuscarRegrasDaEtapa();
}

function detalharAutorizacaoClick(dataRow) {
    _detalheAutorizacao.Codigo.val(dataRow.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "AjusteTabelaFrete/DetalhesAutorizacao", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                Global.abrirModal('divModalDetalhesAutorizacao');
                $("#divModalDetalhesAutorizacao").one('hidden.bs.modal', function () {
                    LimparCamposDetalheAutorizacao();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function GridAprovadores() {
    //-- Grid autorizadores
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "4", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    _gridAutorizacoes = new GridView(_detalhesAjuste.UsuariosAutorizadores.idGrid, "AjusteTabelaFrete/ConsultarAutorizacoes", _ajusteTabelaFrete, menuOpcoes);
}

function BuscarRegrasDaEtapa() {
    var dados = {
        AjusteTabelaFrete: _ajusteTabelaFrete.Codigo.val()
    };
    executarReST("AjusteTabelaFrete/AtualizarRegrasEtapas", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _ajusteTabelaFrete.Situacao.val(arg.Data.Situacao);
                setarEtapaAjuste();
                _gridAutorizacoes.CarregarGrid();
                _gridAjusteTabelaFrete.CarregarGrid();
            } else if (arg.Msg != null) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                EtapaSemRegra();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function LimparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao)
}