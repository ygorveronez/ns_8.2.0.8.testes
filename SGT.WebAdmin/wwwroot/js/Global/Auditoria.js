
/*****MAPEAMENTO******/
var Auditoria = function () {
    this.Auditar = PropertyEntity({ eventClick: null, visible: ko.observable(false) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.knout = PropertyEntity({});
    this.Entidade = PropertyEntity({});
    this.DownloadExcell = PropertyEntity({ val: ko.observable(false) });
    this.Auditoria = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Auditoria });
    

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            __GridAuditoria.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
}

var DetalheAuditoria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Dicionario = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Entidade = PropertyEntity({});
    this.DetalhesAuditoria = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DetalhesAuditoria });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            __GridDetalheAuditoria.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
}

var __Auditoria;
var __DetalheAuditoria;

var __GridAuditoria;
var __GridDetalheAuditoria;



/*****LOADER******/
function LoadAuditoria() {
    if (!__Auditoria)
        __LoadAuditoria();
}

function __IniciaGridsAuditoria() {
    var editar = {
        descricao: Localization.Resources.Gerais.Geral.Detalhes,
        id: guid(),
        evento: "onclick",
        metodo: function (data) {
            __DetalheAuditoria.Codigo.val(data.Codigo);
            __AbrirModalDtalhesAuditoria();
        },
        tamanho: "10"
    };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var ko_Auditoria = {
        Entidade: __Auditoria.Entidade,
        Codigo: __Auditoria.Codigo
    };

    var ko_DetalheAuditoria = {
        Codigo: __DetalheAuditoria.Codigo,
        Dicionario: __DetalheAuditoria.Dicionario,
        Entidade: __Auditoria.Entidade
    };

    var downloadAuditoria = {
        url: "Auditoria/Exportar",
        titulo: Localization.Resources.Gerais.Geral.Auditoria
    };
    var downloadDetalhesAuditoria = {
        url: "Auditoria/ExportarDetalhes",
        titulo: Localization.Resources.Gerais.Geral.DetalhesAuditoria
    };

    if (!__Auditoria.DownloadExcell.val()) {
        downloadAuditoria = null;
        downloadDetalhesAuditoria = null;
    }

    __GridAuditoria = new GridViewExportacao("tableModalAuditoria", "Auditoria/Pesquisa", ko_Auditoria, menuOpcoes, downloadAuditoria, null, 10);
    __GridDetalheAuditoria = new GridViewExportacao("tableModalDetalheAuditoria", "Auditoria/PesquisaComposAlterados", ko_DetalheAuditoria, null, downloadDetalhesAuditoria, null, 10);
}


/*****MODAL******/
function __AbrirModalAuditoria(e, callback) {
    if (e && e.preventDefault)
        e.preventDefault();

    __GridAuditoria.CarregarGrid();

    Global.abrirModal("divModalAuditoria");

    if (callback) {
        $("#divModalAuditoria").one("hidden.bs.modal", function (e) {
            callback(e);
        });
    }
}

function __AbrirModalDtalhesAuditoria() {
    __GridDetalheAuditoria.CarregarGrid();
    Global.abrirModal("divModalDetalhesAuditoria");
}


/*****FUNCOES******/
function __ClearAuditoria() {
    __Auditoria.Entidade.val('');
    __Auditoria.knout.val({});
    __Auditoria.Codigo.val(0);

    __DetalheAuditoria.Codigo.val(0);
    __DetalheAuditoria.Dicionario.val("");

    
    $("#divAuditoriaGlobal").hide();
    Global.fecharModal("divModalDetalhesAuditoria");
    Global.fecharModal("divModalAuditoria");
    ocultarTodasAsNotificacoes();
}

function __GerarDicionario(knout, dicionario, asString) {
    var _map = {};

    // Busca primeiro os text do mapeamento knout
    if (knout) {
        for (var k in knout) {
            if (!knout[k].text)
                continue;

            var textoBruto = $.isFunction(knout[k].text) ? knout[k].text() : knout[k].text;
            var textoLimpo = textoBruto;//textoBruto.replace("*", "").replace(":", "");

            if (textoLimpo)
                _map[k] = textoLimpo.trim();
        }
    }

    // Se vier um dicionario, sobrepoe 
    if (!$.isArray(dicionario)) {
        for (var i in dicionario) {
            _map[i] = dicionario[i].trim();

        }
    }

    // Transforma o obj em mapeamento pro servidor (um array de objetos)
    //var _mapeamendo = [];
    //for (var k in _map)
    //    _mapeamendo.push({
    //        Nome: k,
    //        Descricao: _map[k]
    //    });

    // Retorna como string
    if (asString)
        return JSON.stringify(_map);
    else
        return _map;
}

