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
/// <reference path="../../../js/libs/TreeViewLoad.js" />
/// <reference path="PerfilAcesso.js" />



//*******MAPEAMENTO KNOUCKOUT*******
var _ListaPaginas;
var _ListaModulos;

var PaginaUsuarioMap = function () {
    this.Pagina = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.PermissaoDeAcesso = PropertyEntity({ type: types.map, val: "A" });
    this.PermissaoDeAlteracao = PropertyEntity({ type: types.map, val: "A" });
    this.PermissaoDeDelecao = PropertyEntity({ type: types.map, val: "A" });
    this.PermissaoDeInclusao = PropertyEntity({ type: types.map, val: "A" });
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

function buscarPaginas() {
    executarReST("Pagina/BuscarModulosMultiCTe", null, function (arg) {
        if (arg.Success) {
            _ListaModulos = arg.Data;
            preecherHTMLTreeview();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    })
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


function preecherHTMLTreeview() {
    var html = "<ul>";
    html += criarHTMLModulos(_ListaModulos);
    html += "</ul>";
    $("#divTreeView").html(html);
    loadTreeView();
}

function criarHTMLModulos(modulos) {
    var html = "";
    $.each(modulos, function (i, modulo) {
        html += "<li>";
        html += "<span><i class='fal fa-lg " + modulo.Icone + "'></i> " + modulo.Descricao + "</span> &nbsp; &nbsp; <text style='position:absolute; margin-top:3px;'><input type='checkbox' onclick='ckbModuloLiberadoClick(" + modulo.CodigoModulo + ");' id='ckbModuloLiberado_" + modulo.CodigoModulo + "' name='checkbox-inline'>&nbsp; <label for='ckbModuloLiberado_" + modulo.CodigoModulo + "' > Liberar acesso total ao menu " + modulo.Descricao + "</label></text>"
        
        html += "<ul id='ulModulo_" + modulo.CodigoModulo + "'>";
        if (modulo.Formularios.length > 0)
            html += criarHTMLFormularios(modulo);
        if (modulo.ModulosFilho.length > 0)
            html += criarHTMLModulos(modulo.ModulosFilho);
        html += "</ul>";
        html += "</li>";
    });
    return html;
}

function criarHTMLFormularios(modulo) {
    var html = "";
    html += "<li>";
    html += "<table class='table table-bordered table-hover table-condensed table-striped' style='margin-left: -37px;position: relative;z-index: 1;margin-top: -5px;background: #FFF;' id='tableFormularios_" + modulo.CodigoModulo + "'>";

    html += "<thead>";
    html += "<tr>";
    html += "<td style='width:8%' title='Liberar Acesso'><input type='checkbox' style='margin-top:0px' onclick='ckbTodosFormulariosClick(" + modulo.CodigoModulo + ");' id='ckb_liberarTodos_" + modulo.CodigoModulo + "' />&nbsp;<label for='ckb_liberarTodos_" + modulo.CodigoModulo + "'>Liberar Acesso</label></td>";
    html += "<td style='width:8%' title='Somente Leitura'><input type='checkbox' style='margin-top:0px'  onclick='ckbSomenteLeituraTodosFormulariosClick(" + modulo.CodigoModulo + ");' id='ckb_SomenteLeituraTodos_" + modulo.CodigoModulo + "' />&nbsp;<label for='ckb_SomenteLeituraTodos_" + modulo.CodigoModulo + "'>Somente Leitura </label></td>";
    html += "<td><label>Formulário</label></td>";
    html += "</tr>";
    html += "</thead>";
    html += "<tbody>";
    $.each(modulo.Formularios, function (i, formulario) {
        html += "<tr><td><input type='checkbox' style='margin-top:0px' class='liberarAcesso' onclick='ckbLiberarAcessoClick(" + modulo.CodigoModulo + ", " + formulario.CodigoFormulario + ");' id='ckbLiberar_" + formulario.CodigoFormulario + "' /></td>";
        html += "<td><input type='checkbox' style='margin-top:0px' class='apenasLeitura' onclick='ckbSomenteLeituraClick(" + modulo.CodigoModulo + ", " + formulario.CodigoFormulario + ");' id='ckbSomenteLeitura_" + formulario.CodigoFormulario + "' /></td>";
        html += "<td><label>" + formulario.Descricao + "</label></td>";
        html += "</tr>";

        if (formulario.PermissoesPersonalizadas.length > 0) {
            html += "<tr id='trPermissaoPersonalizada_" + formulario.CodigoFormulario + "'  class='trPermissaoPersonalizada' style='display:none'><td colspan='3'><h4 style='padding:5px;'> " + formulario.Descricao + " Permissões Especiais </h4><table id='' class='table table-bordered table-hover table-condensed table-striped'>";
            $.each(formulario.PermissoesPersonalizadas, function (i, permissao) {
                html += "<tr>";
                html += "<td><input type='checkbox' class='permissaoPersonalizada' style='margin-top:0px' id='ckbLiberarPermissao_" + formulario.CodigoFormulario + "_" + permissao.CodigoPermissao + "' /><label style='margin-left:10px;' for='ckbLiberarPermissao_" + formulario.CodigoFormulario + "_" + permissao.CodigoPermissao + "'><b>" + permissao.Descricao + "</b></label></td>";
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


function buscarPermissoesFormularios() {
    _perfil.FormulariosPerfil.list = new Array();
    _perfil.ModulosPerfil.list = new Array();
    PreecherPermissoes(_ListaModulos);
}

function PreecherPermissoes(modulos) {
    $.each(modulos, function (i, modulo) {
        if ($("#ckbModuloLiberado_" + modulo.CodigoModulo).prop("checked")) {
            var moduloMap = new ModuloUsuarioMap();
            moduloMap.CodigoModulo.val = modulo.CodigoModulo;
            _perfil.ModulosPerfil.list.push(moduloMap);
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
                _perfil.FormulariosPerfil.list.push(formularioMap);
            }
        });

        if (modulo.ModulosFilho.length > 0)
            PreecherPermissoes(modulo.ModulosFilho); //faz a recursividade de modulos
    });
}

function setarPermissoesModulosFormularios() {
    $.each(_perfil.ModulosPerfil.list, function (i, modulo) {
        $("#ckbModuloLiberado_" + modulo.CodigoModulo.val).prop("checked", true);
        ckbModuloLiberadoClick(modulo.CodigoModulo.val);
    });
    var listaModuloFormulario = new Array();
    $.each(_perfil.FormulariosPerfil.list, function (i, formulario) {
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


function limparPermissoesModulosFormularios() {
    _perfil.FormulariosPerfil.list = new Array();
    _perfil.ModulosPerfil.list = new Array();
    preecherHTMLTreeview();
}