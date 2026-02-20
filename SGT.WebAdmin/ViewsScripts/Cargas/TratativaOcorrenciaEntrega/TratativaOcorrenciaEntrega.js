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
/// <reference path="../../enumeradores/enumtratativadevolucao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _tratativaOcorrenciaEntrega;
var _pesquisaTratativaOcorrenciaEntrega;
var _gridTratativaOcorrenciaEntrega;
var _configuracaoTratativaDevolucao;

var _tipoDevolucao = [
    { text: "Total", value: "false" },
    { text: "Parcial", value: "true" }
];

var TratativaOcorrenciaEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: "*Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410, tipoEmissaoDocumentoOcorrencia: EnumTipoEmissaoDocumentoOcorrencia.Todos });
    this.Tratativa = PropertyEntity({ val: ko.observable(_configuracaoTratativaDevolucao[0].value), options: _configuracaoTratativaDevolucao, text: "Tratativa:  ", def: _configuracaoTratativaDevolucao[0].value, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoDevolucao = PropertyEntity({ val: ko.observable("Total"), options: _tipoDevolucao, def: "Total", text: "Tipo de Devolução: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.Tratativa.val.subscribe(function (novoValor) {
        if (novoValor == EnumTratativaDevolucao.Revertida) {
            _tratativaOcorrenciaEntrega.TipoDevolucao.enable(false);
        } else
            _tratativaOcorrenciaEntrega.TipoDevolucao.enable(true);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaTratativaOcorrenciaEntrega = function () {
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: "*Tipo da Ocorrência:", idBtnSearch: guid(), issue: 410, tipoEmissaoDocumentoOcorrencia: EnumTipoEmissaoDocumentoOcorrencia.Todos });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTratativaOcorrenciaEntrega.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadTratativaOcorrenciaEntrega() {
    return BuscarTiposTratativas().then(function () {
        _pesquisaTratativaOcorrenciaEntrega = new PesquisaTratativaOcorrenciaEntrega();
        KoBindings(_pesquisaTratativaOcorrenciaEntrega, "knockoutPesquisaTratativaOcorrenciaEntrega", false, _pesquisaTratativaOcorrenciaEntrega.Pesquisar.id);

        _tratativaOcorrenciaEntrega = new TratativaOcorrenciaEntrega();
        KoBindings(_tratativaOcorrenciaEntrega, "knockoutTratativaOcorrenciaEntrega");

        new BuscarTipoOcorrencia(_tratativaOcorrenciaEntrega.TipoOcorrencia, null, null, null, null, null, null, null, true);
        new BuscarTipoOcorrencia(_pesquisaTratativaOcorrenciaEntrega.TipoOcorrencia, null, null, null, null, null, null, null, true);
        buscarTratativaOcorrenciaEntrega();
    });
}

function adicionarClick(e, sender) {
    Salvar(_tratativaOcorrenciaEntrega, "TratativaOcorrenciaEntrega/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTratativaOcorrenciaEntrega.CarregarGrid();
                limparCampostratativaOcorrenciaEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tratativaOcorrenciaEntrega, "TratativaOcorrenciaEntrega/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTratativaOcorrenciaEntrega.CarregarGrid();
                limparCampostratativaOcorrenciaEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tratativaOcorrenciaEntrega, "TratativaOcorrenciaEntrega/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTratativaOcorrenciaEntrega.CarregarGrid();
                    limparCampostratativaOcorrenciaEntrega();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCampostratativaOcorrenciaEntrega();
}

function editartratativaOcorrenciaEntregaClick(itemGrid) {
    // Limpa os campos
    limparCampostratativaOcorrenciaEntrega();

    _tratativaOcorrenciaEntrega.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tratativaOcorrenciaEntrega, "TratativaOcorrenciaEntrega/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaTratativaOcorrenciaEntrega.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tratativaOcorrenciaEntrega.Atualizar.visible(true);
                _tratativaOcorrenciaEntrega.Excluir.visible(true);
                _tratativaOcorrenciaEntrega.Cancelar.visible(true);
                _tratativaOcorrenciaEntrega.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarTratativaOcorrenciaEntrega() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editartratativaOcorrenciaEntregaClick, tamanho: "7", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "TratativaOcorrenciaEntrega/ExportarPesquisa",
        titulo: "Motivo Rejeição"
    };


    // Inicia Grid de busca
    _gridTratativaOcorrenciaEntrega = new GridViewExportacao(_pesquisaTratativaOcorrenciaEntrega.Pesquisar.idGrid, "TratativaOcorrenciaEntrega/Pesquisa", _pesquisaTratativaOcorrenciaEntrega, menuOpcoes, configExportacao);
    _gridTratativaOcorrenciaEntrega.CarregarGrid();
}

function limparCampostratativaOcorrenciaEntrega() {
    _tratativaOcorrenciaEntrega.Atualizar.visible(false);
    _tratativaOcorrenciaEntrega.Cancelar.visible(false);
    _tratativaOcorrenciaEntrega.Excluir.visible(false);
    _tratativaOcorrenciaEntrega.Adicionar.visible(true);
    LimparCampos(_tratativaOcorrenciaEntrega);
}

function BuscarTiposTratativas() {
    var p = new promise.Promise();

    executarReST("TratativaOcorrenciaEntrega/BuscarTiposTratativas", {
        Tipos: JSON.stringify([
            EnumTratativaDevolucao.Rejeitada,
            EnumTratativaDevolucao.Revertida,
            EnumTratativaDevolucao.Reentregue,
            EnumTratativaDevolucao.EntregarEmOutroCliente,
            EnumTratativaDevolucao.DescartarMercadoria,
            EnumTratativaDevolucao.QuebraPeso])
    }, function (r) {
        if (r.Success) {
            _configuracaoTratativaDevolucao = new Array();

            for (var i = 0; i < r.Data.length; i++)
                _configuracaoTratativaDevolucao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }

        p.done();
    });

    return p;
};