function __LoadAuditoria() {
    __Auditoria = new Auditoria();
    KoBindings(__Auditoria, "knoutModalAuditoria");

    __DetalheAuditoria = new DetalheAuditoria();
    KoBindings(__DetalheAuditoria, "knoutModalDetalhesAuditoria");

    __IniciaGridsAuditoria();
    __Auditoria.DownloadExcell.val.subscribe(__IniciaGridsAuditoria);

    LocalizeCurrentPage();

    $("#divAuditoriaGlobal").on('click', __AbrirModalAuditoria);
    $(window).on('hashchange', __ClearAuditoria)
}

/*****CONSTRUTORES******/
function HeaderAuditoria(entidade, knout, propCodigo, dicionario, downloadExcell, sempreExibir) {
    /**
     * Entidade: string que contem o nome da entidade mapeada no servidor
     * Knout: Objeto knockout usando na tela (para correção do campo "De" "Para" e uso do código incremental)
     * PropCodigo: Nome da propriedade do objeto knockout para controlar código único (Valor Padrão: Codigo)
     * Dicionario: Objeto com "palavra" "valor" usado para melhor visualização da auditoria "De" "Para" (Parâmetro Opcional)
     *
     * Descrição:
     * Esse construtor é utilizado em telas que possuem CRUD simples
     * É a função autonoma que seta os valores relacionados e exibe o botão para 
     * visualizar as auditorias. 
     * 
     * A abertura do modal é controlado pelo acionamento do botão
     */
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        downloadExcell = true;

    if (sempreExibir == null)
        sempreExibir = false;

    if (!_CONFIGURACAO_TMS.PermiteAuditar && !sempreExibir)
        return false;

    propCodigo = propCodigo || "Codigo";
    downloadExcell = downloadExcell || false;

    if (!__Auditoria)
        __LoadAuditoria();

    __Auditoria.Entidade.val(entidade);
    __Auditoria.knout.val(knout);
    __Auditoria.DownloadExcell.val(downloadExcell);

    if (knout && propCodigo in knout) {
        knout[propCodigo].val.subscribe(function (novoCodigo) {
            __Auditoria.Codigo.val(novoCodigo);
        });
    }

    var stringDicionario = __GerarDicionario(knout, dicionario || {}, true);
    __DetalheAuditoria.Dicionario.val(stringDicionario);

    $("#divAuditoriaGlobal").show();
}
function OpcaoAuditoria(entidade, propCodigo, knout, dicionario) {
    /**
    * Não usar
    */
    if (!_CONFIGURACAO_TMS.PermiteAuditar)
        return function () { };

    propCodigo = propCodigo || "Codigo";

    return function (dataRow) {
        if ("data" in dataRow)
            dataRow = dataRow.data;

        if (!(propCodigo in dataRow))
            return false;

        // Salva Entidade e Ko do header para setar quando fechar o modal do grid
        var _bk_Entidade = __Auditoria.Entidade.val();
        var _bk_Codigo = __Auditoria.Codigo.val();
        var _bk_knout = __Auditoria.knout.val();
        var _bk_Dicionario = __DetalheAuditoria.Dicionario.val();

        __Auditoria.Entidade.val(entidade);
        __Auditoria.knout.val({});
        __Auditoria.Codigo.val(dataRow[propCodigo]);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
            __Auditoria.DownloadExcell.val(true);

        var stringDicionario = __GerarDicionario(knout, dicionario || {}, true);
        __DetalheAuditoria.Dicionario.val(stringDicionario);

        __AbrirModalAuditoria(null, function () {
            __Auditoria.Entidade.val(_bk_Entidade);
            __Auditoria.Codigo.val(_bk_Codigo);
            __Auditoria.knout.val(_bk_knout);
            __DetalheAuditoria.Dicionario.val(_bk_Dicionario);
        });
    }
}

function PermiteAuditar() {
    return _CONFIGURACAO_TMS.PermiteAuditar;
}

function VisibilidadeOpcaoAuditoria() {
    return PermiteAuditar();
}

function GerarDicionarioKnout(knout, extend) {
    return __GerarDicionario(knout, extend || {}, false);
}

function AuditoriaExportavel(val) {
    __Auditoria.DownloadExcell.val(val);
}