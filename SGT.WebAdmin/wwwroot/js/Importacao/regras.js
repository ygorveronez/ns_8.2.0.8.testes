var __required_helper = function (val) {
    return ((val != null) && (val != undefined) && (typeof val == "string" && val != "" || typeof val == "number" && val > 0)) === true;
}
var Regras = {
    required: function (val, params, campo) {
        var valida = __required_helper(val);

        if (!valida)
            return campo.Descricao + " é obrigatório.";

        return valida;
    },
    required_if: function (valCampo, params, campo, linha) {
        var param = params[0];
        var paramValor = params[1];
        var valRequisito = null;
        var valida = true;

        if (typeof linha[param] != "undefined" && typeof linha[param].valor != "undefined")
            valRequisito = linha[param].valor;

        if (valRequisito == paramValor)
            valida = __required_helper(valCampo) === true;

        if (!valida)
            return campo.Descricao + " é obrigatório quando quando " + linha[param].Descricao + " está preenchido com " + paramValor + ".";

        return valida;
    },
    size: function (val, params, campo) {
        var valida = !(val.length != params[0]);

        if (!valida && __required_helper(val))
            return campo.Descricao + " deve ter " + params[0] + " caractere(s).";

        return valida;
    },
    numbers: function (val, params, campo) {
        var patt = /^[0-9]*$/g;
        var valida = patt.test(val);

        if (!valida && __required_helper(val))
            return campo.Descricao + " deve ser apenas números.";

        return valida;
    },
    digits: function (val, params, campo) {
        var patt = /^[a-zA-Z]+$/g;
        var valida = patt.test(val);

        if (!valida && __required_helper(val))
            return campo.Descricao + " deve ser apenas letras.";

        return valida;
    },
    cpfcnpj: function (val, params, campo) {
        var valida = (val.length == 11 && ValidarCPF(val)) || (val.length == 14 && ValidarCNPJ(val));

        if (!valida && __required_helper(val))
            return campo.Descricao + " informado é inválido.";

        return valida;
    },
    min_length: function (val, params, campo) {
        var valida = !(val.length < params[0]);
        if (!valida)
            return campo.Descricao + " deve ter no mínimo " + params[0] + " caractere(s).";

        return valida;
    },
    max_length: function (val, params, campo) {
        var valida = !(val.length > params[0]);
        if (!valida)
            return campo.Descricao + " deve ter no máximo " + params[0] + " caractere(s).";

        return valida;
    },
    tipo_dono: function (val, params, campo) {
        val = (val + "").toLocaleLowerCase();
        var valida = (val == "p" || val == "t");

        if (!valida)
            return campo.Descricao + " deve ser P (Próprio) ou T (Terceiro).";

        return valida;
    },
    tipo_veiculo: function (val, params, campo) {
        var valida = (val == "0" || val == "1");

        if (!valida)
            return campo.Descricao + " deve ser 0 (Tração) ou 1 (Reboque).";

        return valida;
    },
    tipo_rodado: function (val, params, campo) {
        var valida = (val == "00" || val == "01" || val == "02" || val == "03" || val == "04" || val == "05" || val == "06");

        if (!valida)
            return campo.Descricao + " deve ser 00 (Não Aplicado); 01 (Truck); 02 (Toco); 03 (Cavalo); 04 (Van); 05 (Utilitários); 06 (Outros).";

        return valida;
    },
    tipo_carroceria: function (val, params, campo) {
        var valida = (val == "00" || val == "01" || val == "02" || val == "03" || val == "04" || val == "05" || val == "06");

        if (!valida)
            return campo.Descricao + " deve ser 00 (Não Aplicado); 01 (Aberta); 02 (Fechada Baú); 03 (Granel); 04 (Porta Container); 05 (Sider).";

        return valida;
    },
    tipo_proprietario: function (val, params, campo) {
        var valida = (val == "0" || val == "1" || val == "2");

        if (!valida)
            return campo.Descricao + " deve ser 0 (TAC Agregado); 1 (TAC Independente); 2 (Outros).";

        return valida;
    },
};


window.regras = Regras;