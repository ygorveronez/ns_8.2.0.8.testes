/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Empresa.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoCargaGuarita.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

var _guaritaFluxoEntrega;

var GuaritaFluxoEntrega = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.NumeroCarga = PropertyEntity({ text: "Carga:", val: ko.observable(""), def: "" });
    this.CargaData = PropertyEntity({ text: "Data:", val: ko.observable(""), def: "" });
    this.CargaHora = PropertyEntity({ text: "Hora:", val: ko.observable(""), def: "" });
    this.DescricaoSituacao = PropertyEntity({ text: "Situação:", val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: "Transportador:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ text: "Fornecedor:", val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: "Tipo da Operação:", val: ko.observable(""), def: "" });

    this.Auditar = PropertyEntity({ visible: ko.observable(false), eventClick: AuditarGuarita });
    
    this.DataInicioViagem = PropertyEntity({ val: ko.observable(""), def: "" });
    this.DataPrevisao = PropertyEntity({ text: "Data Previsão Carregamento:", val: ko.observable(""), def: "" });
    this.DataCarregamento = PropertyEntity({ text: "Data Previsão Chegada:", val: ko.observable(""), def: "" });
    this.Motorista = PropertyEntity({ text: "Motorista:", val: ko.observable(""), def: "" });
    this.MotoristaTelefone = PropertyEntity({ text: "Telefone:", val: ko.observable(""), def: "" });
    this.Veiculo = PropertyEntity({ text: "Veículo:", val: ko.observable(""), def: "" });
    this.CodigoIntegracaoDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: "Destinatário:", val: ko.observable(""), def: "" });

    this.InformarSaidaVeiculo = PropertyEntity({ eventClick: informarSaidaVeiculoClick, type: types.event, text: "Confirmar Saída", visible: ko.observable(false) });
}

function LoadGuaritaFluxoEntrega() {
    _guaritaFluxoEntrega = new GuaritaFluxoEntrega();
    KoBindings(_guaritaFluxoEntrega, "knockoutGuaritaFluxoEntrega");

    _guaritaFluxoEntrega.DataInicioViagem.val.subscribe(SetaDataHoraCarga);
}

function AuditarGuarita() {
    var _fn = OpcaoAuditoria("CargaJanelaCarregamentoGuarita", "Codigo", _guaritaFluxoEntrega);

    _fn({ Codigo: _guaritaFluxoEntrega.Codigo.val() });
}

function ExibirDetalhesSaidaGuaritaFluxoEntrega(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;
    LimparCampos(_guaritaFluxoEntrega);

    executarReST("Guarita/BuscarPorCarga", { Carga: knoutFluxo.Carga.val() }, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                PreencherObjetoKnout(_guaritaFluxoEntrega, r);

                var fluxoAberto = (_fluxoAtual.Situacao.val() == EnumSituacaoEtapaFluxoGestaoEntrega.Aguardando);

                SetaTituloGuarita(opt.text);
                _guaritaFluxoEntrega.InformarSaidaVeiculo.visible(false);
                _guaritaFluxoEntrega.DataPrevisao.val(_guaritaFluxoEntrega.DataCarregamento.val());


                if (_guaritaFluxoEntrega.Situacao.val() == EnumSituacaoCargaGuarita.AguardandoLiberacao) {
                    if (fluxoAberto) {
                        _guaritaFluxoEntrega.InformarSaidaVeiculo.visible(true);
                    }
                } /*else if (_guaritaFluxoEntrega.Situacao.val() == EnumSituacaoCargaGuarita.Liberada) {
                    if (fluxoAberto) {
                        _guaritaFluxoEntrega.InformarSaidaVeiculo.visible(true);
                    }
                }*/

                if (_fluxoAtual.CargaCancelada.val()) {
                    for (var i in _guaritaFluxoEntrega) {
                        if (_guaritaFluxoEntrega[i].type == types.event && $.isFunction(_guaritaFluxoEntrega[i].visible))
                            _guaritaFluxoEntrega[i].visible(false);
                    }
                }

                ExibeModalEtapa("#divModalGuaritaFluxoEntrega");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function informarSaidaVeiculoClick(e) {
    exibirConfirmacao("Confirmação", "Você tem certeza que deseja confirmar a saída?", function () {
        executarReST("Guarita/SaidaVeiculo", { Codigo: e.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Veículo liberado com sucesso!");
                    AtualizarFluxoEntrega();
                    Global.fecharModal('divModalGuaritaFluxoEntrega');

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function SetaTituloGuarita(titulo) {
    $("#guarita-titulo").text(titulo);
}

function SetaDataHoraCarga() {
    var dataHora = _guaritaFluxoEntrega.DataInicioViagem.val();

    var data = "";
    var hora = "";

    if (dataHora != null && dataHora != "") {
        var splittedTime = dataHora.split(" ");
        data = splittedTime[0] || "";
        hora = splittedTime[1] || "";
    }

    _guaritaFluxoEntrega.CargaData.val(data);
    _guaritaFluxoEntrega.CargaHora.val(hora);
}