/// <reference path="../libs/jquery-2.1.1.js" />
/// <reference path="../plugin/bootstrap-timepicker/bootstrap-timepicker.min.js" />
/// <reference path="../bootstrap/bootstrap.js" />
/// <reference path="knockout-3.1.0.js" />
/// <reference path="Mensagem.js" />
/// <reference path="Rest.js" />
/// <reference path="libs/jquery.globalize.js" />
/// <reference path="libs/jquery.globalize.pt-BR.js" />
/// <reference path="Validacao.js" />
/// <reference path="../app.config.js" />

var _status = [
    { text: "Ativo", value: true },
    { text: "Inativo", value: false }
];

var _statusPesquisa = [
    { text: "Ativo", value: 1 },
    { text: "Inativo", value: 2 },
    { text: "Todos", value: 0 }
];

var typesKnockout = { string: "string", dynamic: "dynamic", report: "report", bool: "bool", decimal: "decimal", int: "int", date: "date", dateTime: "dateTime", time: "time", cpf: "cpf", cnpj: "cnpj", email: "email", cpfCnpj: "cpfCnpj", phone: "phone", selectMultiple: "selectMultiple", cep: "cep" }
var types = { map: "map", event: "event", entity: "entity", listEntity: "listEntity", local: "local", file: "file" }

function PropertyEntity(options) {
    var defaults = {
        val: ko.observable(""),
        requiredClass: ko.observable("input"),
        getType: typesKnockout.string,
        type: types.map,
        maxlength: 150,
        text: "",
        def: "",
        id: guid(),
        idGrid: "",
        configDecimal: { precision: 2, allowZero: false, allowNegative: false },
        configInt: { precision: 0, allowZero: false },
        eventClick: null,
        eventChange: null,
        required: false,
        visible: true,
        idFade: "",
        visibleFade: true,
        list: null,
        codEntity: 0,
        defCodEntity: 0,
        col: 0,
        idBtnSearch: "",
        idTab: "",
        enable: true,
        options: null,
        dateRangeInit: null,
        dateRangeLimit: null,
        dateInit: null,
        dateLimit: null,
        icon: "",
        cssClass: "",
        url: "",
        issue: 0,
        params: {},
        basicTable: null
    };
    var settings = $.extend({}, defaults, options);
    return settings;
}

function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

function guid() {
    return (S4() + S4() + S4() + S4() + S4() + S4() + S4() + S4());
}


function SalvarListEntity(kout) {
    var entidade = new Object();
    $.each(kout, function (i, prop) {
        if (prop != null && prop.type != null) {
            if (prop.type == types.map) {
                entidade[i] = new PropertyEntity({ val: prop.val(), getType: prop.getType });
            } else if (prop.type == types.entity) {
                entidade[i] = new PropertyEntity({ val: prop.val(), getType: prop.getType, codEntity: prop.codEntity(), type: types.entity });
            } else if (prop.type == types.listEntity) {
                entidade[i] = new PropertyEntity({ val: prop.val(), getType: prop.getType, list: new Array(), type: types.listEntity });
                $.each(prop.list, function (j, item) {
                    var itemEntity = new Object();
                    $.each(item, function (x, propItem) {
                        itemEntity[x] = propItem;
                    });
                    entidade[i].list.push(itemEntity);
                });
            }
        }
    });
    return entidade;
}

function AtualizarListEntity(kout, entidade) {
    $.each(kout, function (i, prop) {
        if (prop.type == types.map) {
            entidade[i].val = prop.val();
            entidade[i].getType = prop.getType;
        }
    });
}

function EditarListEntity(kout, data) {
    $.each(data, function (i, obj) {
        if (kout[i] != null) {
            if (kout[i].type == types.map) {
                kout[i].val(obj);
            }
        }
    });
}

