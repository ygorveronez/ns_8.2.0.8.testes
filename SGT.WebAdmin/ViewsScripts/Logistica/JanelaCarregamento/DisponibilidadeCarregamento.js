/// <reference path="CargaPendente.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Filial.js" />

var DisponibilidadeCarregamento = function () {
    var $this = this;

    $this.Load = function () {

        var dataCarregamento = moment();
        if (_dadosPesquisaCarregamento.DataCarregamento != null && _dadosPesquisaCarregamento.DataCarregamento != "")
            dataCarregamento = moment(_dadosPesquisaCarregamento.DataCarregamento, "DD/MM/YYYY");

        $this.Rotas = new Array();
        $this.CodigoRota = _pesquisaJanelaCarregamento.Rota.val() != "" ? _pesquisaJanelaCarregamento.Rota.codEntity() : 0;

        if ($this.CodigoRota > 0) {
            for (var i = 0; i < _centroCarregamentoAtual.Rotas.length; i++) {
                if (_centroCarregamentoAtual.Rotas[i].Codigo == $this.CodigoRota) {
                    $this.Rotas = [_centroCarregamentoAtual.Rotas[i]];
                    break;
                }
            }
        } else {
            $this.Rotas = _centroCarregamentoAtual.Rotas;
        }

        for (var i = 0; i < $this.Rotas.length; i++) {
            var rota = $this.Rotas[i];
            var html = '<div class="col col-xs-12 col-sm-12 col-md-12 col-lg-12 padding-bottom-10" style="padding: 0;" id="divRota_#idDivRota"><div class="well"><header><h2>#tituloRota</h2></header><fieldset style="padding-top: 10px;"><div class="row"><div class="col col-xs-12 col-sm-12 col-md-12 col-lg-12" id="divCalendarioRota_#idDivRota"></div></div></fieldset></div></div>';
            $("#divContainerDisponibilidadeCarregamento").append(html.replace(/#idDivRota/g, rota.Codigo).replace(/#tituloRota/g, rota.Descricao));

            $("#divCalendarioRota_" + rota.Codigo).fullCalendar({
                header: {
                    left: 'title',
                    center: '',
                    right: 'prev,next'
                },
                defaultView: 'basicWeek',
                lang: 'pt-br',
                height: 300,
                defaultDate: dataCarregamento,
                columnFormat: 'ddd DD/MM',
                titleFormat: 'DD [de] MMMM [de] YYYY',
                eventRender: function (event, element) {
                    element.find('span.fc-title').html(element.find('span.fc-title').text());
                    element.css('padding', '5px 5px 5px 5px');
                    element.find('div.fc-content').css('white-space', 'normal');
                },
                events: function (start, end, timezone, callback) {
                    var dataInicial = start.format("DD/MM/YYYY");
                    var dataFinal = end.format("DD/MM/YYYY");
                    $this.CarregarCargas(this.options.rota.Codigo, dataInicial, dataFinal, callback);
                },
                rota: rota
            });
        }
    }

    $this.CarregarCargas = function (codigoRota, dataInicial, dataFinal, callback) {
        var dados = {
            DataInicial: dataInicial,
            DataFinal: dataFinal,
            Rota: codigoRota,
            CentroCarregamento: _dadosPesquisaCarregamento.CentroCarregamento
        };

        executarReST("JanelaCarregamento/ObterDisponibilidadeCarregamento", dados, function (r) {
            if (r.Success) {

                var previsoes = new Array();

                for (var i = 0; i < r.Data.length; i++) {
                    var disponibilidade = r.Data[i];

                    for (var x = 0; x < disponibilidade.Previsoes.length; x++) {
                        var previsao = disponibilidade.Previsoes[x];

                        var evento = {
                            title: "<b>(" + previsao.QuantidadeCargasUtilizadas + " / " + previsao.QuantidadeCargas + "(" + previsao.QuantidadeCargasExcedentes + "))</b> " + previsao.Descricao,
                            start: moment(disponibilidade.Dia, "DD/MM/YYYY").format("YYYY-MM-DD"),
                            previsao: previsao
                        };

                        if (previsao.QuantidadeCargasUtilizadas < previsao.QuantidadeCargas)
                            evento.className = "well-green";
                        else if (previsao.QuantidadeCargasUtilizadas < (previsao.QuantidadeCargas + previsao.QuantidadeCargasExcedentes))
                            evento.className = "well-orange";
                        else if (previsao.QuantidadeCargasUtilizadas >= (previsao.QuantidadeCargasExcedentes + previsao.QuantidadeCargas))
                            evento.className = "well-red";

                        previsoes.push(evento);
                    }
                }

                callback(previsoes);

            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }

    $this.Destroy = function () {
        if ($this.Rotas != null) {
            for (var i = 0; i < $this.Rotas.length; i++)
                $("#divCalendarioRota_" + $this.Rotas[i].Codigo).fullCalendar('destroy');
        }

        $("#divContainerDisponibilidadeCarregamento").html("");
    }

    $this.Render = function () {
        if ($this.Rotas != null) {
            for (var i = 0; i < $this.Rotas.length; i++)
                $("#divCalendarioRota_" + $this.Rotas[i].Codigo).fullCalendar('render');
        }
    }
}