// #region Objetos Globais do Arquivo

var $container;
var $resumo;
var _tableCanhoto = null;

// #endregion Objetos Globais do Arquivo

// #region Funções Inicialização

function loadControleAbas() {
    $resumo = $('#paginacaoResumo');

    $container = $('#paginacaoCanhotos').twbsPagination({
        first: 'Primeiro',
        prev: 'Anterior',
        next: 'Próximo',
        last: 'Último',
        initiateStartPageClick: false,
        totalPages: 1,
        visiblePages: 3,
        onPageClick: function (event, page) {
            _tableCanhoto.page(page - 1).draw('page');
            atualizarResumo();
        }
    });
}

// #endregion Funções Inicialização

// #region Funções Públicas

function iniciarControleAbas(table) {
    _tableCanhoto = table;
    var datapages = _tableCanhoto.page.info();

    if ((datapages == undefined) || isNaN(datapages.pages)) {
        limparResumo();
        $container.twbsPagination('disable');
        return;
    }

    $container.twbsPagination("changeTotalPages", datapages.pages, 1);
    atualizarResumo();
}

// #endregion Funções Públicas

// #region Funções Privadas

function atualizarResumo() {
    var datapages = _tableCanhoto.page.info();
    var resumo = Localization.Resources.Canhotos.Canhoto.ExibindoStartAteEndDeTotalRegistros;

    resumo = resumo
        .replace("_START_", Globalize.format(datapages.start + 1, "n0"))
        .replace("_END_", Globalize.format(datapages.end, "n0"))
        .replace("_TOTAL_", Globalize.format(datapages.recordsTotal, "n0"));

    $resumo.html(resumo);
}

function limparResumo() {
    $resumo.html('');
}

// #endregion Funções Privadas
