function AtualizarGridCIOT() {
    var configuracao = $("body").data("configuracaoEmpresa");

    var dados = {
        inicioRegistros: 0,
        NumeroInicial: $("#txtNumeroInicial").val(),
        NumeroFinal: $("#txtNumeroFinal").val(),
        DataInicial: $("#txtDataInicial").val(),
        DataFinal: $("#txtDataFinal").val(),
        Placa: $("#txtFiltroPlaca").val(),
        CIOT: $("#txtFiltroCIOT").val(),
        Status: $("#selFiltroStatus").val(),
    };

    var opcoes = [];

    opcoes.push({ Descricao: "Editar", Evento: EditarCIOT });
    if (!OpcaoAbertura) opcoes.push({ Descricao: "Emitir", Evento: EmitirCIOT });
    if (!OpcaoAbertura) opcoes.push({ Descricao: "Emitir CT-es", Evento: EmitirCTes });
    opcoes.push({ Descricao: "Cancelar", Evento: AbrirTelaCancelamentoCIOT });

    if (configuracao != null && configuracao.TipoIntegradoraCIOT == 3)
        opcoes.push({ Descricao: "Encerrar", Evento: EncerrarCIOT });

    opcoes.push({ Descricao: "Contrato", Evento: DownloadContrato });
    opcoes.push({ Descricao: "Recibo", Evento: DownloadRecibo });
    opcoes.push({ Descricao: "Integrações", Evento: AbrirTelaIntegracoes });

    CriarGridView("/IntegracaoSigaFacil/Consultar?callback=?", dados, "tbl_ciot_table", "tbl_ciot", "tbl_paginacao_ciot", opcoes, [0], null);
}

function DownloadContrato(ciot) {
    executarDownload("/IntegracaoSigaFacil/DownloadContratoTransporte", { CodigoCIOT: ciot.data.Codigo });
}

function DownloadRecibo(ciot) {
    executarDownload("/IntegracaoSigaFacil/DownloadRecibo", { CodigoCIOT: ciot.data.Codigo });
}

function EncerrarCIOT(ciot) {
    jConfirm("Deseja realmente <b>encerrar</b> o CIOT Nº " + ciot.data.Numero + " ? <b>Este processo é irreversível!</br>", "Atenção", function (res) {
        if (res) {
            executarRest("/IntegracaoSigaFacil/Encerrar?callback=?", { CodigoCIOT: ciot.data.Codigo }, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Encerramento solicitado com sucesso!", "Sucesso!");
                    AtualizarGridCIOT();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    });
}

function EmitirCTes(ciot) {
    jConfirm("Deseja realmente emitir os CT-es do CIOT " + ciot.data.Numero + "?", "Atenção!", function (confirm) {
        if (confirm) {
            executarRest("/IntegracaoSigaFacil/EmitirCTes?callback=?", { CodigoCIOT: ciot.data.Codigo }, function (r) {

                if (r.Sucesso) {
                    ExibirMensagemSucesso("Emissão CTe-s solicitada com sucesso!", "Sucesso!");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }

                AtualizarGridCIOT();
            });
        }
    });
}

function EmitirCIOT(ciot) {
    executarRest("/IntegracaoSigaFacil/Emitir?callback=?", { CodigoCIOT: ciot.data.Codigo }, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("CIOT emitido com sucesso.", "Sucesso!");
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }

        AtualizarGridCIOT();
    });
}

function EditarCIOT(ciot) {
    executarRest("/IntegracaoSigaFacil/ObterDetalhes" + (EFreteAbertura ? "Abertura" : "") + "?callback=?", { CodigoCIOT: ciot.data.Codigo }, function (r) {
        if (r.Sucesso) {
            RenderizarCIOT(r.Objeto);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}