function Salvar(kout, url, callback, sender, callbackRequiredField, callbackErro) {
    var tudoCerto = true;
    var emailValido = true;
    var cpfValido = true;
    var cnpjValido = true;
    var entidade = new Object();
    $.each(kout, function (i, prop) {
        var validarCampoObrigatorio = true;
        if (prop.type == types.map) {

            if (prop.getType != null) {
                if (prop.getType == typesKnockout.decimal && typeof (prop.val()) == "string") {
                    if (prop.val() == "") {
                        entidade[i] = null;
                    } else {
                        //entidade[i] = Globalize.parseFloat(prop.val());
                        entidade[i] = prop.val();
                    }
                } else if (prop.getType == typesKnockout.int && typeof (prop.val()) == "string") {
                    if (prop.val() == "") {
                        entidade[i] = null;
                    } else {
                        entidade[i] = Globalize.parseInt(prop.val());
                        //entidade[i] = prop.val();
                    }
                } else if (prop.getType == typesKnockout.email) {
                    if (ValidarEmail(prop.val())) {
                        entidade[i] = prop.val();
                        prop.requiredClass("input");
                    } else {
                        prop.requiredClass("input state-error");
                        validarCampoObrigatorio = false;
                        emailValido = false;
                    }
                } else if (prop.getType == typesKnockout.cpf) {
                    if (ValidarCPF(prop.val(), prop.required)) {
                        entidade[i] = prop.val();
                        prop.requiredClass("input");
                    } else {
                        prop.requiredClass("input state-error");
                        validarCampoObrigatorio = false;
                        cpfValido = false;
                    }
                } else if (prop.getType == typesKnockout.cnpj) {
                    if (ValidarCNPJ(prop.val(), prop.required)) {
                        entidade[i] = prop.val();
                        prop.requiredClass("input");
                    } else {
                        prop.requiredClass("input state-error");
                        validarCampoObrigatorio = false;
                        cnpjValido = false;
                    }
                } else if (prop.getType == typesKnockout.report) {
                    var tempReport = $.extend({}, prop.val());
                    if (tempReport.Grid != null) {
                        tempReport.PropriedadeAgrupa = tempReport.Grid.group.enable ? tempReport.Grid.group.propAgrupa : "";
                        tempReport.OrdemAgrupamento = tempReport.Grid.group.dirOrdena;

                        $.each(tempReport.Grid.header, function (i, head) {
                            if (i == tempReport.Grid.order[0].column) {
                                tempReport.PropriedadeOrdena = head.data;
                                return false;
                            }
                        });

                        tempReport.OrdemOrdenacao = tempReport.Grid.order[0].dir;
                        tempReport.Grid = JSON.stringify(tempReport.Grid);

                    }
                    entidade[i] = JSON.stringify(tempReport);
                } else if (prop.getType == typesKnockout.selectMultiple) {
                    entidade[i] = JSON.stringify(prop.val());
                } else {
                    entidade[i] = prop.val();
                }
            } else {
                entidade[i] = prop.val();
            }

            if (validarCampoObrigatorio) {
                if (!ValidarCampoObrigatorioMap(prop)) {
                    tudoCerto = false;
                }
            }

        } else if (prop.type == types.entity) {
            if (prop.val() == "" || prop.codEntity() == 0) {
                prop.codEntity(0);
                prop.val("");
            }
            entidade[i] = prop.codEntity();

            if (!ValidarCampoObrigatorioEntity(prop)) {
                tudoCerto = false;
            }
        } else if (prop.type == types.listEntity) {
            entidade[i] = JSON.stringify(recursiveListEntity(prop));
        }
    });
    if (tudoCerto) {
        if (emailValido) {
            if (cpfValido) {
                if (cnpjValido) {
                    var btn;
                    if (sender != null) {
                        btn = $("#" + sender.currentTarget.id);
                        btn.button('loading');
                    }
                    executarReST(url, entidade, function (e) {
                        callback(e);
                        if (sender != null) {
                            btn.button('reset');
                        }
                    }, callbackErro);
                } else {
                    exibirMensagem("atencao", "CNPJ Inválido", "Por favor, informe um CNPJ válido.");
                }
            } else {
                exibirMensagem("atencao", "CPF Inválido", "Por favor, informe um CPF válido.");
            }
        } else {
            exibirMensagem("atencao", "E-mail Inválido", "Por favor, informe um e-mail válido.");
        }
    } else {
        if (callbackRequiredField != null) {
            callbackRequiredField();
        } else {
            exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        }
    }
}

