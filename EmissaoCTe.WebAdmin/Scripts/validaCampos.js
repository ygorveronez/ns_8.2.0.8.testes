$(document).ready(function () {
    $(".maskedInput").focus(function () {
        var field = this;
        setTimeout(function () { selectAllText(field.id); }, 20);
    });
    $("input:text").attr("autocomplete", "off");
    $(".phone").mask("(99) 9999-9999?9").change(function () { SetarMascaraTelefone(this); });
    setTimeout(function () {
        if ($("#ui-datepicker-div").length > 0)
            $("#ui-datepicker-div").hide();
    }, 800);
    if (document.forms[0].attachEvent) {
        document.forms[0].attachEvent("onsubmit", makeInvalidRed);
    }
    else if (document.forms[0].addEventListener) {
        document.forms[0].addEventListener("submit", makeInvalidRed, false);
    }
});

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

function selectAllText(fieldId) {
    $('#' + fieldId).select();
}

function makeInvalidRed() {
    try {
        if (Page_Validators) {
            var validator;
            var ctls = new Array();
            for (var i = 0; i < Page_Validators.length; i++) {
                validator = Page_Validators[i];
                if (!validator.isvalid) {
                    ctls[validator.controltovalidate] = false;
                }
                else {
                    if (typeof (ctls[validator.controltovalidate]) == 'undefined') {
                        ctls[validator.controltovalidate] = true;
                    }
                    else {
                        ctls[validator.controltovalidate] = ctls[validator.controltovalidate] && true;
                    }
                }
            }
            for (var key in ctls) {
                if (ctls[key] == false) {
                    CampoComErro('#' + key);
                    $("#divMensagemAlerta .mensagem").text("Os campos com asterísco (*) ou em vermelho são obrigatórios.");
                    $("#divMensagemAlerta").slideDown();
                } else {
                    CampoSemErro('#' + key);
                }
            }
        }
    } catch (e) { }
}

function CampoComErro(idCampo) {
    $(idCampo).addClass("error");
}
function CampoSemErro(idCampo) {
    $(idCampo).removeClass("error");
}

function ValidarCPF(cpf) {
    cpf = cpf.replace(/[^\w\s]/gi, '');
    var numeros, digitos, soma, i, resultado, digitos_iguais;
    digitos_iguais = 1;
    if (cpf.length < 11)
        return false;
    for (i = 0; i < cpf.length - 1; i++)
        if (cpf.charAt(i) != cpf.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
    if (!digitos_iguais) {
        numeros = cpf.substring(0, 9);
        digitos = cpf.substring(9);
        soma = 0;
        for (i = 10; i > 1; i--)
            soma += numeros.charAt(10 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(0))
            return false;
        numeros = cpf.substring(0, 10);
        soma = 0;
        for (i = 11; i > 1; i--)
            soma += numeros.charAt(11 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(1))
            return false;
        return true;
    }
    else
        return false;
}
function ValidarCNPJ(cnpj) {
    cnpj = cnpj.replace(/[^\w\s]/gi, '');
    var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
    digitos_iguais = 1;
    if (cnpj.length != 14)
        return false;
    for (i = 0; i < cnpj.length - 1; i++)
        if (cnpj.charAt(i) != cnpj.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
    if (!digitos_iguais) {
        tamanho = cnpj.length - 2;
        numeros = cnpj.substring(0, tamanho);
        digitos = cnpj.substring(tamanho);
        soma = 0;
        pos = tamanho - 7;
        for (i = tamanho; i >= 1; i--) {
            soma += numeros.charAt(tamanho - i) * pos--;
            if (pos < 2)
                pos = 9;
        }
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(0))
            return false;
        tamanho = tamanho + 1;
        numeros = cnpj.substring(0, tamanho);
        soma = 0;
        pos = tamanho - 7;
        for (i = tamanho; i >= 1; i--) {
            soma += numeros.charAt(tamanho - i) * pos--;
            if (pos < 2)
                pos = 9;
        }
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(1))
            return false;
        return true;
    }
    else
        return false;
}

function ValidarEmail(mail) {
    var er = new RegExp(/^[A-Za-z0-9_\-\.]+@[A-Za-z0-9_\-\.]{2,}\.[A-Za-z0-9]{2,}(\.[A-Za-z0-9])?/);
    if (typeof (mail) == "string") {
        if (er.test(mail)) { return true; }
    } else if (typeof (mail) == "object") {
        if (er.test(mail.value)) {
            return true;
        }
    } else {
        return false;
    }
}