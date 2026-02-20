/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroControleExpedicao;
var _containerControleExpedicao;
var _crudCRUDCadastroControleExpedicao;
var _gridControleExpedicaoModeloVeicularCargaExclusivo;
var _importacaoExpedicao;

/*
 * Declaração das Classes
 */

var ContainerControleExpedicao = function () {
    this._listaControleExpedicao = new Array();
    this._listaKnockoutControleExpedicao = new Array();

    this._init();
}

ContainerControleExpedicao.prototype = {
    adicionarControleExpedicao: function (cadastroControleExpedicao) {
        if (!this._validarRegistroDuplicado(cadastroControleExpedicao))
            return false;

        var listaControleExpedicao = this.obterListaControleExpedicao();

        listaControleExpedicao.push(cadastroControleExpedicao);

        this.atualizar(listaControleExpedicao);

        return true;
    },
    atualizar: function (listaControleExpedicao) {
        this._listaControleExpedicao = listaControleExpedicao;

        for (var i = 0; i < this._listaKnockoutControleExpedicao.length; i++) {
            if (this._listaKnockoutControleExpedicao[i] != null)
                this._listaKnockoutControleExpedicao[i].recarregarGrid();
        }
    },
    atualizarControleExpedicao: function (cadastroControleExpedicao) {
        if (!this._validarRegistroDuplicado(cadastroControleExpedicao))
            return false;

        var listaControleExpedicao = this.obterListaControleExpedicao();

        for (var i = 0; i < listaControleExpedicao.length; i++) {
            if (cadastroControleExpedicao.Codigo == listaControleExpedicao[i].Codigo) {
                listaControleExpedicao.splice(i, 1, cadastroControleExpedicao);
                break;
            }
        }

        this.atualizar(listaControleExpedicao);

        return true;
    },
    excluirControleExpedicaoPorCodigo: function (codigo) {
        var listaControleExpedicao = this.obterListaControleExpedicao();

        for (var i = 0; i < listaControleExpedicao.length; i++) {
            if (listaControleExpedicao[i].Codigo == codigo) {
                listaControleExpedicao.splice(i, 1);
                break;
            }
        }

        this.atualizar(listaControleExpedicao);
    },
    importarPorDiaSemana: function (diaSemana, diaSemanaImportar) {
        var listaControleExpedicao = this.obterListaControleExpedicao();
        var listaControleExpedicaoDiaSemanaImportado = new Array();

        for (var i = listaControleExpedicao.length - 1; i >= 0; i--) {
            var controleExpedicao = listaControleExpedicao[i];

            if (controleExpedicao.DiaSemana != diaSemanaImportar)
                listaControleExpedicaoDiaSemanaImportado.push(controleExpedicao);

            if (controleExpedicao.DiaSemana == diaSemana) {
                var controleExpedicaoImportar = $.extend(true, {}, controleExpedicao);

                controleExpedicaoImportar.DiaSemana = diaSemanaImportar;
                controleExpedicaoImportar.Codigo = guid();
                
                listaControleExpedicaoDiaSemanaImportado.push(controleExpedicaoImportar);
            }
        }

        this.atualizar(listaControleExpedicaoDiaSemanaImportado);
    },
    importarParaDemaisDias: function (diaSemana) {
        var listaControleExpedicaoPorDiaSemana = this.obterListaControleExpedicaoPorDiaSemana(diaSemana);
        var listaControleExpedicaoDemaisDias = new Array();

        for (var i = listaControleExpedicaoPorDiaSemana.length - 1; i >= 0; i--) {
            var controleExpedicao = listaControleExpedicaoPorDiaSemana[i];

            for (var j = 0; j < this._listaKnockoutControleExpedicao.length; j++) {
                var knockoutControleExpedicao = this._listaKnockoutControleExpedicao[j];

                if (knockoutControleExpedicao._diaSemana != diaSemana) {
                    var controleExpedicaoImportar = $.extend(true, {}, controleExpedicao);

                    controleExpedicaoImportar.DiaSemana = knockoutControleExpedicao._diaSemana;
                    controleExpedicaoImportar.Codigo = guid();

                    listaControleExpedicaoDemaisDias.push(controleExpedicaoImportar);
                }
            }
        }

        var listaControleExportacao = listaControleExpedicaoPorDiaSemana.concat(listaControleExpedicaoDemaisDias);

        this.atualizar(listaControleExportacao);
    },
    obterControleExpedicaoPorCodigo: function (codigo) {
        var listaControleExpedicao = this.obterListaControleExpedicao();

        for (var i = 0; i < listaControleExpedicao.length; i++) {
            var controleExpedicao = listaControleExpedicao[i];

            if (controleExpedicao.Codigo == codigo)
                return controleExpedicao;
        }

        return undefined;
    },
    obterListaControleExpedicao: function () {
        return this._listaControleExpedicao.slice();
    },
    obterListaControleExpedicaoPorDiaSemana: function (diaSemana) {
        var listaControleExpedicao = this.obterListaControleExpedicao();
        var listaControleExpedicaoPorDiaSemana = new Array();

        for (var i = 0; i < listaControleExpedicao.length; i++) {
            var controleExpedicao = listaControleExpedicao[i];

            if (controleExpedicao.DiaSemana == diaSemana)
                listaControleExpedicaoPorDiaSemana.push(controleExpedicao);
        }

        return listaControleExpedicaoPorDiaSemana;
    },
    obterListaControleExpedicaoSalvar: function () {
        var listaControleExpedicao = this.obterListaControleExpedicao();

        return JSON.stringify(listaControleExpedicao);
    },
    _init: function () {
        this._listaKnockoutControleExpedicao.push(new ControleExpedicao(EnumDiaSemana.Segunda, "knockoutControleExpedicao_Segunda", this));
        this._listaKnockoutControleExpedicao.push(new ControleExpedicao(EnumDiaSemana.Terca, "knockoutControleExpedicao_Terca", this));
        this._listaKnockoutControleExpedicao.push(new ControleExpedicao(EnumDiaSemana.Quarta, "knockoutControleExpedicao_Quarta", this));
        this._listaKnockoutControleExpedicao.push(new ControleExpedicao(EnumDiaSemana.Quinta, "knockoutControleExpedicao_Quinta", this));
        this._listaKnockoutControleExpedicao.push(new ControleExpedicao(EnumDiaSemana.Sexta, "knockoutControleExpedicao_Sexta", this));
        this._listaKnockoutControleExpedicao.push(new ControleExpedicao(EnumDiaSemana.Sabado, "knockoutControleExpedicao_Sabado", this));
        this._listaKnockoutControleExpedicao.push(new ControleExpedicao(EnumDiaSemana.Domingo, "knockoutControleExpedicao_Domingo", this));
    },
    _validarRegistroDuplicado: function (cadastroControleExpedicao) {
        var listaControleExpedicao = this.obterListaControleExpedicaoPorDiaSemana(cadastroControleExpedicao.DiaSemana);

        for (var i = 0; i < listaControleExpedicao.length; i++) {
            var controleExpedicao = listaControleExpedicao[i];

            if (
                (controleExpedicao.Codigo != cadastroControleExpedicao.Codigo) &&
                (controleExpedicao.Produto.Codigo == cadastroControleExpedicao.Produto.Codigo) &&
                (controleExpedicao.ClienteDestino.Codigo == cadastroControleExpedicao.ClienteDestino.Codigo)
            ) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.JaExisteUmControleDeExpedicaoCadastradoComProdutoDestinoInformados);
                return false;
            }
        }

        return true;
    }
};

