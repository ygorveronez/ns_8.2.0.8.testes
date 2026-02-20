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
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/Container.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Navio.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumGeradoPendente.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _conferencia;
var _informacaoManual;
var _replicarNumero;
var _arquivoMercante;
var _gridConferencia;
var _gridArquivos;
var _PermissoesPersonalizadas = null;
var _importacaoArquivoMercante;

var _situacaoCTeAlteracaoArquivoMercante = [
    { text: "Autorizado", value: "A" },
    { text: "Pendente", value: "P" },
    { text: "Enviado", value: "E" },
    { text: "Rejeitado", value: "R" },
    { text: "Cancelado", value: "C" },
    { text: "Anulado", value: "Z" },
    { text: "Inutilizado", value: "I" },
    { text: "Denegado", value: "D" },
    { text: "Em Digitação", value: "S" },
    { text: "Em Cancelamento", value: "K" },
    { text: "Em Inutilização", value: "L" }
];

var _tipoCargaPerigosa = [
    { text: "Todas", value: 0 },
    { text: "Somente Carga Perigosa", value: 1 },
    { text: "Sem Carga Perigosa", value: 2 }
];

var _tipoTransbordo = [
    { text: "Gerar Manifesto M3 (Todos)", value: 0 },
    { text: "Gerar Manifesto M3 (Sem número do manifesto absorvido)", value: 2 },
    { text: "Gerar Baldeação M4", value: 1 }
];

var _tipoArquivo = [
    { text: "Todos", value: "" },
    { text: "Arquivo M3", value: "M3" },
    { text: "Arquivo M4", value: "M4" }
];

var _statusArquivo = [
    { text: "Sucesso", value: 1 },
    { text: "Falha", value: 2 },
    { text: "Todos", value: 0 }
];

var Conferencia = function () {
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Origem:", idBtnSearch: guid() });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Container:", idBtnSearch: guid() });
    this.NavioTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Navio Transbordo:", idBtnSearch: guid() });
    this.PortoTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Transbordo:", idBtnSearch: guid() });

    this.NumeroControle = PropertyEntity({ text: "Número Controle: " });
    this.NumeroBooking = PropertyEntity({ text: "Número Booking: " });
    this.NumeroCE = PropertyEntity({ text: "Número CE: " });
    this.NumeroManifesto = PropertyEntity({ text: "Número Manifesto: " });
    this.NumeroManifestoTransbordo = PropertyEntity({ text: "Nº Manifesto Transbordo: " });

    this.PossuiTransbordo = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Possui Transbordo: " });
    this.Manifesto = PropertyEntity({ val: ko.observable(EnumGeradoPendente.Todos), options: EnumGeradoPendente.obterOpcoesPesquisa(), def: EnumGeradoPendente.Todos, text: "Manifesto: " });
    this.ManifestoTransbordo = PropertyEntity({ val: ko.observable(EnumGeradoPendente.Todos), options: EnumGeradoPendente.obterOpcoesPesquisa(), def: EnumGeradoPendente.Todos, text: "Man. Transbordo: " });    
    this.CE = PropertyEntity({ val: ko.observable(EnumGeradoPendente.Todos), options: EnumGeradoPendente.obterOpcoesPesquisa(), def: EnumGeradoPendente.Todos, text: "CE: " });
    this.Status = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _situacaoCTeAlteracaoArquivoMercante, text: "Status:" });
    this.TipoCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), text: "Tipo CTe:" });
    this.Balsa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Balsa:"), idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.Pesquisa = PropertyEntity({ eventClick: pesquisarConferenciaClick, type: types.event, text: "Pesquisar" });
    this.Grid = PropertyEntity({ idGrid: guid() });
};

