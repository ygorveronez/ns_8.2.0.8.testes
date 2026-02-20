/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/PerfilAcessoTransportador.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _ListaPaginas;
var _ListaModulos;
var _permissaoTransportador;

function PermissaoTransportador() {
    this.PerfilAcessoTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.PerfilAcesso.getFieldDescription(), issue: 597, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TransportadorAdministrador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Transportadores.Transportador.EstePerfilPermissoesAdministradorAcessoCompletoSistema, visible: ko.observable(true) });
    this.HabilitaSincronismoDocumentosDestinados = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Transportadores.Transportador.HabilitarSincronismoDocumentosDestinadosEmpresa, visible: ko.observable(false) });
}

var FormularioUsuarioMap = function () {
    this.CodigoFormulario = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.SomenteLeitura = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: false });
    this.PermissoesPersonalizadas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
}

var PermissaoPersonalizadaMap = function () {
    this.CodigoPermissaoPersonalizada = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
}

var ModuloUsuarioMap = function () {
    this.CodigoModulo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******

function loadPermissaoTransportador() {
    _permissaoTransportador = new PermissaoTransportador();
    KoBindings(_permissaoTransportador, "knockoutCadastroPermissaoTransportador");

    new BuscarPerfilAcessoTransportador(_permissaoTransportador.PerfilAcessoTransportador, PerfilAcessoOnChange);
    BuscarPaginas();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        _permissaoTransportador.PerfilAcessoTransportador.visible(false);
        _permissaoTransportador.TransportadorAdministrador.val(true);
        _permissaoTransportador.HabilitaSincronismoDocumentosDestinados.val(false);
        _permissaoTransportador.HabilitaSincronismoDocumentosDestinados.visible(true);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _permissaoTransportador.HabilitaSincronismoDocumentosDestinados.visible(true);
    }
}

