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
/// <reference path="../../../js/Global/PermissoesPersonalizadas.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cargo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoComissaoFuncionario.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="ComissaoFuncionarioMotorista.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridComissaoFuncionario;
var _comissaoFuncionario;
var _CRUDcomissaoFuncionario;
var _pesquisaComissaoFuncionario;
var _PermissoesPersonalizadas;

var _SituacaoComissaoFuncionario = [
    { text: "Todas", value: EnumSituacaoComissaoFuncionario.todos },
    { text: "Em geração", value: EnumSituacaoComissaoFuncionario.EmGeracao },
    { text: "Gerada", value: EnumSituacaoComissaoFuncionario.Gerada },
    { text: "Finalizada", value: EnumSituacaoComissaoFuncionario.Finalizada },
    { text: "Cancelada", value: EnumSituacaoComissaoFuncionario.Cancelada },
    { text: "Falha na geração", value: EnumSituacaoComissaoFuncionario.FalhaNaGeracao }
];

var PesquisaComissaoFuncionario = function () {

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.SituacaoComissaoFuncionario = PropertyEntity({ val: ko.observable(EnumSituacaoComissaoFuncionario.todos), options: _SituacaoComissaoFuncionario, def: EnumSituacaoComissaoFuncionario.todos, text: ko.observable("Situação: ") });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridComissaoFuncionario.CarregarGrid();
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

var ComissaoFuncionario = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoComissaoFuncionario = PropertyEntity({ val: ko.observable(EnumSituacaoComissaoFuncionario.todos), options: _SituacaoComissaoFuncionario, def: EnumSituacaoComissaoFuncionario.todos, text: "Situação: " });

    this.DataInicio = PropertyEntity({ text: "*Data Início: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "*Data Final: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.NumeroDiasEmViagem = PropertyEntity({ text: "Dias em viagem:", val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 3, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.PercentualComissao = PropertyEntity({ text: "*Percentual:", val: ko.observable(_CONFIGURACAO_TMS.Percentual), def: "", getType: typesKnockout.decimal, maxlength: 5, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.PercentualBaseCalculoComissao = PropertyEntity({ text: "*Percentual da Base de Cálculo:", val: ko.observable(_CONFIGURACAO_TMS.PercentualDaBaseDeCalculo), def: "", getType: typesKnockout.decimal, maxlength: 6, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorDiaria = PropertyEntity({ text: "Valor da Diária:", val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 6, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.MensagemFalhaGeracao = PropertyEntity({});
    this.MesagemBaseCalculoComissao = PropertyEntity({ text: ko.observable("*Mensagem da Base de Cálculo: "), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: "*Localidade:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.CargoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "Cargo:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true), val: ko.observable("") });
    this.PercentualGerado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Centro Resultado"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarUsoCentroResultadoComissaoMotorista) });
    this.ImportarPlanilhaListagemMotoristas = PropertyEntity({ text: "Importar planilha com listagem dos motoristas?", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.IncluirCargasAberto = PropertyEntity({ text: "Incluir cargas em aberto", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DataInicioCargasAberto = PropertyEntity({ text: "*Data Início: ", getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataFimCargasAberto = PropertyEntity({ text: "*Data Final: ", dateRangeInit: this.DataInicioCargasAberto, getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataInicioCargasAberto.dateRangeLimit = this.DataFimCargasAberto;
    this.DataFimCargasAberto.dateRangeLimit = this.DataInicioCargasAberto;

    this.IncluirCargasAberto.val.subscribe((novoValor) => {
        this.DataInicioCargasAberto.required(novoValor);
        this.DataFimCargasAberto.required(novoValor);

        this.DataInicioCargasAberto.visible(novoValor);
        this.DataFimCargasAberto.visible(novoValor);

        if (!novoValor) {
            this.DataInicioCargasAberto.val("");
            this.DataFimCargasAberto.val("");
        }
    });

    this.ImportarPlanilhaListagemMotoristas.val.subscribe((novoValor) => {
        if (_comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.AgImportacaoPlanilha)
            _CRUDcomissaoFuncionario.ImportarPlanilhaMotoristas.visible(novoValor);
        else 
            _CRUDcomissaoFuncionario.ImportarPlanilhaMotoristas.visible(false);

    });
}

var CRUDComissaoFuncionario = function () {

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Gerar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
    this.MandarGerarNovamente = PropertyEntity({ eventClick: gerarNovamenteClick, type: types.event, text: "Solicitar Novamente a Geração", visible: ko.observable(false), enable: ko.observable(true) });
    this.GerarNovaComissao = PropertyEntity({ eventClick: gerarNovaComissaoClick, type: types.event, text: "Limpar (Criar Novo)", visible: ko.observable(false), enable: ko.observable(true) });
    this.ReAbrirComissao = PropertyEntity({ eventClick: reAbrirComissaoClick, type: types.event, text: "Reabrir", visible: ko.observable(false), enable: ko.observable(true) });
    this.FinalizarComissao = PropertyEntity({ eventClick: finalizarComissaoClick, type: types.event, text: "Finalizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.BaixarPDFComissoes = PropertyEntity({ eventClick: BaixarPDFComissoesClick, type: types.event, text: "PDF", visible: ko.observable(false), enable: ko.observable(true) });
    this.BaixarExcelComissoes = PropertyEntity({ eventClick: BaixarExcelComissoesClick, type: types.event, text: "Excel", visible: ko.observable(false), enable: ko.observable(true) });
    this.ExportarComissoes = PropertyEntity({ eventClick: ExportarComissoesClick, type: types.event, text: "Exportar Comissões", visible: ko.observable(false), enable: ko.observable(true) });
    this.ImportarPlanilhaMotoristas = PropertyEntity({
        type: types.local,
        text: "Importar Planilha de motoristas",
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default me-2",
        UrlImportacao: "ComissaoFuncionario/Importar?CodigoComissaoFuncionario=" + _comissaoFuncionario.Codigo.val(),
        UrlConfiguracao: "ComissaoFuncionario/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O085_ComissaoFuncionario,
        CallbackImportacao: function () {
            PreecherDadosComissao();
            _gridMotoristas.CarregarGrid();
        }
    });

}

//*******EVENTOS*******


function loadComissaoFuncionario() {

    _comissaoFuncionario = new ComissaoFuncionario();
    KoBindings(_comissaoFuncionario, "knockoutCadastroComissaoFuncionario");

    _CRUDcomissaoFuncionario = new CRUDComissaoFuncionario();
    KoBindings(_CRUDcomissaoFuncionario, "knockoutCRUDComissaoFuncionario");

    HeaderAuditoria("ComissaoFuncionario", _comissaoFuncionario);

    _pesquisaComissaoFuncionario = new PesquisaComissaoFuncionario();
    KoBindings(_pesquisaComissaoFuncionario, "knockoutPesquisaComissaoFuncionario", false, _pesquisaComissaoFuncionario.Pesquisar.id);

    new BuscarLocalidades(_comissaoFuncionario.Localidade);
    new BuscarMotoristas(_comissaoFuncionario.Motorista, callbackMotorista);
    new BuscarCargos(_comissaoFuncionario.CargoMotorista);
    BuscarCentroResultado(_comissaoFuncionario.CentroResultado);

    buscarComissaoFuncionarios();
    loadComissaoFuncionarioMotorista();
    loadComissaoFuncionarioMotoristaDocumento();

    if (_notificacaoGlobal != null && _notificacaoGlobal.CodigoObjeto.val() > 0) {
        _comissaoFuncionario.Codigo.val(_notificacaoGlobal.CodigoObjeto.val())
        iniciarControleManualRequisicao();
        PreecherDadosComissao(function () {
            finalizarControleManualRequisicao();
        });
    }

    validarPermissoesPersonalizadas();

    if (_CONFIGURACAO_TMS.UtilizarComissaoPorCargo) {
        _comissaoFuncionario.NumeroDiasEmViagem.required(false);
        _comissaoFuncionario.NumeroDiasEmViagem.visible(false);
        _comissaoFuncionario.PercentualComissao.required(false);
        _comissaoFuncionario.PercentualComissao.visible(false);
        _comissaoFuncionario.PercentualBaseCalculoComissao.required(false);
        _comissaoFuncionario.PercentualBaseCalculoComissao.visible(false);
        _comissaoFuncionario.ValorDiaria.required(false);
        _comissaoFuncionario.ValorDiaria.visible(false);
        _comissaoFuncionario.Localidade.required(false);
        _comissaoFuncionario.Localidade.visible(false);
        _comissaoFuncionario.CargoMotorista.visible(true);

        _comissaoFuncionario.MesagemBaseCalculoComissao.required(false);
        _comissaoFuncionario.MesagemBaseCalculoComissao.text("Observação");
        document.getElementById("divTitulo").innerHTML = "Premiações";
        document.getElementById("divPesquisa").innerHTML = "Pesquisar Premiações";
        document.getElementById("divAba").innerHTML = "Dados da Premição";
    }
}
function callbackMotorista(motorista) {
    _comissaoFuncionario.Motorista.val(motorista.Descricao);
    _comissaoFuncionario.Motorista.codEntity(motorista.Codigo);

    if (motorista.Codigo > 0) {
        _comissaoFuncionario.ImportarPlanilhaListagemMotoristas.val(false);
        _comissaoFuncionario.ImportarPlanilhaListagemMotoristas.visible(false);
    }
    else {
        _comissaoFuncionario.ImportarPlanilhaListagemMotoristas.visible(true);

    }
}

function validarPermissoesPersonalizadas() {
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Criar, _PermissoesPersonalizadas)) {
        _CRUDcomissaoFuncionario.Adicionar.enable(false);
        _CRUDcomissaoFuncionario.MandarGerarNovamente.enable(false);
    }

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Alterar, _PermissoesPersonalizadas))
        _CRUDcomissaoFuncionario.Atualizar.enable(false);

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Cancelar, _PermissoesPersonalizadas))
        _CRUDcomissaoFuncionario.Cancelar.enable(false);

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ReAbrir, _PermissoesPersonalizadas))
        _CRUDcomissaoFuncionario.ReAbrirComissao.enable(false);

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Finalizar, _PermissoesPersonalizadas))
        _CRUDcomissaoFuncionario.FinalizarComissao.enable(false);
}

function ExportarComissoesClick(e, sender) {
    var data = { Codigo: _comissaoFuncionario.Codigo.val() };

    executarDownload("ComissaoFuncionario/ExportarCSV", data);
}
function BaixarPDFComissoesClick(e, sender) {
    BaixarRelatorioComissoes(EnumTipoArquivoRelatorio.PDF);
}

function BaixarExcelComissoesClick(e, sender) {
    BaixarRelatorioComissoes(EnumTipoArquivoRelatorio.XLS);
}

function BaixarRelatorioComissoes(tipoArquivo) {
    var data = { Codigo: _comissaoFuncionario.Codigo.val(), TipoArquivo: tipoArquivo };

    executarReST("ComissaoFuncionario/BaixarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    })
}

function adicionarClick(e, sender) {
    iniciarControleManualRequisicao();
    Salvar(_comissaoFuncionario, "ComissaoFuncionario/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _comissaoFuncionario.Codigo.val(arg.Data);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "cadastrado");
                BuscarProcessamentosPendentes();
                _gridComissaoFuncionario.CarregarGrid(function () {
                    PreecherDadosComissao(function () {
                        finalizarControleManualRequisicao();
                        $("#myTab a:eq(1)").tab("show");
                    });
                });
                _CRUDcomissaoFuncionario = new CRUDComissaoFuncionario();
                KoBindings(_CRUDcomissaoFuncionario, "knockoutCRUDComissaoFuncionario");

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                finalizarControleManualRequisicao();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            finalizarControleManualRequisicao();
        }
    }, sender, function () {
        finalizarControleManualRequisicao();
    });
}

function gerarNovamenteClick(e, sender) {
    iniciarControleManualRequisicao();
    Salvar(_comissaoFuncionario, "ComissaoFuncionario/TentarGerarNovamente", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitado com Sucesso.");
                BuscarProcessamentosPendentes();
                _gridComissaoFuncionario.CarregarGrid(function () {
                    PreecherDadosComissao(function () {
                        finalizarControleManualRequisicao();
                        $("#myTab a:eq(1)").tab("show");
                    });
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                finalizarControleManualRequisicao();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            finalizarControleManualRequisicao();
        }
    }, sender, function () {
        finalizarControleManualRequisicao();
    });
}

function atualizarClick(e, sender) {
    Salvar(_comissaoFuncionario, "ComissaoFuncionario/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function gerarNovaComissaoClick(e) {
    limparCamposComissaoFuncionario();
}

function finalizarComissaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar a comissão?", function () {
        iniciarControleManualRequisicao();
        Salvar(_comissaoFuncionario, "ComissaoFuncionario/FinalizarComissao", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com Sucesso.");
                    _gridComissaoFuncionario.CarregarGrid(function () {
                        PreecherDadosComissao(function () {
                            finalizarControleManualRequisicao();
                        });
                    });
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    finalizarControleManualRequisicao
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                finalizarControleManualRequisicao
            }
        }, sender, function () {
            finalizarControleManualRequisicao();
        });
    });
}

function reAbrirComissaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja reabrir a comissão?", function () {
        iniciarControleManualRequisicao();
        Salvar(_comissaoFuncionario, "ComissaoFuncionario/ReabrirComissao", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Re-aberto com Sucesso.");
                    _gridComissaoFuncionario.CarregarGrid(function () {
                        PreecherDadosComissao(function () {
                            finalizarControleManualRequisicao();
                        });
                    });
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    finalizarControleManualRequisicao();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                finalizarControleManualRequisicao();
            }
        }, sender, function () {
            finalizarControleManualRequisicao();
        });
    });
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja reabrir a comissão?", function () {
        Salvar(_comissaoFuncionario, "ComissaoFuncionario/CancelarComissao", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelada com Sucesso.");
                    _gridComissaoFuncionario.CarregarGrid();
                    limparCamposComissaoFuncionario();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

//*******MÉTODOS*******


function buscarComissaoFuncionarios() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarComissaoFuncionario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridComissaoFuncionario = new GridView(_pesquisaComissaoFuncionario.Pesquisar.idGrid, "ComissaoFuncionario/Pesquisa", _pesquisaComissaoFuncionario, menuOpcoes, null);
    _gridComissaoFuncionario.CarregarGrid();
}

