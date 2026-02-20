/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/GrupoProduto.js" />
/// <reference path="../../Enumeradores/EnumRegimeLimpeza.js" />
/// <reference path="AnexosChecklist.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroChecklist;

/*
 * Declaração das Classes
 */

var CadastroChecklist = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.UltimaCarga = new ChecklistCarga(Localization.Resources.Logistica.JanelaCarregamentoTransportador.UltimaCargaCargaAtual);
    this.PenultimaCarga = new ChecklistCarga(Localization.Resources.Logistica.JanelaCarregamentoTransportador.PenultimaCarga);
    this.AntepenultimaCarga = new ChecklistCarga(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AntepenultimaCarga);
    this.DataChecklistUltima = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Data.getRequiredFieldDescription(), getType: typesKnockout.date, enable: ko.observable(false), required: true });
    this.DataChecklistPenultima = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Data.getRequiredFieldDescription(), getType: typesKnockout.date, enable: ko.observable(false), required: true });
    this.DataChecklistAntepenultima = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Data.getRequiredFieldDescription(), getType: typesKnockout.date, enable: ko.observable(false), required: true });
    this.Salvar = PropertyEntity({ eventClick: salvarChecklistClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, enable: ko.observable(false) });

    this.HabilitarCampos = (habilitar) => {
        this.Salvar.enable(habilitar);
        this.DataChecklistUltima.enable(habilitar);
        this.DataChecklistPenultima.enable(habilitar);
        this.DataChecklistAntepenultima.enable(habilitar);
        this.UltimaCarga.HabilitarCampos(habilitar);
        this.PenultimaCarga.HabilitarCampos(habilitar);
        this.AntepenultimaCarga.HabilitarCampos(habilitar);
    };

    this.ObterFormDataAnexosChecklist = () => {
        const formData = new FormData();
        this.UltimaCarga.Anexos.val().forEach(function (anexo) {
            if (isNaN(anexo.Codigo)) {
                formData.append("ArquivoUltima", anexo.Arquivo);
                formData.append("DescricaoUltima", anexo.Descricao);
            }
        });

        this.PenultimaCarga.Anexos.val().forEach(function (anexo) {
            if (isNaN(anexo.Codigo)) {
                formData.append("ArquivoPenultima", anexo.Arquivo);
                formData.append("DescricaoPenultima", anexo.Descricao);
            }
        });

        this.AntepenultimaCarga.Anexos.val().forEach(function (anexo) {
            if (isNaN(anexo.Codigo)) {
                formData.append("ArquivoAntepenultima", anexo.Arquivo);
                formData.append("DescricaoAntepenultima", anexo.Descricao);
            }
        });

        return formData;
    }
}

var ChecklistCarga = function (titulo) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Titulo = PropertyEntity({ text: titulo }); 
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TracaoCavalo.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.VeiculoCarretaUm.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.VeiculoCarretaDois.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.TerceiroReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.VeiculoCarretaTres.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.GrupoProduto.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.RegimeLimpeza = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(-1), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.RegimeLimpeza.getRequiredFieldDescription(), options: EnumRegimeLimpeza.obterOpcoes(), def: -1, enable: ko.observable(false), required: true });

    this.Anexos = PropertyEntity({ eventClick: anexosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, val: ko.observableArray() });

    this.HabilitarCampos = (habilitar) => {
        this.Veiculo.enable(habilitar);
        this.Reboque.enable(habilitar);
        this.SegundoReboque.enable(habilitar);
        this.TerceiroReboque.enable(habilitar);
        this.GrupoProduto.enable(habilitar);
        this.RegimeLimpeza.enable(habilitar);
    };
}

/*
 * Declaração das Funções de Inicialização
 */