var ControleExpedicaoDiaSemana = function (controleExpedicao) {
    this.Grid = PropertyEntity({ type: types.local });

    this.Adicionar = PropertyEntity({ eventClick: function () { controleExpedicao.exibirModalCadastro(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.ImportarDeOutroDia = PropertyEntity({ eventClick: function () { controleExpedicao.importarDeOutroDia(); }, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.ImportarDeOutroDia, visible: ko.observable(true) });
    this.ImportarParaOsDemaisDias = PropertyEntity({ eventClick: function () { controleExpedicao.importarParaDemaisDias(); }, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.ImportarParaOsDemaisDias, visible: ko.observable(true) });
}

var ControleExpedicao = function (diaSemana, idKnockout, container) {
    this._container = container;
    this._controleExpedicaoDiaSemana = undefined;
    this._diaSemana = diaSemana;
    this._grid = undefined;
    this._idKnockout = idKnockout;

    this._init();
}

ControleExpedicao.prototype = {
    exibirModalCadastro: function () {
        _cadastroControleExpedicao.ControleExpedicao = this;
        _cadastroControleExpedicao.Codigo.val(guid());
        _cadastroControleExpedicao.DiaSemana.val(this._diaSemana);

        controlarBotoesCadastroControleExpedicaoHabilitados(false);
        exibirModalCadastroControleExpedicao();
    },
    importarDeOutroDia: function () {
        $("#" + _importacaoExpedicao.DiaSemana.id + " option").attr("disabled", false);
        $("#" + _importacaoExpedicao.DiaSemana.id + " option[value='" + this._diaSemana + "']").attr("disabled", true);
        $("#" + _importacaoExpedicao.DiaSemana.id).find('option:enabled:first').prop('selected', true);
        $("#" + _importacaoExpedicao.DiaSemana.id).change();

        _importacaoExpedicao.DiasSemanaImportar = this._diaSemana;

        exibirModalImportacaoControleExpedicao();
    },
    importarParaDemaisDias: function () {
        var self = this;

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroCarregamento.DesejaRealmenteImportarOsDadosDeParaOsDemaisDiasOsDadosExistentesNosDemaisDiasSeraoApagados.format(EnumDiaSemana.obterDescricaoResumida(self._diaSemana)), function () {
            self._container.importarParaDemaisDias(self._diaSemana);

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
        });
    },
    recarregarGrid: function () {
        var listaControleExpedicao = this._container.obterListaControleExpedicaoPorDiaSemana(this._diaSemana);
        var listaControleExpedicaoGrid = new Array();

        for (var i = 0; i < listaControleExpedicao.length; i++) {
            var controleExpedicao = listaControleExpedicao[i];

            listaControleExpedicaoGrid.push({
                Codigo: controleExpedicao.Codigo,
                Produto: controleExpedicao.Produto.Descricao,
                ClienteDestino: controleExpedicao.ClienteDestino.Descricao,
                Quantidade: controleExpedicao.Quantidade
            });
        }

        this._grid.CarregarGrid(listaControleExpedicaoGrid);
    },
    _editar: function (registroSelecionado) {
        var controleExpedicao = this._container.obterControleExpedicaoPorCodigo(registroSelecionado.Codigo);

        if (controleExpedicao) {
            _cadastroControleExpedicao.ControleExpedicao = this;
            _cadastroControleExpedicao.Codigo.val(controleExpedicao.Codigo);
            _cadastroControleExpedicao.DiaSemana.val(controleExpedicao.DiaSemana);
            _cadastroControleExpedicao.ClienteDestino.codEntity(controleExpedicao.ClienteDestino.Codigo);
            _cadastroControleExpedicao.ClienteDestino.entityDescription(controleExpedicao.ClienteDestino.Descricao);
            _cadastroControleExpedicao.ClienteDestino.val(controleExpedicao.ClienteDestino.Descricao);
            _cadastroControleExpedicao.Produto.codEntity(controleExpedicao.Produto.Codigo);
            _cadastroControleExpedicao.Produto.entityDescription(controleExpedicao.Produto.Descricao);
            _cadastroControleExpedicao.Produto.val(controleExpedicao.Produto.Descricao);
            _cadastroControleExpedicao.Quantidade.val(controleExpedicao.Quantidade);

            _gridControleExpedicaoModeloVeicularCargaExclusivo.CarregarGrid(controleExpedicao.ModelosVeicularesCargaExclusivo);

            controlarBotoesCadastroControleExpedicaoHabilitados(true);
            exibirModalCadastroControleExpedicao();
        }
    },
    _init: function () {
        var self = this;

        self._controleExpedicaoDiaSemana = new ControleExpedicaoDiaSemana(self);
        KoBindings(self._controleExpedicaoDiaSemana, self._idKnockout);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: function (registroSelecionado) { self._editar(registroSelecionado); } }] };
        var header = [
            { data: "Codigo", visible: false },
            { data: "Produto", title: Localization.Resources.Logistica.CentroCarregamento.Produto, width: "35%" },
            { data: "Quantidade", title: Localization.Resources.Logistica.CentroCarregamento.Quantidade, width: "20%" },
            { data: "ClienteDestino", title: Localization.Resources.Logistica.CentroCarregamento.Destino, width: "35%" }
        ];

        this._grid = new BasicDataTable(self._controleExpedicaoDiaSemana.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
        this._grid.CarregarGrid(new Array());
    }
};

var CadastroControleExpedicao = function () {
    this.ControleExpedicao = undefined;
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiaSemana = PropertyEntity({ val: ko.observable(EnumDiaSemana.Domingo), def: EnumDiaSemana.Domingo, getType: typesKnockout.int });
    this.ClienteDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Logistica.CentroCarregamento.Destino.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Logistica.CentroCarregamento.Produto.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Quantidade = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.PrevisaoEmLitros.getRequiredFieldDescription(), required: true });

    this.ListaModeloVeicularCargaExclusivo = PropertyEntity({ type: types.local, text: Localization.Resources.Gerais.Geral.Adicionar, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid(), idBtnSearch: guid() });
}

