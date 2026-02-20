/// <reference path="../../Consultas/Integradora.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumOrigemAuditado.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _controleIntegracao;
var _gridDetalhesAuditoria;
var _pesquisaControleIntegracao;
var _pesquisaDetalhesAuditoria;

/*
 * Declaração das Classes
 */

var ControleIntegracao = function () {
    this.Sistema = new ControleIntegracaoSistema(EnumOrigemAuditado.Sistema);
    this.ListaWebService = ko.observableArray();
}

var ControleIntegracaoSistema = function (origem) {
    this.WebService = PropertyEntity({ id: "tab" + EnumOrigemAuditado.obterNome(origem), idGrid: guid(), idGridRetorno: guid() });
    this._grid;
    this._gridRetorno;
    this._origem = origem;
    this._pesquisaControleIntegracao;

    this._init();
}

ControleIntegracaoSistema.prototype = {
    carregarDados: function () {
        this._grid.CarregarGrid();

        if (this._gridRetorno) {
            this._gridRetorno.CarregarGrid();
            $('#' + this.WebService.idGridRetorno + '-container').show();
        }
    },
    _init: function () {
        this._pesquisaControleIntegracao = this._obterPesquisaControleIntegracao();
        this._grid = this._obterGrid();

        if (this._isExibirGridRetorno())
            this._gridRetorno = this._obterGridRetorno();
    },
    _isExibirGridRetorno: function () {
        return this._origem == EnumOrigemAuditado.WebServiceCargas;
    },
    _obterGrid: function () {
        var opcaoDetalhes = { descricao: Localization.Resources.Integracoes.ControleIntegracao.Detalhes, id: "clasEditar", evento: "onclick", metodo: detalheAuditoriaClick, tamanho: "20", icone: "" };
        var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };
        var configuracoesExportacao = { url: "ControleIntegracao/ExportarPesquisa", titulo: Localization.Resources.Integracoes.ControleIntegracao.AuditoriaZero.format(EnumOrigemAuditado.obterDescricao(this._origem) ) };

        return new GridViewExportacao(this.WebService.idGrid, "ControleIntegracao/Pesquisa", this._pesquisaControleIntegracao, menuOpcoes, configuracoesExportacao);
    },
    _obterGridRetorno: function () {
        var configuracoesExportacao = { url: "ControleIntegracao/ExportarPesquisaRetorno", titulo: Localization.Resources.Integracoes.ControleIntegracao.AuditoriaDeRetornos.format(EnumOrigemAuditado.obterDescricao(this._origem))  };

        return new GridViewExportacao(this.WebService.idGridRetorno, "ControleIntegracao/PesquisaRetorno", this._pesquisaControleIntegracao, null, configuracoesExportacao, undefined, 10);
    },
    _obterPesquisaControleIntegracao: function () {
        if (this._origem == EnumOrigemAuditado.Sistema)
            return new PesquisaControleIntegracaoSistema(this._origem);

        return new PesquisaControleIntegracaoWebService(this._origem);
    }
}

var ControleIntegracaoWebService = function (origem) {
    ControleIntegracaoSistema.call(this, origem);

    this.Disponibilidade = PropertyEntity({ val: ko.observable(Localization.Resources.Integracoes.ControleIntegracao.Disponivel), cssClass: ko.observable("fa fa-check"), visible: ko.observable(false) });
    this.DownloadXml = PropertyEntity({ eventClick: this._downloadXmlTesteDisponibilidade, type: types.event, text: Localization.Resources.Integracoes.ControleIntegracao.DownloadXmlsDeTeste, idGrid: guid() });
    this.ExecutarTeste = PropertyEntity({ eventClick: this._executarTesteDisponibilidade, type: types.event, text: Localization.Resources.Integracoes.ControleIntegracao.ExecutarTeste, idGrid: guid() });

    this._init();
}

ControleIntegracaoWebService.prototype = Object.create(ControleIntegracaoSistema.prototype);
ControleIntegracaoWebService.prototype.constructor = ControleIntegracaoWebService;

ControleIntegracaoWebService.prototype._controlarDisponibilidade = function (disponivel) {
    if (disponivel) {
        this.Disponibilidade.cssClass("fa fa-check");
        this.Disponibilidade.val(Localization.Resources.Integracoes.ControleIntegracao.Disponivel);
        this.Disponibilidade.visible(true);
    }
    else {
        this.Disponibilidade.cssClass("fa fa-close");
        this.Disponibilidade.val(Localization.Resources.Integracoes.ControleIntegracao.Indisponivel);
        this.Disponibilidade.visible(true);
    }
}

