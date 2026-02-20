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
/// <reference path="../../Consultas/TipoDeBidding.js" />
/// <reference path="../../Enumeradores/EnumRequisito.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridBiddingChecklistQuestionarioPadrao;
var _biddingChecklistQuestionarioPadrao;
var _pesquisaBiddingChecklistQuestionarioPadrao;
var _gridQuestionarioAnexo, _anexosQuestionario, _anexoQuestionario;
var listaAnexos = [];

var _requisitoOptions = [
    { text: "Desejável", value: 0 },
    { text: "Indispensável", value: 1 }
];

var PesquisaBiddingChecklistQuestionarioPadrao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.TipoBidding = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Bidding:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBiddingChecklistQuestionarioPadrao.CarregarGrid();
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

var BiddingChecklistQuestionarioPadrao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.TipoBidding = PropertyEntity({ text: "*Tipo de Bidding: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });
    this.Requisito = PropertyEntity({ text: "*Requisito:", required: true, getType: typesKnockout.dynamic, options: _requisitoOptions, val: ko.observable("0"), def: "0" });
};

var CRUDBiddingChecklistQuestionarioPadrao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var AnexosChecklist = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoQuestionario();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoModalQuestionarioClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

var AnexoChecklist = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoQuestionario.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoQuestionarioClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadBiddingChecklistQuestionarioPadrao() {
    _biddingChecklistQuestionarioPadrao = new BiddingChecklistQuestionarioPadrao();
    KoBindings(_biddingChecklistQuestionarioPadrao, "knockoutCadastroBiddingChecklistQuestionarioPadrao");

    HeaderAuditoria("BiddingChecklistQuestionarioPadrao", _biddingChecklistQuestionarioPadrao);

    _crudBiddingChecklistQuestionarioPadrao = new CRUDBiddingChecklistQuestionarioPadrao();
    KoBindings(_crudBiddingChecklistQuestionarioPadrao, "knockoutCRUDBiddingChecklistQuestionarioPadrao");

    _pesquisaBiddingChecklistQuestionarioPadrao = new PesquisaBiddingChecklistQuestionarioPadrao();
    KoBindings(_pesquisaBiddingChecklistQuestionarioPadrao, "knockoutPesquisaBiddingChecklistQuestionarioPadrao", false, _pesquisaBiddingChecklistQuestionarioPadrao.Pesquisar.id);

    _anexosQuestionario = new AnexosChecklist();
    KoBindings(_anexosQuestionario, "knockoutQuestionarioAnexos")

    _anexoQuestionario = new AnexoChecklist();
    KoBindings(_anexoQuestionario, "knockoutQuestionarioAnexo");

    buscarBiddingChecklistQuestionarioPadrao();
    BuscarTipoDeBidding(_biddingChecklistQuestionarioPadrao.TipoBidding);
    BuscarTipoDeBidding(_pesquisaBiddingChecklistQuestionarioPadrao.TipoBidding);
    loadGridChecklistAnexo();
}

function adicionarClick(e, sender) {
    Salvar(_biddingChecklistQuestionarioPadrao, "BiddingChecklistQuestionarioPadrao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                enviarArquivosAnexadosQuestionarioPadrao(retorno.Data.Codigo, _anexosQuestionario.Anexos.val());
                _gridBiddingChecklistQuestionarioPadrao.CarregarGrid();
                limparCamposBiddingChecklistQuestionarioPadrao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    }, sender);
}

