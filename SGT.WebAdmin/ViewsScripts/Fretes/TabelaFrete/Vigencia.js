/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="VigenciaAnexo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridVigencia;
var _vigencia;
var _cadastroVigencia;
var _CRUDCadastroVigencia;

/*
 * Declaração das Classes
 */

var CadastroVigencia = function () {
    var visivelTipoTabelaFrete = (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS);
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.Transportador.getFieldDescription()), visible: ko.observable(visivelTipoTabelaFrete), idBtnSearch: guid() });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Fretes.TabelaFrete.DataInicial.getRequiredFieldDescription(), required: true });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Fretes.TabelaFrete.DataFinal.getFieldDescription() });
    this.ListaAnexo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Anexos, type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.ListaAnexo.val.subscribe(function () {
        recarregarGridVigenciaAnexo();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarVigenciaAnexoModalClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarAnexos, visible: ko.observable(true) });
}

var CRUDCadastroVigencia = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarVigenciaClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarVigenciaClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarVigenciaClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirVigenciaClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Excluir, visible: ko.observable(false) });
}

var Vigencia = function () {
    var visivelTipoTabelaFrete = (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS);
    this.ValidarPorDataCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.ValidarPelaDataDeCarregamento, val: _tabelaFrete.ValidarPorDataCarregamento.val });
    this.PermitirVigenciasSobrepostas = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.PermitirVigenciasSobrepostas, val: _tabelaFrete.PermitirVigenciasSobrepostas.val });
    this.UsarComoDataBaseVigenciaDataAtual = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.UsarComoDataBaseParaVigenciaDataAtual, val: _tabelaFrete.UsarComoDataBaseVigenciaDataAtual.val });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.Transportador), visible: ko.observable(visivelTipoTabelaFrete), idBtnSearch: guid() });

    this.ListaVigencia = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaVigencia.val.subscribe(function () {
        recarregarGridVigencia();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarVigenciaModalClick, type: types.event, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.Adicionar), visible: ko.observable(true) });
    this.Filtrar = PropertyEntity({ eventClick: filtrarVigenciasClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Filtrar });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridVigencia() {
    var linhasPorPaginas = 5;
    var opcaoEditar = { descricao: Localization.Resources.Fretes.TabelaFrete.Editar, id: guid(), metodo: editarVigenciaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Empresa", visible: false },
        { data: "DataInicial", title: Localization.Resources.Fretes.TabelaFrete.DataInicial, width: "25%" },
        { data: "DataFinal", title: Localization.Resources.Fretes.TabelaFrete.DataFinal, width: "25%" },
        { data: "Transportador", title: Localization.Resources.Fretes.TabelaFrete.Transportador, width: "30%", visible: _cadastroVigencia.Empresa.visible() },
        { data: "Situacao", title: Localization.Resources.Fretes.TabelaFrete.Situacao, width: "25%" }
    ];

    _gridVigencia = new BasicDataTable(_vigencia.ListaVigencia.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridVigencia.CarregarGrid([]);
}

function loadVigencia() {
    _vigencia = new Vigencia();
    KoBindings(_vigencia, "knockoutVigencia");

    _cadastroVigencia = new CadastroVigencia();
    KoBindings(_cadastroVigencia, "knockoutCadastroVigencia");

    _CRUDCadastroVigencia = new CRUDCadastroVigencia();
    KoBindings(_CRUDCadastroVigencia, "knockoutCRUDCadastroVigencia");

    new BuscarTransportadores(_vigencia.Empresa);
    new BuscarTransportadores(_cadastroVigencia.Empresa);

    loadVigenciaAnexo();
    loadGridVigencia();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarVigenciaClick() {
    if (!validarDadosVigencia())
        return;

    _vigencia.ListaVigencia.val().push(obterCadastroVigenciaSalvar());

    recarregarGridVigencia();
    fecharModalCadastroVigencia();
}

function filtrarVigenciasClick() {
    recarregarGridVigencia();
}

function adicionarVigenciaModalClick() {
    _cadastroVigencia.Codigo.val(guid());

    controlarExibicaoBotoesCadastroVigencia(false);
    exibirModalCadastroVigencia();
}

function atualizarVigenciaClick() {
    if (!validarDadosVigencia())
        return;

    var listaVigencia = obterListaVigencia();

    for (var i = 0; i < listaVigencia.length; i++) {
        if (_cadastroVigencia.Codigo.val() == listaVigencia[i].Codigo) {
            listaVigencia.splice(i, 1, obterCadastroVigenciaSalvar());
            break;
        }
    }

    _vigencia.ListaVigencia.val(listaVigencia);

    fecharModalCadastroVigencia();
}

function cancelarVigenciaClick() {
    fecharModalCadastroVigencia();
}

function editarVigenciaClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroVigencia, { Data: registroSelecionado });

    controlarExibicaoBotoesCadastroVigencia(true);
    exibirModalCadastroVigencia();
}

