/// <reference path="../../Consultas/MotivoRejeicaoAuditoria.js" />
/// <reference path="../../../js/Global/Mensagem.js" />

var _auditarRegistroCanhoto;

var AuditarRegistroCanhoto = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Numero:", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() })
    this.Chave = PropertyEntity({ text: "Chave:", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.DataDigitalizacao = PropertyEntity({ text: "Data digitalização:", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.DataRecebimentoFisico = PropertyEntity({ text: "Data Recibimento físico:", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.DataAprovacao = PropertyEntity({ text: "Data aprovação:", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), visible: ko.observable(true), idGrid: guid() });

    this.MotivoRejeicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo Rejeição", idBtnSearch: guid(), required: ko.observable(false) });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarAuditoriaCanhoto, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Aprovar = PropertyEntity({ eventClick: aprovarAuditoriaCanhoto, type: types.event, text: "Aprovar", visible: ko.observable(true) });

}

//*******EVENTOS*******

function LoadAuditarRegistroCanhoto() {
    _auditarRegistroCanhoto = new AuditarRegistroCanhoto();
    KoBindings(_auditarRegistroCanhoto, "KnoutAuditarRegistro");

    new BuscarMotivoRejeicao(_auditarRegistroCanhoto.MotivoRejeicao, retornoBusquedaMotivo)
}

function AbrirModalAuditarRegistroCanhotoClick(Codigo) {
    LimparCampos(_auditarRegistroCanhoto);

    executarReST("Canhoto/BuscarDetalhesDoCanhotoParaAuditoria", { Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_auditarRegistroCanhoto, { Data: arg.Data })                
                Global.abrirModal('divModalAuditarRegistro');
            }
            if (arg.Data === false)
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        if (!arg.Success)
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function retornoBusquedaMotivo(data) {
    _auditarRegistroCanhoto.MotivoRejeicao.codEntity(data.Codigo);
    _auditarRegistroCanhoto.MotivoRejeicao.val(data.Descricao);
}

function aprovarAuditoriaCanhoto(e) {
    executarReST("Canhoto/AprovarAuditoria", { Codigo: e.Codigo.val(), Descricao: e.Descricao.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                Global.fecharModal('divModalAuditarRegistro');
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function rejeitarAuditoriaCanhoto(e) {
   
    const data = { Codigo: e.Codigo.val(), Descricao: e.Descricao.val(), MotivoRejeicao: e.MotivoRejeicao.codEntity() };
    executarReST("Canhoto/RejeitarAuditoria", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                Global.fecharModal('divModalAuditarRegistro');
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}