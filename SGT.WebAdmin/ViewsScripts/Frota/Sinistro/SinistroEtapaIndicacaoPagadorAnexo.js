/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />

var _anexoIndicacaoPagador;
var _listaAnexoIndicacaoPagador;
var _gridAnexoIndicacaoPagador;
var _indicacaoPagadorSinistroAnexo;

var IndicacaoPagadorSinistroAnexo = function () {
    this.AdicionarAnexo = PropertyEntity({ eventClick: adicionarAnexoIndicacaoPagadorModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridIndicacaoPagadorAnexo();
    });
};

var AnexoIndicacaoPagador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoIndicacaoPagador.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoIndicacaoPagadorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

function loadEtapaIndicacaoPagadorAnexo() {
    _indicacaoPagadorSinistroAnexo = new IndicacaoPagadorSinistroAnexo();
    KoBindings(_indicacaoPagadorSinistroAnexo, "knockoutFluxoSinistroIndicacaoPagadorAnexo");

    _anexoIndicacaoPagador = new AnexoIndicacaoPagador();
    KoBindings(_anexoIndicacaoPagador, "knockoutIndicacaoPagadorAnexo");

    loadGridAnexosIndicacaoPagador();
}

function loadGridAnexosIndicacaoPagador() {
    var linhasPorPagina = 5;

    var opcaoDownload = { descricao: "Download", id: guid(), evento: "onclick", metodo: downloadIndicacaoPagadorAnexoClick, icone: "", visibilidade: true };
    var opcaoExcluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: removerIndicacaoPagadorAnexoClick, icone: "", visibilidade: obterVisibilidadeExcluirAnexoIndicacaoPagador };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoDownload, opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "40%", className: "text-align-left" }
    ];

    _gridAnexoIndicacaoPagador = new BasicDataTable(_indicacaoPagadorSinistroAnexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPagina);
    _gridAnexoIndicacaoPagador.CarregarGrid([]);
}

function obterVisibilidadeExcluirAnexoIndicacaoPagador() {
    return _etapaDadosSinistro.Etapa.val() == EnumEtapaSinistro.IndicacaoPagador;
}

function recarregarGridIndicacaoPagadorAnexo() {
    var anexosGrid = _indicacaoPagadorSinistroAnexo.Anexos.val();
    _gridAnexoIndicacaoPagador.CarregarGrid(anexosGrid);
}

function adicionarAnexoIndicacaoPagadorClick() {
    if (!ValidarCamposObrigatorios(_anexoIndicacaoPagador)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var formData = new FormData();

    formData.append("Arquivo", document.getElementById(_anexoIndicacaoPagador.Arquivo.id).files[0]);
    formData.append("Descricao", _anexoIndicacaoPagador.Descricao.val());

    enviarArquivo("SinistroIndicacaoPagadorAnexo/AnexarArquivos", { Codigo: _etapaDadosSinistro.Codigo.val() }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                adicionarAnexoIndicacaoPagadadorLocal(retorno.Data.Anexos);
                LimparCampos(_anexoIndicacaoPagador);
                Global.fecharModal("divModalIndicacaoPagadorAnexo");
                exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar o arquivo.", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function adicionarAnexoIndicacaoPagadadorLocal(anexos) {
    var anexosGrid = _gridAnexoIndicacaoPagador.BuscarRegistros();
    anexos.forEach(function (anexo) {
        anexosGrid.push({
            Codigo: anexo.Codigo,
            Descricao: anexo.Descricao,
            NomeArquivo: anexo.NomeArquivo
        });
    });

    _gridAnexoIndicacaoPagador.CarregarGrid(anexosGrid);
}

function adicionarAnexoIndicacaoPagadorModalClick() {
    Global.abrirModal('divModalIndicacaoPagadorAnexo');
}

function downloadIndicacaoPagadorAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    
    executarDownload("SinistroIndicacaoPagadorAnexo/DownloadAnexo", dados);
}

function removerIndicacaoPagadorAnexoClick(registroSelecionado) {
    executarReST("SinistroIndicacaoPagadorAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                removerAnexoIndicacaoPagadorLocal(registroSelecionado);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });

    var anexosGrid = _gridAnexoDevolucaoPallets.BuscarRegistros();

    for (var i = 0; i < anexosGrid.length; i++) {
        if (anexosGrid[i].Codigo == registroSelecionado.Codigo) {
            anexosGrid.splice(i, 1);
            break;
        }
    }

    _gridAnexoDevolucaoPallets.CarregarGrid(anexosGrid);
}

function removerAnexoIndicacaoPagadorLocal(registroSelecionado) {
    var anexosGrid = _gridAnexoIndicacaoPagador.BuscarRegistros();

    for (var i = 0; i < anexosGrid.length; i++) {
        if (anexosGrid[i].Codigo == registroSelecionado.Codigo) {
            anexosGrid.splice(i, 1);
            break;
        }
    }
    
    _gridAnexoIndicacaoPagador.CarregarGrid(anexosGrid);
}

function setarEtapaDesabilitadaIndicacaoPagadorAnexo(habilitar) {
    _indicacaoPagadorSinistroAnexo.AdicionarAnexo.enable(habilitar);
}