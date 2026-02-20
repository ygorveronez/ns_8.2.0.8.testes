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
/// <reference path="../../../js/libs/TreeViewLoad.js" />
/// <reference path="PerfilAcessoMobile.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _ListaModulosMobile;

var FormularioUsuarioMobileMap = function () {
    this.CodigoFormulario = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.SomenteLeitura = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: false });
    this.PermissoesPersonalizadas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
};

var PermissaoPersonalizadaMobileMap = function () {
    this.CodigoPermissaoPersonalizada = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
};

var ModuloUsuarioMobileMap = function () {
    this.CodigoModulo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function buscarPaginasMobile() {

    executarReST("Pagina/BuscarModulosMobile", null, function (arg) {
        if (arg.Success) {
            _ListaModulosMobile = arg.Data;
            preencherHTMLTreeviewMobile();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ckbLiberarAcessoMobileClick(codigoModulo, codigoFormulario) {
    if ($("#ckbLiberarMobile_" + codigoFormulario).prop("checked")) {
        if ($("#tableFormulariosMobile_" + codigoModulo + " input.liberarAcesso:checkbox:not(:checked)").length == 0) {
            $("#ckb_liberarTodosMobile_" + codigoModulo).prop("checked", true);
        }
        $("#trPermissaoPersonalizadaMobile_" + codigoFormulario).slideDown();
    } else {
        $("#ckbSomenteLeituraMobile_" + codigoFormulario).prop("checked", false);
        $("#ckb_liberarTodosMobile_" + codigoModulo).prop("checked", false);
        $("#ckb_SomenteLeituraTodos_" + codigoModulo).prop("checked", false);
        $("#trPermissaoPersonalizadaMobile_" + codigoFormulario).slideUp();
    }
}

function ckbSomenteLeituraMobileClick(codigoModulo, codigoFormulario) {
    if ($("#ckbSomenteLeituraMobile_" + codigoFormulario).prop("checked")) {
        $("#ckbLiberarMobile_" + codigoFormulario).prop("checked", true);
        if ($("#tableFormulariosMobile_" + codigoModulo + " input.apenasLeitura:checkbox:not(:checked)").length == 0) {
            $("#ckb_liberarTodosMobile_" + codigoModulo).prop("checked", true);
            $("#ckb_SomenteLeituraTodos_" + codigoModulo).prop("checked", true);
            $("#trPermissaoPersonalizadaMobile_" + codigoFormulario).slideDown();
        }
    } else {
        $("#ckbSomenteLeituraMobile_" + codigoFormulario).prop("checked", false);
        $("#ckb_SomenteLeituraTodos_" + codigoModulo).prop("checked", false);
    }
}

function ckbTodosFormulariosMobileClick(codigoModulo) {
    if ($("#ckb_liberarTodosMobile_" + codigoModulo).prop("checked")) {
        $("#tableFormulariosMobile_" + codigoModulo + " input.liberarAcesso").prop("checked", true);
        $("#tableFormulariosMobile_" + codigoModulo + " input.permissaoPersonalizada").prop("checked", true);
        $("#tableFormulariosMobile_" + codigoModulo + " tr.trPermissaoPersonalizada").slideDown();
    } else {
        $("#tableFormulariosMobile_" + codigoModulo + " input").prop("checked", false);
        $("#tableFormulariosMobile_" + codigoModulo + " tr.trPermissaoPersonalizada").slideUp();
    }
}

function ckbSomenteLeituraTodosFormulariosMobileClick(codigoModulo) {
    if ($("#ckb_SomenteLeituraTodos_" + codigoModulo).prop("checked")) {
        $("#tableFormulariosMobile_" + codigoModulo + " input").prop("checked", true);
        $("#tableFormulariosMobile_" + codigoModulo + " input.permissaoPersonalizada").prop("checked", false);
        $("#tableFormulariosMobile_" + codigoModulo + " tr.trPermissaoPersonalizada").slideDown();
    } else {
        $("#tableFormulariosMobile_" + codigoModulo + " input.apenasLeitura").prop("checked", false);
    }
}

function ckbModuloLiberadoMobileClick(codigoModulo) {
    if ($("#ckbModuloLiberadoMobile_" + codigoModulo).prop("checked")) {
        $("#ulModuloMobile_" + codigoModulo).slideUp();
    } else {
        $("#ulModuloMobile_" + codigoModulo).slideDown();
        $("#ulModuloMobile_" + codigoModulo + " li").show();
    }
}

//*******MÉTODOS*******


function preencherHTMLTreeviewMobile() {
    var html = "<ul>";
    html += criarHTMLModulosMobile(_ListaModulosMobile);
    html += "</ul>";
    $("#divTreeViewMobile").html(html);
    loadTreeView();
}

function criarHTMLModulosMobile(modulos) {
    var html = "";
    $.each(modulos, function (i, modulo) {
        html += "<li class='d-flex align-items-center flex-wrap'>";
        html += "<span><i class='fal " + modulo.Icone + "'></i> " + modulo.Descricao + "</span> &nbsp; <div class='custom-control custom-checkbox'><input type='checkbox' class='custom-control-input' onclick='ckbModuloLiberadoMobileClick(" + modulo.CodigoModulo + ");' id='ckbModuloLiberadoMobile_" + modulo.CodigoModulo + "' name='checkbox-inline'>&nbsp; <label for='ckbModuloLiberadoMobile_" + modulo.CodigoModulo + "'  class='custom-control-label'> Liberar acesso total ao menu " + modulo.Descricao + "</label></div>";
        html += "<ul id='ulModuloMobile_" + modulo.CodigoModulo + "' class='w-100'>";
        if (modulo.Formularios.length > 0)
            html += criarHTMLFormulariosMobile(modulo);
        if (modulo.ModulosFilho.length > 0)
            html += criarHTMLModulosMobile(modulo.ModulosFilho);
        html += "</ul>";
        html += "</li>";
    });
    return html;
}

function criarHTMLFormulariosMobile(modulo) {
    var html = "";
    html += "<li>";
    html += "<table class='table table-bordered table-hover table-condensed table-striped' style='margin-left: -37px;position: relative;z-index: 1;margin-top: -5px;background: #FFF;' id='tableFormulariosMobile_" + modulo.CodigoModulo + "'>";

    html += "<thead>";
    html += "<tr>";
    html += "<td style='width:8%' title='Liberar Acesso'><input type='checkbox' style='margin-top:0px' onclick='ckbTodosFormulariosMobileClick(" + modulo.CodigoModulo + ");' id='ckb_liberarTodosMobile_" + modulo.CodigoModulo + "' />&nbsp;<label for='ckb_liberarTodosMobile_" + modulo.CodigoModulo + "'>Liberar Acesso</label></td>";
    html += "<td style='width:8%' title='Somente Leitura'><input type='checkbox' style='margin-top:0px'  onclick='ckbSomenteLeituraTodosFormulariosMobileClick(" + modulo.CodigoModulo + ");' id='ckb_SomenteLeituraTodos_" + modulo.CodigoModulo + "' />&nbsp;<label for='ckb_SomenteLeituraTodos_" + modulo.CodigoModulo + "'>Somente Leitura </label></td>";
    html += "<td><label>Formulário</label></td>";
    html += "</tr>";
    html += "</thead>";
    html += "<tbody>";
    $.each(modulo.Formularios, function (i, formulario) {
        html += "<tr><td><input type='checkbox' style='margin-top:0px' class='liberarAcesso' onclick='ckbLiberarAcessoMobileClick(" + modulo.CodigoModulo + ", " + formulario.CodigoFormulario + ");' id='ckbLiberarMobile_" + formulario.CodigoFormulario + "' /></td>";
        html += "<td><input type='checkbox' style='margin-top:0px' class='apenasLeitura' onclick='ckbSomenteLeituraMobileClick(" + modulo.CodigoModulo + ", " + formulario.CodigoFormulario + ");' id='ckbSomenteLeituraMobile_" + formulario.CodigoFormulario + "' /></td>";
        html += "<td><label>" + formulario.Descricao + "</label></td>";
        html += "</tr>";

        if (formulario.PermissoesPersonalizadas.length > 0) {
            html += "<tr id='trPermissaoPersonalizadaMobile_" + formulario.CodigoFormulario + "'  class='trPermissaoPersonalizada' style='display:none'><td colspan='3'><h4 style='padding:5px;'> " + formulario.Descricao + " Permissões Especiais </h4><table id='' class='table table-bordered table-hover table-condensed table-striped'>";
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

function buscarPermissoesFormulariosMobile() {
    _perfilAcessoMobile.FormulariosPerfilMobile.list = new Array();
    _perfilAcessoMobile.ModulosPerfilMobile.list = new Array();
    PreencherPermissoesMobile(_ListaModulosMobile);
}

function PreencherPermissoesMobile(modulos) {
    $.each(modulos, function (i, modulo) {
        if ($("#ckbModuloLiberadoMobile_" + modulo.CodigoModulo).prop("checked")) {
            var moduloMap = new ModuloUsuarioMobileMap();
            moduloMap.CodigoModulo.val = modulo.CodigoModulo;
            _perfilAcessoMobile.ModulosPerfilMobile.list.push(moduloMap);
        }

        $.each(modulo.Formularios, function (j, formulario) {
            if ($("#ckbLiberarMobile_" + formulario.CodigoFormulario).prop("checked")) {
                var formularioMap = new FormularioUsuarioMobileMap();
                formularioMap.CodigoFormulario.val = formulario.CodigoFormulario;
                formularioMap.SomenteLeitura.val = $("#ckbSomenteLeituraMobile_" + formulario.CodigoFormulario).prop("checked");
                formularioMap.PermissoesPersonalizadas.list = new Array();
                $.each(formulario.PermissoesPersonalizadas, function (p, permissaoPersonalizada) {
                    if ($("#ckbLiberarPermissao_" + formulario.CodigoFormulario + "_" + permissaoPersonalizada.CodigoPermissao).prop("checked")) {
                        var permissaoPersonalizadaMap = new PermissaoPersonalizadaMobileMap();
                        permissaoPersonalizadaMap.CodigoPermissaoPersonalizada.val = permissaoPersonalizada.CodigoPermissao;
                        formularioMap.PermissoesPersonalizadas.list.push(permissaoPersonalizadaMap);
                    }
                });
                _perfilAcessoMobile.FormulariosPerfilMobile.list.push(formularioMap);
            }
        });

        if (modulo.ModulosFilho.length > 0)
            PreencherPermissoesMobile(modulo.ModulosFilho); //faz a recursividade de modulos
    });
}

function setarPermissoesModulosFormulariosMobile() {
    $.each(_perfilAcessoMobile.ModulosPerfilMobile.list, function (i, modulo) {
        $("#ckbModuloLiberadoMobile_" + modulo.CodigoModulo.val).prop("checked", true);
        ckbModuloLiberadoMobileClick(modulo.CodigoModulo.val);
    });
    var listaModuloFormularioMobile = new Array();
    $.each(_perfilAcessoMobile.FormulariosPerfilMobile.list, function (i, formulario) {
        if ($("#ckbLiberarMobile_" + formulario.CodigoFormulario.val).closest("table").attr("id") != null) {
            $("#ckbLiberarMobile_" + formulario.CodigoFormulario.val).prop("checked", true);
            if (formulario.SomenteLeitura.val) {
                $("#ckbSomenteLeituraMobile_" + formulario.CodigoFormulario.val).prop("checked", true);
            }

            $("#trPermissaoPersonalizadaMobile_" + formulario.CodigoFormulario.val).show();

            var idModulo = parseInt($("#ckbLiberarMobile_" + formulario.CodigoFormulario.val).closest("table").attr("id").split("_")[1]);

            $.each(formulario.PermissoesPersonalizadas.list, function (p, permissaoPersonalizada) {
                $("#ckbLiberarPermissao_" + formulario.CodigoFormulario.val + "_" + permissaoPersonalizada.CodigoPermissao.val).prop("checked", true);
            });

            //utilizado apenas para disparar os eventos de cliente para deixar o layout visivel conforme as permissões
            if (listaModuloFormularioMobile.length > 0) {
                var novoModulo = true;
                $.each(listaModuloFormularioMobile, function (j, moduloFormulario) {
                    if (moduloFormulario.modulo == idModulo) {
                        novoModulo = false;
                        return false;
                    }
                });
                if (novoModulo)
                    listaModuloFormularioMobile.push({ formulario: formulario.CodigoFormulario.val, modulo: idModulo });
            } else
                listaModuloFormularioMobile.push({ formulario: formulario.CodigoFormulario.val, modulo: idModulo });
        }
    });

    $.each(listaModuloFormularioMobile, function (i, moduloFormulario) {
        ckbLiberarAcessoMobileClick(moduloFormulario.modulo, moduloFormulario.formulario);
        ckbSomenteLeituraMobileClick(moduloFormulario.modulo, moduloFormulario.formulario);
    });
}


function limparPermissoesModulosFormulariosMobile() {
    _perfilAcessoMobile.FormulariosPerfilMobile.list = new Array();
    _perfilAcessoMobile.ModulosPerfilMobile.list = new Array();
    preencherHTMLTreeviewMobile();
}