var InformacaoManual = function () {
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Terminal Origem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Viagem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.NumeroControle = PropertyEntity({ text: "*Número Controle: ", required: ko.observable(true) });
    this.NumeroCE = PropertyEntity({ text: "*Número CE: ", required: ko.observable(true) });
    this.NumeroManifesto = PropertyEntity({ text: "*Número Manifesto: ", required: ko.observable(true) });
    this.NumeroManifestoTransbordo = PropertyEntity({ text: "Nº Manifesto Transbordo: ", required: ko.observable(false) });

    this.Atualizar = PropertyEntity({ eventClick: atualizarInformacaoManualClick, type: types.event, text: "Atualizar" });
    this.Cancelar = PropertyEntity({ eventClick: cancelarInformacaoManualClick, type: types.event, text: "Cancelar" });
};

var ReplicarNumero = function () {
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Terminal Origem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Viagem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.NumeroControleAntigo = PropertyEntity({ text: "*Número Controle Antigo: ", required: ko.observable(true) });
    this.NumeroControleNovo = PropertyEntity({ text: "*Número Controle Novo: ", required: ko.observable(true) });

    this.Atualizar = PropertyEntity({ eventClick: atualizarReplicarNumeroClick, type: types.event, text: "Atualizar" });
    this.Cancelar = PropertyEntity({ eventClick: cancelarReplicarNumeroClick, type: types.event, text: "Cancelar" });
};

var ArquivoMercante = function () {
    this.PedidoNavioViagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Viagem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Porto Origem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto de Atracação:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Terminal Origem:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Terminal de Atracação:"), idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.ComConhecimentosCancelados = PropertyEntity({ text: "Deseja gerar com os CT-es cancelados?", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.ManifestoPorBalsa = PropertyEntity({ text: "Gerar manifesto por balsa?", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.Balsa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Balsa:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoCargaPerigosa = PropertyEntity({ val: ko.observable(0), options: _tipoCargaPerigosa, def: 0, text: "*Tipo Carga Perigosa: " });
    this.TipoTransbordo = PropertyEntity({ val: ko.observable(0), options: _tipoTransbordo, def: 0, text: "*Tipo do Arquivo: " });

    this.GerarArquivoMercante = PropertyEntity({ eventClick: GerarArquivoMercanteClick, type: types.event, text: "Gerar Arquivo Mercante", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarArquivoMercanteClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo Mercante:", val: ko.observable(""), visible: ko.observable(true) });
    this.Enviar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });

    this.ManifestoPorBalsa.val.subscribe((valor) => {
        _arquivoMercante.Balsa.required(valor);
    });
};

