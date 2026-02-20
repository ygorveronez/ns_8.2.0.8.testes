/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumTipoEnvolvidoSinistro.js" />
/// <reference path="SinistroEtapaDocumentacaoAnexo.js" />
/// <reference path="SinistroEtapaDados.js" />

var _etapaDocumentacaoSinistro,
    _gridEnvolvidos;

var DocumentacaoSinistro = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Tipo = PropertyEntity({ options: ko.observableArray(EnumTipoEnvolvidoSinistro.obterOpcoes()), def: EnumTipoEnvolvidoSinistro.Proprio, val: ko.observable(EnumTipoEnvolvidoSinistro.Proprio), text: "*Tipo:", required: true, enable: ko.observable(true) });
    this.Nome = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "*Nome:", required: true, enable: ko.observable(true) });
    this.CPF = PropertyEntity({ getType: typesKnockout.cpf, val: ko.observable(""), def: "", text: "CPF:", enable: ko.observable(true) });
    this.RG = PropertyEntity({ text: "RG:", maxlength: 50, enable: ko.observable(true) });
    this.CNH = PropertyEntity({ text: "CNH:", maxlength: 17, enable: ko.observable(true) });
    this.TelefonePrincipal = PropertyEntity({ text: "Telefone Contato 1:", getType: typesKnockout.phone, enable: ko.observable(true) });
    this.TelefoneSecundario = PropertyEntity({ text: "Telefone Contato 2:", getType: typesKnockout.phone, enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Veículo:", maxlength: 50, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ val: ko.observable(""), text: "Observação:", maxlength: 300, enable: ko.observable(true) });

    this.Envolvidos = PropertyEntity({ type: types.local, val: ko.observableArray([]), idGrid: guid() });

    this.Envolvidos.val.subscribe(function () {
        recarregarGridEnvolvidos();
    });

    this.Salvar = PropertyEntity({ type: types.event, text: "Salvar", idBtn: guid(), eventClick: salvarEnvolvidoClick, visible: ko.observable(true) });

    //CRUD
    this.IniciarManutencao = PropertyEntity({ eventClick: iniciarManutencaoClick, type: types.event, text: "Iniciar Manutenção", visible: ko.observable(true) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaDocumentacaoClick, type: types.event, text: "Voltar Etapa", visible: ko.observable(false) });

    //Anexos
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridDocumentacaoAnexo();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentacaoModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true), enable: ko.observable(true) });
};

function loadEtapaDocumentacaoSinistro() {
    _etapaDocumentacaoSinistro = new DocumentacaoSinistro();
    KoBindings(_etapaDocumentacaoSinistro, "knockoutFluxoSinistroDocumentacao");

    $("#" + _etapaDocumentacaoSinistro.CNH.id).mask("09999999999999999", { selectOnFocus: true, clearIfNotMatch: true });

    loadGridEnvolvidosSinistro();
    loadEtapaDocumentacaoAnexo();
}

function loadGridEnvolvidosSinistro() {
    var linhasPorPagina = 2;

    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarEnvolvidoClick, icone: "" };
    var opcaoExcluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirEnvolvidoClick, icone: "", visibilidade: obterVisibilidadeExcluirEnvolvidoDocumentacao };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar, opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "RG", visible: false },
        { data: "CPF", visible: false },
        { data: "CNH", visible: false },
        { data: "TelefonePrincipal", visible: false },
        { data: "TelefoneSecundario", visible: false },
        { data: "Observacao", visible: false },
        { data: "TipoDescricao", title: "Tipo", width: "25%", className: "text-align-left" },
        { data: "Nome", title: "Nome", width: "25%", className: "text-align-left" },
        { data: "Veiculo", title: "Veículo", width: "25%", className: "text-align-left" }
    ];

    _gridEnvolvidos = new BasicDataTable(_etapaDocumentacaoSinistro.Envolvidos.idGrid, header, menuOpcoes, null, null, linhasPorPagina);
    _gridEnvolvidos.CarregarGrid([]);
}

function obterVisibilidadeExcluirEnvolvidoDocumentacao() {
    return _etapaDadosSinistro.Etapa.val() == EnumEtapaSinistro.Documentacao;
}

