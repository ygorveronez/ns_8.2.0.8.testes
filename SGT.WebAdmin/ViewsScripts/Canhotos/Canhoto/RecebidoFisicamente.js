var _canhotoRecebidoFisicamente;

var CanhotoRecebidoFisicamente = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.ObservacaoRecebimentoFisico = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Canhotos.Canhoto.ObservacoesDoRecebimento.getFieldDescription(), maxlength: 500, required: false });
    this.ReceberFisicamente = PropertyEntity({ eventClick: EnviarCanhotoFisicoClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.RecebidoFisicamente, visible: ko.observable(true) });
    this.DataRecebimento = PropertyEntity({ getType: typesKnockout.date, text: "Data:", required: false });
    this.DataEntregaCliente = PropertyEntity({ getType: typesKnockout.date, text: "Data da Entrega no Cliente:", required: false, visible: ko.observable(_exigirDataEntregaNotaClienteCanhotosReceberFisicamente) });
    this.NumeroProtocolo = PropertyEntity({ val: ko.observable(0), text: "Protocolo:", def: 0, getType: typesKnockout.int, required: false });
}

//*******EVENTOS*******

function LoadCanhotoRecebidoFisicamente() {
    _canhotoRecebidoFisicamente = new CanhotoRecebidoFisicamente();
    KoBindings(_canhotoRecebidoFisicamente, "divModalReceberCanhotoFisicamente");
}

function AbrirModalCanhotoRecebidoFisicamenteClick(e) {
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhoto_PermiteConfirmarRecebimentoCanhotoFisico, _PermissoesPersonalizadasCanhotos)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaConfirmarRecebimentoCanhoto, 10000)
        return;
    }
    LimparCampos(_canhotoRecebidoFisicamente);

    _canhotoRecebidoFisicamente.Codigo.val(e.Codigo);
        
    Global.abrirModal('divModalReceberCanhotoFisicamente');
}

function EnviarCanhotoFisicoClick(e) {

    var dados = {
        Codigo: _canhotoRecebidoFisicamente.Codigo.val(),
        ObservacaoRecebimentoFisico: _canhotoRecebidoFisicamente.ObservacaoRecebimentoFisico.val(),
        DataRecebimento: _canhotoRecebidoFisicamente.DataRecebimento.val(),
        NumeroProtocolo: _canhotoRecebidoFisicamente.NumeroProtocolo.val(),
        DataEntregaCliente: _canhotoRecebidoFisicamente.DataEntregaCliente.val(),
    };

    executarReST("Canhoto/EnviarCanhotoFisico", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.CanhotoEnviadoComSucesso);
                Global.fecharModal('divModalReceberCanhotoFisicamente');
                buscarCanhotos();

                if (arg.Data.Mensagem != "")
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Data.Mensagem, 10000);
                else if (arg.Data.ExigeCanhotoParaFaturamento === true)
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Canhotos.Canhoto.TomadorExigeCanhotoParaFaturamento, Localization.Resources.Canhotos.Canhoto.TomadorDoCanhotoExigeMesmoParaRealizarFaturamento, 10000);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}