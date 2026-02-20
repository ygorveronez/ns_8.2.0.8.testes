/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoOfertaHUB.js" />

// #region Objetos Globais do Arquivo

var _cadastroGrupo;
var _grupo;
var _gridGrupoTransportadores;
var _gridTransportadoresGrupo;
var _listaSequenciasGrupo = [];

// #endregion Objetos Globais do Arquivo

// #region Classes

var CadastroGrupoTransportadoresHUBOfertas = function () {
    var self = this;
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.text, val: ko.observable(""), text: ko.observable("*Descrição grupo:"), required: true });
    this.SequenciaOferta = PropertyEntity({ text: "*Sequência de Oferta:", options: ko.observable(_listaSequenciasGrupo), val: ko.observable(""), def: "", visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.TempoDeixarDeOfertarAntesDoCarregamento = PropertyEntity({ type: types.text, val: ko.observable(""), visible: ko.observable(false), text: ko.observable("*Tempo para deixar de ofertar antes do carregamento (min):"), required: ko.observable(false) });
    this.TempoOfertarExclusivamenteParaGrupo = PropertyEntity({ type: types.text, val: ko.observable(""), visible: ko.observable(true), text: ko.observable("Tempo para ofertar exclusivamente para o grupo (min):") });
    this.Transportadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Empresa = PropertyEntity({ text: ko.observable("Transportadores:"), codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ListaTransportadores = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
    this.AdicionarTransportador = PropertyEntity({ eventClick: adicionarTransportadorClick, type: types.event, text: ko.observable("Adicionar Transportador"), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarGrupoClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarGrupoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarGrupoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirGrupoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var GrupoTransportadoresHUBOfertas = function () {
    this.TipoOferta = PropertyEntity({ text: ko.observable("*Tipo de Oferta:"), val: ko.observable(EnumTipoOfertaHUB.Exclusiva), options: EnumTipoOfertaHUB.obterOpcoes(), required: true });
    this.LiberarSpotAbertoAposTempoLimiteGrupos = PropertyEntity({ type: types.boolean, val: ko.observable(false), visible: ko.observable(true), text: ko.observable("Liberar para spot após tempo limite dos grupos:") });
    this.DiasAntecedenciaHUBOfertas = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: "Dias de Antecedência:", visible: ko.observable(true) });
    this.HoraEnvioOfertaHUBOfertas = PropertyEntity({ getType: typesKnockout.time, val: ko.observable(""), def: "", text: "Hora de Envio:", visible: ko.observable(true) });
    this.EnviarTransportadorRotaSegundaHUB = PropertyEntity({ getType: typesKnockout.bool, text: "Segunda-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaTercaHUB = PropertyEntity({ getType: typesKnockout.bool, text: "Terça-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaQuartaHUB = PropertyEntity({ getType: typesKnockout.bool, text: "Quarta-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaQuintaHUB = PropertyEntity({ getType: typesKnockout.bool, text: "Quinta-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaSextaHUB = PropertyEntity({ getType: typesKnockout.bool, text: "Sexta-Feira", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaSabadoHUB = PropertyEntity({ getType: typesKnockout.bool, text: "Sábado", val: ko.observable(false), def: false });
    this.EnviarTransportadorRotaDomingoHUB = PropertyEntity({ getType: typesKnockout.bool, text: "Domingo", val: ko.observable(false), def: false });

    this.ListaGrupo = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarGrupoModalClick, type: types.event, text: ko.observable("Adicionar Grupo"), visible: true });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGrupoTransportadoresHUBOfertas() {
    _grupo = new GrupoTransportadoresHUBOfertas();
    KoBindings(_grupo, "knockoutGrupoTransportadoresHUBOfertas");

    _cadastroGrupo = new CadastroGrupoTransportadoresHUBOfertas();
    KoBindings(_cadastroGrupo, "knockoutCadastroGrupoTransportadoresHUBOfertas");

    new BuscarTransportadores(_cadastroGrupo.Empresa);

    loadGridGrupoTransportadoresHUBOfertas();
    loadGridTransportadoresGrupo();
}

function loadGridGrupoTransportadoresHUBOfertas() {
    var ordenacaoPadrao = { column: 3, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarGrupoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Transportadores", title: "Transportadores", visible: false },
        { data: "DescricaoEmpresa", title: "Transportador", width: "25%" },
        { data: "Descricao", title: "Descrição do Grupo", width: "25%" },
        { data: "SequenciaOferta", title: "Sequência de Oferta", width: "15%", className: "text-align-center" },
        { data: "TempoOfertarExclusivamenteParaGrupo", title: "Tempo Exclusivo (min)", width: "15%", className: "text-align-center" }
    ];

    _gridGrupoTransportadores = new BasicDataTable(_grupo.ListaGrupo.idGrid, header, menuOpcoes, ordenacaoPadrao);
    _gridGrupoTransportadores.CarregarGrid([]);
}

function loadGridTransportadoresGrupo() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirTransportadorGrupoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoEmpresa", title: "Transportador", width: "70%" }
    ];

    _gridTransportadoresGrupo = new BasicDataTable(_cadastroGrupo.ListaTransportadores.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridTransportadoresGrupo.CarregarGrid([]);
}

function carregarSequenciasGrupoTransportadores(adicionar) {
    var codigoConfiguracaoRotaFrete = _configuracaoRotaFrete?.Codigo?.val() || 0;

    if (codigoConfiguracaoRotaFrete > 0) {
        executarReST("ConfiguracaoRotaFrete/ContarGruposPorConfiguracaoRotaFrete",
            { CodigoConfiguracaoRotaFrete: codigoConfiguracaoRotaFrete },
            function (retorno) {
                if (retorno.Success && retorno.Data) {
                    var totalGrupos = parseInt(retorno.Data);
                    _listaSequenciasGrupo = [];

                    for (var i = 1; i <= totalGrupos + (adicionar ? 1 : 0); i++) {
                        _listaSequenciasGrupo.push({
                            text: i.toString(),
                            value: i
                        });
                    }
                    
                    var gruposEmTela = obterListaGrupoTransportadoresHUBOfertas();
                    if (gruposEmTela && gruposEmTela.length > 0) {
                        var sequenciasUsadas = {};
                        
                        _listaSequenciasGrupo.forEach(function(seq) {
                            sequenciasUsadas[seq.value] = true;
                        });
                        
                        gruposEmTela.forEach(function(grupo) {
                            var sequenciaGrupo = parseInt(grupo.SequenciaOferta);
                            if (sequenciaGrupo && !sequenciasUsadas[sequenciaGrupo]) {
                                _listaSequenciasGrupo.push({
                                    text: sequenciaGrupo.toString(),
                                    value: sequenciaGrupo
                                });
                                sequenciasUsadas[sequenciaGrupo] = true;
                            }
                        });
                        
                        _listaSequenciasGrupo.sort(function(a, b) {
                            return a.value - b.value;
                        });
                    }

                    _cadastroGrupo.SequenciaOferta.options(_listaSequenciasGrupo);
                } else {
                    console.error("Erro ao carregar sequências de grupos:", retorno.Msg);
                }
            }
        );
    } else if (adicionar) {
        if (!_listaSequenciasGrupo || _listaSequenciasGrupo.length === 0) {
            _listaSequenciasGrupo = [{
                text: "1",
                value: 1
            }];
        } else {
            const maxValue = Math.max(..._listaSequenciasGrupo.map(x => x.value));
            _listaSequenciasGrupo.push({
                text: (maxValue + 1).toString(),
                value: maxValue + 1
            });
        }

        _cadastroGrupo.SequenciaOferta.options(_listaSequenciasGrupo);
    }
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarTransportadorClick() {
    if (!_cadastroGrupo.Empresa.codEntity() || _cadastroGrupo.Empresa.codEntity() <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Campo obrigatório", "Selecione um transportador");
        return;
    }

    var novoTransportador = {
        Codigo: _cadastroGrupo.Empresa.codEntity(),
        DescricaoEmpresa: _cadastroGrupo.Empresa.val()
    };

    var transportadores = _gridTransportadoresGrupo.BuscarRegistros();
    transportadores.push(novoTransportador);
    _cadastroGrupo.ListaTransportadores.val(transportadores);

    _gridTransportadoresGrupo.CarregarGrid(transportadores);
    _cadastroGrupo.Empresa.codEntity(0);
    _cadastroGrupo.Empresa.val("");
}

function excluirTransportadorGrupoClick(registroSelecionado) {
    var transportadores = _gridTransportadoresGrupo.BuscarRegistros();

    for (var i = 0; i < transportadores.length; i++) {
        if (transportadores[i].Codigo == registroSelecionado.Codigo) {
            transportadores.splice(i, 1);
            break;
        }
    }

    _gridTransportadoresGrupo.CarregarGrid(transportadores);
}

function adicionarGrupoClick() {
    if (!validarCadastroGrupoTransportadoresHUBOfertas())
        return;

    var transportadores = _gridTransportadoresGrupo.BuscarRegistros();

    var novoGrupo = obterCadastroGrupoSalvar();
    novoGrupo.Transportadores = transportadores;

    var listaGrupo = obterListaGrupoTransportadoresHUBOfertas();
    listaGrupo.push(novoGrupo);
    _grupo.ListaGrupo.val(listaGrupo);

    recarregarGridGrupoTransportadoresHUBOfertas();
    fecharModalCadastroGrupoTransportadoresHUBOfertas();
}

function adicionarGrupoModalClick() {
    _cadastroGrupo.Codigo.val(guid());

    controlarBotoesGrupoTransportadoresHUBOfertasHabilitados(false);
    carregarSequenciasGrupoTransportadores(true);

    _gridTransportadoresGrupo.CarregarGrid([]);
    exibirModalCadastroGrupoTransportadoresHUBOfertas();
}

function atualizarGrupoClick() {
    if (!validarCadastroGrupoTransportadoresHUBOfertas())
        return;

    var listaGrupo = obterListaGrupoTransportadoresHUBOfertas();
    var transportadores = _gridTransportadoresGrupo.BuscarRegistros();

    var grupoAtualizado = obterCadastroGrupoSalvar();
    grupoAtualizado.Transportadores = transportadores;

    for (var i = 0; i < listaGrupo.length; i++) {
        if (listaGrupo[i].Codigo == _cadastroGrupo.Codigo.val()) {
            listaGrupo.splice(i, 1, grupoAtualizado);
            break;
        }
    }

    _grupo.ListaGrupo.val(listaGrupo);

    recarregarGridGrupoTransportadoresHUBOfertas();
    fecharModalCadastroGrupoTransportadoresHUBOfertas();
}

function cancelarGrupoClick() {
    fecharModalCadastroGrupoTransportadoresHUBOfertas();
}
function ObterDadosGridTransportadores(registroSelecionado) {
    var transportadoresExistentes = [];
    var CodigoGrupo = registroSelecionado?.Codigo || 0;
    if (registroSelecionado.Transportadores && registroSelecionado.Transportadores.length > 0)
        transportadoresExistentes = registroSelecionado.Transportadores;


    _gridTransportadoresGrupo.CarregarGrid(registroSelecionado.Transportadores);

}

function editarGrupoClick(registroSelecionado) {
    carregarSequenciasGrupoTransportadores(false);
    
    preencherDadosGrupoTransportadoresHUBOfertas(registroSelecionado);
    ObterDadosGridTransportadores(registroSelecionado);

    controlarBotoesGrupoTransportadoresHUBOfertasHabilitados(true);
    exibirModalCadastroGrupoTransportadoresHUBOfertas();
}

function excluirGrupoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir este grupo de transportadores?", function () {
        var listaGrupo = obterListaGrupoTransportadoresHUBOfertas();

        for (var i = 0; i < listaGrupo.length; i++) {
            if (listaGrupo[i].Codigo == _cadastroGrupo.Codigo.val()) {
                listaGrupo.splice(i, 1);
            }
        }

        _grupo.ListaGrupo.val(listaGrupo);

        recarregarGridGrupoTransportadoresHUBOfertas();
        fecharModalCadastroGrupoTransportadoresHUBOfertas();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposGrupoTransportadoresHUBOfertas() {
    _grupo.ListaGrupo.val([]);
    _grupo.LiberarSpotAbertoAposTempoLimiteGrupos.val(false);
    _grupo.DiasAntecedenciaHUBOfertas.val(0);
    _grupo.HoraEnvioOfertaHUBOfertas.val("");
    _grupo.EnviarTransportadorRotaSegundaHUB.val("");
    _grupo.EnviarTransportadorRotaTercaHUB.val("");
    _grupo.EnviarTransportadorRotaQuartaHUB.val("");
    _grupo.EnviarTransportadorRotaQuintaHUB.val("");
    _grupo.EnviarTransportadorRotaSextaHUB.val("");
    _grupo.EnviarTransportadorRotaSabadoHUB.val("");
    _grupo.EnviarTransportadorRotaDomingoHUB.val("");

    recarregarGridGrupoTransportadoresHUBOfertas();
}

function preencherGrupoTransportadoresHUBOfertas(grupos, dadosConfiguracaoRotaFrete) {
    _grupo.ListaGrupo.val(grupos);
    _grupo.LiberarSpotAbertoAposTempoLimiteGrupos.val(dadosConfiguracaoRotaFrete.LiberarSpotAbertoAposTempoLimiteGrupos)
    _grupo.HoraEnvioOfertaHUBOfertas.val(dadosConfiguracaoRotaFrete.HoraEnvioOfertaHUBOfertas)
    _grupo.DiasAntecedenciaHUBOfertas.val(dadosConfiguracaoRotaFrete.DiasAntecedenciaHUBOfertas)
    _grupo.TipoOferta.val(dadosConfiguracaoRotaFrete.TipoOferta)
    _grupo.EnviarTransportadorRotaSegundaHUB.val(dadosConfiguracaoRotaFrete.EnviarTransportadorRotaSegundaHUB)
    _grupo.EnviarTransportadorRotaTercaHUB.val(dadosConfiguracaoRotaFrete.EnviarTransportadorRotaTercaHUB)
    _grupo.EnviarTransportadorRotaQuartaHUB.val(dadosConfiguracaoRotaFrete.EnviarTransportadorRotaQuartaHUB)
    _grupo.EnviarTransportadorRotaQuintaHUB.val(dadosConfiguracaoRotaFrete.EnviarTransportadorRotaQuintaHUB)
    _grupo.EnviarTransportadorRotaSextaHUB.val(dadosConfiguracaoRotaFrete.EnviarTransportadorRotaSextaHUB)
    _grupo.EnviarTransportadorRotaSabadoHUB.val(dadosConfiguracaoRotaFrete.EnviarTransportadorRotaSabadoHUB)
    _grupo.EnviarTransportadorRotaDomingoHUB.val(dadosConfiguracaoRotaFrete.EnviarTransportadorRotaDomingoHUB)
    _grupo.TipoOferta.val(dadosConfiguracaoRotaFrete.TipoOferta)

    recarregarGridGrupoTransportadoresHUBOfertas();
}

function preencherGrupoTransportadoresHUBOfertasSalvar(configuracaoRotaFrete) {
    configuracaoRotaFrete["GrupoTransportadoresHUBOfertas"] = obterListaGrupoTransportadoresHUBOfertasSalvar();
    configuracaoRotaFrete["TipoOferta"] = _grupo.TipoOferta.val();
    configuracaoRotaFrete["LiberarSpotAbertoAposTempoLimiteGrupos"] = _grupo.LiberarSpotAbertoAposTempoLimiteGrupos.val();
    configuracaoRotaFrete["EnviarTransportadorRotaSegundaHUB"] = _grupo.EnviarTransportadorRotaSegundaHUB.val();
    configuracaoRotaFrete["EnviarTransportadorRotaTercaHUB"] = _grupo.EnviarTransportadorRotaTercaHUB.val();
    configuracaoRotaFrete["EnviarTransportadorRotaQuartaHUB"] = _grupo.EnviarTransportadorRotaQuartaHUB.val();
    configuracaoRotaFrete["EnviarTransportadorRotaQuintaHUB"] = _grupo.EnviarTransportadorRotaQuintaHUB.val();
    configuracaoRotaFrete["EnviarTransportadorRotaSextaHUB"] = _grupo.EnviarTransportadorRotaSextaHUB.val();
    configuracaoRotaFrete["EnviarTransportadorRotaSabadoHUB"] = _grupo.EnviarTransportadorRotaSabadoHUB.val();
    configuracaoRotaFrete["EnviarTransportadorRotaDomingoHUB"] = _grupo.EnviarTransportadorRotaDomingoHUB.val();
    configuracaoRotaFrete["HoraEnvioOfertaHUBOfertas"] = _grupo.HoraEnvioOfertaHUBOfertas.val();
    configuracaoRotaFrete["DiasAntecedenciaHUBOfertas"] = _grupo.DiasAntecedenciaHUBOfertas.val();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesGrupoTransportadoresHUBOfertasHabilitados(isEdicao) {
    _cadastroGrupo.Adicionar.visible(!isEdicao);
    _cadastroGrupo.Atualizar.visible(isEdicao);
    _cadastroGrupo.Cancelar.visible(isEdicao);
    _cadastroGrupo.Excluir.visible(isEdicao);
}

function exibirModalCadastroGrupoTransportadoresHUBOfertas() {
    Global.abrirModal('divModalCadastroGrupoTransportadoresHUBOfertas');
    $("#divModalCadastroGrupoTransportadoresHUBOfertas").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroGrupo);
    });
}

function fecharModalCadastroGrupoTransportadoresHUBOfertas() {
    Global.fecharModal('divModalCadastroGrupoTransportadoresHUBOfertas');
}

function isCadastroGrupoDuplicado() {
    var listaGrupo = obterListaGrupoTransportadoresHUBOfertas();

    for (var i = 0; i < listaGrupo.length; i++) {
        var grupo = listaGrupo[i];

        if ((grupo.Codigo != _cadastroGrupo.Codigo.val()) &&
            (grupo.CodigoEmpresa == _cadastroGrupo.Empresa.codEntity()) &&
            (grupo.Descricao == _cadastroGrupo.Descricao.val()))
            return true;
    }

    return false;
}

function isSequenciaOfertaDuplicada() {
    var listaGrupo = obterListaGrupoTransportadoresHUBOfertas();
    var sequencia = _cadastroGrupo.SequenciaOferta.val();

    if (!sequencia) return false;

    for (var i = 0; i < listaGrupo.length; i++) {
        var grupo = listaGrupo[i];

        if ((grupo.Codigo != _cadastroGrupo.Codigo.val()) &&
            (grupo.SequenciaOferta == sequencia))
            return true;
    }

    return false;
}

function obterTransportadoresConsolidados() {
    var transportadores = _gridTransportadoresGrupo.BuscarRegistros();
    if (!transportadores || transportadores.length === 0)
        return "";

    return transportadores.map(t => t.DescricaoEmpresa).join(", ");
}


function obterCadastroGrupoSalvar() {
    var tipoOfertaDescricao = "";
    var descricaoTransportadores = obterTransportadoresConsolidados();

    return {
        Codigo: _cadastroGrupo.Codigo.val(),
        Descricao: _cadastroGrupo.Descricao.val(),
        Transportadores: null,
        CodigoEmpresa: _cadastroGrupo.Empresa.codEntity(),
        DescricaoEmpresa: descricaoTransportadores,
        SequenciaOferta: _cadastroGrupo.SequenciaOferta.val(),
        TempoOfertarExclusivamenteParaGrupo: _cadastroGrupo.TempoOfertarExclusivamenteParaGrupo.val(),
        TempoDeixarDeOfertarAntesDoCarregamento: _cadastroGrupo.TempoDeixarDeOfertarAntesDoCarregamento.val(),
    };
}

function obterListaGrupoTransportadoresHUBOfertas() {
    var listaGrupo = _grupo.ListaGrupo.val();
    return listaGrupo ? listaGrupo.slice() : [];
}

function obterListaGrupoTransportadoresHUBOfertasSalvar() {
    var listaGrupo = obterListaGrupoTransportadoresHUBOfertas();
    return JSON.stringify(listaGrupo);
}

function preencherDadosGrupoTransportadoresHUBOfertas(registroSelecionado) {
    _cadastroGrupo.Codigo.val(registroSelecionado.Codigo);
    _cadastroGrupo.Descricao.val(registroSelecionado.Descricao);
    _cadastroGrupo.SequenciaOferta.val(registroSelecionado.SequenciaOferta);
    _cadastroGrupo.Transportadores.val(registroSelecionado.Transportadores);
    _cadastroGrupo.TempoOfertarExclusivamenteParaGrupo.val(registroSelecionado.TempoOfertarExclusivamenteParaGrupo);
    _cadastroGrupo.Empresa.codEntity(0);
    _cadastroGrupo.Empresa.val("");
}

function recarregarGridGrupoTransportadoresHUBOfertas() {
    var listaGrupo = obterListaGrupoTransportadoresHUBOfertas();
    _gridGrupoTransportadores.CarregarGrid(listaGrupo);
}

function validarCadastroGrupoTransportadoresHUBOfertas() {
    if (!ValidarCamposObrigatorios(_cadastroGrupo)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, informe os campos obrigatórios");
        return false;
    }

    if (isCadastroGrupoDuplicado()) {
        exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "Já existe um grupo com esta descrição");
        return false;
    }

    if (isSequenciaOfertaDuplicada()) {
        exibirMensagem(tipoMensagem.atencao, "Sequência Duplicada", "A sequência de oferta informada já está sendo utilizada por outro grupo");
        return false;
    }

    var transportadores = _gridTransportadoresGrupo.BuscarRegistros();
    var codigosTransportadores = {};
    for (var i = 0; i < transportadores.length; i++) {
        var codigoTransportador = transportadores[i].Codigo;
        if (codigosTransportadores[codigoTransportador]) {
            exibirMensagem(tipoMensagem.atencao, "Transportador duplicado", "Existem transportadores duplicados na lista. Verifique e remova as duplicatas.");
            return false;
        }
        codigosTransportadores[codigoTransportador] = true;
    }

    return true;
}

// #endregion Funções Privadas