/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumTipoHistoricoInfracao.js" />

var _acompanhamentoAdicionarHistorico,
    _gridAnexosAcompanhamentoSinistro;

var AcompanhamentoSinistroAdicionarHistorico = function () {
    this.AdicionarAnexo = PropertyEntity({ eventClick: adicionarAnexoHistoricoModalClick, type: types.event, text: "Adicionar Anexo", visible: ko.observable(true), idBtnSearch: guid() });

    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observableArray(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Sinistro = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Observacao = PropertyEntity({ val: ko.observable(""), text: "Observação:", maxlength: 300, enable: ko.observable(true) });
    this.Data = PropertyEntity({ val: ko.observable(""), text: "*Data/Hora:", getType: typesKnockout.dateTime, required: true });
    this.Tipo = PropertyEntity({ options: EnumTipoHistoricoInfracao.obterOpcoesFluxoSinistro(), val: ko.observable(EnumTipoHistoricoInfracao.Deferido), def: EnumTipoHistoricoInfracao.Deferido, text: "*Tipo:", required: true, enable: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridHistoricoAnexo();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarHistoricoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarHistoricoClick, type: types.event, text: "Atualizar", visible: this.Codigo.val });
}

function loadAcompanhamentoHistoricoSinistro() {
    _acompanhamentoAdicionarHistorico = new AcompanhamentoSinistroAdicionarHistorico();
    KoBindings(_acompanhamentoAdicionarHistorico, "knockoutAdicionarHistorico");

    loadGridAcompanhamentoSinistroHistoricoAnexo();
    loadEtapaAcompanhamentoHistoricoAnexo();
}

function loadGridAcompanhamentoSinistroHistoricoAnexo() {
    var linhasPorPagina = 5;

    var opcaoDownload = { descricao: "Download Anexo", id: guid(), evento: "onclick", metodo: downloadAnexoHistoricoSinistroClick, icone: "", visiblidade: true };
    var opcaoExcluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: removerAnexoHistoricoSinistroClick, icone: "", visiblidade: true };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoDownload, opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "40%", className: "text-align-left" }
    ];

    _gridAnexosAcompanhamentoSinistro = new BasicDataTable(_acompanhamentoAdicionarHistorico.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPagina);
    _gridAnexosAcompanhamentoSinistro.CarregarGrid([]);
}

function removerAnexoHistoricoSinistroClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        return removerAnexoHistoricoLocal(registroSelecionado);

    executarReST("SinistroHistoricoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                removerAnexoHistoricoLocal(registroSelecionado);

                var anexosExistentes = _etapaAcompanhamentoSinistro.Anexos.val();

                for (var i = 0; i < anexosExistentes.length; i++) {
                    if (anexosExistentes[i].Codigo == registroSelecionado.Codigo) {
                        anexosExistentes.splice(i, 1);
                        break;
                    }
                }

                _etapaAcompanhamentoSinistro.Anexos.val(anexosExistentes);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function removerAnexoHistoricoLocal(registroRemover) {
    var registros = _gridAnexosAcompanhamentoSinistro.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroRemover.Codigo) {
            registros.splice(i, 1);
            break;
        }
    }

    _acompanhamentoAdicionarHistorico.Anexos.val(registros);
}

function downloadAnexoHistoricoSinistroClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        return exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível realizar o download pois o anexo ainda não foi adicionado.")

    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("SinistroHistoricoAnexo/DownloadAnexo", dados);
}

function adicionarHistoricoClick() {
    if (!ValidarCamposObrigatorios(_acompanhamentoAdicionarHistorico)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios");
        return;
    }

    executarReST("SinistroHistorico/Adicionar", RetornarObjetoPesquisa(_acompanhamentoAdicionarHistorico), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Histórico adicionado com sucesso");

                adicionarAnexosHistorico(retorno.Data.Codigo);

                _etapaAcompanhamentoSinistro.Historico.val.push(retorno.Data);
                Global.fecharModal('divModalAdicionarHistorico');

                if (retorno.Data.FluxoFinalizado)
                    editarSinistroClick({ Codigo: _etapaDadosSinistro.Codigo.val() });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarHistoricoClick() {
    if (!ValidarCamposObrigatorios(_acompanhamentoAdicionarHistorico)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios");
        return;
    }

    executarReST("SinistroHistorico/Atualizar", RetornarObjetoPesquisa(_acompanhamentoAdicionarHistorico), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Histórico atualizado com sucesso");

                var registros = _etapaAcompanhamentoSinistro.Historico.val();

                for (var i = 0; i < registros.length; i++) {
                    if (registros[i].Codigo == retorno.Data.Codigo) {
                        registros[i] = retorno.Data;
                        break;
                    }
                }
                adicionarAnexosHistorico(retorno.Data.Codigo);

                _etapaAcompanhamentoSinistro.Historico.val(registros);
                Global.fecharModal('divModalAdicionarHistorico');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function adicionarAnexosHistorico(codigoHistorico) {
    if (_acompanhamentoAdicionarHistorico.Anexos.val().length == 0)
        return;

    var formData = new FormData();

    _acompanhamentoAdicionarHistorico.Anexos.val().forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    var dados = {
        Codigo: codigoHistorico
    };

    enviarArquivo("SinistroHistoricoAnexo/AnexarArquivos", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivos anexados com sucesso");

                var anexosExistentes = _etapaAcompanhamentoSinistro.Anexos.val();

                arg.Data.Anexos.forEach(function (anexoRetornado) {
                    if (!anexosExistentes.some(function (anexoExistente) { return anexoExistente.Codigo == anexoRetornado.Codigo; })) {
                        var novoAnexo = {
                            Codigo: anexoRetornado.Codigo,
                            Descricao: anexoRetornado.Descricao,
                            NomeArquivo: anexoRetornado.NomeArquivo,
                            CodigoHistorico: codigoHistorico
                        };

                        anexosExistentes.push(novoAnexo);
                    }
                });

                _etapaAcompanhamentoSinistro.Anexos.val(anexosExistentes);
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar arquivo.", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function recarregarGridHistoricoAnexo() {
    var registros = _acompanhamentoAdicionarHistorico.Anexos.val();

    var registrosAdicionar = [];

    registros.forEach(function (registro) {
        registrosAdicionar.push({
            Codigo: registro.Codigo,
            Descricao: registro.Descricao,
            NomeArquivo: registro.NomeArquivo
        });
    });

    _gridAnexosAcompanhamentoSinistro.CarregarGrid(registrosAdicionar);
}