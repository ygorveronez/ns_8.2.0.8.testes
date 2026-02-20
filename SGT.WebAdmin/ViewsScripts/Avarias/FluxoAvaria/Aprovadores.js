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
/// <reference path="SolicitacaoAvaria.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _detalhesAprovacaoSolicitacao;
var _gridAutorizacoes;
var _detalheAutorizacao;

var DetalhesFluxoAprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DetalhesAvaria = PropertyEntity({ visible: ko.observable(true) });
    
    this.Solicitante = PropertyEntity({ text: "Solicitante:", val: ko.observable("") });
    this.DataSolicitacao = PropertyEntity({ text: "Data Solicitação:", val: ko.observable("") });
    this.NumeroAprovadores = PropertyEntity({ text: "Número de Aprovadores:", val: ko.observable("") });
    this.Aprovacoes = PropertyEntity({ text: "Aprovações:", val: ko.observable("") });
    this.Reprovacoes = PropertyEntity({ text: "Reprovações:", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.ResponsavelLote = PropertyEntity({ text: "Responsável Lote:", val: ko.observable("") });

    this.UsuariosAutorizadores = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });

    // Mensagem da etapa
    this.MensagemEtapaSemRegra = PropertyEntity({ type: types.local, visible: ko.observable(false), title: "Autorização Pendente", text: '<b class="margin-bottom-10" style="display:block">Nenhuma regra encontrada.</b><b>A solicitação permanecerá aguardando autorização.</b>' });
}

var DetalheAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Justificativa = PropertyEntity({ text: "Justificativa:", val: ko.observable(""), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadAutorizadores() {
    _detalhesFluxoAprovacao = new DetalhesFluxoAprovacao();
    KoBindings(_detalhesFluxoAprovacao, "knockoutAvariaAprovacao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    // Grid so aprovadores
    GridAprovadores();
}


function detalharAutorizacaoClick(dataRow) {
    _detalheAutorizacao.Codigo.val(dataRow.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "SolicitacaoAvaria/DetalhesAutorizacao", function (arg) {
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

function buscarRegrasOcorrenciaClick() {
    BuscarRegrasDaEtapa();
}

//*******MÉTODOS*******

function GridAprovadores() {
    //-- Grid autorizadores
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "4", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    _gridAutorizacoes = new GridView(_detalhesFluxoAprovacao.UsuariosAutorizadores.idGrid, "FluxoAvaria/ConsultarAutorizacoes", _fluxoAvaria, menuOpcoes);
}

function BuscarRegrasDaEtapa() {
    var dados = {
        Avaria: _fluxoAvaria.Codigo.val()
    };
    executarReST("FluxoAvaria/AtualizarRegrasEtapas", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _fluxoAvaria.Situacao.val(arg.Data.Situacao);
                setarEtapasAvaria();
                _CRUDAvaria.Reprocessar.visible(false);
                _gridAutorizacoes.CarregarGrid();
                _gridAvaria.CarregarGrid()
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