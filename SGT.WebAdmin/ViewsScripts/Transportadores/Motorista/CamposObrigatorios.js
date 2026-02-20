var _CamposObrigatoriosUtilizados = [];

function SetarCamposObrigatoriosMotorista() {

    InformarObrigatoriedadeCamposMotorista(false)

    executarReST("MotoristaCampoObrigatorio/BuscarMotoristaCampoObrigatorioPadrao", {}, function (r) {
        if (r.Success && r.Data && r.Data.Campos) {
            _CamposObrigatoriosUtilizados = r.Data.Campos;
            InformarObrigatoriedadeCamposMotorista(true)
        }
    });
}

function InformarObrigatoriedadeCamposMotorista(obrigatorio) {
    if (_CamposObrigatoriosUtilizados == null || _CamposObrigatoriosUtilizados.length <= 0)
        return;

    for (var i = 0; i < _CamposObrigatoriosUtilizados.length; i++) {
        var campo = _CamposObrigatoriosUtilizados[i].Campo;

        var [nomeObjetoKnockout, nomePropriedade] = campo.split("."); // Separa o nome do objeto da propriedade (vêm junto do banco, na tabela T_MOTORISTA_CAMPO)

        if (!nomeObjetoKnockout || !nomePropriedade) {
            return;
        }

        if (window[nomeObjetoKnockout].hasOwnProperty(nomePropriedade)) {
            propriedade = window[nomeObjetoKnockout][nomePropriedade];

            propriedade.required = obrigatorio;
            if (obrigatorio) {
                propriedade.text("*" + propriedade.text().replace('*', ''))
            } else {
                propriedade.text(propriedade.text().replace('*', ''))
            }

            if (propriedade.options != null && propriedade.options.length > 0 && _motorista.Codigo.val() <= 0) {
                if (obrigatorio === true)
                    propriedade.val("");
                else
                    propriedade.val(propriedade.options[0].value);
            }
        }
    }
}