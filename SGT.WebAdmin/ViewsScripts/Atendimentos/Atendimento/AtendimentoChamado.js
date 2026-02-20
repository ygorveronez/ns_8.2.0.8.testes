/// <reference path="../../Consultas/TipoAtendimento.js" />
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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Sistema.js" />
/// <reference path="../../Consultas/Modulo.js" />
/// <reference path="../../Consultas/Tela.js" />
/// <reference path="Atendimento.js" />
/// <reference path="AtendimentoEtapa.js" />
/// <reference path="../../Enumeradores/EnumStatusAtendimentoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _atendimentoChamado;
var _gridAnexos;

var _prioridadeChamado = [
    { text: "Baixa", value: EnumPrioridadeAtendimento.Baixa },
    { text: "Normal", value: EnumPrioridadeAtendimento.Normal },
    { text: "Alta", value: EnumPrioridadeAtendimento.Alta }
];

var _statusAtendimentoChamado = [
    { text: "Aberto", value: EnumStatusAtendimentoChamado.Aberto },
    { text: "Cancelado", value: EnumStatusAtendimentoChamado.Cancelado },
    { text: "Finalizado", value: EnumStatusAtendimentoChamado.Finalizado },
    { text: "Parcial", value: EnumStatusAtendimentoChamado.Parcial },
    { text: "Pendente de ret. Cliente", value: EnumStatusAtendimentoChamado.PendenteRetornoCliente },
    { text: "Pendente de ret. Suporte", value: EnumStatusAtendimentoChamado.PendenteRetornoSuporte },
    { text: "Em Treinamento", value: EnumStatusAtendimentoChamado.EmTreinamento },
    { text: "Em Atendimento", value: EnumStatusAtendimentoChamado.EmAtendimento }
];

var AnexoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.map, val: ko.observable("") });
}

var RespostaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Resposta = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoFuncionario = PropertyEntity({ type: types.map, val: ko.observable(""), getType: typesKnockout.int });
    this.Funcionario = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DataHora = PropertyEntity({ type: types.map, val: ko.observable(""), getType: typesKnockout.dateTime });
}

var AtendimentoChamado = function () {
    this.CodigoTarefa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Tela = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tela:", idBtnSearch: guid(), visible: ko.observable(true), enable: false });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Módulo:", idBtnSearch: guid(), visible: ko.observable(true), enable: false });
    this.Sistema = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Sistema:", idBtnSearch: guid(), visible: ko.observable(true), required: true, enable: false });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Solicitante:", idBtnSearch: guid(), visible: ko.observable(true), enable: false });

    this.TipoAtendimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo do Atendimento:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.StatusChamado = PropertyEntity({ val: ko.observable(EnumStatusAtendimentoChamado.Aberto), options: _statusAtendimentoChamado, def: EnumStatusAtendimentoChamado.Aberto, text: "*Status do Chamado: ", visible: ko.observable(true) });
    this.PrioridadeChamado = PropertyEntity({ val: ko.observable(EnumPrioridadeAtendimento.Normal), options: _prioridadeChamado, def: EnumPrioridadeAtendimento.Normal, text: "*Prioridade do Chamado: ", visible: ko.observable(true) });

    this.TituloChamado = PropertyEntity({ text: "*Título/Descrição do Chamado: ", maxlength: 100, enable: false, visible: ko.observable(true) });
    this.MotivoChamado = PropertyEntity({ text: "*Motivo \ Problema Relatado pelo Cliente: ", required: true, maxlength: 5000, enable: false, visible: ko.observable(true) });
    this.SolucaoChamado = PropertyEntity({ text: "*Justificativa \ Solução Aplicada \ Observação do Suporte: ", maxlength: 5000, required: true, visible: ko.observable(true) });

    this.DescricaoAnexo = PropertyEntity({ text: "*Descrição Anexo:", getType: typesKnockout.string, maxlength: 500, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), required: ko.observable(true), visible: ko.observable(false) });
    this.SalvarAnexo = PropertyEntity({ eventClick: SalvarAnexoClick, type: types.event, text: "Salvar Anexo", visible: ko.observable(false), enable: ko.observable(true) });
    this.AnexosAtendimento = PropertyEntity({ type: types.local, id: guid() });

    this.Resposta = PropertyEntity({ text: "*Resposta: ", required: ko.observable(false), maxlength: 5000,visible: ko.observable(true) });
    this.FuncionarioResposta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.SalvarResposta = PropertyEntity({ eventClick: SalvarRespostaClick, type: types.event, text: "Salvar Resposta", visible: ko.observable(true), enable: ko.observable(true) });
    this.RespostasChamado = PropertyEntity({ type: types.local, id: guid() });
}