function loadChecklist() {
    _cadastroChecklist = new CadastroChecklist();
    KoBindings(_cadastroChecklist, "knockoutCadastroChecklist");

    const somenteDisponiveis = !_CONFIGURACAO_TMS.NaoExigeInformarDisponibilidadeDeVeiculo;

    const codigoCarga = _detalhesCarga.Codigo.val();

    // Última
    BuscarVeiculos(_cadastroChecklist.UltimaCarga.Veiculo, (e) => retornoConsultaVeiculoChecklist(_cadastroChecklist.UltimaCarga, e, true),
        _detalhesCarga.Transportador, _detalhesCarga.ModeloVeiculo, null, true, null, _detalhesCarga.TipoCarga, true, somenteDisponiveis, codigoCarga, _detalhesCarga.TipoVeiculo);

    BuscarVeiculos(_cadastroChecklist.UltimaCarga.Reboque, (e) => retornoConsultaReboqueChecklist(_cadastroChecklist.UltimaCarga, e),
        _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, codigoCarga, "1");

    BuscarVeiculos(_cadastroChecklist.UltimaCarga.SegundoReboque, (e) => retornoConsultaSegundoReboqueChecklist(_cadastroChecklist.UltimaCarga, e),
        _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, codigoCarga, "1");

    BuscarVeiculos(_cadastroChecklist.UltimaCarga.TerceiroReboque, (e) => retornoConsultaTerceiroReboqueChecklist(_cadastroChecklist.UltimaCarga, e),
        _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, codigoCarga, "1");

    BuscarGruposProdutos(_cadastroChecklist.UltimaCarga.GrupoProduto, (e) => retornoGrupoProduto(_cadastroChecklist.UltimaCarga, e), null, true);

    // Penúltima
    BuscarVeiculos(_cadastroChecklist.PenultimaCarga.Veiculo, (e) => retornoConsultaVeiculoChecklist(_cadastroChecklist.PenultimaCarga, e),
        _detalhesCarga.Transportador, _detalhesCarga.ModeloVeiculo, null, true, null, _detalhesCarga.TipoCarga, true, somenteDisponiveis, codigoCarga, _detalhesCarga.TipoVeiculo);

    BuscarVeiculos(_cadastroChecklist.PenultimaCarga.Reboque, (e) => retornoConsultaReboqueChecklist(_cadastroChecklist.PenultimaCarga, e),
        _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, codigoCarga, "1");

    BuscarVeiculos(_cadastroChecklist.PenultimaCarga.SegundoReboque, (e) => retornoConsultaSegundoReboqueChecklist(_cadastroChecklist.PenultimaCarga, e),
        _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, codigoCarga, "1");

    BuscarVeiculos(_cadastroChecklist.PenultimaCarga.TerceiroReboque, (e) => retornoConsultaTerceiroReboqueChecklist(_cadastroChecklist.PenultimaCarga, e),
        _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, codigoCarga, "1");

    BuscarGruposProdutos(_cadastroChecklist.PenultimaCarga.GrupoProduto, (e) => retornoGrupoProduto(_cadastroChecklist.PenultimaCarga, e), null, true);

    // Antepenúltima
    BuscarVeiculos(_cadastroChecklist.AntepenultimaCarga.Veiculo, (e) => retornoConsultaVeiculoChecklist(_cadastroChecklist.AntepenultimaCarga, e),
        _detalhesCarga.Transportador, _detalhesCarga.ModeloVeiculo, null, true, null, _detalhesCarga.TipoCarga, true, somenteDisponiveis, codigoCarga, _detalhesCarga.TipoVeiculo);

    BuscarVeiculos(_cadastroChecklist.AntepenultimaCarga.Reboque, (e) => retornoConsultaReboqueChecklist(_cadastroChecklist.AntepenultimaCarga, e),
        _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, codigoCarga, "1");

    BuscarVeiculos(_cadastroChecklist.AntepenultimaCarga.SegundoReboque, (e) => retornoConsultaSegundoReboqueChecklist(_cadastroChecklist.AntepenultimaCarga, e),
        _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, codigoCarga, "1");

    BuscarVeiculos(_cadastroChecklist.AntepenultimaCarga.TerceiroReboque, (e) => retornoConsultaTerceiroReboqueChecklist(_cadastroChecklist.AntepenultimaCarga, e),
        _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, codigoCarga, "1");

    BuscarGruposProdutos(_cadastroChecklist.AntepenultimaCarga.GrupoProduto, (e) => retornoGrupoProduto(_cadastroChecklist.AntepenultimaCarga, e), null, true);
}

/*
 * Declaração das Funções
 */

function anexosClick(e) {
    loadAnexosChecklist(e);
    Global.abrirModal('divModalGerenciarAnexosChecklist');
}

