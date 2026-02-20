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
/// <reference path="../../Enumeradores/EnumTipoComponenteFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoCampoValorTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoCampoAcertoViagem.js" />
/// <reference path="../../Enumeradores/EnumDefinicaoDataEnvioIntegracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridComponenteFrete;
var _componenteFrete;
var _pesquisaComponenteFrete;

var _tipoAcertoViagem = [
    { text: "Nada Fazer", value: EnumTipoCampoAcertoViagem.NadaFazer },
    { text: "Somar", value: EnumTipoCampoAcertoViagem.Somar },
    { text: "Subtrair", value: EnumTipoCampoAcertoViagem.Subtrair }
];

var _TipoCampoComponenteFrete = [
    { text: "Valor Fixo", value: EnumTipoCampoValorTabelaFrete.AumentoValor },
    { text: "Percentual Sobre a Nota Fiscal", value: EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal }
];

var _definicaoDataEnvioIntegracao = [
    { text: "ETA – POL", value: EnumDefinicaoDataEnvioIntegracao.EtaPol },
    { text: "ETD – POL", value: EnumDefinicaoDataEnvioIntegracao.EtdPol },
    { text: "ETA – POD", value: EnumDefinicaoDataEnvioIntegracao.EtaPod },
    { text: "ETS – POD", value: EnumDefinicaoDataEnvioIntegracao.EtsPod },
    { text: "Coleta JO", value: EnumDefinicaoDataEnvioIntegracao.ColetaJo },
    { text: "Entrega JO", value: EnumDefinicaoDataEnvioIntegracao.EntregaJo }
];

