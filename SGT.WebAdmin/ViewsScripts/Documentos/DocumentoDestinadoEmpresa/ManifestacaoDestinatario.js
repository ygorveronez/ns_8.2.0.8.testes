/// <reference path="DocumentoDestinadoEmpresa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoManifestacaoDestinatario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _situacaoMDeManifestacao = [
    { text: "Ciência da operação", value: EnumSituacaoManifestacaoDestinatario.CienciaOperacao },
    { text: "Confirmada a operação", value: EnumSituacaoManifestacaoDestinatario.ConfirmadaOperacao },
    { text: "Desconhecida", value: EnumSituacaoManifestacaoDestinatario.Desconhecida },
    { text: "Operação não realizada", value: EnumSituacaoManifestacaoDestinatario.OperacaoNaoRealizada }
];

var _situacaoEventoDesacordo = [
    { text: "Desacordo", value: EnumSituacaoManifestacaoDestinatario.DescordoServico }
];

var _gridManifestacaoDocumento;
var _manifestacaoDestinatario;
var _eventoDesacordo;
var _modalEmissaoManifestacaoDestinatario;
var _modalEmissaoDesacordo;

var ManifestacaoDestinatario = function () {
    this.TipoManifestacao = PropertyEntity({ text: "*Tipo de Manifestação:", val: ko.observable(EnumSituacaoManifestacaoDestinatario.CienciaOperacao.toString()), options: _situacaoMDeManifestacao, def: EnumSituacaoManifestacaoDestinatario.CienciaOperacao.toString() });
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", val: ko.observable(""), maxlength: 255, def: "", visible: ko.observable(false) });
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int });

    this.EmitirManifestacao = PropertyEntity({
        eventClick: function (e) {
            EmitirManifestacao();
        }, type: types.event, text: "Emitir Manifestação", idGrid: guid(), visible: ko.observable(true)
    });

    this.TipoManifestacao.val.subscribe(function (novoValor) {
        if (novoValor == EnumSituacaoManifestacaoDestinatario.OperacaoNaoRealizada || novoValor == EnumSituacaoManifestacaoDestinatario.Desconhecida)
            _manifestacaoDestinatario.Justificativa.visible(true);
        else
            _manifestacaoDestinatario.Justificativa.visible(false);
    });
};

