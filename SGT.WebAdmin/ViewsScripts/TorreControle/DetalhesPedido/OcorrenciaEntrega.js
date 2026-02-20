var _ocorrenciaEntrega;
var _gridAdicionarAnexoOcorrencia;


var OcorrenciaEntrega = function () {
    this.CodigoCarga = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription(), val: ko.observable() });
    this.CodigoCargaEntrega = PropertyEntity({ val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), options: ko.observable(new Array()), def: 0, text: Localization.Resources.Cargas.ControleEntrega.Ocorrencia.getFieldDescription(), visible: ko.observable(true) });
    this.DataOcorrencia = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Data.getFieldDescription(), getType: typesKnockout.dateTime, def: Global.DataHoraAtual(), val: ko.observable(Global.DataHoraAtual()) });
    this.ObservacaoOcorrencia = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Observacoes.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string, maxlength: 2000 });

    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".jpg,.tif,.pdf", text: "Upload", icon: "fal fa-file", visible: ko.observable(true) });
    this.AnexosOcorrenciaAdicionar = PropertyEntity({ val: ko.observableArray([]), idGrid: guid() });
    this.AnexosOcorrencia = PropertyEntity({ type: types.dynamic, list: new Array(), idGrid: guid() });
    this.AdicionarOcorrenciaEntrega = PropertyEntity({ eventClick: adicionarOcorrenciaEntregaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

}

function LoadOcorrenciaEntrega() {
    _ocorrenciaEntrega = new OcorrenciaEntrega();
    KoBindings(_ocorrenciaEntrega, "knockoutOcorrenciaEntrega");

    ObterOcorrenciasEntrega();
    CarregarGridAnexoOcorrenciaEntregaAdicionar();
}

function adicionarOcorrenciaEntregaClick() {
    let ocorrenciaTipoOcorrencia = 0;

    const adicionado = {
        Codigo: _ocorrenciaEntrega.Ocorrencia.val(),
        Descricao: $("#" + _ocorrenciaEntrega.Ocorrencia.id + " option:selected").text(),
        DataOcorrencia: _ocorrenciaEntrega.DataOcorrencia.val(),
        ObservacaoOcorrencia: _ocorrenciaEntrega.ObservacaoOcorrencia.val(),
        CodigoTipoOcorrencia: ocorrenciaTipoOcorrencia,
    }

    const anexos = new FormData();
    _ocorrenciaEntrega.AnexosOcorrencia.list.forEach(function (anexo) {
        if (isNaN(anexo.Codigo))
            anexos.append("AnexosOcorrencia", anexo.Arquivo)
    });

    const dados = { Codigo: _ocorrenciaEntrega.CodigoCargaEntrega.val(), Ocorrencia: adicionado.Codigo, DataOcorrencia: _ocorrenciaEntrega.DataOcorrencia.val(), ObservacaoOcorrencia: _ocorrenciaEntrega.ObservacaoOcorrencia.val(), TipoOcorrencia: adicionado.CodigoTipoOcorrencia, };
    const codigoCarga = _ocorrenciaEntrega.CodigoCarga.val()

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.RealmenteDesejaAdicionarEssaOcorrencia, function () {
        enviarArquivo("ControleEntregaEntrega/AdicionarOcorrencia", dados, anexos, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    fecharModalAdicionarOcorrenciaEntrega();
                    limparAnexosOcorrenciaEntregaAdicionar();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
                    loadDetalhesPedidosTorreControle(codigoCarga);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function ObterOcorrenciasEntrega() {
    const ocorrenciasOption = new Array();
    executarReST("TipoOcorrencia/BuscarOcorrenciasMobile", { UsadoParaMotivoRejeicaoColetaEntrega: false, TipoAplicacaoColetaEntrega: EnumTipoAplicacaoColetaEntrega.Entrega, Codigo: _ocorrenciaEntrega.CodigoCargaEntrega.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                const ocorrencias = arg.Data;
                if (ocorrencias.length > 0) {
                    for (let i = 0; i < ocorrencias.length; i++) {
                        ocorrenciasOption.push({ text: ocorrencias[i].Descricao, value: ocorrencias[i].Codigo });
                    }
                    _ocorrenciaEntrega.Ocorrencia.options(ocorrenciasOption);
                    _ocorrenciaEntrega.Ocorrencia.val(ocorrencias[0].Codigo);
                    return;
                }
            }
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Sugestao, arg.Msg, 16000);
        }
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Sucesso.Falha, arg.Msg);
    }, null);
}

function limparAnexosOcorrenciaEntregaAdicionar() {
    _ocorrenciaEntrega.AnexosOcorrencia.list = new Array();
    RecarregarGridAnexoOcorrenciaEntregaAdicionar();
}

function CarregarGridAnexoOcorrenciaEntregaAdicionar() {
    const linhasPorPaginas = 5;
    const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoOcorrenciaEntregaClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoRemover] };
    _ocorrenciaEntrega.Upload.file = document.getElementById(_ocorrenciaEntrega.Upload.idFile);
    const header = [
        { data: "Codigo", visible: false },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "100%", className: "text-align-left" }
    ];

    _gridAdicionarAnexoOcorrencia = new BasicDataTable(_ocorrenciaEntrega.AnexosOcorrenciaAdicionar.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAdicionarAnexoOcorrencia.CarregarGrid([]);
}

function UploadChange() {
    if (_ocorrenciaEntrega.Upload.file.files.length > 0) {
        for (let i = 0; i <= _ocorrenciaEntrega.Upload.file.files.length - 1; i++) {
            let data = { Codigo: guid(), Arquivo: _ocorrenciaEntrega.Upload.file.files[i] }
            _ocorrenciaEntrega.AnexosOcorrencia.list.push(data);
        }
    }
    RecarregarGridAnexoOcorrenciaEntregaAdicionar()
}
function RecarregarGridAnexoOcorrenciaEntregaAdicionar() {
    const dados = []
    for (let i = 0; i <= _ocorrenciaEntrega.AnexosOcorrencia.list.length - 1; i++) {
        let arquivo = _ocorrenciaEntrega.AnexosOcorrencia.list[i];
        dados.push({ Codigo: arquivo.Codigo, NomeArquivo: arquivo.Arquivo.name })
    }
    _gridAdicionarAnexoOcorrencia.CarregarGrid(dados);
}
function removerAnexoOcorrenciaEntregaClick(e) {
    _ocorrenciaEntrega.AnexosOcorrencia.list = _ocorrenciaEntrega.AnexosOcorrencia.list.filter((obj) => obj.Codigo !== e.Codigo);
    RecarregarGridAnexoOcorrenciaEntregaAdicionar();
}

function fecharModalAdicionarOcorrenciaEntrega() {
    LimparCampos(_ocorrenciaEntrega);
    Global.fecharModal("divModalAdicionarOcorrenciaEntrega");
}