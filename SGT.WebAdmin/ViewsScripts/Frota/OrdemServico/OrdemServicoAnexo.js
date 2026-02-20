/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/GrupoServico.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />
/// <reference path="PesquisaOrdemServico.js" />
/// <reference path="ResumoOrdemServico.js" />
/// <reference path="EtapaOrdemServico.js" />
/// <reference path="Servico.js" />
/// <reference path="Orcamento.js" />
/// <reference path="Fechamento.js" />
/// <reference path="Aprovacao.js" />
/// <reference path="OrdemServicoLote.js" />

var _lancamentoArquivo;

var LancamentoArquivo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });
    this.CodigoOrdemServicoFrotaOrcamentoServico = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.CodigoAnexo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), required: true });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _lancamentoArquivo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(RecarregarGridAnexo);

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

function LoadAnexoArquivoOS() {
    _lancamentoArquivo = new LancamentoArquivo();
    KoBindings(_lancamentoArquivo, "knoutControleArquivoOrdemServico");

    loadGridAnexo();
}

function AdicionarAnexoOSClick(e, sender) {
    BuscarAnexos();
}

function loadGridAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "fal fa-download" };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "fal fa-trash" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_lancamentoArquivo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

function RecarregarGridAnexo() {
    var anexos = ObterAnexos();

    _gridAnexo.CarregarGrid(anexos);
}

function LimparCamposAnexosOS() {
    LimparCampos(_lancamentoArquivo);
    _lancamentoArquivo.Anexos.val(_lancamentoArquivo.Anexos.def);
}

function AdicionarAnexoClick() {
    var arquivo = document.getElementById(_lancamentoArquivo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _lancamentoArquivo.DescricaoAnexo.val(),
        NomeArquivo: _lancamentoArquivo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_lancamentoArquivo.Codigo.val() > 0)
        EnviarAnexos(_lancamentoArquivo.Codigo.val(), [anexo]);
    else {
        var anexos = ObterAnexos();
        anexos.push(anexo);
        _lancamentoArquivo.Anexos.val(anexos.slice());
    }

    LimparCamposAnexo();
    arquivo.value = null;
}

function EnviarArquivosAnexados(codigo) {
    var anexos = ObterAnexos();

    EnviarAnexos(codigo, anexos);
}

function ObterAnexos() {
    return _lancamentoArquivo.Anexos.val().slice();
}

function LimparCamposAnexo() {
    _lancamentoArquivo.DescricaoAnexo.val("");
    _lancamentoArquivo.NomeArquivo.val("");
    _lancamentoArquivo.Arquivo.val("");
}

function RemoverAnexoLocal(registroSelecionado) {
    var listaAnexos = ObterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _lancamentoArquivo.Anexos.val(listaAnexos);
}

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

function SalvarClick(e, sender) {
    EnviarArquivosAnexados(_servicoOrcamentoOrdemServico.Codigo.val());
}

function EnviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexo(anexos);

    if (formData) {
        enviarArquivo("OrcamentoOrdemServicoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _lancamentoArquivo.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("OrcamentoOrdemServicoAnexo/DownloadAnexoOverride", { Codigo: registroSelecionado.Codigo });
    _gridControleArquivo.CarregarGrid();
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        RemoverAnexoLocal(registroSelecionado);
    else {
        executarReST("OrcamentoOrdemServicoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    RemoverAnexoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function BuscarAnexos() {
    LimparCamposAnexosOS();

    executarReST("OrcamentoOrdemServicoAnexo/BuscarAnexosPorOS", { Codigo: _servicoOrcamentoOrdemServico.Codigo.val() }, function (e) {
        if (e.Success) {
            _lancamentoArquivo.Anexos.val(e.Data.Anexos);
            Global.abrirModal('divModalControleArquivoOrdemServico');
        } else {
            Global.abrirModal('divModalControleArquivoOrdemServico');
        }
    }, null);
}