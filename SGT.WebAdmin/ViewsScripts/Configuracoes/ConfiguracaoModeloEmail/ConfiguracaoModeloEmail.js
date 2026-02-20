/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
///<reference path="../../Enumeradores/EnumTipoModeloEmail.js"/>
///<reference path="../../Enumeradores/EnumEmailEnviarPara.js"/>
///<reference path="../../../ViewsScripts/Enumeradores/EnumAtivoInativo.js"/>
///<reference path="../../../ViewsScripts/Enumeradores/EnumGatilhoNotificacao.js"/>

// #region Objetos Globais do Arquivo

var _configuracaoModeloEmail;
var _gridConfiguracaoModeloEmail;
var _pesquisaConfiguracaoModeloEmail;
var _crudModeloEmail;

// #endregion Objetos Globais do Arquivo

// #region Classes
var PesquisaConfiguracaoModeloEmail = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "" });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoModeloEmail.Todas), options: EnumTipoModeloEmail.obterOpcoesPesquisa(), def: EnumTipoModeloEmail.Todas, text: "Tipo de modelo: " });
    this.Status = PropertyEntity({ text: "Status:", val: ko.observable(EnumAtivoInativo.Todos), def: ko.observable(EnumAtivoInativo.Todos), options: EnumAtivoInativo.obterOpcoesPesquisa() });

    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: loadGridConfiguracaoModeloEmail, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Exibir Filtros", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var ConfiguracaoModeloEmail = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), def: "", required: true });
    this.Status = PropertyEntity({ text: "*Status:", val: ko.observable(EnumAtivoInativo.Ativo), def: ko.observable(EnumAtivoInativo.Ativo), options: EnumAtivoInativo.obterOpcoes(), required: true });
    this.Tipo = PropertyEntity({ text: "*Tipo Modelo:", val: ko.observable(EnumTipoModeloEmail.Padrao), def: ko.observable(EnumTipoModeloEmail.Padrao), options: EnumTipoModeloEmail.obterOpcoes(), required: true });
    this.EnviarPara = PropertyEntity({ text: "*Enviar Para:", val: ko.observable(EnumEmailEnviarPara.Transportador), def: ko.observable(EnumEmailEnviarPara.Transportador), options: EnumEmailEnviarPara.obterOpcoes(), required: true });
    this.GatilhoNotificacao = PropertyEntity({ text: "*Gatilho Notificação:", val: ko.observable(EnumGatilhoNotificacao.AdicionarAgendamento), def: ko.observable(EnumGatilhoNotificacao.AdicionarAgendamento), options: EnumGatilhoNotificacao.obterOpcoes(), required: true, visible: ko.observable(false) });
    this.Assunto = PropertyEntity({ text: "*Assunto:", val: ko.observable(""), def: "", required: true });
    this.Corpo = PropertyEntity({ required: true, val: ko.observable(""), def: "" });
    this.RodaPe = PropertyEntity({ required: false, val: ko.observable("") });

    this.TagNumeroPedido = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagNumeroPedido"); }, type: types.event, text: "Número pedido" });
    this.TagNumeroPedidoCliente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagNumeroPedidoCliente"); }, type: types.event, text: "Número do pedido no Cliente" });
    this.TagNumeroNotaFiscal = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagNumeroNotaFiscal"); }, type: types.event, text: "Número nota fiscal" });
    this.TagCarga = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCarga"); }, type: types.event, text: "Carga" });
    this.TagDataAgendamento = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagDataAgendamento"); }, type: types.event, text: "Data de agendamento" });
    this.TagRazaoSocialRemetente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagRazaoSocialRemetente"); }, type: types.event, text: "Razão social Remetente" });

    this.TagCNPJRemetente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCNPJRemetente"); }, type: types.event, text: "CNPJ Remetente" });
    this.TagEnderecoRemetente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagEnderecoRemetente"); }, type: types.event, text: "Endereço Remetente" });
    this.TagComplementoRemetente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagComplementoRemetente"); }, type: types.event, text: "Complemento Remetente" });
    this.TagBairroRemetente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagBairroRemetente"); }, type: types.event, text: "Bairro Remetente" });
    this.TagCidadeUFRemetente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCidadeUFRemetente"); }, type: types.event, text: "Cidade/UF Remetente" });
    this.TagTelefoneRemetente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagTelefoneRemetente"); }, type: types.event, text: "Telefone Remetente" });
    this.TagRazaoSocialDestinatario = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagRazaoSocialDestinatario"); }, type: types.event, text: "Razão social Destinatário" });
    this.TagCNPJDestinatario = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCNPJDestinatario"); }, type: types.event, text: "CNPJ Destinatário" });
    this.TagEnderecoDestinatario = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagEnderecoDestinatario"); }, type: types.event, text: "Endereço Destinatário" });
    this.TagComplementoDestinatario = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagComplementoDestinatario"); }, type: types.event, text: "Complemento Destinatário" });
    this.TagBairroDestinatario = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagBairroDestinatario"); }, type: types.event, text: "Bairro Destinatário" });
    this.TagCidadeUFDestinatario = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCidadeUFDestinatario"); }, type: types.event, text: "Cidade/UF Destinatário" });
    this.TagTelefoneDestinatario = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagTelefoneDestinatario"); }, type: types.event, text: "Telefone Destinatário" });
    this.TagRazaoSocialTransportador = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagRazaoSocialTransportador"); }, type: types.event, text: "Razão social Transportador" });
    this.TagCNPJTransportador = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCNPJTransportador"); }, type: types.event, text: "CNPJ Transportador" });
    this.TagEnderecoTransportador = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagEnderecoTransportador"); }, type: types.event, text: "Endereço Transportador" });
    this.TagComplementoTransportador = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagComplementoTransportador"); }, type: types.event, text: "Complemento Transportador" });
    this.TagBairroTransportador = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagBairroTransportador"); }, type: types.event, text: "Bairro Transportador" });
    this.TagCidadeUFTransportador = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCidadeUFTransportador"); }, type: types.event, text: "Cidade/UF Transportador" });
    this.TagTelefoneTransportador = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagTelefoneTransportador"); }, type: types.event, text: "Telefone Transportador" });
    this.TagDataSugestaoEntrega = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagDataSugestaoEntrega"); }, type: types.event, text: "Data de sugestão de entrega" });
    this.TagCodigoIntegracaoFilial = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCodigoIntegracaoFilial"); }, type: types.event, text: "Código de integração da Filial " });
    this.TagTipoCarga = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagTipoCarga"); }, type: types.event, text: "Tipo de Carga " });
    this.TagCodigoIntegracaoDestinatarioPedido = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCodigoIntegracaoDestinatarioPedido"); }, type: types.event, text: "Código de integração do destinatário do pedido" });
    this.TagCanalClientes = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCanalClientes"); }, type: types.event, text: "Canal dos clientes" });
    this.TagQtdVolumesCarga = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagQtdVolumesCarga"); }, type: types.event, text: "Quantidade Volumes por carga" });
    this.TagSenhaAgendamento = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagSenhaEntregaAgendamento"); }, type: types.event, text: "Senha Agendamento Entrega" });
    this.TagNFO = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagNFO"); }, type: types.event, text: "Nota Fiscal Origem" });
    this.TagNFD = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagNFD"); }, type: types.event, text: "Nota Fiscal de Devolução" });
    this.TagNumeroDevolucao = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagNumeroDevolucao"); }, type: types.event, text: "Número da Devolução" });
    this.TagRazaoSocialCNPJTransportador = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagRazaoSocialCNPJTransportador"); }, type: types.event, text: "Nome e CNPJ do transportador" });
    this.TagStatus = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagStatus"); }, type: types.event, text: "Status" });
    this.TagDataColeta = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagDataColeta"); }, type: types.event, text: "Data Coleta" });
    this.TagRazaoSocialRecebedor = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagRazaoSocialRecebedor"); }, type: types.event, text: "Razão Social do Recebedor" });
    this.TagCNJPRecebedor = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCNJPRecebedor"); }, type: types.event, text: "CNPJ do Recebedor" });
    this.TagEnderecoRecebedor = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagEnderecoRecebedor"); }, type: types.event, text: "Endereço do Recebedor" });
    this.TagDataHoraAgendamentoColeta = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagDataHoraAgendamentoColeta"); }, type: types.event, text: "Data e Hora Agendamento Coleta" });
    this.TagSenhaAgendamentoColeta = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagSenhaAgendamentoColeta"); }, type: types.event, text: "Senha do Agendamento Coleta" });

    this.CodigoAnexo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAnexoRemovido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo do Email:", val: ko.observable(""), required: ko.observable(false), visible: ko.observable(true) });
    this.DownloadArquivo = PropertyEntity({ eventClick: downloadArquivoClick, type: types.event, text: "Download Anexo", visible: ko.observable(false) });

    this.Tipo.val.subscribe(function (valor) {
        if (valor == EnumTipoModeloEmail.GestaoCustoContabilDevolucao || valor == EnumTipoModeloEmail.ImprocedenciaCenarioPosEntregaDevolucao) {
            $('#tags-tipo-gestao-devolucao').show();
            $('#tags-tipo-agendamento-entrega').hide();
        }
        else {
            $('#tags-tipo-gestao-devolucao').hide();
            $('#tags-tipo-agendamento-entrega').show();
        }

        _configuracaoModeloEmail.GatilhoNotificacao.visible(valor == EnumTipoModeloEmail.AgendamentoColeta);

    });
}