function recursiveListEntity(prop) {
    var listaTemp = new Array();
    $.each(prop.list, function (j, item) {
        var itemEntity = new Object();
        $.each(item, function (i, propItem) {
            if (propItem.type == types.map) {
                if (propItem.getType != null) {
                    if (propItem.getType == typesKnockout.decimal && typeof (propItem.val) == "string") {
                        if (propItem.val == "") {
                            itemEntity[i] = null;
                        } else {
                            itemEntity[i] = Globalize.parseFloat(propItem.val);
                        }
                    } else {
                        if (propItem.getType == typesKnockout.int && typeof (propItem.val) == "string") {
                            if (propItem.val == "") {
                                itemEntity[i] = null;
                            } else {
                                itemEntity[i] = Globalize.parseInt(propItem.val);
                            }
                        } else {
                            itemEntity[i] = propItem.val;
                        }
                    }
                } else {
                    itemEntity[i] = propItem.val;
                }
            } else if (propItem.type == types.listEntity) {
                itemEntity[i] = recursiveListEntity(propItem);
            } else if (propItem.type == types.entity) {
                itemEntity[i] = new Object();
                itemEntity[i].Descricao = propItem.val;
                itemEntity[i].Codigo = propItem.codEntity;
            }
        });
        listaTemp.push(itemEntity);
    });
    return listaTemp;
}

function BuscarPorCodigo(kout, url, callback, callbackErro) {
    var dados = new Object();
    dados.Codigo = kout.Codigo.val();
    executarReST(url, dados, function (e) {
        if (e.Success) {
            PreencherObjetoKnout(kout, e);
            if (callback != null) {
                callback(e);
            }
        } else {
            if (callbackErro != null) {
                callbackErro(e);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
            }
        }
    });
}

function PreencherObjetoKnout(kout, e) {

    $.each(e.Data, function (i, obj) {
        if (kout[i] != null) {
            if (kout[i].requiredClass != null) {
                if ($("#" + kout[i].id).is("select"))
                    kout[i].requiredClass("select");
                else
                    kout[i].requiredClass("input");
            }
            if (kout[i].type == types.map) {
                if (kout[i].getType != null) {
                    if (kout[i].getType == typesKnockout.decimal) {
                        if (obj != null) {
                            kout[i].val(Globalize.format(obj, "n2"));
                        } else {
                            kout[i].val("");
                        }
                    } else {
                        if (kout[i].getType == typesKnockout.int) {
                            if (obj != null) {
                                kout[i].val(obj);
                            } else {
                                kout[i].val("");
                            }
                        } else {
                            kout[i].val(obj);

                            if ((kout[i].getType == typesKnockout.date || kout[i].getType == typesKnockout.dateTime) && (kout[i].dateRangeInit != null || kout[i].dateRangeLimit != null))
                                $("#" + kout[i].id).trigger("change");
                            else if (kout[i].getType == typesKnockout.selectMultiple)
                                $("#" + kout[i].id).selectpicker('render');
                        }
                    }
                } else {
                    kout[i].val(obj);
                }
            } else if (kout[i].type == types.entity) {
                if (obj != null) {
                    kout[i].codEntity(obj.Codigo);
                    kout[i].val(obj.Descricao);
                } else {
                    kout[i].codEntity(kout[i].defCodEntity);
                    kout[i].val(kout[i].def);
                }
            } else if (kout[i].type == types.listEntity) {
                kout[i].list = recursiveObjetoRetorno(obj);
            }
        }

    });
}

