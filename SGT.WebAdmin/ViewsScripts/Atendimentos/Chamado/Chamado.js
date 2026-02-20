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
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Consultas/Sistema.js" />
/// <reference path="../../Consultas/Modulo.js" />
/// <reference path="../../Consultas/Tela.js" />
/// <reference path="../../Consultas/TipoAtendimento.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="ChamadoEtapa.js" />
/// <reference path="ChamadoAnexos.js" />
/// <reference path="ChamadoRespostas.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeAtendimento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _prioridadeChamado = [
    { text: "Baixa", value: EnumPrioridadeAtendimento.Baixa },
    { text: "Normal", value: EnumPrioridadeAtendimento.Normal },
    { text: "Alta", value: EnumPrioridadeAtendimento.Alta }
];

var _statusChamado = [
    { text: "Aberto", value: EnumStatusAtendimentoChamado.Aberto },
    { text: "Cancelado", value: EnumStatusAtendimentoChamado.Cancelado },
    { text: "Finalizado", value: EnumStatusAtendimentoChamado.Finalizado }
];

var _prioridadePesquisa = [
    { text: "Todos", value: EnumPrioridadeAtendimento.Todos },
    { text: "Baixa", value: EnumPrioridadeAtendimento.Baixa },
    { text: "Normal", value: EnumPrioridadeAtendimento.Normal },
    { text: "Alta", value: EnumPrioridadeAtendimento.Alta }
];

var _gridChamado;
var _chamado;
var _pesquisaChamado;
var funcionarioChamadoLogado = null;

var PesquisaChamado = function () {
    this.MotivoProblema = PropertyEntity({ text: "Motivo / Problema: " });
    this.Titulo = PropertyEntity({ text: "Título: " });
    this.Prioridade = PropertyEntity({ val: ko.observable(0), options: _prioridadePesquisa, def: 0, text: "Prioridade: " });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Abertura Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Abertura Final: ", getType: typesKnockout.date });

    this.Status = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "Chamado/ObterTodosStatus", params: { Tipo: 0, Ativo: _statusPesquisa.Todos }, text: "Status: ", options: ko.observable(new Array()), visible: ko.observable(true) });

    this.Tela = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tela:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Sistema = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Sistema:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Módulo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitante:", idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridChamado.CarregarGrid();
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

var Chamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número: ", val: ko.observable(""), getType: typesKnockout.int, enable: ko.observable(false) });
    this.Data = PropertyEntity({ text: "Data Abertura: ", enable: ko.observable(false), val: ko.observable(Global.DataHoraAtual()), def: ko.observable(Global.DataHoraAtual()) });
    this.Titulo = PropertyEntity({ text: "*Título/Descrição do Chamado: ", required: true, maxlength: 100 });

    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Solicitante:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tela = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tela:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Módulo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Sistema = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Sistema:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.TipoAtendimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Atendimento:", idBtnSearch: guid(), visible: ko.observable(true), required: false });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    this.Prioridade = PropertyEntity({ val: ko.observable(EnumPrioridadeAtendimento.Normal), options: _prioridadeChamado, def: EnumPrioridadeAtendimento.Normal, text: "*Prioridade: " });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusAtendimentoChamado.Aberto), options: _statusChamado, def: EnumStatusAtendimentoChamado.Aberto, text: "*Status: " });

    this.MotivoProblema = PropertyEntity({ text: "*Motivo / Problema Relatado: ", required: true, maxlength: 5000 });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 5000 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.ListaAnexos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosNovos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosExcluidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.ListaRespostas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaRespostasNovas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaRespostasExcluidas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

//*******EVENTOS*******

function loadChamado() {
    _chamado = new Chamado();
    KoBindings(_chamado, "knockoutChamado");

    _pesquisaChamado = new PesquisaChamado();
    KoBindings(_pesquisaChamado, "knockoutPesquisaChamado", false, _pesquisaChamado.Pesquisar.id);

    HeaderAuditoria("AtendimentoTarefa", _chamado);
    
    new BuscarSistemas(_pesquisaChamado.Sistema);
    new BuscarModulos(_pesquisaChamado.Modulo);
    new BuscarTelas(_pesquisaChamado.Tela);
    new BuscarFuncionario(_pesquisaChamado.Solicitante);

    new BuscarSistemas(_chamado.Sistema, RetornoSelecaoSistema);
    new BuscarModulos(_chamado.Modulo, RetornoSelecaoModulo, null, _chamado.Sistema);
    new BuscarTelas(_chamado.Tela, RetornoSelecaoSistemaModulo, null, _chamado.Sistema, _chamado.Modulo);   
    new BuscarTransportadores(_chamado.Empresa, null, null, true);

    new BuscarTipoAtendimento(_chamado.TipoAtendimento);
    new BuscarFuncionario(_chamado.Solicitante);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        _chamado.Empresa.visible(true);
        _chamado.Empresa.required(true);
    }

    buscarChamados();
    loadEtapaChamado();
    loadChamadoAnexos();
    loadChamadoRespostas();
    PreencheFuncionarioLogado();

}

function RetornoSelecaoSistema(data) {
    _chamado.Sistema.codEntity(data.Codigo);
    _chamado.Sistema.val(data.Descricao);
    LimparCampoEntity(_chamado.Modulo);
}

