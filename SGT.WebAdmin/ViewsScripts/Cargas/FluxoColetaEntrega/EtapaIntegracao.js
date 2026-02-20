var _etapaEtapaIntegracao;
var _gridIntegracaoFluxoOcorrencia;
var _gridHistoricoIntegracaoFluxoOcorrencia;
var _pesquisaHistoricoIntegracaoFluxoOcorrencia;

var EtapaIntegracao = function () {
    this.CodigoColetaEntrega = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoFluxoOcorrencia.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function LoadEtapaIntegracaoFluxoOcorrencia() {
    _etapaEtapaIntegracao = new EtapaIntegracao();
    KoBindings(_etapaEtapaIntegracao, "divModalDetalhesEtapaIntegracao");

    ConfigurarPesquisaIntegracaoFluxoOcorrencia();
}

function ExibirDetalhesEtapaIntegracao(e) {
    _fluxoAtual = e;
    var data = { CodigoColetaEntrega: e.Codigo.val() }

    _etapaEtapaIntegracao.CodigoColetaEntrega.val(e.Codigo.val());

    executarReST("EtapaIntegracao/GerarIntegracoes", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridIntegracaoFluxoOcorrencia.CarregarGrid();                
                Global.abrirModal("divModalDetalhesEtapaIntegracao");

                if (arg.Data.EtapaLiberada)
                    atualizarFluxoColetaEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

var PesquisaHistoricoIntegracaoFluxoOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

function ConfigurarPesquisaIntegracaoFluxoOcorrencia() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoFluxoOcorrencia, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracaoFluxoOcorrencia, tamanho: "20", icone: "" });

    _gridIntegracaoFluxoOcorrencia = new GridView(_etapaEtapaIntegracao.Pesquisar.idGrid, "EtapaIntegracao/Pesquisa", _etapaEtapaIntegracao, menuOpcoes);
}

function ReenviarIntegracaoFluxoOcorrencia(data) {
    executarReST("EtapaIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoFluxoOcorrencia.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoFluxoOcorrencia(integracao) {
    BuscarHistoricoIntegracaoFluxoOcorrencia(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoFluxoOcorrencia");
}

function BuscarHistoricoIntegracaoFluxoOcorrencia(integracao) {
    _pesquisaHistoricoIntegracaoFluxoOcorrencia = new PesquisaHistoricoIntegracaoFluxoOcorrencia();
    _pesquisaHistoricoIntegracaoFluxoOcorrencia.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCarga, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoFluxoOcorrencia = new GridView("tblHistoricoIntegracaoFluxoOcorrencia", "EtapaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoFluxoOcorrencia, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoFluxoOcorrencia.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCarga(historicoConsulta) {
    executarDownload("EtapaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}