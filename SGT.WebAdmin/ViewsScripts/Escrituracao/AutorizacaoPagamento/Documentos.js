// #region Objetos Globais do Arquivo

var _documentos;

// #endregion Objetos Globais do Arquivo

// #region Classes

var Documentos = function (idContainerDocumentos) {
    this._idContainerDocumentos = idContainerDocumentos;

    this._init();
};

Documentos.prototype = {
    carregar: function (listaDocumentosPorCarga) {
        if (listaDocumentosPorCarga.length == 0) {
            this._carregarSemRegistros();
            return;
        }

        this._carregarDocumentosPorCarga(listaDocumentosPorCarga);
        this._carregarDocumentosPorDestinatario(listaDocumentosPorCarga);
        this._adicionarEventoClick();
    },
    limpar: function () {
        this._removerEventoClick();
        this._carregarSemRegistros();
    },
    _adicionarEventoClick: function () {
        $("#" + this._idContainerDocumentos).on('click', '.dd-handle', function (e) {
            e.stopPropagation();
            $(e.currentTarget).parent().toggleClass('dd-collapsed');
        });
    },
    _carregarDocumentosPorCarga: function (listaDocumentosPorCarga) {
        var html = '<ol class="dd-list">';

        for (var i = 0; i < listaDocumentosPorCarga.length; i++) {
            var documentosPorCarga = listaDocumentosPorCarga[i];

            html += '<li class="dd-item">';
            html += '    <div class="dd-handle">Carga ' + documentosPorCarga.CodigoCargaEmbarcador + '</div>';
            html += '    <ol class="dd-list">';
            html += '        <li class="dd-item">';
            html += '            <div class="row">';
            html += '                <div class="col col-xs-6 col-md-3">';
            html += '                    <label><b>Peso: </b> <span>' + documentosPorCarga.Peso + '</span></label>';
            html += '                </div>';
            html += '                <div class="col col-xs-6 col-md-3">';
            html += '                    <label><b>Valor do Frete: </b> <span>' + documentosPorCarga.ValorFrete + '</span></label>';
            html += '                </div>';
            html += '                <div class="col col-xs-6 col-md-3">';
            html += '                    <label><b>Valor sem ICMS: </b> <span>' + documentosPorCarga.ValorFreteSemIcms + '</span></label>';
            html += '                </div>';
            html += '                <div class="col col-xs-6 col-md-3">';
            html += '                    <label><b>Valor por Tonelada: </b> <span>' + documentosPorCarga.ValorFretePorTonelada + '</span></label>';
            html += '                </div>';
            html += '                <div class="col col-xs-6 col-md-3">';
            html += '                    <label><b>Percentual de Ocupação: </b> <span>' + documentosPorCarga.PercentualOcupacao + '</span></label>';
            html += '                </div>';
            html += '            </div>';
            html += '        </li>';

            for (var j = 0; j < documentosPorCarga.Destinatarios.length; j++) {
                var destinatario = documentosPorCarga.Destinatarios[j];

                html += '<li class="dd-item">';
                html += '    <div class="dd-handle dd-handle-child">' + destinatario.DescricaoDestinatario + '</div>';
                html += '    <ol class="dd-list">';
                html += '        <li class="dd-item" >';
                html += '            <div class="row">';
                html += '                <div class="col col-xs-6 col-md-3">';
                html += '                    <label><b>Valor do Frete: </b> <span>' + destinatario.ValorFreteCliente + '</span></label>';
                html += '                </div>';
                html += '                <div class="col col-xs-6 col-md-3">';
                html += '                    <label><b>Valor sem ICMS: </b> <span>' + destinatario.ValorFreteClienteSemIcms + '</span></label>';
                html += '                </div>';
                html += '                <div class="col col-xs-6 col-md-3">';
                html += '                    <label><b>Valor por Tonelada: </b> <span>' + destinatario.ValorFreteClientePorTonelada + '</span></label>';
                html += '                </div>';
                html += '            </div>';
                html += '        </li>';
                html += '        <li class="dd-item" >';
                html += '            <table width="100%" class="table table-bordered table-hover table-condensed table-striped dataTable no-footer" id="grid_documentos-' + documentosPorCarga.CodigoCarga + '-' + destinatario.CpfCnpjDestinatario + '" cellspacing="0"></table>';
                html += '        </li>';
                html += '    </ol>';
                html += '</li>';
            }

            html += '    </ol>';
            html += '</li>';
        }

        html += '</ol>'

        $("#" + this._idContainerDocumentos).html(html);
    },
    _carregarDocumentosPorDestinatario: function (listaDocumentosPorCarga) {
        for (var i = 0; i < listaDocumentosPorCarga.length; i++) {
            var documentosPorCarga = listaDocumentosPorCarga[i];

            for (var j = 0; j < documentosPorCarga.Destinatarios.length; j++) {
                var destinatario = documentosPorCarga.Destinatarios[j];
                var header = [
                    { data: "Documento", title: "Documento", width: "9%", className: "text-align-center", orderable: false },
                    { data: "Tipo", title: "Tipo", width: "8%", className: "text-align-center", orderable: false },
                    { data: "Origem", title: "Origem", width: "17%", className: "text-align-left", orderable: false },
                    { data: "Destino", title: "Destino", width: "17%", className: "text-align-left", orderable: false },
                    { data: "Ocorrencia", title: "Ocorrência", width: "9%", className: "text-align-center", orderable: false },
                    { data: "TipoOcorrencia", title: "Tipo de Ocorrência", width: "16%", className: "text-align-left", orderable: false },
                    { data: "FreteEmergencial", title: "Frete Emergencial", width: "8%", className: "text-align-center", orderable: false },
                    { data: "ValorFrete", title: "Valor do Frete", width: "8%", className: "text-align-right", orderable: false },
                    { data: "ValorFreteSemIcms", title: "Valor sem ICMS", width: "8%", className: "text-align-right", orderable: false }
                ];

                var gridDocumentos = new BasicDataTable("grid_documentos-" + documentosPorCarga.CodigoCarga + "-" + destinatario.CpfCnpjDestinatario, header);

                gridDocumentos.CarregarGrid(destinatario.ListaDocumentos);
            }
        }
    },
    _carregarSemRegistros: function () {
        var html = '';

        html += '<div class="row">';
        html += '    <div class="col col-sm-12">';
        html += '        <div class="alert alert-info">Não existem documentos para este pagamento.</div>';
        html += '    </div>';
        html += '</div>';

        $("#" + this._idContainerDocumentos).html(html);
    },
    _init: function () {
        this._carregarSemRegistros();
    },
    _removerEventoClick: function () {
        $("#" + this._idContainerDocumentos).off('click', '**');
    }
};

// #endregion Classes

// #region Funções de Inicialização

function loadDocumentos() {
    _documentos = new Documentos("pagamento_documentos");
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function limparDocumentos() {
    _documentos.limpar();
}

function preencherDocumentos(listaDocumentosPorCarga) {
    _documentos.carregar(listaDocumentosPorCarga);
}

// #endregion Funções Públicas