var CRUDCadastroControleExpedicao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarControleExpedicaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarControleExpedicaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirControleExpedicaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var ImportacaoControleExpedicao = function () {
    this.DiaSemana = PropertyEntity({ type: types.map, val: ko.observable(EnumDiaSemana.Segunda), options: ko.observable(EnumDiaSemana.obterOpcoes()), text: Localization.Resources.Logistica.CentroCarregamento.Dia.getRequiredFieldDescription(), def: EnumDiaSemana.Segunda, required: true });
    this.DiasSemanaImportar;

    this.Importar = PropertyEntity({ eventClick: importarExpedicaoDoDiaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Importar, visible: ko.observable(true), enable: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadControleExpedicao() {
    _cadastroControleExpedicao = new CadastroControleExpedicao();
    KoBindings(_cadastroControleExpedicao, "knockoutCadastroControleExpedicao");

    _crudCRUDCadastroControleExpedicao = new CRUDCadastroControleExpedicao();
    KoBindings(_crudCRUDCadastroControleExpedicao, "knockoutCRUDCadastroControleExpedicao");

    _importacaoExpedicao = new ImportacaoControleExpedicao();
    KoBindings(_importacaoExpedicao, "divModalImportacaoControleExpedicao");

    loadGridControleExpedicaoModeloVeicularCargaExclusivo();

    new BuscarClientes(_cadastroControleExpedicao.ClienteDestino);
    new BuscarProdutos(_cadastroControleExpedicao.Produto);

    _containerControleExpedicao = new ContainerControleExpedicao();
}

function loadGridControleExpedicaoModeloVeicularCargaExclusivo() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirControleExpedicaoModeloVeicularCargaExclusivoClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridControleExpedicaoModeloVeicularCargaExclusivo = new BasicDataTable(_cadastroControleExpedicao.ListaModeloVeicularCargaExclusivo.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_cadastroControleExpedicao.ListaModeloVeicularCargaExclusivo, undefined, undefined, undefined, undefined, undefined, _gridControleExpedicaoModeloVeicularCargaExclusivo);
    _cadastroControleExpedicao.ListaModeloVeicularCargaExclusivo.basicTable = _gridControleExpedicaoModeloVeicularCargaExclusivo;

    _gridControleExpedicaoModeloVeicularCargaExclusivo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarControleExpedicaoClick() {
    if (!ValidarCamposObrigatorios(_cadastroControleExpedicao)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var cadastroControleExpedicao = obterControleExpedicaoSalvar();

    if (_containerControleExpedicao.adicionarControleExpedicao(cadastroControleExpedicao))
        fecharModalCadastroControleExpedicao();
}

function atualizarControleExpedicaoClick() {
    if (!ValidarCamposObrigatorios(_cadastroControleExpedicao)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var cadastroControleExpedicao = obterControleExpedicaoSalvar();

    if (_containerControleExpedicao.atualizarControleExpedicao(cadastroControleExpedicao))
        fecharModalCadastroControleExpedicao();
}

function excluirControleExpedicaoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.CentroCarregamento.RealmenteDesejaExcluirControleDeExpedicao, function () {
        _containerControleExpedicao.excluirControleExpedicaoPorCodigo(_cadastroControleExpedicao.Codigo.val());
        fecharModalCadastroControleExpedicao();
    });
}

function excluirControleExpedicaoModeloVeicularCargaExclusivoClick(registroSelecionado) {
    var listaModeloVeicularCargaExclusivo = obterListaModeloVeicularCargaExclusivo();

    for (var i = 0; i < listaModeloVeicularCargaExclusivo.length; i++) {
        if (registroSelecionado.Codigo == listaModeloVeicularCargaExclusivo[i].Codigo) {
            listaModeloVeicularCargaExclusivo.splice(i, 1);
            break;
        }
    }

    _gridControleExpedicaoModeloVeicularCargaExclusivo.CarregarGrid(listaModeloVeicularCargaExclusivo);
}

function importarExpedicaoDoDiaClick() {
    var diaImportarDe = _importacaoExpedicao.DiaSemana.val();

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Logistica.CentroCarregamento.DesejaRealmenteImportarOsDadosDeParaOsDadosExistentesDeSeraoApagados.format(EnumDiaSemana.obterDescricaoResumida(diaImportarDe), EnumDiaSemana.obterDescricaoResumida(_importacaoExpedicao.DiasSemanaImportar), EnumDiaSemana.obterDescricaoResumida(_importacaoExpedicao.DiasSemanaImportar)), function () {
        _containerControleExpedicao.importarPorDiaSemana(diaImportarDe, _importacaoExpedicao.DiasSemanaImportar);

        fecharModalImportacaoControleExpedicao();
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosImportadosComSucesso);
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposControleExpedicao() {
    _containerControleExpedicao.atualizar(new Array());
}

function preencherControleExpedicao(dadosControleExpedicao) {
    _containerControleExpedicao.atualizar(dadosControleExpedicao);
}

function preencherControleExpedicaoSalvar(centroCarregamento) {
    centroCarregamento["ControleExpedicao"] = _containerControleExpedicao.obterListaControleExpedicaoSalvar();
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesCadastroControleExpedicaoHabilitados(isEdicao) {
    _crudCRUDCadastroControleExpedicao.Atualizar.visible(isEdicao);
    _crudCRUDCadastroControleExpedicao.Excluir.visible(isEdicao);
    _crudCRUDCadastroControleExpedicao.Adicionar.visible(!isEdicao);
}

function exibirModalCadastroControleExpedicao() {
    Global.abrirModal('divModalCadastroControleExpedicao');
    $("#divModalCadastroControleExpedicao").one('hidden.bs.modal', function () {
        limparCamposCadastroControleExpedicao();
    });
}

function exibirModalImportacaoControleExpedicao() {
    Global.abrirModal('divModalImportacaoControleExpedicao');
}

function fecharModalCadastroControleExpedicao() {    
    Global.fecharModal('divModalCadastroControleExpedicao');
}

function fecharModalImportacaoControleExpedicao() {
    Global.fecharModal('divModalImportacaoControleExpedicao');
}

function limparCamposCadastroControleExpedicao() {
    LimparCampos(_cadastroControleExpedicao);

    _gridControleExpedicaoModeloVeicularCargaExclusivo.CarregarGrid([]);
}

function obterControleExpedicaoSalvar() {
    return {
        Codigo: _cadastroControleExpedicao.Codigo.val(),
        ClienteDestino: {
            Codigo: _cadastroControleExpedicao.ClienteDestino.codEntity(),
            Descricao: _cadastroControleExpedicao.ClienteDestino.val()
        },
        DiaSemana: _cadastroControleExpedicao.DiaSemana.val(),
        Produto: {
            Codigo: _cadastroControleExpedicao.Produto.codEntity(),
            Descricao: _cadastroControleExpedicao.Produto.val()
        },
        Quantidade: _cadastroControleExpedicao.Quantidade.val(),
        ModelosVeicularesCargaExclusivo: obterListaModeloVeicularCargaExclusivo()
    };
}

function obterListaModeloVeicularCargaExclusivo() {
    return _gridControleExpedicaoModeloVeicularCargaExclusivo.BuscarRegistros();
}