var ImportacaoArquivoMercante = function () {
    this.PedidoViagemDirecao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Navio/Viagem/Direção:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Porto Origem:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable("Porto Destino:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoArquivo = PropertyEntity({ val: ko.observable(""), def: "", text: "Tipo do Arquivo:", options: _tipoArquivo });
    this.StatusArquivo = PropertyEntity({ val: ko.observable(0), options: _statusArquivo, def: 0, text: "Status do Arquivo: " });

    this.InformarEmailEnvio = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.EmailEnvio = PropertyEntity({ text: "Informar o e-mail", required: false, enable: ko.observable(false), visible: ko.observable(true), maxlength: 2000 });

    this.InformarEmailEnvio.val.subscribe(function (novoValor) {
        _importacaoArquivoMercante.EmailEnvio.enable(novoValor);
    });

    this.Arquivos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.Pesquisa = PropertyEntity({ eventClick: PesquisaImportacaoArquivoMercanteClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });
    this.ListaArquivos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.DownloadLote = PropertyEntity({ eventClick: DownloadLoteClick, type: types.event, text: ko.observable("Download em Lote"), visible: ko.observable(true) });
    this.EnviarPorEmail = PropertyEntity({ eventClick: EnviarPorEmailClick, type: types.event, text: ko.observable("Enviar por E-mail"), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAlteracaoArquivoMercante() {
    _conferencia = new Conferencia();
    KoBindings(_conferencia, "tabConferencia");

    _informacaoManual = new InformacaoManual();
    KoBindings(_informacaoManual, "tabInformacaoManual");

    _replicarNumero = new ReplicarNumero();
    KoBindings(_replicarNumero, "tabReplicarNumero");

    _arquivoMercante = new ArquivoMercante();
    KoBindings(_arquivoMercante, "tabArquivoMercante");

    _importacaoArquivoMercante = new ImportacaoArquivoMercante();
    KoBindings(_importacaoArquivoMercante, "tabImportacaoArquivoMercante");

    new BuscarPedidoViagemNavio(_importacaoArquivoMercante.PedidoViagemDirecao);
    new BuscarPorto(_importacaoArquivoMercante.PortoOrigem);
    new BuscarPorto(_importacaoArquivoMercante.PortoDestino);

    CriarGridImportacaoArquivoMercante();
    buscarImportacaoArquivoMercante();

    new BuscarPedidoViagemNavio(_conferencia.Viagem);
    new BuscarTipoTerminalImportacao(_conferencia.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_conferencia.TerminalDestino);
    new BuscarContainers(_conferencia.Container);
    new BuscarPedidoViagemNavio(_conferencia.NavioTransbordo);
    new BuscarPorto(_conferencia.PortoTransbordo);
    BuscarNavios(_conferencia.Balsa, null, null, EnumTipoEmbarcacao.Balsa);

    new BuscarPedidoViagemNavio(_informacaoManual.Viagem);
    new BuscarTipoTerminalImportacao(_informacaoManual.TerminalOrigem);

    new BuscarPedidoViagemNavio(_replicarNumero.Viagem);
    new BuscarTipoTerminalImportacao(_replicarNumero.TerminalOrigem);

    new BuscarEmpresa(_arquivoMercante.Empresa);
    new BuscarPedidoViagemNavio(_arquivoMercante.PedidoNavioViagem);
    new BuscarPorto(_arquivoMercante.PortoOrigem);
    new BuscarPorto(_arquivoMercante.PortoDestino);
    new BuscarTipoTerminalImportacao(_arquivoMercante.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_arquivoMercante.TerminalDestino);

    BuscarNavios(_arquivoMercante.Balsa, null, null, EnumTipoEmbarcacao.Balsa);

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.AlteracaoArquivoMercante_RemoverObrigatoriedadeTerminalAtracacao, _PermissoesPersonalizadas)) {
        _arquivoMercante.TerminalDestino.required(false);
        _arquivoMercante.TerminalDestino.text("Terminal de Atracação:");
    } else {
        _arquivoMercante.TerminalDestino.required(true);
        _arquivoMercante.TerminalDestino.text("*Terminal de Atracação:");
    }

    loadAlteracaoArquivoMercanteIntegracao();
    controlarAbaIntegracao();

    montarGridConferencias();
}

//ABA CONFERÊNCIA

function pesquisarConferenciaClick() {
    var valido = ValidarCamposObrigatorios(_conferencia);
    if (valido) {
        _gridConferencia.CarregarGrid();
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function callbackEditarColuna(cte, row, head, callbackTabPress) {
    var dados = {
        Codigo: cte.Codigo,
        NumeroManifesto: cte.NumeroManifesto,
        NumeroCE: cte.NumeroCE,
        NumeroManifestoTransbordo: cte.NumeroManifestoTransbordo
    };
    executarReST("AlteracaoArquivoMercante/AtualizarConhecimentosConferencia", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro alterado com sucesso.");

                CompararEAtualizarGridEditableDataRow(cte, arg.Data);
                setTimeout(function () {
                    _gridConferencia.AtualizarDataRow(row, cte);
                }, 500);
                
            } else {
                //_gridConferencia.DesfazerAlteracaoDataRow(row);
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 99999999);
            }
        } else {
            //_gridConferencia.DesfazerAlteracaoDataRow(row);
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(cte, arg.Data);
                setTimeout(function () {
                    _gridConferencia.AtualizarDataRow(row, cte);
                }, 500);
                //_gridConferencia.enable(true);
            }

            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 99999999);
        }
    });
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridConferencia.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem, 99999999);
}

