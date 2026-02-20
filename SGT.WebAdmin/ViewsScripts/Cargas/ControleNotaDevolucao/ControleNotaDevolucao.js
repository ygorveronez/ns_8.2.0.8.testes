/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumStatusControleNotaDevolucao.js" />
/// <reference path="../../../ViewsScripts/Chamados/Chamado/Chamado.js" />
/// <reference path="../../../ViewsScripts/Consultas/Cliente.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridControleNotaDevolucao;
var _pesquisaControleNotaDevolucao;
var _notaDevolucao;
var _motivoRejeitar;
var _informarChave;

var PesquisaControleNotaDevolucao = function () {
    this.Chave = PropertyEntity({ text: "Chave: " });
    this.NumeroCarga = PropertyEntity({ text: "Número Carga: " });
    this.NumeroChamado = PropertyEntity({ text: "Número Atendimento: ", getType: typesKnockout.int });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date });
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;

    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(EnumStatusControleNotaDevolucao.Todos), options: EnumStatusControleNotaDevolucao.obterOpcoesPesquisa(), def: EnumStatusControleNotaDevolucao.Todos });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleNotaDevolucao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

var NotaDevolucao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoChamado = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Chave = PropertyEntity({ text: "Chave: ", visible: ko.observable(true), val: ko.observable("") });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true), val: ko.observable("") });
    this.Serie = PropertyEntity({ text: "Série: ", visible: ko.observable(true), val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: "Destinatário: ", visible: ko.observable(true), val: ko.observable("") });

    this.Motivo = PropertyEntity({ text: "Motivo: ", visible: ko.observable(true), val: ko.observable("") });
    this.Carga = PropertyEntity({ text: "Carga: ", visible: ko.observable(true), val: ko.observable("") });
    this.Atendimento = PropertyEntity({ text: "Atendimento: ", visible: ko.observable(true), val: ko.observable("") });
    this.NotaOrigem = PropertyEntity({ text: "NF-e Origem: ", visible: ko.observable(true), val: ko.observable("") });
    this.ObservacaoMotorista = PropertyEntity({ text: "Observação Motorista: ", visible: ko.observable(true), val: ko.observable("") });

    //Botões
    this.DownloadDanfe = PropertyEntity({ text: "Download DANFE Destinados", eventClick: downloadDanfeClick, visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ text: "Rejeitar", eventClick: rejeitarNotaClick, visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ text: "Confirmar", eventClick: confirmarNotaClick, visible: ko.observable(true) });
    this.VerDetalhesChamado = PropertyEntity({ eventClick: verDetalhesChamadoClick, type: types.event, text: "Ver Detalhes" });
};

var MotivoRejeitar = function () {
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ text: "*Motivo:", maxlength: 2000, required: true });

    this.Rejeitar = PropertyEntity({
        eventClick: function (e) {
            motivoRejeitarNotaClick(e);
        }, type: types.event, text: "Rejeitar", idGrid: guid(), visible: ko.observable(true)
    });
};

var InformarChave = function () {
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int });
    this.Chave = PropertyEntity({ text: "*Chave:", maxlength: 44, required: true });

    this.Confirmar = PropertyEntity({
        eventClick: function (e) {
            informarChaveNotaClick(e);
        }, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadControleNotaDevolucao() {
    _pesquisaControleNotaDevolucao = new PesquisaControleNotaDevolucao();
    KoBindings(_pesquisaControleNotaDevolucao, "knockoutPesquisaControleNotaDevolucao", false, _pesquisaControleNotaDevolucao.Pesquisar.id);

    _notaDevolucao = new NotaDevolucao();
    KoBindings(_notaDevolucao, "knockoutNotaDevolucao");

    HeaderAuditoria("ControleNotaDevolucao", _notaDevolucao);

    _motivoRejeitar = new MotivoRejeitar();
    KoBindings(_motivoRejeitar, "divModalMotivoRejeitar");

    _informarChave = new InformarChave();
    KoBindings(_informarChave, "divModalInformarChave");

    new BuscarClientes(_pesquisaControleNotaDevolucao.Destinatario);

    loadChamado(false);

    buscarControleNotaDevolucao();
}

function downloadDanfeClick(e) {
    var dados = {
        Chave: e.Chave.val()
    };

    executarDownload("ControleNotaDevolucao/DownloadDANFENFeDestinados", dados);
}

function rejeitarNotaClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar a nota?", function () {
        _motivoRejeitar.Codigo.val(e.Codigo.val());
        Global.abrirModal('divModalMotivoRejeitar');
    });
}

function confirmarNotaClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja confirmar a nota?", function () {
        executarReST("ControleNotaDevolucao/ConfirmarNotaDevolucao", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");
                    Global.fecharModal('divModalNotaDevolucao');
                    _gridControleNotaDevolucao.CarregarGrid();
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function motivoRejeitarNotaClick(e) {
    var valido = ValidarCamposObrigatorios(e);
    if (valido) {
        executarReST("ControleNotaDevolucao/RejeitarNotaDevolucao", { Codigo: e.Codigo.val(), Motivo: e.Motivo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Rejeitado com sucesso");
                    Global.fecharModal('divModalMotivoRejeitar');
                    Global.fecharModal('divModalNotaDevolucao');
                    _gridControleNotaDevolucao.CarregarGrid();
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function informarChaveNotaClick(e) {
    var valido = ValidarCamposObrigatorios(e);
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }
    else if (!ValidarChaveAcesso(e.Chave.val())) {
        exibirMensagem(tipoMensagem.atencao, "Campos Inválidos", "Informe uma chave de NFe válida!");
        return;
    }

    executarReST("ControleNotaDevolucao/InformarChaveNotaDevolucao", { Codigo: e.Codigo.val(), Chave: e.Chave.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Chave infomada com sucesso");
                Global.fecharModal('divModalInformarChave');
                _gridControleNotaDevolucao.CarregarGrid();
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

}

function verDetalhesChamadoClick(e) {
    buscarChamadoPorCodigo(e.CodigoChamado.val(), function () {
        $modalChamado = $("#divModalChamadoOcorrencia");

        $modalChamado.modal("show");
        $modalChamado.one('hidden.bs.modal', function () {
            $(window).one('keyup', function (e) {
                if (e.keyCode == 27)
                    Global.fecharModal('divModalNotaDevolucao');
            });
        });

        $(window).unbind('keyup');
        $(window).one('keyup', function (e) {
            if (e.keyCode == 27)
                $modalChamado.modal('hide');
        });
    });
}

//*******MÉTODOS*******

function buscarControleNotaDevolucao() {
    var detalhes = { descricao: "Detalhes", id: guid(), metodo: detalharNotaDevolucao, icone: "" };
    var baixarImagem = { descricao: "Baixar Imagem NF-e Devolução", id: guid(), metodo: baixarImagemNotaDevolucaoClick, icone: "", visibilidade: visibilidadeBaixarImagem };
    var informarChave = { descricao: "Informar Chave", id: guid(), metodo: informarChaveClick, icone: "", visibilidade: visibilidadeInformarChave };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [detalhes, baixarImagem, informarChave] };

    _gridControleNotaDevolucao = new GridView(_pesquisaControleNotaDevolucao.Pesquisar.idGrid, "ControleNotaDevolucao/Pesquisa", _pesquisaControleNotaDevolucao, menuOpcoes);
    _gridControleNotaDevolucao.CarregarGrid();
}

function visibilidadeBaixarImagem(data) {
    return data.PossuiImagem;
}

function visibilidadeInformarChave(data) {
    return data.Status === EnumStatusControleNotaDevolucao.AgNotaFiscal;
}

function baixarImagemNotaDevolucaoClick(e) {
    var data = { Codigo: e.CargaEntregaNFeDevolucao };
    executarDownload("ChamadoAnalise/DownloadImagemNFDevolucao", data);
}

function informarChaveClick(e) {
    _informarChave.Codigo.val(e.Codigo);
    Global.abrirModal('divModalInformarChave');
}

function detalharNotaDevolucao(e) {
    _notaDevolucao.Codigo.val(e.Codigo);

    BuscarPorCodigo(_notaDevolucao, "ControleNotaDevolucao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var status = arg.Data.Status;
                _notaDevolucao.DownloadDanfe.visible(false);
                _notaDevolucao.Rejeitar.visible(false);
                _notaDevolucao.Confirmar.visible(false);
                if (status === EnumStatusControleNotaDevolucao.Conferido || status === EnumStatusControleNotaDevolucao.ComNotaFiscal)
                    _notaDevolucao.DownloadDanfe.visible(true);
                if (status === EnumStatusControleNotaDevolucao.AgNotaFiscal || status === EnumStatusControleNotaDevolucao.ComNotaFiscal || status === EnumStatusControleNotaDevolucao.ComChaveNotaFiscal)
                    _notaDevolucao.Rejeitar.visible(true);
                if (status === EnumStatusControleNotaDevolucao.ComNotaFiscal)
                    _notaDevolucao.Confirmar.visible(true);

                Global.abrirModal('divModalNotaDevolucao');
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}