var _CamposObrigatoriosUtilizados = [];

function SetarCamposObrigatoriosPessoa() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        InformarObrigatoriedadeCamposPessoa(false)

        executarReST("PessoaCampoObrigatorio/BuscarParaPessoa", { Cliente: _pessoa.TipoCliente.val(), Fornecedor: _pessoa.TipoFornecedor.val(), Terceiro: _pessoa.TipoTransportador.val() }, function (r) {
            if (r.Success && r.Data && r.Data.Campos) {

                _CamposObrigatoriosUtilizados = r.Data.Campos;

                InformarObrigatoriedadeCamposPessoa(true)
            }
        });
    }
}

function InformarObrigatoriedadeCamposPessoa(obrigatorio) {
    if (_CamposObrigatoriosUtilizados == null || _CamposObrigatoriosUtilizados.length <= 0)
        return;

    for (var i = 0; i < _CamposObrigatoriosUtilizados.length; i++) {
        var campo = _CamposObrigatoriosUtilizados[i].Campo;

        var propriedade = null;

        if (_pessoa.hasOwnProperty(campo))
            propriedade = _pessoa[campo];

        if (propriedade == null && _pessoaAdicional.hasOwnProperty(campo))
            propriedade = _pessoaAdicional[campo];

        if (propriedade == null && _dadoBancario.hasOwnProperty(campo))
            propriedade = _dadoBancario[campo];

        if (propriedade == null && _transportadorTerceiro.hasOwnProperty(campo))
            propriedade = _transportadorTerceiro[campo];

        if (propriedade == null && _fornecedor.hasOwnProperty(campo))
            propriedade = _fornecedor[campo];

        if (propriedade == null && _configuracaoEmissaoCTe.Configuracao.hasOwnProperty(campo))
            propriedade = _configuracaoEmissaoCTe.Configuracao[campo];

        if (propriedade == null && _configuracaoFatura.Configuracao.hasOwnProperty(campo))
            propriedade = _configuracaoFatura.Configuracao[campo];

        if (propriedade != null) {
            propriedade.required = obrigatorio;

            if (propriedade.options != null && propriedade.options.length > 0 && _pessoa.Codigo.val() <= 0) {
                if (obrigatorio === true)
                    propriedade.val("");
                else
                    propriedade.val(propriedade.options[0].value);
            }
        }
    }
}