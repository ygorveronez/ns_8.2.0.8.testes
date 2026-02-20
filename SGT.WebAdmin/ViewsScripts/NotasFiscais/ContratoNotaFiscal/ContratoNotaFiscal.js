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
/// <reference path="Configuracao.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContratoNotaFiscal;
var _contratoNotaFiscal;
var _crudContratoNotaFiscal;
var _pesquisaContratoNotaFiscal;

var PesquisaContratoNotaFiscal = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.Contrato = PropertyEntity({ text: "Contrato: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContratoNotaFiscal.CarregarGrid();
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

var ContratoNotaFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", maxlength: 200, required: ko.observable(true) });
    this.Ativo = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true) });

    this.Contrato = PropertyEntity({ required: true, val: ko.observable("") });

    this.TagRazaoSocialCliente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagRazaoSocialCliente"); }, type: types.event, text: "Razão Social Cliente" });
    this.TagCNPJCliente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCNPJCliente"); }, type: types.event, text: "CNPJ Cliente" });
    this.TagEnderecoCliente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagEnderecoCliente"); }, type: types.event, text: "Endereço Cliente" });
    this.TagComplementoCliente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagComplementoCliente"); }, type: types.event, text: "Complemento Cliente" });
    this.TagBairroCliente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagBairroCliente"); }, type: types.event, text: "Bairro Cliente" });
    this.TagCidadeUFCliente = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCidadeUFCliente"); }, type: types.event, text: "Cidade/UF Cliente" });

    this.TagRazaoSocialAdmin = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagRazaoSocialAdmin"); }, type: types.event, text: "Razão Social Admin" });
    this.TagCNPJAdmin = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCNPJAdmin"); }, type: types.event, text: "CNPJ Admin" });
    this.TagEnderecoAdmin = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagEnderecoAdmin"); }, type: types.event, text: "Endereço Admin" });
    this.TagComplementoAdmin = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagComplementoAdmin"); }, type: types.event, text: "Complemento Admin" });
    this.TagBairroAdmin = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagBairroAdmin"); }, type: types.event, text: "Bairro Admin" });
    this.TagCidadeUFAdmin = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagCidadeUFAdmin"); }, type: types.event, text: "Cidade/UF Admin" });

    this.TagResponsavelAdmin = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagResponsavelAdmin"); }, type: types.event, text: "Responsável Admin" });
    this.TagDataAtual = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagDataAtual"); }, type: types.event, text: "Data Atual" });
    this.TagDataCadastro = PropertyEntity({ eventClick: function (e) { inserirTagTextoEdicao("#TagDataCadastro"); }, type: types.event, text: "Data Cadastro" });

    this.CodigoAnexo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAnexoRemovido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo do Contrato:", val: ko.observable(""), required: ko.observable(false), visible: ko.observable(true) });
    this.RemoverArquivo = PropertyEntity({ eventClick: removerArquivoClick, type: types.event, text: "Remover Contrato", visible: ko.observable(false), enable: ko.observable(true) });
    this.DownloadArquivo = PropertyEntity({ eventClick: downloadArquivoClick, type: types.event, text: "Download Contrato", visible: ko.observable(false) });

    this.Configuracoes = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Transportadores = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDContratoNotaFiscal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadContratoNotaFiscal() {

    _pesquisaContratoNotaFiscal = new PesquisaContratoNotaFiscal();
    KoBindings(_pesquisaContratoNotaFiscal, "knockoutPesquisaContratoNotaFiscal", false, _pesquisaContratoNotaFiscal.Pesquisar.id);

    _contratoNotaFiscal = new ContratoNotaFiscal();
    KoBindings(_contratoNotaFiscal, "knockoutCadastroContratoNotaFiscal");

    HeaderAuditoria("EmpresaContrato", _contratoNotaFiscal);

    _crudContratoNotaFiscal = new CRUDContratoNotaFiscal();
    KoBindings(_crudContratoNotaFiscal, "knockoutCRUDContratoNotaFiscal");

    LoadConfiguracaoContratoNotaFiscal();
    LoadTransportadorContratoNotaFiscal();

    buscarContratoNotaFiscals();

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
    var contratoSalvar = ObterContratoSalvar();

    if (!ValidarCamposObrigatorios(_contratoNotaFiscal) || string.IsNullOrWhiteSpace(contratoSalvar.Contrato))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    executarReST("ContratoNotaFiscal/Adicionar", contratoSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (temAnexo()) {
                    EnviarAnexo(retorno.Data.Codigo);
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com Sucesso!");
                    _gridContratoNotaFiscal.CarregarGrid();
                    limparCamposContratoNotaFiscal();
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick(e, sender) {
    var contratoSalvar = ObterContratoSalvar();

    if (!ValidarCamposObrigatorios(_contratoNotaFiscal) || string.IsNullOrWhiteSpace(contratoSalvar.Contrato) || contratoSalvar.Contrato == "<br>")
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Informe os Campos Obrigatórios!");

    executarReST("ContratoNotaFiscal/Atualizar", contratoSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (temAnexo()) {
                    EnviarAnexo(_contratoNotaFiscal.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridContratoNotaFiscal.CarregarGrid();
                    limparCamposContratoNotaFiscal();
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o contrato selecionado?", function () {
        ExcluirPorCodigo(_contratoNotaFiscal, "ContratoNotaFiscal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridContratoNotaFiscal.CarregarGrid();
                    limparCamposContratoNotaFiscal();
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
    limparCamposContratoNotaFiscal();
}

function removerArquivoClick(e, sender) {
    if (_contratoNotaFiscal.CodigoAnexo.val() > 0) {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir o Anexo do Contrato?", function () {
            var codigoAnexo = _contratoNotaFiscal.CodigoAnexo.val();
            limparCamposContratoNotaFiscalAnexo();
            _contratoNotaFiscal.CodigoAnexoRemovido.val(codigoAnexo);
        });
    }
}

function downloadArquivoClick(e, sender) {
    if (_contratoNotaFiscal.CodigoAnexo.val() > 0) {
        var dados = { Codigo: _contratoNotaFiscal.Codigo.val() };
        executarDownload("ContratoNotaFiscal/DownloadAnexo", dados);
    }
}

//*******MÉTODOS*******

function EnviarAnexo(codigoContrato) {
    var file = document.getElementById(_contratoNotaFiscal.Arquivo.id);
    file = file.files[0];
    var formData = new FormData();
    formData.append("upload", file);
    var data = {
        CodigoContrato: codigoContrato
    };
    enviarArquivo("ContratoNotaFiscal/EnviarAnexo?callback=?", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo também inserido com sucesso");
                _gridContratoNotaFiscal.CarregarGrid();
                limparCamposContratoNotaFiscal();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function buscarContratoNotaFiscals() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContratoNotaFiscal, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContratoNotaFiscal = new GridView(_pesquisaContratoNotaFiscal.Pesquisar.idGrid, "ContratoNotaFiscal/Pesquisa", _pesquisaContratoNotaFiscal, menuOpcoes, null);
    _gridContratoNotaFiscal.CarregarGrid();
}

function editarContratoNotaFiscal(contratoNotaFiscalGrid) {
    limparCamposContratoNotaFiscal();
    _contratoNotaFiscal.Codigo.val(contratoNotaFiscalGrid.Codigo);
    BuscarPorCodigo(_contratoNotaFiscal, "ContratoNotaFiscal/BuscarPorCodigo", function (arg) {
        _pesquisaContratoNotaFiscal.ExibirFiltros.visibleFade(false);
        _crudContratoNotaFiscal.Atualizar.visible(true);
        _crudContratoNotaFiscal.Cancelar.visible(true);
        _crudContratoNotaFiscal.Excluir.visible(true);
        _crudContratoNotaFiscal.Adicionar.visible(false);

        if (_contratoNotaFiscal.CodigoAnexo.val() > 0) {
            _contratoNotaFiscal.Arquivo.visible(false);
            _contratoNotaFiscal.RemoverArquivo.visible(true);
            _contratoNotaFiscal.DownloadArquivo.visible(true);
        }

        PreencherObjetoKnout(_configuracaoContratoNotaFiscal, { Data: arg.Data.Configuracoes });
        preencherListaTransportadorContratoNotaFiscal(arg.Data);

        $("#txtEditor").summernote('code', arg.Data.Contrato);
    }, null);
}

function limparCamposContratoNotaFiscal() {
    _crudContratoNotaFiscal.Atualizar.visible(false);
    _crudContratoNotaFiscal.Cancelar.visible(false);
    _crudContratoNotaFiscal.Excluir.visible(false);
    _crudContratoNotaFiscal.Adicionar.visible(true);
    LimparCampos(_contratoNotaFiscal);
    LimparCamposConfiguracaoContratoNotaFiscal();
    LimparCamposTransportadorContratoNotaFiscal();

    limparCamposContratoNotaFiscalAnexo();
    $("#txtEditor").summernote('code', '');

    Global.ResetarAbas();
}

function limparCamposContratoNotaFiscalAnexo() {
    _contratoNotaFiscal.CodigoAnexo.val(0);
    _contratoNotaFiscal.CodigoAnexoRemovido.val(0);
    _contratoNotaFiscal.Arquivo.val("");
    _contratoNotaFiscal.Arquivo.visible(true);
    _contratoNotaFiscal.RemoverArquivo.visible(false);
    _contratoNotaFiscal.DownloadArquivo.visible(false);
}

function temAnexo() {
    return _contratoNotaFiscal.Arquivo.val() != "";
}

function ObterContratoSalvar() {
    _contratoNotaFiscal.Contrato.val($('#txtEditor').summernote('code'));
    _contratoNotaFiscal.Configuracoes.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoContratoNotaFiscal)));

    var contrato = RetornarObjetoPesquisa(_contratoNotaFiscal);

    contrato["Transportadores"] = ObterTransportadorContratoNotaFiscalSalvar();

    return contrato;
}

function inserirTagTextoEdicao(tag) {
    $("#txtEditor").summernote('editor.insertText', tag);
}