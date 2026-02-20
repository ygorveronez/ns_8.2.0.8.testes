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
/// <reference path="Ocorrencia.js" />
/// <reference path="Anexos.js" />

//*******MAPEAMENTO*******

var _anexosVeiculoContrato;
var _gridAnexosVeiculoContrato;

var AnexosVeiculoContrato = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosVeiculoContrato.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosVeiculoContrato();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoVeiculoContratoClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Adicionar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAnexosVeiculoContrato() {
    _anexosVeiculoContrato = new AnexosVeiculoContrato();
    KoBindings(_anexosVeiculoContrato, "knockoutCadastroAnexosVeiculoContrato");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Download, id: guid(), metodo: downloadAnexoVeiculoContratoClick, icone: "", visibilidade: visibleDownloadOcorrencia };
    var remover = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Remover, id: guid(), metodo: removerAnexoVeiculoContratoClick, icone: "", visibilidade: visibleRemoverOcorrencia };

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
    _gridAnexosVeiculoContrato = new BasicDataTable(_anexosVeiculoContrato.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosVeiculoContrato.CarregarGrid([]);

    //-- Controle de anexos
    _ocorrencia.SituacaoOcorrencia.val.subscribe(function () {
        AlternaTelaDeAnexos(_anexosVeiculoContrato);
    });
}

function anexarVeiculoContrato(itemGrid) {
    _anexosVeiculoContrato.CodigoVeiculo.val(itemGrid.CodigoVeiculo);
    RenderizarGridAnexosVeiculoContrato();
    Global.abrirModal('divModalGerenciarAnexosVeiculoContrato');
}

function downloadAnexoVeiculoContratoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("OcorrenciaContratoVeiculoAnexo/DownloadAnexo", data);
}

function removerAnexoVeiculoContratoClick(dataRow) {
    var listaAnexos = GetAnexosVeiculoContratos(true);
    RemoverAnexoOcorrencia(dataRow, listaAnexos, _anexosVeiculoContrato, "OcorrenciaContratoVeiculoAnexo/ExcluirAnexo");
}

function adicionarAnexoVeiculoContratoClick() {
    // Permissao
    if (!PodeGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Anexos, Localization.Resources.Ocorrencias.Ocorrencia.SituacaoOcorrenciaNaoPermiteAnexarAquivos);

    // Busca o input de arquivos
    var file = document.getElementById(_anexosVeiculoContrato.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.Anexos, Localization.Resources.Ocorrencias.Ocorrencia.NenhumArquivoSelecionado);

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        CodigoVeiculo: _anexosVeiculoContrato.CodigoVeiculo.val(),
        Descricao: _anexosVeiculoContrato.Descricao.val(),
        NomeArquivo: _anexosVeiculoContrato.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_ocorrencia.Codigo.val() > 0) {
        EnviarAnexoVeiculoContrato(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexos = GetAnexosVeiculoContratos(true);
        listaAnexos.push(anexo);
        _anexosVeiculoContrato.Anexos.val(listaAnexos.slice());
    }

    // Limpa os campos mas mantem o codigo veiculo
    var codigoVeiculo = _anexosVeiculoContrato.CodigoVeiculo.val();
    LimparCampos(_anexosVeiculoContrato);
    _anexosVeiculoContrato.CodigoVeiculo.val(codigoVeiculo);
    _anexosVeiculoContrato.Arquivo.val("");
    file.value = null;
}

//*******MÉTODOS*******
function GetAnexosVeiculoContratos(todosAnexos) {
    var codigoVeiculo = _anexosVeiculoContrato.CodigoVeiculo.val();

    // Retorna um clone do array para não prender a referencia
    var lista = (_anexosVeiculoContrato.Anexos.val() || []).slice();

    return lista.filter(function (anexo) {
        if (todosAnexos || anexo.CodigoVeiculo == codigoVeiculo)
            return anexo;
    });
}

function RenderizarGridAnexosVeiculoContrato() {
    // Busca a lista
    var anexos = GetAnexosVeiculoContratos();

    // E chama o metodo da grid
    _gridAnexosVeiculoContrato.CarregarGrid(anexos);
}


function EnviarArquivosAnexadosVeiculoContrato(hashAnexos) {
    // Busca a lista
    var grupoAnexos = AgruparAnexosVeiculoContratos();

    // Limpa grid
    _anexosVeiculoContrato.Anexos.val([]);

    if (!$.isEmptyObject(grupoAnexos)) {
        for (var i in grupoAnexos) {
            var dados = {
                CodigoOcorrencia: _ocorrencia.Codigo.val(),
                CodigoVeiculo: grupoAnexos[i].CodigoVeiculo,
                HashAnexos: hashAnexos,
            };

            CriaEEnviaFormDataVeiculoContrato(grupoAnexos[i].Anexos, dados);
        }
    }
}

function EnviarAnexoVeiculoContrato(anexo) {
    var anexos = [anexo];
    var dados = {
        CodigoOcorrencia: _ocorrencia.Codigo.val(),
        CodigoVeiculo: anexo.CodigoVeiculo,
    }

    CriaEEnviaFormDataVeiculoContrato(anexos, dados);
}

function CriaEEnviaFormDataVeiculoContrato(anexos, dados) {
    // Busca todos file
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("OcorrenciaContratoVeiculoAnexo/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                var anexosRetornos = GetAnexosVeiculoContratos(true);

                arg.Data.Anexos.map(function (an) {
                    var jaIncluso = anexosRetornos.some(function (_anexo) { return _anexo.Codigo == an.Codigo });
                    if (!jaIncluso) anexosRetornos.push(an);
                })

                _anexosVeiculoContrato.Anexos.val(anexosRetornos);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.Ocorrencia.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ArquivoAnexadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelAnexarArquivo, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function AgruparAnexosVeiculoContratos() {
    // Pega anexos
    var lista = GetAnexosVeiculoContratos(true);

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

function PreencherAnexosVeiculoContratos() {
    executarReST("OcorrenciaContratoVeiculoAnexo/BuscarAnexosVeiculoContratoPorCodigo", { Ocorrencia: _ocorrencia.Codigo.val() }, function (arg) {
        if (arg.Success) {
            _anexosVeiculoContrato.Anexos.val(arg.Data.Anexos);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
        }
    });
}

function LimparOcorrenciaAnexosVeiculoContrato() {
    LimparCampos(_anexosVeiculoContrato);
    _anexosVeiculoContrato.Anexos.val([])
}