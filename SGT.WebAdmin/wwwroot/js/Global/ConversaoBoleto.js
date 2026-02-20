function calcula_barra(linha) {
    barra = linha.replace(/[^0-9]/g, '');

    if (modulo11_banco('34191000000000000001753980229122525005423000') !== "1") {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Função 'modulo11_banco' está com erro!");
        return linha;
    }

    if (barra.length < 47)
        barra = barra + '00000000000'.substr(0, 47 - barra.length);

    if (barra.length !== 47) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "A linha do código de barras está incompleta!" + barra.length);
        return linha;
    }

    barra = barra.substr(0, 4)
        + barra.substr(32, 15)
        + barra.substr(4, 5)
        + barra.substr(10, 10)
        + barra.substr(21, 10);

    if (modulo11_banco(barra.substr(0, 4) + barra.substr(5, 39)) !== barra.substr(4, 1)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Digito verificador " + barra.substr(4, 1) + ", o correto é " + modulo11_banco(barra.substr(0, 4) + barra.substr(5, 39)) + "\nO sistema não altera automaticamente o dígito correto na quinta casa!");
        return linha;
    }

    return barra;
}

function calcula_linha(barra) {
    linha = barra.replace(/[^0-9]/g, '');

    if (modulo10('399903512') !== "8") {
        exibirMensagem(tipoMensagem.aviso, "Função 'modulo10' está com erro!");
        return barra;
    }

    if (linha.length !== 44) {
        exibirMensagem(tipoMensagem.aviso, "A linha do código de barras está incompleta");
        return barra;
    }

    var campo1 = linha.substr(0, 4) + linha.substr(19, 1) + '.' + linha.substr(20, 4);
    var campo2 = linha.substr(24, 5) + '.' + linha.substr(24 + 5, 5);
    var campo3 = linha.substr(34, 5) + '.' + linha.substr(34 + 5, 5);
    var campo4 = linha.substr(4, 1);     // Digito verificador
    var campo5 = linha.substr(5, 14);    // Vencimento + Valor

    if (modulo11_banco(linha.substr(0, 4) + linha.substr(5, 99)) !== campo4) {
        exibirMensagem(tipoMensagem.aviso, "Digito verificador " + campo4 + ", o correto é " + modulo11_banco(linha.substr(0, 4) + linha.substr(5, 99)) + "\nO sistema não altera automaticamente o dígito correto na quinta casa!");
        return barra;
    }

    if (campo5 === 0)
        campo5 = '000';

    linha = campo1 + modulo10(campo1)
        + ' '
        + campo2 + modulo10(campo2)
        + ' '
        + campo3 + modulo10(campo3)
        + ' '
        + campo4
        + ' '
        + campo5
        ;

    return linha;
}

function validar_valor(numero, valor, valor2) {

    if (valor !== "" && numero !== "") {
        var linha = numero.replace(/[^0-9]/g, '');
        var ehCodigoBarras = false;
        if (linha.length === 44)
            ehCodigoBarras = true;

        var valorLinha = "";
        if (!ehCodigoBarras) {
            if (linha.length !== 47) {
                exibirMensagem(tipoMensagem.aviso, "Validação", "A linha digitável está incompleta.");
                return;
            }
            valorLinha = linha.substr(37, 10);
        }
        else {
            if (linha.length !== 44) {
                exibirMensagem(tipoMensagem.aviso, "Validação", "O código de barras está incompleta.");
                return;
            }
            valorLinha = linha.substr(9, 10);
        }

        var valorConvertido = valor.replace(",", "").replace(".", "");
        valorConvertido = ("0000000000" + valorConvertido).slice(-10);

        if (valorConvertido !== valorLinha) {
            if (valor2) {
                validar_valor(numero, valor2);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Validação", "O valor do Número do Boleto não está de acordo com o Valor Original.");
        }
        else
            exibirMensagem(tipoMensagem.ok, "Validação", "O valor do Número do Boleto está correto com o Valor Original.");
    }

}

//function fator_vencimento (dias) {
//    var currentDate, t, d, mes;
//    t = new Date();
//    currentDate = new Date();
//    currentDate.setFullYear(1997,9,7);//alert(currentDate.toLocaleString());
//    t.setTime(currentDate.getTime() + (1000 * 60 * 60 * 24 * dias));//alert(t.toLocaleString());
//    mes = (currentDate.getMonth()+1); if (mes < 10) mes = "0" + mes;
//    dia = (currentDate.getDate()+1); if (dia < 10) dia = "0" + dia;

//    return(t.toLocaleString());
//}

function modulo10(numero) {

    numero = numero.replace(/[^0-9]/g, '');
    var soma = 0;
    var peso = 2;
    var contador = numero.length - 1;

    while (contador >= 0) {
        multiplicacao = (numero.substr(contador, 1) * peso);
        if (multiplicacao >= 10) { multiplicacao = 1 + (multiplicacao - 10); }
        soma = soma + multiplicacao;
        if (peso === 2) {
            peso = 1;
        } else {
            peso = 2;
        }
        contador = contador - 1;
    }
    var digito = 10 - (soma % 10);
    if (digito === 10) digito = 0;
    return digito.toString();
}


function modulo11_banco(numero) {
    numero = numero.replace(/[^0-9]/g, '');

    var soma = 0;
    var peso = 2;
    var base = 9;
    var resto = 0;
    var contador = numero.length - 1;


    for (var i = contador; i >= 0; i--) {
        soma = soma + (numero.substring(i, i + 1) * peso);

        if (peso < base) {
            peso++;
        } else {
            peso = 2;
        }
    }
    var digito = (11 - (soma % 11));

    if (digito > 9) digito = 0;

    if (digito === 0) digito = 1;
    return digito.toString();
}