function editarComissaoFuncionario(comissaoFuncionarioGrid, callbackGrid) {
    limparCamposComissaoFuncionario();
    _comissaoFuncionario.Codigo.val(comissaoFuncionarioGrid.Codigo);

    PreecherDadosComissao(function () {
        _pesquisaComissaoFuncionario.ExibirFiltros.visibleFade(false);
    });

    _CRUDcomissaoFuncionario = new CRUDComissaoFuncionario();
    KoBindings(_CRUDcomissaoFuncionario, "knockoutCRUDComissaoFuncionario");
    
}

function PreecherDadosComissao(callback) {

    BuscarPorCodigo(_comissaoFuncionario, "ComissaoFuncionario/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            $("#liMotoristas").show();
            _comissaoFuncionario.DataInicio.enable(false);
            _comissaoFuncionario.DataFim.enable(false);
            _comissaoFuncionario.NumeroDiasEmViagem.enable(false);
            _comissaoFuncionario.PercentualComissao.enable(false);
            _comissaoFuncionario.PercentualBaseCalculoComissao.enable(false);
            _comissaoFuncionario.ValorDiaria.enable(false);
            _comissaoFuncionario.Localidade.enable(false);
            _comissaoFuncionario.CargoMotorista.enable(false);
            _comissaoFuncionario.Motorista.enable(false);
            _comissaoFuncionario.MesagemBaseCalculoComissao.enable(false);

            
            ocultarCamposPadroes();
            _CRUDcomissaoFuncionario.GerarNovaComissao.visible(true);
            _CRUDcomissaoFuncionario.Adicionar.visible(false);
            _CRUDcomissaoFuncionario.ImportarPlanilhaMotoristas.visible(false);
            _comissaoFuncionarioMotorista.Motorista.visible(false);
            _comissaoFuncionarioMotorista.Motorista.visibleGeracao(false);
            _comissaoFuncionarioMotorista.Motorista.visibleFalha(false);

            habilitarEditarGridFuncionarioMotorista();
            habilitarEditarGridDocumentos();

            var carragerGridMotoristas = false;
            if (_comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.Gerada) {
                _CRUDcomissaoFuncionario.Cancelar.visible(true);
                _CRUDcomissaoFuncionario.FinalizarComissao.visible(true);
                _CRUDcomissaoFuncionario.BaixarPDFComissoes.visible(true);
                _CRUDcomissaoFuncionario.BaixarExcelComissoes.visible(true);
                _CRUDcomissaoFuncionario.ExportarComissoes.visible(true);
                _CRUDcomissaoFuncionario.Atualizar.visible(true);
                _comissaoFuncionario.Localidade.enable(true);                
                _comissaoFuncionario.MesagemBaseCalculoComissao.enable(true);
                carragerGridMotoristas = true;
            } else if (_comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.Finalizada) {
                _CRUDcomissaoFuncionario.ReAbrirComissao.visible(true);
                _CRUDcomissaoFuncionario.BaixarPDFComissoes.visible(true);
                _CRUDcomissaoFuncionario.BaixarExcelComissoes.visible(true);
                _CRUDcomissaoFuncionario.ExportarComissoes.visible(true);
                desativarEditarGridFuncionarioMotorista();
                desativarEditarGridDocumentos();
                carragerGridMotoristas = true;
            } else if (_comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.FalhaNaGeracao) {
                _CRUDcomissaoFuncionario.Cancelar.visible(true);
                _CRUDcomissaoFuncionario.MandarGerarNovamente.visible(true);
                _comissaoFuncionarioMotorista.MensagemFalhaGeracao.val(_comissaoFuncionario.MensagemFalhaGeracao.val());
                _comissaoFuncionarioMotorista.Motorista.visibleFalha(true);
            } else if (_comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.AgGeracao || _comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.EmGeracao) {
                _comissaoFuncionarioMotorista.Motorista.visibleGeracao(true);
                SetarPercentualProcessamento(_comissaoFuncionario.PercentualGerado.val());
            } else if (_comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.Cancelada) {
                desativarEditarGridFuncionarioMotorista();
                desativarEditarGridDocumentos();
                carragerGridMotoristas = true;
            } else if (_comissaoFuncionario.SituacaoComissaoFuncionario.val() == EnumSituacaoComissaoFuncionario.AgImportacaoPlanilha) {
                carragerGridMotoristas = true;
                _CRUDcomissaoFuncionario.Cancelar.visible(true);
                _CRUDcomissaoFuncionario.ImportarPlanilhaMotoristas.visible(true);
            }

            validarPermissoesPersonalizadas();
            if (!carragerGridMotoristas) {
                if (callback != null)
                    callback();
            } else {
                _comissaoFuncionarioMotorista.Motorista.visible(true);
                carregarComissoesMotoristas(callback);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}


function ocultarCamposPadroes() {
    _CRUDcomissaoFuncionario.Atualizar.visible(false);
    _CRUDcomissaoFuncionario.FinalizarComissao.visible(false);
    _CRUDcomissaoFuncionario.ReAbrirComissao.visible(false);
    _CRUDcomissaoFuncionario.GerarNovaComissao.visible(false);
    _CRUDcomissaoFuncionario.Cancelar.visible(false);
    _CRUDcomissaoFuncionario.MandarGerarNovamente.visible(false);
    _CRUDcomissaoFuncionario.BaixarPDFComissoes.visible(false);
    _CRUDcomissaoFuncionario.BaixarExcelComissoes.visible(false);
    _CRUDcomissaoFuncionario.ExportarComissoes.visible(false);
}

function limparCamposComissaoFuncionario() {
    _CRUDcomissaoFuncionario.Adicionar.visible(true);
    ocultarCamposPadroes();

    _comissaoFuncionario.DataInicio.enable(true);
    _comissaoFuncionario.DataFim.enable(true);
    _comissaoFuncionario.NumeroDiasEmViagem.enable(true);
    _comissaoFuncionario.PercentualComissao.enable(true);
    _comissaoFuncionario.PercentualBaseCalculoComissao.enable(true);
    _comissaoFuncionario.ValorDiaria.enable(true);
    _comissaoFuncionario.Localidade.enable(true);
    _comissaoFuncionario.CargoMotorista.enable(true);
    _comissaoFuncionario.Motorista.enable(true);
    _comissaoFuncionario.MesagemBaseCalculoComissao.enable(true);
    LimparDetalhesComissaoMotorista();


    LimparCampos(_comissaoFuncionario);
    validarPermissoesPersonalizadas();
    resetarTabs();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function VerificarSeComissaoNotificadaEstaSelecionada(codigo) {
    if (codigo == _comissaoFuncionario.Codigo.val()) {
        desativarControleRequisicao();
        _comissaoFuncionario.Codigo.val(codigo);
        PreecherDadosComissao(function () {
            _gridComissaoFuncionario.CarregarGrid(function () {
                ativarControleRequisicao();
            });
        });
    }
}