function PerfilAcessoOnChange(perfil) {
    executarReST("PerfilAcessoTransportador/BuscarPorPerfil", { Perfil: perfil.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data == null) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            } else {
                limparPermissoesTransportador();
                PreencherObjetoKnout(_transportador, arg);
                SetarPermissoesModulosFormularios();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function ckbLiberarAcessoClick(codigoModulo, codigoFormulario) {
    if ($("#ckbLiberar_" + codigoFormulario).prop("checked")) {
        if ($("#tableFormularios_" + codigoModulo + " input.liberarAcesso:checkbox:not(:checked)").length == 0) {
            $("#ckb_liberarTodos_" + codigoModulo).prop("checked", true);
        }
        $("#trPermissaoPersonalizada_" + codigoFormulario).slideDown();
    } else {
        $("#ckbSomenteLeitura_" + codigoFormulario).prop("checked", false);
        $("#ckb_liberarTodos_" + codigoModulo).prop("checked", false);
        $("#ckb_SomenteLeituraTodos_" + codigoModulo).prop("checked", false);
        $("#trPermissaoPersonalizada_" + codigoFormulario).slideUp();
    }
}

function ckbSomenteLeituraClick(codigoModulo, codigoFormulario) {
    if ($("#ckbSomenteLeitura_" + codigoFormulario).prop("checked")) {
        $("#ckbLiberar_" + codigoFormulario).prop("checked", true);
        if ($("#tableFormularios_" + codigoModulo + " input.apenasLeitura:checkbox:not(:checked)").length == 0) {
            $("#ckb_liberarTodos_" + codigoModulo).prop("checked", true);
            $("#ckb_SomenteLeituraTodos_" + codigoModulo).prop("checked", true);
            $("#trPermissaoPersonalizada_" + codigoFormulario).slideDown();
        }
    } else {
        $("#ckbSomenteLeitura_" + codigoFormulario).prop("checked", false);
        $("#ckb_SomenteLeituraTodos_" + codigoModulo).prop("checked", false);
    }
}

function ckbTodosFormulariosClick(codigoModulo) {
    if ($("#ckb_liberarTodos_" + codigoModulo).prop("checked")) {
        $("#tableFormularios_" + codigoModulo + " input.liberarAcesso").prop("checked", true);
        $("#tableFormularios_" + codigoModulo + " input.permissaoPersonalizada").prop("checked", true);
        $("#tableFormularios_" + codigoModulo + " tr.trPermissaoPersonalizada").slideDown();
    } else {
        $("#tableFormularios_" + codigoModulo + " input").prop("checked", false);
        $("#tableFormularios_" + codigoModulo + " tr.trPermissaoPersonalizada").slideUp();
    }
}

function ckbSomenteLeituraTodosFormulariosClick(codigoModulo) {
    if ($("#ckb_SomenteLeituraTodos_" + codigoModulo).prop("checked")) {
        $("#tableFormularios_" + codigoModulo + " input").prop("checked", true);
        $("#tableFormularios_" + codigoModulo + " input.permissaoPersonalizada").prop("checked", false);
        $("#tableFormularios_" + codigoModulo + " tr.trPermissaoPersonalizada").slideDown();
    } else {
        $("#tableFormularios_" + codigoModulo + " input.apenasLeitura").prop("checked", false);
    }
}

function ckbModuloLiberadoClick(codigoModulo) {
    if ($("#ckbModuloLiberado_" + codigoModulo).prop("checked")) {
        $("#ulModulo_" + codigoModulo).slideUp();
    } else {
        $("#ulModulo_" + codigoModulo).slideDown();
        $("#ulModulo_" + codigoModulo + " li").show();
    }
}


//*******MÉTODOS*******
function BuscarPaginas() {
    executarReST("Pagina/BuscarModulosMultiCTe", null, function (arg) {
        if (arg.Success) {
            _ListaModulos = arg.Data;
            PreecherHTMLTreeview();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    })
}

function PreecherHTMLTreeview() {
    var html = "<ul>";
    html += CriarHTMLModulos(_ListaModulos);
    html += "</ul>";

    $("#divTreeView").html(html);
    loadTreeView();
}

function CriarHTMLModulos(modulos) {
    var html = [];

    $.each(modulos, function (i, modulo) {
        var _html = [
            "<li>",
            "<span><i class='fa fa-lg " + modulo.Icone + "'></i> " + modulo.Descricao + "</span>",
            " &nbsp; ",
            "<div style='position:absolute; margin-top:7px;display: inline-block'>",
            "<input type='checkbox' onclick='ckbModuloLiberadoClick(" + modulo.CodigoModulo + ");' id='ckbModuloLiberado_" + modulo.CodigoModulo + "' name='checkbox-inline'>",
            " &nbsp; ",
            "<label for='ckbModuloLiberado_" + modulo.CodigoModulo + "' > " + Localization.Resources.Transportadores.Transportador.LiberarAcessoTotalAoMenu + " " + modulo.Descricao + "</label>",
            "</div>",
            "<ul id='ulModulo_" + modulo.CodigoModulo + "'>",
            (modulo.Formularios.length > 0 ? CriarHTMLFormularios(modulo) : ""),
            (modulo.ModulosFilho.length > 0 ? CriarHTMLModulos(modulo.ModulosFilho) : ""),
            "</ul>",
            "</li>"
        ];

        html.push(_html.join("\n"));
    });

    return html.join("\n");
}

function CriarHTMLFormularios(modulo) {
    var html = [
        "<li>",
        "<table class='table table-bordered table-hover table-condensed table-striped' style='margin-left: -37px;position: relative;z-index: 1;margin-top: -5px;background: #FFF;' id='tableFormularios_" + modulo.CodigoModulo + "'>",
        "<thead>",
        "<tr>",
        "<td style='width:8%' title=" + Localization.Resources.Transportadores.Transportador.LiberarAcesso + ">",
        "<input type='checkbox' style='margin-top:0px' onclick='ckbTodosFormulariosClick(" + modulo.CodigoModulo + ");' id='ckb_liberarTodos_" + modulo.CodigoModulo + "' />",
        "&nbsp;",
        "<label for='ckb_liberarTodos_" + modulo.CodigoModulo + "'>" + Localization.Resources.Transportadores.Transportador.LiberarAcesso + "</label>",
        "</td>",

        "<td style='width:8%' title=" + Localization.Resources.Transportadores.Transportador.SomenteLeitura + " >",
        "<input type='checkbox' style='margin-top:0px' onclick='ckbSomenteLeituraTodosFormulariosClick(" + modulo.CodigoModulo + ");' id='ckb_SomenteLeituraTodos_" + modulo.CodigoModulo + "' />",
        "&nbsp;",
        "<label for='ckb_SomenteLeituraTodos_" + modulo.CodigoModulo + "'>" + Localization.Resources.Transportadores.Transportador.SomenteLeitura + " </label>",
        "</td>",
        "<td>",
        "<label>Formulário</label>",
        "</td>",
        "</tr>",
        "</thead>",
        "<tbody>"
    ];

    $.each(modulo.Formularios, function (i, formulario) {
        var _html = [
            "<tr>",
            "<td>",
            "<input type='checkbox' style='margin-top:0px' class='liberarAcesso' onclick='ckbLiberarAcessoClick(" + modulo.CodigoModulo + ", " + formulario.CodigoFormulario + ");' id='ckbLiberar_" + formulario.CodigoFormulario + "' />",
            "</td>",
            "<td>",
            "<input type='checkbox' style='margin-top:0px' class='apenasLeitura' onclick='ckbSomenteLeituraClick(" + modulo.CodigoModulo + ", " + formulario.CodigoFormulario + ");' id='ckbSomenteLeitura_" + formulario.CodigoFormulario + "' />",
            "</td>",
            "<td>",
            "<label>" + formulario.Descricao + "</label>",
            "</td>",
            "</tr>",
            (formulario.PermissoesPersonalizadas.length > 0 ? CriarHTMLPermissoesPersonalizadas(formulario) : ""),
        ];

        html.push(_html.join("\n"));
    });

    html.push("</tbody>");
    html.push("</table>");
    html.push("</li>");

    return html.join("\n");
}

function CriarHTMLPermissoesPersonalizadas(formulario) {
    var html = "<tr id='trPermissaoPersonalizada_" + formulario.CodigoFormulario + "'  class='trPermissaoPersonalizada' style='display:none'><td colspan='3'><h4 style='padding:5px;'> " + formulario.Descricao + Localization.Resources.Transportadores.Transportador.PermissoesEspeciais + "</h4><table id='' class='table table-bordered table-hover table-condensed table-striped'>";
    $.each(formulario.PermissoesPersonalizadas, function (i, permissao) {
        html += "<tr>";
        html += "<td><input type='checkbox' class='permissaoPersonalizada' style='margin-top:0px' id='ckbLiberarPermissao_" + formulario.CodigoFormulario + "_" + permissao.CodigoPermissao + "' /><label style='margin-left:10px;' for='ckbLiberarPermissao_" + formulario.CodigoFormulario + "_" + permissao.CodigoPermissao + "'><b>" + permissao.Descricao + "</b></label></td>";
        html += "</tr>";
    });
    html += "</table></td></tr>";

    return html;
}

function SetarPermissoesModulosFormularios() {
    $.each(_transportador.ModulosTransportador.list, function (i, modulo) {
        $("#ckbModuloLiberado_" + modulo.CodigoModulo.val).prop("checked", true);
        ckbModuloLiberadoClick(modulo.CodigoModulo.val);
    });

    var listaModuloFormulario = new Array();
    $.each(_transportador.FormulariosTransportador.list, function (i, formulario) {
        if ($("#ckbLiberar_" + formulario.CodigoFormulario.val).closest("table").attr("id") != null) {
            $("#ckbLiberar_" + formulario.CodigoFormulario.val).prop("checked", true);
            if (formulario.SomenteLeitura.val) {
                $("#ckbSomenteLeitura_" + formulario.CodigoFormulario.val).prop("checked", true);
            }

            $("#trPermissaoPersonalizada_" + formulario.CodigoFormulario.val).show();

            var idModulo = parseInt($("#ckbLiberar_" + formulario.CodigoFormulario.val).closest("table").attr("id").split("_")[1]);

            $.each(formulario.PermissoesPersonalizadas.list, function (p, permissaoPersonalizada) {
                $("#ckbLiberarPermissao_" + formulario.CodigoFormulario.val + "_" + permissaoPersonalizada.CodigoPermissao.val).prop("checked", true);
            });

            //utilizado apenas para disparar os eventos de cliente para deixar o layout visivel conforme as permissões
            if (listaModuloFormulario.length > 0) {
                var novoModulo = true;
                $.each(listaModuloFormulario, function (j, moduloFormulario) {
                    if (moduloFormulario.modulo == idModulo) {
                        novoModulo = false;
                        return false;
                    }
                });
                if (novoModulo)
                    listaModuloFormulario.push({ formulario: formulario.CodigoFormulario.val, modulo: idModulo })
            } else
                listaModuloFormulario.push({ formulario: formulario.CodigoFormulario.val, modulo: idModulo })
        }
    });

    $.each(listaModuloFormulario, function (i, moduloFormulario) {
        ckbLiberarAcessoClick(moduloFormulario.modulo, moduloFormulario.formulario);
        ckbSomenteLeituraClick(moduloFormulario.modulo, moduloFormulario.formulario);
    });
}

function limparPermissoesTransportador() {
    _transportador.FormulariosTransportador.list = new Array();
    _transportador.ModulosTransportador.list = new Array();
    PreecherHTMLTreeview();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        _permissaoTransportador.PerfilAcessoTransportador.visible(false);
        _permissaoTransportador.TransportadorAdministrador.val(true);
        _permissaoTransportador.HabilitaSincronismoDocumentosDestinados.val(false);
        _permissaoTransportador.HabilitaSincronismoDocumentosDestinados.visible(true);
    }
}

function BuscarPermissoesFormularios() {
    _transportador.FormulariosTransportador.list = new Array();
    _transportador.ModulosTransportador.list = new Array();
    PreecherPermissoes(_ListaModulos);
}

function PreecherPermissoes(modulos) {
    $.each(modulos, function (i, modulo) {
        if ($("#ckbModuloLiberado_" + modulo.CodigoModulo).prop("checked")) {
            var moduloMap = new ModuloUsuarioMap();
            moduloMap.CodigoModulo.val = modulo.CodigoModulo;
            _transportador.ModulosTransportador.list.push(moduloMap);
        }

        $.each(modulo.Formularios, function (j, formulario) {
            if ($("#ckbLiberar_" + formulario.CodigoFormulario).prop("checked")) {
                var formularioMap = new FormularioUsuarioMap();
                formularioMap.CodigoFormulario.val = formulario.CodigoFormulario;
                formularioMap.SomenteLeitura.val = $("#ckbSomenteLeitura_" + formulario.CodigoFormulario).prop("checked");
                formularioMap.PermissoesPersonalizadas.list = new Array();
                $.each(formulario.PermissoesPersonalizadas, function (p, permissaoPersonalizada) {
                    if ($("#ckbLiberarPermissao_" + formulario.CodigoFormulario + "_" + permissaoPersonalizada.CodigoPermissao).prop("checked")) {
                        var permissaoPersonalizadaMap = new PermissaoPersonalizadaMap();
                        permissaoPersonalizadaMap.CodigoPermissaoPersonalizada.val = permissaoPersonalizada.CodigoPermissao;
                        formularioMap.PermissoesPersonalizadas.list.push(permissaoPersonalizadaMap);
                    }
                });
                _transportador.FormulariosTransportador.list.push(formularioMap);
            }
        });

        if (modulo.ModulosFilho.length > 0)
            PreecherPermissoes(modulo.ModulosFilho); //faz a recursividade de modulos
    });
}