ControleIntegracaoWebService.prototype._downloadXmlTesteDisponibilidade = function () {
    executarDownload("ControleIntegracao/DownloadXmlsTesteDisponibilidade", {});
};

ControleIntegracaoWebService.prototype._executarTesteDisponibilidade = function () {
    var self = this;

    executarReST("ControleIntegracao/TestarDisponibilidade", {},
        function (retorno) {
            var disponivel = retorno.Success && retorno.Data;

            self._controlarDisponibilidade(disponivel);
        },
        function () {
            self._controlarDisponibilidade(false);
        }
    );
};


var PesquisaControleIntegracao = function () {
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Integracoes.ControleIntegracao.CodigoDeIntegracao.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50 });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Integracoes.ControleIntegracao.DataInicio.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.DataLimite = PropertyEntity({ text: Localization.Resources.Integracoes.ControleIntegracao.DataLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.Integradora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Integracoes.ControleIntegracao.Integradora.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Integracoes.ControleIntegracao.NumeroDaCarga.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50 });
    this.NumeroCte = PropertyEntity({ text: Localization.Resources.Integracoes.ControleIntegracao.NumeroDoCtE.getFieldDescription(), maxlength: 20, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Integracoes.ControleIntegracao.Usuario.getFieldDescription(), idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: pesquisarClick, type: types.event, text: Localization.Resources.Integracoes.ControleIntegracao.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); }, type: types.event, text: Localization.Resources.Integracoes.ControleIntegracao.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PesquisaControleIntegracaoSistema = function (origem) {
    this.DataInicio = _pesquisaControleIntegracao.DataInicio;
    this.DataLimite = _pesquisaControleIntegracao.DataLimite;
    this.Usuario = _pesquisaControleIntegracao.Usuario;
    this.Origem = PropertyEntity({ val: ko.observable(origem) });
}

var PesquisaControleIntegracaoWebService = function (origem) {
    this.CodigoIntegracao = _pesquisaControleIntegracao.CodigoIntegracao;
    this.DataInicio = _pesquisaControleIntegracao.DataInicio;
    this.DataLimite = _pesquisaControleIntegracao.DataLimite;
    this.Integradora = _pesquisaControleIntegracao.Integradora;
    this.NumeroCarga = _pesquisaControleIntegracao.NumeroCarga;
    this.NumeroCte = _pesquisaControleIntegracao.NumeroCte;
    this.Origem = PropertyEntity({ val: ko.observable(origem) });
}

var PesquisaDetalhesAuditoria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadControleIntegracao() {
    _pesquisaControleIntegracao = new PesquisaControleIntegracao();
    KoBindings(_pesquisaControleIntegracao, "knockoutPesquisaControleIntegracao", false, _pesquisaControleIntegracao.Pesquisar.id);

    _pesquisaDetalhesAuditoria = new PesquisaDetalhesAuditoria();

    new BuscarIntegradora(_pesquisaControleIntegracao.Integradora);
    new BuscarFuncionario(_pesquisaControleIntegracao.Usuario);

    _controleIntegracao = new ControleIntegracao();
    KoBindings(_controleIntegracao, "knockoutControleIntegracao");

    loadControleIntegracaoWebService();
    loadGridDetalhesAuditoria();

    pesquisar();
}

function loadControleIntegracaoWebService() {
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceCargas));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceCTe));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceEmpresa));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceFilial));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceImpressao));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceJanelaCarregamento));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceMDFe));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceNFe));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceNFS));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServiceOcorrencias));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServicePallet));
    _controleIntegracao.ListaWebService.push(new ControleIntegracaoWebService(EnumOrigemAuditado.WebServicePessoas));
}

function loadGridDetalhesAuditoria() {
    _gridDetalhesAuditoria = new GridView("grid-detalhes-auditoria", "ControleIntegracao/PerquisaDetalhes", _pesquisaDetalhesAuditoria);
    _gridDetalhesAuditoria.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function detalheAuditoriaClick(auditoriaSelecionada) {
    _pesquisaDetalhesAuditoria.Codigo.val(auditoriaSelecionada.Codigo);
    _gridDetalhesAuditoria.CarregarGrid();

    Global.abrirModal('divModalDetalhesAuditoria');
}

function pesquisarClick() {
    _pesquisaControleIntegracao.ExibirFiltros.visibleFade(false);

    pesquisar();
}

/*
 * Declaração das Funções
 */

function pesquisar() {
    _controleIntegracao.Sistema.carregarDados();

    var listaWebService = _controleIntegracao.ListaWebService();

    for (var i = 0; i < listaWebService.length; i++) {
        listaWebService[i].carregarDados();
    }
}