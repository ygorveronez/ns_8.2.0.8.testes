/// <reference path="../../Consultas/TipoAnexo.js" />

// #region Objetos Globais do Arquivo

var _cadastroCargaVeiculoContainerAnexo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CadastroCargaVeiculoContainerAnexo = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.TipoAnexo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Anexo:", idBtnSearch: guid(), required: true });

    this.Arquivo.val.subscribe(function (novoValor) { self.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });

    this.Adicionar = PropertyEntity({ eventClick: this._adicionarAnexo, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    this._cargaVeiculoContainerAnexo;
    this._codigoCargaVeiculoContainer;
    this._permitirGerenciarCargaVeiculoContainerAnexo;
    this._gridAnexo;

    this._init();
}

CadastroCargaVeiculoContainerAnexo.prototype = {
    exibirModal: function (cargaVeiculoContainerAnexo, codigoCargaVeiculoContainer, permitirGerenciarCargaVeiculoContainerAnexo) {
        var self = this;
        self._cargaVeiculoContainerAnexo = cargaVeiculoContainerAnexo;
        self._codigoCargaVeiculoContainer = codigoCargaVeiculoContainer;
        self._permitirGerenciarCargaVeiculoContainerAnexo = permitirGerenciarCargaVeiculoContainerAnexo;
        self._gridAnexo.CarregarGrid(self._cargaVeiculoContainerAnexo.obterListaAnexo());

        Global.abrirModal('divModalCargaVeiculoContainerAnexo');
        $("#divModalCargaVeiculoContainerAnexo").one('hidden.bs.modal', function () {
            LimparCampos(self);

            self._cargaVeiculoContainerAnexo = undefined;
            self._codigoCargaVeiculoContainer = 0;
            self._gridAnexo.CarregarGrid([]);
        });
    },
    _adicionarAnexo: function () {
        var self = this;
        if (!self._isPermitirGerenciarCargaVeiculoContainerAnexo())
            return exibirMensagem(tipoMensagem.atencao, "Anexos", "Status não permite adicionar anexo");

        var arquivo = document.getElementById(self.Arquivo.id);

        if (arquivo.files.length == 0)
            return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

        var anexo = {
            Codigo: guid(),
            Descricao: self.Descricao.val(),
            NomeArquivo: self.NomeArquivo.val(),
            Arquivo: arquivo.files[0],
            TipoAnexo: self.TipoAnexo.codEntity()
        };

        self._cargaVeiculoContainerAnexo.adicionarAnexo(self._codigoCargaVeiculoContainer, anexo, function () {
            _cadastroCargaVeiculoContainerAnexo.Arquivo.val("");
            LimparCampos(self);

            self._gridAnexo.CarregarGrid(self._cargaVeiculoContainerAnexo.obterListaAnexo());

            arquivo.value = null;
        });
    },
    _downloadCargaVeiculoContainerAnexo: function (registroSelecionado) {
        executarDownload("CargaVeiculoContainerAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
    },
    _init: function () {
        KoBindings(this, "knockoutCadastroCargaVeiculoContainerAnexo");

        this._loadGridAnexo();
        new BuscarTipoAnexo(this.TipoAnexo);
    },
    _isExibirOpcaoDownloadCargaVeiculoContainerAnexo: function (registroSelecionado) {
        return !isNaN(registroSelecionado.Codigo);
    },
    _isPermitirGerenciarCargaVeiculoContainerAnexo: function () {
        return this._permitirGerenciarCargaVeiculoContainerAnexo;
    },
    _loadGridAnexo: function () {
        var self = this;
        var linhasPorPaginas = 7;
        var opcaoDownload = { descricao: "Download", id: guid(), metodo: self._downloadCargaVeiculoContainerAnexo, icone: "", visibilidade: self._isExibirOpcaoDownloadCargaVeiculoContainerAnexo };
        var opcaoRemover = { descricao: "Remover", id: guid(), metodo: function (registroSelecionado) { self._removerCargaVeiculoContainerAnexo(registroSelecionado); }, icone: "", visibilidade: function () { return self._isPermitirGerenciarCargaVeiculoContainerAnexo(); } };
        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "TipoAnexo", visible: false },
            { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
            { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
        ];

        self._gridAnexo = new BasicDataTable(self.Codigo.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
        self._gridAnexo.CarregarGrid([]);
    },
    _removerCargaVeiculoContainerAnexo: function (registroSelecionado) {
        var self = this;

        self._cargaVeiculoContainerAnexo.removerAnexo(registroSelecionado, self._isPermitirGerenciarCargaVeiculoContainerAnexo(), function () {
            self._gridAnexo.CarregarGrid(self._cargaVeiculoContainerAnexo.obterListaAnexo());
        });
    }
}

var CargaVeiculoContainerAnexo = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), visible: ko.observable(true) });
}

