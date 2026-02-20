//*******MAPEAMENTO KNOUCKOUT*******

var _mapaPercursoMDFe;

//*******EVENTOS*******

function LoadPercurso() {
    _mapaPercursoMDFe = new MapaMDFe();
    _mapaPercursoMDFe.LoadMapaMDFe("mapaPercursoMDFe", function () { });
}

//*******MÉTODOS*******

function BuscarPercursoMDFe() {
    var data = {
        Origem: _cargaMDFeManual.Origem.codEntity(),
        Destino: _cargaMDFeManual.Destino.codEntity(),
        UsaDadosCTe: _cargaMDFeManual.UsarDadosCTe.val(),
        Destinos: JSON.stringify(_cargaMDFeManual.ListaDestinos.val())
    };

    if (data.Origem > 0 && (data.Destino > 0 || _cargaMDFeManual.ListaDestinos.val().length > 0)) {
        executarReST("CargaMDFeManualPercurso/BuscarRotaParaMDFe", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    AtualizarInformacoesMapaMDFeManual(arg.Data);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function AtualizarInformacoesMapaMDFeManual(retorno) {
    if (retorno.EstadoDestino != "EX" && retorno.EstadoDestino != "EX") {
        _mapaPercursoMDFe.LimparMapa();

        _mapaPercursoMDFe.SetEstadoDestino(retorno.EstadoDestino);

        $.each(retorno.EstadosOrigensDestinos, function (i, estadosMDFe) {
            if (estadosMDFe.Origem != null)
                _mapaPercursoMDFe.AddOrigemDestino(estadosMDFe.Origem.Estado, estadosMDFe.Destino.Estado, estadosMDFe.Codigo);
        });

        $.each(retorno.rotasParaMDFe, function (i, rota) {
            _mapaPercursoMDFe.AddLocalidadesBuscaAPI(rota.Cidade + "," + rota.Estado);
        });

        $.each(retorno.passagens, function (i, estado) {
            _mapaPercursoMDFe.AddEstadoPassagem(estado.Sigla, estado.Posicao);
        });

        _mapaPercursoMDFe.AtualizarDisplayMapa();
    }
}