function enviarArquivosAnexadosQuestionarioPadrao(codigo, anexos) {
    const formData = obterFormDataQuestionarioAnexos(anexos);

    if (!formData)
        return;

    enviarArquivo("BiddingChecklistAnexoPadrao/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                return exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");

            return exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
        }
        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick(e, sender) {
    Salvar(_biddingChecklistQuestionarioPadrao, "BiddingChecklistQuestionarioPadrao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridBiddingChecklistQuestionarioPadrao.CarregarGrid();
                limparCamposBiddingChecklistQuestionarioPadrao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo de Bidding " + _biddingChecklistQuestionarioPadrao.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_biddingChecklistQuestionarioPadrao, "BiddingChecklistQuestionarioPadrao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridBiddingChecklistQuestionarioPadrao.CarregarGrid();
                    limparCamposBiddingChecklistQuestionarioPadrao();
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
    limparCamposBiddingChecklistQuestionarioPadrao();
}

//*******MÉTODOS*******

function obterFormDataQuestionarioAnexos(anexos) {
    if (anexos.length > 0) {
        let formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function buscarBiddingChecklistQuestionarioPadrao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBiddingChecklistQuestionarioPadrao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBiddingChecklistQuestionarioPadrao = new GridView(_pesquisaBiddingChecklistQuestionarioPadrao.Pesquisar.idGrid, "BiddingChecklistQuestionarioPadrao/Pesquisa", _pesquisaBiddingChecklistQuestionarioPadrao, menuOpcoes, null);
    _gridBiddingChecklistQuestionarioPadrao.CarregarGrid();
}

function editarBiddingChecklistQuestionarioPadrao(biddingChecklistQuestionarioPadraoGrid) {
    limparCamposBiddingChecklistQuestionarioPadrao();
    _biddingChecklistQuestionarioPadrao.Codigo.val(biddingChecklistQuestionarioPadraoGrid.Codigo);
    BuscarPorCodigo(_biddingChecklistQuestionarioPadrao, "BiddingChecklistQuestionarioPadrao/BuscarPorCodigo", function (retorno) {
        _pesquisaBiddingChecklistQuestionarioPadrao.ExibirFiltros.visibleFade(false);
        _biddingChecklistQuestionarioPadrao.TipoBidding.codEntity(retorno.Data.TipoBidding.Codigo);
        _biddingChecklistQuestionarioPadrao.TipoBidding.val(retorno.Data.TipoBidding.Descricao);
        _anexosQuestionario.Anexos.val(retorno.Data.Anexos.slice());
        _crudBiddingChecklistQuestionarioPadrao.Atualizar.visible(true);
        _crudBiddingChecklistQuestionarioPadrao.Cancelar.visible(true);
        _crudBiddingChecklistQuestionarioPadrao.Excluir.visible(true);
        _crudBiddingChecklistQuestionarioPadrao.Adicionar.visible(false);
    }, null);
}

function limparCamposBiddingChecklistQuestionarioPadrao() {
    _crudBiddingChecklistQuestionarioPadrao.Atualizar.visible(false);
    _crudBiddingChecklistQuestionarioPadrao.Cancelar.visible(false);
    _crudBiddingChecklistQuestionarioPadrao.Excluir.visible(false);
    _crudBiddingChecklistQuestionarioPadrao.Adicionar.visible(true);
    _anexosQuestionario.Anexos.val([]);
    LimparCampos(_biddingChecklistQuestionarioPadrao);
}

function loadGridChecklistAnexo() {
    const linhasPorPaginas = 2;
    const opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoQuestionarioClick, icone: "", visibilidade: true };
    const opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoQuestionarioClick, icone: "", visibilidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridQuestionarioAnexo = new BasicDataTable(_anexosQuestionario.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridQuestionarioAnexo.CarregarGrid([]);
}

function adicionarAnexoModalQuestionarioClick() {
    Global.abrirModal('divModalQuestionarioAnexo');
    $("#divModalQuestionarioAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_anexoQuestionario);
    });
}

function adicionarAnexoQuestionarioClick() {
    var arquivo = document.getElementById(_anexoQuestionario.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoQuestionario)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoQuestionario.Descricao.val(),
        NomeArquivo: _anexoQuestionario.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    listaAnexos.push(anexo);

    _anexosQuestionario.Anexos.val(listaAnexos.slice());

    _anexoQuestionario.Arquivo.val("");

    Global.fecharModal("divModalQuestionarioAnexo");
}

function removerAnexoQuestionarioClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoQuestionarioLocal(registroSelecionado);
    else {
        executarReST("BiddingChecklistAnexoPadrao/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerAnexoQuestionarioLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function downloadAnexoQuestionarioClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    if (isNaN(registroSelecionado.Codigo)) {
        exibirMensagem("atencao", "Não é possível fazer o download do anexo pois o mesmo ainda não foi enviado.");
        return;
    }
    executarDownload("BiddingChecklistAnexoPadrao/DownloadAnexo", dados);
}

function obterAnexosQuestionario() {
    return _anexosQuestionario.Anexos.val().slice();
}

function recarregarGridAnexoQuestionario() {
    const anexos = obterAnexosQuestionario();

    _gridQuestionarioAnexo.CarregarGrid(anexos);
}

function removerAnexoQuestionarioLocal(registroSelecionado) {
    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });
    _anexosQuestionario.Anexos.val(listaAnexos.slice());
    recarregarGridAnexoQuestionario();
}