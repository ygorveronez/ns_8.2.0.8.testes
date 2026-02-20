/// <reference path="../libs/jquery-2.1.1.js" />
/// <reference path="../bootstrap/bootstrap.js" />
/// <reference path="../knockout/knockout-3.3.0.js" />
/// <reference path="Mensagem.js" />
/// <reference path="Rest.js" />
/// <reference path="../libs/jquery.globalize.js" />
/// <reference path="../libs/jquery.globalize.pt-BR.js" />
/// <reference path="Validacao.js" />
/// <reference path="../app.config.js" />

var _status = [
    { text: Localization.Resources.Enumeradores.Situacao.Ativo, value: true },
    { text: Localization.Resources.Enumeradores.Situacao.Inativo, value: false }
];

var _statusPesquisa = [
    { text: Localization.Resources.Enumeradores.Situacao.Ativo, value: 1 },
    { text: Localization.Resources.Enumeradores.Situacao.Inativo, value: 2 },
    { text: Localization.Resources.Gerais.Geral.Todos, value: 0 }
];

var _TiposDespesas = [

    { text: Localization.Resources.Enumeradores.TiposDespesas.DespesaGeral, value: 1 },
    { text: Localization.Resources.Enumeradores.TiposDespesas.DespesacomAlimentação, value: 2 },
];

var _TiposDespesasPesquisa = [

    { text: Localization.Resources.Enumeradores.TiposDespesas.DespesaGeral, value: 1 },
    { text: Localization.Resources.Enumeradores.TiposDespesas.DespesacomAlimentação, value: 2 },
    { text: Localization.Resources.Gerais.Geral.Todas, value: 0 }
];
var _statusFem = [
    { text: Localization.Resources.Enumeradores.Situacao.Ativa, value: true },
    { text: Localization.Resources.Enumeradores.Situacao.Inativa, value: false }
];

var _statusFemPesquisa = [
    { text: Localization.Resources.Enumeradores.Situacao.Ativa, value: 1 },
    { text: Localization.Resources.Enumeradores.Situacao.Inativa, value: 2 },
    { text: Localization.Resources.Gerais.Geral.Todas, value: 0 }
];

var typesKnockout = { string: "string", dynamic: "dynamic", report: "report", bool: "bool", decimal: "decimal", int: "int", date: "date", month: "month", year: "year", dateTime: "dateTime", time: "time", timeSec: "timeSec", cpf: "cpf", cnpj: "cnpj", email: "email", multiplesEmails: "multiplesEmails", cpfCnpj: "cpfCnpj", placa: "placa", phone: "phone", selectMultiple: "selectMultiple", cep: "cep", basicTable: "basicTable", raizCnpj: "raizCnpj", mask: "mask" }
var types = { map: "map", event: "event", entity: "entity", multiplesEntities: "multiplesEntities", listEntity: "listEntity", local: "local", file: "file" }

function PropertyEntity(options) {
    var defaults = {
        val: ko.observable(""),
        requiredClass: ko.observable("form-control"),
        getType: typesKnockout.string,
        type: types.map,
        maxlength: 100,
        text: "",
        def: "",
        id: guid(),
        idGrid: "",
        eventClick: null,
        eventChange: null,
        required: false,
        visible: true,
        idFade: "",
        visibleFade: true,
        list: null,
        codEntity: 0,
        defCodEntity: 0,
        entityDescription: ko.observable(""),
        multiplesEntities: ko.observableArray([]),
        multiplesEntitiesConfig: {},
        col: 0,
        idBtnSearch: "",
        idTab: "",
        enable: true,
        options: null,
        dateRangeInit: null,
        dateRangeLimit: null,
        icon: "",
        cssClass: "",
        url: "",
        issue: 0,
        params: {},
        maskParams: null,
        basicTable: null,
        cleanEntityCallback: undefined,
        selectMultipleTitle: Localization.Resources.Gerais.Geral.Todos,
        textBtnSearch: Localization.Resources.Gerais.Geral.Buscar,
        get$: function () {
            return settings.id != "" ? $("#" + settings.id) : null;
        },
        tipoFiltroPesquisa: null,
        callbackRetornoPesquisa: undefined
    };

    var settings = $.extend({}, defaults, options);

    settings.configDecimal = ConfigDecimal(options ? options.configDecimal : undefined);
    settings.configInt = ConfigInt(options ? options.configInt : undefined);
    settings.multiplesEntitiesConfig = $.extend({}, { propDescricao: "Descricao", propCodigo: "Codigo" }, settings.multiplesEntitiesConfig);

    return settings;
}

function ConfigDecimal(options) {
    var defaults = {
        allowNegative: false,
        allowZero: false,
        decimal: ',',
        precision: 2,
        thousands: '.'
    };

    if (ObterCulturaConfigurada() != "pt-BR") {
        defaults.decimal = '.';
        defaults.thousands = ',';
    }

    if (!options)
        return defaults;

    return $.extend({}, defaults, options);
}

function ConfigInt(options) {
    var defaults = {
        allowNegative: false,
        allowZero: false,
        precision: 0,
        thousands: '.'
    };

    if (ObterCulturaConfigurada() != "pt-BR")
        defaults.thousands = ',';

    if (!options)
        return defaults;

    return $.extend({}, defaults, options);
}

function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

