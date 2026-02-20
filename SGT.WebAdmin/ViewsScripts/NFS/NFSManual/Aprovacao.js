/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLancamentoNFSManual.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _aprovacao;
var _detalheAutorizacao;
var _gridAutorizacoes;
var _gridNFSGeradas;

var Aprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Solicitante = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Solicitante:", enable: ko.observable(true) });
    this.DataSolicitacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data da Solicitação:", enable: ko.observable(true) });
    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações:", enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Situação:", enable: ko.observable(true) });

    this.DetalheAprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(false), val: ko.observable(true), def: true });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true });

    this.Regras = PropertyEntity({ type: types.map, idGrid: guid() });
    this.NFSGeradas = PropertyEntity({ type: types.map, idGrid: guid(), visible: ko.observable(false), text: ko.observable("") });

    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
    this.ExibirAprovadores = PropertyEntity({
        eventClick: toggleExibirAprovadoresClick, type: types.event, text: ko.pureComputed(function () {
            return this.ExibirAprovadores.val() ? "Exibir Aprovadores" : "Ocultar Aprovadores";
        }, this), visible: ko.observable(false),  val: ko.observable(true), def: true
    });
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
function loadAprovacao() {
    _aprovacao = new Aprovacao();
    KoBindings(_aprovacao, "knockoutAprovacao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    // Inicia grids
    GridAprovadores();
    GridNFSGeradas();
}

function toggleExibirAprovadoresClick() {
    var valorAtual = _aprovacao.ExibirAprovadores.val();
    _aprovacao.ExibirAprovadores.val(!valorAtual);
    _aprovacao.DetalheAprovadores.visible(valorAtual);
}

function reprocessarRegrasClick(e, sender) {
    executarReST("NFSManual/ReprocessarRegras", { Codigo: _dadosEmissao.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PossuiRegra)
                {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS está aguardando aprovação.");
                    _aprovacao.PossuiRegras.val(true);
                    _gridAutorizacoes.CarregarGrid();
                    _nfsManual.Situacao.val(arg.Data.Situacao);
                    SetarEtapasNFS();
                    PreencherResumo(arg.Data.Resumo);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a NFS.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function detalharAutorizacaoClick(dataRow) {
    _detalheAutorizacao.Codigo.val(dataRow.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "NFSManual/DetalhesAutorizacao", function (arg) {
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
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoes = new GridView(_aprovacao.Regras.idGrid, "NFSManual/PesquisaAutorizacoes", _aprovacao, menuOpcoes);
    _gridAutorizacoes.CarregarGrid();
}

function GridNFSGeradas() {
    var baixarDANFSE = { descricao: "Baixar DANFSE", id: guid(), metodo: baixarDacteClick, icone: "" };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "" };
    let sincronizarDocumento = { descricao: "Sincronizar Documento", id: guid(), metodo: function (datagrid) { sincronizarCTeClick(datagrid); }, visibilidade: VisibilidadeSincronizarDocumento };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDANFSE, baixarXML, sincronizarDocumento] };

    _gridNFSGeradas = new GridView(_aprovacao.NFSGeradas.idGrid, "NFSManual/PesquisaNFSGerados", _aprovacao, menuOpcoes);
}

function baixarXMLCTeClick(e) {
    if ("CodigoNFSe" in e) {
        executarDownload("NFSManual/DownloadXML", {
            Codigo: _dadosEmissao.Codigo.val()
        });
    } else {
        executarDownload("CargaCTe/DownloadXML", {
            CodigoCTe: e.CodigoCTE,
            CodigoEmpresa: e.CodigoEmpresa
        });
    }
}

function baixarDacteClick(e) {
    if ("CodigoNFSe" in e) {
        executarDownload("NFSManual/DownloadDANFSE", {
            Codigo: _dadosEmissao.Codigo.val()
        });
    } else {
        executarDownload("CargaCTe/DownloadDacte", {
            CodigoCTe: e.CodigoCTE,
            CodigoEmpresa: e.CodigoEmpresa
        });
    }
}

function sincronizarCTeClick(e, knout) {
    if (e.SituacaoCTe == EnumStatusCTe.ENVIADO) {
        var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("CargaCTe/SincronizarDocumentoEmProcessamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridNFSGeradas.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}
function VisibilidadeSincronizarDocumento(data) {
    if (data.HabilitarSincronizarDocumento == true) {
        return true;
    } else {
        return false;
    }
}
function ListarAprovacoes(data) {
    _aprovacao.Codigo.val(data.Codigo);
    if (_nfsManual.Situacao.val() == EnumSituacaoLancamentoNFSManual.SemRegra) {
        _aprovacao.PossuiRegras.val(false);

        _aprovacao.NFSGeradas.visible(false);
        _aprovacao.ExibirAprovadores.visible(false);
        _aprovacao.DetalheAprovadores.visible(true);
    } else {
        _aprovacao.PossuiRegras.val(true);

        _aprovacao.NFSGeradas.visible(false);
        _aprovacao.ExibirAprovadores.visible(false);
        _aprovacao.DetalheAprovadores.visible(true);

        PreencherResumo(data.Resumo);
        _gridAutorizacoes.CarregarGrid();
    }

    if (_nfsManual.Situacao.val() == EnumSituacaoLancamentoNFSManual.EmEmissao || _nfsManual.Situacao.val() == EnumSituacaoLancamentoNFSManual.AgIntegracao || _nfsManual.Situacao.val() == EnumSituacaoLancamentoNFSManual.Finalizada || _nfsManual.Situacao.val() == EnumSituacaoLancamentoNFSManual.FalhaIntegracao || _nfsManual.Situacao.val() == EnumSituacaoLancamentoNFSManual.FalhaEmissao) {
        _aprovacao.PossuiRegras.val(true);

        _aprovacao.NFSGeradas.visible(true);
        _aprovacao.ExibirAprovadores.visible(true);
        _aprovacao.DetalheAprovadores.visible(false);

        if (data.CargasMultiCTe)
            _aprovacao.NFSGeradas.text("Notas de Serviço vinculadas à NFS Manual");
        else
            _aprovacao.NFSGeradas.text("");
        _gridNFSGeradas.CarregarGrid();
    }

    
}

function PreencherResumo(data) {
    if (data == null) return;
    PreencherObjetoKnout(_aprovacao, { Data: data });
}

function LimparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao);
}

function LimparCamposAprovacao() {
    LimparCampos(_detalheAutorizacao);
    LimparCampos(_aprovacao);
    GridAprovadores();
}