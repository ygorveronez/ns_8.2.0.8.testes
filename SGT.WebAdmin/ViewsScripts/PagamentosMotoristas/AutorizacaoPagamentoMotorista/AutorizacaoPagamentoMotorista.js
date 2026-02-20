/// <reference path="AutorizarRegras.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoMotorista.js" />
/// <reference path="../../Enumeradores/EnumResponsavelOcorrencia.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="AutorizacaoPagamentoMotoristaAnexo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pagamentoMotorista;
var _valores;
var _rejeicao;
var _autorizacao;
var _gridPagamentoMotorista;
var _pesquisaPagamentoMotoristas;

var _responsavelPagamentoMotorista = [
    { text: "Remetente", value: EnumResponsavelOcorrencia.Remetente },
    { text: "Destinatário", value: EnumResponsavelOcorrencia.Destinatario }
];

var _responsavelPagamentoMotorista = [
    { text: "Remetente", value: EnumResponsavelOcorrencia.Remetente },
    { text: "Destinatário", value: EnumResponsavelOcorrencia.Destinatario }
];

var RegraPagamentoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Tomador = PropertyEntity({ text: "Responsável: ", val: ko.observable(EnumResponsavelOcorrencia.Destinatario), options: _responsavelPagamentoMotorista, def: EnumResponsavelOcorrencia.Destinatario, visible: ko.observable(false), permiteSelecionarTomador: ko.observable(false) });
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarPagamentoMotoristaClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
};

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarPagamentoMotoristasSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var PagamentoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorPagamentoMotorista = PropertyEntity({ text: "Valor do Pagamento: ", visible: ko.observable(true), val: ko.observable("") });
    this.NumeroPagamentoMotorista = PropertyEntity({ text: "Número do Pagamento: ", visible: ko.observable(true), val: ko.observable("") });
    this.DataPagamentoMotorista = PropertyEntity({ text: "Data do Pagamento: ", visible: ko.observable(true), val: ko.observable("") });

    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true), val: ko.observable("") });
    this.CodigoCarga = PropertyEntity({ text: "Carga: ", visible: ko.observable(true), val: ko.observable("") });
    this.TipoPagamentoMotorista = PropertyEntity({ text: "Tipo do Pagamento: ", visible: ko.observable(true), val: ko.observable("") });

    this.Motorista = PropertyEntity({ text: "Motorista: ", visible: ko.observable(true), val: ko.observable("") });
    this.Solicitante = PropertyEntity({ text: "Solicitante: ", visible: ko.observable(true), val: ko.observable("") });

    this.Observacao = PropertyEntity({ text: "Observação: ", visible: ko.observable(true), val: ko.observable("") });
    this.MotivoCancelamento = PropertyEntity({ text: "Motivo do Cancelamento: ", visible: ko.observable(true), val: ko.observable("") });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.EtapaAutorizacao = PropertyEntity({ val: ko.observable(0), def: 0 });
};

var PesquisaPagamentoMotoristas = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.NumeroPagamentoMotorista = PropertyEntity({ text: "Número do Pagamento:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPagamentoMotorista.AutorizacaoPendente), options: EnumSituacaoPagamentoMotorista.obterOpcoesPesquisa(), def: EnumSituacaoPagamentoMotorista.AutorizacaoPendente, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoPagamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Pagamento:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarPagamentoMotoristas();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasPagamentoMotoristasClick, text: "Aprovar Pagamentos", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: rejeitarMultiplasRegrasClick, text: "Rejeitar Pagamentos" });
    this.DelegarPagamentoMotorista = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: delegarMultiplasPagamentoMotoristasClick, text: "Delegar Pagamentos", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadAutorizacao() {
    _pagamentoMotorista = new PagamentoMotorista();
    KoBindings(_pagamentoMotorista, "knockoutPagamentoMotorista");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoPagamentoMotorista");

    _pesquisaPagamentoMotoristas = new PesquisaPagamentoMotoristas();
    KoBindings(_pesquisaPagamentoMotoristas, "knockoutPesquisaPagamentoMotorista", false, _pesquisaPagamentoMotoristas.Pesquisar.id);

    // Busca componentes pesquisa
    new BuscarPagamentoMotoristaTipo(_pesquisaPagamentoMotoristas.TipoPagamento);
    new BuscarMotorista(_pesquisaPagamentoMotoristas.Motorista, retornoBuscarMotorista);
    new BuscarFuncionario(_pesquisaPagamentoMotoristas.Usuario);
    new BuscarCentroResultado(_pesquisaPagamentoMotoristas.CentroResultado);
    loadPagamentoMotoristaAnexo();
    loadRegras();

    // Filtrar Alcadas Do Usuario
    FiltrarAlcadasDoUsuario(buscarPagamentoMotoristas);
}

function retornoBuscarMotorista(data) {
    _pesquisaPagamentoMotoristas.Motorista.codEntity(data.Codigo);
    _pesquisaPagamentoMotoristas.Motorista.val(data.Nome);
}

//*******MÉTODOS*******
function buscarPagamentoMotoristas() {
    //-- Cabecalho
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharPagamentoMotorista, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    //-- Reseta
    _pesquisaPagamentoMotoristas.SelecionarTodos.val(false);
    _pesquisaPagamentoMotoristas.AprovarTodas.visible(false);
    _pesquisaPagamentoMotoristas.RejeitarTodas.visible(false);
    _pesquisaPagamentoMotoristas.DelegarPagamentoMotorista.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaPagamentoMotoristas.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var ordenacaoPadrao = { column: 2, dir: orderDir.asc };

    var configExportacao = {
        url: "AutorizacaoPagamentoMotorista/ExportarPesquisa",
        titulo: "Autorização Pagamento"
    };

    _gridPagamentoMotorista = new GridView(_pesquisaPagamentoMotoristas.Pesquisar.idGrid, "AutorizacaoPagamentoMotorista/Pesquisa", _pesquisaPagamentoMotoristas, menuOpcoes, ordenacaoPadrao, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridPagamentoMotorista.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaPagamentoMotoristas.Situacao.val();
    var possuiSelecionado = _gridPagamentoMotorista.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaPagamentoMotoristas.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoPagamentoMotorista.AgAprovacao || situacaoPesquisa == EnumSituacaoPagamentoMotorista.AutorizacaoPendente);
    var situacaoPermiteSelecaoDelegar = (situacaoPesquisa == EnumSituacaoPagamentoMotorista.AgAprovacao || situacaoPesquisa == EnumSituacaoPagamentoMotorista.AutorizacaoPendente);

    // Esconde todas opções
    _pesquisaPagamentoMotoristas.AprovarTodas.visible(false);
    _pesquisaPagamentoMotoristas.RejeitarTodas.visible(false);
    _pesquisaPagamentoMotoristas.DelegarPagamentoMotorista.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaPagamentoMotoristas.AprovarTodas.visible(true);
            _pesquisaPagamentoMotoristas.RejeitarTodas.visible(true);
        }
        if (situacaoPermiteSelecaoDelegar) {
            _pesquisaPagamentoMotoristas.DelegarPagamentoMotorista.visible(false);
        }
    }
}

function rejeitarPagamentoMotoristasSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas os pagamentos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaPagamentoMotoristas);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Justificativa = rejeicao.Justificativa;
        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaPagamentoMotoristas.SelecionarTodos.val();
        dados.PagamentoMotoristasSelecionadas = JSON.stringify(_gridPagamentoMotorista.ObterMultiplosSelecionados());
        dados.PagamentoMotoristasNaoSelecionadas = JSON.stringify(_gridPagamentoMotorista.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPagamentoMotorista/ReprovarMultiplasPagamentoMotoristas", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de pagamentos foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de pagamento foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    buscarPagamentoMotoristas();
                    cancelarRejeicaoSelecionadosClick();

                    if (!string.IsNullOrWhiteSpace(arg.Data.MensagemRetorno))
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.MensagemRetorno);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function rejeitarMultiplasRegrasClick() {
    LimparCampos(_rejeicao);
    Global.abrirModal('divModalRejeitarPagamentoMotorista');
}

function delegarMultiplasPagamentoMotoristasClick() {
    Global.abrirModal('divModalDelegarPagamentoMotorista');
}

function aprovarMultiplasPagamentoMotoristasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas os pagamentos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaPagamentoMotoristas);

        dados.SelecionarTodos = _pesquisaPagamentoMotoristas.SelecionarTodos.val();
        dados.PagamentoMotoristasSelecionadas = JSON.stringify(_gridPagamentoMotorista.ObterMultiplosSelecionados());
        dados.PagamentoMotoristasNaoSelecionadas = JSON.stringify(_gridPagamentoMotorista.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPagamentoMotorista/AprovarMultiplasPagamentoMotoristas", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de pagamentos foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de pagamento foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    buscarPagamentoMotoristas();

                    if (!string.IsNullOrWhiteSpace(arg.Data.MensagemRetorno))
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.MensagemRetorno);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharPagamentoMotorista(ocorrenciaGrid) {
    limparCamposPagamentoMotorista();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaPagamentoMotoristas);
    _pagamentoMotorista.Codigo.val(ocorrenciaGrid.Codigo);
    _pagamentoMotorista.Usuario.val(pesquisa.Usuario);
    _pagamentoMotorista.EtapaAutorizacao.val(pesquisa.EtapaAutorizacao);

    BuscarPorCodigo(_pagamentoMotorista, "AutorizacaoPagamentoMotorista/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                var data = arg.Data;
                AtualizarGridRegras();

                if (arg.Data.PermiteSelecionarTomador) {
                    _autorizacao.Tomador.val(arg.Data.Tomador);
                    _autorizacao.Tomador.permiteSelecionarTomador(true);
                }
                else
                    _autorizacao.Tomador.permiteSelecionarTomador(false);

                _anexo.Anexos.val(data.Anexos);

                // Abre modal da ocorrencia
                Global.abrirModal("divModalPagamentoMotorista");
                $("#divModalPagamentoMotorista").one('hidden.bs.modal', function () {
                    limparCamposPagamentoMotorista();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);
    Global.fecharModal("divModalRejeitarPagamentoMotorista");
}

function limparCamposPagamentoMotorista() {
    resetarTabs();
    limparCamposAnexo();
    limparRegras();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function FiltrarAlcadasDoUsuario(callback) {
    // Oculta campos conforme configurações
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario) {
        executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false && arg.Data != null) {
                    _pesquisaPagamentoMotoristas.Usuario.codEntity(arg.Data.Codigo);
                    _pesquisaPagamentoMotoristas.Usuario.val(arg.Data.Nome);
                    callback();
                }
            }
        })
    } else {
        callback();
    }
}