function recursiveObjetoRetorno(obj) {
    var lista = new Array();
    $.each(obj, function (p, objeto) {
        var entidade = new Object();
        $.each(objeto, function (i, prop) {
            if (prop != null) {
                entidade[i] = PropertyEntity();
                if (typeof (prop) == "object") {
                    if (prop.tipo == null) {
                        if (Object.prototype.toString.call(prop) === "[object Array]") {
                            entidade[i].list = recursiveObjetoRetorno(prop);
                            entidade[i].type = types.listEntity;
                        } else if (Object.prototype.toString.call(prop) === "[object Object]") {
                            entidade[i].val = prop.Descricao;
                            entidade[i].codEntity = prop.Codigo;
                            entidade[i].type = types.entity;
                        }
                    } else if (prop.tipo == typesKnockout.decimal) {
                        var precision = prop.configDecimal ? ("n" + prop.configDecimal.precision) : "n2";
                        entidade[i].val = Globalize.format(prop.val, precision);
                        entidade[i].getType = typesKnockout.decimal;
                    }
                } else {
                    if (typeof (prop) == "number") {
                        if (prop.toString().split('.').length > 1) {
                            entidade[i].getType = typesKnockout.decimal;
                            entidade[i].val = Globalize.format(prop, "n2");
                        } else {
                            entidade[i].getType = typesKnockout.int;
                            entidade[i].val = prop;
                        }
                        entidade[i].def = 0;
                    } else {
                        entidade[i].val = prop;
                    }
                }
            }
        });
        lista.push(entidade);
    });
    return lista;
}

function ExcluirPorCodigo(kout, url, callback, sender, callbackErro) {
    var dados = new Object();
    dados.Codigo = kout.Codigo.val();
    if (sender != null) {
        var btn = $("#" + sender.currentTarget.id);
        btn.button('loading');
    }
    executarReST(url, dados, function (e) {
        callback(e);
        if (sender != null) {
            btn.button('reset');
        }
    }, callbackErro);
}



function LimparCampos(kout) {
    $.each(kout, function (i, prop) {
        if (typeof prop == "object") {
            if (prop.type == types.map) {
                prop.val(prop.def);

                if (((prop.getType == typesKnockout.date || prop.getType == typesKnockout.dateTime) && (prop.dateRangeInit != null || prop.dateRangeLimit != null)))
                    $("#" + prop.id).trigger("change");
                else if (prop.getType == typesKnockout.selectMultiple)
                    $("#" + prop.id).selectpicker('render');

            } else if (prop.type == types.entity) {
                LimparCampoEntity(prop)
            } else if (prop.type == types.listEntity) {
                LimparCampoEntity(prop)
                prop.list = new Array();
            }

            if ($("#" + prop.id).is("select"))
                prop.requiredClass("select");
            else
                prop.requiredClass("input");
        }
    });
}

function LimparCampoEntity(prop) {
    prop.codEntity(prop.defCodEntity);
    prop.val(prop.def);
}

function ValidarCamposObrigatorios(kout) {
    var tudoCerto = true;
    $.each(kout, function (i, prop) {
        if (prop.type == types.map) {
            if (!ValidarCampoObrigatorioMap(prop)) {
                tudoCerto = false;
            }
        } else if (prop.type == types.entity || prop.type == types.listEntity) {
            if (!ValidarCampoObrigatorioEntity(prop)) {
                tudoCerto = false;
            }
        }
    });
    return tudoCerto;
}


function ValidarCampoObrigatorioEntity(prop) {
    var valido = true;
    if (prop.required) {
        if (prop.val() == "" || prop.codEntity() == 0) {
            $("#" + prop.id).keyup(function () {
                if ($("#" + prop.id).val().trim() == "") {
                    prop.requiredClass("input state-error");
                } else {
                    prop.requiredClass("input");
                }
            })
            prop.val(prop.def);
            prop.codEntity(prop.defCodEntity);
            prop.requiredClass("input state-error");
            valido = false;
        } else {
            prop.requiredClass("input");
        }
    } else if (prop.requiredClass != null) {
        prop.requiredClass("input");
    }
    return valido;
}