var EventoDesacordo = function () {
    this.TipoManifestacao = PropertyEntity({ text: "*Tipo de Manifestação:", val: ko.observable(EnumSituacaoManifestacaoDestinatario.DescordoServico.toString()), options: _situacaoEventoDesacordo, def: EnumSituacaoManifestacaoDestinatario.DescordoServico.toString() });
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", val: ko.observable(""), maxlength: 255, def: "", visible: ko.observable(true), required: true });
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.ExisteMotivoDesacordo), required: ko.observable(_CONFIGURACAO_TMS.ExisteMotivoDesacordo) });

    this.EmitirDesacordo = PropertyEntity({
        eventClick: function (e) {
            EmitirDesacordo();
        }, type: types.event, text: "Emitir Desacordo", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadManifestacaoDestinatario() {
    _manifestacaoDestinatario = new ManifestacaoDestinatario();
    KoBindings(_manifestacaoDestinatario, "divModalEmissaoManifestacaoDestinatario");

    _eventoDesacordo = new EventoDesacordo();
    KoBindings(_eventoDesacordo, "divModalEmissaoDesacordo");

    new BuscarMotivoDesacordo(_eventoDesacordo.Motivo);

    _modalEmissaoManifestacaoDestinatario = new bootstrap.Modal(document.getElementById("divModalEmissaoManifestacaoDestinatario"), { backdrop: true, keyboard: true });
}

function EmitirManifestacao() {
    if ((_manifestacaoDestinatario.TipoManifestacao.val() == EnumSituacaoManifestacaoDestinatario.OperacaoNaoRealizada || _manifestacaoDestinatario.TipoManifestacao.val() == EnumSituacaoManifestacaoDestinatario.Desconhecida) && _manifestacaoDestinatario.Justificativa.val().length < 20) {
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "A justificativa deve ter entre 20 e 255 caracteres.");
        return;
    }

    CarregarDocumentos();

    if (_manifestacaoDestinatario.Codigo.val() > 0) {
        var dados = RetornarObjetoPesquisa(_manifestacaoDestinatario);

        executarReST("DocumentoDestinadoEmpresa/EmitirManifestacaoDestinatario", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Manifestação do destinatário emitida com sucesso!");
                    _gridDocumentoDestinadoEmpresa.CarregarGrid();

                    _modalEmissaoManifestacaoDestinatario.hide();
                    LimparCampos(_manifestacaoDestinatario);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    } else {
        if (!string.IsNullOrWhiteSpace(_pesquisaDocumentoDestinadoEmpresa.ListaDocumentos.val())) {
            exibirConfirmacao("Atenção!", "A emissão pode demorar alguns minutos, de acordo com a quantidade de NF-es constantes na consulta. Deseja prosseguir?", function () {
                var dados = RetornarObjetoPesquisa(_pesquisaDocumentoDestinadoEmpresa);
                dados["TipoManifestacao"] = _manifestacaoDestinatario.TipoManifestacao.val();
                dados["Justificativa"] = _manifestacaoDestinatario.Justificativa.val();

                executarReST("DocumentoDestinadoEmpresa/EmitirManifestacaoDestinatarioLote", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Manifestação do destinatário emitida com sucesso!");
                            _gridDocumentoDestinadoEmpresa.CarregarGrid();

                            _modalEmissaoManifestacaoDestinatario.hide();
                            LimparCampos(_manifestacaoDestinatario);
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário selecionar ao menos uma nota para gerar os documentos de entrada.");
            return;
        }
    }
}


function EmitirDesacordo() {
    if (_eventoDesacordo.Justificativa.val().length < 20) {
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "A justificativa deve ter entre 20 e 255 caracteres.");
        return;
    }
    if (_eventoDesacordo.Motivo.required() && _eventoDesacordo.Motivo.codEntity() <= 0){
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "É obrigatório adicionar um Motivo.");
        return;
    }

    CarregarDocumentos();

    if (_eventoDesacordo.Codigo.val() > 0) {
        var dados = RetornarObjetoPesquisa(_eventoDesacordo);

        executarReST("DocumentoDestinadoEmpresa/EmitirDesacordoCTe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg != null ? arg.Msg : "Desacordo emitido com sucesso!");
                    _gridDocumentoDestinadoEmpresa.CarregarGrid();

                    _modalEmissaoDesacordo.hide();
                    LimparCampos(_eventoDesacordo);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
                    _modalEmissaoDesacordo.hide();
                    LimparCampos(_eventoDesacordo);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    } else {
        if (!string.IsNullOrWhiteSpace(_pesquisaDocumentoDestinadoEmpresa.ListaDocumentos.val())) {
            exibirConfirmacao("Atenção!", "A emissão pode demorar alguns minutos, de acordo com a quantidade de CT-es constantes na consulta. Deseja prosseguir?", function () {
                var dados = RetornarObjetoPesquisa(_pesquisaDocumentoDestinadoEmpresa);
                dados["TipoManifestacao"] = _eventoDesacordo.TipoManifestacao.val();
                dados["Justificativa"] = _eventoDesacordo.Justificativa.val();
                dados["Motivo"] = _eventoDesacordo.Motivo.codEntity();
                console.log(dados)
                console.log(_eventoDesacordo.Motivo.val())

                executarReST("DocumentoDestinadoEmpresa/EmitirDesacordoCTeLote", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg != null ? arg.Msg : "Desacordo emitido com sucesso!");
                            _gridDocumentoDestinadoEmpresa.CarregarGrid();

                            _modalEmissaoDesacordo.hide();
                            LimparCampos(_eventoDesacordo);
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
                            _modalEmissaoDesacordo.hide();
                            LimparCampos(_eventoDesacordo);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário selecionar ao menos um CTe para Emitir o Desacordo.");
            return;
        }
    }
}

//********METODOS*********

function AbrirTelaEmissaoManifestacaoDestinatario() {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoManifestacao, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Você não possui permissão para gerar manifestações!");
        return;
    }

    _modalEmissaoManifestacaoDestinatario.show();
}

function AbrirTelaEmissaoDesacordo() {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoDestinado_PermiteEmitirDesacordo, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Você não possui permissão para Emitir Desacordo");
        return;
    }

    _modalEmissaoDesacordo.show();
}