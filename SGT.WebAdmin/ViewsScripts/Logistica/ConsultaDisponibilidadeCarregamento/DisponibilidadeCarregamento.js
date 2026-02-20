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

var DisponibilidadeCarregamento = function () {
    var $this = this;

    $this.CallbackDetalhesReserva = function (dados) { };
    $this.CallbackDetalhesEmReserva = function (dados) { };

    $this.Load = function () {
        // Cria a data para FC manipular
        var dataCarregamento = moment();
        if (_dadosPesquisaCarregamento.DataCarregamento != null && _dadosPesquisaCarregamento.DataCarregamento != "" && _dadosPesquisaCarregamento.DataCarregamento != "__/__/____")
            dataCarregamento = moment(_dadosPesquisaCarregamento.DataCarregamento, "DD/MM/YYYY");

        // Array de rota para gerar as semanas
        $this.Rotas = new Array();
        $this.CodigoRota = 0;//_pesquisaConsultaDisponibilidadeCarregamento.Rota.val() != "" ? _pesquisaConsultaDisponibilidadeCarregamento.Rota.codEntity() : 0;

        // Quando uma rota é selecionada
        // Itera todas rotas do centro de carregamento e procura a rota selecionada
        if ($this.CodigoRota > 0) {
            for (var i = 0; i < _centroCarregamentoAtual.Rotas.length; i++) {
                if (_centroCarregamentoAtual.Rotas[i].Codigo == $this.CodigoRota) {
                    $this.Rotas = [_centroCarregamentoAtual.Rotas[i]]; // Cria um array de um elemento só (a rota selecionada)
                    break;
                }
            }
        } else {
            $this.Rotas = _centroCarregamentoAtual.Rotas;
        }

        // Gera o container de cada rota
        $this.Rotas.forEach(function (rota, i) {
            // Renderiza a rota e aplica no DOM
            $("#divContainerDisponibilidadeCarregamento").append(RenderTemplate(GetTemplate("Rota"), rota));

            // Busca a rota específica e gera o calendário
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
                    // Coloca o HTML na tag
                    element.find('.fc-content').html(event._html);

                    // Ativa o tootle tip do header
                    element.find('[data-toggle="tooltip"]').tooltip({
                        container: 'body'
                    });
                },
                events: function (start, end, timezone, callback) {
                    // Busca os eventos (no caso, disponibilidades)
                    var dataInicial = start.format("DD/MM/YYYY");
                    var dataFinal = end.format("DD/MM/YYYY");
                    $this.CarregarCargas(this.options.rota.Codigo, dataInicial, dataFinal, callback);
                },
                rota: rota
            })
            .on("click", "a[data-dia]", function (e) {
                if (e && e.preventDefault) e.preventDefault();

                var dados = {
                    PrevisaoCarregamento: $(this).data("previsao"),
                    Data: $(this).data("dia"),
                    CentroCarregamento: _dadosPesquisaCarregamento.CentroCarregamento
                };
                $this.CallbackDetalhesReserva(dados);
                }).on("click", "a[data-reserva]", function (e) {
                    if (e && e.preventDefault) e.preventDefault();

                    var dados = {
                        Rota: $(this).data("rota"),
                        PrevisaoCarregamento: $(this).data("previsao"),
                        Data: $(this).data("reserva"),
                        CentroCarregamento: _dadosPesquisaCarregamento.CentroCarregamento
                    };

                    $this.CallbackDetalhesEmReserva(dados);
                });;

        }); 
    }

    $this.CarregarCargas = function (codigoRota, dataInicial, dataFinal, callback) {
        // Monta dados de busca
        var dados = {
            DataInicial: dataInicial,
            DataFinal: dataFinal,
            Rota: codigoRota,
            CentroCarregamento: _dadosPesquisaCarregamento.CentroCarregamento
        };

        executarReST("ConsultaDisponibilidadeCarregamento/ObterDisponibilidadeCarregamento", dados, function (r) {
            if (r.Success) {
                // Objeto de retorno
                var previsoes = new Array();

                // Itera cada disponibilidade para montar o conteúdo
                r.Data.forEach(function (disponibilidade, i) {
                    // Objeto de previsão
                    var evento = {
                        title: disponibilidade.Previsoes.length + "Itens",
                        start: moment(disponibilidade.Dia, "DD/MM/YYYY").format("YYYY-MM-DD"),
                        previsoes: disponibilidade.Previsoes,
                        className: "disponibilidades-container",
                        _html: RenderizaTabelaDisponibilidade(disponibilidade.Previsoes)
                    };

                    // Inclui no objeto de retorno
                    previsoes.push(evento);
                });

                // Retorna o objeto
                callback(previsoes);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }

    $this.Destroy = function () {
        if ($this.Rotas != null) {
            for (var i = 0; i < $this.Rotas.length; i++)
                $("#divCalendarioRota_" + $this.Rotas[i].Codigo).off("click").fullCalendar('destroy');
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

function RenderizaTabelaDisponibilidade(obj) {
    // Manipula os objetos antes de serem renderizados
    obj.map(function (o) {
        o.CssSituacao = o.Livres > 0 ? 'success' : 'danger';

        return o;
    });

    // Renderiza o conteudo
    var _body = RenderTemplate(GetTemplate("BodyTabela"), obj);

    // Renderiza a estrutura da tabela
    var _tabela = RenderTemplate(GetTemplate("Tabela"), {
        Body: _body
    });

    // Retorna o conteúdo
    return _tabela;
}

function RenderTemplate(html, data) {
    /* Troca todas as ocorrências interpoladas pelo valor do objeto
     * HTML é uma string do teamplate para recnderizar
     * Data pode ser um objeto ou um array de objeto para renderizar
     *
     * Ex:
     * Template = <div>{{Hello}}</div>
     * Data = {Hello: "World"}
     * Return = <div>World</div>
     *
     * Template = <li>{{Foo}}</li>
     * Data = [{Foo: "Bar"}, {Foo: "Baz"}, {Foo: "Bax"}]
     * Return = <li>Bar</li><li>Baz</li><li>Bax</li>
     */

    // Abertura da tag
    var _iterpolate = "{{";

    // Fechamento da tag
    var iterpolate_ = "}}";

    // Template para retornar
    var returnHtml = "";

    // Função helper
    var _fnRender = function (html, data) {
        // Itera todas props do objeto e troca no tempalte
        for (var i in data)
            html = html.replace(new RegExp(_iterpolate + i + iterpolate_, 'gm'), data[i]);

        return html;
    }

    // Se for um array de dados, itera todos
    if ($.isArray(data)) {
        data.forEach(function (_data) {
            returnHtml += _fnRender(html, _data);
        });
    } else {
        returnHtml = _fnRender(html, data);
    }

    return returnHtml;
}

function GetTemplate(id) {
    /* Retorna o conteudo da tag de template
     * Caso a tag ja foi renderizada pega, ele retorna do objeto
     *
     * O HTML deve estar dentro de uma tag script com type=text/template
     * Ex: <script type="text/template" class="templateExemplo"><span></span></script>
     */

    // Acesso global
    var wtpl = window.templatesCarregados;
    wtpl = wtpl || {};

    // Se nao esta no objto global
    if (!(id in wtpl)) 
        wtpl[id] = $("script.template" + id).html();
    
    return wtpl[id];
}