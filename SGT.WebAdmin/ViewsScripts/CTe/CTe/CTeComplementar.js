var CTeComplementar = function (cte) {

    var instancia = this;

    this.ChaveCTeComplementado = PropertyEntity({ text: "*Chave do CT-e a ser Complementado:", maxlength: 44, required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutCTeComplementar);

        $("#" + instancia.ChaveCTeComplementado.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });
    };

    this.Validar = function () {
        if (cte.CTe.Tipo.val() === EnumTipoCTe.Complementar) {
            var valido = ValidarCamposObrigatorios(instancia);

            if (!valido) {
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios!", "Verifique os campos obrigatórios!");
            } else if (instancia.ChaveCTeComplementado.val().trim().replace(/\s/g, "").length !== 44) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "CT-e Complementar", "Verifique a chave do CT-e a ser complementado, ela deve possuir 44 dígitos.");
            }

            if (!valido)
                $('a[href="#divCTeOutros_' + cte.IdModal + '"]').tab("show");

            return valido;
        }
        else
            return true;
    };

    this.DesativarCTeComplementar = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };
};