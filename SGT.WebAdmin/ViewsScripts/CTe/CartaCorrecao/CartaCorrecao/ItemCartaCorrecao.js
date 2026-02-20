
var ItemCartaCorrecao = function (cartaCorrecao) {
    var self = this;

    self.CartaCorrecao = cartaCorrecao;
    self.Grid = PropertyEntity({ type: types.local });

    self.Codigo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    self.CampoAlterado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Campo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    self.DescricaoCampoAlterado = PropertyEntity({ getType: typesKnockout.string, text: "Descrição Campo Alterado:", visible: ko.observable(true) });
    self.NumeroItemAlterado = PropertyEntity({ getType: typesKnockout.int, text: "Nº Item:", visible: ko.observable(false), configInt: { thousands: '', precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), enable: ko.observable(true) });
    self.ValorAlterado = PropertyEntity({ getType: typesKnockout.string, text: "Valor:", maxlength: ko.observable(1), visible: ko.observable(false), required: true, enable: ko.observable(true) });
    self.IndicadorRepeticao = PropertyEntity({ getType: typesKnockout.bool, text: "Indicador de repetição:", val: ko.observable(false), def: false });
    self.TipoCampo = PropertyEntity({ text: "Tipo do Campo:", val: ko.observable(EnumTipoCampoCCe.Texto), def: EnumTipoCampoCCe.Texto });
    self.QuantidadeCaracteres = PropertyEntity({ getType: typesKnockout.int, text: "Quantidade de Caracteres:", val: ko.observable(0), def: 0 });
    self.QuantidadeDecimais = PropertyEntity({ getType: typesKnockout.int, text: "Quantidade de Decimais:", val: ko.observable(0), def: 0 });
    self.QuantidadeInteiros = PropertyEntity({ getType: typesKnockout.int, text: "Quantidade de Inteiros:", val: ko.observable(0), def: 0 });

    self.Adicionar = PropertyEntity({ eventClick: self._adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    self.Atualizar = PropertyEntity({ eventClick: self._atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    self.Excluir = PropertyEntity({ eventClick: self._excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
    self.Cancelar = PropertyEntity({ eventClick: self._cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
};

ItemCartaCorrecao.prototype = {
    _adicionarClick: function () {
        var self = this;

        if (!ValidarCamposObrigatorios(self)) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
            return;
        }

        self.Codigo.val(guid());

        self.CartaCorrecao.ListaItens.val().push(RetornarObjetoPesquisa(self));

        self._carregarGrid();
        self._limparCampos();
    },
    _atualizarClick: function () {
        var self = this;

        if (!ValidarCamposObrigatorios(self)) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
            return;
        }

        var listaItens = self.CartaCorrecao.ListaItens.val();

        for (var i = 0; i < listaItens.length; i++) {
            if (self.Codigo.val() == listaItens[i].Codigo) {
                listaItens[i] = RetornarObjetoPesquisa(self);
                break;
            }
        }

        self.CartaCorrecao.ListaItens.val(listaItens);

        self._carregarGrid();
        self._limparCampos();
    },
    _excluirClick: function () {
        var self = this;
        var listaItens = self.CartaCorrecao.ListaItens.val();

        for (var i = 0; i < listaItens.length; i++) {
            if (self.Codigo.val() == listaItens[i].Codigo) {
                listaItens.splice(i, 1);
                break;
            }
        }

        self.CartaCorrecao.ListaItens.val(listaItens);

        self._carregarGrid();
        self._limparCampos();
    },
    _cancelarClick: function () {
        var self = this;

        self._limparCampos();
    },
    _editarClick: function (data) {
        var self = this;
        var listaItens = self.CartaCorrecao.ListaItens.val();

        self._limparCampos();

        for (var i = 0; i < listaItens.length; i++) {
            if (data.Codigo == listaItens[i].Codigo) {
                var item = listaItens[i];
                PreencherObjetoKnout(self, { Data: item });

                self.CampoAlterado.codEntity(item.CampoAlterado);
                self.CampoAlterado.val(item.DescricaoCampoAlterado);

                self.Adicionar.visible(false);
                self.Atualizar.visible(true);
                self.Excluir.visible(true);
                self.Cancelar.visible(true);

                self._campoAlteradoChange();

                return;
            }
        }
    },
    _limparCampos: function () {
        var self = this;

        self.Adicionar.visible(true);
        self.Atualizar.visible(false);
        self.Excluir.visible(false);
        self.Cancelar.visible(false);

        LimparCampos(self);

        self._campoAlteradoChange();
        self.ValorAlterado.visible(false);
    },
    _carregarGrid: function () {
        var self = this;
        var data = new Array();

        $.each(self.CartaCorrecao.ListaItens.val(), function (i, item) {
            var itemGrid = new Object();

            itemGrid.Codigo = item.Codigo;
            itemGrid.Campo = item.DescricaoCampoAlterado;
            itemGrid.Valor = item.ValorAlterado;
            itemGrid.NumeroItem = item.NumeroItemAlterado;

            data.push(itemGrid);
        });

        self._grid.CarregarGrid(data);
    },
    _retornoSelecaoCampoAlterado: function (data) {
        var self = this;
        executarReST("CampoCartaCorrecao/BuscarPorCodigo", { Codigo: data.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    self.CampoAlterado.val(r.Data.Descricao);
                    self.CampoAlterado.codEntity(r.Data.Codigo);
                    self.DescricaoCampoAlterado.val(r.Data.Descricao);
                    self.IndicadorRepeticao.val(r.Data.IndicadorRepeticao);
                    self.QuantidadeCaracteres.val(r.Data.QuantidadeCaracteres);
                    self.QuantidadeDecimais.val(r.Data.QuantidadeDecimais);
                    self.QuantidadeInteiros.val(r.Data.QuantidadeInteiros);
                    self.TipoCampo.val(r.Data.TipoCampo);

                    self._campoAlteradoChange();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Atenção", r.Msg);
            }
        });
    },
    _resetarCampoAlterado: function () {
        var self = this;

        self.ValorAlterado.get$().maskMoney('destroy');

        if (self.ValorAlterado.get$().data("DateTimePicker"))
            self.ValorAlterado.get$().data("DateTimePicker").destroy();

        self.ValorAlterado.visible(true);
        self.ValorAlterado.required = false;
        self.NumeroItemAlterado.required = false;
    },
    _campoAlteradoChange: function () {
        var self = this;

        self._resetarCampoAlterado();

        switch (self.TipoCampo.val()) {
            case EnumTipoCampoCCe.Texto:
                self.ValorAlterado.maxlength(self.QuantidadeCaracteres.val());
                break;
            case EnumTipoCampoCCe.Inteiro:
                self.ValorAlterado.maxlength(self.QuantidadeCaracteres.val());
                self.ValorAlterado.get$().maskMoney(ConfigInt({ thousands: '', allowZero: true, selectAllOnFocus: true }));
                self.ValorAlterado.required = true;
                break;
            case EnumTipoCampoCCe.Decimal:
                self.ValorAlterado.maxlength(self.QuantidadeInteiros.val() + self.QuantidadeDecimais.val() + 1);
                self.ValorAlterado.get$().maskMoney(ConfigDecimal({ thousands: '', precision: self.QuantidadeDecimais.val(), allowZero: true, selectAllOnFocus: true }));
                self.ValorAlterado.required = true;
                break;
            case EnumTipoCampoCCe.Data:
                self.ValorAlterado.maxlength(10);
                self.ValorAlterado.get$().datetimepicker({ locale: 'pt-br', useCurrent: false, format: 'DD/MM/YYYY' });
                self.ValorAlterado.get$().mask("00/00/0000", { selectOnFocus: true, clearIfNotMatch: true });
                self.ValorAlterado.required = true;
                break;
            default:
                self.ValorAlterado.visible(false);
                self.ValorAlterado.required = false;
                break;
        }

        self.NumeroItemAlterado.visible(self.IndicadorRepeticao.val());
        self.NumeroItemAlterado.required = self.IndicadorRepeticao.val();
    },
    _loadGrid: function () {
        var self = this;

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: function (data) { self._editarClick(data); } }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Campo", title: "Campo", width: "30%" },
            { data: "Valor", title: "Valor", width: "40%" },
            { data: "NumeroItem", title: "Nº Item", width: "20%" }
        ];

        self._grid = new BasicDataTable(self.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

        self._carregarGrid();
    },
    _bind: function () {
        var self = this;

        KoBindings(self, "knockoutItemCartaCorrecao_" + self.CartaCorrecao._id);

        new BuscarCamposCartaCorrecao(self.CampoAlterado, function (data) { self._retornoSelecaoCampoAlterado(data); });
    },
    Load: function () {
        var self = this;

        self._bind();
        self._loadGrid();

        return self;
    }
};