function excluirVigenciaClick() {
    if (!isNaN(_cadastroVigencia.Codigo.val())) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.Atencao, Localization.Resources.Fretes.TabelaFrete.NaoPossivelExcluirVigenciaCadastrada);
        return;
    }

    exibirConfirmacao(Localization.Resources.Fretes.TabelaFrete.Confirmacao, Localization.Resources.Fretes.TabelaFrete.RealmenteDesejaExcluirEssaVigencia, function () {
        var listaVigencia = obterListaVigencia();

        for (var i = 0; i < listaVigencia.length; i++) {
            if (_cadastroVigencia.Codigo.val() == listaVigencia[i].Codigo) {
                listaVigencia.splice(i, 1);
                break;
            }
        }

        _vigencia.ListaVigencia.val(listaVigencia);

        fecharModalCadastroVigencia();
    });
}

/*
 * Declaração das Funções Públicas
 */

function atualizarVigenciaAnexo() {
    var listaVigencia = obterListaVigencia();

    for (var i = 0; i < listaVigencia.length; i++) {
        var vigencia = listaVigencia[i];

        if (_cadastroVigencia.Codigo.val() == vigencia.Codigo) {
            vigencia.ListaAnexo = obterListaVigenciaAnexo()
            listaVigencia.splice(i, 1, vigencia);
            break;
        }
    }

    _vigencia.ListaVigencia.val(listaVigencia);
}

function isExisteVigenciaCadastrada() {
    var listaVigencia = obterListaVigencia();

    return listaVigencia.length > 0;
}

function limparCamposVigencia() {
    _vigencia.ListaVigencia.val(new Array());
    limparCamposVigenciaAnexo();
}

function preencherVigencia(dadosVigencia) {
    PreencherObjetoKnout(_vigencia, { Data: dadosVigencia });

    for (var i = 0; i < dadosVigencia.Vigencias.length; i++) {
        var vigencia = dadosVigencia.Vigencias[i];

        _vigencia.ListaVigencia.val().push({
            Codigo: vigencia.Codigo,
            DataInicial: vigencia.DataInicial,
            DataFinal: vigencia.DataFinal,
            Transportador: vigencia.Empresa.Descricao,
            Empresa: vigencia.Empresa,
            Situacao: obterSituacaoVigencia(vigencia.DataInicial, vigencia.DataFinal),
            ListaAnexo: vigencia.Anexos
        });
    }

    recarregarGridVigencia();
}

function preencherVigenciaSalvar(tabelaFrete) {
    var listaVigencia = obterListaVigencia();

    tabelaFrete["Vigencias"] =  JSON.stringify(listaVigencia);
}

/*
 * Declaração das Funções Privadas
 */

function controlarExibicaoBotoesCadastroVigencia(isEdicao) {
    _CRUDCadastroVigencia.Adicionar.visible(!isEdicao);
    _CRUDCadastroVigencia.Atualizar.visible(isEdicao);
    _CRUDCadastroVigencia.Cancelar.visible(isEdicao);
    _CRUDCadastroVigencia.Excluir.visible(isEdicao && isNaN(_cadastroVigencia.Codigo.val()));
}

function exibirModalCadastroVigencia() {
    Global.abrirModal('divModalCadastroVigencia');
    $("#divModalCadastroVigencia").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroVigencia);
    });
}

function fecharModalCadastroVigencia() {
    Global.fecharModal('divModalCadastroVigencia');
}

function obterCadastroVigenciaSalvar() {
    return {
        Codigo: _cadastroVigencia.Codigo.val(),
        DataInicial: _cadastroVigencia.DataInicial.val(),
        DataFinal: _cadastroVigencia.DataFinal.val(),
        Situacao: obterSituacaoVigencia(_cadastroVigencia.DataInicial.val(), _cadastroVigencia.DataFinal.val()),
        Transportador: _cadastroVigencia.Empresa.val(),
        Empresa: { Codigo: _cadastroVigencia.Empresa.codEntity(), Descricao: _cadastroVigencia.Empresa.val() },
        ListaAnexo: obterListaVigenciaAnexo()
    };
}