//*******EVENTOS*******

function loadAtendimentoChamado() {
    _atendimentoChamado = new AtendimentoChamado();
    KoBindings(_atendimentoChamado, "knockoutChamadosAtendimento");

    new BuscarTelas(_atendimentoChamado.Tela);
    new BuscarModulos(_atendimentoChamado.Modulo);
    new BuscarSistemas(_atendimentoChamado.Sistema);
    new BuscarTipoAtendimento(_atendimentoChamado.TipoAtendimento);

    var baixarAnexo = { descricao: "Baixar", id: guid(), metodo: BaixarAnexoClick, icone: "" };
    var excluirAnexo = { descricao: "Excluir", id: guid(), metodo: ExcluirAnexoClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [baixarAnexo, excluirAnexo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoAtendimento", visible: false },
        { data: "DescricaoAnexo", title: "Descrição", width: "40%" },
        { data: "Arquivo", title: "Caminho / Nome Arquivo", width: "50%" }
    ];

    _gridAnexos = new BasicDataTable(_atendimentoChamado.AnexosAtendimento.id, header, menuOpcoes);

    recarregarGridListaAnexosChamado();

    var menuOpcoesChamado = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirRespostaClick, tamanho: 10 }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoChamado", visible: false },
        { data: "Resposta", title: "Resposta", width: "50%", },
        { data: "CodigoFuncionario", visible: false },
        { data: "Funcionario", title: "Funcionário", width: "25%" },
        { data: "DataHora", title: "Data e Hora", width: "15%" }
    ];

    _gridRespostas = new BasicDataTable(_atendimentoChamado.RespostasChamado.id, header, menuOpcoesChamado, { column: 5, dir: orderDir.desc });

    recarregarGridListaRespostasChamado();
}

function BaixarAnexoClick(data) {
    if (VerificaNovosAnexosLancados(data.Codigo))
        executarDownload("Atendimento/DownloadAnexo", { CodigoAnexo: data.Codigo });
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível baixar os Anexos após gravar o Atendimento");
}

function ExcluirAnexoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o anexo " + data.DescricaoAnexo + "?", function () {
        $.each(_atendimento.ListaAnexos.list, function (i, listaAnexos) {
            if (data.Codigo == listaAnexos.Codigo.val) {
                _atendimento.ListaAnexos.list.splice(i, 1);

                if (VerificaNovosAnexosLancados(data.Codigo)) {
                    var listaAnexosExcluidos = new AnexoMap();
                    listaAnexosExcluidos.Codigo.val = data.Codigo;
                    _atendimento.ListaAnexosExcluidos.list.push(listaAnexosExcluidos);
                }

                return false;
            }
        });

        $.each(_atendimento.ListaAnexosNovos.list, function (i, listaAnexosNovos) {
            if (data.Codigo == listaAnexosNovos.Codigo.val) {
                _atendimento.ListaAnexosNovos.list.splice(i, 1);

                return false;
            }
        });

        recarregarGridListaAnexosChamado();
    });
}

function ExcluirRespostaClick(data) {
    if (data.CodigoFuncionario != _atendimento.Funcionario.codEntity()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível excluir respostas de outro funcionário!");
    } else {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir a resposta: " + data.Resposta + "?", function () {
            $.each(_atendimento.ListaRespostas.list, function (i, listaRespostas) {
                if (data.Codigo == listaRespostas.Codigo.val) {
                    _atendimento.ListaRespostas.list.splice(i, 1);

                    if (VerificaNovasRespostasLancados(data.Codigo)) {
                        var listaRespostasExcluidas = new RespostaMap();
                        listaRespostasExcluidas.Codigo.val = data.Codigo;
                        _atendimento.ListaRespostasExcluidas.list.push(listaRespostasExcluidas);
                    }

                    return false;
                }
            });

            $.each(_atendimento.ListaRespostasNovas.list, function (i, listaRespostasNovas) {
                if (data.Codigo == listaRespostasNovas.Codigo.val) {
                    _atendimento.ListaRespostasNovas.list.splice(i, 1);

                    return false;
                }
            });

            recarregarGridListaRespostasChamado();
        });
    }
}

