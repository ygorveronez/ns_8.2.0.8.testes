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
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _liberacaoIntegracao, _CRUDLiberacaoIntegracao;
var _listaCategorias, _listaCategoriasMarcadas;

var LiberacaoIntegracao = function () {
   
};

var CRUDLiberacaoIntegracao = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Configuracoes.LiberacaoIntegracao.Atualizar, visible: ko.observable(true) });
};

function loadLiberacaoIntegracao() {

    _liberacaoIntegracao = new LiberacaoIntegracao();
    KoBindings(_liberacaoIntegracao, "knockoutLiberacaoIntegracao");

    _CRUDLiberacaoIntegracao = new CRUDLiberacaoIntegracao();
    KoBindings(_CRUDLiberacaoIntegracao, "knockoutCRUDLiberacaoIntegracao");

    HeaderAuditoria("LiberacaoIntegracao", _liberacaoIntegracao);

    ObterDados();

}

function ObterDados() {
    executarReST("LiberacaoIntegracao/ObterDados", {}, function (r) {
        if (r.Success && r.Data) {
            _listaCategorias = r.Data;
            preecherHTMLTreeview();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function preecherHTMLTreeview() {
    var html = "<ul>";
    html += criarHTMLCategorias(_listaCategorias);
    html += "</ul>";
    $("#divTreeView").html(html);
    loadTreeView();
}

function criarHTMLCategorias(categorias) {
    var html = "";
    $.each(categorias, function (i, categoria) {
        html += "<li>";
        html += "<span style='cursor: pointer;' data-codigo='" + categoria.Codigo + "'><i class='fa " + categoria.Icone + "'></i> " + categoria.Descricao + "</span>";
        html += "<ul id='ulCategoria_" + categoria.Codigo + "' >";
        if (categoria.Integracoes.length > 0)
            html += criarHTMLIntegracoes(categoria);
        html += "</ul>";
        html += "</li>";
    });
    return html;
}
function criarHTMLIntegracoes(categoria) {
    var html = "";
    html += "<li>";
    html += "<table class='table table-bordered table-hover table-condensed table-striped' style='margin-left: -37px;position: relative;z-index: 1;margin-top: -5px;background: #FFF;' id='tableCategorias_" + categoria.Codigo + "'>";
    html += "<thead>";
    html += "<tr>";
    html += "<td style='width:8%' title='Liberar Integração'>Liberar</td>";
    html += "<td><label>Integração</label></td>";
    html += "</tr>";
    html += "</thead>";
    html += "<tbody>";
    $.each(categoria.Integracoes, function (i, integracao) {
        html += "<tr><td><input type='checkbox' style='margin-top:0px' " + (integracao.Ativo ? "checked" : "") + " id='ckbLiberar_" + categoria.Codigo + "_" + integracao.Codigo + "' onclick='ckbLiberarIntegracaoClick(" + categoria.Codigo + ", " + integracao.Codigo + ");' /></td>";
        html += "<td><label>" + integracao.Descricao + "</label></td>";
        html += "</tr>";
        
        if (Object.keys(integracao.Parametros).length > 0) {
            var parametrosDisplay = integracao.Ativo ? "" : "style='display:none'";
            html += "<tr id='trParametro_" + integracao.Codigo + "' class='trParametro' " + parametrosDisplay + ">";
            // html += "<td colspan='3'><h4 style='padding:5px;'> " + integracao.Descricao + " - Parâmetros </h4>";
            html += "<td colspan='2'>";
            html += "<table class='table table-bordered table-hover table-condensed table-striped'>";
            $.each(integracao.Parametros, function (key, parametro) {
                html += "<tr>";
                html += "<td><input type='checkbox' " + (parametro.Valor == "True" ? "checked" : "") + " style='margin-top:0px' id='Parametro_" + integracao.Codigo + "_" + key + "' />";
                html += "<label style='margin-left:10px;' for='Parametro_" + integracao.Codigo + "_" + key + "'>" + parametro.Descricao + "</label></td>";
                html += "</tr>";
            });
            html += "</table></td></tr>";
        }
    });

    html += "</tbody>";
    html += "</table>";
    html += "</li>";
    return html;
}

//*******EVENTOS*******
function ckbLiberarIntegracaoClick(codigoCategoria, codigoIntegracao) {
    if ($("#ckbLiberar_" + codigoCategoria + "_" + codigoIntegracao).prop("checked")) {
        $("#trParametro_" + codigoIntegracao).slideDown(); 
    } else {
        $("#trParametro_" + codigoIntegracao).slideUp();
    }
}
function atualizarClick(e, sender) {
    var categoriasParaSalvar = [];

    $.each(_listaCategorias, function (i, categoria) {
        var categoriaParaSalvar = {
            Codigo: categoria.Codigo,
            Descricao: categoria.Descricao,
            Integracoes: []
        };
        
        $.each(categoria.Integracoes, function (j, integracao) {
            var integracaoParaSalvar = {
                Codigo: integracao.Codigo,
                Descricao: integracao.Descricao,
                Ativo: $("#ckbLiberar_" + categoria.Codigo + "_" + integracao.Codigo).prop("checked"),
                Parametros: {} 
            };
            
            if (Object.keys(integracao.Parametros).length > 0) {
                $.each(integracao.Parametros, function (key, parametro) {
                    var valorParametro = $("#Parametro_" + integracao.Codigo + "_" + key).prop("checked");

                    integracaoParaSalvar.Parametros[key] = {
                        NomeColuna: parametro.NomeColuna,
                        Descricao: parametro.Descricao,
                        Valor: valorParametro
                    };
                });
            }

            categoriaParaSalvar.Integracoes.push(integracaoParaSalvar);
        });

        categoriasParaSalvar.push(categoriaParaSalvar);
    });

    executarReST("LiberacaoIntegracao/Atualizar", { categorias: categoriasParaSalvar }, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
            ObterDados();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}
