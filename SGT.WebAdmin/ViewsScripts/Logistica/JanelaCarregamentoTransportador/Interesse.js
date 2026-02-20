/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumMoedaCotacaoBancoCentral.js" />
/// <reference path="../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="InteresseCargaComponenteFrete.js" />
/// <reference path="JanelaCarregamentoTransportador.js" />

// #region Objetos Globais do Arquivo

var _informarValorFrete;
var _interesseHorarioDiferente;
var _informarInteresseDadosTransporte;
// #endregion Objetos Globais do Arquivo

// #region Classes

var InteresseCarga = function (carga) {
    this.Grid = undefined;
    this.ComponentesFrete = PropertyEntity({ type: types.local, val: ko.observableArray([]), def: [] });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoCargaEmbarcador = PropertyEntity({});
    this.CobrarOutroDocumento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.Moeda = PropertyEntity({});
    this.TipoFreteEscolhido = PropertyEntity({});
    this.ValorCotacaoMoeda = PropertyEntity({ getType: typesKnockout.decimal });
    this.ValorFrete = PropertyEntity({ text: "Valor do Frete:", getType: typesKnockout.decimal });
    this.MenorLance = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false), text: "Menor Lance: " });

    this.AdicionarComponenteFrete = PropertyEntity({ eventClick: adicionarInteresseCargaComponenteFrete, type: types.event, text: "Adicionar", idGrid: guid() });

    PreencherObjetoKnout(this, { Data: carga });
}

