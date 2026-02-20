/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumStatusBalanca.js" />
/// <reference path="PesagemFinalAnexo.js" />
/// <reference path="PesagemFinalIntegracoes.js" />
/// <reference path="PesagemAprovacao.js" />

var _pesagemFinal;
var _pesagemFinalCRUD;
var _opcoesBalancaPesagemFinal;

var PesagemFinal = function () {
    this.CodigoGuarita = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.CodigoPesagemIntegracao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.CodigoPesagem = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });

    this.PesagemFinal = PropertyEntity({ text: "*Pesagem Final:", val: ko.observable(""), getType: typesKnockout.decimal, required: true, enable: ko.observable(true) });
    this.PorcentagemPerda = PropertyEntity({ text: "*Porcentagem de Refugo:", val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 5, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Lacre = PropertyEntity({ text: "*Lacre:", val: ko.observable(""), getType: typesKnockout.string, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false), maxlength: 60 });
    this.LoteInterno = PropertyEntity({ text: "Lote Interno:", val: ko.observable(""), getType: typesKnockout.string, required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.ProblemaIntegracao = PropertyEntity({ text: "Motivo do problema na integração: ", visible: ko.observable(true) });

    this.Balanca = PropertyEntity({ text: "Balança:", val: ko.observable(null), options: ko.observable(_opcoesBalancaPesagem), def: null, visible: ko.observable(false) });
    this.GerarIntegracao = PropertyEntity({ eventClick: gerarAtualizarIntegracaoPesagemFinalClick, type: types.event, text: "Gerar/Atualizar Integração", idGrid: guid(), visible: ko.observable(false) });
};

var PesagemFinalCRUD = function () {
    this.SalvarPesagem = PropertyEntity({ eventClick: salvarPesagemFinalClick, type: types.event, text: "Salvar Informações de Pesagem", visible: ko.observable(true) });
    this.DesbloquearTicketBalanca = PropertyEntity({ eventClick: desbloquearTicketBalancaClick, type: types.event, text: "Desbloquear Ticket Balança", visible: ko.observable(false) });
    this.RefazerPesagemTicketBalanca = PropertyEntity({ eventClick: refazerPesagemTicketBalancaClick, type: types.event, text: "Refazer Pesagem Ticket Balança", visible: ko.observable(false) });
    this.RefazerConsultaPesagemBalanca = PropertyEntity({ eventClick: refazerConsultaPesagemBalancaClick, type: types.event, text: "Refazer Consulta da Pesagem Final na Balança", visible: ko.observable(false) });
};

function LoadPesagemFinal() {
    _pesagemFinal = new PesagemFinal();
    KoBindings(_pesagemFinal, "knockoutPesagemFinal");

    _pesagemFinalCRUD = new PesagemFinalCRUD();
    KoBindings(_pesagemFinalCRUD, "knockoutPesagemFinalCRUD");

    loadAnexoPesagemFinal();
    loadPesagemFinalIntegracoes();
    loadPesagemAprovacao();
}

function abrirPesagemFinalFluxoPatioClick(codigo) {
    _pesagemFinal.CodigoGuarita.val(codigo);

    executarReST("Guarita/BuscarInformacoesPesagemFinal", { Codigo: _pesagemFinal.CodigoGuarita.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                PreencherObjetoKnout(_pesagemFinal, r);

                _listaAnexoPesagemFinal.Anexos.val(data.Anexos);

                $("#liTabPesagemFinalAnexo").removeClass("active");
                $("#liTabPesagemFinalIntegracao").removeClass("active");
                $("#liTabPesagemFinalAprovacao").removeClass("active");
                $("#liTabPesagemFinalAnexo a").removeClass("active");
                $("#liTabPesagemFinalIntegracao a").removeClass("active");
                $("#liTabPesagemFinalAprovacao a").removeClass("active");
                $("#tabPesagemFinalAnexo").removeClass("active show");
                $("#tabPesagemFinalIntegracao").removeClass("active show");
                $("#tabPesagemFinalAprovacao").removeClass("active show");
                $("#liTabPesagemFinalAprovacao").hide();

                if (data.ExibirAnexos)
                    $("#liTabPesagemFinalAnexo").show();
                else
                    $("#liTabPesagemFinalAnexo").hide();

                if (data.CodigoPesagemIntegracao > 0)
                    $("#liTabPesagemFinalIntegracao").show();
                else
                    $("#liTabPesagemFinalIntegracao").hide();

                if (data.ExibirAnexos) {
                    $("#liTabPesagemFinalAnexo").addClass("active");
                    $("#liTabPesagemFinalAnexo a").addClass("active");
                    $("#tabPesagemFinalAnexo").addClass("active show");
                }
                else if (data.CodigoPesagemIntegracao > 0) {
                    $("#liTabPesagemFinalIntegracao").addClass("active");
                    $("#liTabPesagemFinalIntegracao a").addClass("active");
                    $("#tabPesagemFinalIntegracao").addClass("active show");
                }

                _pesagemFinalCRUD.DesbloquearTicketBalanca.visible(false);
                _pesagemFinalCRUD.RefazerPesagemTicketBalanca.visible(false);
                _pesagemFinalCRUD.RefazerConsultaPesagemBalanca.visible(false);
                _pesagemFinal.ProblemaIntegracao.visible(false);

                if (data.StatusBalanca === EnumStatusBalanca.TicketBloqueado)
                    _pesagemFinalCRUD.DesbloquearTicketBalanca.visible(true);
                else if (data.PermiteRefazerPesagem)
                    _pesagemFinalCRUD.RefazerPesagemTicketBalanca.visible(true);
                else if (data.StatusBalanca === EnumStatusBalanca.FalhaIntegracao && data.PermiteReenviarConsultaRejeitada) {
                    _pesagemFinalCRUD.RefazerConsultaPesagemBalanca.visible(true);
                    _pesagemFinal.ProblemaIntegracao.visible(true);
                }

                _pesagemFinal.Lacre.enable(data.PodeEditar);
                _pesagemFinal.Lacre.visible(data.ExibirLacre);
                _pesagemFinal.Lacre.required(data.ExibirLacre);
                _pesagemFinal.LoteInterno.visible(data.ExibirInformacoesLoteInterno);
                _pesagemFinal.PesagemFinal.enable(data.PodeEditar);
                _pesagemFinal.PorcentagemPerda.enable(data.PodeEditar);
                _pesagemFinal.PorcentagemPerda.visible(data.ExibirPercentualRefugo);
                _pesagemFinal.PorcentagemPerda.required(data.ExibirPercentualRefugo);

                _pesagemFinalCRUD.SalvarPesagem.visible(data.PodeEditar);

                preencherBalancasPesagemFinal(data);
                preencherPesagemAprovacao(_pesagemFinal.CodigoGuarita.val());

                Global.abrirModal('divModalPesagemFinal');

                $("#divModalPesagemFinal").one('hidden.bs.modal', function () {
                    LimparCampos(_pesagemFinal);
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function salvarPesagemFinalClick() {
    executarReST("Guarita/ValidarRegrasPesagemFinal", { CodigoGuarita: _pesagemFinal.CodigoGuarita.val(), PesagemFinal: _pesagemFinal.PesagemFinal.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                if (!string.IsNullOrWhiteSpace(arg.Msg)) {
                    exibirConfirmacao("Confirmação", arg.Msg, function () {
                        salvarInformacoesPesagemFinalClick();
                    });
                }
                else
                    salvarInformacoesPesagemFinalClick();

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}

function salvarInformacoesPesagemFinalClick() {
    Salvar(_pesagemFinal, "Guarita/SalvarInformacoesPesagemFinal", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados da pesagem salvos.");
                enviarArquivosAnexadosPesagemFinal(_pesagemFinal.CodigoGuarita.val());
                Global.fecharModal('divModalPesagemFinal');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function desbloquearTicketBalancaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente desbloquear a balança para a pesagem final?", function () {
        executarReST("Pesagem/DesbloquearPesagemBalanca", { CodigoGuarita: _pesagemFinal.CodigoGuarita.val(), PesagemFinal: _pesagemFinal.PesagemFinal.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _pesagemFinalCRUD.DesbloquearTicketBalanca.visible(false);
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ticket para uso da balança desbloqueado com sucesso.");
                    Global.fecharModal('divModalPesagemFinal');
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function refazerPesagemTicketBalancaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente refazer a última operação na balança? <br>Obs: o retorno da integração da balança pode demorar, terá que aguardar!", function () {
        executarReST("Pesagem/RefazerPesagemBalanca", { CodigoGuarita: _pesagemFinal.CodigoGuarita.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Operação de refazer aplicada com sucesso.");
                    Global.fecharModal('divModalPesagemFinal');
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function refazerConsultaPesagemBalancaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente refazer a consulta da pesagem na balança?", function () {
        executarReST("Pesagem/Integrar", { Codigo: _pesagemFinal.CodigoPesagemIntegracao.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Operação solicitada com sucesso! Aguarde o sistema processar a consulta.");
                    Global.fecharModal('divModalPesagemFinal');
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function gerarAtualizarIntegracaoPesagemFinalClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente gerar/atualizar a integração da balança " + ($("#" + _pesagemFinal.Balanca.id + "  option:selected").text()) + "?", function () {
        executarReST("Pesagem/GerarAtualizarPesagemBalanca", { CodigoGuarita: _pesagemFinal.CodigoGuarita.val(), CodigoBalancaFinal: _pesagemFinal.Balanca.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    $("#liTabPesagemFinalIntegracao").show();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Integração com a balança gerada/atualizada com sucesso.");
                    carregarPesagemFinalIntegracoes();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function preencherBalancasPesagemFinal(data) {
    _opcoesBalancaPesagemFinal = new Array();
    var possuiBalancaPadrao = false;

    for (var i = 0; i < data.Balancas.length; i++) {
        _opcoesBalancaPesagemFinal.push({ value: data.Balancas[i].Codigo, text: data.Balancas[i].Descricao });

        if (data.Balanca.Codigo > 0 && data.Balanca.Codigo == data.Balancas[i].Codigo) {
            _pesagemFinal.Balanca.val(_opcoesBalancaPesagemFinal[i].value);
            possuiBalancaPadrao = true;
        }
    }

    _pesagemFinal.Balanca.options(_opcoesBalancaPesagemFinal);
    _pesagemFinal.Balanca.visible(data.Balancas.length > 0 && data.PodeEditar);
    _pesagemFinal.GerarIntegracao.visible(data.Balancas.length > 0 && data.PodeEditar);
    if (!possuiBalancaPadrao)
        _pesagemFinal.Balanca.val(null);
}

function atualizarPesagemFinalPorEvento(peso) {
    _pesagemFinal.PesagemFinal.val(peso);
    carregarPesagemFinalIntegracoes();
}