function montarGridConferencias() {
    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: true };
    var configExportacao = {
        url: "AlteracaoArquivoMercante/ExportarPesquisa",
        titulo: "Conferência Arquivo Mercante"
    };

    _gridConferencia = new GridViewExportacao(_conferencia.Grid.idGrid, "AlteracaoArquivoMercante/Pesquisa", _conferencia, null, configExportacao, null, 10, null, null, editarColuna);
}

//ABA INFORMAÇÃO MANUAL

function atualizarInformacaoManualClick() {
    var valido = ValidarCamposObrigatorios(_informacaoManual);

    if (valido) {
        _informacaoManual.NumeroCE.requiredClass("form-control");
        _informacaoManual.NumeroManifesto.requiredClass("form-control");
        _informacaoManual.NumeroManifestoTransbordo.requiredClass("form-control");

        if (_informacaoManual.NumeroCE.val() != "" && _informacaoManual.NumeroCE.val().trim().length != 15) {
            valido = false;
            _informacaoManual.NumeroCE.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.atencao, "Atenção", "O número de CE deve conter 15 caracteres.", 99999999);
            return;
        }
        if (_informacaoManual.NumeroManifesto.val() != "" && _informacaoManual.NumeroManifesto.val().trim().length != 13) {
            valido = false;
            _informacaoManual.NumeroManifesto.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.atencao, "Atenção", "O número de Manifesto deve conter 13 caracteres.", 99999999);
            return;
        }
        if (_informacaoManual.NumeroManifestoTransbordo.val() != "" && _informacaoManual.NumeroManifestoTransbordo.val().trim().length != 13) {
            valido = false;
            _informacaoManual.NumeroManifestoTransbordo.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.atencao, "Atenção", "O número de Manifesto deve conter 13 caracteres.", 99999999);
            return;
        }
    }
    if (valido) {
        Salvar(_informacaoManual, "AlteracaoArquivoMercante/AtualizarConhecimentosInformacaoManual", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", arg.QuantidadeRegistros + " registro(s) atualizado(s).");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 99999999);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg, 99999999);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!", 99999999);
}

function cancelarInformacaoManualClick() {
    LimparCampos(_informacaoManual);
}

//ABA REPLICAR NÚMERO

function atualizarReplicarNumeroClick() {
    var valido = ValidarCamposObrigatorios(_replicarNumero);
    if (valido) {
        Salvar(_replicarNumero, "AlteracaoArquivoMercante/AtualizarConhecimentosReplicarNumero", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", arg.QuantidadeRegistros + " registro(s) atualizado(s).");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 99999999);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg, 99999999);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function cancelarReplicarNumeroClick() {
    LimparCampos(_replicarNumero);
}

//ABA GERAR ARQUIVO MERCANTE

function importarClick(e, sender) {
    var file = document.getElementById(_arquivoMercante.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("AlteracaoArquivoMercante/ImportarArquivoMercante?callback=?", null, formData, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg)
            LimparCampo(_arquivoMercante.Arquivo);
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 99999999);
        }
    });
}

function GerarArquivoMercanteClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_arquivoMercante);
    if (valido) {
        data = RetornarObjetoPesquisa(_arquivoMercante);
        executarDownload("AlteracaoArquivoMercante/GerarArquivoMercante", data, function (arg) {

            Salvar(_arquivoMercante, "AlteracaoArquivoMercante/ConsultarDocumentacaoMercantePendente", function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo gerado com sucesso e sem pendências.");
                    } else {
                        $("#tabArquivoMercante").before('<p class="alert alert-info no-margin"><button class="close" data-dismiss="alert">×</button><i class="fa-fw fa fa-info"></i><strong>Atenção!</strong> Retorno da geração do Arquivo Mercante:<br/>' + arg.Msg.replace(/\n/g, "<br />") + '</p>');
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 99999999);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg, 99999999);
                }
            }, sender);

        }, null);
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function cancelarArquivoMercanteClick() {
    LimparCampos(_arquivoMercante);
}

