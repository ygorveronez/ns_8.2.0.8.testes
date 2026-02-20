/// <reference path="CargaPendente.js" />
/// <reference path="ControleCorCarga.js" />
/// <reference path="..\..\Enumeradores\EnumSituacaoCargaJanelaCarregamento.js" />

var DadosGradeTipoOperacao = function () {
}

DadosGradeTipoOperacao.prototype = {
    ObterDadosDisponibilidade: function (grade, dados) {
        var cor = _geradorCor.ObterNovaCor(grade);

        var evento = {
            id: "grade-tipo-operacao-" + grade.Periodo + "-" + grade.TipoOperacao,
            groupId: "grade-tipo-operacao-" + grade.Periodo,
            constraint: "horarioDisponivel",
            className: "grade-tipo-operacao",
            gradeTipoOperacao: grade,
            backgroundColor: cor.backgroundColor,
            borderColor: cor.borderColor,
            start: moment(dados.DataAtual + " " + dados.HoraInicio, "DD/MM/YYYY HH:mm:ss"),
            end: moment(dados.DataAtual + " " + dados.HoraTermino, "DD/MM/YYYY HH:mm:ss").add(dados.ToleranciaExcessoTempo, "minutes"),
            title: grade.Descricao,
            _html: this._obterHtmlDadosDisponibilidade(grade)
        };

        return evento;
    },
    _obterHtmlDadosDisponibilidade: function (grade) {
        var html = '';

        html += '<div class="txt-bold grade-dados">' + grade.Descricao + '<br />' + grade.Total + '</div>';

        return html;
    }
}

var GeradorCorGradeTipoOperacao = function () {
    var _hexParaRGB = function (hex) {
        var result = /^([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
        return result ? {
            r: parseInt(result[1], 16),
            g: parseInt(result[2], 16),
            b: parseInt(result[3], 16)
        } : null;
    }

    var _shadeColor = function (color, percent) {
        var R = parseInt(color.substring(0, 2), 16);
        var G = parseInt(color.substring(2, 4), 16);
        var B = parseInt(color.substring(4, 6), 16);

        R = parseInt(R * (100 + percent) / 100);
        G = parseInt(G * (100 + percent) / 100);
        B = parseInt(B * (100 + percent) / 100);

        R = (R < 255) ? R : 255;
        G = (G < 255) ? G : 255;
        B = (B < 255) ? B : 255;

        var RR = ((R.toString(16).length == 1) ? "0" + R.toString(16) : R.toString(16));
        var GG = ((G.toString(16).length == 1) ? "0" + G.toString(16) : G.toString(16));
        var BB = ((B.toString(16).length == 1) ? "0" + B.toString(16) : B.toString(16));

        return "#" + RR + GG + BB;
    }

    var _obterCorPorTipoOperacao = function (codigo) {
        if (codigo == 0) return _corPadrao;

        if (!(codigo in _mapeamentoTipoOperacao)) {
            var cor = _cores.pop();
            _mapeamentoTipoOperacao[codigo] = cor;
        }

        return _mapeamentoTipoOperacao[codigo];
    }

    var _mapeamentoTipoOperacao = {};

    var _corPadrao = 'a5a5a5';

    var _porcentagemTransparenciaBg = 40;

    var _porcentagemBordaEscura = 40;

    var _cores = [
        "ccc48f", "807b59", "e6e3cf", "bfb360", "a2cc8f", "658059", "d6e6cf", "7dbf60", "ccac8f", "806b59", "e6d9cf", "bf8d60",
        "6b6c91", "5e5f80", "d1d2e6", "6c6ebf", "7a567a", "7f5980", "e5cfe6", "65917c", "59806d", "cfe6db", "60bf92", "735980",
        "cc8f8f", "805959", "e6cfcf", "bf6060", "ccb48f", "807059", "e6dccf", "bf9960", "6e567a",
    ];

    this.ObterNovaCor = function (grade) {
        var cor = _obterCorPorTipoOperacao(grade.TipoOperacao);
        var rgb = _hexParaRGB(cor);

        return {
            backgroundColor: 'rgba(' + rgb.r + ', ' + rgb.g + ', ' + rgb.b + ',.' + _porcentagemTransparenciaBg + ')',
            borderColor: _shadeColor(cor, _porcentagemBordaEscura)
        };
    }
}