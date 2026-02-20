/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDigitalizacaoCanhoto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCanhoto.js" />
/// <reference path="../../Enumeradores/EnumTipoCanhoto.js" />

var _knoutDetalhesCanhoto;

var DetalhesCanhoto = function () {
    this.NotasAvuso = PropertyEntity({ text: "Notas Canhoto Avulso: ", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.Chave = PropertyEntity({ text: "Chave de Acesso: ", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });
    this.Numero = PropertyEntity({ text: "Número:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DescricaoTipoCanhoto = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "Data Emissão:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: "Destinatário: ", val: ko.observable(""), def: "", visible: ko.observable(true), idGrid: guid() });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCanhoto.Todas), def: EnumSituacaoCanhoto.Todas, visible: ko.observable(true) });
    this.SituacaoDigitalizacaoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoDigitalizacaoCanhoto.Todas), def: EnumSituacaoDigitalizacaoCanhoto.Todas, visible: ko.observable(true) });

    this.DescricaoSituacao = PropertyEntity({ text: "Situação do Canhoto:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DescricaoDigitalizacao = PropertyEntity({ text: "Situação Digitalização:", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ text: "Empresa: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "Valor: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Peso = PropertyEntity({ text: "Peso: ", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.NaturezaOP = PropertyEntity({ text: "Natureza da Operação: ", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DescricaoModalidadeFrete = PropertyEntity({ text: "Tipo de Pagamento: ", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Carga = PropertyEntity({ text: "Carga: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Motoristas = PropertyEntity({ text: "Motoristas: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ text: "Emitente: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ text: "Local de Armazenamento do Canhoto: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ text: "Justificativa:", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.MotivoRejeicaoDigitalizacao = PropertyEntity({ text: "Motivo da Rejeição da Imagem:", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.TipoCanhoto = PropertyEntity({ val: ko.observable(EnumTipoCanhoto.Todos), def: EnumTipoCanhoto.Todos, visible: ko.observable(true) });
    this.Auditar = PropertyEntity({ eventClick: auditarCanhotoClick, type: types.event, text: "Auditar", visible: ko.observable(false) });

    this.ObservacaoRecebimentoFisico = PropertyEntity({ text: "Observação do Recebimento Físico:", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DataRecebimento = PropertyEntity({ text: "Data de Recebimento", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.NumeroProtocolo = PropertyEntity({ text: "Nº Protocolo", val: ko.observable(""), def: "", visible: ko.observable(false) });

    this.BuscarCoordenadas = PropertyEntity({ visible: false });
    this.PrecisaoCoordenadas = PropertyEntity({ visible: false });
}

function loadDetalhesCanhoto() {
    _knoutDetalhesCanhoto = new DetalhesCanhoto();
    KoBindings(_knoutDetalhesCanhoto, "KnoutDetalhesCanhoto");
}


function auditarCanhotoClick(e, sender) {

}

function BuscarDetalhesCanhoto(codigo, callback) {
    var dados = {
        Codigo: codigo
    }
    executarReST("Canhoto/BuscarDetalhesCanhoto", dados, function (arg) {
        if (arg.Success) {
            var retorno = { Data: arg.Data };
            PreencherObjetoKnout(_knoutDetalhesCanhoto, retorno);

            if (arg.Data.LocalArmazenamento == "")
                _knoutDetalhesCanhoto.LocalArmazenamento.visible(false);
            else {
                var strLocal = arg.Data.LocalArmazenamento;
                _knoutDetalhesCanhoto.LocalArmazenamento.visible(true);
                if (arg.Data.PacoteArmazenado > 0)
                    strLocal += " no pacote " + arg.Data.PacoteArmazenado + " na posição " + arg.Data.PosicaoNoPacote;
                _knoutDetalhesCanhoto.LocalArmazenamento.val(strLocal);
            }

            if (_knoutDetalhesCanhoto.Situacao.val() == EnumSituacaoCanhoto.Justificado)
                _knoutDetalhesCanhoto.Justificativa.visible(true);

            if (_knoutDetalhesCanhoto.SituacaoDigitalizacaoCanhoto.val() == EnumSituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada)
                _knoutDetalhesCanhoto.MotivoRejeicaoDigitalizacao.visible(true);


            if (_knoutDetalhesCanhoto.TipoCanhoto.val() == EnumTipoCanhoto.NFe) {
                _knoutDetalhesCanhoto.Chave.visible(true);
                _knoutDetalhesCanhoto.NotasAvuso.visible(false);
                _knoutDetalhesCanhoto.NaturezaOP.visible(true);
            } else if (_knoutDetalhesCanhoto.TipoCanhoto.val() == EnumTipoCanhoto.Avulso) {
                _knoutDetalhesCanhoto.Chave.visible(false);
                _knoutDetalhesCanhoto.NotasAvuso.visible(true);
            }


            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
                _knoutDetalhesCanhoto.LocalArmazenamento.visible(false);
                _knoutDetalhesCanhoto.Empresa.visible(false);
            }

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                _knoutDetalhesCanhoto.Empresa.visible(false);
                _knoutDetalhesCanhoto.Filial.visible(false);
            }

            if (callback != null)
                callback();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    })
}