function RetornoSelecaoModulo(data) {
    if (data.CodigoSistema > 0) {
        LimparCampoEntity(_chamado.Sistema);
        _chamado.Sistema.codEntity(data.CodigoSistema);
        _chamado.Sistema.val(data.Sistema);
    }
    _chamado.Modulo.codEntity(data.Codigo);
    _chamado.Modulo.val(data.Descricao);
}

function RetornoSelecaoSistemaModulo(data) {
    if (data.CodigoSistema > 0) {
        LimparCampoEntity(_chamado.Sistema);
        _chamado.Sistema.codEntity(data.CodigoSistema);
        _chamado.Sistema.val(data.Sistema);
    }
    if (data.CodigoModulo > 0) {
        LimparCampoEntity(_chamado.Modulo);
        _chamado.Modulo.codEntity(data.CodigoModulo);
        _chamado.Modulo.val(data.Modulo);
    }

    _chamado.Tela.codEntity(data.Codigo);
    _chamado.Tela.val(data.Descricao);
}

function adicionarClick(e, sender) {
    if (_chamado.ListaRespostasNovas.list.length > 0)
        e.ListaRespostasNovas.text = JSON.stringify(_chamado.ListaRespostasNovas.list);

    Salvar(e, "Chamado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_chamado.ListaAnexosNovos.list.length > 0)
                    EnviarAnexos(arg.Data.Codigo);
                else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                    _gridChamado.CarregarGrid();
                    limparCamposChamado();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (_chamado.ListaAnexosExcluidos.list.length > 0)
        e.ListaAnexosExcluidos.text = JSON.stringify(_chamado.ListaAnexosExcluidos.list);
    if (_chamado.ListaRespostasExcluidas.list.length > 0)
        e.ListaRespostasExcluidas.text = JSON.stringify(_chamado.ListaRespostasExcluidas.list);
    if (_chamado.ListaRespostasNovas.list.length > 0)
        e.ListaRespostasNovas.text = JSON.stringify(_chamado.ListaRespostasNovas.list);

    Salvar(e, "Chamado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_chamado.ListaAnexosNovos.list.length > 0)
                    EnviarAnexos(_chamado.Codigo.val());
                else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridChamado.CarregarGrid();
                    limparCamposChamado();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Chamado " + _chamado.MotivoProblema.val() + "?", function () {
        ExcluirPorCodigo(_chamado, "Chamado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridChamado.CarregarGrid();
                limparCamposChamado();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposChamado();
}

//*******MÉTODOS*******

function PreencheFuncionarioLogado() {
    var _fillName = function () {
        _chamado.Solicitante.codEntity(funcionarioChamadoLogado.Codigo);
        _chamado.Solicitante.val(funcionarioChamadoLogado.Nome);
    }

    if (funcionarioChamadoLogado != null) _fillName();

    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                funcionarioChamadoLogado = {
                    Codigo: arg.Data.Codigo,
                    Nome: arg.Data.Nome
                };
                _fillName();
            }
        }
    });
}

function EnviarAnexos(codigoChamado) {
    var file;
    var fileCount = _chamado.ListaAnexosNovos.list.length;
    var documentos = new Array();
    for (var i = 0; i < _chamado.ListaAnexosNovos.list.length; i++) {
        file = _chamado.ListaAnexosNovos.list[i].Arquivo;
        var formData = new FormData();
        formData.append("upload", file);
        var data = {
            ListaAnexos: "",
            DescricaoAnexo: _chamado.ListaAnexosNovos.list[i].DescricaoAnexo.val,
            CodigoChamado: codigoChamado
        };
        enviarArquivo("Chamado/EnviarAnexos?callback=?", data, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    documentos.push({ Codigo: codigoChamado });
                    if (documentos.length == fileCount) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                        _gridChamado.CarregarGrid();
                        limparCamposChamado();
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function buscarChamados() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarChamado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridChamado = new GridView(_pesquisaChamado.Pesquisar.idGrid, "Chamado/Pesquisa", _pesquisaChamado, menuOpcoes, null);
    _gridChamado.CarregarGrid();
}

function editarChamado(chamadoGrid) {
    limparCamposChamado();
    _chamado.Codigo.val(chamadoGrid.Codigo);
    BuscarPorCodigo(_chamado, "Chamado/BuscarPorCodigo", function (arg) {
        _pesquisaChamado.ExibirFiltros.visibleFade(false);
        _chamado.Atualizar.visible(true);
        _chamado.Cancelar.visible(true);
        _chamado.Adicionar.visible(false);
        _chamado.Empresa.enable(false);

        recarregarGridListaAnexos();
        recarregarGridListaRespostas();
    }, null);
}

function limparCamposChamado() {
    _chamado.Atualizar.visible(false);
    _chamado.Cancelar.visible(false);
    _chamado.Adicionar.visible(true);
    _chamado.Empresa.enable(true);
    LimparCampos(_chamado);
    _chamado.Data.val(Global.DataHoraAtual());
    limparCamposChamadoAnexos();
    recarregarGridListaAnexos();
    recarregarGridListaRespostas();
    PreencheFuncionarioLogado();
}
