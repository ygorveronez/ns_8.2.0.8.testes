/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/ExtratoBancarioTipoLancamento.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
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
/// <reference path="../../Enumeradores/EnumTipoDocumentoMovimento.js" />
/// <reference path="../../Enumeradores/EnumTipoGeracaoMovimento.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumDebitoCredito.js" />
/// <reference path="../RateioDespesaVeiculo/RateioDespesaVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridExtratoBancario;
var _extratoBancario;
var _pesquisaExtratoBancario;

var _TipoGeracao = [
    { text: "Manual", value: EnumTipoGeracaoMovimento.Manual },
    { text: "Automática", value: EnumTipoGeracaoMovimento.Automatica }
];

var PesquisaExtratoBancario = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, def: ko.observable("") });
    this.DataMovimento = PropertyEntity({ text: "Data: ", getType: typesKnockout.date });
    this.Valor = PropertyEntity({ text: "Valor: ", getType: typesKnockout.decimal });
    this.Documento = PropertyEntity({ text: "Nº Documento: " });
    this.Observacao = PropertyEntity({ text: "Observação: " });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Conta:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DebitoCredito = PropertyEntity({ val: ko.observable(EnumDebitoCredito.Todos), options: EnumDebitoCredito.ObterOpcoesPesquisa(), text: "Débito/Crédito: ", def: EnumDebitoCredito.Todos, required: false });
    this.ExtratoConsolidado = PropertyEntity({ val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano("Consolidado", "Não Consolidado"), text: "Situação do Movimento:", def: "", required: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridExtratoBancario.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ExtratoBancario = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, def: ko.observable(""), enable: false });
    this.DataMovimento = PropertyEntity({ text: "*Data: ", required: true, getType: typesKnockout.date });
    this.Valor = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal });
    this.DebitoCredito = PropertyEntity({ val: ko.observable(EnumDebitoCredito.Credito), options: EnumDebitoCredito.ObterOpcoes(), text: "*Débito/Crédito: ", def: EnumDebitoCredito.Credito, required: true });
    this.TipoDocumentoMovimento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoMovimento.Manual), options: EnumTipoDocumentoMovimento.obterOpcoes(), text: "*Tipo do Documento: ", def: EnumTipoDocumentoMovimento.Manual, required: true });
    this.Documento = PropertyEntity({ text: "*Nº Documento: ", required: true });

    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Conta:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.ExtratoBancarioTipoLancamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Lançamento:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.CodigoLancamento = PropertyEntity({ text: "Cód. Tipo Lançamento: ", required: false, val: ko.observable("") });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500 });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Limpar Campos", visible: ko.observable(true) });

    //Aba Importação
    this.PlanoContaImportacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Plano de Conta:", idBtnSearch: guid() });
    this.EmpresaImportacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), visible: ko.observable(true) });
    this.Enviar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadExtratoBancario() {
    _extratoBancario = new ExtratoBancario();
    KoBindings(_extratoBancario, "knockoutCadastroExtratoBancario");

    HeaderAuditoria("ExtratoBancario", _extratoBancario);

    _pesquisaExtratoBancario = new PesquisaExtratoBancario();
    KoBindings(_pesquisaExtratoBancario, "knockoutPesquisaExtratoBancario", false, _pesquisaExtratoBancario.Pesquisar.id);

    new BuscarPlanoConta(_pesquisaExtratoBancario.PlanoConta, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarTransportadores(_pesquisaExtratoBancario.Empresa, null, null, true);

    new BuscarPlanoConta(_extratoBancario.PlanoConta, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarTransportadores(_extratoBancario.Empresa, null, null, true);
    new BuscarPlanoConta(_extratoBancario.PlanoContaImportacao, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarTransportadores(_extratoBancario.EmpresaImportacao, null, null, true);
    new BuscarExtratoBancarioTipoLancamento(_extratoBancario.ExtratoBancarioTipoLancamento, RetornoExtratoBancarioTipoLancamento);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _pesquisaExtratoBancario.Empresa.visible(false);
        _extratoBancario.Empresa.visible(false);
        _extratoBancario.Empresa.required(false);
        _extratoBancario.EmpresaImportacao.visible(false);
        _extratoBancario.EmpresaImportacao.required(false);
    }

    buscarExtratoBancarios();
    LoadImportacaoConfirmacao();
}

function RetornoExtratoBancarioTipoLancamento(data) {
    _extratoBancario.ExtratoBancarioTipoLancamento.codEntity(data.Codigo);
    _extratoBancario.ExtratoBancarioTipoLancamento.val(data.Descricao);
    _extratoBancario.CodigoLancamento.val(data.CodigoIntegracao);
}

function adicionarClick(e, sender) {
    Salvar(e, "ExtratoBancario/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridExtratoBancario.CarregarGrid();
                limparCamposExtratoBancario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ExtratoBancario/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridExtratoBancario.CarregarGrid();
                limparCamposExtratoBancario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o extrato selecionado?", function () {
        ExcluirPorCodigo(_extratoBancario, "ExtratoBancario/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridExtratoBancario.CarregarGrid();
                    limparCamposExtratoBancario();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposExtratoBancario();
}

function importarClick(e, sender) {
    var file = document.getElementById(_extratoBancario.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    _extratoBancario.PlanoContaImportacao.requiredClass("form-control");
    _extratoBancario.EmpresaImportacao.requiredClass("form-control");
    _extratoBancario.Arquivo.requiredClass("form-control");

    if (_extratoBancario.PlanoContaImportacao.val() != "" && (_extratoBancario.EmpresaImportacao.val() != "" || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) && _extratoBancario.Arquivo.val() != "") {
        var parametros = {
            CodigoPlanoConta: _extratoBancario.PlanoContaImportacao.codEntity(),
            CodigoEmpresa: _extratoBancario.EmpresaImportacao.codEntity(),
            ApenasLeitura: true
        };
        enviarArquivo("ExtratoBancario/ImportarExtratoBancario?callback=?", parametros, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    LimparCamposImportacaoConfirmacao();                    
                    Global.abrirModal("divModalImportacaoConfirmacao");
                    _importacaoInformacao.SaldoInicial.val(arg.Data.SaldoInicial);
                    _importacaoInformacao.SaldoFinal.val(arg.Data.SaldoFinal);
                    _gridImportacaoConfirmacao.CarregarGrid(arg.Data.Itens);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios para a importação!");
        if (_extratoBancario.PlanoContaImportacao.val() == "")
            _extratoBancario.PlanoContaImportacao.requiredClass("form-control is-invalid");
        if (_extratoBancario.EmpresaImportacao.val() == "" && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
            _extratoBancario.EmpresaImportacao.requiredClass("form-control is-invalid");
        if (_extratoBancario.Arquivo.val() == "")
            _extratoBancario.Arquivo.requiredClass("form-control is-invalid");
    }
}

//*******MÉTODOS*******

function buscarExtratoBancarios() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarExtratoBancario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "ExtratoBancario/ExportarPesquisa",
        titulo: "Movimentação Financeira"
    };

    _gridExtratoBancario = new GridViewExportacao(_pesquisaExtratoBancario.Pesquisar.idGrid, "ExtratoBancario/Pesquisa", _pesquisaExtratoBancario, menuOpcoes, configExportacao, { column: 0, dir: orderDir.desc });
    _gridExtratoBancario.CarregarGrid();
}

function editarExtratoBancario(ExtratoBancarioGrid) {
    limparCamposExtratoBancario();
    _extratoBancario.Codigo.val(ExtratoBancarioGrid.Codigo);
    BuscarPorCodigo(_extratoBancario, "ExtratoBancario/BuscarPorCodigo", function (arg) {
        _pesquisaExtratoBancario.ExibirFiltros.visibleFade(false);
        _extratoBancario.Atualizar.visible(true);
        _extratoBancario.Excluir.visible(true);
        _extratoBancario.Adicionar.visible(false);
    }, null);
}

function limparCamposExtratoBancario() {
    _extratoBancario.Atualizar.visible(false);
    _extratoBancario.Excluir.visible(false);
    _extratoBancario.Adicionar.visible(true);
    LimparCampos(_extratoBancario);
    _extratoBancario.Codigo.val("");
    resetarTabs();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}