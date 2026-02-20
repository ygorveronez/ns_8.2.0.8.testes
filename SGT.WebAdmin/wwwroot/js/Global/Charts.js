/// <reference path="CRUD.js" />
/// <reference path="../libs/jquery-2.1.1.js" />
/// <reference path="../plugin/bootstrap-timepicker/bootstrap-timepicker.min.js" />
/// <reference path="../bootstrap/bootstrap.js" />
/// <reference path="knockout-3.1.0.js" />
/// <reference path="Mensagem.js" />
/// <reference path="Rest.js" />
/// <reference path="libs/jquery.globalize.js" />
/// <reference path="libs/jquery.globalize.pt-BR.js" />
/// <reference path="Validacao.js" />
/// <reference path="../app.config.js" />
/// <reference path="../libs/d3.js" />
var ChartType = {
    Bar: 0,
    BarHorizontal: 1,
    Pie: 2,
    Line: 3,
    GroupBar: 4,
    GroupBarHorizontal: 5
};

var ChartPropertyType = {
    string: 0,
    int: 1,
    decimal: 2
}

var ChartOrder = {
    Ascending: 0,
    Descending: 1
};

var Chart = function (options) {
    var $this = this;

    if (d3 == null)
        throw "A biblioteca D3 não está disponível!";

    if (options == null)
        options = {};

    $this.defaults = {
        type: ChartType.Bar,
        idContainer: "",
        properties: {
            x: 'text',
            xType: ChartPropertyType.string,
            xMaxLength: 50,
            y: 'value',
            yType: ChartPropertyType.int,
            yMaxLength: 50,
            color: null,
            order: null,
        },
        xTitle: null,
        yTitle: null,
        title: "Gráfico",
        breakPoint: 600, //para diminuir os dados de x/y na tela e evitar sobreposição
        onInit: function () { },
        afterInit: function () { },
        url: "",
        data: null,
        params: null,
        knockoutParams: null,
        margin: {
            top: 50,
            right: 20,
            left: 70,
            bottom: 50
        },
        pieLabels: {
            inner: {
                format: "value"
            },
            value: {
                fontSize: 25,
                color: "#fff",
            },
            mainLabel: {
                fontSize: 15
            }
        },
        width: 0, //0 = auto
        height: 0, //0 = auto
        fileName: null,
        drillDownSettings: null
    };

    $this.settings = $.extend(true, {}, $this.defaults, options);

    $this.init = function () {
        delete $this.title;
        delete $this.xTitle;
        delete $this.yTitle;
        delete $this.breadcumb;

        $('#' + $this.settings.idContainer).empty();

        window.addEventListener('resize', $this.render);

        $this.svg = d3.select('#' + $this.settings.idContainer).append('svg');

        if ($this.settings.type == ChartType.Bar) {
            $this.x = d3.scaleBand().padding(0.1);
            $this.y = d3.scaleLinear();
        } else {
            $this.x = d3.scaleLinear();
        }

        $this.background = $this.svg.append("rect")
                                   .style("fill", "white")
                                   .on("click", $this.goToParentGraph);

        $this.g = $this.svg.append("g");
        $this.gx = $this.g.append("g").attr("class", "axis axis--x");
        $this.gy = $this.g.append("g").attr("class", "axis axis--y");


        if ($this.settings.type == ChartType.BarHorizontal || $this.settings.type == ChartType.GroupBarHorizontal) {
            $this.gy.style('shape-rendering', 'crispEdges')
                    .append("line")
                    .attr("y1", "100%")
                    .style('fill', 'none')
                    .style('stroke', '#000');
        }

        $this.search();
    }

    $this.destroy = function () {
        $('#' + $this.settings.idContainer).empty();

        $(window).off('resize', $this.render);
    }

    $this.search = function () {
        $this.getData().then(function () {
            $this.render();
        });
    }

    $this.render = function () {
        if (!$('#' + $this.settings.idContainer).is(":visible"))
            return;

        $this.updateDimensions();

        $this.svg.attr('width', $this.width + $this.margin.right + $this.margin.left)
                 .attr('height', $this.height + $this.margin.top + $this.margin.bottom);

        $this.g.attr("transform", "translate(" + $this.margin.left + "," + $this.margin.top + ")");

        if ($this.settings.type == ChartType.Bar) {
            $this.x.rangeRound([0, $this.width]).domain($this.data.map(function (d) { return $this.getPropertyValue(d, 'x', true); }));
            $this.y.rangeRound([$this.height, 0]).domain([0, d3.max($this.data, function (d) { return $this.getPropertyValue(d, 'y', true); })]).nice();
            $this.gx.attr("transform", "translate(0," + $this.height + ")").call(d3.axisBottom($this.x));
            $this.gy.call(d3.axisLeft($this.y));

        }
        else if ($this.settings.type == ChartType.BarHorizontal) {
            var max = d3.max($this.data, function (d) {
                return $this.getPropertyValue(d, 'x', true);
            });
            $this.x.range([0, $this.width]).domain([0, max]).nice();
            $this.gx.call(
                d3.axisTop()
                    .ticks(max > 10 ? 10 : max)
                    .tickFormat(d3.format("d"))
                    .scale($this.x));

        }
        else if ($this.settings.type == ChartType.GroupBarHorizontal) {
            var max = d3.max($this.data, function (d) {
                var val = 0;
                for (var i = 0; i < $this.settings.properties.x.length; i++) {
                    var propVal = d[$this.settings.properties.x[i]["prop"]];
                    if (propVal > val)
                        val = propVal;
                }
                return val;
            });
            $this.x.range([0, $this.width]).domain([0, max]).nice();
            $this.gx.call(
                d3.axisTop()
                    .ticks(max > 10 ? 10 : max)
                    .tickFormat(d3.format("d"))
                    .scale($this.x));

        }
        else if ($this.settings.type == ChartType.Pie) {
            $this.destroy();

            var pie = new d3pie($this.settings.idContainer, {
                header: {
                    title: {
                        text: $this.settings.title
                    }
                },
                data: {
                    content: $this.data
                },
                labels: $this.settings.pieLabels,
                size: {
                    canvasHeight: $this.height,
                    canvasWidth: $this.width
                }
            });
            pie.isOpeningSegment = true;

            $this.pie = pie;
            //$this.pie.svg.attr("transform", "translate(100, 0)");
        }
        
        $this.background.attr("width", $this.width + $this.margin.right + $this.margin.left)
                        .attr("height", $this.height + $this.margin.top + $this.margin.bottom);

        $this.renderGraphTitle();
        $this.renderXTitle();
        $this.renderYTitle();
        $this.renderBreadcumb();
        $this.renderBars();
        $this.renderLegends();
    }

    $this.getData = function () {
        var p = new promise.Promise();

        if ($this.settings.data != null) {
            $this.data = $this.settings.data;
            $this.order();
            p.done();
        } else {
            var requestData = $this.settings.params || RetornarObjetoPesquisa($this.settings.knockoutParams);

            executarReST($this.settings.url, requestData, function (r) {
                if (r.Success) {
                    if (r.Data) {
                        $this.data = r.Data;
                        $this.order();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
                }
                p.done();
            });
        }

        return p;
    }

    $this.renderBars = function () {
        $this.g.selectAll(".bar").remove();
        $this.g.selectAll(".bartext").remove();
        $this.g.selectAll(".bartextend").remove();
        $this.g.selectAll(".group").remove();

        switch ($this.settings.type) {
            case ChartType.Bar:
                $this.renderBarsVertical();
                break;
            case ChartType.BarHorizontal:
                $this.renderBarsHorizontal();
                break;
            case ChartType.GroupBarHorizontal:
                $this.renderGroupBarsHorizontal();
            default:
                break;
        }
    }

    $this.renderBarsVertical = function () {
        $this.g.selectAll(".bar")
                   .data($this.data)
                   .enter()
                   .append("rect")
                   .attr("class", "bar")
                   .attr("x", function (d) { return $this.x($this.getPropertyValue(d, 'x', true)); })
                   .attr("y", function (d) { return $this.y($this.getPropertyValue(d, 'y', true)); })
                   .attr("width", $this.x.bandwidth())
                   .attr("height", function (d) { return $this.height - $this.y($this.getPropertyValue(d, 'y', true)); })
                   .style("cursor", $this.getCursorStyle())
                   .style("fill", function (d) { return d[$this.settings.properties.color] || "#4682b4" })
                   .on("click", function (d, i) { $this.drillDown(d, i); });

        $this.g.selectAll(".bartext")
               .data($this.data)
               .enter()
               .append("text")
               .style("fill", "#000")
               .style("font-weight", "bold")
               .style("font-size", "10px")
               .attr("class", "bartext")
               .attr("text-anchor", "middle")
               .attr("fill", "white")
               .attr("x", function (d, i) {
                   return $this.x($this.getPropertyValue(d, 'x', true)) + $this.x.bandwidth() / 2;
               })
               .attr("y", function (d, i) {
                   return $this.y($this.getPropertyValue(d, 'y', true)) - 2;
               })
               .text(function (d) {
                   return $this.getPropertyValue(d, 'yText', true) || $this.getPropertyValue(d, 'y');
               });
    }

    $this.renderBarsHorizontal = function () {
        $this.g.selectAll(".bar")
               .data($this.data)
               .enter()
               .append("rect")
               .attr("class", "bar")
               .attr("width", function (d) { return $this.x($this.getPropertyValue(d, 'x', true)); })
               .attr("height", 20)
               .attr("transform", function (d, i) { return "translate(0," + ((20 * i * 1.2) + 5) + ")"; })
               .style("cursor", $this.getCursorStyle())
               .style("fill", function (d) { return d[$this.settings.properties.color] || "#4682b4" })
               .on("click", function (d, i) { $this.drillDown(d, i); });

        $this.g.selectAll(".bartext")
               .data($this.data)
               .enter()
               .append("text")
               .attr("class", "bartext")
               .attr("transform", function (d, i) { return "translate(0," + ((20 * i * 1.2) + 5) + ")"; })
               .attr("x", -6)
               .attr("y", 20 / 2)
               .attr("dy", ".35em")
               .style("text-anchor", "end")
               .style("font-weight", "bold")
               .style("font-size", "10px")
               .style("cursor", $this.getCursorStyle())
               .text(function (d) { return $this.getPropertyValue(d, 'y'); })
               .on("click", function (d, i) { $this.drillDown(d, i); })
               .append("title")
               .text(function (d) {
                   return $this.getPropertyValue(d, 'yText', true) || $this.getPropertyValue(d, 'y');
               });

        $this.g.selectAll(".bartextend")
               .data($this.data)
               .enter()
               .append("text")
               .attr("class", "bartextend")
               .attr("transform", function (d, i) { return "translate(0," + ((20 * i * 1.2) + 5) + ")"; })
               .attr("x", function (d) { return $this.x($this.getPropertyValue(d, 'x', true)) + 5; })
               .attr("y", 20 / 2)
               .attr("dy", ".35em")
               .style("text-anchor", "start")
               .style("font-weight", "bold")
               .style("font-size", "10px")
               .style("cursor", $this.getCursorStyle())
               .text(function (d) {
                   return $this.getPropertyValue(d, 'xText', true) || $this.getPropertyValue(d, 'x');
               })
               .on("click", function (d, i) { $this.drillDown(d, i); });
    }

    $this.renderGroupBarsHorizontal = function () {
        var group = $this.g.selectAll(".group")
                           .data($this.data)
                           .enter()
                           .append("g")
                           .attr("class", "group")
                           .attr("transform", function (d, i) { return "translate(0," + ((($this.settings.properties.x.length * 22) * i * 1.2) + 5) + ")"; });

        for (var i = 0; i < $this.settings.properties.x.length; i++) {
            var prop = $this.settings.properties.x[i]["prop"];
            var color = $this.settings.properties.x[i]["color"];

            group.selectAll(".bar." + prop)
                 .data(function (d) { return [d] })
                 .enter()
                 .append("rect")
                 .attr("class", "bar " + prop)
                 .attr("x", 0)
                 .attr("y", i * 22)
                 .attr("width", function (d) { return $this.x(d[prop]) })
                 .attr("height", 20)
                 .style("fill", color)
                 .style("cursor", $this.getCursorStyle())
                 .attr("data-property", prop)
                 .on("click", function (d, i, x) { $this.drillDown(d, i, $(x).attr("data-property")); });

            group.selectAll(".bartextend " + prop)
                   .data(function (d) { return [d] })
                   .enter()
                   .append("text")
                   .attr("class", "bartextend " + prop)
                   .attr("transform", function (d, i) { return "translate(0," + ((22 * i * 1.2) + 5) + ")"; })
                   .attr("x", function (d) { return $this.x(d[prop]) + 5; })
                   .attr("y", (22 * i) + 4)
                   .attr("dy", ".35em")
                   .style("text-anchor", "start")
                   .style("font-weight", "bold")
                   .style("font-size", "10px")
                   .style("cursor", $this.getCursorStyle())
                   .text(function (d) {
                       return d[prop];
                   })
                   .attr("data-property", prop)
                   .on("click", function (d, i, x) { $this.drillDown(d, i, $(x).attr("data-property")); });
        }

        $this.g.selectAll(".bartext")
               .data($this.data)
               .enter()
               .append("text")
               .attr("class", "bartext")
               .attr("transform", function (d, i) { return "translate(0," + ((($this.settings.properties.x.length * 22) * i * 1.2) + 5) + ")"; })
               .attr("x", -6)
               .attr("y", 40 / 2)
               .attr("dy", ".35em")
               .style("text-anchor", "end")
               .style("font-weight", "bold")
               .style("font-size", "10px")
               .style("cursor", $this.getCursorStyle())
               .text(function (d) { return $this.getPropertyValue(d, 'y'); })
               .append("title")
               .text(function (d) {
                   return $this.getPropertyValue(d, 'yText', true) || $this.getPropertyValue(d, 'y');
               });
    }

    $this.renderGraphTitle = function () {
        if ($this.settings.title != null) {
            if ($this.title != null) {
                $this.title.attr("x", (($this.width + $this.margin.left + $this.margin.right) / 2));
            } else {
                $this.title = $this.svg.append("text")
                                   .attr("x", (($this.width + $this.margin.left + $this.margin.right) / 2))
                                   .attr("y", 16)
                                   .attr("text-anchor", "middle")
                                   .style("font-weight", "bold")
                                   .style("font-size", "16px")
                                   .text($this.settings.title);
            }
        }
    }

    $this.renderXTitle = function () {
        if ($this.settings.xTitle != null) {

            var translateX, translateY;

            if ($this.settings.type == ChartType.Bar) {
                translateX = ($this.width + $this.margin.left + $this.margin.right) / 2;
                translateY = $this.height + $this.margin.top + $this.margin.bottom - 10;
            } else if ($this.settings.type == ChartType.BarHorizontal || $this.settings.type == ChartType.GroupBarHorizontal) {
                translateX = ($this.width + $this.margin.left + $this.margin.right) / 2;
                if ($this.hasBreadcumb() === true)
                    translateY = 55;
                else
                    translateY = 40;
            }

            if ($this.xTitle != null) {
                $this.xTitle.attr("transform", "translate(" + translateX + " ," + translateY + ")");
            } else {
                $this.xTitle = $this.svg.append("text")
                                        .attr("transform", "translate(" + translateX + " ," + translateY + ")")
                                        .style("text-anchor", "middle")
                                        .style("font-size", "11px")
                                        .text($this.settings.xTitle);
            }
        }
    }

    $this.renderYTitle = function () {
        if ($this.settings.yTitle != null) {

            var translateX = 0 - (($this.height + $this.margin.top + $this.margin.bottom) / 2);
            var translateY = 5;

            if ($this.yTitle != null) {
                $this.yTitle.attr("x", translateX)
            } else {
                $this.yTitle = $this.svg.append("text")
                                        .attr("transform", "rotate(-90)")
                                        .attr("y", translateY)
                                        .attr("x", translateX)
                                        .attr("dy", "1em")
                                        .style("text-anchor", "middle")
                                        .style("font-size", "11px")
                                        .text($this.settings.yTitle);
            }
        }
    }

    $this.renderBreadcumb = function () {
        if ($this.settings.breadcumbTitle != null) {
            var breadcumbTitle = $this.getBreadcumbText($this.settings.breadcumbTitle);
            var parentGraph = $this.settings.parentGraph;

            while (parentGraph != null) {
                var parentBreadcumbTitle = $this.getBreadcumbText(parentGraph.settings.breadcumbTitle);
                breadcumbTitle = parentBreadcumbTitle + " >> " + breadcumbTitle;
                parentGraph = parentGraph.settings.parentGraph;
            }

            var translateX = 10;
            var translateY = 40;

            if ($this.breadcumb != null) {
                $this.breadcumb.attr("transform", "translate(" + translateX + " ," + translateY + ")");
            } else {
                $this.breadcumb = $this.svg.append("text")
                                           .attr("transform", "translate(" + translateX + " ," + translateY + ")")
                                           .style("font-size", "11px")
                                           .text(breadcumbTitle);
            }
        }
    }

    $this.renderLegends = function () {
        if ($this.settings.type == ChartType.GroupBarHorizontal) {
            $this.legend = $this.svg.selectAll(".legend")
                                    .data($this.settings.properties.x)
                                    .enter()
                                    .append('g')
                                    .attr('class', 'legend')
                                    .attr('transform', function (d, i) { return 'translate(' + 10 + ',' + ($this.height + $this.margin.top + $this.margin.bottom - ((i + 1) * 15)) + ')'; });


            $this.legend.append('rect')
                        .attr('width', 12)
                        .attr('height', 12)
                        .style('fill', function (d, i) { return d.color })
                        .style('stroke', function (d, i) { return d.color });

            $this.legend.append('text')
                        .attr('x', 15)
                        .attr('y', 6)
                        .text(function (d) { return d.text; })
                        .attr("dy", ".35em")
                        .style("font-weight", "bold")
                        .style("font-size", "10px");
        }
    }

    $this.getBreadcumbText = function (breadcumbTitle) {
        if (typeof breadcumbTitle === "function")
            return breadcumbTitle($this.settings.drillDownObject);
        else
            return breadcumbTitle;
    }

    $this.hasBreadcumb = function () {
        if ($this.settings.breadcumbTitle != null)
            return true;

        return false;
    }

    $this.updateDimensions = function () {
        $this.containerWidth = document.getElementById($this.settings.idContainer).clientWidth;
        $this.margin = $.extend({}, $this.settings.margin);

        if ($this.settings.width > 0)
            $this.width = $this.settings.width;
        else
            $this.width = $this.containerWidth - $this.margin.right - $this.margin.left;

        if ($this.settings.type == ChartType.BarHorizontal) {
            $this.height = $this.data.length * 25 + $this.margin.top + $this.margin.bottom;
        } else if ($this.settings.type == ChartType.GroupBarHorizontal) {
            $this.height = $this.data.length * ($this.settings.properties.x.length * 26) + $this.margin.top + $this.margin.bottom;
        } else {
            if ($this.settings.height > 0)
                $this.height = $this.settings.height;
            else
                $this.height = .5 * $this.width;
        }

        if ($this.hasBreadcumb() === true) {
            $this.height += 15;
            $this.margin.top += 15;
        }
    }

    $this.getPropertyValue = function (obj, xy, rawValue) {
        var value = "";

        if (typeof $this.settings.properties[xy] === "function")
            value = $this.settings.properties[xy](obj);
        else
            value = obj[$this.settings.properties[xy]];

        if (rawValue === true)
            return value;

        switch ($this.settings.properties[xy + "Type"]) {
            case ChartPropertyType.decimal:
                var decimalPlaces = $this.settings.properties[xy + "DecimalPlaces"] || "2";
                return Globalize.format(value, "n" + decimalPlaces);
            case ChartPropertyType.int:
                return Globalize.format(value, "n0");
            case ChartPropertyType.string:
                var maxLength = $this.settings.properties[xy + "MaxLength"];
                return value != null ? value.substring(0, (value.length > maxLength ? maxLength : value.length)) : "";
            default:
                return value;
        }
    }

    $this.getCursorStyle = function () {
        return ($this.settings.drillDownSettings != null) || ($this.settings.click != null) ? "pointer" : "default"  
    }

    $this.drillDown = function (d, i, prop) {
        if ($this.settings.drillDownSettings != null) {
            if (!$this.setDrillDownSettings(d, prop))
                return;

            $this.childGraph = new Chart($this.drillDownSettings)

            $this.childGraph.init();
        }

        if ($this.settings.click != null) {
            $this.settings.click(d, i, prop);
        }
    }

    $this.setDrillDownSettings = function (obj, i, prop) {
        $this.drillDownSettings = $this.getDrillDownSettings(obj);

        if ($this.drillDownSettings == null)
            return false;

        if ($this.drillDownSettings.idContainer == null)
            $this.drillDownSettings.idContainer = $this.settings.idContainer;

        if ($this.drillDownSettings.type == null)
            $this.drillDownSettings.type = $this.settings.type;

        if ($this.drillDownSettings.margin == null)
            $this.drillDownSettings.margin = $this.settings.margin;

        if ($this.drillDownSettings.title == null)
            $this.drillDownSettings.title = $this.settings.title;

        if ($this.drillDownSettings.fileName == null)
            $this.drillDownSettings.fileName = $this.settings.fileName;

        $this.drillDownSettings.params = $this.getDrillDownParams(obj);

        if (prop != null)
            $this.drillDownSettings.params["Property"] = prop;

        $this.drillDownSettings.parentGraph = $this;
        $this.drillDownSettings.drillDownObject = obj;

        return true;
    }

    $this.getDrillDownSettings = function (obj) {
        if (typeof $this.settings.drillDownSettings === "function")
            return $this.settings.drillDownSettings(obj);
        else
            return $this.settings.drillDownSettings;
    }

    $this.getDrillDownParams = function (obj) {

        var params = $.extend({}, ($this.settings.params || RetornarObjetoPesquisa($this.settings.knockoutParams)));

        if ($this.drillDownSettings.drillDownParams != null && $this.drillDownSettings.drillDownParams.length > 0) {
            for (var i = 0; i < $this.drillDownSettings.drillDownParams.length; i++)
                params[$this.drillDownSettings.drillDownParams[i]['as']] = obj[$this.drillDownSettings.drillDownParams[i]['property']];
        }

        return params;
    }

    $this.goToParentGraph = function () {
        if ($this.settings.parentGraph != null) {
            $this.settings.parentGraph.init();
        }
    }

    $this.order = function () {
        if ($this.data != null && $this.settings.properties.order != null) {
            var direction = ChartOrder.Ascending;
            var property = "";

            if (typeof $this.settings.properties.order == "object") {
                property = $this.settings.properties.order["prop"];
                direction = $this.settings.properties.order["dir"];
            } else {
                property = $this.settings.properties.order;
            }

            $this.data.sort(function (a, b) {
                var aVal = a[property];
                var bVal = b[property];

                if (aVal > bVal)
                    return (direction == ChartOrder.Ascending ? 1 : -1);
                else if (aVal < bVal)
                    return (direction == ChartOrder.Ascending ? -1 : 1);
                else
                    return 0;
            });
        }
    }

    $this.download = function () {
        $(".downloadGraficoSGT").remove();

        var canvas = document.createElement("canvas");
        canvas.width = $this.width + $this.margin.right + $this.margin.left;
        canvas.height = $this.height + $this.margin.top + $this.margin.bottom;

        var svg = $("#" + $this.settings.idContainer).html().replace(/>\s+/g, ">").replace(/\s+</g, "<").replace(" xlink=", " xmlns:xlink=").replace(/\shref=/g, " xlink:href=");

        canvg(canvas, svg);

        var imgURL = canvas.toDataURL("image/png");

        var image = new Image();
        image.onload = function () {
            var filename = ($this.settings.fileName || "Gráfico") + ".png";
            var a = document.createElement("a");
            a.classList.add("downloadGraficoSGT")
            a.download = filename;
            a.style.display = "none";
            a.href = imgURL.replace("data:image/png;", "data:application/octet-stream;headers=Content-Disposition%3A%20attachment%3B%20filename=" + filename + ";");
            $("#" + $this.settings.idContainer).append(a);
            a.click();

        };
        image.src = imgURL;
    }

    $this.updateOptions = function (options) {
        $this.settings = $.extend(true, {}, $this.settings, options);
    };

    return $this;
}