//ABA IMPORTACAO ARQUIVO MERCANTE

function CriarGridImportacaoArquivoMercante() {
    var somenteLeitura = false;

    _importacaoArquivoMercante.SelecionarTodos.visible(true);
    _importacaoArquivoMercante.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _importacaoArquivoMercante.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    var download = { descricao: "Download", id: guid(), evento: "onclick", metodo: downloadArquivo, tamanho: "5", icone: "" };
    var detalhe = { descricao: "Detalhe", id: guid(), evento: "onclick", metodo: detalheArquivo, tamanho: "5", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [download, detalhe] };

    _gridArquivos = new GridView(_importacaoArquivoMercante.Arquivos.idGrid, "AlteracaoArquivoMercante/PesquisaImportacaoArquivoMercante", _importacaoArquivoMercante, menuOpcoes, null, null, null, null, null, multiplaescolha);
}

function detalheArquivo(data) {
    executarReST("AlteracaoArquivoMercante/BuscarDetalhes", { Codigo: data.Codigo }, function (arg) {
        if (arg.Success) {            
            Global.abrirModal('divModalDetalheMensagemRetorno');
            $("#pMensagemAlerta").remove();
            $("#contentDetalheMensagemRetorno").before('<p id="pMensagemAlerta" class="alert alert-info no-margin"><button class="close" data-dismiss="alert">×</button><i class="fa-fw fa fa-info"></i><strong>Atenção!</strong> Retorno da importação:<br/>' + arg.Data.Retorno.replace(/\n/g, "<br />") + '</p>');
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 99999999);
        }
    });

}

function downloadArquivo(data) {
    executarDownload("AlteracaoArquivoMercante/DownloadArquivo", { Codigo: data.Codigo });
}

function buscarImportacaoArquivoMercante() {
    _importacaoArquivoMercante.SelecionarTodos.visible(true);
    _importacaoArquivoMercante.SelecionarTodos.val(false);

    _gridArquivos.AtualizarRegistrosSelecionados([]);
    _gridArquivos.CarregarGrid();
}

function PesquisaImportacaoArquivoMercanteClick(e, sender) {
    buscarImportacaoArquivoMercante();
}

function DownloadLoteClick(e, sender) {
    var data = null;
    var todosSelecionado = _importacaoArquivoMercante.SelecionarTodos.val();

    _importacaoArquivoMercante.ListaArquivos.val("");
    if (!todosSelecionado)
        _importacaoArquivoMercante.ListaArquivos.val(PreencherListaCodigos());
    data = RetornarObjetoPesquisa(_importacaoArquivoMercante);

    exibirConfirmacao("Atenção!", "Deseja realizar o download de todos os arquivos selecionados?", function () {
        executarDownload("AlteracaoArquivoMercante/DownloadLoteArquivo", data);
    });
}

function EnviarPorEmailClick(e, sender) {
    if (!_importacaoArquivoMercante.SelecionarTodos.val()) {
        EnviarPorEmail(false);
    } else {
        EnviarPorEmail(true);
    }
}

function EnviarPorEmail(todosSelecionado) {
    var data = null;

    _importacaoArquivoMercante.ListaArquivos.val("");
    if (!todosSelecionado)
        _importacaoArquivoMercante.ListaArquivos.val(PreencherListaCodigos());
    data = RetornarObjetoPesquisa(_importacaoArquivoMercante);

    exibirConfirmacao("Atenção!", "Realmente deseja enviar por e-mail em lote para os arquivos selecionados?", function () {
        executarReST("AlteracaoArquivoMercante/EnviarPorEmail", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail iniciado com sucesso.");
                buscarImportacaoArquivoMercante();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 99999999);
            }
        });
    });
}

function PreencherListaCodigos() {
    var codigos = new Array();
    var titulosSelecionados = _gridArquivos.ObterMultiplosSelecionados();
    $.each(titulosSelecionados, function (i, carga) {
        codigos.push({ Codigo: carga.Codigo });
    });
    return JSON.stringify(codigos);
}