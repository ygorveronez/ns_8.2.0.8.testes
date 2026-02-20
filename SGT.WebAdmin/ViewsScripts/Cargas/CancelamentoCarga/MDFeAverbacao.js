
//*******EVENTOS*******

function BuscarCargasMDFeAverbacao() {
    var cancelar = { descricao: Localization.Resources.Cargas.CancelamentoCarga.CancelarAverbacao , id: guid(), metodo: CancelarAverbacaoMDFeClick, icone: "", visibilidade: VisibilidadeCancelarAverbacaoMDFe };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [cancelar] };

    _gridCargaMDFeAverbacao = new GridView(_mdfe.BuscarAverbacoes.idGrid, "CargaMDFe/ConsultarCargaMDFeAverbacao", _mdfe, menuOpcoes);
    _gridCargaMDFeAverbacao.CarregarGrid(function () {
        if (_gridCargaMDFeAverbacao.NumeroRegistros() > 0) {
            $("#liAverbacaoMDFes").show();
        } else {
            $("#liAverbacaoMDFes").hide();
        }
    });
}

function BuscarAverbacoesMDFesCargaClick() {
    _gridCargaMDFeAverbacao.CarregarGrid();
}

function CancelarAverbacaoMDFeClick(datagrid) {
    var data = {
        Codigo: datagrid.Codigo,
        Carga: _cancelamento.Carga.codEntity()
    };

    executarReST("CargaMDFe/CancelarAvervacao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaMDFeAverbacao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function VisibilidadeCancelarAverbacaoMDFe(datagrid) {
    return datagrid.Status == EnumStatusAverbacaoMDFe.Sucesso;
}

function LiberarCancelamentoComAverbacaoMDFeRejeitadaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.CancelamentoCarga.LiberarCancelamento, function () {
        executarReST("CancelamentoCargaMDFe/LiberarAverbacaoRejeitada", { Codigo: _cancelamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CancelamentoLiberado);
                    BuscarCancelamentoPorCodigo(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}