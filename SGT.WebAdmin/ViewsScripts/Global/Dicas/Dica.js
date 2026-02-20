/// <reference path="DicaAnexo.js" />

var _pesquisaDica;
var _gridDicas;
var _dica;
var _habilitarEdicao;
var _crudDica;

var PesquisaDica = function () {
    this.Issue = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.NovaDica = PropertyEntity({ type: types.event, eventClick: novaDicaClick, text: "Nova Dica", icone: "fal fa-plus", visible: ko.observable(false) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDicas.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

var Dica = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Issue = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Titulo = PropertyEntity({ text: "Título:", val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.LinkVideo = PropertyEntity({ text: "Link do vídeo:", val: ko.observable(""), def: "", visible: ko.observable(true), enable: ko.observable(false) });

    this.Anexos = PropertyEntity({});
};

var CRUDDica = function () {
    this.Adicionar = PropertyEntity({ type: types.event, eventClick: adicionarDicaClick, text: "Adicionar", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ type: types.event, eventClick: atualizarDicaClick, text: "Atualizar", visible: ko.observable(false) });
    this.Fechar = PropertyEntity({ type: types.event, eventClick: fecharDicaClick, text: "Fechar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ type: types.event, eventClick: excluirDicaClick, text: "Excluir", visible: ko.observable(false) });
};

function loadDicas() {
    BuscarPermissaoUsuario().then(function () { 
        _pesquisaDica = new PesquisaDica();
        KoBindings(_pesquisaDica, "knockoutPesquisaDica");

        _dica = new Dica();
        KoBindings(_dica, "knockoutDica");
        
        loadDicaAnexo();

        _crudDica = new CRUDDica();
        KoBindings(_crudDica, "knockoutCRUDDica");

        if (_habilitarEdicao) {
            _pesquisaDica.NovaDica.visible(true);
            _crudDica.Adicionar.visible(true);
            _crudDica.Excluir.visible(true);
        }

        $("#tituloModalDica").text("Dica");

        $("#descricaoDica").summernote({
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

        $("#descricaoDica").summernote('disable');

        $("#divModalDica").
            on("shown.bs.modal", function () { Global.ResetarAba("modalCadastroDica"); });

        LoadGridDica();
    });
}

function BuscarPermissaoUsuario() {
    let p = new promise.Promise();

    executarReST("Dica/ObterPermissaoUsuario", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                _habilitarEdicao = r.Data.PermiteInserirDicas;

            } else {
                _habilitarEdicao = false;
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        } else {
            _habilitarEdicao = false;
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function LoadGridDica() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarDica, tamanho: "10", icone: "" };
    var visualizar = { descricao: "Visualizar", id: guid(), evento: "onclick", metodo: visualizarDica, tamanho: "10", icone: "" };

    if (_habilitarEdicao) {
        var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, visualizar], tamanho: 10 };
    } else {
        var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [visualizar], tamanho: 10 };
    }

    _gridDicas = new GridView(_pesquisaDica.Pesquisar.idGrid, "Dica/Pesquisa", _pesquisaDica, menuOpcoes);
}

function carregarDicas(issue) {
    _pesquisaDica.Issue.val(issue);
    _gridDicas.CarregarGrid();
}

function limparCamposDica() {
    LimparCampos(_dica);
    limparDicaAnexos();
    $('#descricaoDica').summernote('code', '');
}

function novaDicaClick() {
    manipularModalDica(false);

    Global.abrirModal("divModalDica");
}

function editarDica(data) {
    limparCamposDica();
    _dica.Codigo.val(data.Codigo);
    BuscarPorCodigo(_dica, "Dica/BuscarPorCodigo", function (r) {
        $('#descricaoDica').summernote('code', _dica.Descricao.val());
        manipularModalDica(false);
        preencherDicaAnexos(r.Data.Anexos);
        Global.abrirModal("divModalDica");
    }, null);
    
}

function visualizarDica(data) {
    limparCamposDica();
    _dica.Codigo.val(data.Codigo);
    BuscarPorCodigo(_dica, "Dica/BuscarPorCodigo", function (r) {
        $('#descricaoDica').summernote('code', _dica.Descricao.val());
        $("#iFrameVideoDica").prop("src", _dica.LinkVideo.val());
        preencherDicaAnexos(r.Data.Anexos);
        manipularModalDica(true);
        Global.abrirModal("divModalDica");
    }, null);
}

function fecharDicaClick() {
    limparCamposDica();
    $("#iFrameVideoDica").prop("src", "");
    Global.fecharModal("divModalDica");
}

function adicionarDicaClick(e, sender) {
    _dica.Issue.val(_pesquisaDica.Issue.val());
    _dica.Descricao.val($('#descricaoDica').summernote('code'));

    Salvar(_dica, "Dica/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado com sucesso");
                _gridDicas.CarregarGrid();
                enviarArquivosAnexadosDica(r.Data.Codigo);
                Global.fecharModal("divModalDica");
                limparCamposDica();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, sender);
}

function atualizarDicaClick(e, sender) {
    _dica.Descricao.val($('#descricaoDica').summernote('code'));

    Salvar(_dica, "Dica/Atualizar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridDicas.CarregarGrid();
                Global.fecharModal("divModalDica");
                limparCamposDica();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, sender);
}

function excluirDicaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a dica " + _dica.Titulo.val() + "?", function () {
        ExcluirPorCodigo(_dica, "Dica/ExcluirPorCodigo", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Dica excluída com sucesso");
                    _gridDicas.CarregarGrid();
                    Global.fecharModal("divModalDica");
                    limparCamposDica();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, null);
    });
}

function manipularModalDica(visualizar) {
    if (!visualizar && _habilitarEdicao) {
        if (_dica.Codigo.val() > 0) {
            _crudDica.Adicionar.visible(false);
            _crudDica.Atualizar.visible(true);
        }
        else {
            _crudDica.Adicionar.visible(true);
            _crudDica.Atualizar.visible(false);
        }

        _listaDicaAnexo.Adicionar.visible(true);
        _crudDica.Excluir.visible(true);

        _dica.Titulo.enable(true);
        $("#descricaoDica").summernote('enable');
        _dica.LinkVideo.visible(true);
        _dica.LinkVideo.enable(true);

        $("#liTabDicaVideo").hide();
    } else {
        _crudDica.Adicionar.visible(false);
        _crudDica.Atualizar.visible(false);
        _crudDica.Excluir.visible(false);

        _listaDicaAnexo.Adicionar.visible(false);

        _dica.Titulo.enable(false);
        $("#descricaoDica").summernote('disable');
        _dica.LinkVideo.visible(false);
        _dica.LinkVideo.enable(false);

        $("#liTabDicaVideo").show();
    } 
}