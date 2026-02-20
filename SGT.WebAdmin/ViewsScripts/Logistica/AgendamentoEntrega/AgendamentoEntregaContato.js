/// <reference path="../../Enumeradores/EnumTipoContatoAgendamentoEntregaPedido.js" />

var _agendamentoEntregaContato;
var _agendamentoEntregaContatoAdicionar;
var _gridAgendamentoEntregaAnexos;

var _agendamentoEntregaContatoTransportador;
var _gridAgendamentoEntregaAnexosTransportador;

var AgendamentoEntregaContato = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.TipoContato = PropertyEntity({ getType: typesKnockout.select, def: "", options: EnumTipoContatoAgendamentoEntregaPedido.obterOpcoes(), val: ko.observable("") });
    this.CodigoTransportador = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.CodigoCargaEntrega = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });

    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.AnexosTransportador = PropertyEntity({ text: "Anexos Transportador", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.NomeTransportador = PropertyEntity({ text: "Nome:", val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.TelefoneTransportador = PropertyEntity({ text: "Telefone:", val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.EmailTransportador = PropertyEntity({ text: "E-mail:", val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.LocalidadeTransportador = PropertyEntity({ text: "Localidade:", val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Telefone2Transportador = PropertyEntity({ text: "Telefone 2:", val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ObservacaoTransportador = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.DataAgendamento = PropertyEntity({ text: "Data de Agendamento:", val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoAgendamentoEntregaContato();
    });

    this.AnexosTransportador.val.subscribe(function () {
        recarregarGridAnexoTransportadorAgendamentoEntregaContato();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoAgendamentoEntregaModalClick, type: types.event, text: "Adicionar Contato", visible: ko.observable(visibilidadeBotaoAdicionarAnexoAgendamentoEntrega) });
    this.NotificarTransportadorPorEmail = PropertyEntity({ eventClick: enviarEmailNotificacaoTransportadorClick, type: types.event, text: "Notificar por email", visible: ko.observable(false) });
}

var AgendamentoEntregaAnexoAdicionar = function () {
    this.Descricao = PropertyEntity({ text: "*Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 400, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _agendamentoEntregaContatoAdicionar.NomeArquivo.val(nomeArquivo);
    });

    this.Adicionar = PropertyEntity({ eventClick: salvarAgendamentoEntregaContatoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

function loadAgendamentoEntregaContato() {
    _agendamentoEntregaContato = new AgendamentoEntregaContato();
    _agendamentoEntregaContatoAdicionar = new AgendamentoEntregaAnexoAdicionar();

    KoBindings(_agendamentoEntregaContato, "knockoutAgendamentoEntregaContato");
    KoBindings(_agendamentoEntregaContatoAdicionar, "knockoutAnexoAgendamentoEntregaAdicionar");

    loadGridAgendamentoEntregaAnexo();
    loadGridAgendamentoEntregaAnexoTransportador();
}

function loadGridAgendamentoEntregaAnexo() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoAgendamentoEntregaClick, icone: "", visibilidade: visibilidadeDownloadAnexoAgendamentoEntrega };
    var opcaoRemover = {
        descricao: "Remover", id: guid(), metodo: removerAnexoAgendamentoEntregaClick, icone: "", visibilidade: function () { return !isPesquisarSomenteCargasFinalizadas(); }
    };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "25%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" },
        { data: "DataCadastro", title: "Data", width: "25%", className: "text-align-left" },
        { data: "UsuarioCadastro", title: "Usuário", width: "25%", className: "text-align-left" }
    ];

    _gridAgendamentoEntregaAnexos = new BasicDataTable(_agendamentoEntregaContato.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAgendamentoEntregaAnexos.CarregarGrid([]);
}

function loadGridAgendamentoEntregaAnexoTransportador() {
    var linhasPorPaginas = 2;
    var opcaoDownload = {
        descricao: "Download", id: guid(), metodo: downloadAnexoTransportadorEntregaClick, icone: "", visibilidade: true
    };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoDownload] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "50%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "50%", className: "text-align-left" },
    ];

    _gridAgendamentoEntregaAnexosTransportador = new BasicDataTable(_agendamentoEntregaContato.AnexosTransportador.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAgendamentoEntregaAnexosTransportador.CarregarGrid([]);
}

function downloadAnexoAgendamentoEntregaClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    var url = _agendamentoEntregaContato.TipoContato.val() == EnumTipoContatoAgendamentoEntregaPedido.Cliente ? "AgendamentoEntregaPedidoContatoCliente/DownloadAnexo" : "AgendamentoEntregaPedidoContatoTransportador/DownloadAnexo";

    executarDownload(url, dados);
}

function downloadAnexoTransportadorEntregaClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    var url = _agendamentoEntregaContato.TipoContato.val() == EnumTipoContatoAgendamentoEntregaPedido.Cliente ? "PessoaAnexo/DownloadAnexo" : "TransportadorAnexo/DownloadAnexo";

    executarDownload(url, dados);
}

function removerAnexoAgendamentoEntregaClick(registroSelecionado) {
    var url = _agendamentoEntregaContato.TipoContato.val() == EnumTipoContatoAgendamentoEntregaPedido.Cliente ? "AgendamentoEntregaPedidoContatoCliente/ExcluirAnexo" : "AgendamentoEntregaPedidoContatoTransportador/ExcluirAnexo";

    if (isNaN(registroSelecionado.Codigo))
        removerAnexoAgendamentoEntregaLocal(registroSelecionado);
    else {
        executarReST(url, { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerAnexoAgendamentoEntregaLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function removerAnexoAgendamentoEntregaLocal(registroSelecionado) {
    var listaAnexos = obterAnexosAgendamentoEntrega();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _agendamentoEntregaContato.Anexos.val(listaAnexos);
}

function salvarAgendamentoEntregaContatoClick() {
    var arquivo = document.getElementById(_agendamentoEntregaContatoAdicionar.Arquivo.id);

    /*if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }*/

    if (!ValidarCamposObrigatorios(_agendamentoEntregaContatoAdicionar)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _agendamentoEntregaContatoAdicionar.Descricao.val(),
        NomeArquivo: _agendamentoEntregaContatoAdicionar.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var listaAnexos = obterAnexosAgendamentoEntrega();

    listaAnexos.push(anexo);

    enviarAnexos(_agendamentoEntregaContato.Codigo.val(), [anexo]);

    _agendamentoEntregaContatoAdicionar.Arquivo.val("");

    Global.fecharModal("divModalAdicionarAnexoAgendamentoEntrega");
}

function enviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexosAgendamentoEntrega(anexos);

    var url = _agendamentoEntregaContato.TipoContato.val() == EnumTipoContatoAgendamentoEntregaPedido.Cliente ? "AgendamentoEntregaPedidoContatoCliente/AnexarArquivos" : "AgendamentoEntregaPedidoContatoTransportador/AnexarArquivos";

    if (formData) {
        enviarArquivo(url, { Codigo: codigo, CodigoCargaEntrega: _agendamentoEntregaContato.CodigoCargaEntrega.val() }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    carregarContatosAgendamentoEntrega();
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterFormDataAnexosAgendamentoEntrega(anexos) {
    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function exibirModalAgendamentoEntregaContato(codigo, tipoContato, codigoTransportador, dataAgendamento, codigoCargaEntrega) {
    _agendamentoEntregaContato.Codigo.val(codigo);
    _agendamentoEntregaContato.TipoContato.val(tipoContato);
    _agendamentoEntregaContato.CodigoTransportador.val(codigoTransportador);
    _agendamentoEntregaContato.DataAgendamento.val(dataAgendamento);
    _agendamentoEntregaContato.CodigoCargaEntrega.val(codigoCargaEntrega);
    _agendamentoEntregaContato.Adicionar.visible(_pesquisaAgendamentoEntregaPedido.StatusCarga.val() !== 1);

    carregarContatosAgendamentoEntrega();

    $("#divModalAgendamentoEntregaContato")
        .modal("show").on("hidden.bs.modal", function () {
            LimparCampos(_agendamentoEntregaContato);
        });
}

function carregarContatosAgendamentoEntrega() {
    var url = _agendamentoEntregaContato.TipoContato.val() == EnumTipoContatoAgendamentoEntregaPedido.Cliente ? "AgendamentoEntregaPedidoContatoCliente/ObterAnexo" : "AgendamentoEntregaPedidoContatoTransportador/ObterAnexo";

    executarReST(url, { Codigo: _agendamentoEntregaContato.Codigo.val(), CodigoCargaEntrega: _agendamentoEntregaContato.CodigoCargaEntrega.val(), CodigoTransportador: _agendamentoEntregaContato.CodigoTransportador.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _agendamentoEntregaContato.Anexos.val(retorno.Data.Anexos);

                _agendamentoEntregaContato.AnexosTransportador.val(retorno.Data.AnexosTransportador);
                _agendamentoEntregaContato.NomeTransportador.val(retorno.Data.NomeTransportador);
                _agendamentoEntregaContato.TelefoneTransportador.val(retorno.Data.TelefoneTransportador);
                _agendamentoEntregaContato.EmailTransportador.val(retorno.Data.EmailTransportador);
                _agendamentoEntregaContato.LocalidadeTransportador.val(retorno.Data.LocalidadeTransportador);
                _agendamentoEntregaContato.Telefone2Transportador.val(retorno.Data.Telefone2Transportador);
                _agendamentoEntregaContato.ObservacaoTransportador.val(retorno.Data.ObservacaoTransportador);
                _agendamentoEntregaContato.NotificarTransportadorPorEmail.visible(_agendamentoEntregaContato.TipoContato.val() == EnumTipoContatoAgendamentoEntregaPedido.Transportador && _agendamentoEntregaContato.EmailTransportador.val() != '' && _agendamentoEntregaContato.DataAgendamento.val() != '');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function visibilidadeBotaoAdicionarAnexoAgendamentoEntrega() {
    return true;
}

function visibilidadeDownloadAnexoAgendamentoEntrega(registroSelecionado) {
    return !string.IsNullOrWhiteSpace(registroSelecionado.NomeArquivo);
}

function enviarEmailNotificacaoTransportadorClick() {
    _envioEmailAgendamento.ContatoTransportador.val(true);
    _envioEmailAgendamento.EnviarEmailParaTodosOsPedidosDoMesmoDestinoECarga.visible(false);
    _envioEmailAgendamento.TituloModal.val("Envio de Email de Notificação ao Transportador");
    abrirModalEnvioEmailAgendamentoNotificacao();
}

function adicionarAnexoAgendamentoEntregaModalClick() {
    Global.abrirModal('divModalAdicionarAnexoAgendamentoEntrega');
    $("#divModalAdicionarAnexoAgendamentoEntrega").one("hidden.bs.modal", function () {
        LimparCampos(_agendamentoEntregaContatoAdicionar);
    });
}

function recarregarGridAnexoAgendamentoEntregaContato() {
    var anexos = obterAnexosAgendamentoEntrega();

    _gridAgendamentoEntregaAnexos.CarregarGrid(anexos);
}

function obterAnexosAgendamentoEntrega() {
    return _agendamentoEntregaContato.Anexos.val().slice();
}

function recarregarGridAnexoTransportadorAgendamentoEntregaContato() {
    var anexosTransportador = obterAnexosTransportadorAgendamentoEntrega();

    _gridAgendamentoEntregaAnexosTransportador.CarregarGrid(anexosTransportador);
}

function obterAnexosTransportadorAgendamentoEntrega() {
    return _agendamentoEntregaContato.AnexosTransportador.val().slice();
}