function guid() {
    return ('a' + S4() + S4() + S4() + S4() + S4() + S4() + S4() + S4());
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
        } else if (prop.type == types.entity) {
            entidade[i].val = prop.val();
            entidade[i].codEntity = prop.codEntity();
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

function PreencherEditarListEntity(kout, data) {
    $.each(data, function (i, obj) {
        if (kout[i] != null) {
            if (kout[i].type == types.map) {
                kout[i].val(obj.val);
            } else if (kout[i].type == types.entity) {
                kout[i].val(obj.val);
                kout[i].codEntity(obj.codEntity);
            }
        }
    });
}

function Salvar(kout, url, callback, sender, callbackRequiredField, callbackErro) {
    var tudoCerto = true;
    var emailValido = true;
    var cpfValido = true;
    var cnpjValido = true;
    var listaEmailValida = true;
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
                }
                else if (prop.getType == typesKnockout.int && typeof (prop.val()) == "string") {
                    if (prop.val() == "") {
                        entidade[i] = null;
                    } else {
                        entidade[i] = Globalize.parseInt(prop.val());
                        //entidade[i] = prop.val();
                    }
                }
                else if (prop.getType == typesKnockout.email) {
                    if (ValidarEmail(prop.val())) {
                        entidade[i] = prop.val();
                        prop.requiredClass("form-control");
                    } else {
                        prop.requiredClass("form-control is-invalid");
                        validarCampoObrigatorio = false;
                        emailValido = false;
                    }
                }
                else if (prop.getType == typesKnockout.multiplesEmails) {
                    if (ValidarMultiplosEmails(prop.val())) {
                        entidade[i] = prop.val();
                        prop.requiredClass("form-control");
                    } else {
                        prop.requiredClass("form-control is-invalid");
                        validarCampoObrigatorio = false;
                        listaEmailValida = false;
                    }
                }
                else if (prop.getType == typesKnockout.cpf) {
                    if (_CONFIGURACAO_TMS.Pais == EnumPaises.Brasil) {
                        if (ValidarCPF(prop.val(), prop.required)) {
                            entidade[i] = prop.val();
                            prop.requiredClass("form-control");
                        } else {
                            prop.requiredClass("form-control is-invalid");
                            validarCampoObrigatorio = false;
                            cpfValido = false;
                        }
                    } else {
                        entidade[i] = prop.val();
                        prop.requiredClass("form-control");
                    }
                }
                else if (prop.getType == typesKnockout.cnpj) {
                    if (_CONFIGURACAO_TMS.Pais == EnumPaises.Brasil) {
                        if (ValidarCNPJ(prop.val(), prop.required)) {
                            entidade[i] = prop.val();
                            prop.requiredClass("form-control");
                        } else {
                            prop.requiredClass("form-control is-invalid");
                            validarCampoObrigatorio = false;
                            cnpjValido = false;
                        }
                    } else {
                        entidade[i] = prop.val();
                        prop.requiredClass("form-control");
                    }
                }
                else if (prop.getType == typesKnockout.basicTable) {
                    var data = prop.basicTable != null ? prop.basicTable.BuscarRegistros() : [];
                    entidade[i] = JSON.stringify(data);
                }
                else if (prop.getType == typesKnockout.report) {
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
                }
                else if (prop.getType == typesKnockout.selectMultiple) {
                    entidade[i] = JSON.stringify(prop.val());
                }
                else {
                    entidade[i] = prop.val();
                }
            }
            else {
                entidade[i] = prop.val();
            }

            if (validarCampoObrigatorio && (prop.getType == null || prop.getType != typesKnockout.dynamic)) {
                if (!ValidarCampoObrigatorioMap(prop)) {
                    tudoCerto = false;
                }
            }

        }
        else if (prop.type == types.entity) {
            if ((prop.val() == "" || prop.codEntity() == 0) && !(prop.val() == "0" && prop.codEntity() > 0)) {
                prop.codEntity(0);
                prop.val("");
            }
            entidade[i] = prop.codEntity();

            if (!ValidarCampoObrigatorioEntity(prop))
                tudoCerto = false;
        }
        else if (prop.type == types.multiplesEntities) {
            entidade[i] = JSON.stringify(recursiveMultiplesEntities(prop));

            if (!ValidarCampoObrigatorioEntity(prop))
                tudoCerto = false;
        }
        else if (prop.type == types.listEntity) {
            entidade[i] = JSON.stringify(recursiveListEntity(prop));
        }
    });

    if (tudoCerto) {
        if (emailValido) {
            if (listaEmailValida) {
                if (cpfValido) {
                    if (cnpjValido) {
                        var btn;
                        if (sender != null && !string.IsNullOrWhiteSpace(sender.currentTarget.id)) {
                            btn = $("#" + sender.currentTarget.id);
                            btn.button('loading');
                        }
                        executarReST(url, entidade, function (e) {
                            callback(e);
                            if (sender != null && btn != null) {
                                btn.button('reset');
                            }
                        }, callbackErro);
                    }
                    else {
                        exibirMensagem("atencao", "CNPJ Inválido", "Por favor, informe um CNPJ válido.");
                    }
                }
                else {
                    exibirMensagem("atencao", "CPF Inválido", "Por favor, informe um CPF válido.");
                }
            }
            else {
                exibirMensagem("atencao", "E-mail Inválido", "Por favor, informe um ou mais e-mails válidos separados por ponto e vírgula ( ; ).");
            }
        }
        else {
            exibirMensagem("atencao", "E-mail Inválido", "Por favor, informe um e-mail válido.");
        }
    }
    else {
        if (callbackRequiredField != null) {
            callbackRequiredField();
        }
        else {
            exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    }
}