function ValidarCampoObrigatorioMap(prop) {
    var valido = true;
    if (prop.required === true || (typeof prop.required === "function" && prop.required() === true)) {
        if ($("#" + prop.id).is("select")) {
            if (prop.val() === null || prop.val() === undefined || prop.val() === "") {
                prop.requiredClass("select state-error");
                valido = false;
            } else {
                prop.requiredClass("select");
            }
        } else {
            if (prop.val() === "" || prop.val() === null) {
                $("#" + prop.id).keyup(function () {
                    if ($("#" + prop.id).val().trim() == "") {
                        prop.requiredClass("input state-error");
                    } else {
                        prop.requiredClass("input");
                    }
                });
                prop.requiredClass("input state-error");
                valido = false;
            } else {
                prop.requiredClass("input");
            }
        }
    }
    else if (prop.requiredClass != null) {
        if ($("#" + prop.id).is("select"))
            prop.requiredClass("select");
        else
            prop.requiredClass("input");
    }
    return valido;
}

function KoBindings(view, idElemento, validaSomenteLeitura, idBtnFocusEnterKeyPress) {

    ko.cleanNode($("#" + idElemento)[0]);
    ko.applyBindings(view, $("#" + idElemento)[0]);

    if (idBtnFocusEnterKeyPress != null) {
        $('#' + idElemento).keypress(function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode == 13) {
                $('#' + idBtnFocusEnterKeyPress).trigger('click');
            }
        });
        if (validaSomenteLeitura == null)
            validaSomenteLeitura = false;
    }

    if (validaSomenteLeitura == null)
        validaSomenteLeitura = true;

    $.each(view, function (i, prop) {

        switch (prop.getType) {
            case typesKnockout.int:
                $("#" + prop.id).maskMoney(prop.configInt);
                break;
            case typesKnockout.decimal:
                $("#" + prop.id).maskMoney(prop.configDecimal);
                break;
            case typesKnockout.date:
                ConfigurarCampoDate(prop);
                break;
            case typesKnockout.dateTime:
                ConfigurarCampoDateTime(prop);
                break;
            case typesKnockout.time:
                ConfigurarCampoTime(prop);
                break;
            case typesKnockout.email:
                $("#" + prop.id).addClass("lowercase");
                break;
            case typesKnockout.cpf:
                $("#" + prop.id).mask("999.999.999-99");
                break;
            case typesKnockout.cnpj:
                $("#" + prop.id).mask("99.999.999/9999-99");
                break;
            case typesKnockout.cpfCnpj:
                $("#" + prop.id).mask("99999999999?999");
                break;
            case typesKnockout.phone:
                $("#" + prop.id).mask("(99) 9999-9999?9");
                $("#" + prop.id).on('change', function () { SetarMascaraTelefone(this); });
                break;
            case typesKnockout.selectMultiple:
                SetarSelectMultiple(prop);
                break;
            case typesKnockout.cep:
                $("#" + prop.id).mask("99.999-999");
                break;
            default:
                break;
        }

        if ($("#" + prop.id).is("select"))
            prop.requiredClass("select");

        AdicionarAjudaCampo(prop);

        if (validaSomenteLeitura && _FormularioSomenteLeitura) {
            $("#" + prop.id).prop("disabled", true);
            if (prop.idBtnSearch != "") {
                $("#" + prop.idBtnSearch).prop("disabled", true);
            }
        }
    });
}