var PesquisaComponenteFrete = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, text: "Situação: " });
    this.TipoComponenteFrete = PropertyEntity({ val: ko.observable(EnumTipoComponenteFrete.TODOS), options: EnumTipoComponenteFrete.obterOpcoesPesquisa(), def: EnumTipoComponenteFrete.TODOS, text: "Tipo do Componente de Frete: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridComponenteFrete.CarregarGrid();
        }, type: types.event, text: "Pesquisar", id: guid(), idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ComponenteFrete = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true });
    this.TipoComponenteFrete = PropertyEntity({ val: ko.observable(EnumTipoComponenteFrete.OUTROS), options: EnumTipoComponenteFrete.obterOpcoes(), def: EnumTipoComponenteFrete.OUTROS, text: "*Tipo do Componente de Frete: ", issue: 84 });
    this.TipoComponenteFreteDOCCOB = PropertyEntity({ val: ko.observable(EnumTipoComponenteFrete.OUTROS), options: EnumTipoComponenteFrete.obterOpcoes(), def: EnumTipoComponenteFrete.OUTROS, text: "Tipo do Componente de Frete para DOCCOB: ", issue: 84 });
    this.TipoCampoAcertoViagem = PropertyEntity({ val: ko.observable(EnumTipoCampoAcertoViagem.NadaFazer), options: _tipoAcertoViagem, def: EnumTipoCampoAcertoViagem.NadaFazer, text: "*Tipo do uso ao Acerto: " });
    this.DefinicaoDataEnvioIntegracao = PropertyEntity({ val: ko.observable(EnumDefinicaoDataEnvioIntegracao.EtaPol), options: _definicaoDataEnvioIntegracao, def: EnumDefinicaoDataEnvioIntegracao.EtaPol, text: "Data Fim de Prestação do Serviço (Arrival Date): " });
    this.DefinicaoDataEnvioIntegracaoEmbarque = PropertyEntity({ val: ko.observable(EnumDefinicaoDataEnvioIntegracao.EtaPol), options: _definicaoDataEnvioIntegracao, def: EnumDefinicaoDataEnvioIntegracao.EtaPol, text: "Data Início de Prestação do Serviço (Departure Date): " });
    this.TipoValor = PropertyEntity({ val: ko.observable(EnumTipoCampoValorTabelaFrete.AumentoValor), options: _TipoCampoComponenteFrete, visible: ko.observable(true), def: EnumTipoCampoValorTabelaFrete.AumentoValor, text: "*Tipo do Valor para o Componente de Frete: ", issue: 372 });
    this.DescontarValorTotalAReceber = PropertyEntity({ text: "O valor deste componente deve ser descontado do valor a receber (não será destacado no documento emitido)", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), issue: 373 });
    this.AcrescentaValorTotalAReceber = PropertyEntity({ text: "O valor deste componente deve ser acrescentado no valor a receber (não será destacado no documento emitido)", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), issue: 374 });
    this.SomarComponenteFreteLiquido = PropertyEntity({ text: "O valor deste componente deve ser somado ao valor líquido do frete (não será destacado no documento emitido)", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), issue: 375 });
    this.DescontarComponenteFreteLiquido = PropertyEntity({ text: "O valor deste componente deve ser descontado do valor líquido do frete ", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), issue: 375 });
    this.DescontarComponenteNotaFiscalServico = PropertyEntity({ text: "O valor deste componente deve ser descontado do valor da nota fiscal de serviço", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoSomarValorTotalAReceber = PropertyEntity({ text: "O valor deste componente não deve ser somado ao valor a receber (Prestação)", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), issue: 80001 });
    this.NaoSomarValorTotalPrestacao = PropertyEntity({ text: "O valor deste componente não deve ser somado ao valor total do serviço", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), issue: 0 });
    this.NaoIncluirBaseCalculoImpostos = PropertyEntity({ text: "O valor deste componente não deve ser incluso na base de cálculo dos impostos", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.NaoDeveIncidirSobreNotasFiscaisPateles = PropertyEntity({ text: "Não deve incidir sobre notas fiscais de paletes", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), issue: 0 });
    this.ImprimirDescricaoComponenteEmComplementos = PropertyEntity({ text: "Imprimir a descrição deste componente em CT-es complementares", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.SomenteParaCargaPerigosa = PropertyEntity({ text: "Somente para Carga Perigosa", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CodigoEmbarcador = PropertyEntity({ maxlength: 50, text: "Código de Integração:", issue: 15 });
    this.ChargeId = PropertyEntity({ maxlength: 50, text: "Charge Id:", issue: 15 });
    this.ChargeCodeNet = PropertyEntity({ minLength: 3, maxlength: 8, text: "Charge Code Net:", issue: 15 });
    this.ChargeCodeGross = PropertyEntity({ minLength: 3, maxlength: 8, text: "Charge Code Gross:", issue: 15 });
    this.ComponenteFreteBundle = PropertyEntity({ text: "Componente bundle?", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.CalcularPISComponente = PropertyEntity({ text: "Calcular PIS/COFINS do componente", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CalcularICMSComponente = PropertyEntity({ text: "Calcular o ICMS para o componente", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.EnviarComponenteNFTP = PropertyEntity({ text: "Esse componente não deve ser enviado na integração para NFTP", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PossuiBundle = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Possui bundle? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.ImprimirOutraDescricaoCTe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.DescricaoCTe = PropertyEntity({ text: "Exibir outra descrição componente?", required: false, enable: ko.observable(false), maxlength: 15 });

    this.GerarMovimentoAutomatico = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento financeiro automatizado para esse componente de frete: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Emissão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cancelamento:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoAnulacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Anulação (com nota de anulação do embarcador):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.GerarMovimentoAutomatico.val.subscribe(function (novoValor) {
        GerarMovimentoAutomaticoChange(novoValor);
    });

    this.PossuiBundle.val.subscribe(function (novoValor) {
        PossuiBundleChange(novoValor);
    });

    this.ImprimirOutraDescricaoCTe.val.subscribe(function (novoValor) {
        if (novoValor) {
            _componenteFrete.DescricaoCTe.required = true;
            _componenteFrete.DescricaoCTe.enable(true);
        } else {
            _componenteFrete.DescricaoCTe.enable(false);
            _componenteFrete.DescricaoCTe.required = false;
            _componenteFrete.DescricaoCTe.val("");
        }
    });

    configurarSubscribe(this.TipoComponenteFrete);
    configurarSubscribe(this.TipoComponenteFreteDOCCOB);
}

//*******EVENTOS*******

function loadComponenteFrete() {

    _componenteFrete = new ComponenteFrete();
    KoBindings(_componenteFrete, "knockoutCadastroComponenteFrete");

    _pesquisaComponenteFrete = new PesquisaComponenteFrete();
    KoBindings(_pesquisaComponenteFrete, "knockoutPesquisaComponenteFrete", false, _pesquisaComponenteFrete.Pesquisar.id);

    HeaderAuditoria("ComponenteFrete", _componenteFrete);

    _componenteFrete.PossuiBundle.val(false);

    new BuscarTipoMovimento(_componenteFrete.TipoMovimentoEmissao);
    new BuscarTipoMovimento(_componenteFrete.TipoMovimentoCancelamento);
    new BuscarTipoMovimento(_componenteFrete.TipoMovimentoAnulacao);
    new BuscarTipoMovimento(_componenteFrete.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador);

    buscarComponenteFrete();

    BuscarComponentesDeFrete(_componenteFrete.ComponenteFreteBundle);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        $("#liTabMovimentoFinanceiro").removeClass("d-none");



    _componenteFrete.PossuiBundle.val.subscribe(function (newValue) {
        if (!newValue) {
            _componenteFrete.ComponenteFreteBundle.val('');
            _componenteFrete.ComponenteFreteBundle.codEntity(0);
        }
    });

}

function GerarMovimentoAutomaticoChange(novoValor) {
    if (novoValor) {
        _componenteFrete.GerarMovimentoAutomatico.visibleFade(true);
        _componenteFrete.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.required(true);
        _componenteFrete.TipoMovimentoEmissao.required(true);
        _componenteFrete.TipoMovimentoCancelamento.required(true);
        _componenteFrete.TipoMovimentoAnulacao.required(true);
    } else {
        _componenteFrete.GerarMovimentoAutomatico.visibleFade(false);
        _componenteFrete.TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador.required(false);
        _componenteFrete.TipoMovimentoEmissao.required(false);
        _componenteFrete.TipoMovimentoCancelamento.required(false);
        _componenteFrete.TipoMovimentoAnulacao.required(false);
    }
}

function PossuiBundleChange(novoValor) {
    if (novoValor) {
        _componenteFrete.PossuiBundle.visibleFade(true);
    } else {
        _componenteFrete.PossuiBundle.visibleFade(false);
    }
}

function configurarSubscribe(tipoComponente) {
    tipoComponente.val.subscribe(function (novoValor) {
        tipoComponenteFreteChange(_componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles, novoValor === EnumTipoComponenteFrete.ADVALOREM);
        tipoComponenteFreteChange(_componenteFrete.NaoIncluirBaseCalculoImpostos, novoValor === EnumTipoComponenteFrete.PEDAGIO);
        tipoComponenteFreteChange(_componenteFrete.TipoValor, novoValor === EnumTipoComponenteFrete.OUTROS, true);
        tipoComponenteFreteChange(_componenteFrete.DescontarValorTotalAReceber, novoValor === EnumTipoComponenteFrete.OUTROS);
    });
}

function tipoComponenteFreteChange(propriedade, condicao, alterarApenasVisibilidade) {
    propriedade.visible(condicao);
    if (!alterarApenasVisibilidade && !condicao) propriedade.val(false);
}

function adicionarClick(e, sender) {
    if ((_componenteFrete.ChargeCodeNet.val()?.length > 1 && _componenteFrete.ChargeCodeNet.val()?.length < 3) || (_componenteFrete.ChargeCodeGross.val()?.length > 1 && _componenteFrete.ChargeCodeGross.val()?.length < 3)) {
        exibirMensagem(tipoMensagem.atencao, "Alerta", "Charge Code Net e ChargeCodeGross devem possuir mais de 3 caracteres.");
        return;
    }

    if (_componenteFrete.PossuiBundle.val() == true && (_componenteFrete.ComponenteFreteBundle.val() == '' || _componenteFrete.ComponenteFreteBundle.codEntity() == 0)) {
        exibirMensagem(tipoMensagem.atencao, "Alerta", "Componente Bundle deve ser preenchido quando possuir bundle.");
        return;
    }

    Salvar(e, "ComponenteFrete/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridComponenteFrete.CarregarGrid();
                limparCamposComponenteFrete();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if ((_componenteFrete.ChargeCodeNet.val()?.length > 1 && _componenteFrete.ChargeCodeNet.val()?.length < 3) || (_componenteFrete.ChargeCodeGross.val()?.length > 1 && _componenteFrete.ChargeCodeGross.val()?.length < 3)) {
        exibirMensagem(tipoMensagem.atencao, "Alerta", "Charge Code Net e ChargeCodeGross devem possuir mais de 3 caracteres.");
        return;
    }
    
    if (_componenteFrete.PossuiBundle.val() == true && (_componenteFrete.ComponenteFreteBundle.val() == '' || _componenteFrete.ComponenteFreteBundle.codEntity() == 0)) {
        exibirMensagem(tipoMensagem.atencao, "Alerta", "Componente Bundle deve ser preenchido quando possuir bundle.");
        return;
    }
    Salvar(e, "ComponenteFrete/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridComponenteFrete.CarregarGrid();
                limparCamposComponenteFrete();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o componente de frete " + _componenteFrete.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_componenteFrete, "ComponenteFrete/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridComponenteFrete.CarregarGrid();
                    limparCamposComponenteFrete();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposComponenteFrete();
}

//*******MÉTODOS*******

function buscarComponenteFrete() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarComponenteFrete, tamanho: 15, icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridComponenteFrete = new GridView(_pesquisaComponenteFrete.Pesquisar.idGrid, "ComponenteFrete/Pesquisa", _pesquisaComponenteFrete, menuOpcoes, null);
    _gridComponenteFrete.CarregarGrid();
}

function editarComponenteFrete(componenteFreteGrid) {
    _componenteFrete.Codigo.val(componenteFreteGrid.Codigo);
    BuscarPorCodigo(_componenteFrete, "ComponenteFrete/BuscarPorCodigo", function (arg) {
        _pesquisaComponenteFrete.ExibirFiltros.visibleFade(false);
        _componenteFrete.PossuiBundle.visibleFade(true);
        _componenteFrete.Atualizar.visible(true);
        _componenteFrete.Cancelar.visible(true);
        _componenteFrete.Excluir.visible(true);
        _componenteFrete.Adicionar.visible(false);
    }, null);
}

function limparCamposComponenteFrete() {
    _componenteFrete.Atualizar.visible(false);
    _componenteFrete.Cancelar.visible(false);
    _componenteFrete.Excluir.visible(false);
    _componenteFrete.Adicionar.visible(true);
    LimparCampos(_componenteFrete);
    $(".nav-tabs a:first").tab("show");
}

