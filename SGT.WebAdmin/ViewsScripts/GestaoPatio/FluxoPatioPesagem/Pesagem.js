/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="PesagemAnexo.js" />
/// <reference path="PesagemFinal.js" />
/// <reference path="PesagemIntegracoes.js" />

var _pesagem;
var _pesagemCRUD;
var _gridAnexoPesagem;
var _opcoesBalancaPesagem;

var _opcoesProdutorRural = [
    { text: "Não", value: "0" },
    { text: "Sim", value: "1" }
];

var Pesagem = function () {
    this.CodigoGuarita = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.CodigoPesagem = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });

    this.PesagemInicial = PropertyEntity({ text: ko.observable("*Pesagem Inicial:"), val: ko.observable(""), getType: typesKnockout.decimal, required: ko.observable(true), enable: ko.observable(true) });
    this.QuantidadeCaixas = PropertyEntity({ text: "*Quantidade de Caixas:", val: ko.observable(0), def: 0, getType: typesKnockout.int, configInt: { allowNegative: false, allowZero: true }, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Pressao = PropertyEntity({ text: ko.observable("Pressão:"), val: ko.observable(""), getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });
    this.ProdutorRural = PropertyEntity({ text: "*Produtor Rural:", val: ko.observable(""), options: _opcoesProdutorRural, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    this.Balanca = PropertyEntity({ text: "Balança:", val: ko.observable(null), options: ko.observable(_opcoesBalancaPesagem), def: null, visible: ko.observable(false) });
    this.GerarIntegracao = PropertyEntity({ eventClick: gerarAtualizarIntegracaoPesagemClick, type: types.event, text: "Gerar/Atualizar Integração", idGrid: guid(), visible: ko.observable(false) });
};

var PesagemCRUD = function () {
    this.SalvarPesagem = PropertyEntity({ eventClick: salvarPesagemClick, type: types.event, text: "Salvar Informações de Pesagem", visible: ko.observable(true) });
    this.CriarTicketBalanca = PropertyEntity({ eventClick: criarTicketBalancaClick, type: types.event, text: "Criar Ticket Balança", visible: ko.observable(false) });
};

function LoadPesagemFluxoPatio(callback) {
    $.get("Content/Static/GestaoPatio/FluxoPatioPesagem.html?dyn=" + guid(), function (data) {
        $("#ModaisPesagem").html(data);

        _pesagem = new Pesagem();
        KoBindings(_pesagem, "knockoutPesagem");

        _pesagemCRUD = new PesagemCRUD();
        KoBindings(_pesagemCRUD, "knockoutPesagemCRUD");

        LoadAnexoPesagem();
        loadPesagemDetalhes();
        loadPesagemIntegracoes();
        LoadPesagemFinal();
        LoadPesagemLoteInterno();

        if (callback instanceof Function)
            callback();
    });
}

function abrirPesagemFluxoPatioClick(codigo) {
    _pesagem.CodigoGuarita.val(codigo);

    executarReST("Guarita/BuscarInformacoesPesagemInicial", { Codigo: _pesagem.CodigoGuarita.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                PreencherObjetoKnout(_pesagem, r);
                _listaAnexoPesagem.Anexos.val(data.Anexos);

                _pesagemCRUD.CriarTicketBalanca.visible(false);

                if (!data.PesagemBalancaIniciada && !data.ExibirAnexos)
                    $("#container-lista-aba-pesagem-inicial").hide();
                else {
                    $("#container-lista-aba-pesagem-inicial").show();
                    $("#liTabPesagemInicialAnexo").removeClass("active");
                    $("#liTabPesagemInicialIntegracao").removeClass("active");
                    $("#tabPesagemInicialAnexo").removeClass("active in");
                    $("#tabPesagemInicialIntegracao").removeClass("active in");

                    if (data.ExibirAnexos)
                        $("#liTabPesagemInicialAnexo").show();
                    else
                        $("#liTabPesagemInicialAnexo").hide();

                    if (data.PesagemBalancaIniciada)
                        $("#liTabPesagemInicialIntegracao").show();
                    else
                        $("#liTabPesagemInicialIntegracao").hide();

                    if (data.ExibirAnexos) {
                        $("#liTabPesagemInicialAnexo").addClass("active");
                        $("#tabPesagemInicialAnexo").addClass("active in");
                    }
                    else {
                        $("#liTabPesagemInicialIntegracao").addClass("active");
                        $("#tabPesagemInicialIntegracao").addClass("active in");
                    }
                }

                if (!data.PesagemBalancaIniciada && data.IntegracaoToledo)
                    _pesagemCRUD.CriarTicketBalanca.visible(true);

                _pesagem.PesagemInicial.enable(data.PodeEditar);
                _pesagem.Pressao.enable(data.PodeEditar);
                _pesagem.Pressao.visible(data.ExibirPressao);
                _pesagem.ProdutorRural.enable(data.PodeEditar);
                _pesagem.ProdutorRural.visible(data.ExibirInformacoesProdutor);
                _pesagem.ProdutorRural.required(data.ExibirInformacoesProdutor);
                _pesagem.QuantidadeCaixas.enable(data.PodeEditar);
                _pesagem.QuantidadeCaixas.visible(data.ExibirQuantidadeCaixas);
                _pesagem.QuantidadeCaixas.required(data.ExibirQuantidadeCaixas);
                _pesagemCRUD.SalvarPesagem.visible(data.PodeEditar);

                preencherBalancasPesagemInicial(data);

                Global.abrirModal('divModalPesagem');

                $("#divModalPesagem").one('hidden.bs.modal', function () {
                    Global.ResetarAbas();
                    LimparCampos(_pesagem);
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function salvarPesagemClick() {
    executarReST("Guarita/ValidarRegrasPesagemInicial", { CodigoGuarita: _pesagem.CodigoGuarita.val(), PesagemInicial: _pesagem.PesagemInicial.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                if (!string.IsNullOrWhiteSpace(arg.Msg)) {
                    exibirConfirmacao("Confirmação", arg.Msg, function () {
                        salvarInformacoesPesagemClick();
                    });
                }
                else
                    salvarInformacoesPesagemClick();

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}

function salvarInformacoesPesagemClick() {
    Salvar(_pesagem, "Guarita/SalvarInformacoesPesagemInicial", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados da pesagem salvos.");
                enviarArquivosAnexadosPesagem(_pesagem.CodigoGuarita.val());
                Global.fecharModal('divModalPesagem');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}

function criarTicketBalancaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente criar o ticket com a balança?", function () {
        executarReST("Pesagem/IniciarPesagemBalanca", { CodigoGuarita: _pesagem.CodigoGuarita.val(), PesagemInicial: _pesagem.PesagemInicial.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _pesagem.CodigoPesagem.val(r.Data);
                    _pesagemCRUD.CriarTicketBalanca.visible(false);
                    $("#liTabPesagemInicialIntegracao").show();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ticket com a balança iniciada com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function gerarAtualizarIntegracaoPesagemClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente gerar/atualizar a integração da balança " + ($("#" + _pesagem.Balanca.id + "  option:selected").text()) + "?", function () {
        executarReST("Pesagem/GerarAtualizarPesagemBalanca", { CodigoGuarita: _pesagem.CodigoGuarita.val(), CodigoBalancaInicial: _pesagem.Balanca.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _pesagem.CodigoPesagem.val(r.Data);
                    $("#liTabPesagemInicialIntegracao").show();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Integração com a balança gerada/atualizada com sucesso.");
                    carregarIntegracoes();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function atualizarPesagemInicialPorEvento(peso) {
    _pesagem.PesagemInicial.val(peso);
    carregarIntegracoes();
}

function preencherBalancasPesagemInicial(data) {
    _opcoesBalancaPesagem = new Array();
    var possuiBalancaPadrao = false;

    for (var i = 0; i < data.Balancas.length; i++) {
        _opcoesBalancaPesagem.push({ value: data.Balancas[i].Codigo, text: data.Balancas[i].Descricao });

        if (data.Balanca.Codigo > 0 && data.Balanca.Codigo == data.Balancas[i].Codigo) {
            _pesagem.Balanca.val(_opcoesBalancaPesagem[i].value);
            possuiBalancaPadrao = true;
        }
    }

    _pesagem.Balanca.options(_opcoesBalancaPesagem);
    _pesagem.Balanca.visible(data.Balancas.length > 0 && data.PodeEditar);
    _pesagem.GerarIntegracao.visible(data.Balancas.length > 0 && data.PodeEditar);
    if (!possuiBalancaPadrao)
        _pesagem.Balanca.val(null);
}