var InteresseDadosTransporte = function (carga) {
    this.VeiculoInteresse = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.Veiculo.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), required: true, cssClass: ko.observable("col col-xs-12") });
    this.MotoristaInteresse = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), label: "Motorista:", val: ko.observable(""), def: "", enable: ko.observable(true), required: true });

    /* Campos para Cotação */
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Cargas = PropertyEntity({ type: types.local, val: ko.observableArray([]), def: [] });
    this.ExigirConfirmacaoTracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo (Carreta): "), idBtnSearch: guid(), visible: ko.observable(false), required: false });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo (Carreta 2):"), idBtnSearch: guid(), visible: ko.observable(false), required: false });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable(""), visible: false });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo: "), idBtnSearch: guid(), visible: ko.observable(false), required: false });
    this.ExigirConfirmacaoTracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.SalvarDadosTransporte = PropertyEntity({ eventClick: function (e, sender) { confirmarSalvarDadosTransporteClick(e, sender); }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Confirmar, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    /* Campos para Cotação */

    PreencherObjetoKnout(this, { Data: carga });
}
var InformarValorFrete = function () {
    this.Cargas = PropertyEntity({ type: types.local, val: ko.observableArray([]), def: [] });
    this.MensagemAlerta = PropertyEntity({});

    this.Atualizar = PropertyEntity({ eventClick: atualizarValorFreteInformadoClick, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
}

var InteresseHorarioDiferente = function () {
    this.Cargas = PropertyEntity({ type: types.local, val: ko.observableArray([]), def: [] });
    this.CarregamentoProximoDia = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), text: ko.observable("Posso carregar no próximo dia.") });
    this.HorarioCarregamento = PropertyEntity({ text: "Horário de Carregamento:", getType: typesKnockout.time, idTab: guid(), visible: ko.observable(true) });
    this.ValorFreteTransportador = PropertyEntity({ text: "Valor frete:", getType: typesKnockout.decimal, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.MensagemRetorno = PropertyEntity({ });

    /* Campos para Cotação */
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.ExigirConfirmacaoTracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo (Carreta): "), idBtnSearch: guid(), visible: ko.observable(false), required: false });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo (Carreta 2):"), idBtnSearch: guid(), visible: ko.observable(false), required: false });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable(""), visible: false });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo: "), idBtnSearch: guid(), visible: ko.observable(false), required: false });
    /* Campos para Cotação */

    this.SalvarInteresse = PropertyEntity({ eventClick: function (e, sender) { salvarInteresseHorarioDiferenteCargaClick(e, sender); }, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadInteresseHorarioDiferente() {
    _interesseHorarioDiferente = new InteresseHorarioDiferente();
    KoBindings(_interesseHorarioDiferente, "divModalSolicitarHorarioCarregamentoDiferente");

    _informarValorFrete = new InformarValorFrete();
    KoBindings(_informarValorFrete, "divModalInformarValorFrete");

    var somenteDisponiveis = !_CONFIGURACAO_TMS.NaoExigeInformarDisponibilidadeDeVeiculo;

    new BuscarVeiculos(_interesseHorarioDiferente.Veiculo, retornoConsultaVeiculoInteresseHorarioDiferente, _interesseHorarioDiferente.Transportador, _interesseHorarioDiferente.ModeloVeiculo, null, true, null, _interesseHorarioDiferente.TipoCarga, true, somenteDisponiveis, _interesseHorarioDiferente.CodigoCarga, _interesseHorarioDiferente.TipoVeiculo);
    new BuscarVeiculos(_interesseHorarioDiferente.Reboque, retornoConsultaReboqueInteresseHorarioDiferente, _interesseHorarioDiferente.Transportador, null, null, true, null, null, true, somenteDisponiveis, _interesseHorarioDiferente.CodigoCarga, "1");
    new BuscarVeiculos(_interesseHorarioDiferente.SegundoReboque, retornoConsultaSegundoReboqueInteresseHorarioDiferente, _interesseHorarioDiferente.Transportador, null, null, true, null, null, true, somenteDisponiveis, _interesseHorarioDiferente.CodigoCarga, "1");

    //$("#" + _interesseHorarioDiferente.HorarioCarregamento.id).datetimepicker({
    //    locale: 'pt-br',
    //    useCurrent: false,
    //    timeFormat: 'HH:mm'
    //}).on('dp.show', function () {
    //    $('a.btn[data-action="incrementMinutes"], a.btn[data-action="decrementMinutes"]').removeAttr('data-action').attr('disabled', true);
    //    $('span.timepicker-minute[data-action="showMinutes"]').removeAttr('data-action').attr('disabled', true).text('00');
    //    $('body').on('click', "#" + _interesseHorarioDiferente.HorarioCarregamento.id + ' + div a[href="#"]', handleADatePickerClick);
    //}).on('dp.change', function () {
    //    $(this).val($(this).val().split(':')[0] + ':00')
    //    $('span.timepicker-minute').text('00');
    //    $('body').off('click', "#" + _interesseHorarioDiferente.HorarioCarregamento.id + ' + div a[href="#"]', handleADatePickerClick);
    //});

    loadInteresseCargaComponenteFrete();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function atualizarValorFreteInformadoClick() {
    var dados = {
        Cargas: obterListaCargasSalvar(_informarValorFrete.Cargas.val())
    };

    executarReST("JanelaCarregamentoTransportador/InformarValorFrete", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                $.each(retorno.Data.Cargas, function (j, carga) {
                    for (var i = 0; i < cargas.length; i++) {
                        if (cargas[i].Codigo.val() == carga.Codigo) {
                            AdicionarCarga(carga, i);
                            break;
                        }
                    }
                });

                Global.fecharModal('divModalInformarValorFrete');
            }
            else
                _informarValorFrete.MensagemAlerta.val(retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function handleADatePickerClick(e) {
    if (e && e.preventDefault)
        e.preventDefault();
}

function removerInteresseClick(e, sender) {
    var dados = { Carga: e.Carga.val() };
    exibirConfirmacao("Confirmação", "Realmente deseja remover o interesse na carga?", function () {
        executarReST("JanelaCarregamentoTransportador/RemoverInteresseCarga", dados, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                    $.each(r.Data.Cargas, function (j, carga) {
                        for (var i = 0; i < cargas.length; i++) {
                            if (cargas[i].Codigo.val() == carga.Codigo) {
                                AdicionarCarga(carga, i);
                                break;
                            }
                        }
                    });
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function salvarInteresseHorarioDiferenteCargaClick(e) {
    salvarInteresseCarga(_interesseHorarioDiferente.Cargas.val(), e.CarregamentoProximoDia.val(), e.HorarioCarregamento.val(), e.Veiculo.codEntity(), e.Reboque.codEntity(), e.SegundoReboque.codEntity())
        .then(function (success, msg) {
            if (success) {
                Global.fecharModal('divModalSolicitarHorarioCarregamentoDiferente');
            }
            else {
                _interesseHorarioDiferente.MensagemRetorno.val(msg);
            }
        });
}

function tenhoInteresseClick(e, sender) {
    var data = { Carga: e.Carga.val(), CodigoJanelaCarregamentoTransportador: e.CodigoJanelaCarregamentoTransportador.val(), CodigoJanelaCarregamento: e.Codigo.val() };

    executarReST("JanelaCarregamentoTransportador/BuscarValidacaoCentroCarregamento", data, function (retorno) {
        if (retorno.Success && retorno.Data) {
            exibirConfirmacao("Confirmação", retorno.Data.MensagemConfirmacao, function () { tenhoInteresse(e, sender); },
                function () { }, "Desejo participar do leilão", "Prefiro não participar");
        } else {
            tenhoInteresse(e, sender);
        }; 
    });
}

function tenhoInteresse(e, sender) {
    var data = { Carga: e.Carga.val() };
    var horarioCarregamento = e.Dados.InicioCarregamento ? moment(e.Dados.InicioCarregamento, "DD/MM/YYYY HH:mm").format("HH:00") : "";
    var valorFreteTransportador = e.Dados.ValorFrete;

    _interesseHorarioDiferente.HorarioCarregamento.visible(true);
    _interesseHorarioDiferente.CarregamentoProximoDia.visible(true);

    var _RequestProximaData = function () {
        executarReST("JanelaCarregamentoTransportador/BuscarProximaDataDisponivel", data, function (retorno) {
            if (retorno.Success) {
                PreencherObjetoKnout(_interesseHorarioDiferente, retorno);

                _interesseHorarioDiferente.CarregamentoProximoDia.visible(retorno.Data.PermitirSelecaoHorarioCarregamento);
                _interesseHorarioDiferente.HorarioCarregamento.visible(retorno.Data.PermitirSelecaoHorarioCarregamento);

                _interesseHorarioDiferente.HorarioCarregamento.val(horarioCarregamento);
                _interesseHorarioDiferente.ValorFreteTransportador.val(valorFreteTransportador);
                _interesseHorarioDiferente.CarregamentoProximoDia.text(retorno.Data.DataCarregamentoProximoDia);

                controlarExibicaoCamposVeiculoInteresseHorarioDiferente(retorno.Data);

                for (var i = 0; i < retorno.Data.Cargas.length; i++) {
                    var interesseCarga = new InteresseCarga(retorno.Data.Cargas[i]);

                    _interesseHorarioDiferente.Cargas.val.push(interesseCarga);

                    carregarGridComponentesFrete(interesseCarga);

                    $("#" + interesseCarga.ValorFrete.id).maskMoney(interesseCarga.ValorFrete.configDecimal);
                }

                $("a[href='#tab-dados-interesse-carga']").click();

                if (e.BloquearComponentesDeFreteJanelaCarregamentoTransportador.val())
                    $(".solicitar-horario-carregamento-diferente-componentes-frete").hide();
                else
                    $(".solicitar-horario-carregamento-diferente-componentes-frete").show();

                Global.abrirModal('divModalSolicitarHorarioCarregamentoDiferente');
                $("#divModalSolicitarHorarioCarregamentoDiferente").one('hidden.bs.modal', function () {
                    limparCamposInteresseHorarioDiferente();
                });
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        });
    }

    if ((e.Dados.PermitirTransportadorInformarValorFrete || e.Dados.CargaLiberadaCotacao)) {
        _interesseHorarioDiferente.ValorFreteTransportador.visible(true);

        if (e.Dados.OcultarEdicaoDataHora || e.Dados.HorarioFixoCarregamento) {
            _interesseHorarioDiferente.HorarioCarregamento.visible(false);
            _interesseHorarioDiferente.CarregamentoProximoDia.visible(false);
        }
        _RequestProximaData();
    }
    else {
        var InformarNovoHorario = function () {
            _RequestProximaData();
        }

        var MarcarInteresse = function () {
            var apenasInteresseCarga = new InteresseCarga({
                CodigoCarga: e.Carga.val(),
                ValorFrete: e.ValorFrete.val()
            });

            salvarInteresseCarga([apenasInteresseCarga], false)
                .then(function (success, msg) {
                    if (!success) {
                        exibirMensagem(tipoMensagem.aviso, "Interesse na carga", msg);
                    }
                });
        }

        _interesseHorarioDiferente.ValorFreteTransportador.visible(false);
        exibirConfirmacao(Localization.Resources.Logistica.JanelaCarregamentoTransportador.Atencao, Localization.Resources.Logistica.JanelaCarregamentoTransportador.DesejaSolicitarOCarregamentoEmOutroHorarioParaACarga.format(e.Numero.val()), InformarNovoHorario, MarcarInteresse, Localization.Resources.Logistica.JanelaCarregamentoTransportador.SimInformarOutroHorario, Localization.Resources.Logistica.JanelaCarregamentoTransportador.NaoApenasInformarOInteresse);
    }
}

function informarValorFreteClick(e) {
    executarReST("JanelaCarregamentoTransportador/ObterDadosFrete", { Carga: e.Carga.val() }, function (retorno) {
        if (retorno.Success) {
            for (var i = 0; i < retorno.Data.length; i++) {
                var interesseCarga = new InteresseCarga(retorno.Data[i]);

                _informarValorFrete.Cargas.val.push(interesseCarga);

                carregarGridComponentesFrete(interesseCarga);

                $("#" + interesseCarga.ValorFrete.id).maskMoney(interesseCarga.ValorFrete.configDecimal);
            }

            if (e.BloquearComponentesDeFreteJanelaCarregamentoTransportador.val())
                $(".informar-valor-frete-componentes-frete").hide();
            else
                $(".informar-valor-frete-componentes-frete").show();

            Global.abrirModal('divModalInformarValorFrete');
            $("#divModalInformarValorFrete").one('hidden.bs.modal', function () {
                limparCamposInformarValorFrete();
            });
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function carregarGridComponentesFrete(interesseCarga) {
    var possuiPercentualSobreNota = true;
    var opcaoEditar = { descricao: "Editar", tamanho: 15, id: guid(), metodo: function (e) { editarInteresseCargaComponenteFrete(interesseCarga, e); }, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    if (interesseCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Embarcador)
        possuiPercentualSobreNota = false;

    var header = [
        { data: "Codigo", visible: false },
        { data: "ComponenteFrete", visible: false },
        { data: "DescricaoComponente", visible: false },
        { data: "TipoComponenteFrete", visible: false },
        { data: "CobrarOutroDocumento", visible: false },
        { data: "TipoValor", visible: false },
        { data: "DescontarValorTotalAReceber", visible: false },
        { data: "DescricaoModeloDocumentoFiscal", visible: false },
        { data: "CodigoModeloDocumentoFiscal", visible: false },
        { data: "ValorSugerido", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%" },
        { data: "AbreviacaoDocumentoFiscal", title: "Doc.", width: "10%" },
        { data: "Percentual", title: "Percentual", width: "15%", className: "text-align-right", visible: possuiPercentualSobreNota }
    ];

    if (interesseCarga.Moeda.val() !== null && interesseCarga.Moeda.val() !== EnumMoedaCotacaoBancoCentral.Real)
        header.push({ data: "ValorTotalMoeda", title: "Val. Moeda", width: "15%", className: "text-align-right" });
    else
        header.push({ data: "ValorTotalMoeda", visible: false });

    header.push({ data: "Valor", title: "Valor", width: "15%", className: "text-align-right" });

    interesseCarga.Grid = new BasicDataTable(interesseCarga.AdicionarComponenteFrete.idGrid, header, menuOpcoes);
    interesseCarga.Grid.CarregarGrid([]);
}

function controlarExibicaoCamposVeiculoInteresseHorarioDiferente(dadosTransporte) {
    var reboqueVisivel = false;
    var segundoReboqueVisivel = false;
    var veiculoVisivel = dadosTransporte.ExigirInformarVeiculo;

    if (_interesseHorarioDiferente.ExigirConfirmacaoTracao.val() && dadosTransporte.ExigirInformarVeiculo) {
        reboqueVisivel = (dadosTransporte.NumeroReboques >= 1);
        segundoReboqueVisivel = (dadosTransporte.NumeroReboques > 1);
    }

    _interesseHorarioDiferente.Veiculo.required = veiculoVisivel;
    _interesseHorarioDiferente.Veiculo.text((_interesseHorarioDiferente.Veiculo.required ? "*" : "") + (reboqueVisivel ? "Tração (Cavalo):" : "Veiculo:"));
    _interesseHorarioDiferente.Veiculo.visible(veiculoVisivel);

    _interesseHorarioDiferente.Reboque.required = reboqueVisivel;
    _interesseHorarioDiferente.Reboque.text((_interesseHorarioDiferente.Reboque.required ? "*" : "") + (segundoReboqueVisivel ? "Veículo (Carreta 1):" : "Veículo (Carreta):"));
    _interesseHorarioDiferente.Reboque.visible(reboqueVisivel);

    _interesseHorarioDiferente.SegundoReboque.required = segundoReboqueVisivel;
    _interesseHorarioDiferente.SegundoReboque.text((_interesseHorarioDiferente.SegundoReboque.required ? "*" : "") + "Veículo (Carreta 2):");
    _interesseHorarioDiferente.SegundoReboque.visible(segundoReboqueVisivel);
}

function limparCamposInformarValorFrete() {
    LimparCampos(_informarValorFrete);

    _informarValorFrete.Cargas.val([]);
}

function limparCamposInteresseHorarioDiferente() {
    LimparCampos(_interesseHorarioDiferente);

    _interesseHorarioDiferente.Cargas.val([]);
}

function obterListaCargasSalvar(listaCargas) {
    var listaCargasSalvar = new Array();

    for (var i = 0; i < listaCargas.length; i++) {
        var carga = listaCargas[i];
        var componentesFrete = carga.ComponentesFrete.val();
        var componentesFreteSalvar = [];

        for (var j = 0; j < componentesFrete.length; j++) {
            var componenteFrete = componentesFrete[j];

            componentesFreteSalvar.push({
                CobrarOutroDocumento: componenteFrete.CobrarOutroDocumento,
                ModeloDocumentoFiscal: componenteFrete.CodigoModeloDocumentoFiscal,
                ComponenteFrete: componenteFrete.ComponenteFrete,
                Percentual: componenteFrete.Percentual,
                ValorComponente: componenteFrete.Valor,
                ValorSugerido: componenteFrete.ValorSugerido,
                ValorTotalMoeda: componenteFrete.ValorTotalMoeda
            });
        }

        listaCargasSalvar.push({
            Codigo: carga.CodigoCarga.val(),
            Valor: carga.ValorFrete.val(),
            ComponentesFrete: componentesFreteSalvar
        });
    }

    return JSON.stringify(listaCargasSalvar);
}

function retornoConsultaReboqueInteresseHorarioDiferente(reboqueSelecionado) {
    if (_interesseHorarioDiferente.SegundoReboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_interesseHorarioDiferente.Reboque);
    }
    else {
        _interesseHorarioDiferente.Reboque.codEntity(reboqueSelecionado.Codigo);
        _interesseHorarioDiferente.Reboque.entityDescription(reboqueSelecionado.Placa);
        _interesseHorarioDiferente.Reboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaSegundoReboqueInteresseHorarioDiferente(reboqueSelecionado) {
    if (_interesseHorarioDiferente.Reboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_interesseHorarioDiferente.SegundoReboque);
    }
    else {
        _interesseHorarioDiferente.SegundoReboque.codEntity(reboqueSelecionado.Codigo);
        _interesseHorarioDiferente.SegundoReboque.entityDescription(reboqueSelecionado.Placa);
        _interesseHorarioDiferente.SegundoReboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaVeiculoInteresseHorarioDiferente(veiculoSelecionado) {
    _interesseHorarioDiferente.Veiculo.codEntity(veiculoSelecionado.Codigo);

    if (_interesseHorarioDiferente.ExigirConfirmacaoTracao.val()) {
        _interesseHorarioDiferente.Veiculo.entityDescription(veiculoSelecionado.Placa);
        _interesseHorarioDiferente.Veiculo.val(veiculoSelecionado.Placa);
    }
    else {
        _interesseHorarioDiferente.Veiculo.entityDescription(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
        _interesseHorarioDiferente.Veiculo.val(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
    }

    Global.setarFocoProximoCampo(_interesseHorarioDiferente.Veiculo.id);
}
function loadInformarInteresseDadosTransporte(dados, listaCargas) {
    
    _informarInteresseDadosTransporte = new InteresseDadosTransporte();
    KoBindings(_informarInteresseDadosTransporte, "knockoutInteresseDadosCarga");


    _informarInteresseDadosTransporte.Cargas.val(dados.Cargas);
    _informarInteresseDadosTransporte.CodigoCarga.val(listaCargas[0].CodigoCarga.val());
    
    var somenteDisponiveis = !_CONFIGURACAO_TMS.NaoExigeInformarDisponibilidadeDeVeiculo;

    BuscarVeiculos(_informarInteresseDadosTransporte.VeiculoInteresse, retornoConsultaVeiculoInteresse, _informarInteresseDadosTransporte.Transportador, _informarInteresseDadosTransporte.ModeloVeiculo, null, true, null, _informarInteresseDadosTransporte.TipoCarga, true, somenteDisponiveis, _informarInteresseDadosTransporte.Codigo, _informarInteresseDadosTransporte.TipoVeiculo);
    BuscarMotoristas(_informarInteresseDadosTransporte.MotoristaInteresse, null, _informarInteresseDadosTransporte.Transportador, null, null, null, null, null, true);

    Global.abrirModal('divModalInteresseInformarDadosTransporte');

}

function retornoConsultaVeiculoInteresse(veiculoSelecionado) {
    _informarInteresseDadosTransporte.VeiculoInteresse.codEntity(veiculoSelecionado.Codigo);

    if (_informarInteresseDadosTransporte.ExigirConfirmacaoTracao.val()) {
        _informarInteresseDadosTransporte.VeiculoInteresse.entityDescription(veiculoSelecionado.Placa);
        _informarInteresseDadosTransporte.VeiculoInteresse.val(veiculoSelecionado.Placa);
    }
    else {
        _informarInteresseDadosTransporte.VeiculoInteresse.entityDescription(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
        _informarInteresseDadosTransporte.VeiculoInteresse.val(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
    }
}
function salvarInteresseCarga(listaCargas, carregamentoProximoDia, horarioCarregamento, veiculo, reboque, segundoReboque) {
    var p = new promise.Promise();

    var dados = {
        HorarioCarregamento: horarioCarregamento,
        CarregamentoProximoDia: carregamentoProximoDia,
        Cargas: obterListaCargasSalvar(listaCargas),
        Veiculo: veiculo,
        Reboque: reboque,
        SegundoReboque: segundoReboque
    };

    executarReST("JanelaCarregamentoTransportador/ValidarLanceLimite", dados, function (retorno) {
        if (retorno.Success) {
            if (!string.IsNullOrWhiteSpace(retorno.Msg)) {
                exibirConfirmacao("Confirmação", retorno.Msg, function () {
                    executarInformarInteresseCarga(dados, p, listaCargas);
                }, function () { return p.done(false) });
            } else {
                executarInformarInteresseCarga(dados, p, listaCargas);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });

    return p;
}

function executarInformarInteresseCarga(dados, p, listaCargas) {
    executarReST("JanelaCarregamentoTransportador/InformarInteresseCarga", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var cargaReferencia = retorno.Data.Cargas[0];
                exibirTermoConfirmacaoChegadaHorario(cargaReferencia);

                var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                $.each(retorno.Data.Cargas, function (j, carga) {
                    for (var i = 0; i < cargas.length; i++) {
                        if (cargas[i].Codigo.val() == carga.Codigo) {
                            AdicionarCarga(carga, i);
                            break;
                        }
                    }
                });

                if (_CONFIGURACAO_TMS.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga) {
                    loadInformarInteresseDadosTransporte(dados, listaCargas);
                }

                p.done(true);
            }
            else
                p.done(false, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });

    return p;
}
function confirmarSalvarDadosTransporteClick(e, sender) {

    if (_informarInteresseDadosTransporte.VeiculoInteresse.val() == "") {
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }
    if (_informarInteresseDadosTransporte.MotoristaInteresse.val() == "") {
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }

    var dados = {
        Cargas: _informarInteresseDadosTransporte.Cargas.val(),
        Carga: _informarInteresseDadosTransporte.CodigoCarga.val(),
        Veiculo: _informarInteresseDadosTransporte.VeiculoInteresse.codEntity(),
        Motorista: _informarInteresseDadosTransporte.MotoristaInteresse.codEntity(),
        DadosInformadosNoInteresse : true
    };
    executarReST("JanelaCarregamentoTransportador/SalvarDadosTransporteCarga", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Dados salvos com sucesso!");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
    
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });

    _informarInteresseDadosTransporte.Cargas.val([]);

    Global.fecharModal('divModalInteresseInformarDadosTransporte');

    
}

// #region Funções Privadas