function AdicionarAjudaCampo(prop) {
    if (prop.issue > 0) {
        if ($("#" + prop.id).is(":checkbox")) {
            var htmlCheck = "<a href='javascript:void(0);' title='Entenda a opção' onclick='iniciarAjuda(" + prop.issue + ");' tabindex='-1'>&nbsp;"
            htmlCheck += "<i class='fa fa-question-circle' style='border-style:none !important; background: none !important; margin-left: 5px; margin-top: 2px; z-index:2; left:auto !important; display: inline !important'></i></a>";
            var textoCheck = prop.text != null ? prop.text : prop.text() != null ? prop.text() : "";
            $("#" + prop.id).parent().find("b").html(textoCheck + htmlCheck);
        } else if ($("#" + prop.id).is(":button")) {
            if ($("#" + prop.id).hasClass("btnPesquisarFiltroPesquisa")) {
                $("#" + prop.id).wrap('<div class="btn-group" style="float: right;"></div>');
                $("#" + prop.id).before('<button class="' + $("#" + prop.id).attr("class") + '" onclick="iniciarAjuda(' + prop.issue + ');" style="margin-left: -10px !important; padding-left: 4px; padding-right: 4px; border: 0; background: none; box-shadow: none;" tabindex="-1"><i class="fa fa-question-circle"></i></button>');
            }
        } else if (prop.type == types.event && $("#" + prop.idBtnSearch).is(":button")) {
            $("#" + prop.idBtnSearch).wrap('<div class="btn-group"></div>');
            $("#" + prop.idBtnSearch).after('<button class="' + $("#" + prop.idBtnSearch).attr("class") + '" onclick="iniciarAjuda(' + prop.issue + ');" style="padding-left: 4px; padding-right: 6px; padding-top: 9px; border: 0; background: none; box-shadow: none; color: #3276b1;" tabindex="-1"><i class="fa fa-question-circle" style="font-size: 18px;"></i></button>');
        } else {
            $("#" + prop.id).parent().parent().find("b").after("<a href='javascript:void(0);' title='Entenda a opção' onclick='iniciarAjuda(" + prop.issue + ");' tabindex='-1'>&nbsp;<i class='fa fa-question fa-x2'></i></a>");
        }
    }
}

function ConfigurarCampoDateTime(prop) {
    //configurações de data na url: http://www.jqueryrain.com/?Qoeemzn0 

    $("#" + prop.id).datetimepicker({
        locale: 'pt-br',
        useCurrent: false,
        //inline: true,
        sideBySide: true,
        format: 'DD/MM/YYYY HH:mm'
    });

    if (prop.dateRangeInit != null) {
        $("#" + prop.id).on("dp.change", function (e) {
            $("#" + prop.dateRangeInit.id).data("DateTimePicker").maxDate(e.date);
        });
    } else {
        if (prop.dateRangeLimit != null) {
            $("#" + prop.id).on("dp.change", function (e) {
                $("#" + prop.dateRangeLimit.id).data("DateTimePicker").minDate(e.date);
            });
        }
    }

    if (prop.dateInit != null) {
        $("#" + prop.id).data("DateTimePicker").minDate(prop.dateInit.val());
        $("#" + prop.dateInit.id).on("dp.change", function (e) {
            if ($("#" + prop.id).data("DateTimePicker") != null) {
                $("#" + prop.id).data("DateTimePicker").minDate(e.date);
            }
        });
    }

    if (prop.dateLimit != null) {
        var maxIni = "";
        if (prop.dateLimit.val() != "")
            maxIni = new Date(parseInt(prop.dateLimit.val().split('/')[2]), parseInt(prop.dateLimit.val().split('/')[1]) - 1, parseInt(prop.dateLimit.val().split('/')[0]), 23, 59, 59, 0);
        $("#" + prop.id).data("DateTimePicker").maxDate(maxIni);
        $("#" + prop.dateLimit.id).on("dp.change", function (e) {
            var max = "";
            if (e.date != null) {
                var strdata = $("#" + prop.dateLimit.id).val();
                max = new Date(parseInt(strdata.split('/')[2]), parseInt(strdata.split('/')[1]) - 1, parseInt(strdata.split('/')[0]), 23, 59, 59, 0);
            }

            if ($("#" + prop.id).data("DateTimePicker") != null) {
                $("#" + prop.id).data("DateTimePicker").maxDate(max);
            }
        });
    }


    $("#" + prop.id).mask("99/99/9999 99:99");
}

function ConfigurarCampoTime(prop) {

    $("#" + prop.id).datetimepicker({
        locale: 'pt-br',
        useCurrent: false,
        format: 'HH:mm'
    });

    $("#" + prop.id).mask("99:99");
}

