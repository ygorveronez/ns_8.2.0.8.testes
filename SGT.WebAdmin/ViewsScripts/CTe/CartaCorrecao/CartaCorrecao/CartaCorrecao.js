var CartaCorrecao = function (cartaCorrecaoCTe) {
    var self = this;

    self.CartaCorrecaoCTe = cartaCorrecaoCTe;

    self.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    self.CTe = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    self.Status = PropertyEntity({ val: ko.observable(EnumStatusCCe.EmDigitacao), def: EnumStatusCCe.EmDigitacao });
    self.DataEmissao = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), text: "Data de Emissão:", enable: ko.observable(true) });

    self.ListaItens = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    self.Itens = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    self._id = guid();
};

var CartaCorrecaoCRUD = function (cartaCorrecao) {
    var self = this;

    self.CartaCorrecao = cartaCorrecao;

    self.Adicionar = PropertyEntity({ eventClick: self._adicionarClick, type: types.event, text: "Adicionar e Emitir", visible: ko.observable(true), enable: ko.observable(true) });
    self.Atualizar = PropertyEntity({ eventClick: self._atualizarClick, type: types.event, text: "Atualizar e Emitir", visible: ko.observable(false), enable: ko.observable(true) });
    self.Cancelar = PropertyEntity({ eventClick: self._cancelarClick, type: types.event, text: "Voltar / Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
};

CartaCorrecaoCRUD.prototype = {
    _setarListasSelecao: function () {
        var self = this;

        self.CartaCorrecao.Itens.val(JSON.stringify(self.CartaCorrecao.ListaItens.val()));
    },
    _adicionarClick: function () {
        var self = this;

        self._setarListasSelecao();

        Salvar(self.CartaCorrecao, "CartaCorrecao/Adicionar", function (r) {
            if (r.Success) {
                if (r.Data) {
                    self.CartaCorrecao._fecharModal();
                    self.CartaCorrecao._limparCampos();

                    if (self.CartaCorrecao.CartaCorrecaoCTe != null)
                        self.CartaCorrecao.CartaCorrecaoCTe._grid.CarregarGrid();

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Atenção", r.Msg);
            }
        });
    },
    _atualizarClick: function () {
        var self = this;

        self._setarListasSelecao();

        Salvar(self.CartaCorrecao, "CartaCorrecao/Atualizar", function (r) {
            if (r.Success) {
                if (r.Data) {
                    self.CartaCorrecao._fecharModal();
                    self.CartaCorrecao._limparCampos();

                    if (self.CartaCorrecao.CartaCorrecaoCTe != null)
                        self.CartaCorrecao.CartaCorrecaoCTe._grid.CarregarGrid();

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Atenção", r.Msg);
            }
        });
    },
    _cancelarClick: function () {
        var self = this;

        self.CartaCorrecao._limparCampos();
        self.CartaCorrecao._fecharModal();
    },
    _bind: function () {
        var self = this;

        KoBindings(self, "knockoutCRUDCartaCorrecao_" + self.CartaCorrecao._id);
    },
    _limparCampos: function () {
        var self = this;

        self.Atualizar.visible(false);
        self.Adicionar.visible(true);
        self.Cancelar.visible(true);
    },
    Load: function () {
        var self = this;

        self._bind();

        return self;
    }
};

CartaCorrecao.prototype = {
    _limparCampos: function () {
        var self = this;

        LimparCampos(self);

        self.DataEmissao.val(Global.DataHoraAtual());
        self.ListaItens.val([]);

        if (self.CRUD != null)
            self.CRUD._limparCampos();

        if (self.ItemCartaCorrecao != null) {
            self.ItemCartaCorrecao._limparCampos();
            self.ItemCartaCorrecao._carregarGrid();

            SetarEnableCamposKnockout(self.ItemCartaCorrecao, true);
        }

        SetarEnableCamposKnockout(self, true);
    },
    _loadHTML: function () {
        var self = this;
        var promiseHTML = new promise.Promise();
        if (string.IsNullOrWhiteSpace(self._html)) {
            $.get("Content/Static/CTe/CartaCorrecao.html?dyn=" + guid(), function (data) {
                self._html = data;
                promiseHTML.done();
            });
        } else {
            promiseHTML.done();
        }
        return promiseHTML;
    },
    _vincularHTML: function () {
        var self = this;

        if (!$("#" + self._id).length) {
            var html = self._html.replace(/#divModalCartaCorrecao/g, self._id);

            $('#js-page-content').append(html);
        }
    },
    _abrirModal: function () {        
        Global.abrirModal(this._id);
    },
    _fecharModal: function () {
        Global.fecharModal(this._id);
    },
    _bind: function () {
        var self = this;

        if (self._binded !== true) {
            self._vincularHTML();

            KoBindings(self, "knockoutCartaCorrecao_" + self._id);

            self.ItemCartaCorrecao = new ItemCartaCorrecao(self).Load();
            self.CRUD = new CartaCorrecaoCRUD(self).Load();

            self._binded = true;
        }
    },
    _carregarDadosGerais: function () {
        var self = this;

        var promiseDadosGerais = new promise.Promise();

        if (self.Codigo.val() > 0) {
            BuscarPorCodigo(self, "CartaCorrecao/BuscarPorCodigo", function (r) {
                if (r.Success) {
                    if (r.Data) {
                        var habilitarCampos = r.Data.Status == EnumStatusCCe.EmDigitacao || r.Data.Status == EnumStatusCCe.Rejeicao;

                        SetarEnableCamposKnockout(self, habilitarCampos);
                        SetarEnableCamposKnockout(self.ItemCartaCorrecao, habilitarCampos);

                        if (habilitarCampos) {
                            self.CRUD.Atualizar.visible(true);
                            self.CRUD.Adicionar.visible(false);
                            self.CRUD.Cancelar.visible(true);
                        } else {
                            self.CRUD.Atualizar.visible(false);
                            self.CRUD.Adicionar.visible(false);
                            self.CRUD.Cancelar.visible(true);
                        }

                        self.ItemCartaCorrecao._carregarGrid();

                        promiseDadosGerais.done();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Atenção", r.Msg);
                }
            });
        } else {
            promiseDadosGerais.done();
        }

        return promiseDadosGerais;
    },
    Load: function (codigoCTe, codigoCartaCorrecao) {
        var self = this;

        if (codigoCartaCorrecao == null)
            codigoCartaCorrecao = 0;

        self._limparCampos();

        self.CTe.val(codigoCTe);
        self.CTe.def = codigoCTe;

        self.Codigo.val(codigoCartaCorrecao);
        self.Codigo.def = codigoCartaCorrecao;

        self._loadHTML().then(function () {
            self._bind();

            self._carregarDadosGerais().then(function () {
                self._abrirModal();
            });
        });

        return self;
    }
};