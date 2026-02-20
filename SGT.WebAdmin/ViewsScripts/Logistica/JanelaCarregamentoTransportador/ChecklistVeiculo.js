/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../Consultas/GrupoProduto.js" />
/// <reference path="../../Enumeradores/EnumRegimeLimpeza.js" />
/// <reference path="../../Enumeradores/EnumOrdemCargaChecklist.js" />
/// <reference path="AnexosChecklist.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

let _cadastroChecklistVeiculo;

/*
 * Declaração das Classes
 */

var CadastroChecklistVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, id: guid() });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CargasChecklist = PropertyEntity({ val: ko.observableArray([]), def: ko.observableArray([]) });
    this.Salvar = PropertyEntity({ eventClick: salvarChecklistClickVeiculo, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, enable: ko.observable(true) });

    this.HabilitarCampos = (habilitar) => {
        this.Salvar.enable(habilitar);
    };
}

var ChecklistCarga = function (titulo) {
    this.CodigoChecklist = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Titulo = PropertyEntity({ text: titulo });
    this.DataChecklist = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Data.getRequiredFieldDescription(), val: ko.observable(), getType: typesKnockout.date, enable: ko.observable(false), required: true });
    this.RegimeLimpeza = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(EnumRegimeLimpeza.Nenhum), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.RegimeLimpeza.getRequiredFieldDescription(), options: EnumRegimeLimpeza.obterOpcoes(), def: EnumRegimeLimpeza.Nenhum, enable: ko.observable(false), required: true });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.GrupoProduto.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.OrdemCargaChecklist = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(1), options: EnumOrdemCargaChecklist.obterOpcoes(), def: -1 });

    this.Anexos = PropertyEntity({ eventClick: anexosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, val: ko.observableArray() });

    this.HabilitarCampos = (habilitar) => {
        this.GrupoProduto.enable(habilitar);
        this.RegimeLimpeza.enable(habilitar);
        this.DataChecklist.enable(habilitar);
    };
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCadastroChecklistVeiculo() {
    _cadastroChecklistVeiculo = new CadastroChecklistVeiculo();
    KoBindings(_cadastroChecklistVeiculo, "knockoutCadastroChecklist");


}