function ConfigurarCampoDate(prop) {
    //configurações de data na url: http://www.jqueryrain.com/?Qoeemzn0 

    $("#" + prop.id).datetimepicker({
        locale: 'pt-br',
        useCurrent: false,
        format: 'DD/MM/YYYY'
    });

    if (prop.dateRangeInit != null) {
        $("#" + prop.id).on("dp.change", function (e) {
            $("#" + prop.dateRangeInit.id).data("DateTimePicker").maxDate(e.date);
        });
    } else {
        if (prop.dateRangeLimit != null) {
            $("#" + prop.id).on("dp.change", function (e) {
                $("#" + prop.dateRangeLimit.id).data("DateTimePicker").minDate(e.date);
            });
        }
    }

    if (prop.dateInit != null) {
        if (prop.dateInit.val() != "")
            $("#" + prop.id).data("DateTimePicker").minDate(prop.dateInit.val());
        $("#" + prop.dateInit.id).on("dp.change", function (e) {
            if ($("#" + prop.id).data("DateTimePicker") != null) {
                $("#" + prop.id).data("DateTimePicker").minDate(e.date);
            }
        });
    }

    if (prop.dateLimit != null) {
        if (prop.dateLimit.val() != "")
            $("#" + prop.id).data("DateTimePicker").maxDate(prop.dateLimit.val());
        $("#" + prop.dateLimit.id).on("dp.change", function (e) {
            if ($("#" + prop.id).data("DateTimePicker") != null) {
                $("#" + prop.id).data("DateTimePicker").maxDate(e.date);
            }
        });
    }

    $("#" + prop.id).mask("99/99/9999");
}

function SetarSelectMultiple(prop) {
    //configurações do selectpicker na url: https://silviomoreto.github.io/bootstrap-select/

    if (prop.url != null && prop.url.trim() != "") {
        executarReST(prop.url, prop.params, function (r) {
            if (r.Success) {
                prop.options(r.Data);
                $("#" + prop.id).selectpicker();
            }
        });
    } else {
        $("#" + prop.id).selectpicker();
    }
}

function SetarMascaraTelefone(elem) {
    var phone, element;
    element = $(elem);
    element.unmask();
    phone = element.val().replace(/\D/g, '');
    if (phone.length > 10) {
        element.mask("(99) 99999-999?9");
    } else {
        element.mask("(99) 9999-9999?9");
    }
}

function ValidarCamposEntity(knout) {
    $.each(knout, function (i, prop) {
        if (prop.type == types.entity || prop.type == types.listEntity)
            if (prop.val() == "" || prop.codEntity() == 0) {
                prop.val(prop.def);
                prop.codEntity(prop.defCodEntity);
            }
    });
}

function RetornarObjetoPesquisa(knout) {
    var entidade = new Object();

    if (knout != null) {
        $.each(knout, function (i, prop) {
            if (prop.type == types.map) {
                if (prop.getType != null) {
                    if (prop.getType == typesKnockout.decimal && typeof (prop.val()) == "string") {
                        if (prop.val() == "") {
                            entidade[i] = null;
                        } else {
                            entidade[i] = prop.val();
                            //entidade[i] = Globalize.parseFloat(prop.val());
                        }
                    } else if (prop.getType == typesKnockout.int && typeof (prop.val()) == "string") {
                        if (prop.val() == "") {
                            entidade[i] = null;
                        } else {
                            entidade[i] = Globalize.parseInt(prop.val());
                        }
                    } else if (prop.getType == typesKnockout.selectMultiple) {
                        entidade[i] = JSON.stringify(prop.val());
                    } else {
                        entidade[i] = prop.val();
                    }
                } else {
                    entidade[i] = prop.val();
                }
            } else if (prop.type == types.entity) {
                if (prop.val() == "" || prop.codEntity() == 0) {
                    prop.codEntity(0);
                    prop.val("");
                }
                entidade[i] = prop.codEntity();
            }
        });
    }

    return entidade;
}