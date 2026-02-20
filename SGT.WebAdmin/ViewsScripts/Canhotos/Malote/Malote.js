/// <reference path="Conferencia.js" />
/// <reference path="Detalhes.js" />
/// <reference path="Inconsistencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoMaloteCanhoto.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******

var _malote;
var _pesquisaMalote;
var _gridMalote;
var _DivInconsistencia;
var _situacaoMaloteCanhoto = [
    { text: "Todos", value: "" },
    { text: "Gerado", value: EnumSituacaoMaloteCanhoto.Gerado },
    { text: "Confirmado", value: EnumSituacaoMaloteCanhoto.Confirmado },
    { text: "Inconsistente", value: EnumSituacaoMaloteCanhoto.Inconsistente },
    { text: "Cancelado", value: EnumSituacaoMaloteCanhoto.Cancelado },

];

var Malote = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });

    this.Quantidade = PropertyEntity({ text: "Quantidade Canhotos:" });
    this.Protocolo = PropertyEntity({ text: "Número Protocolo:" });
    this.Data = PropertyEntity({ text: "Data/Hora: " });
    this.Operador = PropertyEntity({ text: "Operador:" });
    this.Origem = PropertyEntity({ text: "Origem:" });
    this.Destino = PropertyEntity({ text: "Destino:" });
    this.Filial = PropertyEntity({ text: "Filial:" });
    this.Motivo = PropertyEntity({ text: "Motivo:" });
    this.tpOperacao = PropertyEntity({ text: "Tipo Operação:" });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.FilialCadastro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid(), visible: ko.observable(true) });

    // CRUD
    this.Conferencia = PropertyEntity({ eventClick: conferenciaClick, type: types.event, text: "Conferir Digitalizações", visible: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(false) });


    this.GerarMalote = PropertyEntity({ eventClick: GerarMaloteClick, type: types.event, text: "Gerar Malote", visible: ko.observable(true) });

    this.DadosCanhoto = PropertyEntity({ text: "*Informação para Baixa: ", idBtnSearch: guid(), eventClick: biparCanhotoMaloteDireto, enable: ko.observable(true), visible: ko.observable(false) });

    this.Confirmar = PropertyEntity({ text: "Confirmar: ", idBtnSearch: guid(), eventClick: confirmarMaloteDireto, enable: ko.observable(true), visible: ko.observable(false) });

}

