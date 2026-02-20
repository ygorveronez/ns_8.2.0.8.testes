/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _aprovacao;
var _delegar;
var _detalheAutorizacao;
var _gridAutorizacoes;

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

    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });

    this.ExibirAprovadores = PropertyEntity({
        eventClick: toggleExibirAprovadoresClick, type: types.event, text: ko.pureComputed(function () {
            return this.ExibirAprovadores.val() ? "Ocultar Aprovadores" : "Exibir Aprovadores";
        }, this), visible: ko.observable(false), val: ko.observable(true), def: true
    });
}

var DetalheAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.PodeAprovar = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.justificativa = PropertyEntity({ text: "Justificativa:", val: ko.observable(""), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
    this.MotivoRejeicao = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(false) });
    this.Delegar = PropertyEntity({ eventClick: abrirModalDelegarClick, type: types.event, text: "Delegar", visible: ko.observable(false) });
    this.Aprovar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarContratoClick, text: "Aprovar", visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarContratoClick, text: "Rejeitar", visible: ko.observable(false) });
}

var Delegar = function () {
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Usuário:", idBtnSearch: guid() });

    this.Delegar = PropertyEntity({ type: types.event, eventClick: delegarClick, text: "Delegar" });
}

//*******EVENTOS*******
function loadAprovacao() {
    _aprovacao = new Aprovacao();
    KoBindings(_aprovacao, "knockoutAprovacao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    _delegar = new Delegar();
    KoBindings(_delegar, "knockoutDelegar");

    new BuscarFuncionario(_delegar.Usuario);

    $("#divModalDelegar").on('hidden.bs.modal', function () {
        LimparCampoEntity(_delegar.Usuario);
    });

    // Inicia grids
    GridAprovadores();
}

function toggleExibirAprovadoresClick() {
    var valorAtual = _aprovacao.ExibirAprovadores.val();

    _aprovacao.ExibirAprovadores.val(!valorAtual);
    _aprovacao.DetalheAprovadores.visible(valorAtual);
}

function abrirModalDelegarClick(e) {
    Global.abrirModal('divModalDelegar');
}

function delegarClick() {
    var dados = {
        Contrato: _contratoFreteTransportador.Codigo.val(),
        Usuario: _delegar.Usuario.codEntity()
    };

    if (dados.Usuario == 0)
        return exibirMensagem(tipoMensagem.aviso, "Delegar", "Nenhum usuário selecionado.");

    executarReST("AutorizacaoContratoFreteTransportador/Delegar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                $("#divModalDelegar").modal('hide');
                _gridAutorizacoes.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function reprocessarRegrasClick(e, sender) {
    var dados = {
        Codigo: _contratoFreteTransportador.Codigo.val()
    };
    executarReST("ContratoFreteTransportador/ReprocessarRegras", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PossuiRegra) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguardando aprovação.");
                    EditarContratoFreteTransportador(dados);
                    _gridContratoFreteTransportador.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function aprovarContratoClick(e, sender) {
    var dados = {
        Codigo: _detalheAutorizacao.Codigo.val()
    };

    executarReST("AutorizacaoContratoFreteTransportador/Aprovar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                _gridAutorizacoes.CarregarGrid();
                Global.fecharModal('divModalDetalhesAutorizacao');
                _detalheAutorizacao.Delegar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function rejeitarContratoClick(e, sender) {
    if (_detalheAutorizacao.MotivoRejeicao.visible() == false)
        return _detalheAutorizacao.MotivoRejeicao.visible(true);

    var dados = {
        Codigo: _detalheAutorizacao.Codigo.val(),
        Motivo: _detalheAutorizacao.MotivoRejeicao.val(),
    };

    executarReST("AutorizacaoContratoFreteTransportador/Rejeitar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                _gridAutorizacoes.CarregarGrid();
                _detalheAutorizacao.Delegar.visible(false);
                Global.fecharModal('divModalDetalhesAutorizacao');
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

    BuscarPorCodigo(_detalheAutorizacao, "ContratoFreteTransportador/DetalhesAutorizacao", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                if (!arg.Data.PermitirDelegar)
                    _detalheAutorizacao.Delegar.visible(false);
                else if (_contratoFreteTransportador.Situacao.val() == EnumSituacaoContratoFreteTransportador.AgAprovacao && _PermissaoDelegar)
                    _detalheAutorizacao.Delegar.visible(true);

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

    _gridAutorizacoes = new GridView(_aprovacao.Regras.idGrid, "ContratoFreteTransportador/PesquisaAutorizacoes", _aprovacao, menuOpcoes);
    _gridAutorizacoes.CarregarGrid();
}

function ListarAprovacoes(data) {
    _aprovacao.Codigo.val(data.Codigo);

    if (_contratoFreteTransportador.Situacao.val() == EnumSituacaoContratoFreteTransportador.SemRegra) {
        _aprovacao.PossuiRegras.val(false);
    }
    else {
        _aprovacao.PossuiRegras.val(true);

        PreencherResumo(data.Resumo);
        _gridAutorizacoes.CarregarGrid();
    }
}

function PreencherResumo(data) {
    if (data == null) return;
    PreencherObjetoKnout(_aprovacao, { Data: data });
}

function LimparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao);
    _detalheAutorizacao.MotivoRejeicao.visible(false);
}

function LimparCamposAprovacao() {
    LimparCampos(_detalheAutorizacao);
    LimparCampos(_aprovacao);
    _detalheAutorizacao.Delegar.visible(false);
    GridAprovadores();
}