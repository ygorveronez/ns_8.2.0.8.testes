/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../Consultas/Tranportador.js" />

// #region Objetos Globais do Arquivo

var _cargaAgrupadaDadosContainer;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CargaAgrupadaDados = function (cargaAgrupada) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable(Localization.Resources.Cargas.Carga.Transportador.getRequiredFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ConsultaEmpresa;

    PreencherObjetoKnout(this, { Data: cargaAgrupada });
}

var CargaAgrupadaDadosContainer = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RaizCNPJEmpresa = PropertyEntity({ });
    this.ListaCargaAgrupadaDados = ko.observableArray(new Array());

    this.Atualizar = PropertyEntity({ eventClick: atualizarCargaAgrupadaDadosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true), enable: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadCargaAgrupadaDados() {
    _cargaAgrupadaDadosContainer = new CargaAgrupadaDadosContainer();
    KoBindings(_cargaAgrupadaDadosContainer, "knockoutAlterarDadosCargaAgrupada");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function atualizarCargaAgrupadaDadosClick() {
    var listaCargaAgrupadaDados = _cargaAgrupadaDadosContainer.ListaCargaAgrupadaDados().slice();
    var listaCargaAgrupadaDadosSalvar = [];

    for (var i = 0; i < listaCargaAgrupadaDados.length; i++) {
        var cargaAgrupadaDados = listaCargaAgrupadaDados[i];

        listaCargaAgrupadaDadosSalvar.push({
            Codigo: cargaAgrupadaDados.Codigo.val(),
            Empresa: cargaAgrupadaDados.Empresa.codEntity()
        });
    }

    var dados = {
        Carga: _cargaAgrupadaDadosContainer.CodigoCarga.val(),
        CargasAgrupadas: JSON.stringify(listaCargaAgrupadaDadosSalvar)
    };

    executarReST("Carga/AtualizarCargaAgrupadaDados", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargasAgrupadasAlteradasComSucesso);
                Global.fecharModal("divModalAlterarDadosCargaAgrupada");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções de Públicas

function exibirDetalhesCargaAgrupada(cargaSelecionada) {
    executarReST("Carga/ObterCargaAgrupadaDetalhes", { Carga: cargaSelecionada.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            limparListaCargaAgrupadaDados();

            _cargaAgrupadaDadosContainer.CodigoCarga.val(cargaSelecionada.Codigo.val());
            _cargaAgrupadaDadosContainer.RaizCNPJEmpresa.val(cargaSelecionada.RaizCNPJEmpresa.val());
            _cargaAgrupadaDadosContainer.Atualizar.visible(isPermitirEditarCargaAgrupadaDados(cargaSelecionada));

            for (var i = 0; i < retorno.Data.length; i++) {
                var knoutCargaAgrupadaDados = new CargaAgrupadaDados(retorno.Data[i]);

                _cargaAgrupadaDadosContainer.ListaCargaAgrupadaDados.push(knoutCargaAgrupadaDados);

                criarConsultasCargaAgrupadaDados(knoutCargaAgrupadaDados);
                controlarVisibilidadeCamposCargaAgrupadaDados(knoutCargaAgrupadaDados, cargaSelecionada);
                controlarComponentesHabilitadosEdicaoCargaAgrupadaDados(knoutCargaAgrupadaDados);
            }

            var carouselAlterarDadosCargasAgrupadas = document.querySelector('#carousel-alterar-dados-cargas-agrupadas');
            new bootstrap.Carousel(carouselAlterarDadosCargasAgrupadas, { interval: false });
            Global.abrirModal("divModalAlterarDadosCargaAgrupada");
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, retorno.Msg);
    });
}

// #endregion Funções de Públicas

// #region Funções de Privadas

function controlarComponentesHabilitadosEdicaoCargaAgrupadaDados(knoutCargaAgrupadaDados) {
    var habilitarEdicao = _cargaAgrupadaDadosContainer.Atualizar.visible();

    knoutCargaAgrupadaDados.Empresa.enable(habilitarEdicao);
}

function controlarVisibilidadeCamposCargaAgrupadaDados(knoutCargaAgrupadaDados, cargaSelecionada) {
    knoutCargaAgrupadaDados.Empresa.visible(cargaSelecionada.CargaDeComplemento.val());
}

function criarConsultasCargaAgrupadaDados(knoutCargaAgrupadaDados) {
    knoutCargaAgrupadaDados.ConsultaEmpresa = new BuscarTransportadores(knoutCargaAgrupadaDados.Empresa, null, null, null, null, null, null, null, _cargaAgrupadaDadosContainer.RaizCNPJEmpresa);
}

function limparListaCargaAgrupadaDados() {
    var listaCargaAgrupadaDados = _cargaAgrupadaDadosContainer.ListaCargaAgrupadaDados().slice();

    for (var i = 0; i < listaCargaAgrupadaDados.length; i++)
        listaCargaAgrupadaDados[i].ConsultaEmpresa.Destroy();

    _cargaAgrupadaDadosContainer.ListaCargaAgrupadaDados.removeAll();
}

function isPermitirEditarCargaAgrupadaDados(cargaSelecionada) {
    if (!cargaSelecionada.ExigeNotaFiscalParaCalcularFrete.val())
        return cargaSelecionada.EtapaDadosTransportador.enable();

    return cargaSelecionada.EtapaInicioTMS.enable();
}

// #endregion Funções de Privadas