var PesquisaMalote = function () {
    this.Protocolo = PropertyEntity({ text: "Número Protocolo:", val: ko.observable(""), def: "" });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.NumeroCanhoto = PropertyEntity({ text: "Número Canhoto:", val: ko.observable(""), def: "" });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Emitente:", idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoMaloteCanhoto, text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMalote.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadMalote() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaMalote = new PesquisaMalote();
    KoBindings(_pesquisaMalote, "knockoutPesquisaMalote", false, _pesquisaMalote.Pesquisar.id);

    // Instancia objeto principal
    _malote = new Malote();
    KoBindings(_malote, "knockoutMalote");

    HeaderAuditoria("Malote", _malote);

    if (_CONFIGURACAO_TMS.PermitirCriacaoDiretaMalotes) {
        $("#divNovoMalote").show();
        $("#divDadosMaloteEditar").hide();
        $("#divCadastroMalote").show();

        _malote.Cancelar.visible(false);
        _malote.Imprimir.visible(false);
        _malote.Conferencia.visible(false);

        //_malote.Limpar.visible(true);
        _malote.DadosCanhoto.visible(true);
    }

    // Instancia buscas
    new BuscarClientes(_pesquisaMalote.Emitente);
    new BuscarFuncionario(_pesquisaMalote.Operador, null, null, null, null, null, null, null, null, true);
    new BuscarTransportadores(_pesquisaMalote.Transportador);

    new BuscarTransportadores(_malote.Transportador);
    new BuscarFilial(_malote.FilialCadastro);
    new BuscarTiposOperacao(_malote.TipoOperacao);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaMalote.Transportador.visible(false);
        _malote.Conferencia.visible(false);
    }

    _malote.DadosCanhoto.get$()
        .on("keydown", function (e) {
            var ENTER_KEY = 13;
            var key = e.which || e.keyCode || 0;
            if (key === ENTER_KEY)
                biparCanhotoMaloteDireto();
        });

    _malote.DadosCanhoto.val.subscribe(ChangeDadosCanhoto);

    $.get("Content/Static/Canhotos/DetalheCanhoto.html?dyn=" + guid(), function (data) {
        $("#divDetalhesCanhoto").html(data.replace(/#KnoutDetalhesCanhoto/g, guid()));
        $("#KnoutDetalhesCanhoto > .nav").hide();
        loadDetalhesCanhoto();
        loadInconsistencia();
        loadConferencia();
    });

    // Inicia busca
    BuscarMalote();

    _DivInconsistencia = new bootstrap.Modal(document.getElementById("ModalDivInconsistencia"), { backdrop: true, keyboard: true });
}

function ChangeDadosCanhoto() {
    if (_malote.DadosCanhoto.val() != "" && _malote.DadosCanhoto.val().length == 44) {
        biparCanhotoMaloteDireto();
    }
}

function ImprimirClick(e, sender) {
    executarDownload("MaloteCanhoto/Imprimir", { Codigo: _malote.Codigo.val() });
}

function CancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar o malote?", function () {
        ExcluirPorCodigo(_malote, "MaloteCanhoto/Cancelar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");
                    _gridMalote.CarregarGrid();
                    LimparCamposMalote();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function LimparClick() {
    _gridMalote.CarregarGrid();
    LimparCamposMalote();
}

function editarMaloteClick(itemGrid) {
    // Limpa os campos
    LimparCamposMalote();

    // Seta o codigo do objeto
    _malote.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_malote, "MaloteCanhoto/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaMalote.ExibirFiltros.visibleFade(false);

                editarMalote(arg);

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function confirmarMaloteClick(itemGrid) {
    exibirConfirmacao("Confirmar Malote", "Deseja confirmar recebimento do Malote?", function () {
        var dados = {
            Codigo: itemGrid.Codigo
        };

        executarReST("MaloteCanhoto/ConfirmarMalote", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");
                    _gridMalote.CarregarGrid();

                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}


function confirmarMaloteDireto(e) {
    exibirConfirmacao("Confirmar Malote", "Deseja confirmar recebimento do Malote?", function () {
        var dados = {
            Codigo: e.Codigo.val()
        };

        executarReST("MaloteCanhoto/ConfirmarMalote", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");
                    _gridMalote.CarregarGrid();
                    LimparCamposMalote();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });

}


function inconsistenciaMaloteClick(itemGrid) {
    _inconsistencia.Codigo.val(itemGrid.Codigo);
    _DivInconsistencia.show();
}

function GerarMaloteClick() {
    var valido = ValidarCamposObrigatorios(_malote);

    if (valido) {
        executarReST("MaloteCanhoto/GerarMaloteDireto", RetornarObjetoPesquisa(_malote), function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Malote Gerado com sucesso");
                    PreencherObjetoKnout(_malote, arg);
                    editarMalote(arg);
                    _gridMalote.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);

    } else
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");

}

function biparCanhotoMaloteDireto(e, sender) {
    if (_malote.DadosCanhoto.val() == "") {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe os dados do canhoto");
        return;
    }

    setTimeout(function () {
        executarReST("MaloteCanhoto/BiparCanhotoMaloteDireto", RetornarObjetoPesquisa(_malote), function (arg) {
            _malote.DadosCanhoto.val("");
            if (arg.Success) {
                if (arg.Data !== false) {
                    PreencherObjetoKnout(_malote, arg);
                    editarMalote(arg);
                    $("#" + _malote.DadosCanhoto.id).focus();
                    _gridMalote.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }, 30);
}

function editarMalote(arg) {

    $("#divCadastroMalote").show();
    $("#divDadosMaloteEditar").show();
    $("#divNovoMalote").hide();
    _malote.GerarMalote.visible(false);

    _gridCanhoto.CarregarGrid();

    _malote.DadosCanhoto.visible(false);
    _malote.Confirmar.visible(false);
    _malote.Limpar.visible(true);

    if (EnumSituacaoMaloteCanhoto.Gerado == arg.Data.Situacao) {
        _malote.Cancelar.visible(true);

        if (_CONFIGURACAO_TMS.PermitirCriacaoDiretaMalotes) {
            _malote.DadosCanhoto.visible(true);
            _malote.Confirmar.visible(true);
        }
    }
    else {
        _malote.Cancelar.visible(false);
    }

    if (EnumSituacaoMaloteCanhoto.Cancelado == arg.Data.Situacao)
        _malote.Imprimir.visible(false);
    else
        _malote.Imprimir.visible(true);

    if (EnumSituacaoMaloteCanhoto.Gerado == arg.Data.Situacao)
        _malote.Conferencia.visible(true);
    else
        _malote.Conferencia.visible(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _malote.Cancelar.visible(false);
        _malote.Conferencia.visible(false);
    }
}

function VisibilidadeConfirmar(data) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)
        return EnumSituacaoMaloteCanhoto.Gerado == data.Situacao;
    else
        return false;
}

function VisibilidadeInconsistente(data) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)
        return EnumSituacaoMaloteCanhoto.Gerado == data.Situacao;
    else
        return false;
}

//*******MÉTODOS*******
function BuscarMalote() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Detalhar", id: guid(), evento: "onclick", metodo: editarMaloteClick, tamanho: "20", icone: "" };
    var confirmar = { descricao: "Confirmar", id: guid(), evento: "onclick", metodo: confirmarMaloteClick, visibilidade: VisibilidadeConfirmar, tamanho: "20", icone: "" };
    var inconsistencia = { descricao: "Inconsistente", id: guid(), evento: "onclick", metodo: inconsistenciaMaloteClick, visibilidade: VisibilidadeInconsistente, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [editar, confirmar, inconsistencia],
        tamanho: "7",
        descricao: "Opções"
    };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        menuOpcoes.tipo = TypeOptionMenu.link;
        menuOpcoes.opcoes = [editar];
    }

    // Inicia Grid de busca
    _gridMalote = new GridView(_pesquisaMalote.Pesquisar.idGrid, "MaloteCanhoto/Pesquisa", _pesquisaMalote, menuOpcoes, null);
    _gridMalote.CarregarGrid();
}

function LimparCamposMalote() {
    LimparCampos(_malote);
    _malote.Limpar.visible(false);
    _malote.GerarMalote.visible(true);

    if (_CONFIGURACAO_TMS.PermitirCriacaoDiretaMalotes) {

        $("#divNovoMalote").show();
        $("#divDadosMaloteEditar").hide();
        $("#divCadastroMalote").show();

        _malote.Cancelar.visible(false);
        _malote.Imprimir.visible(false);
        _malote.Conferencia.visible(false);
        _malote.Confirmar.visible(false);

        _malote.DadosCanhoto.visible(true);
        _gridCanhoto.CarregarGrid();
    } else
        $("#divCadastroMalote").hide();

    _pesquisaMalote.ExibirFiltros.visibleFade(true);
}