function SalvarAnexoClick(e, sender) {
    var valido = true;
    _atendimentoChamado.Arquivo.requiredClass("form-control");

    if (_atendimentoChamado.Arquivo.val() == "") {
        valido = false;
        _atendimentoChamado.Arquivo.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var codigo = guid();
        var listaAnexosGrid = new AnexoMap();

        listaAnexosGrid.Codigo.val = codigo;
        listaAnexosGrid.DescricaoAnexo.val = _atendimentoChamado.DescricaoAnexo.val();
        listaAnexosGrid.Arquivo.val = _atendimentoChamado.Arquivo.val();
        _atendimento.ListaAnexos.list.push(listaAnexosGrid);

        var listaAnexos = new AnexoMap();

        listaAnexos.Codigo.val = codigo;
        listaAnexos.DescricaoAnexo.val = _atendimentoChamado.DescricaoAnexo.val();
        var file = document.getElementById(_atendimentoChamado.Arquivo.id);
        listaAnexos.Arquivo = file.files[0];
        _atendimento.ListaAnexosNovos.list.push(listaAnexos);

        recarregarGridListaAnexosChamado();
        _atendimentoChamado.DescricaoAnexo.val("");
        _atendimentoChamado.Arquivo.val("");
        $("#" + _atendimentoChamado.DescricaoAnexo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function SalvarRespostaClick(e, sender) {
    var valido = true;
    var nomeFuncionario, codigoFuncionario;

    _atendimentoChamado.Resposta.requiredClass("form-control ");

    if (_atendimentoChamado.Resposta.val() == "") {
        valido = false;
        _atendimentoChamado.Resposta.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var codigo = guid();
        var listaRespostasGrid = new RespostaMap();

        listaRespostasGrid.Codigo.val = codigo;
        listaRespostasGrid.Resposta.val = _atendimentoChamado.Resposta.val();
        listaRespostasGrid.CodigoFuncionario.val = _atendimento.Funcionario.codEntity();
        listaRespostasGrid.Funcionario.val = _atendimento.Funcionario.val();
        listaRespostasGrid.DataHora.val = moment().format("DD/MM/YYYY HH:mm:ss");
        _atendimento.ListaRespostas.list.push(listaRespostasGrid);

        var listaRespostas = new RespostaMap();

        listaRespostas.Codigo.val = codigo;
        listaRespostas.Resposta.val = _atendimentoChamado.Resposta.val();
        listaRespostas.CodigoFuncionario.val = _atendimento.Funcionario.codEntity();
        listaRespostas.DataHora.val = moment().format("DD/MM/YYYY HH:mm:ss");

        _atendimento.ListaRespostasNovas.list.push(listaRespostas);

        recarregarGridListaRespostasChamado();
        limparCamposChamadoRespostasChamado();
        $("#" + _atendimentoChamado.Resposta.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

//*******MÉTODOS*******

function VerificaNovosAnexosLancados(codigoAnexo) {
    var valido = true;
    $.each(_atendimento.ListaAnexosNovos.list, function (i, listaAnexos) {
        if (codigoAnexo == listaAnexos.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function VerificaNovasRespostasLancados(codigoResposta) {
    var valido = true;
    $.each(_atendimento.ListaRespostasNovas.list, function (i, listaRespostas) {
        if (codigoResposta == listaRespostas.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function recarregarGridListaAnexosChamado() {

    var data = new Array();

    $.each(_atendimento.ListaAnexos.list, function (i, listaAnexos) {
        var listaAnexosGrid = new Object();

        listaAnexosGrid.Codigo = listaAnexos.Codigo.val;
        listaAnexosGrid.CodigoAtendimento = _atendimento.Codigo.val();
        listaAnexosGrid.DescricaoAnexo = listaAnexos.DescricaoAnexo.val;
        listaAnexosGrid.Arquivo = listaAnexos.Arquivo.val;

        data.push(listaAnexosGrid);
    });

    _gridAnexos.CarregarGrid(data);
}

function recarregarGridListaRespostasChamado() {

    var data = new Array();

    $.each(_atendimento.ListaRespostas.list, function (i, listaRespostas) {
        var listaRespostasGrid = new Object();

        listaRespostasGrid.Codigo = listaRespostas.Codigo.val;
        listaRespostasGrid.CodigoChamado = _atendimento.Codigo.val();
        listaRespostasGrid.Resposta = listaRespostas.Resposta.val;
        listaRespostasGrid.CodigoFuncionario = listaRespostas.CodigoFuncionario.val;
        listaRespostasGrid.Funcionario = listaRespostas.Funcionario.val;
        listaRespostasGrid.DataHora = listaRespostas.DataHora.val;

        data.push(listaRespostasGrid);
    });

    _gridRespostas.CarregarGrid(data);
}

function limparCamposAtendimentoChamado() {
    _atendimentoChamado.DescricaoAnexo.val("");
    _atendimentoChamado.Arquivo.val("");
}

function limparCamposChamadoRespostasChamado() {
    _atendimentoChamado.Resposta.val("");
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}