function recursiveMultiplesEntities(prop) {
    var data = prop.multiplesEntities();
    var formatted = [];

    data.map(function (d) {
        formatted.push(d[prop.multiplesEntitiesConfig.propCodigo]);
    });

    return formatted;
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

function BuscarPorCodigo(kout, url, callback, callbackErro, exibirLoading) {
    if (exibirLoading == null)
        exibirLoading = true;

    var dados = new Object();
    dados.Codigo = kout.Codigo.val();
    executarReST(url, dados, function (e) {
        if (e.Success) {
            if (e.Data) {
                PreencherObjetoKnout(kout, e);
                if (callback != null)
                    callback(e);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
        } else {
            if (callbackErro != null) {
                callbackErro(e);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
            }
        }
    }, null, exibirLoading);
}

function SetarEnableCampo(prop, enable) {
    if (prop.enable != null) {
        if (typeof prop.enable == "function")
            prop.enable(enable);
        else
            prop.enable = enable;
    }
}

function SetarEnableCamposKnockout(kout, enable) {
    $.each(kout, function (i, prop) {
        SetarEnableCampo(prop, enable);
    });
}

function PreencherObjetoKnout(kout, e) {
    $.each(e.Data, function (i, obj) {
        if (kout[i] != null) {
            if (kout[i].requiredClass != null) {
                //if ($("#" + kout[i].id).is("select"))
                //    kout[i].requiredClass("select");
                //else
                kout[i].requiredClass("form-control");
            }

            if (kout[i].type == types.map) {
                if (kout[i].getType != null) {
                    if (kout[i].getType == typesKnockout.decimal) {
                        if (obj != null)
                            kout[i].val(Globalize.format(obj, "n2"));
                        else
                            kout[i].val("");
                    }
                    else if (kout[i].getType == typesKnockout.int) {
                        if (obj != null)
                            kout[i].val(obj);
                        else
                            kout[i].val("");
                    }
                    else if (kout[i].getType == typesKnockout.basicTable) {
                        var data = [];

                        if (obj != null)
                            data = obj;

                        kout[i].basicTable.CarregarGrid(data);
                    }
                    else {
                        kout[i].val(obj);

                        if (kout[i].getType == typesKnockout.date || kout[i].getType == typesKnockout.dateTime || kout[i].getType == typesKnockout.month || kout[i].getType == typesKnockout.time || kout[i].getType == typesKnockout.timeSec || kout[i].getType == typesKnockout.year) {
                            if (kout[i].updateValue instanceof Function)
                                kout[i].updateValue();

                            if ((kout[i].getType == typesKnockout.date || kout[i].getType == typesKnockout.dateTime || kout[i].getType == typesKnockout.month) && (kout[i].dateRangeInit != null || kout[i].dateRangeLimit != null || kout[i].year != null))
                                $("#" + kout[i].id).trigger("change");
                        }
                        else if (kout[i].getType == typesKnockout.selectMultiple)
                            $("#" + kout[i].id).selectpicker('val', obj);
                    }
                }
                else
                    kout[i].val(obj);
            }
            else if (kout[i].type == types.entity) {
                if (obj != null) {
                    kout[i].val(obj.Descricao);
                    kout[i].entityDescription(obj.Descricao);
                    kout[i].codEntity(obj.Codigo);
                }
                else {
                    var valorPadrao = ObterValorPadrao(kout[i]);

                    kout[i].val(valorPadrao);
                    kout[i].entityDescription(valorPadrao);
                    kout[i].codEntity(kout[i].defCodEntity);
                }
            }
            else if (kout[i].type == types.multiplesEntities)
                kout[i].multiplesEntities(obj);
            else if (kout[i].type == types.listEntity)
                kout[i].list = recursiveObjetoRetorno(obj);
        }

    });
}

function PreencherObjetoKnoutLegenda(koutLegenda, configuracoesLegendas) {
    for (var i = 0; i < configuracoesLegendas.length; i++) {
        var configuracaoLegenda = configuracoesLegendas[i];
        var koutLegendaPropriedade = koutLegenda[configuracaoLegenda.NomePropriedade];

        if (!koutLegendaPropriedade)
            continue;

        PreencherKnoutLegenda(koutLegendaPropriedade, configuracaoLegenda);
    }
}

function PreencherKnoutLegenda(koutLegendaPropriedade, configuracaoLegenda) {
    if (!configuracaoLegenda)
        return;

    if (configuracaoLegenda.Descricao) {
        if (koutLegendaPropriedade.text instanceof Function)
            koutLegendaPropriedade.text(configuracaoLegenda.Descricao);
        else
            koutLegendaPropriedade.text = configuracaoLegenda.Descricao;
    }

    if (koutLegendaPropriedade.visible instanceof Function)
        koutLegendaPropriedade.visible(configuracaoLegenda.Exibir);
    else
        koutLegendaPropriedade.visible = configuracaoLegenda.Exibir;
}

function PreencherObjetoKnoutLegendaTotalizadores(koutLegenda, totalizadores) {
    for (var nomePropriedade in koutLegenda) {
        var propriedade = koutLegenda[nomePropriedade];

        if (!(propriedade.totalItens instanceof Function))
            continue;

        var totalItens = Boolean(totalizadores) ? totalizadores[nomePropriedade] : undefined;

        propriedade.totalItens(totalItens != undefined ? " [" + totalItens + "]" : "");
    }
}

function recursiveObjetoRetorno(obj) {
    var lista = new Array();
    if (obj != null) {
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
    }
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
    if (!kout)
        return;

    $.each(kout, function (i, prop) {
        LimparCampo(prop);
    });
}

function BloquearCampos(kout) {
    if (!kout)
        return;

    $.each(kout, function (i, prop) {
        if ($.isFunction(prop.enable))
            prop.enable(false)
    });
}

function DesbloquearCampos(kout) {
    if (!kout)
        return;

    $.each(kout, function (i, prop) {
        if ($.isFunction(prop.enable))
            prop.enable(true)
    });
}

function LimparCampoData(kout) {
    if (!kout)
        return;

    $.each(kout, function (i, prop) {
        if (prop.getType == typesKnockout.date || prop.getType == typesKnockout.dateTime || prop.getType == typesKnockout.month || prop.getType == typesKnockout.time || prop.getType == typesKnockout.timeSec || prop.getType == typesKnockout.year) {
            prop.val("");
            if (prop.updateValue instanceof Function)
                prop.updateValue();
        }
    });

}

function LimparCampo(prop) {
    if (typeof prop == "object" && prop.type != null) {
        if (prop.type == types.map) {
            let defaultValue = ObterValorPadrao(prop);

            prop.val(defaultValue);

            if (prop.getType == typesKnockout.date || prop.getType == typesKnockout.dateTime || prop.getType == typesKnockout.month || prop.getType == typesKnockout.time || prop.getType == typesKnockout.timeSec || prop.getType == typesKnockout.year) {
                if (prop.updateValue instanceof Function)
                    prop.updateValue();
            }
            else if (prop.getType == typesKnockout.selectMultiple) {
                $("#" + prop.id).selectpicker("val", defaultValue);
            }
            else if (prop.getType == typesKnockout.basicTable)
                prop.basicTable.CarregarGrid([]);
        }
        else if (prop.type == types.entity) {
            LimparCampoEntity(prop);
        }
        else if (prop.type == types.multiplesEntities) {
            LimparCampoMultiplesEntities(prop);
        }
        else if (prop.type == types.listEntity) {
            prop.list = new Array();
            LimparCampoEntity(prop)
        }

        prop.requiredClass("form-control");
    }
}

function LimparCampoEntity(prop) {
    var valorPadrao = ObterValorPadrao(prop);

    prop.codEntity(prop.defCodEntity);
    prop.val(valorPadrao);
    prop.entityDescription(valorPadrao);

    if (prop.cleanEntityCallback instanceof Function)
        prop.cleanEntityCallback();
}

function LimparCampoMultiplesEntities(prop) {
    prop.multiplesEntities([]);

    if (prop.cleanEntityCallback instanceof Function)
        prop.cleanEntityCallback();
}

function ValidarCamposObrigatorios(kout) {
    var tudoCerto = true;
    $.each(kout, function (i, prop) {
        if (prop.type == types.map && (prop.getType == null || prop.getType != typesKnockout.dynamic)) {
            if (!ValidarCampoObrigatorioMap(prop)) {
                tudoCerto = false;
            }
        } else if (prop.type == types.entity || prop.type == types.listEntity || prop.type == types.multiplesEntities) {
            if (!ValidarCampoObrigatorioEntity(prop)) {
                tudoCerto = false;
            }
        }
    });
    return tudoCerto;
}

function LimparStatusObrigatorioEntity(propriedade) {
    var $campo = $("#" + propriedade.id);

    propriedade.requiredClass("form-control");
}

function ValidarCampoObrigatorioEntity(propriedade) {
    var $campo = $("#" + propriedade.id);
    var campoInformado = !(((propriedade.val() == "") || (propriedade.codEntity() == 0) || (propriedade.val() == undefined) || (propriedade.codEntity() == undefined)) && !(propriedade.val() == "0" && propriedade.codEntity() > 0));
    var campoObrigatorio = (propriedade.required instanceof Function) ? propriedade.required() : propriedade.required;
    var campoVisivel = (propriedade.visible instanceof Function) ? propriedade.visible() : propriedade.visible;

    $campo.off("keyup");

    if (!campoObrigatorio || !campoVisivel || campoInformado) {
        if (propriedade.requiredClass != null)
            propriedade.requiredClass("form-control");

        return true;
    }

    $campo.on("keyup", function () {
        if ($campo.val().trim() == "")
            propriedade.requiredClass("form-control is-invalid");
        else
            propriedade.requiredClass("form-control");
    });

    propriedade.val(ObterValorPadrao(propriedade));
    propriedade.codEntity(propriedade.defCodEntity);
    propriedade.requiredClass("form-control is-invalid");

    return false;
}

function ValidarCampoObrigatorioMap(propriedade) {
    var $campo = $("#" + propriedade.id);

    var campoInformado = false;
    let minLength;

    switch (propriedade.getType) {
        case typesKnockout.month:
            minLength = 2;
            break;
        case typesKnockout.year:
            minLength = 4;
            break;
        case typesKnockout.date:
            minLength = 8;
            break;
        default:
            minLength = 13;
    }

    if (propriedade.getType === typesKnockout.date || propriedade.getType === typesKnockout.dateTime || propriedade.getType === typesKnockout.month || propriedade.getType === typesKnockout.year)
        campoInformado = !((propriedade.val() === null) || (propriedade.val() === undefined) || (propriedade.val().replace(/\//g, "").replace(/:/g, "").replace(/_/g, "") === "") || (propriedade.val().replace(/\//g, "").replace(/:/g, "").replace(/_/g, "").length < minLength));
    else
        campoInformado = !((propriedade.val() === null) || (propriedade.val() === undefined) || (propriedade.val() === ""));


    var campoObrigatorio = (propriedade.required instanceof Function) ? propriedade.required() : propriedade.required;
    var campoTipoSelect = $campo.is("select");

    var campoVisivel = (propriedade.visible instanceof Function) ? propriedade.visible() : propriedade.visible;

    $campo.off("keyup");

    if (!campoObrigatorio || !campoVisivel || campoInformado) {
        if (propriedade.requiredClass != null)
            propriedade.requiredClass("form-control");

        return true;
    }

    if (campoTipoSelect)
        propriedade.requiredClass("form-control is-invalid");
    else {
        $campo.on("keyup", function () {
            if ($campo.val().trim() == "")
                propriedade.requiredClass("form-control is-invalid");
            else
                propriedade.requiredClass("form-control");
        });

        propriedade.requiredClass("form-control is-invalid");
    }

    return false;
}

function KoBindings(knockout, idElemento, validarSomenteLeitura, idBtnFocusEnterKeyPress) {
    var elemento = document.getElementById(idElemento);

    if (elemento == null)
        console.error("Esqueçeu da div #" + idElemento + "!!!!");

    ko.cleanNode(elemento);
    ko.applyBindings(knockout, elemento);

    if (idBtnFocusEnterKeyPress != null) {
        $('#' + idElemento).keypress(function (e) {
            var keyCode = e.keyCode || e.which;

            if (keyCode == 13)
                $('#' + idBtnFocusEnterKeyPress).trigger('click');
        });

        if (validarSomenteLeitura == null)
            validarSomenteLeitura = false;
    }

    if (validarSomenteLeitura == null)
        validarSomenteLeitura = true;

    ConfigurarCamposKnockout(knockout, validarSomenteLeitura);
}

function AdicionarAjudaCampo(prop) {
    if (prop.issue > 0) {
        if ($("#" + prop.id).is(":checkbox")) {
            let htmlCheck = "<button type='button' class='custom-control-helper' title='Entenda a opção' onclick='iniciarAjuda(" + prop.issue + ");' tabindex='-1'><i class='fal fa-question-circle'></i></button>"
            let textoCheck = prop.text != null ? prop.text : prop.text() != null ? prop.text() : "";
            $("#" + prop.id).parent().find("label").html(textoCheck + htmlCheck);
        } else if ($("#" + prop.id).is(":button")) {
            if ($("#" + prop.id).hasClass("btnPesquisarFiltroPesquisa")) {
                $("#" + prop.id).wrap('<div class="btn-group" style="float: right;"></div>');
                $("#" + prop.id).before('<button class="' + $("#" + prop.id).attr("class") + '" onclick="iniciarAjuda(' + prop.issue + ');" style="margin-left: -10px !important; padding-left: 4px; padding-right: 4px; border: 0; background: none; box-shadow: none;" tabindex="-1"><i class="fal fa-question-circle"></i></button>');
            }
        } else if (prop.type == types.event && $("#" + prop.idBtnSearch).is(":button")) {
            //$("#" + prop.idBtnSearch).wrap('<div class="btn-group"></div>');
            //$("#" + prop.idBtnSearch).after('<button class="' + $("#" + prop.idBtnSearch).attr("class") + '" onclick="iniciarAjuda(' + prop.issue + ');" style="padding-left: 4px; padding-right: 6px; padding-top: 9px; border: 0; background: none; box-shadow: none; color: #3276b1;" tabindex="-1"><i class="fal fa-question-circle" style="font-size: 18px;"></i></button>');
            $("#" + prop.idBtnSearch).after("<a href='javascript:void(0);' class='ajuda' title='Entenda a opção' onclick='iniciarAjuda(" + prop.issue + ");' tabindex='-1'>&nbsp;<i class='fal fa-question-circle'></i></a>");
        } else {
            let elem = $("#" + prop.id).parent().parent();
            if (elem.find("label").length > 0 && elem.find(".iniciar-ajuda-" + prop.issue).length == 0) {
                elem.find("label").after("<a href='javascript:void(0);' class='ajuda iniciar-ajuda-" + prop.issue + "' title='Entenda a opção' onclick='iniciarAjuda(" + prop.issue + ");' tabindex='-1'>&nbsp;<i class='fal fa-question-circle'></i></a>");
                return;
            } else {
                elem = $("#" + prop.id).parent().parent().parent().parent();

                if (elem.hasClass("form-group") && elem.find("b").length > 0)
                    elem.find("b").after("<a href='javascript:void(0);' class='ajuda' title='Entenda a opção' onclick='iniciarAjuda(" + prop.issue + ");' tabindex='-1'>&nbsp;<i class='fal fa-question-circle'></i></a>");
            }
        }
    }
}

function ComponenteMultiplasEntidade(prop) {
    // Cria o elemento
    let $objMultiple = $(
        '<div class="input-group-prepend container-componente-multiplas-entidades" id="' + prop.idBtnSearch + '_multiples_entities">' +
        '   <button class="btn btn-default waves-effect waves-themed componente-multiplas-entidades" type="button">' +
        '       <i class="fal fa-th-list"></i>' +
        '   </button>' +
        '</div>'
    );

    // Busca o objeto titulo para inserir o btn
    let $obj = $("#" + prop.id);

    let $labelObj = $obj.parent();
    $labelObj.find("[class*='input-group container-componente-multiplas-entidades']").remove();

    //let $labelTitle = $labelObj.prev();

    $obj.before($objMultiple);
}

function AjustarConfiguracaoCampoKnockout(prop) {
    if (!prop || !prop.getType)
        return;

    if (prop.tempusDominusInstance)
        AddTempusDominusEvents(prop);
}

function ConfigurarCampoKnockout(prop, validarSomenteLeitura) {
    if (!prop || !prop.getType)
        return;

    SetarMascara(prop);

    if (prop.type == types.multiplesEntities)
        ComponenteMultiplasEntidade(prop);

    if ($("#" + prop.id).is("select"))
        prop.requiredClass("form-control");

    AdicionarAjudaCampo(prop);

    if (validarSomenteLeitura && _FormularioSomenteLeitura) {
        $("#" + prop.id).prop("disabled", true);

        if (prop.idBtnSearch != "")
            $("#" + prop.idBtnSearch).prop("disabled", true);
    }
}

function ConfigurarCamposKnockout(knockout, validarSomenteLeitura) {
    if (validarSomenteLeitura == null)
        validarSomenteLeitura = true;

    if (Boolean(knockout) && Boolean(knockout.ModeloFiltrosPesquisa)) {
        new BuscarModeloFiltroPesquisa(knockout.ModeloFiltrosPesquisa, function (res) {
            PreencherJsonFiltroPesquisa(knockout, res.Dados);
            knockout.ModeloFiltrosPesquisa.codEntity(res.Codigo);
            knockout.ModeloFiltrosPesquisa.val(res.ModeloDescricao);

            if (knockout.ModeloFiltrosPesquisa.callbackRetornoPesquisa instanceof Function)
                knockout.ModeloFiltrosPesquisa.callbackRetornoPesquisa();
        }, knockout.ModeloFiltrosPesquisa.tipoFiltroPesquisa);
    }

    for (let nomePropriedade in knockout)
        ConfigurarCampoKnockout(knockout[nomePropriedade], validarSomenteLeitura);

    for (let nomePropriedade in knockout)
        AjustarConfiguracaoCampoKnockout(knockout[nomePropriedade]);
}

//function ConfigurarCampoDateTime(prop) {

//    new tempusDominus.TempusDominus(document.getElementById(prop.id));

//    //$("#" + prop.id).datetimepicker({
//    //    //debug: true,
//    //    locale: _CONFIGURACAO_TMS.Culture.toLowerCase(),
//    //    useCurrent: false,
//    //    //inline: true,
//    //    sideBySide: true,
//    //    format: 'DD/MM/YYYY HH:mm'
//    //});

//    //if (prop.dateRangeInit != null) {
//    //    $("#" + prop.id).on("dp.change", function (e) {
//    //        $("#" + prop.dateRangeInit.id).data("DateTimePicker").maxDate(e.date);
//    //    });
//    //} else {
//    //    if (prop.dateRangeLimit != null) {
//    //        $("#" + prop.id).on("dp.change", function (e) {
//    //            $("#" + prop.dateRangeLimit.id).data("DateTimePicker").minDate(e.date);
//    //        });
//    //    }
//    //}

//    //if (prop.dateInit != null) {
//    //    $("#" + prop.id).data("DateTimePicker").minDate(prop.dateInit.val());
//    //    $("#" + prop.dateInit.id).on("dp.change", function (e) {
//    //        if ($("#" + prop.id).data("DateTimePicker") != null) {
//    //            $("#" + prop.id).data("DateTimePicker").minDate(e.date);
//    //        }
//    //    });
//    //}

//    //if (prop.dateLimit != null) {
//    //    var maxIni = "";
//    //    if (prop.dateLimit.val() != "")
//    //        maxIni = new Date(parseInt(prop.dateLimit.val().split('/')[2]), parseInt(prop.dateLimit.val().split('/')[1]) - 1, parseInt(prop.dateLimit.val().split('/')[0]), 23, 59, 59, 0);
//    //    $("#" + prop.id).data("DateTimePicker").maxDate(maxIni);
//    //    $("#" + prop.dateLimit.id).on("dp.change", function (e) {
//    //        var max = "";
//    //        if (e.date != null) {
//    //            var strdata = $("#" + prop.dateLimit.id).val();
//    //            max = new Date(parseInt(strdata.split('/')[2]), parseInt(strdata.split('/')[1]) - 1, parseInt(strdata.split('/')[0]), 23, 59, 59, 0);
//    //        }

//    //        if ($("#" + prop.id).data("DateTimePicker") != null) {
//    //            $("#" + prop.id).data("DateTimePicker").maxDate(max);
//    //        }
//    //    });
//    //}


//    //$("#" + prop.id).mask("00/00/0000 00:00", { selectOnFocus: true, clearIfNotMatch: true });
//}

//function ConfigurarCampoTime(prop) {
//    /// <input type="time">
//    //$("#" + prop.id).datetimepicker({
//    //    //debug: true,
//    //    locale: _CONFIGURACAO_TMS.Culture.toLowerCase(),
//    //    useCurrent: false,
//    //    format: 'HH:mm'
//    //});

//    //$("#" + prop.id).mask("00:00", { selectOnFocus: true, clearIfNotMatch: true });
//}

//function ConfigurarCampoTimeSec(prop) {
//    /// <input type="time" step="1">
//    //$("#" + prop.id).datetimepicker({
//    //    //debug: true,
//    //    locale: _CONFIGURACAO_TMS.Culture.toLowerCase(),
//    //    useCurrent: false,
//    //    format: 'HH:mm:ss'
//    //});

//    //$("#" + prop.id).mask("00:00:00", { selectOnFocus: true, clearIfNotMatch: true });
//}

function ConfigurarCampoMask(prop) {
    $("#" + prop.id).mask(prop.mask, prop.maskParams != null ? prop.maskParams : { selectOnFocus: true, clearIfNotMatch: true });
}

function ConfigurarCampoDecimal(prop) {
    $("#" + prop.id).configDecimal = {
        precision: 2,
        allowZero: false,
        allowNegative: false
    };

    $("#" + prop.id).maskMoney(prop.configDecimal);
}

//function ConfigurarCampoMonth(prop) {
//    //configurações de data na url: http://www.jqueryrain.com/?Qoeemzn0 

//    //$("#" + prop.id).datetimepicker({
//    //    //debug: true,
//    //    locale: _CONFIGURACAO_TMS.Culture.toLowerCase(),
//    //    viewMode: 'months',
//    //    useCurrent: false,
//    //    format: 'MM/YYYY'
//    //});

//    //if (prop.dateRangeInit != null) {
//    //    $("#" + prop.id).on("dp.change", function (e) {
//    //        $("#" + prop.dateRangeInit.id).data("DateTimePicker").maxDate(e.date);
//    //    });
//    //} else {
//    //    if (prop.dateRangeLimit != null) {
//    //        $("#" + prop.id).on("dp.change", function (e) {
//    //            $("#" + prop.dateRangeLimit.id).data("DateTimePicker").minDate(e.date);
//    //        });
//    //    }
//    //}

//    //if (prop.dateInit != null) {
//    //    if (prop.dateInit.val() != "")
//    //        $("#" + prop.id).data("DateTimePicker").minDate(prop.dateInit.val());
//    //    $("#" + prop.dateInit.id).on("dp.change", function (e) {
//    //        if ($("#" + prop.id).data("DateTimePicker") != null) {
//    //            $("#" + prop.id).data("DateTimePicker").minDate(e.date);
//    //        }
//    //    });
//    //}

//    //if (prop.dateLimit != null) {
//    //    if (prop.dateLimit.val() != "")
//    //        $("#" + prop.id).data("DateTimePicker").maxDate(prop.dateLimit.val());
//    //    $("#" + prop.dateLimit.id).on("dp.change", function (e) {
//    //        if ($("#" + prop.id).data("DateTimePicker") != null) {
//    //            $("#" + prop.id).data("DateTimePicker").maxDate(e.date);
//    //        }
//    //    });
//    //}

//    //$("#" + prop.id).mask("00/0000", { selectOnFocus: true, clearIfNotMatch: true });
//}

function ConfigurarCampoDate(prop) {

    let fieldConfig = GetDateTimeFieldConfigurations(prop.getType);

    SetDateTimeFieldDefaultHtml(prop, fieldConfig);

    SetTempusDominusDefaults(prop, fieldConfig);

    //configurações de data na url: http://www.jqueryrain.com/?Qoeemzn0 



    //$("#" + prop.id).datetimepicker({
    //    //debug: true,
    //    locale: _CONFIGURACAO_TMS.Culture.toLowerCase(),
    //    useCurrent: false,
    //    format: 'DD/MM/YYYY'
    //});

    //if (prop.dateRangeInit != null) {
    //    $("#" + prop.id).on("dp.change", function (e) {
    //        $("#" + prop.dateRangeInit.id).data("DateTimePicker").maxDate(e.date);
    //    });
    //} else {
    //    if (prop.dateRangeLimit != null) {
    //        $("#" + prop.id).on("dp.change", function (e) {
    //            $("#" + prop.dateRangeLimit.id).data("DateTimePicker").minDate(e.date);
    //        });
    //    }
    //}

    //if (prop.dateInit != null) {
    //    if (prop.dateInit.val() != "")
    //        $("#" + prop.id).data("DateTimePicker").minDate(prop.dateInit.val());
    //    $("#" + prop.dateInit.id).on("dp.change", function (e) {
    //        if ($("#" + prop.id).data("DateTimePicker") != null) {
    //            $("#" + prop.id).data("DateTimePicker").minDate(e.date);
    //        }
    //    });
    //}

    //if (prop.dateLimit != null) {
    //    if (prop.dateLimit.val() != "")
    //        $("#" + prop.id).data("DateTimePicker").maxDate(prop.dateLimit.val());
    //    $("#" + prop.dateLimit.id).on("dp.change", function (e) {
    //        if ($("#" + prop.id).data("DateTimePicker") != null) {
    //            $("#" + prop.id).data("DateTimePicker").maxDate(e.date);
    //        }
    //    });
    //}

    //$("#" + prop.id).mask("00/00/0000", { selectOnFocus: true, clearIfNotMatch: true });
}

function SetarSelectMultiple(prop) {
    //configurações do selectpicker na url: https://silviomoreto.github.io/bootstrap-select/
    let inicializarSelectPicker = function () {
        var $el = prop.get$();

        if ($("body").hasClass('mobile-detected')) {
            return;
        }

        if ($el.data('selectpicker'))
            $el.selectpicker('refresh');
        else
            $el.selectpicker();
    };

    if (prop.url != null && prop.url.trim() != "") {
        executarReST(prop.url, prop.params, function (r) {
            if (r.Success) {
                prop.options(r.Data);
                inicializarSelectPicker();
                if (prop.callback != null)
                    prop.callback();
            }
        });
    } else {
        inicializarSelectPicker();
    }
}

function SetarMascaraTelefone(elem) {
    var phone, element;
    element = $(elem);
    element.unmask();
    phone = element.val().replace(/\D/g, '');
    if (_CONFIGURACAO_TMS.PermitirCadastroDeTelefoneInternacional) {
        element.mask("(00) 00000-00090", { selectOnFocus: true, clearIfNotMatch: true });
    }
    else if (phone.length > 10) {
        element.mask("(00) 00000-0009", { selectOnFocus: true, clearIfNotMatch: true });
    } else {
        element.mask("(00) 0000-00009", { selectOnFocus: true, clearIfNotMatch: true });
    }
}

function ValidarCamposEntity(knout) {
    $.each(knout, function (i, prop) {
        if (prop.type == types.entity || prop.type == types.listEntity)
            if ((prop.val() == "" || prop.codEntity() == 0) && !(prop.val() == "0" && prop.codEntity() > 0)) {
                prop.val(ObterValorPadrao(prop));
                prop.codEntity(prop.defCodEntity);
            }
    });
}

function RetornarObjetoPesquisa(knout) {
    var entidade = new Object();

    if (knout != null) {
        $.each(knout, function (i, prop) {
            if (!prop)
                return;

            if (prop.type == types.map) {
                if (prop.getType != null) {
                    if (prop.getType == typesKnockout.decimal && typeof (prop.val()) == "string") {
                        if (prop.val() == "")
                            entidade[i] = null;
                        else
                            entidade[i] = prop.val();
                    }
                    else if (prop.getType == typesKnockout.int && typeof (prop.val()) == "string") {
                        if (prop.val() == "")
                            entidade[i] = null;
                        else
                            entidade[i] = Globalize.parseInt(prop.val());
                    }

                    else if (prop.getType == typesKnockout.basicTable) {
                        var data = prop.basicTable != null ? prop.basicTable.BuscarRegistros() : [];

                        entidade[i] = JSON.stringify(data);
                    }
                    else if (prop.getType == typesKnockout.report) {
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
                    }
                    else if (prop.getType == typesKnockout.selectMultiple)
                        entidade[i] = JSON.stringify(prop.val());
                    else
                        entidade[i] = prop.val();
                }
                else
                    entidade[i] = prop.val();
            }
            else if (prop.type == types.entity) {
                if ((prop.val() == "" || prop.codEntity() == 0) && !(prop.val() == "0" && prop.codEntity() > 0)) {
                    prop.codEntity(prop.defCodEntity);
                    prop.val("");
                }

                entidade[i] = prop.codEntity();
            }
            else if (prop.type == types.listEntity)
                entidade[i] = JSON.stringify(recursiveListEntity(prop));
            else if (prop.type == types.multiplesEntities)
                entidade[i] = JSON.stringify(recursiveMultiplesEntities(prop));
        });
    }

    return entidade;
}

function RetornarObjetoFiltroPesquisa(knocout) {
    var filtro = new Object();

    if (knocout) {
        $.each(knocout, function (propName, prop) {
            var salvarFiltro = true;

            if (prop.salvarFiltro != undefined)
                salvarFiltro = prop.salvarFiltro

            if (salvarFiltro) {
                if (prop.type == types.entity) {
                    var newProp = new Object();
                    newProp.codEntity = prop.codEntity();
                    newProp.val = prop.val();
                    newProp.entityDescription = prop.entityDescription();

                    filtro[propName] = newProp;
                }
                else if (prop.type == types.multiplesEntities) {
                    var multiple = prop.multiplesEntities();

                    var entities = [];

                    multiple.map(function (obj) {
                        var newProp = new Object();
                        newProp = obj;
                        entities.push(newProp);

                    });

                    filtro[propName] = entities;

                }
                else if (prop.type == types.map) {
                    var newProp = new Object();
                    newProp.val = prop.val();
                    newProp.type = prop.getType

                    filtro[propName] = newProp;
                }
                else {
                    //Implementar outros tipos
                }
            }
        });
    }

    return filtro;
}

function RetornarJsonFiltroPesquisa(knocout) {
    var data = RetornarObjetoFiltroPesquisa(knocout);

    return JSON.stringify(data);
}

function ObterFiltroPesquisaPorDescricao(descricao, filtroPesquisa) {
    var obj = null;

    $.each(filtroPesquisa, function (propName, prop) {
        if (descricao == propName) {
            obj = filtroPesquisa[propName];
            return;
        }
    });

    return obj;
}

function ObterValorPadrao(prop) {
    return (prop.def instanceof Function) ? prop.def() : prop.def;
}

function PreencherJsonFiltroPesquisa(knocout, jsonFiltroPesquisa) {
    var filtroPesquisa = JSON.parse(jsonFiltroPesquisa);

    PreencherObjetoFiltroPesquisa(knocout, filtroPesquisa)
}

function PreencherObjetoFiltroPesquisa(knocout, filtroPesquisa) {
    if ((knocout) && (filtroPesquisa)) {

        $.each(knocout, function (propName, prop) {

            var objFiltroPesquisa = ObterFiltroPesquisaPorDescricao(propName, filtroPesquisa);

            if (objFiltroPesquisa) {

                if (prop.type == types.entity) {
                    prop.codEntity(objFiltroPesquisa.codEntity);
                    prop.val(objFiltroPesquisa.val);
                    prop.entityDescription(objFiltroPesquisa.entityDescription);
                }
                else if (prop.type == types.multiplesEntities) {
                    if ($.isArray(objFiltroPesquisa))
                        prop.multiplesEntities(objFiltroPesquisa);
                }
                else if (prop.type == types.map) {
                    prop.val(objFiltroPesquisa.val);
                }
                else if (prop.type == types.event) {
                    //Não mapear eventos
                }
                else {
                    //Implementar outros tipos
                }
            }

        });
    }
}

function SetarGetType(prop, getType) {
    prop.getType = getType;
    SetarMascara(prop);
}

function SetarMascara(prop) {
    switch (prop.getType) {
        case typesKnockout.int:
            $("#" + prop.id).maskMoney(prop.configInt);
            break;
        case typesKnockout.decimal:
            $("#" + prop.id).maskMoney(prop.configDecimal);
            break;
        case typesKnockout.date:
        case typesKnockout.dateTime:
        case typesKnockout.month:
        case typesKnockout.year:
        case typesKnockout.time:
        case typesKnockout.timeSec:
            ConfigurarCampoDate(prop);
            break;
        case typesKnockout.email:
            $("#" + prop.id).addClass("lowercase");
            break;
        case typesKnockout.cpf:
            switch (_CONFIGURACAO_TMS.Pais) {
                case EnumPaises.Brasil:
                    $("#" + prop.id).mask("000.000.000-00", { selectOnFocus: true, clearIfNotMatch: true });
                    break;
            }
            break;
        case typesKnockout.cnpj:
            switch (_CONFIGURACAO_TMS.Pais) {
                case EnumPaises.Brasil:
                    $("#" + prop.id).mask("00.000.000/0000-00", { selectOnFocus: true, clearIfNotMatch: true });
                    break;
            }
            break;
        case typesKnockout.cpfCnpj:
            let options = {
                onKeyPress: function (cpfCnpj, e, field, options) {
                    let masks = ['000.000.000-00999', '00.000.000/0000-00'];
                    let mask = (cpfCnpj.length > 14) ? masks[1] : masks[0];

                    $("#" + prop.id).mask(mask, options);
                },
                selectOnFocus: true,
                clearIfNotMatch: true
            };
            $("#" + prop.id).mask('000.000.000-00999', options);
            break;
        case typesKnockout.raizCnpj:
            $("#" + prop.id).mask("00.000.000", { selectOnFocus: true, clearIfNotMatch: true });
            break;
        case typesKnockout.placa:
            switch (_CONFIGURACAO_TMS.Pais) {
                case EnumPaises.Brasil:
                    $("#" + prop.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });
                    break;
            }
            break;
        case typesKnockout.phone:
            $("#" + prop.id).mask("(00) 0000-00009", { selectOnFocus: true, clearIfNotMatch: true });
            $("#" + prop.id).on('focus', function () { SetarMascaraTelefone(this); });
            break;
        case typesKnockout.selectMultiple:
            SetarSelectMultiple(prop);
            break;
        case typesKnockout.cep:
            $("#" + prop.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
            break;
        case typesKnockout.mask:
            ConfigurarCampoMask(prop);
            break;
        default:
            break;
    }
}

function GetTempusDominusDefaults(options) {
    let defaults = {
        useCurrent: false,
        allowInputToggle: false,
        localization: {
            dayViewHeaderFormat: {
                month: 'long',
                year: 'numeric'
            },
            locale: _CONFIGURACAO_TMS.Culture.toLowerCase(),
            hourCycle: 'h23'
        },
        display: {
            keepOpen: false,
            icons: {
                time: 'fal fa-clock',
                date: 'fal fa-calendar',
                up: 'fal fa-arrow-up',
                down: 'fal fa-arrow-down',
                next: 'fal fa-chevron-right',
                previous: 'fal fa-chevron-left',
                today: 'fal fa-calendar-check',
                clear: 'fal fa-trash',
                close: 'fal fa-times'
            },
            viewMode: 'calendar',
            components: {
                clock: false
            },
            buttons: {
                today: true,
                clear: true,
                close: true
            },
            theme: 'light'
        }
    };

    return $.extend(true, {}, defaults, options);
}

function SetDateTimeFieldDefaultHtml(prop, options) {
    let $obj = $("#" + prop.id);
    let parentClassName = $obj.parent().attr("class");

    if (!parentClassName || !parentClassName.includes("input-group"))
        $obj.wrap('<div class="input-group" id="td_' + prop.id + '"></div>')
    else {
        let parentId = $obj.parent().attr("id");

        if (!parentId)
            $obj.parent().attr("id", "td_" + prop.id);
        else
            options.customId = parentId;
    }

    let newDiv = $(
        '<div class="input-group-append" id="tempus-dominus-append-' + prop.id + '">' +
        '   <span class="btn btn-primary waves-effect waves-themed">' +
        '       <i class="fal ' + options.icon + '"></i>' +
        '   </span>' +
        '</div>'
    );

    $obj.after(newDiv);
}

function SetTempusDominusDefaults(prop, config) {
    let idDatePickerElement = config.customId || ('td_' + prop.id);
    let elDatePicker = document.getElementById(idDatePickerElement);

    if (elDatePicker == null)
        return;

    let defaultTDConfig = GetTempusDominusDefaults(config.tdOptions);
    let $obj = $("#" + prop.id);
    let defaultValue = $("#" + prop.id).val();

    if (!string.IsNullOrWhiteSpace(defaultValue))
        $obj.val("");

    prop.tempusDominusInstance = new tempusDominus.TempusDominus(elDatePicker, defaultTDConfig);

    prop.tempusDominusInstance.dates.formatInput = function (date) {
        if (!date)
            return "";

        return moment(date).format(config.format);
    };

    prop.tempusDominusInstance.dates.parseInput = function (date) {
        if (!date)
            return undefined;

        var dataConvertida = moment(date, config.format);

        if (dataConvertida.isValid())
            return tempusDominus.DateTime.convert(dataConvertida.toDate());

        return undefined;
    };

    prop.tempusDominusInstance.subscribe(tempusDominus.Namespace.events.error, (e) => {
        prop.val("");
        prop.updateValue();
    });

    prop.updateValue = function () {
        prop.tempusDominusInstance.dates.setValue(prop.tempusDominusInstance.dates.parseInput(prop.val()));
    };

    prop.minDate = function (data) {
        prop.tempusDominusInstance.updateOptions({
            restrictions: {
                minDate: prop.tempusDominusInstance.dates.parseInput(data),
            },
        });

        prop.updateValue();
    }

    prop.maxDate = function (data) {
        prop.tempusDominusInstance.updateOptions({
            restrictions: {
                maxDate: prop.tempusDominusInstance.dates.parseInput(data),
            },
        });

        prop.updateValue();
    }

    $obj.mask(config.mask, { selectOnFocus: true, clearIfNotMatch: true });
    $obj.off("keyup");
    $obj.on("keyup", function (e) {
        if (e.which == 84) {
            e.stopPropagation();
            prop.val(moment().format(config.format));
            prop.updateValue();
        }
    });

    if (!string.IsNullOrWhiteSpace(defaultValue)) {
        $obj.val(defaultValue);
        prop.updateValue();
    }
}

function AddTempusDominusEvents(prop) {
    if (prop.dateRangeInit || prop.dateRangeLimit) {
        if (prop.val()) {
            if (prop.dateRangeInit) {
                if (prop.dateRangeInit.tempusDominusInstance)
                    prop.dateRangeInit.maxDate(prop.val());
            }
            else if (prop.dateRangeLimit) {
                if (prop.dateRangeLimit.tempusDominusInstance)
                    prop.dateRangeLimit.minDate(prop.val());
            }
        }

        prop.tempusDominusInstance.subscribe(tempusDominus.Namespace.events.change, (e) => {
            if (prop.dateRangeInit) {
                if (prop.dateRangeInit.tempusDominusInstance) {
                    prop.dateRangeInit.tempusDominusInstance.updateOptions({
                        restrictions: {
                            maxDate: e.date,
                        },
                    });
                }
            }
            else if (prop.dateRangeLimit) {
                if (prop.dateRangeLimit.tempusDominusInstance) {
                    prop.dateRangeLimit.tempusDominusInstance.updateOptions({
                        restrictions: {
                            minDate: e.date,
                        },
                    });
                }
            }
        });
    }
}

function GetDateTimeFieldConfigurations(type) {
    switch (type) {
        case typesKnockout.date:
            return {
                format: "DD/MM/YYYY",
                mask: "00/00/0000",
                icon: 'fa-calendar',
                tdOptions: {

                }
            };
        case typesKnockout.dateTime:
            return {
                format: "DD/MM/YYYY HH:mm",
                mask: "00/00/0000 00:00",
                icon: 'fa-calendar',
                tdOptions: {
                    display: {
                        components: {
                            clock: true
                        },
                        sideBySide: true
                    }
                }
            };
        case typesKnockout.time:
            return {
                format: "HH:mm",
                mask: "00:00",
                icon: 'fa-clock',
                tdOptions: {
                    display: {
                        viewMode: 'clock',
                        components: {
                            calendar: false,
                            clock: true
                        },
                        sideBySide: true
                    }
                }
            };
        case typesKnockout.timeSec:
            return {
                format: "HH:mm:ss",
                mask: "00:00:00",
                icon: 'fa-clock',
                tdOptions: {
                    display: {
                        viewMode: 'clock',
                        components: {
                            calendar: false,
                            clock: true,
                            seconds: true
                        },
                        sideBySide: true
                    }
                }
            };
        case typesKnockout.month:
            return {
                format: "MM/YYYY",
                mask: "00/0000",
                icon: 'fa-calendar',
                tdOptions: {
                    display: {
                        viewMode: 'months',
                        components: {
                            date: false,
                            clock: false
                        }
                    }
                }
            };
        case typesKnockout.year:
            return {
                format: "YYYY",
                mask: "0000",
                icon: 'fa-calendar',
                tdOptions: {
                    display: {
                        viewMode: 'years',
                        components: {
                            date: false,
                            clock: false,
                            month: false
                        }
                    }
                }
            };
        default:
            return {};
    };
}

function SetTempusDominusToRawInput($txt, type) {
    let elDatePicker = document.getElementById($txt.attr("id"));

    if (elDatePicker == null)
        return;

    let config = GetDateTimeFieldConfigurations(type);
    let defaultTDConfig = GetTempusDominusDefaults(config.tdOptions);

    $txt.tempusDominusInstance = new tempusDominus.TempusDominus(elDatePicker, defaultTDConfig);

    $txt.tempusDominusInstance.dates.formatInput = function (date) {
        if (!date)
            return "";

        return moment(date).format(config.format);
    };

    $txt.tempusDominusInstance.dates.parseInput = function (date) {
        if (!date)
            return undefined;

        let dataConvertida = moment(date, config.format);

        if (dataConvertida.isValid())
            return tempusDominus.DateTime.convert(dataConvertida.toDate());

        return undefined;
    };

    $txt.tempusDominusInstance.subscribe(tempusDominus.Namespace.events.error, (e) => {
        $txt.val("");
        $txt.updateValue();
    });

    $txt.updateValue = function () {
        $txt.tempusDominusInstance.dates.setValue($txt.tempusDominusInstance.dates.parseInput($txt.val()));
    };

    $txt.mask(config.mask, { selectOnFocus: true, clearIfNotMatch: true });
    $txt.off("keyup");
    $txt.on("keyup", function (e) {
        if (e.which == 84) {
            e.stopPropagation();
            $txt.val(moment().format(config.format));
            $txt.updateValue();
        }
    });
}

function RunObjectDispose(knockoutObject) {

    $.each(knockoutObject, function (i, prop) {
        if (prop && prop.tempusDominusInstance) {
            prop.tempusDominusInstance.dispose();
            $("#tempus-dominus-append-" + prop.id).remove();
        }
    });

}