function salvarEnvolvidoClick() {
    if (!ValidarCamposObrigatorios(_etapaDocumentacaoSinistro)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios");
        return;
    }

    var data = {
        Codigo: (_etapaDocumentacaoSinistro.Codigo.val() == 0 ? guid() : _etapaDocumentacaoSinistro.Codigo.val()),
        Tipo: _etapaDocumentacaoSinistro.Tipo.val(),
        Nome: _etapaDocumentacaoSinistro.Nome.val(),
        RG: _etapaDocumentacaoSinistro.RG.val(),
        CPF: _etapaDocumentacaoSinistro.CPF.val(),
        CNH: _etapaDocumentacaoSinistro.CNH.val(),
        TelefonePrincipal: _etapaDocumentacaoSinistro.TelefonePrincipal.val(),
        TelefoneSecundario: _etapaDocumentacaoSinistro.TelefoneSecundario.val(),
        Observacao: _etapaDocumentacaoSinistro.Observacao.val(),
        TipoDescricao: EnumTipoEnvolvidoSinistro.obterDescricao(_etapaDocumentacaoSinistro.Tipo.val()),
        Veiculo: _etapaDocumentacaoSinistro.Veiculo.val()
    };

    executarReST("Sinistro/SalvarEnvolvido", $.extend(true, { CodigoSinistro: _etapaDadosSinistro.Codigo.val() }, data), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (data.Codigo == retorno.Data.CodigoEnvolvido) {
                    var registrosGrid = _etapaDocumentacaoSinistro.Envolvidos.val();
                    for (var i = 0; i < registrosGrid.length; i++) {
                        if (registrosGrid[i].Codigo == data.Codigo) {
                            registrosGrid.splice(i, 1, data);
                            _etapaDocumentacaoSinistro.Envolvidos.val(registrosGrid);
                            break;
                        }
                    }
                }
                else {
                    data.Codigo = retorno.Data.CodigoEnvolvido;
                    _etapaDocumentacaoSinistro.Envolvidos.val.push(data);
                }

                LimparCampos(_etapaDocumentacaoSinistro);
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function editarEnvolvidoClick(registroEditar) {
    var data = {
        Codigo: registroEditar.Codigo,
        Tipo: registroEditar.Tipo,
        RG: registroEditar.RG,
        CPF: registroEditar.CPF,
        Nome: registroEditar.Nome,
        Observacao: registroEditar.Observacao,
        Veiculo: registroEditar.Veiculo,
        CNH: registroEditar.CNH,
        TelefonePrincipal: registroEditar.TelefonePrincipal,
        TelefoneSecundario: registroEditar.TelefoneSecundario
    };

    PreencherObjetoKnout(_etapaDocumentacaoSinistro, { Data: data });
}

function excluirEnvolvidoClick(registroExcluir) {
    executarReST("Sinistro/ExcluirEnvolvido", { Codigo: registroExcluir.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var registros = _gridEnvolvidos.BuscarRegistros();

                for (var i = 0; i < registros.length; i++) {
                    if (registros[i].Codigo == registroExcluir.Codigo) {
                        registros.splice(i, 1);
                        _etapaDocumentacaoSinistro.Envolvidos.val(registros);
                        break;
                    }
                }

                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function iniciarManutencaoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente avançar a etapa de Documentação?", function () {
        executarReST("Sinistro/AvancarEtapa", { Codigo: _etapaDadosSinistro.Codigo.val(), Etapa: EnumEtapaSinistro.Manutencao }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    limparFluxoSinistro();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo avançado com sucesso.");
                    CarregarDadosSinistro(retorno.Data.Codigo);
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function voltarEtapaDocumentacaoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente voltar para a etapa Dados?", function () {
        executarReST("Sinistro/VoltarEtapa", { Codigo: _etapaDadosSinistro.Codigo.val(), Etapa: EnumEtapaSinistro.Dados }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo voltado com sucesso.");
                    recarregarGridSinistro();
                    editarSinistroClick({ Codigo: _etapaDadosSinistro.Codigo.val() });
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function recarregarGridEnvolvidos() {
    _gridEnvolvidos.CarregarGrid(_etapaDocumentacaoSinistro.Envolvidos.val());
}

function preencherEtapaDocumentacao(dados) {
    PreencherObjetoKnout(_etapaDocumentacaoSinistro, { Data: dados });

    _etapaDocumentacaoSinistro.Envolvidos.val(dados.Envolvidos);
    _etapaDocumentacaoSinistro.Anexos.val(dados.Anexos);

    if (_etapaDadosSinistro.Etapa.val() !== EnumEtapaSinistro.Documentacao) {
        SetarEnableCamposKnockout(_etapaDocumentacaoSinistro, false);

        _etapaDocumentacaoSinistro.IniciarManutencao.visible(false);
        _etapaDocumentacaoSinistro.VoltarEtapa.visible(false);
        _etapaDocumentacaoSinistro.Salvar.visible(false);
    }
}

function limparCamposSinistroEtapaDocumentacao() {
    LimparCampos(_etapaDocumentacaoSinistro);
    SetarEnableCamposKnockout(_etapaDocumentacaoSinistro, true);

    _etapaDocumentacaoSinistro.IniciarManutencao.visible(true);
    _etapaDocumentacaoSinistro.VoltarEtapa.visible(true);
    _etapaDocumentacaoSinistro.Salvar.visible(true);
}