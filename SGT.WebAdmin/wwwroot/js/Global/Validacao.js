
function ValidarCPF(cpf, obrigatorio) {
    if (obrigatorio == null) {
        obrigatorio = true;
    }
    if (!obrigatorio && cpf == "") {
        return true;
    } else {
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
}
function ValidarCNPJ(cnpj, obrigatorio) {
    if ($.isFunction(obrigatorio))
        obrigatorio = obrigatorio();

    if (obrigatorio == null) {
        obrigatorio = true;
    }
    if (!obrigatorio && cnpj == "") {
        return true;
    } else {
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
}
function ValidarCPFCNPJ(cpfCnpj, obrigatorio) {
    if (obrigatorio == null)
        obrigatorio = true;

    if (!obrigatorio && cnpj == "")
        return true;

    if (string.IsNullOrWhiteSpace(cpfCnpj))
        cpfCnpj = "";

    cpfCnpj = cpfCnpj.replace(/[^\w\s]/gi, '');

    if (cpfCnpj.length == 11)
        return (ValidarCPF(cpfCnpj, obrigatorio));
    else if (cpfCnpj.length == 14)
        return (ValidarCNPJ(cpfCnpj, obrigatorio));

    return false;
}

function ValidarEmail(mail) {
    if (mail != "" && mail != null && mail.trim() != "") {
        var er = new RegExp(/^[A-Za-z0-9_\-\.]+@[A-Za-z0-9_\-\.]{2,}\.[A-Za-z0-9]{2,}(\.[A-Za-z0-9])?$/);
        if (typeof (mail) == "string") {
            return er.test(mail);
        } else if (typeof (mail) == "object") {
            return er.test(mail.value);
        }

        return false;
    }

    return true;
}

function ValidarMultiplosEmails(emails) {
    if (emails && emails.trim()) {
        listaEmails = emails.split(";");

        for (var i = 0; i < listaEmails.length; i++) {
            var email = listaEmails[i];

            if (!email.trim() || !ValidarEmail(email.trim()))
                return false;
        }
    }

    return true;
}

function forceLower(input) {
    input.value = input.value.toLowerCase();
}

function ValidarChaveAcesso(chaveCompleta) {

    chaveCompleta = chaveCompleta.replace(/\D/g, '');

    if (chaveCompleta.length != 44)
        return false;

    var digito = Globalize.parseInt(chaveCompleta.substring(43));
    var chave = chaveCompleta.slice(0, 43);

    var digitoRetorno;
    var soma = 0;
    var resto = 0;
    var peso = [4, 3, 2, 9, 8, 7, 6, 5];

    for (var i = 0; i < chave.length; i++)
        soma += peso[i % 8] * (Globalize.parseInt(chave.substring(i, (i + 1))));

    resto = soma % 11;

    if (resto == 0 || resto == 1)
        digitoRetorno = 0;
    else
        digitoRetorno = 11 - resto;

    if (digito == digitoRetorno)
        return true;
    else
        return false;
}