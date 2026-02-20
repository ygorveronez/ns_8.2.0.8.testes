/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeAtendimento.js" />
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
/// <reference path="../../Consultas/Sistema.js" />
/// <reference path="../../Consultas/Modulo.js" />
/// <reference path="../../Consultas/Tela.js" />
/// <reference path="AtendimentoEtapa.js" />
/// <reference path="AtendimentoChamado.js" />
/// <reference path="../../Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _prioridadeConsulta = [
    { text: "Todos", value: EnumPrioridadeAtendimento.Todos },
    { text: "Baixa", value: EnumPrioridadeAtendimento.Baixa },
    { text: "Normal", value: EnumPrioridadeAtendimento.Normal },
    { text: "Alta", value: EnumPrioridadeAtendimento.Alta }
];

var _gridAtendimento;
var _atendimento;
var _pesquisaAtendimento;
var _botoesAtencimento;

var PesquisaAtendimento = function () {
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Atendente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroInicial = PropertyEntity({ text: "Número do chamado de: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false } });
    this.NumeroFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false } });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data abertura do chamado de: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.date });

    this.MotivoProblema = PropertyEntity({ text: "Motivo / Problema: " });
    this.Titulo = PropertyEntity({ text: "Título: " });
    this.Prioridade = PropertyEntity({ val: ko.observable(EnumPrioridadeAtendimento.Todos), options: _prioridadeConsulta, def: EnumPrioridadeAtendimento.Todos, text: "Prioridade: " });
    this.Status = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "Atendimento/ObterTodosStatus", params: { Tipo: 0, Ativo: _statusPesquisa.Todos }, text: "Status: ", options: ko.observable(new Array()), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAtendimento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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

var Atendimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataAberturaChamado = PropertyEntity({ text: "Data da abertura do chamado: ", val: ko.observable(""), def: ko.observable(""), enable: ko.observable(false) });
    this.DataAtendimento = PropertyEntity({ text: "Data do atendimento: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.dateTime, required: true });
    this.Numero = PropertyEntity({ text: "Número: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: ko.observable(false) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Atendente:", idBtnSearch: guid(), visible: ko.observable(true), required: true });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Cliente:", idBtnSearch: guid(), visible: ko.observable(true), required: true, enable: ko.observable(false) });

    this.PessoaContato = PropertyEntity({ text: "*Pessoa que recebeu o atendimento: ", required: true, maxlength: 5000 });
    this.ObservacaoSuporte = PropertyEntity({ text: "Observação do Suporte: ", maxlength: 5000 });

    this.ListaAnexos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosNovos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosExcluidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.ListaRespostas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaRespostasNovas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaRespostasExcluidas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

var BotoesAtencimento = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadAtendimento() {
    _atendimento = new Atendimento();
    KoBindings(_atendimento, "knockoutAtendimento");

    _botoesAtencimento = new BotoesAtencimento();
    KoBindings(_botoesAtencimento, "knockoutBotoes");

    _pesquisaAtendimento = new PesquisaAtendimento();
    KoBindings(_pesquisaAtendimento, "knockoutPesquisaAtendimento", false, _pesquisaAtendimento.Pesquisar.id);

    new BuscarFuncionario(_pesquisaAtendimento.Funcionario);    
    new BuscarTransportadores(_pesquisaAtendimento.Empresa, null, null, true);
    new BuscarFuncionario(_atendimento.Funcionario);    
    new BuscarTransportadores(_atendimento.Empresa, null, null, true);

    buscarAtendimentos();
    loadEtapaAtendimento();
    loadAtendimentoChamado();

    DesabilitarCamposInstancias(_atendimento);
    DesabilitarCamposInstancias(_atendimentoChamado);
}

function atualizarClick(e, sender) {
    if (ValidarCamposObrigatorios(_atendimento)) {
        if (ValidarCamposObrigatorios(_atendimentoChamado)) {
            var data = {
                Codigo: _atendimento.Codigo.val(),
                DataAberturaChamado: _atendimento.DataAberturaChamado.val(),
                DataAtendimento: _atendimento.DataAtendimento.val(),
                Numero: _atendimento.Numero.val(),
                Funcionario: _atendimento.Funcionario.codEntity(),
                Empresa: _atendimento.Empresa.codEntity(),
                PessoaContato: _atendimento.PessoaContato.val(),
                ObservacaoSuporte: _atendimento.ObservacaoSuporte.val(),
                ListaAnexos: JSON.stringify(_atendimento.ListaAnexos.list),
                ListaAnexosNovos: JSON.stringify(_atendimento.ListaAnexosNovos.list),
                ListaAnexosExcluidos: JSON.stringify(_atendimento.ListaAnexosExcluidos.list),
                CodigoTarefa: _atendimentoChamado.CodigoTarefa.val(),
                Tela: _atendimentoChamado.Tela.codEntity(),
                Modulo: _atendimentoChamado.Modulo.codEntity(),
                Sistema: _atendimentoChamado.Sistema.codEntity(),
                TipoAtendimento: _atendimentoChamado.TipoAtendimento.codEntity(),
                StatusChamado: _atendimentoChamado.StatusChamado.val(),
                PrioridadeChamado: _atendimentoChamado.PrioridadeChamado.val(),
                MotivoChamado: _atendimentoChamado.MotivoChamado.val(),
                SolucaoChamado: _atendimentoChamado.SolucaoChamado.val(),
                ListaRespostas: JSON.stringify(_atendimento.ListaRespostas.list),
                ListaRespostasNovas: JSON.stringify(_atendimento.ListaRespostasNovas.list),
                ListaRespostasExcluidas: JSON.stringify(_atendimento.ListaRespostasExcluidas.list)
            };

            executarReST("Atendimento/Atualizar", data, function (arg) {
                if (arg.Success) {
                    if (_atendimento.ListaAnexosNovos.list.length > 0)
                        EnviarAnexos(data.CodigoTarefa);
                    else {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                        _gridAtendimento.CarregarGrid();
                        limparCamposAtendimento();
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        } else {
            exibirCamposObrigatorio();
        }
    } else {
        exibirCamposObrigatorio();
    }
}

function cancelarClick(e) {
    limparCamposAtendimento();
}

//*******MÉTODOS*******

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function EnviarAnexos(codigoChamado) {
    var file;
    var fileCount = _atendimento.ListaAnexosNovos.list.length;
    var documentos = new Array();

    for (var i = 0; i < _atendimento.ListaAnexosNovos.list.length; i++) {
        file = _atendimento.ListaAnexosNovos.list[i].Arquivo;
        var formData = new FormData();
        formData.append("upload", file);
        var data = {
            ListaAnexos: "",
            DescricaoAnexo: _atendimento.ListaAnexosNovos.list[i].DescricaoAnexo.val,
            CodigoChamado: codigoChamado
        };
        enviarArquivo("Chamado/EnviarAnexos?callback=?", data, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    documentos.push({ Codigo: codigoChamado });
                    if (documentos.length == fileCount) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                        _gridAtendimento.CarregarGrid();
                        limparCamposAtendimento();
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function buscarAtendimentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAtendimento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAtendimento = new GridView(_pesquisaAtendimento.Pesquisar.idGrid, "Atendimento/Pesquisa", _pesquisaAtendimento, menuOpcoes, null);
    _gridAtendimento.CarregarGrid();
}

function editarAtendimento(atendimentoGrid) {
    limparCamposAtendimento();
    _atendimento.Codigo.val(atendimentoGrid.Codigo);
    BuscarPorCodigo(_atendimento, "Atendimento/BuscarPorCodigo", function (arg) {

        var dataChamado = { Data: arg.Data };
        PreencherObjetoKnout(_atendimentoChamado, dataChamado);

        _pesquisaAtendimento.ExibirFiltros.visibleFade(false);
        _botoesAtencimento.Atualizar.visible(true);
        _botoesAtencimento.Cancelar.visible(true);
        HabilitarCamposInstancias(_atendimento);
        HabilitarCamposInstancias(_atendimentoChamado);

        if (_atendimento.DataAtendimento.val() == "" || _atendimento.DataAtendimento.val() == undefined) {
            _atendimento.DataAtendimento.val(Global.DataHoraAtual());
        }

        recarregarGridListaAnexosChamado();
        recarregarGridListaRespostasChamado();
        buscarFuncionarioLogado();
    }, null);
}

function limparCamposAtendimento() {
    _botoesAtencimento.Atualizar.visible(false);
    _botoesAtencimento.Cancelar.visible(false);
    LimparCampos(_atendimento);
    LimparCampos(_atendimentoChamado);
    _atendimentoChamado.Arquivo.val("");
    _atendimento.DataAberturaChamado.val("");
    _atendimento.DataAtendimento.val("");
    _atendimento.Numero.val("");
    recarregarGridListaAnexosChamado();
    recarregarGridListaRespostasChamado();
    DesabilitarCamposInstancias(_atendimento);
    DesabilitarCamposInstancias(_atendimentoChamado);
    resetarTabs();
}

function HabilitarCamposInstancias(instancia) {
    //$.each(instancia, function (i, knout) {
    //    if (knout.enable != null) {
    //        if (knout.enable === true || knout.enable === false)
    //            knout.enable = true;
    //        else
    //            knout.enable(true);
    //    }
    //});
}

function DesabilitarCamposInstancias(instancia) {
    //$.each(instancia, function (i, knout) {
    //    if (knout.enable != null) {
    //        if (knout.enable === true || knout.enable === false)
    //            knout.enable = false;
    //        else
    //            knout.enable(false);
    //    }
    //});
}

function buscarFuncionarioLogado() {
    executarReST("PedidoVenda/BuscarFuncionarioLogado", null, function (r) {
        if (r.Success) {
            _atendimento.Funcionario.codEntity(r.Data.Codigo);
            _atendimento.Funcionario.val(r.Data.Nome);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function resetarTabs() {
    $("#myTab a:eq(0)").tab("show");
    //$("#step1").click();
    //$("#step1 a:eq(0)").tab("show");    
}