
var _mapaMonitoramentoMapa;
var _cargas;
var _markers;
var _mapaGoogle;
var _cargaSelecionada;
var _markerArrayAlert;
var backup_cargasNoMapa = new Array();


function loadMapaMonitoramento() {
    if (_mapaMonitoramentoMapa != null && _markers != null) {
        _markers.clearLayers();

        if (_mapaGoogle != null)
            _mapaGoogle.clear();
    }

    if (!_mapaMonitoramentoMapa) {
        _mapaGoogle = new Mapa("tmp", null, null, null);

        const esriSat = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
            attribution: 'Tiles © Esri, Maxar, Earthstar Geographics, USDA, USGS, AeroGRID, IGN, and GIS User Community',
            maxNativeZoom: 17,
            maxZoom: 19,
        });

        const esriLabels = L.tileLayer('https://services.arcgisonline.com/ArcGIS/rest/services/Reference/World_Boundaries_and_Places/MapServer/tile/{z}/{y}/{x}', {
            attribution: 'Labels © Esri',
            maxNativeZoom: 17,
            maxZoom: 19,
        });

        const esriComLabels = L.layerGroup([esriSat, esriLabels]);

        let openStreetMap = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        })
            
        _mapaMonitoramentoMapa = L.map('divMapaMonitoramento', {zoomControl: false, wheelPxPerZoomLevel: 90, layers: openStreetMap, minZoom: 2, maxZoom: 19 }).setView([-12.30521, -51.17696], 4);

        L.control.layers({ "Ruas": openStreetMap, "Satélite": esriComLabels }, null, { position: 'topleft' }).addTo(_mapaMonitoramentoMapa);

        const quantidadeZoom = L.control({ position: 'bottomleft' });
        quantidadeZoom.onAdd = function () {
            const div = L.DomUtil.create('div', 'zoom-overlay');
            div.style.background = 'rgba(0,0,0,0.6)';
            div.style.color = 'white';
            div.style.padding = '4px 8px';
            div.style.fontSize = '12px';
            div.style.borderRadius = '4px';
            div.innerHTML = 'Zoom: ' + _mapaMonitoramentoMapa.getZoom() + '/19';
            this._div = div;
            return div;
        };
        quantidadeZoom.addTo(_mapaMonitoramentoMapa);

        _mapaMonitoramentoMapa.on('zoomend', function () {
            const zoomAtual = _mapaMonitoramentoMapa.getZoom();
            quantidadeZoom._div.innerHTML = 'Zoom: ' + zoomAtual + '/19';
        });
    }
}

function toggleDestinos(buttonId, destinosId, destinosRestantes) {
    const verMaisBtn = document.getElementById('ver-mais');
    const verMenosBtn = document.getElementById('ver-menos');
    const destinosDiv = document.getElementById(destinosId);

    if (buttonId === 'ver-mais') {
        destinosDiv.style.display = 'block';
        verMaisBtn.style.display = 'none';
        verMenosBtn.style.display = 'inline';
    } else if (buttonId === 'ver-menos') {
        destinosDiv.style.display = 'none';
        verMaisBtn.style.display = 'inline';
        verMenosBtn.style.display = 'none';
    }
}

var MapaMonitoramento = function (mapa, mapaGoogle) {
    var self = this;
    this._mapa = mapa;
    this._cargas;
    this._locaisRaioProximidade;
    this._cargasNoMapa;
    this._veiculosEmRaioProximidade;
    this._markers;
    this._markersLocalRaiosProximidade;
    this._circles;
    this._mapaGoogle = mapaGoogle;
    this._markerVeiculo;
    this._markersEntregas;
    this._markerArrayAlert;
    this._cargaSelecionada;
    this._polilinhaPlanejada;
    this._polilinhaRealizada;
    this._corAnteriorSelecionada;
    this._cargaCritica;
    this._informacoesTempoPermanenciaStatusCarga;
    this._filtrosMapaAplicados = new Array();
    this.signalR = false;
}