var CRUDModeloEmail = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções públicas
function loadConfiguracaoModeloEmail() {

    _pesquisaConfiguracaoModeloEmail = new PesquisaConfiguracaoModeloEmail();
    KoBindings(_pesquisaConfiguracaoModeloEmail, "knockoutPesquisaConfiguracaoModeloEmail", false, _pesquisaConfiguracaoModeloEmail.Pesquisar.id);

    _configuracaoModeloEmail = new ConfiguracaoModeloEmail();
    KoBindings(_configuracaoModeloEmail, "knockoutConfiguracaoModeloEmail");

    _crudModeloEmail = new CRUDModeloEmail();
    KoBindings(_crudModeloEmail, "knockoutCRUDModeloEmail");

    loadGridConfiguracaoModeloEmail();

    $("#txtEditor").summernote({
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link', 'picture']],
            ['view', ['fullscreen', 'codeview']],
        ]
    });

    $("#txtEditorFooter").summernote({
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'underline', 'clear']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['fontname', ['fontname']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['view', ['fullscreen', 'codeview']],
            ['insert', ['link', 'picture']],
        ]
    });
    //Função para ativar o dropdown das cores.
    $("button[data-toggle='dropdown']").each(function (index) { $(this).removeAttr("data-toggle").attr("data-bs-toggle", "dropdown"); });
}
function loadGridConfiguracaoModeloEmail() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarModeloEmail, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConfiguracaoModeloEmail = new GridView(_pesquisaConfiguracaoModeloEmail.Pesquisar.idGrid, "ConfiguracaoModeloEmail/Pesquisa", _pesquisaConfiguracaoModeloEmail, menuOpcoes, null);
    _gridConfiguracaoModeloEmail.CarregarGrid();
}