async function carregarChecklistVeiculo(cargaJanelaCarregamento, veiculo) {
    let data = { CodigoJanelaCarregamentoTransportador: cargaJanelaCarregamento, CodigoVeiculo: veiculo }

    _cadastroChecklistVeiculo.CodigoJanelaCarregamentoTransportador.val(cargaJanelaCarregamento);
    _cadastroChecklistVeiculo.CodigoVeiculo.val(veiculo);

    if (_cadastroChecklistVeiculo.CodigoVeiculo.val() === 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Escolha um veiculo!");
        return;
    }

    executarReST("CargaJanelaCarregamentoTransportadorChecklist/ObterDadosChecklistVeiculos", data, async function (retorno) {
        if (retorno.Success && retorno.Data) {
            let numeroMinimoCargasChecklist = EnumOrdemCargaChecklist.obterOpcoes();
            for (let i = 0; i < numeroMinimoCargasChecklist.length; i++) {
                let item = retorno.Data.CargaJanelaCarregamentoTransportadorChecklist.filter(checklist => checklist.OrdemCargaChecklist == i + 1)[0];
                await adicionarCargaChecklistVeiculo(item, true, i + 1);
            }
            Global.abrirModal('divModalChecklistGMP');
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

/*
 * Declaração das Funções
 */

function anexosClick(e) {
    loadAnexosChecklist(e);
    Global.abrirModal('divModalGerenciarAnexosChecklist');
}

function salvarChecklistClickVeiculo() {
    for (let checklist of _cadastroChecklistVeiculo.CargasChecklist.val()) {

        if (checklist.Anexos.val().length == 0) {
            return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios + " Verifique todas as cargas se não está faltando algum anexo");
        }

        if (!ValidarCamposObrigatoriosChecklist(checklist)) {
            return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios + "Checklist da " + EnumOrdemCargaChecklist.obterDescricao(checklist.OrdemCargaChecklist.val()));
        }
    }
    salvarChecklistVeiculo();
}

function salvarChecklistVeiculo() {
    const dados = {
        CodigoJanelaCarregamentoTransportador: _cadastroChecklistVeiculo.CodigoJanelaCarregamentoTransportador.val(),
        CodigoVeiculo: _cadastroChecklistVeiculo.CodigoVeiculo.val(),
        Checklist: JSON.stringify(obterChecklistVeiculo()),
    };
    executarReST("CargaJanelaCarregamentoTransportadorChecklist/SalvarChecklist", dados, function (retorno) {
        if (retorno.Success) {
            let data = retorno.Data;
            Global.fecharModal('divModalChecklistGMP');

            if (data.Inseriu) {
                enviarArquivos(_cadastroChecklistVeiculo.CargasChecklist.val(), data.Checklist);
            }
        
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function obterChecklistVeiculo() {
    const cargasChecklist = [];
    const cargaArray = _cadastroChecklistVeiculo.CargasChecklist.val().sort((a, b) => a.OrdemCargaChecklist - b.OrdemCargaChecklist);

    for (const carga of cargaArray) {
        cargasChecklist.push({
            CodigoChecklist: carga.CodigoChecklist.val(),
            DataChecklist: carga.DataChecklist.val(),
            RegimeLimpeza: carga.RegimeLimpeza.val(),
            GrupoProduto: {
                Codigo: carga.GrupoProduto.codEntity(),
                Descricao: carga.GrupoProduto.val()
            },
            OrdemCargaChecklist: carga.OrdemCargaChecklist.val(),
        });
    }

    return cargasChecklist;
}

function limparCamposChecklistVeiculo() {
    $("#divChecklistCarga").empty();
    LimparCampos(_cadastroChecklistVeiculo);
    _cadastroChecklistVeiculo.CargasChecklist.val([]);
}

async function adicionarCargaChecklistVeiculo(checklist, enable, enumOrdem) {
    await $.get("Content/Static/Logistica/JanelaCarregamentoTransportador/ChecklistCarga.html?dyn=" + guid(), function (html) {
        let _checklistCarga = new ChecklistCarga(EnumOrdemCargaChecklist.obterDescricao(enumOrdem));

        if (checklist) {
            PreencherObjetoKnout(_checklistCarga, { Data: checklist });
        } else {
            _checklistCarga.OrdemCargaChecklist.val(enumOrdem);
        }

        let guid = (_checklistCarga.CodigoChecklist.val() > 0 ? _checklistCarga.CodigoChecklist.val() : _checklistCarga.CodigoChecklist.id);
        let knockoutGeracaoLaudoProduto = "knockoutCargaChecklistVeiculo";
        let knockoutGeracaoLaudoProdutoDinamico = knockoutGeracaoLaudoProduto + guid;

        html = html.replaceAll(knockoutGeracaoLaudoProduto, knockoutGeracaoLaudoProdutoDinamico);

        $("#divChecklistCarga").append(html);

        KoBindings(_checklistCarga, knockoutGeracaoLaudoProdutoDinamico);

        BuscarGruposProdutos(_checklistCarga.GrupoProduto, (e) => retornoGrupoProdutoVeiculo(_checklistCarga.GrupoProduto, e), null, true);

        SetarEnableCamposKnockout(_checklistCarga, enable);

        _cadastroChecklistVeiculo.CargasChecklist.val().push(_checklistCarga);
    });
}

function ValidarCamposObrigatoriosChecklist(checklist) {
    let valido = true;
    if (!ValidarCamposObrigatorios(checklist)) {
        valido = false;
    }
    if (checklist.RegimeLimpeza.val() == EnumRegimeLimpeza.Nenhum) {
        checklist.RegimeLimpeza.requiredClass("form-control is-invalid");
        valido = false;
    }

    return valido;
}

function retornoGrupoProdutoVeiculo(knout, grupoProdutoSelecionado) {
    if (grupoProdutoSelecionado.NaoPermitirCarregamento)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.JanelaCarregamentoTransportador.GrupoProduto, Localization.Resources.Logistica.JanelaCarregamentoTransportador.GrupoDeProdutoNaoPermiteCarregamento);

    knout.codEntity(grupoProdutoSelecionado.Codigo);
    knout.val(grupoProdutoSelecionado.Descricao);
}