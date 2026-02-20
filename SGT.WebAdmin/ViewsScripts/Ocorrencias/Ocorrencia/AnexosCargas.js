/// <reference path="Anexos.js" />

//*******MAPEAMENTO*******

var _anexosCarga;
var _gridAnexosCarga;

var AnexosCarga = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosCarga.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosCargas();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoCargaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosCargas() {
    _anexosCarga = new AnexosCarga();
    KoBindings(_anexosCarga, "knockoutCadastroAnexosCarga");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Download, id: guid(), metodo: downloadAnexoCargaClick, icone: "", visibilidade: visibleDownloadOcorrencia };
    var remover = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Remover, id: guid(), metodo: removerAnexoCargaClick, icone: "", visibilidade: visibleRemoverOcorrencia };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 9, opcoes: [download, remover] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Ocorrencias.Ocorrencia.Descricao, width: "70%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Ocorrencias.Ocorrencia.Nome, width: "25%", className: "text-align-left" }
    ];

    // Grid
    var linhasPorPaginas = 7;
    _gridAnexosCarga = new BasicDataTable(_anexosCarga.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosCarga.CarregarGrid([]);

    //-- Controle de anexos
    _ocorrencia.SituacaoOcorrencia.val.subscribe(function () {
        AlternaTelaDeAnexos(_anexosCarga);
    });
}

function anexarCargaSumarizada(itemGrid) {
    _anexosCarga.CodigoVeiculo.val(itemGrid.CodigoVeiculo);
    RenderizarGridAnexosCargas();
    Global.abrirModal('divModalGerenciarAnexosCarga');
}

function downloadAnexoCargaClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("OcorrenciaAnexos/DownloadAnexoCarga", data);
}

function removerAnexoCargaClick(dataRow) {
    var listaAnexos = GetAnexosCargas();
    RemoverAnexoOcorrencia(dataRow, listaAnexos, _anexosCarga, "OcorrenciaAnexos/ExcluirAnexoCarga");
}

function adicionarAnexoCargaClick() {
    // Permissao
    if (!PodeGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Anexos, Localization.Resources.Ocorrencias.Ocorrencia.SituacaoOcorrenciaNaoPermiteAnexarAquivos);

    // Busca o input de arquivos
    var file = document.getElementById(_anexosCarga.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Anexos, Localization.Resources.Ocorrencias.Ocorrencia.NenhumArquivoSelecionado);

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        CodigoVeiculo: _anexosCarga.CodigoVeiculo.val(),
        Descricao: _anexosCarga.Descricao.val(),
        NomeArquivo: _anexosCarga.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_ocorrencia.Codigo.val() > 0) {
        EnviarAnexoCarga(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexos = GetAnexosCargas(true);
        listaAnexos.push(anexo);
        _anexosCarga.Anexos.val(listaAnexos.slice());
    }

    // Limpa os campos mas mantem o codigo veiculo
    var codigoVeiculo = _anexosCarga.CodigoVeiculo.val();
    LimparCampos(_anexosCarga);
    _anexosCarga.CodigoVeiculo.val(codigoVeiculo);
    _anexosCarga.Arquivo.val("");
    file.value = null;
}

//*******MÉTODOS*******
function GetAnexosCargas(todosAnexos) {
    var codigoVeiculo = _anexosCarga.CodigoVeiculo.val();

    // Retorna um clone do array para não prender a referencia
    var lista = (_anexosCarga.Anexos.val() || []).slice();

    return lista.filter(function (anexo) {
        if (todosAnexos || anexo.CodigoVeiculo == codigoVeiculo)
            return anexo;
    });
}

function RenderizarGridAnexosCargas() {
    // Busca a lista
    var anexos = GetAnexosCargas();

    // E chama o metodo da grid
    _gridAnexosCarga.CarregarGrid(anexos);
}


function EnviarArquivosAnexadosCarga(hashAnexos) {
    // Busca a lista
    var grupoAnexos = AgruparAnexosCargas();

    // Limpa grid
    _anexosCarga.Anexos.val([]);

    if (!$.isEmptyObject(grupoAnexos)) {
        for (var i in grupoAnexos) {
            var dados = {
                CodigoOcorrencia: _ocorrencia.Codigo.val(),
                CodigoVeiculo: grupoAnexos[i].CodigoVeiculo,
                HashAnexos: hashAnexos,
            };

            CriaEEnviaFormDataCarga(grupoAnexos[i].Anexos, dados);
        }
    }
}

function EnviarAnexoCarga(anexo) {
    var anexos = [anexo];
    var dados = {
        CodigoOcorrencia: _ocorrencia.Codigo.val(),
        CodigoVeiculo: anexo.CodigoVeiculo,
    }

    CriaEEnviaFormDataCarga(anexos, dados);
}

function CriaEEnviaFormDataCarga(anexos, dados) {
    // Busca todos file
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("OcorrenciaAnexos/AnexarArquivosCarga?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                var anexosRetornos = GetAnexosCargas(true);
                arg.Data.Anexos.map(function (an) { anexosRetornos.push(an); })
                _anexosCarga.Anexos.val(anexosRetornos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.Ocorrencia.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ArquivoAnexadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelAnexarArquivo, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function AgruparAnexosCargas() {
    // Pega anexos
    var lista = GetAnexosCargas(true);

    // Agrupa
    var agrupamento = {};
    lista.forEach(function (anexo) {
        var objGrupo = agrupamento[anexo.CodigoVeiculo] || {
            CodigoVeiculo: anexo.CodigoVeiculo,
            Anexos: []
        };

        objGrupo.Anexos.push(anexo);
        agrupamento[anexo.CodigoVeiculo] = objGrupo;
    });

    return agrupamento;
}

function PreencherAnexosCargas() {
    executarReST("OcorrenciaAnexos/BuscarAnexosCargaPorCodigo", { Ocorrencia: _ocorrencia.Codigo.val() }, function (arg) {
        if (arg.Success) {
            _anexosCarga.Anexos.val(arg.Data.Anexos);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
        }
    });
}