// #endregion Funções públicas

// #region Funções privadas
function inserirTagTextoEdicao(tag) {
    $("#txtEditor").summernote('editor.insertText', tag);
}

function adicionarClick() {
    var modeloSalvar = ObterModeloSalvar();

    if (!ValidarCamposObrigatorios(_configuracaoModeloEmail) || string.IsNullOrWhiteSpace(modeloSalvar.Corpo) || _configuracaoModeloEmail.Corpo.val().length <= 11)
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    executarReST("ConfiguracaoModeloEmail/Adicionar", modeloSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (temAnexo()) {
                    EnviarAnexo(retorno.Data.Codigo);
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com Sucesso!");
                    _gridConfiguracaoModeloEmail.CarregarGrid();
                    limparCamposModeloEmail();
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick() {

    var modeloSalvar = ObterModeloSalvar();

    if (!ValidarCamposObrigatorios(_configuracaoModeloEmail) || string.IsNullOrWhiteSpace(modeloSalvar.Corpo) || _configuracaoModeloEmail.Corpo.val().length <= 11)
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    executarReST("ConfiguracaoModeloEmail/Atualizar", modeloSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (temAnexo()) {
                    EnviarAnexo(_configuracaoModeloEmail.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com Sucesso!");
                    _gridConfiguracaoModeloEmail.CarregarGrid();
                    limparCamposModeloEmail();
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function editarModeloEmail(e) {
    limparCamposModeloEmail();
    _configuracaoModeloEmail.Codigo.val(e.Codigo);
    BuscarPorCodigo(_configuracaoModeloEmail, "ConfiguracaoModeloEmail/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoModeloEmail.ExibirFiltros.visibleFade(false);
        _crudModeloEmail.Atualizar.visible(true);
        _crudModeloEmail.Cancelar.visible(true);
        _crudModeloEmail.Adicionar.visible(false);
        _configuracaoModeloEmail.DownloadArquivo.visible(true);

        if (arg.Data.CodigoAnexo == 0)
            _configuracaoModeloEmail.DownloadArquivo.visible(false);

        PreencherObjetoKnout(_configuracaoModeloEmail, arg);

        $("#txtEditor").summernote('code', arg.Data.Corpo);
        $("#txtEditorFooter").summernote('code', arg.Data.RodaPe);
    }, null);
}

function cancelarClick() {
    limparCamposModeloEmail();
}

function limparCamposModeloEmail() {
    _crudModeloEmail.Atualizar.visible(false);
    _crudModeloEmail.Cancelar.visible(false);
    _crudModeloEmail.Adicionar.visible(true);
    _configuracaoModeloEmail.DownloadArquivo.visible(false);
    _configuracaoModeloEmail.Arquivo.val("");

    LimparCampos(_configuracaoModeloEmail);

    $("#txtEditor").summernote('code', '');
    $("#txtEditorFooter").summernote('code', '');
}

function ObterModeloSalvar() {
    _configuracaoModeloEmail.Corpo.val($('#txtEditor').summernote('code'));
    _configuracaoModeloEmail.RodaPe.val($('#txtEditorFooter').summernote('code'));

    var modelo = RetornarObjetoPesquisa(_configuracaoModeloEmail);

    return modelo;
}

function EnviarAnexo(codigoModeloEmail) {
    var file = document.getElementById(_configuracaoModeloEmail.Arquivo.id);
    file = file.files[0];
    var formData = new FormData();
    formData.append("upload", file);
    var data = {
        CodigoModeloEmail: codigoModeloEmail,
        Descricao: _configuracaoModeloEmail.Descricao.val()
    };
    enviarArquivo("ConfiguracaoModeloEmail/EnviarAnexo?callback=?", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo também inserido com sucesso");
                _gridConfiguracaoModeloEmail.CarregarGrid();
                limparCamposModeloEmail();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function temAnexo() {
    return _configuracaoModeloEmail.Arquivo.val() != "";
}

function downloadArquivoClick(e, sender) {
    if (_configuracaoModeloEmail.CodigoAnexo.val() > 0) {
        var dados = { CodigoAnexo: _configuracaoModeloEmail.CodigoAnexo.val() };
        executarDownload("ConfiguracaoModeloEmail/DownloadAnexo", dados);
    }
}
// #endregion Funções privadas