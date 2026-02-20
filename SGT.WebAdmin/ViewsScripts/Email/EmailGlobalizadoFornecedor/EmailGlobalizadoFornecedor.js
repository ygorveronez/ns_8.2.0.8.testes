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
/// <reference path="../../Enumeradores/EnumSituacaoEnvioEmail.js" />
/// <reference path="Fornecedor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEmailFornecedor;
var _emailFornecedor;
var _crudEmailFornecedor;
var _pesquisaEmailFornecedor;

var PesquisaEmailFornecedor = function () {
    this.Descricao = PropertyEntity({ text: "Assunto: ", maxlength: 250, enable: ko.observable(true), getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoEnvioEmail.Todas), options: EnumSituacaoEnvioEmail.obterOpcoesPesquisa(), def: EnumSituacaoEnvioEmail.Todas });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridEmailFornecedor.CarregarGrid();
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

var EmailFornecedor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Assunto: ", maxlength: 250, required: ko.observable(true) });
    this.DataEnvio = PropertyEntity({ text: "*Data Envio:", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoEnvioEmail.AguardandoEnvio), options: EnumSituacaoEnvioEmail.obterOpcoes(), def: EnumSituacaoEnvioEmail.AguardandoEnvio, required: ko.observable(true) });
    this.EnviarTodosFornecedores = PropertyEntity({ text: "Enviar para todos os fornecedores", val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.CorpoEmail = PropertyEntity({ required: true, val: ko.observable("") });

    this.EnviarTodosFornecedores.val.subscribe(function (novoValor) {
        _fornecedorEmailFornecedor.Fornecedor.enable(!novoValor);
    });
};

var CRUDEmailFornecedor = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadEmailFornecedor() {

    _pesquisaEmailFornecedor = new PesquisaEmailFornecedor();
    KoBindings(_pesquisaEmailFornecedor, "knockoutPesquisaEmailFornecedor", false);

    _emailFornecedor = new EmailFornecedor();
    KoBindings(_emailFornecedor, "knockoutEmailFornecedor");

    _crudEmailFornecedor = new CRUDEmailFornecedor();
    KoBindings(_crudEmailFornecedor, "knockoutCRUDEmailFornecedor");

    buscarEmailFornecedors();
    LoadFornecedorEmailFornecedor();
    loadAnexoEmailFornecedor();

    $("#txtEditor").summernote({
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link']],
            ['view', ['fullscreen', 'codeview']],
        ]
    });
}

function adicionarClick(e, sender) {
    var emailFornecedorSalvar = ObterEmailFornecedorSalvar();

    if (!ValidarCamposObrigatorios(_emailFornecedor) || string.IsNullOrWhiteSpace(emailFornecedorSalvar.CorpoEmail))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    if (!_emailFornecedor.EnviarTodosFornecedores.val() && !possuiFornecedoresInformados())
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Fornecedores ou a opção Todos");

    executarReST("EmailGlobalizadoFornecedor/Adicionar", emailFornecedorSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                enviarArquivosAnexados(retorno.Data.Codigo);

                _gridEmailFornecedor.CarregarGrid();
                limparCamposEmailFornecedor();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com Sucesso!");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick(e, sender) {
    var emailFornecedorSalvar = ObterEmailFornecedorSalvar();

    if (!ValidarCamposObrigatorios(_emailFornecedor) || string.IsNullOrWhiteSpace(emailFornecedorSalvar.CorpoEmail))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    if (!_emailFornecedor.EnviarTodosFornecedores.val() && !possuiFornecedoresInformados())
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Fornecedores ou a opção Todos");

    executarReST("EmailGlobalizadoFornecedor/Atualizar", emailFornecedorSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gridEmailFornecedor.CarregarGrid();
                limparCamposEmailFornecedor();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Email para Fornecedor selecionado?", function () {
        ExcluirPorCodigo(_emailFornecedor, "EmailGlobalizadoFornecedor/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridEmailFornecedor.CarregarGrid();
                    limparCamposEmailFornecedor();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposEmailFornecedor();
}

//*******MÉTODOS*******

function buscarEmailFornecedors() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarEmailFornecedor, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridEmailFornecedor = new GridView(_pesquisaEmailFornecedor.Pesquisar.idGrid, "EmailGlobalizadoFornecedor/Pesquisa", _pesquisaEmailFornecedor, menuOpcoes, null);
    _gridEmailFornecedor.CarregarGrid();
}

function editarEmailFornecedor(emailFornecedorGrid) {
    limparCamposEmailFornecedor();
    _emailFornecedor.Codigo.val(emailFornecedorGrid.Codigo);
    BuscarPorCodigo(_emailFornecedor, "EmailGlobalizadoFornecedor/BuscarPorCodigo", function (arg) {
        _pesquisaEmailFornecedor.ExibirFiltros.visibleFade(false);
        _crudEmailFornecedor.Atualizar.visible(true);
        _crudEmailFornecedor.Cancelar.visible(true);
        _crudEmailFornecedor.Excluir.visible(true);
        _crudEmailFornecedor.Adicionar.visible(false);

        preencherListaFornecedorEmailFornecedor(arg.Data);

        _anexo.Anexos.val(arg.Data.Anexos);

        $("#txtEditor").summernote('code', arg.Data.CorpoEmail);
    }, null);
}

function limparCamposEmailFornecedor() {
    _crudEmailFornecedor.Atualizar.visible(false);
    _crudEmailFornecedor.Cancelar.visible(false);
    _crudEmailFornecedor.Excluir.visible(false);
    _crudEmailFornecedor.Adicionar.visible(true);
    LimparCampos(_emailFornecedor);
    LimparCamposFornecedorEmailFornecedor();
    limparCamposAnexo();

    $("#txtEditor").summernote('code', '');

    Global.ResetarAbas();
}

function ObterEmailFornecedorSalvar() {
    _emailFornecedor.CorpoEmail.val($('#txtEditor').summernote('code'));

    var emailFornecedor = RetornarObjetoPesquisa(_emailFornecedor);
    
    emailFornecedor["Fornecedores"] = ObterFornecedorEmailFornecedorSalvar();

    return emailFornecedor;
}