function salvarChecklistClick() {
    let erros = 0;
    if (!ValidarCamposObrigatorios(_cadastroChecklist.UltimaCarga) || !ValidarCampoObrigatorioMap(_cadastroChecklist.DataChecklistUltima))
        erros = erros + 1;
    if (!ValidarCamposObrigatorios(_cadastroChecklist.PenultimaCarga) || !ValidarCampoObrigatorioMap(_cadastroChecklist.DataChecklistPenultima))
        erros = erros + 1;
    if (!ValidarCamposObrigatorios(_cadastroChecklist.AntepenultimaCarga) || !ValidarCampoObrigatorioMap(_cadastroChecklist.DataChecklistAntepenultima))
        erros = erros + 1;

    if (erros > 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    
    salvarChecklist();
}

function salvarChecklist() {
    const anexos = _cadastroChecklist.ObterFormDataAnexosChecklist();
    const dados = { Carga: _detalhesCarga.Codigo.val(), Checklist: JSON.stringify(obterChecklist()) };
    enviarArquivo("CargaJanelaCarregamentoChecklist/SalvarChecklist", dados, anexos, function (retorno) {
        if (retorno.Success) {
            preencherChecklist(retorno.Data.Checklist, true);
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function obterChecklist() {
    const checklist = {
        Codigo: _cadastroChecklist.Codigo.val(),
        UltimaCarga: {
            Codigo: _cadastroChecklist.UltimaCarga.Codigo.val(),
            Data: _cadastroChecklist.DataChecklistUltima.val(),
            Veiculo: _cadastroChecklist.UltimaCarga.Veiculo.codEntity(),
            Reboque: _cadastroChecklist.UltimaCarga.Reboque.codEntity(),
            SegundoReboque: _cadastroChecklist.UltimaCarga.SegundoReboque.codEntity(),
            TerceiroReboque: _cadastroChecklist.UltimaCarga.TerceiroReboque.codEntity(),
            GrupoProduto: _cadastroChecklist.UltimaCarga.GrupoProduto.codEntity(),
            RegimeLimpeza: _cadastroChecklist.UltimaCarga.RegimeLimpeza.val(),
        },
        PenultimaCarga: {
            Codigo: _cadastroChecklist.PenultimaCarga.Codigo.val(),
            Data: _cadastroChecklist.DataChecklistPenultima.val(),
            Veiculo: _cadastroChecklist.PenultimaCarga.Veiculo.codEntity(),
            Reboque: _cadastroChecklist.PenultimaCarga.Reboque.codEntity(),
            SegundoReboque: _cadastroChecklist.PenultimaCarga.SegundoReboque.codEntity(),
            TerceiroReboque: _cadastroChecklist.PenultimaCarga.TerceiroReboque.codEntity(),
            GrupoProduto: _cadastroChecklist.PenultimaCarga.GrupoProduto.codEntity(),
            RegimeLimpeza: _cadastroChecklist.PenultimaCarga.RegimeLimpeza.val(),
        },
        AntepenultimaCarga: {
            Codigo: _cadastroChecklist.AntepenultimaCarga.Codigo.val(),
            Data: _cadastroChecklist.DataChecklistAntepenultima.val(),
            Veiculo: _cadastroChecklist.AntepenultimaCarga.Veiculo.codEntity(),
            Reboque: _cadastroChecklist.AntepenultimaCarga.Reboque.codEntity(),
            SegundoReboque: _cadastroChecklist.AntepenultimaCarga.SegundoReboque.codEntity(),
            TerceiroReboque: _cadastroChecklist.AntepenultimaCarga.TerceiroReboque.codEntity(),
            GrupoProduto: _cadastroChecklist.AntepenultimaCarga.GrupoProduto.codEntity(),
            RegimeLimpeza: _cadastroChecklist.AntepenultimaCarga.RegimeLimpeza.val(),
        },
    }

    return checklist;
}

function preencherChecklist(checklist, habilitarEdicaoDados) {
    if (checklist) {
        _cadastroChecklist.Codigo.val(checklist.Codigo);

        PreencherObjetoKnout(_cadastroChecklist.UltimaCarga, { Data: checklist.UltimaCarga });
        PreencherObjetoKnout(_cadastroChecklist.PenultimaCarga, { Data: checklist.PenultimaCarga });
        PreencherObjetoKnout(_cadastroChecklist.AntepenultimaCarga, { Data: checklist.AntepenultimaCarga });

        _cadastroChecklist.UltimaCarga.Anexos.val(checklist.UltimaCarga.Anexos);
        _cadastroChecklist.PenultimaCarga.Anexos.val(checklist.PenultimaCarga.Anexos);
        _cadastroChecklist.AntepenultimaCarga.Anexos.val(checklist.AntepenultimaCarga.Anexos);

        _cadastroChecklist.DataChecklistUltima.val(checklist.UltimaCarga.DataChecklist);
        _cadastroChecklist.DataChecklistPenultima.val(checklist.PenultimaCarga.DataChecklist);
        _cadastroChecklist.DataChecklistAntepenultima.val(checklist.AntepenultimaCarga.DataChecklist);

        if (_detalhesCarga.Veiculo.codEntity() == 0) {
            _detalhesCarga.Veiculo.codEntity(checklist.UltimaCarga.Veiculo.Codigo);
            _detalhesCarga.Veiculo.val(checklist.UltimaCarga.Veiculo.Descricao);
        }

    } else {
        LimparCampos(_cadastroChecklist)
        LimparCampos(_cadastroChecklist.UltimaCarga)
        LimparCampos(_cadastroChecklist.PenultimaCarga)
        LimparCampos(_cadastroChecklist.AntepenultimaCarga)
    }

    _cadastroChecklist.HabilitarCampos(habilitarEdicaoDados);
}

function limparCamposChecklist(habilitarEdicaoDados) {
    preencherChecklist(null, habilitarEdicaoDados);
}

function retornoConsultaVeiculoChecklist(knout, veiculoSelecionado, ultima) {
    knout.Veiculo.codEntity(veiculoSelecionado.Codigo);

    if (_detalhesCarga.ExigirConfirmacaoTracao.val()) {
        knout.Veiculo.entityDescription(veiculoSelecionado.Placa);
        knout.Veiculo.val(veiculoSelecionado.Placa);
    }
    else {
        knout.Veiculo.entityDescription(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
        knout.Veiculo.val(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
    }

    if (ultima)
        retornoConsultaVeiculo(veiculoSelecionado);
}

function retornoConsultaReboqueChecklist(knout, reboqueSelecionado) {
    if (knout.SegundoReboque.codEntity() == reboqueSelecionado.Codigo || knout.TerceiroReboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.VeiculoCarretaUm, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarDuasCarretasIguais);

        LimparCampoEntity(knout.Reboque);
    }
    else {
        knout.Reboque.codEntity(reboqueSelecionado.Codigo);
        knout.Reboque.entityDescription(reboqueSelecionado.Placa);
        knout.Reboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaSegundoReboqueChecklist(knout, reboqueSelecionado) {
    if (knout.Reboque.codEntity() == reboqueSelecionado.Codigo || knout.TerceiroReboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.VeiculoCarretaDois, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarDuasCarretasIguais);

        LimparCampoEntity(knout.SegundoReboque);
    }
    else {
        knout.SegundoReboque.codEntity(reboqueSelecionado.Codigo);
        knout.SegundoReboque.entityDescription(reboqueSelecionado.Placa);
        knout.SegundoReboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaTerceiroReboqueChecklist(knout, reboqueSelecionado) {
    if (knout.Reboque.codEntity() == reboqueSelecionado.Codigo || knout.SegundoReboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.VeiculoCarretaTres, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarDuasCarretasIguais);

        LimparCampoEntity(knout.TerceiroReboque);
    }
    else {
        knout.TerceiroReboque.codEntity(reboqueSelecionado.Codigo);
        knout.TerceiroReboque.entityDescription(reboqueSelecionado.Placa);
        knout.TerceiroReboque.val(reboqueSelecionado.Placa);
    }
}

function retornoGrupoProduto(knout, grupoProdutoSelecionado) {
    if (grupoProdutoSelecionado.NaoPermitirCarregamento)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.JanelaCarregamentoTransportador.GrupoProduto, Localization.Resources.Logistica.JanelaCarregamentoTransportador.GrupoDeProdutoNaoPermiteCarregamento);

    knout.GrupoProduto.codEntity(grupoProdutoSelecionado.Codigo);
    knout.GrupoProduto.val(grupoProdutoSelecionado.Descricao);
}