CargaVeiculoContainerAnexo.prototype = {
    adicionarAnexo: function (codigoCargaVeiculoContainer, anexo, callbackAnexoAdicionado) {
        if (codigoCargaVeiculoContainer > 0)
            this._enviarListaAnexo(codigoCargaVeiculoContainer, [anexo], callbackAnexoAdicionado);
        else {
            var anexos = this.obterListaAnexo();

            anexos.push(anexo);

            this.Anexos.val(anexos.slice());

            callbackAnexoAdicionado();
        }
    },
    enviarArquivosAnexados: function (codigo) {
        var anexos = this.obterListaAnexo();

        this._enviarListaAnexo(codigo, anexos);
    },
    limparAnexos: function () {
        this.Anexos.val(this.Anexos.def);
    },
    obterListaAnexo: function () {
        return this.Anexos.val().slice();
    },
    preencherAnexos: function (anexos) {
        this.Anexos.val(anexos);
    },
    removerAnexo: function (registroSelecionado, permitirGerenciarAnexo, callbackAnexoRemovido) {
        
        var self = this;

        exibirConfirmacao("Confirmação", "Realmente deseja excluir este anexo?", function () {
            if (isNaN(registroSelecionado.Codigo))
                self._removerAnexoLocal(registroSelecionado, callbackAnexoRemovido);
            else if (permitirGerenciarAnexo) {
                executarReST("CargaVeiculoContainerAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
                    if (retorno.Success) {
                        if (retorno.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                            self._removerAnexoLocal(registroSelecionado, callbackAnexoRemovido);
                        }
                        else
                            exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                });
            }
            else
                return exibirMensagem(tipoMensagem.atencao, "Anexos", "Status não permite remover anexo");
        });       
    },
    _enviarListaAnexo: function (codigo, anexos, callbackAnexoAdicionado) {
        var self = this;
        var formData = self._obterFormDataAnexo(anexos);

        if (formData) {
            enviarArquivo("CargaVeiculoContainerAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");

                        self.Anexos.val(retorno.Data.Anexos);

                        if (callbackAnexoAdicionado instanceof Function)
                            callbackAnexoAdicionado();
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Não foi possível anexar o arquivo.", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        }
    },
    _obterFormDataAnexo: function (anexos) {
        if (anexos.length > 0) {
            var formData = new FormData();

            anexos.forEach(function (anexo) {
                formData.append("Arquivo", anexo.Arquivo);
                formData.append("Descricao", anexo.Descricao);
                formData.append("TipoAnexo", anexo.TipoAnexo);
            });

            return formData;
        }

        return undefined;
    },
    _removerAnexoLocal: function (registroSelecionado, callbackAnexoRemovido) {
        var listaAnexos = this.obterListaAnexo();

        listaAnexos.forEach(function (anexo, i) {
            if (registroSelecionado.Codigo == anexo.Codigo) {
                listaAnexos.splice(i, 1);
            }
        });

        this.Anexos.val(listaAnexos);

        callbackAnexoRemovido();
    }
}

// #endregion Classes

// #region Funções de Inicialização

function loadCargaVeiculoContainerAnexo() {
    _cadastroCargaVeiculoContainerAnexo = new CadastroCargaVeiculoContainerAnexo();
}

// #endregion Funções de Inicialização

// #region Funções de Públicas

function exibirCargaVeiculoContainerAnexo(cargaVeiculoContainerAnexo, codigoCargaVeiculoContainer) {
    _cadastroCargaVeiculoContainerAnexo.exibirModal(cargaVeiculoContainerAnexo, codigoCargaVeiculoContainer, true);
}

// #endregion Funções de Públicas
