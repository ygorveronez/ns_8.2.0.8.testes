/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDescarteLoteProdutoEmbarcador.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _aprovacao;
var _detalheAutorizacao;
var _gridAutorizacoes;
var $detalhesAutorizacao;

var Aprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Situacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Situação:" });
    this.Data = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data:" });
    this.Usuario = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Operador Soliciotou:" });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Produto Embarcador:" });
    this.Quantidade = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Qtd. Descarte:" });
    this.DepositoPosicao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Local Armazenamento:" });
    this.CodigoBarra = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Código de Barra:" });
    this.Justificativa = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Justificativa:" });

    this.DataRetorno = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data do Retorno:" });
    this.Aprovador = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Operador Retorno:" });

    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.ResumoSolicitacao = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(true), def: true });
    this.ResumoRetorno = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(true), def: true });

    this.Regras = PropertyEntity({ type: types.map, idGrid: guid() });

    this.ExibirAprovadores = PropertyEntity({
        eventClick: toggleExibirAprovadoresClick, type: types.event, text: ko.pureComputed(function () {
            return this.ExibirAprovadores.val() ? "Exibir Aprovadores" : "Ocultar Aprovadores";
        }, this), visible: ko.observable(false), val: ko.observable(true), def: true
    });
}

var DetalheAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadAprovacao() {
    _aprovacao = new Aprovacao();
    KoBindings(_aprovacao, "knockoutAprovacao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    // Inicia grids
    GridAprovadores();

    $detalhesAutorizacao = $("#divModalDetalhesAutorizacao");
}

function toggleExibirAprovadoresClick() {
    var valorAtual = _aprovacao.ExibirAprovadores.val();
    _aprovacao.ExibirAprovadores.val(!valorAtual);
}

function detalharAutorizacaoClick(dataRow) {
    _detalheAutorizacao.Codigo.val(dataRow.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "DescarteLoteProdutoEmbarcador/DetalhesAutorizacao", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                $detalhesAutorizacao.modal("show");
                $detalhesAutorizacao.one('hidden.bs.modal', function () {
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
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoes = new GridView(_aprovacao.Regras.idGrid, "DescarteLoteProdutoEmbarcador/PesquisaAutorizacoes", _aprovacao, menuOpcoes);
    _gridAutorizacoes.CarregarGrid();
}

function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}


function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}


function ListarAprovacoes(data) {
    _aprovacao.Codigo.val(data.Codigo);

    if (data.Situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.Finalizado || data.Situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao || data.Situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.Rejeitada) {
        _aprovacao.PossuiRegras.val(true);
        _aprovacao.ExibirAprovadores.visible(true);

        _gridAutorizacoes.CarregarGrid();

        _aprovacao.ResumoSolicitacao.visible(true);
        _aprovacao.ResumoRetorno.visible(true);
        PreencherResumo(data.Resumo);
    }

    if (data.Situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.SemRegra) {
        _aprovacao.PossuiRegras.val(false);

        _aprovacao.ResumoSolicitacao.visible(false);
        _aprovacao.ResumoRetorno.visible(false);
    } else if (data.Situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao) {
        _aprovacao.ExibirAprovadores.val(true);
    } else if (data.Situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.Finalizado) {
        _aprovacao.ExibirAprovadores.val(false);
    } else if (data.Situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.Rejeitada) {
        _aprovacao.ExibirAprovadores.visible(false);
        _aprovacao.ExibirAprovadores.val(true);

        _aprovacao.ResumoSolicitacao.visible(false);
        _aprovacao.ResumoRetorno.visible(false);
    }
}

function PreencherResumo(data) {
    if (data == null) return;

    if (data.ResumoSolicitacao != null)
        PreencherObjetoKnout(_aprovacao, { Data: data.ResumoSolicitacao });
    else
        _aprovacao.ResumoSolicitacao.visible(false);

    if (data.ResumoRetorno != null)
        PreencherObjetoKnout(_aprovacao, { Data: data.ResumoRetorno });
    else
        _aprovacao.ResumoRetorno.visible(false);
}

function LimparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao);
}

function LimparCamposAprovacao() {
    LimparCampos(_detalheAutorizacao);
    LimparCampos(_aprovacao);
    GridAprovadores();
}