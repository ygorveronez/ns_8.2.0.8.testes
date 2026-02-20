/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="FluxoCompra.js" />
/// <reference path="AprovacaoRequisicao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _selectedCodigoFluxoCompra;
var _aprovacaoOrdemCompraTratativa;
var _fluxoCompraTratativa;
var _CRUDTratativaDados;
var _gridFluxoCompraTratativa;
var _statusTratativa = EnumTratativaFluxoCompra.Pendente;
var _permiteUsuarioConcluirTratativa;
var _codigoUsuarioLogado;
var _listaUsuariosRequisicao;

var AprovacaoOrdemCompraTratativa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tratativa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.string });
    this.ConcluirTratativa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.bool });
};

var FluxoCompraTratativa = function () {
    this.Grid = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.Tratativas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDTratativaDados = function () {
    this.TratativaText = PropertyEntity({ text: "*Tratativa", required: ko.observable(true), getType: typesKnockout.string, val: ko.observable(""), id: guid(), maxlength: 2000, visible: ko.observable(true) });
    this.TratativaCheckBox = PropertyEntity({ text: "Concluir a Tratativa?", val: ko.observable(false), id: guid(), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.InserirButton = PropertyEntity({ type: types.event, eventClick: InserirTratativaDadosClick, text: "Adicionar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ type: types.event, eventClick: ExluirTratativaClick, text: "Excluir", visible: ko.observable(true) });
};


//*******EVENTOS*******

function LoadFluxoCompraTratativa() {
    _aprovacaoOrdemCompraTratativa = new AprovacaoOrdemCompraTratativa();
    _fluxoCompraTratativa = new FluxoCompraTratativa();
    _CRUDTratativaDados = new CRUDTratativaDados();

    KoBindings(_fluxoCompraTratativa, "knockoutModalFluxoCompraTratativa");
    KoBindings(_CRUDTratativaDados, "knockoutCRUDModalFluxoCompraTratativa");

    LoadGridFluxoCompraTratativa();
    CarregaUsuarioLogado();
}

function LoadGridFluxoCompraTratativa() {

    let menuOpcoes = null;

    CarregaStatusTratativa();

    if (_statusTratativa != EnumTratativaFluxoCompra.Concluido) {
        let excluir = { descricao: "Excluir", id: guid(), metodo: ExluirTratativaClick, icone: "" };
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [excluir], visible: true };
    }

    let header = [
        { data: "Codigo", visible: false },
        { data: "Operador", title: "Operador", width: "20%" },
        { data: "Data", title: "Data", width: "10%" },
        { data: "Tratativa", title: "Tratativa", width: "60%" },
    ];

    _gridFluxoCompraTratativa = new BasicDataTable(_fluxoCompraTratativa.Grid.id, header, menuOpcoes);

    RecarregarGridFluxoCompraTratativas();
}

function CarregarFluxoCompraTratativa() {

    if (_selectedCodigoFluxoCompra == undefined || _selectedCodigoFluxoCompra == null || _selectedCodigoFluxoCompra == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Fluxo de compra não foi informado");
        return;
    }

    executarReST("FluxoCompraTratativa/BuscarPorCodigo", { Codigo: _selectedCodigoFluxoCompra, GravaAuditoria: true }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_fluxoCompraTratativa, r);
                RecarregarGridFluxoCompraTratativas();
                Global.abrirModal("divModalFluxoCompraTratativa");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
    LoadGridFluxoCompraTratativa();
}

function RecarregarGridFluxoCompraTratativas() {
    let data = new Array();

    $.each(_fluxoCompraTratativa.Tratativas.list, function (i, item) {

        let itemGrid = new Object();
        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Operador = item.Operador.val;
        itemGrid.Data = item.Data.val;
        itemGrid.Tratativa = item.Tratativa.val;
        data.push(itemGrid);
    });

    _gridFluxoCompraTratativa.CarregarGrid(data);

    HabilitarCamposFluxoCompraTratativa();

    if (_statusTratativa == EnumTratativaFluxoCompra.Concluido) {
        CancelarTratativaDadosClick();
        CarregarAprovacaoOrdemCompra();
    }
}

function ExluirTratativaClick(data) {

    exibirConfirmacao("Confirmação", "Realmente deseja remover a trativa selecionada?", function () {
        executarReST("FluxoCompraTratativa/Excluir", { Codigo: data.Codigo }, function (r) {
            if (r.Success)
                CarregarFluxoCompraTratativa();
            else
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        });
    });
}

function InserirTratativaDadosClick() {

    _aprovacaoOrdemCompraTratativa.Codigo.val(_selectedCodigoFluxoCompra);
    _aprovacaoOrdemCompraTratativa.Tratativa.val(_CRUDTratativaDados.TratativaText.val());
    _aprovacaoOrdemCompraTratativa.ConcluirTratativa.val(_CRUDTratativaDados.TratativaCheckBox.val());

    if (!ValidarCamposObrigatorios(_CRUDTratativaDados)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    Salvar(_aprovacaoOrdemCompraTratativa, "FluxoCompraTratativa/Inserir", function (arg) {
        if (arg.Success) {
            LimparCamposFluxoCompraTratativa();
            CarregarFluxoCompraTratativa();
            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

    HabilitarCamposFluxoCompraTratativa();
}

function CancelarTratativaDadosClick() {
    Global.fecharModal('divModalFluxoCompraTratativa');
}

////*******MÉTODOS*******

function SelecionarFluxoCompraTratativaClick(data) {

    _selectedCodigoFluxoCompra = data.Codigo;
    _statusTratativa = EnumTratativaFluxoCompra.obterValorAtravesDescricao(data.SituacaoTratativa);
    _listaUsuariosRequisicao = _aprovacaoRequisicao

    CarregarAprovacaoFluxoCompra();
    LimparCamposFluxoCompraTratativa();
    CarregarFluxoCompraTratativa();
}

function RecarregarGridFluxoCompraTratativa() {
    let data = new Array();

    $.each(_fluxoCompraTratativa.Tratativas.list, function (i, item) {
        let itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Numero = item.Numero.val;
        itemGrid.Fornecedor = item.Fornecedor.val;
        itemGrid.Data = item.Data.val;
        itemGrid.DataPrevisaoRetorno = item.DataPrevisaoRetorno.val;
        itemGrid.Situacao = item.Situacao.val;
        itemGrid.SituacaoTratativa = EnumTratativaFluxoCompra.obterDescricao(item.SituacaoTratativa.val);
        itemGrid.ValorTotal = item.ValorTotal.val;

        data.push(itemGrid);
    });

    _gridFluxoCompraTratativa.CarregarGrid(data);
}

function LimparCamposFluxoCompraTratativa() {

    _CRUDTratativaDados.TratativaText.val("");
    _CRUDTratativaDados.TratativaCheckBox.val("");
}

function CarregaStatusTratativa() {
    if (_selectedCodigoFluxoCompra != 0 && _selectedCodigoFluxoCompra != null) {
        executarReST("FluxoCompraTratativa/ObterStatusTratativa", { Codigo: _selectedCodigoFluxoCompra }, function (r) {
            if (r && r.Success && r.Data) {
                _statusTratativa = r.Data.TratativaConcluida;
            } else
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        });

    }
}

function PermiteUsuarioConcluirTratativa() {

    var retorno = true;

    if (_gridSelecaoRequisicoesMercadoria != undefined) {

        var dadosUsuarioRequisicao = _gridSelecaoRequisicoesMercadoria.GridViewTableData();

        //Verifica se o Usuário Logado é o mesmo da Aprovação da Requisição do Fluxo de Compra
        dadosUsuarioRequisicao.forEach(function (registro) {
            if (registro.CodigoUsuario == _codigoUsuarioLogado) {
                retorno = false;
                return;
            }
        });

    }

    return retorno;
}

function ConcluirTratativaAutomaticamente(codigoFluxoCompra, concluirTratativa) {

    var mensagemTratativa = "Status atualizado para concluído automaticamente pelo sistema, devido aprovação/reprovação da ordem de compra.";

    _aprovacaoOrdemCompraTratativa.Codigo.val(codigoFluxoCompra);
    _aprovacaoOrdemCompraTratativa.Tratativa.val(mensagemTratativa);
    _aprovacaoOrdemCompraTratativa.ConcluirTratativa.val(concluirTratativa);

    Salvar(_aprovacaoOrdemCompraTratativa, "FluxoCompraTratativa/Inserir", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function HabilitarCamposFluxoCompraTratativa() {

    CarregaStatusTratativa();

    if (_statusTratativa == EnumTratativaFluxoCompra.Concluido) {
        _CRUDTratativaDados.TratativaCheckBox.visible(false);
        _CRUDTratativaDados.TratativaText.visible(false);
        _CRUDTratativaDados.InserirButton.visible(false);
        _CRUDTratativaDados.Excluir.visible(false);
    } else {

        //Habilita o Checkbox caso já tenha um item na grid e se o usuário
        //logado não for o requisitante da ordem de compra

        if (_fluxoCompraTratativa.Tratativas.list.length > 0 && PermiteUsuarioConcluirTratativa() == true) {
            _CRUDTratativaDados.TratativaCheckBox.visible(true);
        } else {
            _CRUDTratativaDados.TratativaCheckBox.visible(false);
        }

        _CRUDTratativaDados.TratativaText.visible(true);
        _CRUDTratativaDados.InserirButton.visible(true);
        _CRUDTratativaDados.Excluir.visible(true);
    }
}

var CarregaUsuarioLogado = function () {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                _codigoUsuarioLogado = arg.Data.Codigo;
            }
        }
    });
}