function obterListaVigencia() {
    return _vigencia.ListaVigencia.val().slice();
}

function obterSituacaoVigencia(dataInicial, dataFinal) {
    var dataInicialVigencia = moment(dataInicial, "DD/MM/YYYY");
    var dataFinalVigencia = dataFinal ? moment(dataFinal, "DD/MM/YYYY") : "";

    if ((!dataFinalVigencia && moment().isAfter(dataInicialVigencia)) || moment().isBetween(dataInicialVigencia, dataFinalVigencia))
        return Localization.Resources.Fretes.TabelaFrete.Vigente;
    else if (moment().isBefore(dataInicialVigencia))
        return Localization.Resources.Fretes.TabelaFrete.Proxima;

    return Localization.Resources.Fretes.TabelaFrete.Anterior;
}

function recarregarGridVigencia() {
    var listaVigencia = obterListaVigencia();
    var codigoEmpresaFiltrar = _vigencia.Empresa.codEntity();

    if (codigoEmpresaFiltrar > 0) {
        listaVigencia = listaVigencia.filter(function (vigencia) {
            return vigencia.Empresa.Codigo == codigoEmpresaFiltrar;
        });
    }

    _gridVigencia.CarregarGrid(listaVigencia);
}

function validarDadosVigencia() {
    if (!ValidarCamposObrigatorios(_cadastroVigencia)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
        return false;
    }

    var dataInicial = moment(_cadastroVigencia.DataInicial.val(), "DD/MM/YYYY");
    var dataFinal = _cadastroVigencia.DataFinal.val() != "" ? moment(_cadastroVigencia.DataFinal.val(), "DD/MM/YYYY") : "";

    if (dataFinal && dataInicial.isAfter(dataFinal)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.VigenciaInvalida, Localization.Resources.Fretes.TabelaFrete.DataInicialNaoPodeSerMaiorQueDataFinal);
        return false;
    }

    var listaVigencia = obterListaVigencia();
    var permiteVigenciaSobreposta = _tabelaFrete.PermitirVigenciasSobrepostas.val();

    if (!permiteVigenciaSobreposta) {
        for (var i = 0; i < listaVigencia.length; i++) {
            var vigencia = listaVigencia[i];

            if (vigencia.Codigo != _cadastroVigencia.Codigo.val() && vigencia.Empresa.Codigo == _cadastroVigencia.Empresa.codEntity()) {
                var dataInicialCadastrada = moment(vigencia.DataInicial, "DD/MM/YYYY");
                var dataFinalCadastrada = vigencia.DataFinal ? moment(vigencia.DataFinal, "DD/MM/YYYY") : "";

                if (!validarVigenciaDuplicada(dataInicial, dataFinal, dataInicialCadastrada, dataFinalCadastrada)) {
                    if (vigencia.DataFinal)
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.VigenciaJaExistente, PeriodoDeVigenciaEntrouEmConflitoComAVigenciaDeXAteX.format(vigencia.DataInicial, vigencia.DataFinal));
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.VigenciaJaExistente, PeriodoDeVigenciaEntrouEmConflitoComAVigenciaDeX.format(vigencia.DataInicial));


                    return false;
                }
            }
        }
    }

    return true;
}

function validarVigenciaDuplicada(dataInicial, dataFinal, dataInicialCadastrada, dataFinalCadastrada) {
    if (dataFinal && dataFinalCadastrada) {
        return !(
            dataInicial.isBetween(dataInicialCadastrada, dataFinalCadastrada) ||
            dataFinal.isBetween(dataInicialCadastrada, dataFinalCadastrada) ||
            dataInicialCadastrada.isBetween(dataInicial, dataFinal) ||
            dataFinalCadastrada.isBetween(dataInicial, dataFinal) ||
            dataInicial.isSame(dataInicialCadastrada) ||
            dataInicial.isSame(dataFinalCadastrada) ||
            dataFinal.isSame(dataInicialCadastrada) ||
            dataFinal.isSame(dataFinalCadastrada)
        );
    }

    if (!dataFinal && !dataFinalCadastrada)
        return false;

    if (!dataFinal && dataInicialCadastrada.isAfter(dataInicial))
        return false;

    if (!dataFinalCadastrada && dataInicial.isAfter(dataInicialCadastrada))
        return false;

    return true;
}