MapaMonitoramento.prototype = {
    Load: function (dadosGrid) {
        var self = this;
        if (self._mapa == null)
            return;

        self._cargas = dadosGrid;
        self._cargasNoMapa = new Array();
        self._veiculosEmRaioProximidade = new Array();
        self._markerArrayAlert = new Array();
        self._markersEntregas = ko.observableArray();
        self._markers = L.markerClusterGroup();
        self._markerVeiculo = null;
        self._cargaSelecionada = null;

        self.LoadCargasNoMapa(self);

        self._mapa.addLayer(self._markers);

        try {
            if (self._markers)
                self._mapa.fitBounds(self._markers.getBounds());
        } catch (e) { }

        self._mapa.on('zoomend', function () {
            self.OpenAllPopups(self);
        });

        setTimeout(function () {
            $(".legenda-mapaCards").hide();
            self.LoadLegendas(self);
            self.RecarregarFiltrosAplicados(self);
            self.OpenAllPopups(self);
        }, 500);
    },
    LoadLocaisRaioProximidade: function (dados) {
        var self = this;
        if (self._mapa == null)
            return;

        self._locaisRaioProximidade = dados;
        self._markersLocalRaiosProximidade = L.markerClusterGroup();
        self._circles = new Array();

        self.LoadLocaisNoMapa(self);

        self._mapa.addLayer(self._markersLocalRaiosProximidade);
    },
    LoadCargasNoMapa: function (self) {
        for (var i = 0; i < self._cargas.length; i++)
            self.AdicionarMarker(self, self._cargas[i]);
    },
    LoadLocaisNoMapa: function (self) {
        for (var i = 0; i < self._locaisRaioProximidade.Locais?.length; i++) {
            self.AdicionarMarkerLocal(self, self._locaisRaioProximidade.Locais[i]);
            self.AdicionarRaioProximidade(self, self._locaisRaioProximidade.Locais[i]);
        }
    },
    LimparCargasNoMapa: function (self) {
        if (self._markers)
            self._mapa.removeLayer(self._markers);
    },
    LoadLegendas: function (self) {
        $("#legendaStatusViagem").html("");
        $("#legendaSituacaoCarga").html("");
        $("#legendaAlertas").html("");
        $("#legendaStatusAlertas").html("");
        $("#legendaVeiculosEmLocaisRaioProximidade").html("");
        $("#legendaTendenciaEntrega").html("");
        $("#legenda-StatusViagem").hide();
        $("#legenda-SituacaoCarga").hide();
        $("#legenda-Alertas").hide();
        $("#legenda-StatusAlertas").hide();
        $("#legenda-Tendencia-Entrega").hide();
        $("#titulo-legenda-StatusViagem").html("");
        $("#titulo-legenda-SituacaoCarga").html("");
        $("#titulo-legenda-Alertas").html("");
        $("#titulo-legenda-StatusAlertas").html("");
        $("#titulo-legenda-Veiculos-em-Locais-Raio-Proximidade").html("");
        $("#titulo-legenda-Tendencia-Entrega").html("");

        listaCargas = self._cargasNoMapa;
        if (backup_cargasNoMapa)
            listaCargas = backup_cargasNoMapa;

        var listaLegenda = [];
        var listaStatusViagem = [];
        var filtrarPorGrupoStatusViagem = _configuracaoLegendaUsuarioMonitoramentoMapa && _configuracaoLegendaUsuarioMonitoramentoMapa.ExibirPorGrupoStatusViagem.val();
        if (filtrarPorGrupoStatusViagem)
            listaStatusViagem = Object.groupBy(listaCargas, (x) => x.GrupoStatusViagemCodigo);
        else
            listaStatusViagem = Object.groupBy(listaCargas, (x) => x.TiporRegraViagem);

        var listaSituacaoCarga = Object.groupBy(listaCargas, (x) => x.SituacaoCarga);
        var listaAlertas = Object.groupBy(listaCargas?.filter(x => x.StatusUltimoAlertaMonitoramento != 1), (x) => x.TipoUltimoAlertaMonitoramento);
        var listaStatusAlerta = Object.groupBy(listaCargas.filter(x => x.CodigoUltimoAlerta != "0" && x.StatusUltimoAlertaMonitoramento != 1), (x) => x.StatusUltimoAlertaMonitoramento);
        var listaVeiculosEmLocaisRaiosProximidade = Object.groupBy(listaCargas.filter(x => x.CodigoLocalRaiosProximidade !== 0), (x) => x.CodigoLocalRaiosProximidade);
        var listaTendenciaEntrega = Object.groupBy(listaCargas.filter(x => x.TendenciaProximaParada !== 0), (x) => x.TendenciaProximaParada);

        if (listaStatusViagem) {
            listaLegenda = [];
            var keyStatus = Object.keys(listaStatusViagem);
            for (var i = 0; i < keyStatus.length; i++) {
                var chave = keyStatus[i];
                if (filtrarPorGrupoStatusViagem) {
                    listaLegenda.push({
                        Enum: chave,
                        Descricao: listaStatusViagem[chave][0].GrupoStatusViagemDescricao,
                        Cor: listaStatusViagem[chave][0].GrupoStatusViagemCor,
                        Quantidade: listaStatusViagem[chave].length
                    });
                } else {
                    listaLegenda.push({
                        Enum: chave,
                        Descricao: listaStatusViagem[chave][0].StatusViagem,
                        Cor: listaStatusViagem[chave][0].CorStatusViagem,
                        Quantidade: listaStatusViagem[chave].length
                    });
                }
            };

            if (listaLegenda.length === 0) {
                $('#titulo-legenda-StatusViagem').append('Sem dados');
            }
            else {
                for (var i = 0; i < listaLegenda.length; i++) {
                    self.InserirLegendaMapa("legendaStatusViagem", listaLegenda[i].Enum, listaLegenda[i].Descricao, listaLegenda[i].Cor, listaLegenda[i].Quantidade);
                    var id = "legendaStatusViagemCheckBox" + listaLegenda[i].Enum;
                    $("#" + id).on("click", function (element) { event_FiltroMapa_Click("StatusViagem", element) });
                    if (existeNosFiltros(id))
                        $("#" + id)[0].checked = true;
                }
            }

            if (_configuracaoLegendaUsuarioMonitoramentoMapa && _configuracaoLegendaUsuarioMonitoramentoMapa.StatusViagem.val())
                $("#legenda-StatusViagem").show();
        }

        if (listaSituacaoCarga) {
            listaLegenda = [];
            var keyStatus = Object.keys(listaSituacaoCarga);
            for (var i = 0; i < keyStatus.length; i++) {
                var chave = keyStatus[i];
                listaLegenda.push({
                    Enum: chave,
                    Descricao: listaSituacaoCarga[chave][0].DescricaoSituacaoCarga,
                    Cor: listaSituacaoCarga[chave][0].CorSituacaoCarga,
                    Quantidade: listaSituacaoCarga[chave].length
                });
            };

            if (listaLegenda.length === 0) {
                $('#titulo-legenda-SituacaoCarga').html('Sem dados');
            }
            else {
                for (var i = 0; i < listaLegenda.length; i++) {
                    self.InserirLegendaMapa("legendaSituacaoCarga", listaLegenda[i].Enum, listaLegenda[i].Descricao, listaLegenda[i].Cor, listaLegenda[i].Quantidade);
                    var id = "legendaSituacaoCargaCheckBox" + listaLegenda[i].Enum;
                    $("#" + id).on("click", function (element) { event_FiltroMapa_Click("SituacaoDaCarga", element) });
                    if (existeNosFiltros(id))
                        $("#" + id)[0].checked = true;
                }
            }

            if (_configuracaoLegendaUsuarioMonitoramentoMapa && _configuracaoLegendaUsuarioMonitoramentoMapa.SituacaoCarga.val())
                $("#legenda-SituacaoCarga").show();

        }

        if (listaAlertas) {
            listaLegenda = [];
            var keyStatus = Object.keys(listaAlertas);
            for (var i = 0; i < keyStatus.length; i++) {
                var chave = keyStatus[i];
                if (chave == 0)//cargas sem alerta
                    continue;
                listaLegenda.push({
                    Enum: chave,
                    Descricao: listaAlertas[chave][0].DescricaoUltimoAlertaMonitoramento,
                    Cor: listaAlertas[chave][0].CorUltimoAlertaMonitoramento,
                    Quantidade: listaAlertas[chave].length
                });
            };

            if (listaLegenda.length === 0) {
                $('#titulo-legenda-Alertas').html('Sem dados');
            }
            else {
                for (var i = 0; i < listaLegenda.length; i++) {
                    self.InserirLegendaMapa("legendaAlertas", listaLegenda[i].Enum, listaLegenda[i].Descricao, listaLegenda[i].Cor, listaLegenda[i].Quantidade);
                    var id = "legendaAlertasCheckBox" + listaLegenda[i].Enum;
                    $("#" + id).on("click", function (element) { event_FiltroMapa_Click("Alerta", element) });
                    if (existeNosFiltros(id))
                        $("#" + id)[0].checked = true;
                }
            }

            if (_configuracaoLegendaUsuarioMonitoramentoMapa && _configuracaoLegendaUsuarioMonitoramentoMapa.Alertas.val())
                $("#legenda-Alertas").show();
        }

        if (listaVeiculosEmLocaisRaiosProximidade) {
            self._veiculosEmRaioProximidade = listaVeiculosEmLocaisRaiosProximidade;

            listaLegenda = [];
            var keyStatus = Object.keys(listaVeiculosEmLocaisRaiosProximidade);
            for (var i = 0; i < keyStatus.length; i++) {
                var chave = keyStatus[i];
                listaLegenda.push({
                    Enum: chave,
                    Descricao: listaVeiculosEmLocaisRaiosProximidade[chave][0].DescricaoLocalRaiosProximidade,
                    Cor: listaVeiculosEmLocaisRaiosProximidade[chave][0].CorLocalRaiosProximidade,
                    Quantidade: listaVeiculosEmLocaisRaiosProximidade[chave].length
                });
            };

            if (listaLegenda.length === 0) {
                $('#titulo-legenda-Veiculos-em-Locais-Raio-Proximidade').html('Sem dados');
            }
            else {
                for (var i = 0; i < listaLegenda.length; i++) {
                    self.InserirLegendaMapa("legendaVeiculosEmLocaisRaioProximidade", listaLegenda[i].Enum, listaLegenda[i].Descricao, listaLegenda[i].Cor, listaLegenda[i].Quantidade);
                    var id = "legendaVeiculosEmLocaisRaioProximidadeCheckBox" + listaLegenda[i].Enum;
                    $("#" + id).on("click", function (element) { event_FiltroMapa_Click("LocalRaiosProximidade", element) });
                    if (existeNosFiltros(id))
                        $("#" + id)[0].checked = true;
                }
            }
        }

        if (listaStatusAlerta) {
            listaLegenda = [];
            var keyStatus = Object.keys(listaStatusAlerta);
            for (var i = 0; i < keyStatus.length; i++) {
                var chave = parseInt(keyStatus[i]);
                listaLegenda.push({
                    Enum: chave,
                    Descricao: listaStatusAlerta[chave][0].DescricaoStatusUltimoAlertaMonitoramento,
                    Cor: listaStatusAlerta[chave][0].CorStatusUltimoAlertaMonitoramento,
                    Quantidade: listaStatusAlerta[chave].length
                });
            };

            if (listaLegenda.length === 0) {
                $('#titulo-legenda-StatusAlertas').html('Sem dados');
            }
            else {
                for (var i = 0; i < listaLegenda.length; i++) {
                    self.InserirLegendaMapa("legendaStatusAlertas", listaLegenda[i].Enum, listaLegenda[i].Descricao, listaLegenda[i].Cor, listaLegenda[i].Quantidade);
                    var id = "legendaStatusAlertasCheckBox" + listaLegenda[i].Enum;
                    $("#" + id).on("click", function (element) { event_FiltroMapa_Click("StatusAlertas", element) });
                    if (existeNosFiltros(id))
                        $("#" + id)[0].checked = true;
                }
            }

            if (_configuracaoLegendaUsuarioMonitoramentoMapa && _configuracaoLegendaUsuarioMonitoramentoMapa.StatusAlertas.val())
                $("#legenda-StatusAlertas").show();
        }

        if (listaTendenciaEntrega) {
            listaLegenda = [];
            var keyStatus = Object.keys(listaTendenciaEntrega);
            for (var i = 0; i < keyStatus.length; i++) {
                var chave = keyStatus[i];
                listaLegenda.push({
                    Enum: chave,
                    Descricao: listaTendenciaEntrega[chave][0].TendenciaProximaParadaDescricao,
                    Cor: listaTendenciaEntrega[chave][0].CorTendenciaEntrega,
                    Quantidade: listaTendenciaEntrega[chave].length
                });
            };

            if (listaLegenda.length === 0) {
                $('#titulo-legenda-Tendencia-Entrega').html('Sem dados');
            }
            else {
                for (var i = 0; i < listaLegenda.length; i++) {
                    self.InserirLegendaMapa("legendaTendenciaEntrega", listaLegenda[i].Enum, listaLegenda[i].Descricao, listaLegenda[i].Cor, listaLegenda[i].Quantidade);
                    var id = "legendaTendenciaEntregaCheckBox" + listaLegenda[i].Enum;
                    $("#" + id).on("click", function (element) { event_FiltroMapa_Click("TendenciaProximaParada", element) });
                    if (existeNosFiltros(id))
                        $("#" + id)[0].checked = true;
                }

            }

            if (_configuracaoLegendaUsuarioMonitoramentoMapa && _configuracaoLegendaUsuarioMonitoramentoMapa.TendenciaProximaEntrega.val()) {
                $("#legenda-Tendencia-Entrega").show();

            }
        }

        $("#buttonToggleLegendas").show();
    },
    getCenter: function () {
        return this._mapa ? this._mapa.getCenter() : null;
    },

    getZoom: function () {
        return this._mapa ? this._mapa.getZoom() : 0;
    },

    getBounds: function () {
        return this._mapa ? this._mapa.getBounds() : null;
    },

    setView: function (latlng, zoom) {
        if (this._mapa) {
            this._mapa.setView(latlng, zoom);
        }
    },
    setZoom: function (zoom) {
        if (this._mapa && typeof this._mapa.setZoom === 'function') {
            this._mapa.setZoom(zoom);
        }
    },
    fitBounds: function (bounds) {
        if (this._mapa) {
            this._mapa.fitBounds(bounds);
        }
    },
    AdicionarMarker: function (self, carga) {
        if (!this.ValidarPosicao(carga.Latitude, carga.Longitude))
            return;

        var conteudo = this.CarregarContent(carga);
        var latitude = carga.Latitude;
        var longitude = carga.Longitude;
        if (typeof latitude === 'string') {
            latitude = parseFloat(latitude.replace(",", "."));
            longitude = parseFloat(longitude.replace(",", "."));
        }

        let informacoesTempoPermanenciaStatusCarga = {
            TiporRegraViagem: carga.TiporRegraViagem,
            TempoPermanenciaStatus: carga.TempoStatusEmMinutos,
            TempoPermitidoPermanenciaEmCarregamento: carga.TempoPermitidoPermanenciaEmCarregamento,
            TempoPermitidoPermanenciaNoCliente: carga.TempoPermitidoPermanenciaEmCarregamento
        };

        self._mapa.eachLayer(function (layer) {
            if (layer instanceof L.Popup && layer.isPrincipal) {
                self._mapa.removeLayer(layer);
            }
        });

        let marker = L.marker([latitude, longitude], {
            icon: new self.IconeMarkerCarga(false, carga.Cor, carga.Critico, informacoesTempoPermanenciaStatusCarga)
        });

        let popupPrincipal = L.popup({
            offset: [12, 40],
            maxWidth: 'auto',
            autoClose: false,
            closeOnClick: false
        }).setContent(conteudo);
        popupPrincipal.isPrincipal = true;
        marker.bindPopup(popupPrincipal);

        if (carga.Critico)
            marker.bindTooltip('Carga Crítica', { direction: 'bottom', offset: [18, 50] });

        let popupAlerta = null;

        if (carga.IconeUltimoAlertaExibirTela && carga.CodigoUltimoAlerta > 0) {
            let popupContent = $('<img title="Carga: ' + carga.CargaEmbarcador + ' - ' + carga.DescricaoUltimoAlertaMonitoramento +
                '" src="' + carga.IconeUltimoAlertaExibirTela + '" style="width:30px; cursor: pointer;" >');
            popupContent.on('click', function () {
                alertaCardMapClickMonitoramento(carga);
            });

            popupAlerta = L.popup({
                offset: [16, 35],
                maxWidth: 30,
                minWidth: 30,
                maxHeight: 30,
                autoPan: false,
                closeButton: true,
                closeOnClick: false,
                autoClose: false
            }).setContent(popupContent[0]);

            popupAlerta.isAlert = true;

            marker.bindPopup(popupAlerta);

            marker.on('add', function () {
                marker.openPopup();

                setTimeout(() => {
                    let el = $(popupAlerta.getElement());
                    el.css("z-index", 500);
                }, 100);
            });

            marker.on('remove', function () {
                if (self._mapa.hasLayer(popupAlerta)) {
                    self._mapa.closePopup(popupAlerta);
                }
            });

        }

        marker.on('mouseover', function () {
            self._mapa.eachLayer(function (layer) {
                if (layer instanceof L.Popup) {
                    const el = layer.getElement();
                    if (el) {
                        $(el).css("z-index", 800);
                    }

                    if (layer.isPrincipal && layer !== popupPrincipal) {
                        self._mapa.removeLayer(layer);
                    }
                }
            });

            if (self._mapa.hasLayer(popupPrincipal)) {
                if (popupPrincipal.getElement()) {
                    $(popupPrincipal.getElement()).css("z-index", 900);
                }
                if (popupAlerta && popupAlerta.getElement()) {
                    $(popupAlerta.getElement()).css("z-index", 1000);
                }
            }

            if (!self._mapa.hasLayer(popupPrincipal)) {
                self._mapa.addLayer(popupPrincipal.setLatLng(marker.getLatLng()));

                setTimeout(() => {
                    if (popupPrincipal.getElement()) {
                        $(popupPrincipal.getElement()).css("z-index", 900);
                    }
                    if (popupAlerta && popupAlerta.getElement()) {
                        $(popupAlerta.getElement()).css("z-index", 1000);
                    }
                }, 100);
            }

            if (popupAlerta && !self._mapa.hasLayer(popupAlerta)) {
                self._mapa.addLayer(popupAlerta.setLatLng(marker.getLatLng()));
            }
        });

        marker.on('click', function () {
            self.ClickMarkerVeiculo(self, carga, marker);
        });

        if (self._cargaSelecionada && self._cargaSelecionada.CodigoCarga.val() == carga.CodigoCarga) {
            var icon = self._markerVeiculo.getIcon();
            marker.setIcon(icon);
            self._markerVeiculo = marker;
        }

        self._markers.addLayer(marker);
        self._cargasNoMapa.push(carga);
    },
    AdicionarMarkerLocal: function (self, local) {
        if (!this.ValidarPosicao(local.Coordenadas[0].Area.position.lat, local.Coordenadas[0].Area.position.lng))
            return;

        var latitude = parseFloat(local.Coordenadas[0].Area.position.lat.replace(",", "."));
        var longitude = parseFloat(local.Coordenadas[0].Area.position.lng.replace(",", "."));

        let marker = L.marker([latitude, longitude], { icon: new self.IconeMarkerLocalRaioProximidade(false, local.Cor) });

        marker.on('click', function (element) { obterDadosLocalRaioProximidade(local) });

        self._markersLocalRaiosProximidade.addLayer(marker);

    },
    AdicionarRaioProximidade: function (self, local) {
        if (!this.ValidarPosicao(local.Coordenadas[0].Area.position.lat, local.Coordenadas[0].Area.position.lng))
            return;

        var latitude = parseFloat(local.Coordenadas[0].Area.position.lat.replace(",", "."));
        var longitude = parseFloat(local.Coordenadas[0].Area.position.lng.replace(",", "."));
        var raiosOrdenados = local.Raios;
        raiosOrdenados.sort(function (x, y) { return x.Raio > y.Raio ? -1 : 1 });

        for (var i = 0; i < raiosOrdenados.length; i++) {
            var raio = raiosOrdenados[i];
            var circle = L.circle([latitude, longitude], (raio.Raio * 1000), {
                color: raio.Cor,
                fillColor: raio.Cor,
                fillOpacity: 0.5,
                opacity: 1
            });

            self._circles.push(circle);
            circle.addTo(self._mapa);
        }
    },
    CarregarContent: function (data) {
        var icone = TrackingIconRastreador(data.Rastreador);
        var origem = (data.ClienteOrigem != undefined && data.ClienteOrigem != '') ? (data.ClienteOrigem + '' + (data.CidadeOrigem != undefined && data.CidadeOrigem != '' ? ` (${data.CidadeOrigem})` : '')) : '';
        function displayInfo(label, value) {
            const displayValue = value && value !== '' ? value : '-';
            const textColor = value && value !== '' ? '#333' : '#000';
            const textAlign = value && value !== '' ? 'left' : 'auto';

            return `<label style="font-weight: bold; margin-bottom: 2px;">${label}</label>
        <div style="color: ${textColor}; font-size: 14px; text-align: ${textAlign};">${displayValue}</div>`;
        }

        function displayDestinos(destinos) {
            const destinosArray = destinos.split(',').map(d => d.trim());

            if (destinosArray.length > 2) {
                const primeirosDestinos = destinosArray.slice(0, 2).join(', ');
                const restantesDestinos = destinosArray.slice(2).join(', ');

                return `<div style="margin-left: 8px; color: #808080;">
                <span>${displayInfo('DESTINO', primeirosDestinos)}</span>
                <span style="color: #007bff; cursor: pointer;" id="ver-mais" onclick="toggleDestinos('ver-mais', 'destinos-completos', '${restantesDestinos}')">... Ver mais</span>
                <div id="destinos-completos" style="display: none;">
                    ${displayInfo('DESTINO', restantesDestinos)}
                    <span style="color: #007bff; cursor: pointer;" id="ver-menos" onclick="toggleDestinos('ver-menos', 'destinos-completos', '${restantesDestinos}')"> Ver menos</span>
                </div>
            </div>`;
            } else {
                return `<div style=" color: #808080;">${displayInfo('DESTINO', destinos)}</div>`;
            }
        }

        var html = `<div style="width:500px !important;">

   <div style="font-weight: bold; font-size: 22px;">
        ${data.CargaEmbarcador || '-'}
    </div>
    <br>
    <div style="font-weight: bold; font-size: 14px;">
        ENTREGA
    </div>

    <div style="display: flex; gap: 12px; flex-wrap: nowrap; margin-top: 5px;">
        <div style="margin-left: 0px; color: #808080; width: 244px">
            ${displayInfo('TRANSPORTADORA', data.RazaoSocialTransportador)}
        </div>
        <div  style="color: #808080;">
            ${displayInfo('MOTORISTA', data.Motoristas)}
        </div>
        <div  style="color: #808080;">
            ${displayInfo('PLACA', data.Tracao)}
        </div>           
    </div>

    <div style="display: flex; gap: 12px; flex-wrap: nowrap;">
        <div style="color: #808080; margin-top: 5px; width: 244px">
            ${origem && displayInfo('ORIGEM', origem) || '-'}
        </div>
        <div  style="color: #808080; margin-top: 5px;">
            ${data.Destinos && displayDestinos(data.Destinos) || '-'}
        </div>
    </div>


    <div style="margin-top: 8px;">
        <div class="header" style="font-weight: bold; font-size: 14px;">
            DATAS
        </div>
        <div style="display: flex; gap: 16px">
            <div style="flex: 1;  color: #808080; width:259px; margin-top:8px">
                ${displayInfo('ENTREGA PREVISTA', data.DataEntregaPlanejadaProximaEntrega)}
            </div>
            <div style="flex: 1; color: #808080; margin-top:8px ">
                ${displayInfo('CARREGAMENTO', data.DataCarregamentoPedidoFormatada)}
            </div>
        </div>
    </div>

    <div style="margin-top: 8px;">
        <div style="font-weight: bold; font-size: 14px;">
            PRÓXIMA PARADA
        </div>
        <div style="display: flex; gap: 8px; margin-top: 5px; ">
            <div style="color: #808080; width: 254px">
                ${displayInfo('DATA DE AGENDAMENTO', data.DataAgendamentoParada)}
            </div>
            <div style="color: #808080; width: 98px">
                ${displayInfo('TENDÊNCIA', data.TendenciaProximaParadaDescricao)}
            </div>
            <div style="color: #808080;">
                ${displayInfo('PEDIDO DO CLIENTE', data.NumeroPedidoCliente)}
            </div>
        </div>
    </div>

    <div style="margin-top: 8px;">
        <div style="font-weight: bold; font-size: 14px;">
            RASTREIO
        </div>
        <div style="display: flex; justify-content: space-between; gap: 8px;">
            <div style="display: flex; align-items: center; flex: 1;">
                <div class="rastreador-icone" style="width: 40px;">
                    ${icone || '-'}
                </div>
                <div style="color: #808080; margin-left: 8px; width: 220px;">
                    ${displayInfo('RASTREADOR', data.NomeRastreador)}
                </div>            
                <div style="color: #808080;">
                ${displayInfo('ÚLTIMA POSIÇÃO', data.DataPosicaoAtual)}
                <div title="${data.DataUltimaPosicao || '-'}"></div>
            </div>
          </div>        
        </div>        
    </div>
    </div>`;

        return html;
    },
    IconeMarkerCarga: function (destacado, corStatus, cargaCritica, informacoesTempoPermanenciaStatusCarga) {
        if (destacado) {
            var iconVeiculoDestaque = L.icon({
                iconUrl: '../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_truck.png',
                iconSize: [38, 55],
                popupAnchor: [5, -30]
            });

            return iconVeiculoDestaque;
        } else {
            //return iconVeiculo;
            var iconeVeiculoOptions = null;
            var mapdraw = new MapaDraw();

            if (cargaCritica) {
                iconeVeiculoOptions = {
                    iconUrl: mapdraw.icons.trucknewCirculeCargaCritica(45, 47, corStatus),
                    popupAnchor: [5, -30]
                }
            }
            else if (informacoesTempoPermanenciaStatusCarga.TiporRegraViagem == EnumMonitoramentoStatusViagemTipoRegra.Descarga) {
                let excedeuTempoPermitidoRegra = false
                let className = '';

                if (informacoesTempoPermanenciaStatusCarga.TempoPermitidoPermanenciaNoCliente)
                    excedeuTempoPermitidoRegra = VerificarConfiguracaoTempoPermanenciaStatus(informacoesTempoPermanenciaStatusCarga);

                if (excedeuTempoPermitidoRegra) {
                    className = 'blink';
                    corStatus = 'red';
                }

                iconeVeiculoOptions = {
                    iconUrl: mapdraw.icons.truckStatusViagemMonitoramentoNoCliente(48, 50, corStatus),
                    popupAnchor: [5, -30],
                    className: className
                }
            }

            else if (informacoesTempoPermanenciaStatusCarga.TiporRegraViagem == EnumMonitoramentoStatusViagemTipoRegra.EmCarregamento) {
                let excedeuTempoPermitidoRegra = false
                let className = '';

                if (informacoesTempoPermanenciaStatusCarga.TempoPermitidoPermanenciaEmCarregamento)
                    excedeuTempoPermitidoRegra = VerificarConfiguracaoTempoPermanenciaStatus(informacoesTempoPermanenciaStatusCarga);

                if (excedeuTempoPermitidoRegra) {
                    className = 'blink';
                    corStatus = 'red';
                }

                iconeVeiculoOptions = {
                    iconUrl: mapdraw.icons.truckStatusViagemMonitoramentoEmCarregamento(45, 47, corStatus),
                    popupAnchor: [5, -30],
                    className: className
                };
            }

            else {
                iconeVeiculoOptions = {
                    iconUrl: mapdraw.icons.trucknewCircule(45, 47, corStatus),
                    popupAnchor: [5, -30]
                }
            }

            var iconeVeiculo = L.Icon.extend({
                options: iconeVeiculoOptions
            });

            return new iconeVeiculo();
        }
    },
    IconeMarkerLocalRaioProximidade: function () {

        var iconMarkerLocal = L.icon({
            iconUrl: '../../../content/torrecontrole/acompanhamentocarga/assets/icons/local_pin.png',
            iconSize: [38, 48],
            popupAnchor: [5, -30]
        });

        return iconMarkerLocal;

    },
    ValidarPosicao: function (latitude, longitude) {
        if (latitude == undefined || longitude == undefined)
            return false;
        if (latitude == null || longitude == null)
            return false;
        if (latitude == '' || longitude == '')
            return false;
        if (latitude == '0' || longitude == '0')
            return false;
        return true;
    },
    LimparMapa: function (self) {
        if (self._polilinhaPlanejada != null || self._polilinhaRealizada != null) {
            if (self._polilinhaPlanejada != null || self._polilinhaRealizada != null) {
                try {
                    self._mapa.removeLayer(self._polilinhaPlanejada);
                    self._mapa.removeLayer(self._polilinhaRealizada);
                    $(".legenda-mapaCards").hide();
                }
                catch (e) { }
            }
        }

        if (self._markersEntregas && self._markersEntregas.length > 0) {
            try {
                for (var i = 0; i < self._markersEntregas.length; i++) {
                    if (self._markersEntregas[i] != null) {
                        self._mapa.removeLayer(self._markersEntregas[i]);
                    }
                }
                self._markersEntregas = ko.observableArray();
            }
            catch (e) { }
        }

        if (self._markerVeiculo)
            self._mapa.removeLayer(self._markerVeiculo);

        if (self._markersLocalRaiosProximidade)
            self._mapa.removeLayer(self._markersLocalRaiosProximidade);

        if (self._circles && self._circles.length > 0) {
            try {
                for (var i = 0; i < self._circles.length; i++) {
                    if (self._circles[i] != null) {
                        self._mapa.removeLayer(self._circles[i]);
                    }
                }
                self._circles = ko.observableArray();
            }
            catch (e) { }
        }

        self.LimparCargasNoMapa(self);

        self._polilinhaPlanejada = null;
        self._polilinhaRealizada = null;
    },
    LimparVeiculoSelecionado: function (self) {
        $(".legenda-mapaCards").hide();

        if (self._polilinhaPlanejada != null || self._polilinhaRealizada != null) {
            try {
                self._mapa.removeLayer(self._polilinhaPlanejada);
                self._mapa.removeLayer(self._polilinhaRealizada);
            }
            catch (e) { }
        }

        self._polilinhaPlanejada = null;
        self._polilinhaRealizada = null;

        if (self._markersEntregas && self._markersEntregas.length > 0) {
            try {
                for (var i = 0; i < self._markersEntregas.length; i++) {
                    if (self._markersEntregas[i] != null) {
                        self._mapa.removeLayer(self._markersEntregas[i]);
                    }
                }

                self._markersEntregas = new Array();
            }
            catch (e) { }
        }

        if (self._markerVeiculo != null)
            self._markerVeiculo.setIcon(new self.IconeMarkerCarga(false, self._corAnteriorSelecionada, self._cargaCritica, self._informacoesTempoPermanenciaStatusCarga));
    },
    ClickMarkerVeiculo: function (self, carga, marker) {
        var clicouNoMesmoVeiculo = self._cargaSelecionada == carga;

        self.LimparVeiculoSelecionado(self);
        self._cargaSelecionada = null;

        if (clicouNoMesmoVeiculo) return;

        self._cargaSelecionada = carga;

        if (self._corAnteriorSelecionada == null)
            self._corAnteriorSelecionada = carga.Cor;

        if (self._cargaCritica == null)
            self._cargaCritica = carga.Critico;

        if (self._informacoesTempoPermanenciaStatusCarga == null)
            self._informacoesTempoPermanenciaStatusCarga = {
                TiporRegraViagem: carga.TiporRegraViagem,
                TempoPermanenciaStatus: carga.TempoStatusEmMinutos,
                TempoPermitidoPermanenciaEmCarregamento: carga.TempoPermitidoPermanenciaEmCarregamento,
                TempoPermitidoPermanenciaNoCliente: carga.TempoPermitidoPermanenciaEmCarregamento
            }

        if (self._markerVeiculo != null)//volta o anterior caso clicou em outro
            self._markerVeiculo.setIcon(new self.IconeMarkerCarga(false, self._corAnteriorSelecionada, self._cargaCritica, self._informacoesTempoPermanenciaStatusCarga));

        self._corAnteriorSelecionada = carga.Cor;
        self._cargaCritica = carga.Critico;
        self._markerVeiculo = marker;
        self._informacoesTempoPermanenciaStatusCarga = {
            TiporRegraViagem: carga.TiporRegraViagem,
            TempoPermanenciaStatus: carga.TempoStatusEmMinutos,
            TempoPermitidoPermanenciaEmCarregamento: carga.TempoPermitidoPermanenciaEmCarregamento,
            TempoPermitidoPermanenciaNoCliente: carga.TempoPermitidoPermanenciaEmCarregamento
        };

        //AQUI VAMOS BUSAR A POLILINHA DA CARGA.. PREVISTA E REALISADA. E MARCAR OS PONTOS DOS CLIENTES =D BUSCAR A POLILINHA DO MONITORAMENTO
        var arrayLatLongPrevista = new Array();
        var arrayLatLongRealizada = new Array();
        executarReST("Monitoramento/ObterDadosMapaHistoricoPosicaoTelaAcompanhamentoCarga", {
            Codigo: carga.Codigo
        }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    setTimeout(function () {

                        for (var i = 0; i < arg.Data.wayPointsPrevistos.length; i++) {
                            var latlng = L.latLng(arg.Data.wayPointsPrevistos[i].Latitude, arg.Data.wayPointsPrevistos[i].Longitude);
                            arrayLatLongPrevista.push(latlng);
                        }

                        if (arg.Data.wayPointsRealizados != null) {
                            for (var i = 0; i < arg.Data.wayPointsRealizados.length; i++) {
                                var latlng = L.latLng(arg.Data.wayPointsRealizados[i].Latitude, arg.Data.wayPointsRealizados[i].Longitude);
                                arrayLatLongRealizada.push(latlng);
                            }
                        }

                        self.DesenharEntregas(self, arg.Data.Entregas);

                        var iconVeiculoDestaque = L.icon({
                            iconUrl: '../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_truck.png',
                            iconSize: [38, 55],
                            iconAnchor: [0, 0],
                            popupAnchor: [-3, -76]
                        });

                        self._markerVeiculo.setIcon(iconVeiculoDestaque);
                        self._markerVeiculo.openPopup(self._markerVeiculo.getLatLng());

                        if (arrayLatLongRealizada.length > 0) {
                            var vermelho = '#9400d3';
                            self._polilinhaRealizada = L.polyline(arrayLatLongRealizada, { color: vermelho, smoothFactor: 3.0, weight: 5, }).addTo(self._mapa);
                            self.LegendasMapaCards(self, self._cargaSelecionada.DistanciaPercorrida, vermelho, "rota-realizada", true);
                        }

                        if (arrayLatLongPrevista.length > 0) {
                            var azul = '#016f65'
                            self._polilinhaPlanejada = L.polyline(arrayLatLongPrevista, { color: azul, smoothFactor: 3.0, weight: 5, opacity: 0.6 }).addTo(self._mapa);

                            // zoom the map to the polyline
                            //_mapaAcompanhamento.fitBounds(_polilinhaPlanejada.getBounds());
                            self.LegendasMapaCards(self, self._cargaSelecionada.DistanciaPrevistaFormatada, azul, "rota-planejada", true);
                        }

                        $(".legenda-mapaCards").show();
                    }, 500);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    },
    DesenharEntregas: function (self, entregas) {
        self._markersEntregas = new Array();

        var total = entregas.length;
        if (total && total > 0) {

            for (var i = 0; i < total; i++) {
                var iconeVeiculoOptions = null;

                if ((typeof entregas[i].Latitude) == "string")
                    entregas[i].Latitude = Globalize.parseFloat(entregas[i].Latitude);

                if ((typeof entregas[i].Longitude) == "string")
                    entregas[i].Longitude = Globalize.parseFloat(entregas[i].Longitude);

                iconeVeiculoOptions = {
                    iconUrl: entregas[i].OrdemPrevista == 0 ? "'../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_start.png" : self._mapaGoogle.obterIconSVGMarcador(self._mapaGoogle.obterSVGPin(self.CorEntrega(entregas[i].Situacao), entregas[i].OrdemPrevista))
                };

                let iconeEntrega = L.Icon.extend({
                    options: iconeVeiculoOptions
                });

                var conteudo = self.CarregarContentEntrega(entregas[i]);

                let markerEntrega = L.marker([entregas[i].Latitude, entregas[i].Longitude], { icon: new iconeEntrega() });
                markerEntrega.bindPopup(conteudo, { offset: [12, 0] });
                markerEntrega.addTo(self._mapa);

                self._markersEntregas.push(markerEntrega);
            }
        }
    },
    CarregarContentEntrega: function (entrega) {
        var html = '<div> <strong>' + entrega.Descricao + '</strong><br/><br>';
        var tipoDestino = (entrega.Coleta) ? 'coleta' : 'entrega';
        if (entrega.ValorTotalNF != undefined && entrega.ValorTotalNF != '') html += 'Valor: ' + entrega.ValorTotalNF + '<br/>';
        if (entrega.Status != undefined && entrega.Status != '') html += 'Situação da ' + tipoDestino + ': ' + entrega.Status + '<br/>';
        if (entrega.OrdemPrevista != undefined && entrega.OrdemPrevista > 0) html += 'Sequência ' + tipoDestino + ' prevista : ' + entrega.OrdemPrevista + '<br/>';
        if (entrega.OrdemRealizada != undefined && entrega.OrdemRealizada > 0) html += 'Sequência ' + tipoDestino + ' realizada : ' + entrega.OrdemRealizada + '<br/>';
        if (entrega.Chegada != undefined && entrega.Chegada != '') html += 'Chegada: ' + entrega.Chegada + '<br/>';
        if (entrega.Saida != undefined && entrega.Saida != '') html += 'Saída: ' + entrega.Saida + '<br/>';
        if (entrega.TempoAtendimento != undefined && entrega.TempoAtendimento != '') html += 'Tempo de atendimento: ' + entrega.TempoAtendimento + '<br/>';
        if (entrega.TempoDirigindo != undefined && entrega.TempoDirigindo != '') html += 'Tempo dirigindo: ' + entrega.TempoDirigindo + '<br/>';
        if (entrega.DistanciaPercorrida != undefined && entrega.DistanciaPercorrida != '') html += 'Distância percorrida: ' + entrega.DistanciaPercorrida;

        html += '</div>';

        return html;
    },
    OpenAllPopups: function (self) {
        self._markerArrayAlert.forEach(function (marker) {
            var popup = marker.getPopup();
            marker.bindPopup(popup.getContent()).openPopup();
        });
    },
    CorEntrega: function (situacao) {
        var TRACKING_ENTREGA_PENDENTE_COR = "#D45B5B";
        var TRACKING_ENTREGA_EN_ANDAMENTO_COR = "#DED84C";
        var TRACKING_ENTREGA_REALIZADA_COR = "#5ECC71";

        if (situacao == EnumSituacaoEntrega.Entregue) {
            return TRACKING_ENTREGA_REALIZADA_COR;
        } else if (situacao == EnumSituacaoEntrega.EmCliente) {
            return TRACKING_ENTREGA_EN_ANDAMENTO_COR;
        } else {
            return TRACKING_ENTREGA_PENDENTE_COR;
        }
    },
    LegendasMapaCards: function (self, distancia, cor, classLi, visible) {
        var classVeiculo = "rota-veiculo";
        var classOff = "off";
        let veiculos = typeof self._cargaSelecionada.Veiculos == 'object' ? self._cargaSelecionada.Veiculos.val() : self._cargaSelecionada.Veiculos;
        let distanciaPrevista = typeof self._cargaSelecionada.DistanciaPrevistaFormatada == 'object' ? self._cargaSelecionada.DistanciaTotal.val() : self._cargaSelecionada.DistanciaPrevistaFormatada;
        var element = $("ul.legendaMapaCard li." + classLi);
        $("ul.legendaMapaCard li." + classLi).removeClass(classOff);
        $("ul.legendaMapaCard li." + classVeiculo).remove();
        $("ul.legendaMapaCard li.icone-legenda").remove();

        $("ul.legendaMapaCard").append(
            $('<li class="icone-legenda"><img style="widht:30px; height:30px" src="../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_start.png" ></img><span class="descricao"> Coleta/Inicio</span></li>'),
            $('<li class="icone-legenda"><img style="widht:30px; height:30px" src="../../../content/torrecontrole/acompanhamentocarga/assets/icons/icon_truck.png" ></img><span class="descricao"> Posição atual do veículo </span></li>'),
            $('<li class="rota-realizada rota-veiculo"><i class="fa fa-truck"></i><span class="descricao"> ' + veiculos + '</span><span class="distancia">' + distanciaPrevista + '</span></li>'),
        );

        if (element) {
            element.show();
            if (!visible) element.addClass(classOff);
            element.find("span.distancia").html(distancia);
            element.find("span.linha").css("background-color", cor);
        }
    },
    InserirLegendaMapa: function (idSelector, id, text, color, quantity) {
        if (!color || color == "#FFFFFF") color = "#58594f";

        var numberColor = hexToRgbA(color, 0.9);

        $('#' + idSelector).append(
            '<li id="' + idSelector + "-" + id + '">' +
            '<div class="divLegendaMapaCheckboxDescricao">' +
            '<input type="checkbox" value="' + id + '" id="' + idSelector + 'CheckBox' + id + '" class="legendaMapaCheckbox">' +
            '<div class="legendaMapaCheckboxDescricao">' + (text || "Não Definido") + '</div>' +
            '</div>' +
            '<span style="color:' + numberColor + '; font-weight: 700; font-size: 15px;">' + quantity + '</span>' +
            '</li> ');
    },
    RecarregarMapaComDadosFiltrados: function (self, dadosFiltrados, locaisFiltrados) {
        self.LimparMapa(self);
        self.Load(dadosFiltrados);
        self.LoadLocaisRaioProximidade(locaisFiltrados);
    },
    RecarregarFiltrosAplicados: function (self) {
        for (var i = 0; i < self._filtrosMapaAplicados.length; i++) {
            var filtro = self._filtrosMapaAplicados[i];
            $("#" + filtro.Id)[0].checked = true;
        }
    },
    ObterDadosFiltrados: function (self, _gridMonitoramentoNovo) {
        //Inicia o filtro com as cargas originais retornadas da consulta.
        var dadosFiltrados = backup_cargasNoMapa;

        //Obtém valores dos filtros selecionados.
        var listaValoresFiltradosStatusViagem = self._filtrosMapaAplicados.filter((filtro) => filtro.Type == "StatusViagem").map((filtro) => { return parseInt(filtro.Value) });
        var listaValoresFiltradosSituacaoCarga = self._filtrosMapaAplicados.filter((filtro) => filtro.Type == "SituacaoDaCarga").map((filtro) => { return parseInt(filtro.Value) });
        var listaValoresFiltradosAlerta = self._filtrosMapaAplicados.filter((filtro) => filtro.Type == "Alerta").map((filtro) => { return parseInt(filtro.Value) });
        var listaValoresFiltradosStatusAlerta = self._filtrosMapaAplicados.filter((filtro) => filtro.Type == "StatusAlertas").map((filtro) => { return parseInt(filtro.Value) });
        var listaValoresFiltradosRaiosProximidade = self._filtrosMapaAplicados.filter((filtro) => filtro.Type == "LocalRaiosProximidade").map((filtro) => { return parseInt(filtro.Value) });
        var listaValoresFiltradosTendenciaEntrega = self._filtrosMapaAplicados.filter((filtro) => filtro.Type == "TendenciaProximaParada").map((filtro) => { return parseInt(filtro.Value) });

        //Aplica filtro de Status de Viagem.
        if (listaValoresFiltradosStatusViagem && listaValoresFiltradosStatusViagem.length > 0) {
            if (_configuracaoLegendaUsuarioMonitoramentoMapa && _configuracaoLegendaUsuarioMonitoramentoMapa.ExibirPorGrupoStatusViagem.val())
                dadosFiltrados = dadosFiltrados.filter((carga) => listaValoresFiltradosStatusViagem.includes(carga.GrupoStatusViagemCodigo));
            else
                dadosFiltrados = dadosFiltrados.filter((carga) => listaValoresFiltradosStatusViagem.includes(carga.TiporRegraViagem));
        }

        //Aplica filtro de Situação da Carga.
        if (listaValoresFiltradosSituacaoCarga && listaValoresFiltradosSituacaoCarga.length > 0)
            dadosFiltrados = dadosFiltrados.filter((carga) => listaValoresFiltradosSituacaoCarga.includes(carga.SituacaoCarga));

        //Aplica filtro de Alertas.
        if (listaValoresFiltradosAlerta && listaValoresFiltradosAlerta.length > 0)
            dadosFiltrados = dadosFiltrados.filter((carga) => listaValoresFiltradosAlerta.includes(carga.TipoUltimoAlertaMonitoramento));

        //Aplica filtro de Status de Alertas.
        if (listaValoresFiltradosStatusAlerta && listaValoresFiltradosStatusAlerta.length > 0)
            dadosFiltrados = dadosFiltrados.filter((carga) => carga.CodigoUltimoAlerta != "0" && listaValoresFiltradosStatusAlerta.includes(carga.StatusUltimoAlertaMonitoramento));

        //Aplica filtro de Raios Proximidade.
        if (listaValoresFiltradosRaiosProximidade && listaValoresFiltradosRaiosProximidade.length > 0)
            dadosFiltrados = dadosFiltrados.filter((carga) => listaValoresFiltradosRaiosProximidade.includes(carga.CodigoLocalRaiosProximidade));

        //Aplica filtro de Tendência de Entrega.
        if (listaValoresFiltradosTendenciaEntrega && listaValoresFiltradosTendenciaEntrega.length > 0)
            dadosFiltrados = dadosFiltrados.filter((carga) => listaValoresFiltradosTendenciaEntrega.includes(carga.TendenciaProximaParada));

        //Limpa os registros filtrados anteriormente na grid.
        for (const carga of backup_cargasNoMapa)
            _gridMonitoramentoNovo.setarCorGridPorID(carga.Codigo, "");

        //Marca os novos registros filtrados na grid.
        if (_gridMonitoramentoNovo && self._filtrosMapaAplicados.length > 0) {
            for (const carga of dadosFiltrados)
                _gridMonitoramentoNovo.setarCorGridPorID(carga.Codigo, "#B4FF00");
        }
        return dadosFiltrados;
    },
    ObterLocaisFiltrados: function (self, dados) {
        var dadosFiltrados = dados;
        var filtrosLocais = [];

        if (dados) {
            for (var i = 0; i < self._filtrosMapaAplicados.length; i++) {
                var filtro = self._filtrosMapaAplicados[i];
                $("#" + filtro.Id)[0].checked = true;

                if (filtro.Type == "LocalRaiosProximidade") {
                    filtrosLocais.push(dadosFiltrados.Locais.find((raio) => raio.Codigo == parseInt(filtro.Value)));
                }
            }

        }

        return { Locais: filtrosLocais };
    }
}

function VerificarConfiguracaoTempoPermanenciaStatus(informacoesTempoPermanenciaStatusCarga) {
    let excedeuTempoPermitido = false;

    if (informacoesTempoPermanenciaStatusCarga.TempoPermanenciaStatus) {
        let tempoDecorridoMinutos = parseInt(informacoesTempoPermanenciaStatusCarga.TempoPermanenciaStatus, 10);

        // Obtém o tempo permitido
        let tempoPermitido = parseInt(informacoesTempoPermanenciaStatusCarga.TempoPermitidoPermanenciaEmCarregamento, 10);

        // Verifica se o tempo decorrido já passou do tempo permitido
        excedeuTempoPermitido = tempoDecorridoMinutos > tempoPermitido;
    }

    return excedeuTempoPermitido;
}

