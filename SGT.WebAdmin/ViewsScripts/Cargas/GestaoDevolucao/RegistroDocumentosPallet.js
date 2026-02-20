//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaRegistroDocumentosPallet;
var _gridRegistroDocumentosPallet;
var _tipoDocumentoEnviado;
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaRegistroDocumentosPallet = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.AdicionarXMLNotasFiscais = PropertyEntity({ text: "Adicionar", eventClick: abrirModalAdicionarArquivoRegistroDocumentosPalletclick, visible: ko.observable(true), enable: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Arquivo:", val: ko.observable("") });
    this.AdicionarArquivo = PropertyEntity({ text: "Adicionar", eventClick: adicionarArquivoRegistroDocumentosPallet, visible: ko.observable(true), enable: ko.observable(true) });
    this.AbrirModalAdicionarComprovantePagamento = PropertyEntity({ text: "Adicionar Comprovante", eventClick: abrirModalAdicionarArquivoComprovantePagamentoPalletClick, visible: ko.observable(true), enable: ko.observable(true) });
    this.AdicionarComprovante = PropertyEntity({ text: "Adicionar", eventClick: adicionarArquivoRegistroDocumentosPallet, visible: ko.observable(false), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação", visible: ko.observable(true), enable: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true) });
    this.PossuiDocumentosAnexados = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.ClientePagouNFe = PropertyEntity({ text: ko.observable("Cliente pagou a NFe?"), val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), options: EnumSimNao.obterOpcoes() });
    this.ConfirmarEnvioDocumentos = PropertyEntity({ text: "Confirmar", eventClick: confirmarEnvioDocumentos, visible: ko.observable(true), enable: ko.observable(true) });
    this.CancelarEnvioDocumentos = PropertyEntity({ text: "Cancelar", eventClick: cancelarEnvioDocumentosClick, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, eventClick: limparEnvioArquivo, visible: ko.observable(true), enable: ko.observable(true) });

    //Dados XML Nota Fiscal
    this.Valor = PropertyEntity({ text: "Valor", val: ko.observable(""), getType: typesKnockout.string });
    this.Numero = PropertyEntity({ text: "Número", val: ko.observable(""), getType: typesKnockout.string });
    this.Chave = PropertyEntity({ text: "Chave", val: ko.observable(""), getType: typesKnockout.string });
    this.DataEmissao = PropertyEntity({ text: "Data de emissão", val: ko.observable(""), getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _gestaoDevolucaoEtapaRegistroDocumentosPallet.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoRegistroDocumentosPallet(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/RegistroDocumentosPallet.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaRegistroDocumentosPallet = new GestaoDevolucaoEtapaRegistroDocumentosPallet();
                KoBindings(_gestaoDevolucaoEtapaRegistroDocumentosPallet, "knockoutRegistroDocumentosPallet");

                PreencherObjetoKnout(_gestaoDevolucaoEtapaRegistroDocumentosPallet, r);

                carregarGridRegistroDocumentos();

                controlarVisibilidadeRegistroDocumentos(_gestaoDevolucaoEtapaRegistroDocumentosPallet.PossuiDocumentosAnexados.val())
                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoEtapaRegistroDocumentosPallet);

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function carregarGridRegistroDocumentos() {
    var download = {
        descricao: "Download",
        id: guid(),
        evento: "onclick",
        metodo: downloadRegistroDocumentoPallet,
        tamanho: "10",
        icone: "",
        visibilidade: true
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [download] };

    var dados = {
        CodigoGestaoDevolucao: _gestaoDevolucaoEtapaRegistroDocumentosPallet.CodigoGestaoDevolucao
    }

    _gridRegistroDocumentosPallet = new GridView("grid-gestao-devolucao-registro-documentos-pallet", "GestaoDevolucao/ObterGridRegistroDocumentosPallet", dados, menuOpcoes, null, 25, null, null, null, null, null, null, null, null, null, null, null);
    _gridRegistroDocumentosPallet.CarregarGrid();
}
function abrirModalAdicionarArquivoRegistroDocumentosPalletclick() {
    _gestaoDevolucaoEtapaRegistroDocumentosPallet.AdicionarArquivo.visible(true);
    _gestaoDevolucaoEtapaRegistroDocumentosPallet.AdicionarComprovante.visible(false);
    _tipoDocumentoEnviado = EnumTipoRegistroPermutaPallet.Documento;

    Global.abrirModal('divModalAdicionarArquivosNotasFiscaisPallet');
}

function abrirModalAdicionarArquivoComprovantePagamentoPalletClick() {
    _gestaoDevolucaoEtapaRegistroDocumentosPallet.AdicionarArquivo.visible(false);
    _gestaoDevolucaoEtapaRegistroDocumentosPallet.AdicionarComprovante.visible(true);
    _tipoDocumentoEnviado = EnumTipoRegistroPermutaPallet.Comprovante;

    Global.abrirModal('divModalAdicionarArquivosNotasFiscaisPallet');
}

function adicionarArquivoRegistroDocumentosPallet() {
    var arquivo = document.getElementById(_gestaoDevolucaoEtapaRegistroDocumentosPallet.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        NomeArquivo: _gestaoDevolucaoEtapaRegistroDocumentosPallet.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var formData = obterFormDataAnexo([anexo]);

    formData.append("Arquivo", anexo.Arquivo);
    exibirConfirmacao("Confirmação", "Realmente deseja enviar os arquivos?", function () {
        enviarArquivo("GestaoDevolucao/ImportarArquivosNotaFiscalPermutaPallet",
            {
                CodigoGestaoDevolucao: _gestaoDevolucaoEtapaRegistroDocumentosPallet.CodigoGestaoDevolucao.val(),
                Observacao: _gestaoDevolucaoEtapaRegistroDocumentosPallet.Observacao.val(),
                TipoRegistroPermutaPallet: _tipoDocumentoEnviado == 1 ? EnumTipoRegistroPermutaPallet.Comprovante : EnumTipoRegistroPermutaPallet.Documento
            }, formData, function (retorno) {
                if (retorno.Success) {
                    limparEnvioArquivo();
                    recarregarEtapaRegistroDocumentosPallet();
                    return exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
                }
                return exibirMensagem(tipoMensagem.falha, "Não foi possível anexar o arquivo.", retorno.Msg);
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
    });
}

function confirmarEnvioDocumentos() {

    let dados = {
        CodigoGestaoDevolucao: _gestaoDevolucaoEtapaRegistroDocumentosPallet.CodigoGestaoDevolucao.val(),
        ClientePagouNFe: _gestaoDevolucaoEtapaRegistroDocumentosPallet.ClientePagouNFe.val(),
    }
    exibirConfirmacao("Confirmação", dados.ClientePagouNFe ? "Tem certeza que deseja <b> CONFIRMAR A PERMUTA </b> com as informações incluídas?" : "Tem certeza que deseja <b> DIRECIONAR O AGENDAMENTO DE COLETA </b> com as informações incluídas?", function () {

        executarReST("GestaoDevolucao/ConfirmarRegistrosDocumentosPermutaPallet", dados, function (r) {
            if (r.Success) {
                cancelarEnvioDocumentosClick();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Pagamento informado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function limparEnvioArquivo() {
    LimparCampos(_gestaoDevolucaoEtapaRegistroDocumentosPallet);
    Global.fecharModal('divModalAdicionarArquivosNotasFiscaisPallet');
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas
function obterFormDataAnexo(anexos) {
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

function controlarVisibilidadeRegistroDocumentos(possuiDocumentosAnexados) {
    if (possuiDocumentosAnexados) {
        $('#adicionar-notas-container').hide();
        $('#registro-documentos-container').show();

    } else {
        $('#adicionar-notas-container').show();
        $('#registro-documentos-container').hide();
    }
}

function downloadRegistroDocumentoPallet(e) {
    executarDownload("GestaoDevolucao/DownloadRegistroDocumentoPallet", { Codigo: e.Codigo });
}

function cancelarEnvioDocumentosClick() {
    controlarVisibilidadeGrids();
}

function recarregarEtapaRegistroDocumentosPallet() {
    let id = _etapasGestaoDevolucao.Etapas.val().find(o => o.etapa == 16).id

    if (id)
        document.getElementById(id).click();

    controlarVisibilidadeRegistroDocumentos(true);
}
// #endregion Funções Privadas