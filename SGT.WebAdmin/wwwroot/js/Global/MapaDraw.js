function ShapeCircle() {
    this.type = google.maps.drawing.OverlayType.CIRCLE;
    this.zIndex = -1;
    this.fillColor = "#000000";
    this.fillOpacity = 0.45;
    this.radius = null;
    this.center = null;
    this.strokeWeight = 0;
    this.strokeColor = null;
    this.strokeOpacity = null;
    this.content = null;
}

function ShapeRectangle() {
    this.type = google.maps.drawing.OverlayType.RECTANGLE;
    this.zIndex = -1;
    this.fillColor = null;
    this.fillOpacity = 0.45;
    this.bounds = null;
    this.strokeWeight = 0;
    this.strokeColor = null;
    this.strokeOpacity = null;
    this.content = null;
}

function ShapePolyline() {
    this.type = google.maps.drawing.OverlayType.POLYLINE;
    this.zIndex = -1;
    this.path = null;//Latitude e Longitude
    this.strokeWeight = 3;
    this.strokeColor = null;
    this.strokeOpacity = null;
    this.content = null;
}

function ShapePolygon() {
    this.type = google.maps.drawing.OverlayType.POLYGON;
    this.zIndex = -1;
    this.fillColor = null;
    this.fillOpacity = 0.45;
    this.paths = null;//Latitude e Longitude
    this.strokeWeight = 0;
    this.strokeColor = null;
    this.strokeOpacity = null;
    this.content = null;
}

function ShapeMarker() {
    this.type = google.maps.drawing.OverlayType.MARKER;
    this.position = null;
    this.icon = null;
    this.title = null;
    this.content = null;
    this.draggable = false;
    this.animation = null;
    this.setPosition = function (lat, lng) {
        this.position = new google.maps.LatLng(lat, lng);
    };
    this.setOnDragListener = function (callback) {
        this.onDragListener = callback;
    }
    this.setAnimation = function (animacao) {
        this.animation = animacao;
    }
}

function randomNumber(min, max) {
    return Math.random() * (max - min) + min;
}

infoWindowOpened = null;
function MapaDraw(_map, _divColorPalette) {
    var drawingManager;
    var selectedShape;
    var colors = ['#1E90FF', '#FF0000', '#32CD32', '#FF8C00', '#4B0082',];
    var selectedColor;
    var colorButtons = {};
    var listShapes = new Array();
    var map = _map;
    var self = this;
    var exibirControles = divColorPalette !== undefined;
    var shapeDraggable = false;
    var shapeEditable = false;
    var divColorPalette = divColorPalette;
    var createdGoogleEvents = false;
    var _btnDeleleSelected;

    function clearSelection() {
        if (selectedShape) {
            if (selectedShape.type !== 'marker') {
                selectedShape.setEditable(false);
            }
            selectedShape = null;
            showHideButton(_btnDeleleSelected, false);
        }
    }

    function setSelection(shape) {
        clearSelection();
        if (shape.type !== 'marker') {
            shape.setEditable(true);
            selectColor(shape.get('fillColor') || shape.get('strokeColor'));
        }
        selectedShape = shape;
        showHideButton(_btnDeleleSelected, (shape != null && shape != undefined));
    }

    function deleteSelectedShape() {
        if (selectedShape) {

            selectedShape.setMap(null);

            let idx = listShapes.indexOf(selectedShape);

            listShapes.splice(idx, 1);

            showHideButton(_btnDeleleSelected, false);
        }
    }

    function selectColor(color) {
        if (colorButtons === {})
            return;

        selectedColor = color;
        var buttonsel = null;

        for (var i = 0; i < colors.length; ++i) {
            var currColor = colors[i];

            if (currColor === color)
                buttonsel = currColor;

            colorButtons[currColor].style.border = 'none';
        }

        if (buttonsel)
            colorButtons[buttonsel].style.border = '1px solid #fff';

        var polylineOptions = drawingManager.get('polylineOptions');
        polylineOptions.strokeColor = color;
        drawingManager.set('polylineOptions', polylineOptions);

        var rectangleOptions = drawingManager.get('rectangleOptions');
        rectangleOptions.fillColor = color;
        drawingManager.set('rectangleOptions', rectangleOptions);

        var circleOptions = drawingManager.get('circleOptions');
        circleOptions.fillColor = color;
        drawingManager.set('circleOptions', circleOptions);

        var polygonOptions = drawingManager.get('polygonOptions');
        polygonOptions.fillColor = color;
        drawingManager.set('polygonOptions', polygonOptions);
    }

    function setSelectedShapeColor(color) {
        if (selectedShape) {
            if (selectedShape.type === google.maps.drawing.OverlayType.POLYLINE) {
                selectedShape.set('strokeColor', color);
            } else {
                selectedShape.set('fillColor', color);
            }
        }
    }

    function makeColorButton(color) {
        var button = document.createElement('span');
        button.style.backgroundColor = color;
        button.style.margin = "2px";
        button.style.width = "18px";
        button.style.height = "18px";
        button.style.margin = "2px";
        button.style.float = "left";
        button.style.cursor = "pointer";

        google.maps.event.addDomListener(button, 'click', function () {
            selectColor(color);
            setSelectedShapeColor(color);
        });

        return button;
    }

    function showHideButton(button, show) {
        if (button !== null && button != undefined) {
            if (!show)
                button.style.display = 'none';
            else
                button.style.display = 'block';
        }
    }

    this.deleteAll = function (onlyGragabbles) {
        if (!onlyGragabbles) onlyGragabbles = false;
        newListShapes = [];
        for (var i = 0; i < listShapes.length; i++) {
            if (!onlyGragabbles || (onlyGragabbles && listShapes[i].draggable)) {
                listShapes[i].setMap(null);
            } else {
                newListShapes.push(listShapes[i]);
            }
        }
        listShapes = newListShapes
    };

    this.deletemarkerCluster = function () {
        if (markerCluster != undefined)
            markerCluster.clearMarkers();
    }

    this.deleteSelected = function () {
        deleteSelectedShape();
    }

    this.clear = function (onlyGragabbles) {
        this.deleteAll(onlyGragabbles);
        this.deletemarkerCluster();
    }

    function makeDeleteButton() {
        var button = document.createElement('BUTTON');
        button.innerHTML = 'Limpar tudo';
        button.style.height = "18px";
        button.style.float = 'left';
        google.maps.event.addDomListener(button, 'click', function () {
            self.deleteAll(true);
        });
        return button;
    }

    function makeDeleteSelectedShapeButton() {
        var button = document.createElement('BUTTON');
        button.innerHTML = 'Limpar selecionado';
        button.style.height = "18px";
        button.style.display = 'none';
        button.style.float = 'left';
        button.style.marginLeft = '3px';
        google.maps.event.addDomListener(button, 'click', function () {
            self.deleteSelected();
        });
        return button;
    }

    var polyOptions = {
        strokeWeight: 0,
        fillOpacity: 0.45,
        editable: true,
        draggable: true
    };

    var createDrawingManager = function () {
        if (exibirControles) {

            if (drawingManager !== null && drawingManager !== undefined) {
                drawingManager.setOptions({ drawingControl: true });
                return;
            }

            drawingManager = new google.maps.drawing.DrawingManager({
                drawingControl: exibirControles,
                drawingMode: google.maps.drawing.OverlayType.POLYGON,
                drawingControlOptions: {
                    position: google.maps.ControlPosition.TOP_LEFT,
                    drawingModes: ['marker', 'circle', 'polygon', 'rectangle']
                },

                markerOptions: {
                    draggable: true//,
                    //icon: 'beachflag.png'
                },
                polylineOptions: {
                    editable: true,
                    draggable: true
                },
                rectangleOptions: polyOptions,
                circleOptions: polyOptions,
                polygonOptions: polyOptions,
                map: map
            });
        }
    }

    createDrawingManager();

    function setEvents(newShape) {
        if (!exibirControles)
            return;

        if (newShape.type !== google.maps.drawing.OverlayType.MARKER) {

            drawingManager.setDrawingMode(null);


            google.maps.event.addListener(newShape, 'click', function (e) {
                if (e.vertex !== undefined) {
                    if (newShape.type === google.maps.drawing.OverlayType.POLYGON) {
                        var path = newShape.getPaths().getAt(e.path);
                        path.removeAt(e.vertex);
                        if (path.length < 3) {
                            newShape.setMap(null);
                        }
                    }
                    if (newShape.type === google.maps.drawing.OverlayType.POLYLINE) {
                        var pathPolilyne = newShape.getPath();
                        pathPolilyne.removeAt(e.vertex);
                        if (path.length < 2) {
                            newShape.setMap(null);
                        }
                    }
                }
                setSelection(newShape);
            });
        }
        else {
            google.maps.event.addListener(newShape, 'click', function (e) {
                setSelection(newShape);
            });
        }

    }

    var createGoogleEvents = function () {
        if ((drawingManager) && (!createdGoogleEvents)) {
            createdGoogleEvents = true;
            google.maps.event.addListener(drawingManager, 'overlaycomplete', function (e) {
                var newShape = e.overlay;
                newShape.type = e.type;

                listShapes.push(e.overlay);

                setEvents(newShape);

                setSelection(newShape)

            });

            google.maps.event.addDomListener(document, 'keyup', function (e) {

                var code = (e.keyCode ? e.keyCode : e.which);


                if (code === 46) {
                    deleteSelectedShape();

                }
            });


            google.maps.event.addListener(drawingManager, 'drawingmode_changed', clearSelection);


            google.maps.event.addListener(map, 'click', clearSelection);

        }

    };

    createGoogleEvents();

    var buildPalette = function () {
        if (!exibirControles)
            return;

        map.controls[google.maps.ControlPosition.TOP_LEFT].push(divColorPalette);

        for (var i = 0; i < colors.length; ++i) {
            var currColor = colors[i];
            var colorButton = makeColorButton(currColor);
            divColorPalette.appendChild(colorButton);
            colorButtons[currColor] = colorButton;
        }

        selectColor(colors[0]);

        var buttondelete = makeDeleteButton();
        divColorPalette.appendChild(buttondelete);

        _btnDeleleSelected = makeDeleteSelectedShapeButton();
        divColorPalette.appendChild(_btnDeleleSelected);
    };

    buildPalette();


    this.addShape = function (shape, backgroundOnly, eventShowInfoWindowEvent, callbackShowInfoWindowEvent) {
        var newshape = null;

        if (!backgroundOnly)
            backgroundOnly = false;

        if (shape.type === google.maps.drawing.OverlayType.CIRCLE) {
            newshape = new google.maps.Circle({
                strokeWeight: shape.strokeWeight,
                strokeColor: shape.strokeColor,
                strokeOpacity: shape.strokeOpacity,
                fillColor: (backgroundOnly ? '#000' : shape.fillColor),
                fillOpacity: (backgroundOnly ? 0.25 : shape.fillOpacity),
                center: shape.center,
                radius: shape.radius,
                zIndex: shape.zIndex,
                type: shape.type,
                editable: shapeEditable,
                backgroundOnly: backgroundOnly,
                draggable: shapeDraggable,
                content: shape.content,
                map: map
            });

        }

        if (shape.type === google.maps.drawing.OverlayType.RECTANGLE) {
            newshape = new google.maps.Rectangle({
                strokeWeight: shape.strokeWeight,
                strokeColor: shape.strokeColor,
                strokeOpacity: shape.strokeOpacity,
                fillColor: (backgroundOnly ? '#000' : shape.fillColor),
                fillOpacity: (backgroundOnly ? 0.25 : shape.fillOpacity),
                zIndex: shape.zIndex,
                bounds: shape.bounds,
                type: shape.type,
                editable: shapeEditable,
                draggable: shapeDraggable,
                backgroundOnly: backgroundOnly,
                content: shape.content,
                map: map
            });
        }

        if (shape.type === google.maps.drawing.OverlayType.POLYLINE) {
            newshape = new google.maps.Polyline({
                strokeWeight: shape.strokeWeight,
                strokeColor: (backgroundOnly ? '#000' : shape.strokeColor),
                strokeOpacity: (backgroundOnly ? 0.5 : shape.strokeOpacity),
                zIndex: shape.zIndex,
                path: shape.path,
                type: shape.type,
                editable: shapeEditable,
                draggable: shapeDraggable,
                backgroundOnly: backgroundOnly,
                content: shape.content,
                map: map
            });
        }

        if (shape.type === google.maps.drawing.OverlayType.POLYGON) {
            newshape = new google.maps.Polygon({
                strokeWeight: shape.strokeWeight,
                strokeColor: (backgroundOnly ? '#000' : shape.strokeColor),
                strokeOpacity: (backgroundOnly ? 0.5 : shape.strokeOpacity),
                fillColor: (backgroundOnly ? '#000' : shape.fillColor),
                fillOpacity: (backgroundOnly ? 0.25 : shape.fillOpacity),
                zIndex: shape.zIndex,
                paths: shape.paths,
                type: shape.type,
                editable: shapeEditable,
                draggable: shapeDraggable,
                backgroundOnly: backgroundOnly,
                content: shape.content,
                map: map
            });
        }

        if (shape.type === google.maps.drawing.OverlayType.MARKER) {
            newshape = new google.maps.Marker({
                position: shape.position,
                info: infowindow,
                icon: shape.icon,
                type: shape.type,
                editable: shapeEditable,
                draggable: shape.draggable ? shape.draggable : shapeDraggable,
                backgroundOnly: backgroundOnly,
                map: map,
                title: shape.title,
                content: shape.content,
                animation: shape.animation ? shape.animation : null
            });

            if (shape.draggable && shape.onDragListener) {
                google.maps.event.addListener(newshape, 'dragend', (marker) => shape.onDragListener(marker, map, newshape));
            }
        }

        if (
            (shape.type === google.maps.drawing.OverlayType.MARKER && shape.title != null && shape.title != '')
            ||
            (shape.type !== google.maps.drawing.OverlayType.MARKER && shape.content != null && shape.content != '' && eventShowInfoWindowEvent != null)
        ) {
            var contentInfoWindow = (shape.type === google.maps.drawing.OverlayType.MARKER) ? ((shape.content != null && shape.content != '') ? shape.content : shape.title) : shape.content;
            var infowindow = new google.maps.InfoWindow({
                content: contentInfoWindow
            });

            if (eventShowInfoWindowEvent == 'click') {
                newshape.addListener('click', function (event) {
                    if (shape.type === google.maps.drawing.OverlayType.MARKER) {
                        infowindow.setPosition(newshape.position);
                    } else {
                        infowindow.setPosition(event.latLng);
                    }
                    self.openInfoWindow(infowindow, this);
                    if (callbackShowInfoWindowEvent != undefined && typeof callbackShowInfoWindowEvent === "function") {
                        callbackShowInfoWindowEvent(infowindow, newshape);
                    }
                });
            } else {
                newshape.addListener('mouseover', function () {
                    self.openInfoWindow(infowindow, this);
                    if (callbackShowInfoWindowEvent != undefined && typeof callbackShowInfoWindowEvent === "function") {
                        callbackShowInfoWindowEvent(infowindow, newshape);
                    }
                });
                newshape.addListener('mouseout', function () {
                    self.closeInfoWindow(infowindow);
                });
            }

            newshape.AbrirInfo = function () {
                self.openInfoWindow(infowindow, this);
            }

        }

        if (!backgroundOnly)
            setEvents(newshape);

        listShapes.push(newshape);

        return newshape;

    };

    this.openInfoWindow = function (infowindow, shape) {
        self.closeInfoWindowOpened();
        infowindow.open(map, shape);
        infoWindowOpened = infowindow;
    }
    this.closeInfoWindow = function (infowindow) {
        infowindow.close();
        infoWindowOpened = null;
    }
    this.closeInfoWindowOpened = function () {
        if (infoWindowOpened) {
            infoWindowOpened.close();
            infoWindowOpened = null;
        }
    }

    var markerCluster;
    this.addMarkerCluster = function () {
        listaMakers = [];
        for (var i = 0; i < listShapes.length; i++) {

            if (listShapes[i].type = google.maps.drawing.OverlayType.MARKER)
                listaMakers.push(listShapes[i]);

        }

        var options = {
            imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m',
            minimumClusterSize: 1,
            maxZoom: 8
        }

        markerCluster = new MarkerClusterer(map, listaMakers, options);
    };


    this.centerShapes = function () {

        var bounds = new google.maps.LatLngBounds();

        for (var i = 0; i < listShapes.length; i++) {
            var shape = listShapes[i];

            if (shape.type === google.maps.drawing.OverlayType.CIRCLE) {
                bounds.extend(shape.getBounds().getNorthEast());
                bounds.extend(shape.getBounds().getSouthWest());
            }

            if (shape.type === google.maps.drawing.OverlayType.RECTANGLE) {
                var start = shape.bounds.getNorthEast();
                var end = shape.bounds.getSouthWest();
                bounds.extend(start);
                bounds.extend(end);
            }

            if (shape.type === google.maps.drawing.OverlayType.POLYGON) {
                var path = shape.getPath();
                if (path) {
                    var locations = path.getArray();
                    if (locations) {
                        for (var j = 0; j < locations.length; j++) {
                            var location = locations[j];
                            bounds.extend(location);
                        }
                    }
                }
            }

            if (shape.type === google.maps.drawing.OverlayType.MARKER) {
                bounds.extend(shape.position);
            }

            if (shape.type === google.maps.drawing.OverlayType.POLYLINE) {
                var path = shape.getPath();
                if (path) {
                    var locationsPoliline = path.getArray();
                    if (locationsPoliline) {
                        for (var w = 0; w < locationsPoliline.length; w++) {
                            var locationPoliline = locationsPoliline[w];
                            bounds.extend(locationPoliline);
                        }
                    }
                }
            }

        }

        if (listShapes.length > 0) {
            map.fitBounds(bounds);
            map.panToBounds(bounds);
        }
    };

    this.setCenter = function (position) {
        if (position && map) {
            map.setCenter(position);
        }
    }

    this.getJson = function (shapes) {
        var listShapesJson = [];
        var newshape = null;

        if (!shapes) shapes = listShapes;

        for (var i = 0; i < shapes.length; i++) {
            var shape = shapes[i];

            //Verificando se não é apenas uma camada de fundo...
            //Utilizada na tela Logistica/Locais para apresentar outroas micro-regiões de roteirização ao editar.
            if (!shape.backgroundOnly) {
                if (shape.type === google.maps.drawing.OverlayType.CIRCLE) {
                    newshape = new ShapeCircle();
                    newshape.fillColor = shape.fillColor;
                    newshape.radius = shape.radius;
                    newshape.center = shape.center;
                    newshape.zIndex = shape.zIndex;
                    listShapesJson.push(newshape);
                }
                else if (shape.type === google.maps.drawing.OverlayType.RECTANGLE) {
                    newshape = new ShapeRectangle();
                    newshape.fillColor = shape.fillColor;
                    newshape.zIndex = shape.zIndex;
                    newshape.bounds = shape.bounds;
                    listShapesJson.push(newshape);
                }
                else if (shape.type === google.maps.drawing.OverlayType.POLYLINE) {

                    //Verifica se shape.getPath existe e é uma função
                    if (typeof shape.getPath === "function" && shape.getPath()) {
                        newshape = new ShapePolyline();
                        newshape.strokeColor = shape.strokeColor;
                        newshape.zIndex = shape.zIndex;
                        newshape.path = shape.getPath().getArray();
                        listShapesJson.push(newshape);
                    }

                }
                else if (shape.type === google.maps.drawing.OverlayType.POLYGON) {

                    //Verifica se shape.getPath existe e é uma função
                    if (typeof shape.getPath === "function" && shape.getPath()) {
                        newshape = new ShapePolygon();
                        newshape.zIndex = shape.zIndex;
                        newshape.fillColor = shape.fillColor;
                        newshape.paths = shape.getPath().getArray();
                        listShapesJson.push(newshape);
                    }
                    
                }
                else if (shape.type === google.maps.drawing.OverlayType.MARKER) {
                    newshape = new ShapeMarker();
                    newshape.position = shape.position;
                    newshape.icon = shape.icon;
                    listShapesJson.push(newshape);
                }
            }
        }

        return listShapesJson.length > 0 ? JSON.stringify(listShapesJson) : "";

    };


    var createCollorPalette = function (divMap) {
        if ((divColorPalette == null) || (divColorPalette === undefined)) {
            var createDivColorPalette = function () {
                divColorPalette = document.createElement('div');
                divColorPalette.id = "_divColorPalette_";
                divMap.appendChild(divColorPalette);
            }();

            var setStyleCollorPanel = function () {
                divColorPalette.style.clear = "both";
                divColorPalette.style.margin = "6px";
                divColorPalette.style.left = "170px";
                divColorPalette.style.height = "22px";
                divColorPalette.style.width = "280px";
            }();

            buildPalette();
        }

        divColorPalette.style.display = 'block';

    }

    this.setDrawingMode = function (mode) {
        if (drawingManager !== null && drawingManager !== undefined) {
            drawingManager.setDrawingMode(mode);
        }
    }

    this.ShowDrawPalette = function (divMap) {
        exibirControles = true;
        shapeDraggable = true
        createDrawingManager();

        createCollorPalette(divMap);

        createGoogleEvents();
    }

    this.HideDrawPalette = function () {
        if ((divColorPalette !== null) && (divColorPalette !== undefined))
            divColorPalette.style.display = 'none';

        if (drawingManager !== null && drawingManager !== undefined)
            drawingManager.setOptions({ drawingControl: false });

    }

    //this.setCenter(position) {
    //    _map.setCenter(position);
    //}

    this.setJson = function (jsonString, backgroundOnly, ignoreMarker) {

        if (!backgroundOnly)
            backgroundOnly = false;

        if ((!jsonString) || (jsonString === ""))
            return;

        var listShapesJson = JSON.parse(jsonString);
        for (var i = 0; i < listShapesJson.length; i++) {
            var shape = listShapesJson[i];

            // Evita erro caso o item seja null, undefined ou não seja objeto
            if (!shape || typeof shape !== "object") 
                continue;

            if (ignoreMarker === true && shape.type == 'marker')
                continue;

            this.addShape(shape, backgroundOnly);
        }
    };

    var obterIcon = function (svg) {
        return 'data:image/svg+xml,' + encodeURIComponent(svg);
    };

    this.icons = new function () {

        this.tool = function (color) {
            var svg =
                ' <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="30" viewBox="0 0 192 192" style=" fill:#000000;">' +
                '    <g fill="none" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt" stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0" font-family="none" font-weight="none" font-size="none" text-anchor="none" style="mix-blend-mode: normal">' +
                '        <path d="M0,192v-192h192v192z" fill="none"></path>' +
                '        <g fill="' + color + '">' +
                '            <g id="surface1">' +
                '                <path d="M49.92,0c-0.855,0 -1.695,0.285 -2.4,0.84l-37.32,29.88h79.44l-37.32,-29.88c-0.705,-0.555 -1.545,-0.84 -2.4,-0.84zM7.68,38.4v122.88h84.48v-122.88zM179.04,46.08c-0.48,0.09 -1.005,0.315 -1.44,0.6l-77.76,50.04v9.12l7.68,-4.92l-2.4,10.8l-5.28,3.36v9.12l57.36,-36.84l33,-21.6c0.855,-0.555 1.47,-1.395 1.68,-2.4c0.21,-1.005 0.09,-2.025 -0.48,-2.88l-8.52,-12.72c-0.87,-1.305 -2.385,-1.935 -3.84,-1.68zM30.72,53.76h38.4v34.56h-38.4zM178.56,55.2l4.2,6.24l-9.6,6.36l2.28,-10.56zM165.6,63.6l-2.16,10.44l-9.84,6.48l2.28,-10.68zM146.28,75.96l-2.28,10.8l-9.72,6.12l2.4,-10.68zM127.08,88.32l-2.28,10.68l-10.08,6.48l2.4,-10.8zM3.84,168.96c-2.115,0 -3.84,1.71 -3.84,3.84v15.36c0,2.13 1.725,3.84 3.84,3.84h92.16c2.115,0 3.84,-1.71 3.84,-3.84v-15.36c0,-2.13 -1.725,-3.84 -3.84,-3.84z"></path>' +
                '            </g>' +
                '        </g>' +
                '    </g>' +
                ' </svg>';


            return obterIcon(svg);
        };

        var truckDefault = function () {
            //var svg =
            //    '<svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="20" viewBox="0 0 192 192" style=" fill:#000000;">' +
            //    '  <g fill="none" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt" stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0" font-family="none" font-weight="none" font-size="none" text-anchor="none" style="mix-blend-mode: normal">' +
            //    '    <path d="M0,192v-192h192v192z" fill="none"></path >' +
            //    '    <g>' +
            //    '      <g id="surface1">' +
            //    '        <path d="M172,144h-56v-88h42.23438c3.4375,0 6.5,2.20312 7.59375,5.46875l14.17188,42.53125v32c0,4.42188 -3.57812,8 -8,8" fill="#1abc9c"></path>' +
            //    '        <path d="M116,144h-96c-4.42188,0 -8,-3.57812 -8,-8v-100c0,-4.42188 3.57812,-8 8,-8h88c4.42188,0 8,3.57812 8,8z" fill="#009688"></path>' +
            //    '        <path d="M168,144c0,11.04688 -8.95312,20 -20,20c-11.04688,0 -20,-8.95312 -20,-20c0,-11.04688 8.95312,-20 20,-20c11.04688,0 20,8.95312 20,20" fill="#5d4037"></path>' +
            //    '        <path d="M72,144c0,11.04688 -8.95312,20 -20,20c-11.04688,0 -20,-8.95312 -20,-20c0,-11.04688 8.95312,-20 20,-20c11.04688,0 20,8.95312 20,20" fill="#5d4037"></path>' +
            //    '        <path d="M156,144c0,4.42188 -3.57812,8 -8,8c-4.42188,0 -8,-3.57812 -8,-8c0,-4.42188 3.57812,-8 8,-8c4.42188,0 8,3.57812 8,8" fill="#bcaaa4"></path>' +
            //    '        <path d="M60,144c0,4.42188 -3.57812,8 -8,8c-4.42188,0 -8,-3.57812 -8,-8c0,-4.42188 3.57812,-8 8,-8c4.42188,0 8,3.57812 8,8" fill="#bcaaa4"></path>' +
            //    '        <path d="M164,100h-28c-2.20312,0 -4,-1.79688 -4,-4v-28c0,-2.20312 1.79688,-4 4,-4h20c1.73438,0 3.25,1.09375 3.79688,2.73438l8,24c0.125,0.40625 0.20312,0.84375 0.20312,1.26562v4c0,2.20312 -1.79688,4 -4,4" fill="#795548"></path>' +
            //    '        <path d="M12,68h104v32h-104z" fill="#4db6ac"></path>' +
            //    '      </g>' +
            //    '    </g >' +
            //    '  </g >' +
            //    '</svg > ';

            var svg =
                '<svg width="40" height="48" viewBox="0 0 150 266" fill="none" xmlns="http://www.w3.org/2000/svg" style="transform: rotate(' + randomNumber(10, 360) + 'deg);">' +
                '   <g filter = "url(#filter0_d_78:51)" >' +
                '       <rect x="28.5581" y="28.166" width="74.3655" height="49.7819" fill="black"></rect>' +
                '       <path fill-rule="evenodd" clip-rule="evenodd" d="M65.5357 16.8625C65.5357 16.8625 37.6121 17.3884 33.3389 19.1647C30.7581 20.2376 30.8044 19.1869 28.2659 23.3934C26.4043 26.5309 27.152 43.4644 27.152 43.4644C24.0707 43.4644 16.8809 45.5186 16.8809 46.5458C16.8809 46.9176 16.8809 47.5729 17.908 47.5729C18.7541 47.5729 26.1249 45.5186 27.152 45.5186L27.152 83.522L22.0164 92.2525L27.152 100.983L105.213 100.983L110.348 92.2525L105.213 83.522L105.213 45.5186C105.213 45.5186 113.738 47.5729 114.457 47.5729C115.43 47.5729 116.361 47.171 114.457 46.0322C112.553 44.8934 107.267 43.4644 105.213 43.4644L104.121 25.6705C104.121 25.6705 103.188 22.6496 101.71 20.2115C98.2564 17.7177 65.5357 16.8625 65.5357 16.8625ZM75.309 31.4378C82.8823 31.9724 89.9055 33.0647 95.6595 34.552C97.5634 35.0633 97.6903 35.1563 98.1557 36.3881C98.8326 38.1776 98.4095 43.6624 97.2672 47.3576C96.4633 50.0768 93.9671 56.4447 93.671 56.6306C84.9977 55.6777 74.759 54.9341 65.5357 54.9341C56.4394 54.9341 44.5369 56.0137 37.9367 56.92C37.0483 55.1305 34.6928 48.5429 33.9735 45.9632C33.1274 42.8257 32.9581 36.667 33.7197 35.6676C34.8197 34.2499 46.4122 32.0886 57.074 31.3449C61.4741 31.0195 70.4436 31.066 75.309 31.4378ZM36.3961 58.8711L36.3428 75.305C36.2159 75.305 30.2334 74.2779 30.2334 74.2779L30.2334 42.4373C30.2757 42.4838 34.746 54.1301 36.3961 58.8711ZM102.132 74.2779L95.9688 75.305L95.9688 58.8711L102.131 42.4373L102.132 74.2779Z" fill="#176b2b"></path>' +
                '       <path d="M22.0166 99.6374C22.0166 94.7081 27.5349 90.7121 34.342 90.7121L98.0232 90.7121C104.83 90.7121 110.349 94.7081 110.349 99.6374L110.349 232.773C110.349 237.702 104.83 241.698 98.0232 241.698L34.342 241.698C27.5349 241.698 22.0166 237.702 22.0166 232.773L22.0166 99.6374Z" fill="#6b5206"></path>' +
                '       <line x1="35.3691" y1="209.357" x2="96.9961" y2="209.357" stroke="black"></line>' +
                '       <line x1="35.3687" y1="165.191" x2="96.9956" y2="165.191" stroke="black"></line>' +
                '       <line x1="35.3687" y1="121.025" x2="96.9956" y2="121.025" stroke="black"></line>' +
                '       <rect x="49.7485" y="81.4683" width="15.4067" height="29.7864" rx="6" transform="rotate(-90 49.7485 81.4683)" fill="white"></rect>' +
                '   </g>' +
                '<defs>' +
                '   <filter id="filter0_d_78:51" x="0.880859" y="0.862305" width="138.682" height="264.836" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">' +
                '       <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>' +
                '       <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>' +
                '       <feOffset dx="4" dy="4"></feOffset>' +
                '       <feGaussianBlur stdDeviation="10"></feGaussianBlur>' +
                '       <feComposite in2="hardAlpha" operator="out"></feComposite>' +
                '       <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.5 0"></feColorMatrix>' +
                '       <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_78:51"></feBlend>' +
                '       <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_78:51" result="shape"></feBlend>' +
                '   </filter>' +
                '</defs>' +
                '</svg> '

            return obterIcon(svg);
        };

        this.truck = function (color) {

            if (color == undefined)
                return truckDefault();

            //var svg =
            //    '<svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="20" viewBox="0 0 192 192" style=" fill:#000000;">' +
            //    '  <g fill="none" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt" stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0" font-family="none" font-weight="none" font-size="none" text-anchor="none" style="mix-blend-mode: normal">' +
            //    '    <path d="M0,192v-192h192v192z" fill="none"></path>' +
            //    '    <g>' +
            //    '      <g id="surface1">' +
            //    '        <path d="M172,144h-56v-88h42.23438c3.4375,0 6.5,2.20312 7.59375,5.46875l14.17188,42.53125v32c0,4.42188 -3.57812,8 -8,8" fill="' + color + '"></path>' +
            //    '        <path d="M116,144h-96c-4.42188,0 -8,-3.57812 -8,-8v-100c0,-4.42188 3.57812,-8 8,-8h88c4.42188,0 8,3.57812 8,8z" fill="' + color + '"></path>' +
            //    '        <path d="M168,144c0,11.04688 -8.95312,20 -20,20c-11.04688,0 -20,-8.95312 -20,-20c0,-11.04688 8.95312,-20 20,-20c11.04688,0 20,8.95312 20,20" fill="#5d4037"></path>' +
            //    '        <path d="M72,144c0,11.04688 -8.95312,20 -20,20c-11.04688,0 -20,-8.95312 -20,-20c0,-11.04688 8.95312,-20 20,-20c11.04688,0 20,8.95312 20,20" fill="#5d4037"></path>' +
            //    '        <path d="M156,144c0,4.42188 -3.57812,8 -8,8c-4.42188,0 -8,-3.57812 -8,-8c0,-4.42188 3.57812,-8 8,-8c4.42188,0 8,3.57812 8,8" fill="#bcaaa4"></path>' +
            //    '        <path d="M60,144c0,4.42188 -3.57812,8 -8,8c-4.42188,0 -8,-3.57812 -8,-8c0,-4.42188 3.57812,-8 8,-8c4.42188,0 8,3.57812 8,8" fill="#bcaaa4"></path>' +
            //    '        <path d="M164,100h-28c-2.20312,0 -4,-1.79688 -4,-4v-28c0,-2.20312 1.79688,-4 4,-4h20c1.73438,0 3.25,1.09375 3.79688,2.73438l8,24c0.125,0.40625 0.20312,0.84375 0.20312,1.26562v4c0,2.20312 -1.79688,4 -4,4" fill="#795548"></path>' +
            //    '        <path d="M12,68h104v32h-104z" fill="' + color + '"></path>' +
            //    '      </g>' +
            //    '    </g>' +
            //    '  </g>' +
            //    '</svg>';

            var svg =
                '<svg width="40" height="48" viewBox="0 0 150 266" fill="none" xmlns="http://www.w3.org/2000/svg" style="transform: rotate(' + randomNumber(10, 360) + 'deg);">' +
                '   <g filter = "url(#filter0_d_78:51)" >' +
                '       <rect x="28.5581" y="28.166" width="74.3655" height="49.7819" fill="black"></rect>' +
                '       <path fill-rule="evenodd" clip-rule="evenodd" d="M65.5357 16.8625C65.5357 16.8625 37.6121 17.3884 33.3389 19.1647C30.7581 20.2376 30.8044 19.1869 28.2659 23.3934C26.4043 26.5309 27.152 43.4644 27.152 43.4644C24.0707 43.4644 16.8809 45.5186 16.8809 46.5458C16.8809 46.9176 16.8809 47.5729 17.908 47.5729C18.7541 47.5729 26.1249 45.5186 27.152 45.5186L27.152 83.522L22.0164 92.2525L27.152 100.983L105.213 100.983L110.348 92.2525L105.213 83.522L105.213 45.5186C105.213 45.5186 113.738 47.5729 114.457 47.5729C115.43 47.5729 116.361 47.171 114.457 46.0322C112.553 44.8934 107.267 43.4644 105.213 43.4644L104.121 25.6705C104.121 25.6705 103.188 22.6496 101.71 20.2115C98.2564 17.7177 65.5357 16.8625 65.5357 16.8625ZM75.309 31.4378C82.8823 31.9724 89.9055 33.0647 95.6595 34.552C97.5634 35.0633 97.6903 35.1563 98.1557 36.3881C98.8326 38.1776 98.4095 43.6624 97.2672 47.3576C96.4633 50.0768 93.9671 56.4447 93.671 56.6306C84.9977 55.6777 74.759 54.9341 65.5357 54.9341C56.4394 54.9341 44.5369 56.0137 37.9367 56.92C37.0483 55.1305 34.6928 48.5429 33.9735 45.9632C33.1274 42.8257 32.9581 36.667 33.7197 35.6676C34.8197 34.2499 46.4122 32.0886 57.074 31.3449C61.4741 31.0195 70.4436 31.066 75.309 31.4378ZM36.3961 58.8711L36.3428 75.305C36.2159 75.305 30.2334 74.2779 30.2334 74.2779L30.2334 42.4373C30.2757 42.4838 34.746 54.1301 36.3961 58.8711ZM102.132 74.2779L95.9688 75.305L95.9688 58.8711L102.131 42.4373L102.132 74.2779Z" fill="#707070"></path>' +
                '       <path d="M22.0166 99.6374C22.0166 94.7081 27.5349 90.7121 34.342 90.7121L98.0232 90.7121C104.83 90.7121 110.349 94.7081 110.349 99.6374L110.349 232.773C110.349 237.702 104.83 241.698 98.0232 241.698L34.342 241.698C27.5349 241.698 22.0166 237.702 22.0166 232.773L22.0166 99.6374Z" fill="' + color + '"></path>' +
                '       <line x1="35.3691" y1="209.357" x2="96.9961" y2="209.357" stroke="black"></line>' +
                '       <line x1="35.3687" y1="165.191" x2="96.9956" y2="165.191" stroke="black"></line>' +
                '       <line x1="35.3687" y1="121.025" x2="96.9956" y2="121.025" stroke="black"></line>' +
                '       <rect x="49.7485" y="81.4683" width="15.4067" height="29.7864" rx="6" transform="rotate(-90 49.7485 81.4683)" fill="white"></rect>' +
                '   </g>' +
                '<defs>' +
                '   <filter id="filter0_d_78:51" x="0.880859" y="0.862305" width="138.682" height="264.836" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">' +
                '       <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>' +
                '       <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>' +
                '       <feOffset dx="4" dy="4"></feOffset>' +
                '       <feGaussianBlur stdDeviation="10"></feGaussianBlur>' +
                '       <feComposite in2="hardAlpha" operator="out"></feComposite>' +
                '       <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.5 0"></feColorMatrix>' +
                '       <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_78:51"></feBlend>' +
                '       <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_78:51" result="shape"></feBlend>' +
                '   </filter>' +
                '</defs>' +
                '</svg> '

            return obterIcon(svg);
        };

        this.truckSignal = function (width, height, color, signal) {

            if (color == undefined)
                return truckDefault();

            var colorSignal;
            if (signal)
                colorSignal = '#79dd79';
            else
                colorSignal = '#FF0000';

            //var svg =
            //    '<svg xmlns="http://www.w3.org/2000/svg" id="svg33" viewBox="0 0 192 192" height="' + height + '" width="' + width + '" y="0px" x="0px">' +
            //    '  <g id="g1118">' +
            //    '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" id="path10" fill="none" d="M 0,192 V 0 h 192 v 192 z" />' +
            //    '    <g style="font-family:none;mix-blend-mode:normal;fill:none;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" transform="translate(0,19.2)" id="g29">' +
            //    '      <g id="surface1">' +
            //    '        <path id="path12" fill="' + color + '" d="M 172,144 H 116 V 56 h 42.23438 c 3.4375,0 6.5,2.20312 7.59375,5.46875 L 180.00001,104 v 32 c 0,4.42188 -3.57812,8 -8,8" />' +
            //    '        <path id="path14" fill="' + color + '" d="M 116,144 H 20 c -4.42188,0 -8,-3.57812 -8,-8 V 36 c 0,-4.42188 3.57812,-8 8,-8 h 88 c 4.42188,0 8,3.57812 8,8 z" />' +
            //    '        <path id="path16" fill="#5d4037" d="m 168,144 c 0,11.04688 -8.95312,20 -20,20 -11.04688,0 -20,-8.95312 -20,-20 0,-11.04688 8.95312,-20 20,-20 11.04688,0 20,8.95312 20,20" />' +
            //    '        <path id="path18" fill="#5d4037" d="m 72,144 c 0,11.04688 -8.95312,20 -20,20 -11.04688,0 -20,-8.95312 -20,-20 0,-11.04688 8.95312,-20 20,-20 11.04688,0 20,8.95312 20,20" />' +
            //    '        <path id="path20" fill="#bcaaa4" d="m 156,144 c 0,4.42188 -3.57812,8 -8,8 -4.42188,0 -8,-3.57812 -8,-8 0,-4.42188 3.57812,-8 8,-8 4.42188,0 8,3.57812 8,8" />' +
            //    '        <path id="path22" fill="#bcaaa4" d="m 60,144 c 0,4.42188 -3.57812,8 -8,8 -4.42188,0 -8,-3.57812 -8,-8 0,-4.42188 3.57812,-8 8,-8 4.42188,0 8,3.57812 8,8" />' +
            //    '        <path id="path24" fill="#795548" d="m 164,100 h -28 c -2.20312,0 -4,-1.79688 -4,-4 V 68 c 0,-2.20312 1.79688,-4 4,-4 h 20 c 1.73438,0 3.25,1.09375 3.79688,2.73438 l 8,24 C 167.92188,91.14063 168,91.57813 168,92 v 4 c 0,2.20312 -1.79688,4 -4,4" />' +
            //    '        <path id="path26" fill="' + color + '" d="m 12,68 h 104 v 32 H 12 Z" />' +
            //    '      </g>' +
            //    '    </g>' +
            //    '    <path d="m 169.13977,38.312582 c -0.99669,0 -1.97872,-0.381076 -2.74091,-1.143258 -10.46528,-10.45065 -27.46772,-10.45065 -37.93303,0 -1.50969,1.509697 -3.97211,1.509697 -5.48181,0 -1.5097,-1.509706 -1.5097,-3.972131 0,-5.481819 13.48469,-13.484704 35.41199,-13.484704 48.89666,0 1.50971,1.509688 1.50971,3.972113 0,5.481819 -0.76216,0.762182 -1.75887,1.143258 -2.74091,1.143258 z" fill="' + colorSignal + '" />' +
            //    '    <path d="m 158.64517,48.792534 c -0.99669,0 -1.97873,-0.381076 -2.7409,-1.143258 -2.25723,-2.257227 -5.27663,-3.503096 -8.47192,-3.503096 -3.19528,0 -6.21468,1.245869 -8.47191,3.503096 -1.5097,1.509697 -3.97211,1.509697 -5.48181,0 -1.5097,-1.509705 -1.5097,-3.97213 0,-5.481819 3.72294,-3.722961 8.67711,-5.774968 13.95372,-5.774968 5.27662,0 10.23078,2.052007 13.95372,5.789612 1.50969,1.509704 1.50969,3.97213 0,5.481834 -0.74751,0.747523 -1.74419,1.128599 -2.7409,1.128599 z" fill="' + colorSignal + '" />' +
            //    '    <circle cx="147.43234" cy="56.135826" r="5.1740189" fill="' + colorSignal + '" />' +
            //    '    <path d="m 179.79561,27.656751 c -0.99668,0 -1.97871,-0.381091 -2.7409,-1.143273 -16.34287,-16.328203 -42.90184,-16.328203 -59.23006,0 -1.50969,1.509704 -3.97211,1.509704 -5.48182,0 -1.5097,-1.509705 -1.5097,-3.972115 0,-5.48182 9.38067,-9.38065 21.83935,-14.5400088 35.10418,-14.5400088 13.26483,0 25.72351,5.1593588 35.10417,14.5400088 1.50969,1.509705 1.50969,3.972115 0,5.48182 -0.77684,0.762182 -1.77354,1.143273 -2.75557,1.143273 z" fill="' + colorSignal + '" />' +
            //    '  </g>' +
            //    '</svg>';


            var svg =
                '<svg width="40" height="48" viewBox="0 0 150 266" fill="none" xmlns="http://www.w3.org/2000/svg" style="transform: rotate(' + randomNumber(10, 360) + 'deg);">' +
                ' <g id="g1118">' +
                '    <path d="m 169.13977,38.312582 c -0.99669,0 -1.97872,-0.381076 -2.74091,-1.143258 -10.46528,-10.45065 -27.46772,-10.45065 -37.93303,0 -1.50969,1.509697 -3.97211,1.509697 -5.48181,0 -1.5097,-1.509706 -1.5097,-3.972131 0,-5.481819 13.48469,-13.484704 35.41199,-13.484704 48.89666,0 1.50971,1.509688 1.50971,3.972113 0,5.481819 -0.76216,0.762182 -1.75887,1.143258 -2.74091,1.143258 z" fill="' + colorSignal + '" />' +
                '    <path d="m 158.64517,48.792534 c -0.99669,0 -1.97873,-0.381076 -2.7409,-1.143258 -2.25723,-2.257227 -5.27663,-3.503096 -8.47192,-3.503096 -3.19528,0 -6.21468,1.245869 -8.47191,3.503096 -1.5097,1.509697 -3.97211,1.509697 -5.48181,0 -1.5097,-1.509705 -1.5097,-3.97213 0,-5.481819 3.72294,-3.722961 8.67711,-5.774968 13.95372,-5.774968 5.27662,0 10.23078,2.052007 13.95372,5.789612 1.50969,1.509704 1.50969,3.97213 0,5.481834 -0.74751,0.747523 -1.74419,1.128599 -2.7409,1.128599 z" fill="' + colorSignal + '" />' +
                '    <circle cx="147.43234" cy="56.135826" r="5.1740189" fill="' + colorSignal + '" />' +
                '    <path d="m 179.79561,27.656751 c -0.99668,0 -1.97871,-0.381091 -2.7409,-1.143273 -16.34287,-16.328203 -42.90184,-16.328203 -59.23006,0 -1.50969,1.509704 -3.97211,1.509704 -5.48182,0 -1.5097,-1.509705 -1.5097,-3.972115 0,-5.48182 9.38067,-9.38065 21.83935,-14.5400088 35.10418,-14.5400088 13.26483,0 25.72351,5.1593588 35.10417,14.5400088 1.50969,1.509705 1.50969,3.972115 0,5.48182 -0.77684,0.762182 -1.77354,1.143273 -2.75557,1.143273 z" fill="' + colorSignal + '" />' +
                '    <g filter="url(#filter0_d_78:51)">' +
                '       <rect x="28.5581" y="28.166" width="74.3655" height="49.7819" fill="black"></rect>' +
                '       <path fill-rule="evenodd" clip-rule="evenodd" d="M65.5357 16.8625C65.5357 16.8625 37.6121 17.3884 33.3389 19.1647C30.7581 20.2376 30.8044 19.1869 28.2659 23.3934C26.4043 26.5309 27.152 43.4644 27.152 43.4644C24.0707 43.4644 16.8809 45.5186 16.8809 46.5458C16.8809 46.9176 16.8809 47.5729 17.908 47.5729C18.7541 47.5729 26.1249 45.5186 27.152 45.5186L27.152 83.522L22.0164 92.2525L27.152 100.983L105.213 100.983L110.348 92.2525L105.213 83.522L105.213 45.5186C105.213 45.5186 113.738 47.5729 114.457 47.5729C115.43 47.5729 116.361 47.171 114.457 46.0322C112.553 44.8934 107.267 43.4644 105.213 43.4644L104.121 25.6705C104.121 25.6705 103.188 22.6496 101.71 20.2115C98.2564 17.7177 65.5357 16.8625 65.5357 16.8625ZM75.309 31.4378C82.8823 31.9724 89.9055 33.0647 95.6595 34.552C97.5634 35.0633 97.6903 35.1563 98.1557 36.3881C98.8326 38.1776 98.4095 43.6624 97.2672 47.3576C96.4633 50.0768 93.9671 56.4447 93.671 56.6306C84.9977 55.6777 74.759 54.9341 65.5357 54.9341C56.4394 54.9341 44.5369 56.0137 37.9367 56.92C37.0483 55.1305 34.6928 48.5429 33.9735 45.9632C33.1274 42.8257 32.9581 36.667 33.7197 35.6676C34.8197 34.2499 46.4122 32.0886 57.074 31.3449C61.4741 31.0195 70.4436 31.066 75.309 31.4378ZM36.3961 58.8711L36.3428 75.305C36.2159 75.305 30.2334 74.2779 30.2334 74.2779L30.2334 42.4373C30.2757 42.4838 34.746 54.1301 36.3961 58.8711ZM102.132 74.2779L95.9688 75.305L95.9688 58.8711L102.131 42.4373L102.132 74.2779Z" fill="#707070"></path>' +
                '       <path d="M22.0166 99.6374C22.0166 94.7081 27.5349 90.7121 34.342 90.7121L98.0232 90.7121C104.83 90.7121 110.349 94.7081 110.349 99.6374L110.349 232.773C110.349 237.702 104.83 241.698 98.0232 241.698L34.342 241.698C27.5349 241.698 22.0166 237.702 22.0166 232.773L22.0166 99.6374Z" fill="' + color + '"></path>' +
                '       <line x1="35.3691" y1="209.357" x2="96.9961" y2="209.357" stroke="black"></line>' +
                '       <line x1="35.3687" y1="165.191" x2="96.9956" y2="165.191" stroke="black"></line>' +
                '       <line x1="35.3687" y1="121.025" x2="96.9956" y2="121.025" stroke="black"></line>' +
                '       <rect x="49.7485" y="81.4683" width="15.4067" height="29.7864" rx="6" transform="rotate(-90 49.7485 81.4683)" fill="white"></rect>' +
                '    </g>' +
                ' </g>' +
                ' <defs>' +
                ' <filter id="filter0_d_78:51" x="0.880859" y="0.862305" width="138.682" height="264.836" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">' +
                '   <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>' +
                '   <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>' +
                '   <feOffset dx="4" dy="4"></feOffset>' +
                '   <feGaussianBlur stdDeviation="10"></feGaussianBlur>' +
                '   <feComposite in2="hardAlpha" operator="out"></feComposite>' +
                '   <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.5 0"></feColorMatrix>' +
                '   <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_78:51"></feBlend>' +
                '   <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_78:51" result="shape"></feBlend>' +
                ' </filter>' +
                ' </defs>' +
                ' </svg>'



            return obterIcon(svg);
        };

        this.truckTrailerSignal = function (width, height, color, signal) {
            if (color == undefined) color = '#00FF00';
            var colorSignal;
            if (signal) colorSignal = '#79dd79';
            else colorSignal = '#FF0000';

            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="' + width + '" height="' + height + '" viewBox="0 0 192 192" id="svg33">' +
                '  <g id="g1118">' +
                '    <path d="M 0,192 V 0 h 192 v 192 z" fill="none" id="path10" style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" />' +
                '    <g id="g29" transform="translate(0,19.2)" style="font-family:none;mix-blend-mode:normal;fill:none;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0">' +
                '      <g id="surface1">' +
                '        <path c transform="matrix(9.6,0,0,9.6,0,-19.2)" d="M 12.083984 13.833984 L 12.083984 17 L 17.916016 17 C 18.376628 17 18.75 16.626628 18.75 16.166016 L 18.75 14.666016 C 18.75 14.205403 18.376628 13.833984 17.916016 13.833984 L 12.083984 13.833984 z " style="stroke-width:0.104167" id="path12" />' +
                '        <path fill="' + color + '" transform="matrix(9.6,0,0,9.6,0,-19.2)" d="M 2.0839844,4.9160156 C 1.6233719,4.9160156 1.25,5.2893875 1.25,5.75 V 16.166016 C 1.25,16.626628 1.6233719,17 2.0839844,17 H 17.916016 C 18.376628,17 18.75,16.626628 18.75,16.166016 V 5.75 c 0,-0.4606125 -0.373372,-0.8339844 -0.833984,-0.8339844 z" style="stroke-width:0.104167" id="path14" />' +
                '        <path d="m 116.00625,144 c 0,11.04688 -8.95312,20 -20.000004,20 -11.04688,0 -20,-8.95312 -20,-20 0,-11.04688 8.95312,-20 20,-20 11.046884,0 20.000004,8.95312 20.000004,20" fill="#5d4037" id="path16" />' +
                '        <path d="m 72,144 c 0,11.04688 -8.95312,20 -20,20 -11.04688,0 -20,-8.95312 -20,-20 0,-11.04688 8.95312,-20 20,-20 11.04688,0 20,8.95312 20,20" fill="#5d4037" id="path18" />' +
                '        <path d="m 104.00625,144 c 0,4.42188 -3.57812,8 -8.000004,8 -4.42188,0 -8,-3.57812 -8,-8 0,-4.42188 3.57812,-8 8,-8 4.421884,0 8.000004,3.57812 8.000004,8" fill="#bcaaa4" id="path20" />' +
                '        <path d="m 60,144 c 0,4.42188 -3.57812,8 -8,8 -4.42188,0 -8,-3.57812 -8,-8 0,-4.42188 3.57812,-8 8,-8 4.42188,0 8,3.57812 8,8" fill="#bcaaa4" id="path22" />' +
                '        <path fill="' + color + '" transform="matrix(9.6,0,0,9.6,0,-19.2)" d="M 1.25 9.0839844 L 1.25 12.416016 L 18.75 12.416016 L 18.75 9.0839844 L 1.25 9.0839844 z " style="stroke-width:0.104167" id="path26" />' +
                '      </g>' +
                '    </g>' +
                '    <path id="path21" fill="' + colorSignal + '" d="m 169.13977,38.312582 c -0.99669,0 -1.97872,-0.381076 -2.74091,-1.143258 -10.46528,-10.45065 -27.46772,-10.45065 -37.93303,0 -1.50969,1.509697 -3.97211,1.509697 -5.48181,0 -1.5097,-1.509706 -1.5097,-3.972131 0,-5.481819 13.48469,-13.484704 35.41199,-13.484704 48.89666,0 1.50971,1.509688 1.50971,3.972113 0,5.481819 -0.76216,0.762182 -1.75887,1.143258 -2.74091,1.143258 z" />' +
                '    <path id="path23" fill="' + colorSignal + '" d="m 158.64517,48.792534 c -0.99669,0 -1.97873,-0.381076 -2.7409,-1.143258 -2.25723,-2.257227 -5.27663,-3.503096 -8.47192,-3.503096 -3.19528,0 -6.21468,1.245869 -8.47191,3.503096 -1.5097,1.509697 -3.97211,1.509697 -5.48181,0 -1.5097,-1.509705 -1.5097,-3.97213 0,-5.481819 3.72294,-3.722961 8.67711,-5.774968 13.95372,-5.774968 5.27662,0 10.23078,2.052007 13.95372,5.789612 1.50969,1.509704 1.50969,3.97213 0,5.481834 -0.74751,0.747523 -1.74419,1.128599 -2.7409,1.128599 z" />' +
                '    <circle id="circle25" fill="' + colorSignal + '" r="5.1740189" cy="56.135826" cx="147.43234" />' +
                '    <path id="path27" fill="' + colorSignal + '" d="m 179.79561,27.656751 c -0.99668,0 -1.97871,-0.381091 -2.7409,-1.143273 -16.34287,-16.328203 -42.90184,-16.328203 -59.23006,0 -1.50969,1.509704 -3.97211,1.509704 -5.48182,0 -1.5097,-1.509705 -1.5097,-3.972115 0,-5.48182 9.38067,-9.38065 21.83935,-14.5400088 35.10418,-14.5400088 13.26483,0 25.72351,5.1593588 35.10417,14.5400088 1.50969,1.509705 1.50969,3.972115 0,5.48182 -0.77684,0.762182 -1.77354,1.143273 -2.75557,1.143273 z" />' +
                '  </g>' +
                '</svg>';

            return obterIcon(svg);
        };

        this.trucknewCircule = function (width, height, color) {

            if (color == undefined || color == "") color = '#000000';

            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" width="' + width + '" height="' + height + '" viewBox="0 0 40 40" fill="none">' +
                '<circle cx="20" cy="20" r="19.5" fill="white" stroke="' + color + '"/>' +
                '<path d="M11 20H20V21.5H11V20ZM9.5 16.25H17V17.75H9.5V16.25Z" fill="' + color + '"/>' +
                '<path d="M30.4393 20.4545L28.1893 15.2045C28.1315 15.0696 28.0354 14.9545 27.9129 14.8737C27.7903 14.793 27.6468 14.7499 27.5 14.75H25.25V13.25C25.25 13.0511 25.171 12.8603 25.0303 12.7197C24.8897 12.579 24.6989 12.5 24.5 12.5H12.5V14H23.75V23.417C23.4083 23.6154 23.1092 23.8796 22.87 24.1942C22.6308 24.5088 22.4563 24.8676 22.3565 25.25H17.6435C17.461 24.543 17.0268 23.9269 16.4225 23.517C15.8182 23.1072 15.0851 22.9319 14.3608 23.0239C13.6364 23.1159 12.9705 23.4689 12.4878 24.0168C12.0051 24.5647 11.7388 25.2698 11.7388 26C11.7388 26.7302 12.0051 27.4353 12.4878 27.9832C12.9705 28.5311 13.6364 28.8841 14.3608 28.9761C15.0851 29.0681 15.8182 28.8928 16.4225 28.483C17.0268 28.0731 17.461 27.457 17.6435 26.75H22.3565C22.5197 27.3937 22.8928 27.9646 23.4168 28.3724C23.9409 28.7802 24.586 29.0016 25.25 29.0016C25.914 29.0016 26.5591 28.7802 27.0832 28.3724C27.6072 27.9646 27.9804 27.3937 28.1435 26.75H29.75C29.9489 26.75 30.1397 26.671 30.2803 26.5303C30.421 26.3897 30.5 26.1989 30.5 26V20.75C30.5 20.6484 30.4794 20.5479 30.4393 20.4545ZM14.75 27.5C14.4533 27.5 14.1633 27.412 13.9167 27.2472C13.67 27.0824 13.4777 26.8481 13.3642 26.574C13.2507 26.2999 13.221 25.9983 13.2788 25.7074C13.3367 25.4164 13.4796 25.1491 13.6893 24.9393C13.8991 24.7296 14.1664 24.5867 14.4574 24.5288C14.7483 24.4709 15.0499 24.5006 15.324 24.6142C15.5981 24.7277 15.8324 24.92 15.9972 25.1666C16.162 25.4133 16.25 25.7033 16.25 26C16.2496 26.3977 16.0914 26.779 15.8102 27.0602C15.529 27.3414 15.1477 27.4996 14.75 27.5ZM25.25 16.25H27.005L28.613 20H25.25V16.25ZM25.25 27.5C24.9533 27.5 24.6633 27.412 24.4167 27.2472C24.17 27.0824 23.9777 26.8481 23.8642 26.574C23.7507 26.2999 23.721 25.9983 23.7788 25.7074C23.8367 25.4164 23.9796 25.1491 24.1893 24.9393C24.3991 24.7296 24.6664 24.5867 24.9574 24.5288C25.2483 24.4709 25.5499 24.5006 25.824 24.6142C26.0981 24.7277 26.3324 24.92 26.4972 25.1666C26.662 25.4133 26.75 25.7033 26.75 26C26.7496 26.3977 26.5914 26.779 26.3102 27.0602C26.029 27.3414 25.6477 27.4996 25.25 27.5ZM29 25.25H28.1435C27.9783 24.6076 27.6046 24.0381 27.0809 23.6309C26.5573 23.2238 25.9133 23.0018 25.25 23V21.5H29V25.25Z" fill="' + color + '"/>' +
                '</svg>';

            return obterIcon(svg);
        };

        this.trucknewCirculeCargaCritica = function (width, height, color) {

            if (color == undefined || color == "") color = '#000000';


            var svg =
                '<svg width="' + width + '" height="' + height + '" viewBox="0 0 50 46" fill="none" xmlns="http://www.w3.org/2000/svg">' +
                '<circle cx="20" cy="26" r="19.5" fill="white" stroke="' + color + '"/>' +
                '<path d="M11 26H20V27.5H11V26ZM9.5 22.25H17V23.75H9.5V22.25Z" fill="' + color + '"/>' +
                '<path d="M30.4393 26.4545L28.1893 21.2045C28.1315 21.0696 28.0354 20.9545 27.9129 20.8737C27.7903 20.793 27.6468 20.7499 27.5 20.75H25.25V19.25C25.25 19.0511 25.171 18.8603 25.0303 18.7197C24.8897 18.579 24.6989 18.5 24.5 18.5H12.5V20H23.75V29.417C23.4083 29.6154 23.1092 29.8796 22.87 30.1942C22.6308 30.5088 22.4563 30.8676 22.3565 31.25H17.6435C17.461 30.543 17.0268 29.9269 16.4225 29.517C15.8182 29.1072 15.0851 28.9319 14.3608 29.0239C13.6364 29.1159 12.9705 29.4689 12.4878 30.0168C12.0051 30.5647 11.7388 31.2698 11.7388 32C11.7388 32.7302 12.0051 33.4353 12.4878 33.9832C12.9705 34.5311 13.6364 34.8841 14.3608 34.9761C15.0851 35.0681 15.8182 34.8928 16.4225 34.483C17.0268 34.0731 17.461 33.457 17.6435 32.75H22.3565C22.5197 33.3937 22.8928 33.9646 23.4168 34.3724C23.9409 34.7802 24.586 35.0016 25.25 35.0016C25.914 35.0016 26.5591 34.7802 27.0832 34.3724C27.6072 33.9646 27.9804 33.3937 28.1435 32.75H29.75C29.9489 32.75 30.1397 32.671 30.2803 32.5303C30.421 32.3897 30.5 32.1989 30.5 32V26.75C30.5 26.6484 30.4794 26.5479 30.4393 26.4545ZM14.75 33.5C14.4533 33.5 14.1633 33.412 13.9167 33.2472C13.67 33.0824 13.4777 32.8481 13.3642 32.574C13.2507 32.2999 13.221 31.9983 13.2788 31.7074C13.3367 31.4164 13.4796 31.1491 13.6893 30.9393C13.8991 30.7296 14.1664 30.5867 14.4574 30.5288C14.7483 30.4709 15.0499 30.5006 15.324 30.6142C15.5981 30.7277 15.8324 30.92 15.9972 31.1666C16.162 31.4133 16.25 31.7033 16.25 32C16.2496 32.3977 16.0914 32.779 15.8102 33.0602C15.529 33.3414 15.1477 33.4996 14.75 33.5ZM25.25 22.25H27.005L28.613 26H25.25V22.25ZM25.25 33.5C24.9533 33.5 24.6633 33.412 24.4167 33.2472C24.17 33.0824 23.9777 32.8481 23.8642 32.574C23.7507 32.2999 23.721 31.9983 23.7788 31.7074C23.8367 31.4164 23.9796 31.1491 24.1893 30.9393C24.3991 30.7296 24.6664 30.5867 24.9574 30.5288C25.2483 30.4709 25.5499 30.5006 25.824 30.6142C26.0981 30.7277 26.3324 30.92 26.4972 31.1666C26.662 31.4133 26.75 31.7033 26.75 32C26.7496 32.3977 26.5914 32.779 26.3102 33.0602C26.029 33.3414 25.6477 33.4996 25.25 33.5ZM29 31.25H28.1435C27.9783 30.6076 27.6046 30.0381 27.0809 29.6309C26.5573 29.2238 25.9133 29.0018 25.25 29V27.5H29V31.25Z" fill="' + color + '"/>' +
                '<circle cx="38" cy="12" r="11.25" fill="white" stroke="#FF4E4E" stroke-width="1.5"/>' +
                '<g clip-path="url(#clip0_2016_521)">' +
                '<path d="M38.625 13.9375C38.625 14.1033 38.5592 14.2622 38.4419 14.3794C38.3247 14.4967 38.1658 14.5625 38 14.5625C37.8342 14.5625 37.6753 14.4967 37.5581 14.3794C37.4408 14.2622 37.375 14.1033 37.375 13.9375C37.375 13.7717 37.4408 13.6128 37.5581 13.4956C37.6753 13.3783 37.8342 13.3125 38 13.3125C38.1658 13.3125 38.3247 13.3783 38.4419 13.4956C38.5592 13.6128 38.625 13.7717 38.625 13.9375ZM38.4687 8.78125C38.4687 8.65693 38.4194 8.5377 38.3315 8.44979C38.2435 8.36189 38.1243 8.3125 38 8.3125C37.8757 8.3125 37.7565 8.36189 37.6685 8.44979C37.5806 8.5377 37.5312 8.65693 37.5312 8.78125V11.5938C37.5312 11.7181 37.5806 11.8373 37.6685 11.9252C37.7565 12.0131 37.8757 12.0625 38 12.0625C38.1243 12.0625 38.2435 12.0131 38.3315 11.9252C38.4194 11.8373 38.4687 11.7181 38.4687 11.5938V8.78125Z" fill="#FF4E4E"/>' +
                '<path d="M36.6475 5.02751C37.2494 3.98688 38.7506 3.98688 39.3525 5.02751L44.9569 14.7175C45.5588 15.7594 44.8069 17.0625 43.6038 17.0625H32.3963C31.1925 17.0625 30.4413 15.7594 31.0431 14.7175L36.6475 5.02751ZM38.5413 5.49688C38.4862 5.4022 38.4072 5.32362 38.3122 5.26901C38.2172 5.21439 38.1096 5.18565 38 5.18565C37.8905 5.18565 37.7828 5.21439 37.6878 5.26901C37.5929 5.32362 37.5139 5.4022 37.4588 5.49688L31.855 15.1869C31.8004 15.282 31.7717 15.3897 31.7718 15.4994C31.7719 15.609 31.8008 15.7167 31.8556 15.8117C31.9104 15.9067 31.9892 15.9856 32.0841 16.0406C32.179 16.0956 32.2866 16.1247 32.3963 16.125H43.6038C43.7133 16.1246 43.8209 16.0955 43.9157 16.0406C44.0104 15.9856 44.0892 15.9068 44.1439 15.8119C44.1987 15.717 44.2277 15.6094 44.2278 15.4998C44.228 15.3903 44.1995 15.2826 44.145 15.1875L38.5413 5.49688Z" fill="#FF4E4E"/>' +
                '</g>' +
                '<defs>' +
                '<clipPath id="clip0_2016_521">' +
                '<rect width="15" height="15" fill="white" transform="translate(30.5 3)"/>' +
                '</clipPath>' +
                '</defs>' +
                '</svg>';


            return obterIcon(svg);
        };

        this.truckStatusViagemMonitoramentoEmCarregamento = function (width, height, color) {

            if (color == undefined || color == "") color = 'red';


            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" width="' + width + '" height="' + height + '" viewBox="0 0 40 40" fill="none">' +
                ' <circle cx="20" cy="20" r="19.5" fill="white" stroke="' + color + '" />' +
                '<g transform="translate(8.5, 8)">' +
                '<g clip-path="url(#clip0)">' +
                '<g clip-path="url(#clip0_2270_618)"><path d="M20 8H17.3333V4.66667C17.3333 4.48986 17.2631 4.32029 17.1381 4.19526C17.0131 4.07024 16.8435 4 16.6667 4H2.00001C1.8232 4 1.65363 4.07024 1.52861 4.19526C1.40358 4.32029 1.33334 4.48986 1.33334 4.66667V16.6667C1.33334 16.8435 1.40358 17.013 1.52861 17.1381C1.65363 17.2631 1.8232 17.3333 2.00001 17.3333H2.66668V5.33333H16V13.1333C16.3399 13.0466 16.6892 13.0019 17.04 13H17.3333V9.33333H20C20.3536 9.33333 20.6928 9.47381 20.9428 9.72386C21.1929 9.97391 21.3333 10.313 21.3333 10.6667V11.3333H18.6667V12.6667H21.3333V16H19.7333C19.4995 15.4567 19.1115 14.9939 18.6174 14.6687C18.1234 14.3434 17.5448 14.1701 16.9533 14.1701C16.3619 14.1701 15.7833 14.3434 15.2893 14.6687C14.7952 14.9939 14.4072 15.4567 14.1733 16H9.62001C9.39056 15.4527 9.00397 14.9858 8.50912 14.6583C8.01428 14.3308 7.43342 14.1574 6.84001 14.16C6.27619 14.1724 5.72804 14.3479 5.26178 14.6651C4.79553 14.9824 4.43111 15.4279 4.21257 15.9477C3.99403 16.4676 3.93072 17.0397 4.03028 17.5948C4.12984 18.1499 4.38801 18.6643 4.77361 19.0758C5.1592 19.4873 5.65572 19.7784 6.20317 19.9138C6.75062 20.0493 7.32558 20.0233 7.85857 19.839C8.39157 19.6547 8.8598 19.32 9.20669 18.8754C9.55357 18.4307 9.76428 17.8951 9.81334 17.3333H14C14.0847 18.0625 14.4344 18.7351 14.9826 19.2233C15.5308 19.7115 16.2393 19.9812 16.9733 19.9812C17.7074 19.9812 18.4159 19.7115 18.9641 19.2233C19.5123 18.7351 19.862 18.0625 19.9467 17.3333H22C22.1768 17.3333 22.3464 17.2631 22.4714 17.1381C22.5964 17.013 22.6667 16.8435 22.6667 16.6667V10.6667C22.6667 9.95942 22.3857 9.28115 21.8856 8.78105C21.3855 8.28095 20.7073 8 20 8ZM6.84001 18.6667C6.62344 18.6807 6.40629 18.6502 6.20199 18.577C5.99769 18.5038 5.81058 18.3895 5.65223 18.241C5.49387 18.0926 5.36765 17.9133 5.28136 17.7142C5.19507 17.5151 5.15055 17.3004 5.15055 17.0833C5.15055 16.8663 5.19507 16.6516 5.28136 16.4525C5.36765 16.2533 5.49387 16.074 5.65223 15.9256C5.81058 15.7772 5.99769 15.6629 6.20199 15.5897C6.40629 15.5165 6.62344 15.4859 6.84001 15.5C7.05658 15.4859 7.27373 15.5165 7.47803 15.5897C7.68233 15.6629 7.86944 15.7772 8.0278 15.9256C8.18615 16.074 8.31237 16.2533 8.39866 16.4525C8.48495 16.6516 8.52947 16.8663 8.52947 17.0833C8.52947 17.3004 8.48495 17.5151 8.39866 17.7142C8.31237 17.9133 8.18615 18.0926 8.0278 18.241C7.86944 18.3895 7.68233 18.5038 7.47803 18.577C7.27373 18.6502 7.05658 18.6807 6.84001 18.6667ZM16.9533 18.6667C16.6431 18.651 16.3443 18.5446 16.094 18.3608C15.8437 18.1769 15.6529 17.9236 15.5451 17.6322C15.4374 17.3409 15.4176 17.0244 15.4881 16.7219C15.5586 16.4194 15.7163 16.1442 15.9417 15.9305C16.1672 15.7168 16.4503 15.574 16.7562 15.5198C17.062 15.4655 17.377 15.5022 17.6622 15.6253C17.9474 15.7484 18.1901 15.9525 18.3604 16.2122C18.5306 16.472 18.6209 16.7761 18.62 17.0867C18.6078 17.517 18.4255 17.9249 18.1131 18.221C17.8007 18.5172 17.3837 18.6774 16.9533 18.6667Z" fill="' + color + '" /></g><g clip-path="url(#clip1_2270_618)"><path fill-rule="evenodd" clip-rule="evenodd" d="M10.1333 8.20695C9.96863 8.20695 9.81701 8.24778 9.65122 8.31638C9.49088 8.3828 9.30468 8.48053 9.07302 8.60221L8.51007 8.89757C8.2256 9.04675 7.99829 9.16626 7.82216 9.28358C7.64032 9.40527 7.49985 9.53321 7.39777 9.70662C7.29596 9.87948 7.25022 10.0668 7.22817 10.2897C7.20694 10.5061 7.20694 10.7721 7.20694 11.1069V11.1597C7.20694 11.4946 7.20694 11.7605 7.22817 11.977C7.25022 12.2002 7.29623 12.3872 7.39777 12.5601C7.49985 12.7335 7.64005 12.8614 7.82243 12.9831C7.99802 13.1004 8.2256 13.2199 8.51007 13.3691L9.07302 13.6645C9.30468 13.7861 9.49088 13.8839 9.65122 13.9503C9.81728 14.0189 9.96863 14.0597 10.1333 14.0597C10.298 14.0597 10.4497 14.0189 10.6154 13.9503C10.7758 13.8839 10.962 13.7861 11.1936 13.6645L11.7566 13.3694C12.0411 13.2199 12.2684 13.1004 12.4442 12.9831C12.6266 12.8614 12.7668 12.7335 12.8689 12.5601C12.9707 12.3872 13.0164 12.1999 13.0385 11.977C13.0597 11.7605 13.0597 11.4946 13.0597 11.16V11.1067C13.0597 10.7721 13.0597 10.5061 13.0385 10.2897C13.0164 10.0665 12.9704 9.87948 12.8689 9.70662C12.7668 9.53321 12.6266 9.40527 12.4442 9.28358C12.2686 9.16626 12.0411 9.04675 11.7566 8.89757L11.1936 8.60221C10.962 8.48053 10.7758 8.3828 10.6154 8.31638C10.4494 8.24778 10.298 8.20695 10.1333 8.20695ZM9.25405 8.96808C9.49633 8.84095 9.6662 8.75221 9.80721 8.69395C9.94441 8.63706 10.041 8.61528 10.1333 8.61528C10.2259 8.61528 10.3223 8.63706 10.4595 8.69395C10.6005 8.75221 10.7701 8.84095 11.0123 8.96808L11.5568 9.25391C11.8535 9.40935 12.0618 9.51906 12.2177 9.62305C12.2945 9.6745 12.3547 9.72213 12.4037 9.77005L11.4969 10.2233L9.183 9.00537L9.25405 8.96808ZM8.75725 9.22887L8.70988 9.25391C8.41316 9.40935 8.20491 9.51906 8.0492 9.62305C7.98275 9.66612 7.92051 9.71533 7.86327 9.77005L10.1333 10.9052L11.0472 10.4479L8.81332 9.27242C8.79223 9.2613 8.77325 9.24655 8.75725 9.22887ZM7.66645 10.128C7.65284 10.1863 7.64222 10.2524 7.6346 10.3295C7.61555 10.5238 7.61527 10.7696 7.61527 11.1173V11.1491C7.61527 11.497 7.61527 11.7428 7.6346 11.9369C7.65338 12.1267 7.68905 12.2494 7.74975 12.3529C7.81018 12.4555 7.89757 12.5424 8.0492 12.6436C8.20491 12.7476 8.41316 12.8573 8.70988 13.0128L9.25432 13.2986C9.4966 13.4257 9.6662 13.5145 9.80721 13.5727C9.85185 13.5912 9.89187 13.6059 9.92916 13.6176V11.2594L7.66645 10.128ZM10.3375 13.6174C10.3748 13.6059 10.4148 13.5912 10.4595 13.5727C10.6005 13.5145 10.7701 13.4257 11.0123 13.2986L11.5568 13.0128C11.8535 12.857 12.0618 12.7476 12.2177 12.6436C12.3691 12.5424 12.4565 12.4555 12.5172 12.3529C12.5779 12.2494 12.6133 12.1269 12.6321 11.9369C12.6511 11.7428 12.6514 11.497 12.6514 11.1494V11.1175C12.6514 10.7696 12.6514 10.5238 12.6321 10.3297C12.6259 10.262 12.6153 10.1947 12.6002 10.1283L11.6986 10.5788V11.4056C11.6986 11.4597 11.6771 11.5116 11.6388 11.5499C11.6005 11.5882 11.5486 11.6097 11.4944 11.6097C11.4403 11.6097 11.3884 11.5882 11.3501 11.5499C11.3118 11.5116 11.2903 11.4597 11.2903 11.4056V10.7833L10.3375 11.2596V13.6174Z" fill="' + color + '" /></g> <g clip-path="url(#clip2_2270_618)"><path d="M5.69284 7.50952L6.74284 6.45952C6.7591 6.44321 6.77842 6.43026 6.79969 6.42143C6.82096 6.4126 6.84377 6.40805 6.8668 6.40805C6.88983 6.40805 6.91264 6.4126 6.93391 6.42143C6.95518 6.43026 6.9745 6.44321 6.99076 6.45952L8.04076 7.50952C8.07363 7.5424 8.0921 7.58699 8.0921 7.63348C8.0921 7.67997 8.07363 7.72456 8.04076 7.75744C8.00788 7.79031 7.96329 7.80878 7.9168 7.80878C7.8703 7.80878 7.82572 7.79031 7.79284 7.75744L7.04165 7.00625L7.04165 9.15C7.04165 9.19641 7.02321 9.24093 6.9904 9.27375C6.95758 9.30656 6.91307 9.325 6.86665 9.325C6.82024 9.325 6.77573 9.30656 6.74291 9.27375C6.71009 9.24093 6.69165 9.19641 6.69165 9.15L6.69165 7.00625L5.94046 7.75758C5.90759 7.79046 5.863 7.80893 5.81651 7.80893C5.77001 7.80893 5.72542 7.79046 5.69255 7.75758C5.65967 7.72471 5.6412 7.68012 5.6412 7.63363C5.6412 7.58713 5.65967 7.54254 5.69255 7.50967L5.69284 7.50952Z" fill="' + color + '" /> </g><defs><clipPath id="clip0_2270_618"> <rect width="24" height="24" fill="white" /></clipPath> <clipPath id="clip1_2270_618">  <rect width="6.53333" height="6.53333" fill="white" transform="translate(6.86667 7.86667)" /> </clipPath><clipPath id="clip2_2270_618"> <rect width="3.73333" height="3.73334" fill="white" transform="translate(8.73334 9.73334) rotate(180)" /></clipPath></defs>' +
                '</g>' +
                '</g>' +
                ' </svg>';

            return obterIcon(svg);
        };

        this.truckStatusViagemMonitoramentoNoCliente = function (width, height, color) {

            if (color == undefined || color == "") color = 'red';


            var svg = '<svg xmlns="http://www.w3.org/2000/svg" width="' + width + '" height="' + height + '" viewBox="0 0 40 40" fill="none">' +
                '<circle cx="20" cy="20" r="19.5" fill="white" stroke="' + color + '" /><g transform="translate(8.5, 8)">' +
                '<g clip-path="url(#clip0)"><g clip-path="url(#clip0_2270_618)">' +
                '<path d="M20 8H17.3333V4.66667C17.3333 4.48986 17.2631 4.32029 17.1381 4.19526C17.0131 4.07024 16.8435 4 16.6667 4H2.00001C1.8232 4 1.65363 4.07024 1.52861 4.19526C1.40358 4.32029 1.33334 4.48986 1.33334 4.66667V16.6667C1.33334 16.8435 1.40358 17.013 1.52861 17.1381C1.65363 17.2631 1.8232 17.3333 2.00001 17.3333H2.66668V5.33333H16V13.1333C16.3399 13.0466 16.6892 13.0019 17.04 13H17.3333V9.33333H20C20.3536 9.33333 20.6928 9.47381 20.9428 9.72386C21.1929 9.97391 21.3333 10.313 21.3333 10.6667V11.3333H18.6667V12.6667H21.3333V16H19.7333C19.4995 15.4567 19.1115 14.9939 18.6174 14.6687C18.1234 14.3434 17.5448 14.1701 16.9533 14.1701C16.3619 14.1701 15.7833 14.3434 15.2893 14.6687C14.7952 14.9939 14.4072 15.4567 14.1733 16H9.62001C9.39056 15.4527 9.00397 14.9858 8.50912 14.6583C8.01428 14.3308 7.43342 14.1574 6.84001 14.16C6.27619 14.1724 5.72804 14.3479 5.26178 14.6651C4.79553 14.9824 4.43111 15.4279 4.21257 15.9477C3.99403 16.4676 3.93072 17.0397 4.03028 17.5948C4.12984 18.1499 4.38801 18.6643 4.77361 19.0758C5.1592 19.4873 5.65572 19.7784 6.20317 19.9138C6.75062 20.0493 7.32558 20.0233 7.85857 19.839C8.39157 19.6547 8.8598 19.32 9.20669 18.8754C9.55357 18.4307 9.76428 17.8951 9.81334 17.3333H14C14.0847 18.0625 14.4344 18.7351 14.9826 19.2233C15.5308 19.7115 16.2393 19.9812 16.9733 19.9812C17.7074 19.9812 18.4159 19.7115 18.9641 19.2233C19.5123 18.7351 19.862 18.0625 19.9467 17.3333H22C22.1768 17.3333 22.3464 17.2631 22.4714 17.1381C22.5964 17.013 22.6667 16.8435 22.6667 16.6667V10.6667C22.6667 9.95942 22.3857 9.28115 21.8856 8.78105C21.3855 8.28095 20.7073 8 20 8ZM6.84001 18.6667C6.62344 18.6807 6.40629 18.6502 6.20199 18.577C5.99769 18.5038 5.81058 18.3895 5.65223 18.241C5.49387 18.0926 5.36765 17.9133 5.28136 17.7142C5.19507 17.5151 5.15055 17.3004 5.15055 17.0833C5.15055 16.8663 5.19507 16.6516 5.28136 16.4525C5.36765 16.2533 5.49387 16.074 5.65223 15.9256C5.81058 15.7772 5.99769 15.6629 6.20199 15.5897C6.40629 15.5165 6.62344 15.4859 6.84001 15.5C7.05658 15.4859 7.27373 15.5165 7.47803 15.5897C7.68233 15.6629 7.86944 15.7772 8.0278 15.9256C8.18615 16.074 8.31237 16.2533 8.39866 16.4525C8.48495 16.6516 8.52947 16.8663 8.52947 17.0833C8.52947 17.3004 8.48495 17.5151 8.39866 17.7142C8.31237 17.9133 8.18615 18.0926 8.0278 18.241C7.86944 18.3895 7.68233 18.5038 7.47803 18.577C7.27373 18.6502 7.05658 18.6807 6.84001 18.6667ZM16.9533 18.6667C16.6431 18.651 16.3443 18.5446 16.094 18.3608C15.8437 18.1769 15.6529 17.9236 15.5451 17.6322C15.4374 17.3409 15.4176 17.0244 15.4881 16.7219C15.5586 16.4194 15.7163 16.1442 15.9417 15.9305C16.1672 15.7168 16.4503 15.574 16.7562 15.5198C17.062 15.4655 17.377 15.5022 17.6622 15.6253C17.9474 15.7484 18.1901 15.9525 18.3604 16.2122C18.5306 16.472 18.6209 16.7761 18.62 17.0867C18.6078 17.517 18.4255 17.9249 18.1131 18.221C17.8007 18.5172 17.3837 18.6774 16.9533 18.6667Z" fill="' + color + '" />' +
                '</g>' +
                '<g xmlns="http://www.w3.org/2000/svg" clip-path="url(#clip1_2272_662)">' +
                '<path d="M9.94704 11.3548L10.144 11.5573C9.95604 11.7056 9.75261 11.8226 9.5337 11.9084C9.31479 11.9942 9.08687 12.0547 8.84996 12.09V11.8084C9.06118 11.7765 9.25945 11.7226 9.44478 11.6465C9.63012 11.5705 9.79754 11.4733 9.94704 11.3548ZM11.09 9.85156C11.0634 10.0885 11.0044 10.3161 10.9133 10.5346C10.8221 10.7531 10.7024 10.9579 10.5542 11.149L10.355 10.9497C10.477 10.7948 10.5754 10.6247 10.6505 10.4395C10.7255 10.2542 10.7771 10.0582 10.8052 9.85156H11.09ZM10.5593 7.99634C10.7195 8.21014 10.8394 8.41127 10.9189 8.59973C10.9984 8.78819 11.0555 9.01776 11.09 9.28843H10.8052C10.7771 9.07726 10.7255 8.87895 10.6505 8.69349C10.5754 8.50803 10.477 8.34031 10.355 8.19033L10.5593 7.99634ZM8.28692 7.05V7.33156C7.72417 7.41134 7.25756 7.6624 6.88708 8.08475C6.5166 8.50709 6.33136 9.00218 6.33136 9.57C6.33136 10.1331 6.5166 10.6259 6.88708 11.0482C7.25756 11.4706 7.72417 11.724 8.28692 11.8084V12.09C7.64876 12.0093 7.1164 11.7296 6.68984 11.2509C6.26327 10.7723 6.04999 10.212 6.04999 9.57C6.04999 8.92203 6.26327 8.35909 6.68984 7.88118C7.1164 7.40327 7.64876 7.12621 8.28692 7.05ZM8.86065 7.05C9.09757 7.08078 9.32192 7.13991 9.5337 7.22738C9.74548 7.31486 9.94901 7.4333 10.1443 7.58272L9.96139 7.79614C9.79256 7.66944 9.6152 7.5662 9.42931 7.48642C9.2436 7.40665 9.05405 7.35503 8.86065 7.33156V7.05ZM8.45828 10.5966C8.40501 10.44 8.34048 10.3062 8.2647 10.1951C8.18872 10.0841 8.1136 9.97864 8.03931 9.87859C7.96484 9.77873 7.9021 9.67559 7.85107 9.56915C7.80005 9.46291 7.77454 9.33433 7.77454 9.18341C7.77454 8.96041 7.85145 8.77195 8.00527 8.61803C8.15909 8.46411 8.34733 8.38724 8.56999 8.38743C8.79265 8.38762 8.98098 8.46458 9.13499 8.61831C9.289 8.77205 9.36581 8.96041 9.36544 9.18341C9.36544 9.33433 9.33993 9.46291 9.2889 9.56915C9.23788 9.6754 9.17523 9.77864 9.10094 9.87887C9.02647 9.97874 8.95125 10.0841 8.87528 10.1951C8.7995 10.306 8.73487 10.4398 8.68141 10.5966H8.45828ZM8.56999 9.43513C8.64183 9.43513 8.70177 9.4112 8.74979 9.36333C8.79762 9.31528 8.82154 9.2553 8.82154 9.18341C8.82154 9.11152 8.79762 9.05155 8.74979 9.00349C8.70195 8.95544 8.64202 8.9316 8.56999 8.93197C8.49796 8.93235 8.43802 8.95638 8.39019 9.00405C8.34236 9.05173 8.31844 9.11161 8.31844 9.18369C8.31844 9.25577 8.34236 9.31575 8.39019 9.36361C8.43802 9.41148 8.49796 9.43541 8.56999 9.43541" fill="' + color + '" />' +
                '<g clip-path="url(#clip2_2272_662)">' +
                '<path d="M12.7765 13.0414L11.8315 13.9864C11.8168 14.0011 11.7994 14.0128 11.7803 14.0207C11.7612 14.0287 11.7406 14.0328 11.7199 14.0328C11.6992 14.0328 11.6786 14.0287 11.6595 14.0207C11.6404 14.0128 11.623 14.0011 11.6083 13.9864L10.6633 13.0414C10.6337 13.0118 10.6171 12.9717 10.6171 12.9299C10.6171 12.888 10.6337 12.8479 10.6633 12.8183C10.6929 12.7887 10.7331 12.7721 10.7749 12.7721C10.8167 12.7721 10.8569 12.7887 10.8865 12.8183L11.5625 13.4944V11.565C11.5625 11.5232 11.5791 11.4832 11.6087 11.4536C11.6382 11.4241 11.6783 11.4075 11.72 11.4075C11.7618 11.4075 11.8019 11.4241 11.8314 11.4536C11.8609 11.4832 11.8775 11.5232 11.8775 11.565V13.4944L12.5536 12.8182C12.5832 12.7886 12.6233 12.772 12.6652 12.772C12.707 12.772 12.7471 12.7886 12.7767 12.8182C12.8063 12.8478 12.8229 12.8879 12.8229 12.9297C12.8229 12.9716 12.8063 13.0117 12.7767 13.0413L12.7765 13.0414Z" fill="' + color + '" /></g></g> <defs><clipPath id="clip0_2270_618"> <rect width="24" height="24" fill="white" /></clipPath> <clipPath id="clip1_2270_618">' +
                '<rect width="6.53333" height="6.53333" fill="white" transform="translate(6.86667 7.86667)" />' +
                '</clipPath>' +
                '<clipPath id="clip2_2270_618"> <rect width="3.73333" height="3.73334" fill="white" transform="translate(8.73334 9.73334) rotate(180)" />' +
                '</clipPath>' +
                '</defs>' +
                '</g>' +
                '</g >' +
                '</svg >';

            return obterIcon(svg);
        };

        this.point = function () {
            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="20" viewBox="0 0 192 192" style=" fill:#000000;">' +
                '  <g fill="none" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt" stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0" font-family="none" font-weight="none" font-size="none" text-anchor="none" style="mix-blend-mode: normal">' +
                '    <path d="M0,192v-192h192v192z" fill="none"></path>' +
                '    <g id="Layer_1">' +
                '      <g id="surface1">' +
                '        <path d="M104,164l-8,20l-8,-20v-108h16z" fill="#c5cae9"></path>' +
                '        <path d="M128,40c0,17.6 -14.4,32 -32,32c-17.6,0 -32,-14.4 -32,-32c0,-17.6 14.4,-32 32,-32c17.6,0 32,14.4 32,32z" fill="#001aff"></path>' +
                '        <path d="M76,44c-2.4,0 -4,-1.6 -4,-4c0,-13.2 10.8,-24 24,-24c2.4,0 4,1.6 4,4c0,2.4 -1.6,4 -4,4c-8.8,0 -16,7.2 -16,16c0,2.4 -1.6,4 -4,4z" fill="#ffab91"></path>' +
                '      </g>' +
                '    </g>' +
                '  </g>' +
                '</svg>';

            return obterIcon(svg);
        };

        this.Maker = function () {
            var svg =
                ' <svg id="marker" data-name="marker" xmlns="http://www.w3.org/2000/svg" width="20" height="48" viewBox="0 0 20 48"> ' +
                ' <g id="mapbox-marker-icon"> ' +
                ' <g id="icon"> ' +
                ' <ellipse id="shadow" cx="10" cy="27" rx="9" ry="5" fill="#c4c4c4" opacity="0.3" style="isolation: isolate"/> ' +
                ' <g id="mask" opacity="0.3">' +
                ' <g id="group"> ' +
                ' <path id="shadow-2" data-name="shadow" fill="#bfbfbf" d="M10,32c5,0,9-2.2,9-5s-4-5-9-5-9,2.2-9,5S5,32,10,32Z" fill-rule="evenodd"/> ' +
                ' </g> ' +
                ' </g> ' +
                ' <path id="color" fill="#5b7897" stroke="#23374d" stroke-width="0.5" d="M19.25,10.4a13.0663,13.0663,0,0,1-1.4607,5.2235,41.5281,41.5281,0,0,1-3.2459,5.5483c-1.1829,1.7369-2.3662,3.2784-3.2541,4.3859-.4438.5536-.8135.9984-1.0721,1.3046-.0844.1-.157.1852-.2164.2545-.06-.07-.1325-.1564-.2173-.2578-.2587-.3088-.6284-.7571-1.0723-1.3147-.8879-1.1154-2.0714-2.6664-3.2543-4.41a42.2677,42.2677,0,0,1-3.2463-5.5535A12.978,12.978,0,0,1,.75,10.4,9.4659,9.4659,0,0,1,10,.75,9.4659,9.4659,0,0,1,19.25,10.4Z"/> ' +
                ' <path id="circle" fill="#fff" stroke="#23374d" stroke-width="0.5" d="M13.55,10A3.55,3.55,0,1,1,10,6.45,3.5484,3.5484,0,0,1,13.55,10Z"/> ' +
                '  </g> ' +
                ' </g> ' +
                ' <rect width="20" height="48" fill="none"/> ' +
                '</svg> ';

            return obterIcon(svg);
        };

        this.Cliente = function () {
            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" width="40" height="50" viewBox="0 0 40 50">' +
                '<g id="mapbox-marker-icon">' +
                '  <g id="icon">' +
                '    <path d="M20 1.25C30.3553 1.25 38.75 9.64466 38.75 20C38.75 25.1586 36.0937 31.4696 32.2529 37.0254C28.5648 42.3604 23.9615 46.742 20 48.627C16.0385 46.742 11.4352 42.3604 7.74707 37.0254C3.90633 31.4696 1.25 25.1586 1.25 20C1.25 9.64466 9.64466 1.25 20 1.25Z" fill="#FFEA7C" stroke="#F39A14" stroke-width="2.5"/>' +
                '    <path d="M19.7828 13.6305C20.589 13.6305 21.3771 13.8696 22.0474 14.3175C22.7177 14.7653 23.2402 15.402 23.5487 16.1468C23.8572 16.8916 23.9379 17.7112 23.7806 18.5019C23.6234 19.2926 23.2352 20.0189 22.6651 20.5889C22.095 21.159 21.3687 21.5472 20.578 21.7045C19.7873 21.8618 18.9677 21.7811 18.2229 21.4725C17.4781 21.164 16.8415 20.6416 16.3936 19.9713C15.9457 19.3009 15.7066 18.5128 15.7066 17.7067L15.7107 17.5298C15.7563 16.4803 16.2052 15.489 16.9639 14.7626C17.7226 14.0361 18.7324 13.6306 19.7828 13.6305ZM21.4133 23.4133C22.4943 23.4133 23.5311 23.8427 24.2956 24.6072C25.06 25.3716 25.4894 26.4084 25.4894 27.4895V28.3047C25.4894 28.7371 25.3177 29.1518 25.0119 29.4576C24.7061 29.7634 24.2914 29.9352 23.859 29.9352H15.7066C15.2742 29.9352 14.8595 29.7634 14.5537 29.4576C14.248 29.1518 14.0762 28.7371 14.0762 28.3047V27.4895C14.0762 26.4084 14.5056 25.3716 15.2701 24.6072C16.0345 23.8427 17.0713 23.4133 18.1523 23.4133H21.4133Z" fill="#F39A14"/>' +
                '  </g>' +
                '</g>' +
                '<rect width="40" height="50" fill="none"/>' +
                '</svg>';

            return obterIcon(svg);
        };

        this.truckStop = function (width, height, color) {
            if (!color) color = "#00ff00";
            if (!width) width = 80;
            if (!height) height = 80;
            //var svg =
            //    '<svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="' + width + '" height="' + height + '" viewBox="0 0 268.8 268.8" style="fill:#000000" version="1.1" id="svg1378">' +
            //    '  <g id="g3202">' +
            //    '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" d="m 172,201.6 h -56 v -88 h 42.23438 c 3.4375,0 6.5,2.20312 7.59375,5.46875 L 180.00001,161.6 v 32 c 0,4.42188 -3.57812,8 -8,8" fill="' + color + '" id="path2675" />' +
            //    '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" d="M 116,201.6 H 20 c -4.42188,0 -8,-3.57812 -8,-8 v -100 c 0,-4.42188 3.57812,-8 8,-8 h 88 c 4.42188,0 8,3.57812 8,8 z" fill="' + color + '" id="path2677" />' +
            //    '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" d="m 168,201.6 c 0,11.04688 -8.95312,20 -20,20 -11.04688,0 -20,-8.95312 -20,-20 0,-11.04688 8.95312,-20 20,-20 11.04688,0 20,8.95312 20,20" fill="#5d4037" id="path2679" />' +
            //    '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" d="m 72,201.6 c 0,11.04688 -8.95312,20 -20,20 -11.04688,0 -20,-8.95312 -20,-20 0,-11.04688 8.95312,-20 20,-20 11.04688,0 20,8.95312 20,20" fill="#5d4037" id="path2681" />' +
            //    '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" d="m 156,201.6 c 0,4.42188 -3.57812,8 -8,8 -4.42188,0 -8,-3.57812 -8,-8 0,-4.42188 3.57812,-8 8,-8 4.42188,0 8,3.57812 8,8" fill="#bcaaa4" id="path2683" />' +
            //    '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" d="m 60,201.6 c 0,4.42188 -3.57812,8 -8,8 -4.42188,0 -8,-3.57812 -8,-8 0,-4.42188 3.57812,-8 8,-8 4.42188,0 8,3.57812 8,8" fill="#bcaaa4" id="path2685" />' +
            //    '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" d="m 164,157.6 h -28 c -2.20312,0 -4,-1.79688 -4,-4 v -28 c 0,-2.20312 1.79688,-4 4,-4 h 20 c 1.73438,0 3.25,1.09375 3.79688,2.73438 l 8,24 C 167.92188,148.74063 168,149.17813 168,149.6 v 4 c 0,2.20312 -1.79688,4 -4,4" fill="#795548" id="path2687" />' +
            //    '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" d="m 12,125.6 h 104 v 32 H 12 Z" fill="' + color + '" id="path2689" />' +
            //    '  </g>' +
            //    '  <g id="g3206">' +
            //    '    <path d="m 227.07489,201.77059 -8,20 -8,-20 V 93.770592 h 16 z" fill="#c5cae9" id="path1368" style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:0.999999;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" />' +
            //    '    <path d="m 206.14839,44.651549 -26.1579,25.904574 v 26.922366 l 25.6138,25.613821 h 26.9599 l 25.5951,-25.595071 V 70.537373 l -25.5951,-25.613817 z" style="fill:#ff0000;fill-opacity:1;stroke-width:10.5846;stop-color:#000000" id="rect1421" />' +
            //    '  </g>' +
            //    '</svg>';

            var svg =
                '<svg height="40" width="48" viewBox="0 0 150 266" fill="none" xmlns="http://www.w3.org/2000/svg" style="transform: rotate(' + randomNumber(10, 360) + 'deg);">' +
                '    <g id="g1118">' +
                '        <g filter="url(#filter0_d_78:51)">' +
                '            <rect x="28.5581" y="28.166" width="74.3655" height="49.7819" fill="black"></rect>' +
                '            <path fill-rule="evenodd" clip-rule="evenodd" d="M65.5357 16.8625C65.5357 16.8625 37.6121 17.3884 33.3389 19.1647C30.7581 20.2376 30.8044 19.1869 28.2659 23.3934C26.4043 26.5309 27.152 43.4644 27.152 43.4644C24.0707 43.4644 16.8809 45.5186 16.8809 46.5458C16.8809 46.9176 16.8809 47.5729 17.908 47.5729C18.7541 47.5729 26.1249 45.5186 27.152 45.5186L27.152 83.522L22.0164 92.2525L27.152 100.983L105.213 100.983L110.348 92.2525L105.213 83.522L105.213 45.5186C105.213 45.5186 113.738 47.5729 114.457 47.5729C115.43 47.5729 116.361 47.171 114.457 46.0322C112.553 44.8934 107.267 43.4644 105.213 43.4644L104.121 25.6705C104.121 25.6705 103.188 22.6496 101.71 20.2115C98.2564 17.7177 65.5357 16.8625 65.5357 16.8625ZM75.309 31.4378C82.8823 31.9724 89.9055 33.0647 95.6595 34.552C97.5634 35.0633 97.6903 35.1563 98.1557 36.3881C98.8326 38.1776 98.4095 43.6624 97.2672 47.3576C96.4633 50.0768 93.9671 56.4447 93.671 56.6306C84.9977 55.6777 74.759 54.9341 65.5357 54.9341C56.4394 54.9341 44.5369 56.0137 37.9367 56.92C37.0483 55.1305 34.6928 48.5429 33.9735 45.9632C33.1274 42.8257 32.9581 36.667 33.7197 35.6676C34.8197 34.2499 46.4122 32.0886 57.074 31.3449C61.4741 31.0195 70.4436 31.066 75.309 31.4378ZM36.3961 58.8711L36.3428 75.305C36.2159 75.305 30.2334 74.2779 30.2334 74.2779L30.2334 42.4373C30.2757 42.4838 34.746 54.1301 36.3961 58.8711ZM102.132 74.2779L95.9688 75.305L95.9688 58.8711L102.131 42.4373L102.132 74.2779Z" fill="#176b2b"></path>' +
                '            <path d="M22.0166 99.6374C22.0166 94.7081 27.5349 90.7121 34.342 90.7121L98.0232 90.7121C104.83 90.7121 110.349 94.7081 110.349 99.6374L110.349 232.773C110.349 237.702 104.83 241.698 98.0232 241.698L34.342 241.698C27.5349 241.698 22.0166 237.702 22.0166 232.773L22.0166 99.6374Z" fill="' + color + '"></path>' +
                '            <line x1="35.3691" y1="209.357" x2="96.9961" y2="209.357" stroke="black"></line>' +
                '            <line x1="35.3687" y1="165.191" x2="96.9956" y2="165.191" stroke="black"></line>' +
                '            <line x1="35.3687" y1="121.025" x2="96.9956" y2="121.025" stroke="black"></line>' +
                '            <rect x="49.7485" y="81.4683" width="15.4067" height="29.7864" rx="6" transform="rotate(-90 49.7485 81.4683)" fill="white"></rect>' +
                '        </g>' +
                '        <g id="g3206">' +
                '            <path id="rect1421" style="fill:#ff0000;fill-opacity:1;stroke-width:10.5846;stop-color:#000000" d="m 142.14839,44.651549 -26.1579,25.904574 v 26.922366 l 25.6138,25.613821 h 26.9599 l 25.5951,-25.595071 V 70.537373 l -25.5951,-25.613817 z" />' +
                '        </g>' +
                '    </g>' +
                '    <defs>' +
                '        <filter id="filter0_d_78:51" x="0.880859" y="0.862305" width="138.682" height="264.836" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">' +
                '            <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>' +
                '            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>' +
                '            <feOffset dx="4" dy="4"></feOffset>' +
                '            <feGaussianBlur stdDeviation="10"></feGaussianBlur>' +
                '            <feComposite in2="hardAlpha" operator="out"></feComposite>' +
                '            <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.5 0"></feColorMatrix>' +
                '            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_78:51"></feBlend>' +
                '            <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_78:51" result="shape"></feBlend>' +
                '        </filter>' +
                '    </defs>' +
                '</svg>'

            return obterIcon(svg);
        }

        this.truckTrailerStop = function (width, height, color) {
            if (!color) color = "#00FF00";
            if (!width) width = 30;
            if (!height) height = 30;
            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" id="svg1378"  style="fill:#000000" viewBox="0 0 268.8 268.8" height="' + height + '" width="' + width + '" y="0px" x="0px">' +
                '  <g id="g3202">' +
                '    <path id="path2677" fill="' + color + '" d="M 116,201.6 H 20 c -4.42188,0 -8,-3.57812 -8,-8 v -100 c 0,-4.42188 3.57812,-8 8,-8 h 88 c 4.42188,0 8,3.57812 8,8 z" style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" />' +
                '    <path fill="' + color + '" transform="scale(13.44)" d="m 1.4882812,6.3691406 c -0.3290089,0 -0.59570308,0.2666942 -0.59570308,0.5957032 V 14.404297 C 0.89257812,14.733306 1.1592723,15 1.4882812,15 H 12.796875 c 0.329009,0 0.595703,-0.266694 0.595703,-0.595703 V 6.9648438 c 0,-0.329009 -0.266694,-0.5957032 -0.595703,-0.5957032 z" style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:0.0744048;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" id="path170" />' +
                '    <path id="path2679" fill="#5d4037" d="m 114.24,201.6 c 0,11.04688 -8.95312,20 -20,20 -11.04688,0 -20,-8.95312 -20,-20 0,-11.04688 8.95312,-20 20,-20 11.04688,0 20,8.95312 20,20" style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" />' +
                '    <path id="path2681" fill="#5d4037" d="m 72,201.6 c 0,11.04688 -8.95312,20 -20,20 -11.04688,0 -20,-8.95312 -20,-20 0,-11.04688 8.95312,-20 20,-20 11.04688,0 20,8.95312 20,20" style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" />' +
                '    <path id="path2683" fill="#bcaaa4" d="m 102.24,201.6 c 0,4.42188 -3.57812,8 -8,8 -4.42188,0 -8,-3.57812 -8,-8 0,-4.42188 3.57812,-8 8,-8 4.42188,0 8,3.57812 8,8" style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" />' +
                '    <path id="path2685" fill="#bcaaa4" d="m 60,201.6 c 0,4.42188 -3.57812,8 -8,8 -4.42188,0 -8,-3.57812 -8,-8 0,-4.42188 3.57812,-8 8,-8 4.42188,0 8,3.57812 8,8" style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:1;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" />' +
                '  </g>' +
                '  <g id="g3206">' +
                '    <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:0.999999;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" id="path1368" fill="#c5cae9" d="m 227.07489,201.77059 -8,20 -8,-20 V 93.770592 h 16 z" />' +
                '    <path id="rect1421" style="fill:#ff0000;fill-opacity:1;stroke-width:10.5846;stop-color:#000000" d="m 206.14839,44.651549 -26.1579,25.904574 v 26.922366 l 25.6138,25.613821 h 26.9599 l 25.5951,-25.595071 V 70.537373 l -25.5951,-25.613817 z" />' +
                '  </g>' +
                '</svg>';

            //var svg =
            //'<svg height="' + height + '" width="' + width + '" viewBox="0 0 150 266" fill="none" xmlns="http://www.w3.org/2000/svg" style="transform: rotate(180deg);">' +
            //'    <g id="g1118">' +
            //'        <g filter="url(#filter0_d_78:51)">' +
            //'            <rect x="28.5581" y="28.166" width="74.3655" height="49.7819" fill="black"></rect>' +
            //'            <path fill-rule="evenodd" clip-rule="evenodd" d="M65.5357 16.8625C65.5357 16.8625 37.6121 17.3884 33.3389 19.1647C30.7581 20.2376 30.8044 19.1869 28.2659 23.3934C26.4043 26.5309 27.152 43.4644 27.152 43.4644C24.0707 43.4644 16.8809 45.5186 16.8809 46.5458C16.8809 46.9176 16.8809 47.5729 17.908 47.5729C18.7541 47.5729 26.1249 45.5186 27.152 45.5186L27.152 83.522L22.0164 92.2525L27.152 100.983L105.213 100.983L110.348 92.2525L105.213 83.522L105.213 45.5186C105.213 45.5186 113.738 47.5729 114.457 47.5729C115.43 47.5729 116.361 47.171 114.457 46.0322C112.553 44.8934 107.267 43.4644 105.213 43.4644L104.121 25.6705C104.121 25.6705 103.188 22.6496 101.71 20.2115C98.2564 17.7177 65.5357 16.8625 65.5357 16.8625ZM75.309 31.4378C82.8823 31.9724 89.9055 33.0647 95.6595 34.552C97.5634 35.0633 97.6903 35.1563 98.1557 36.3881C98.8326 38.1776 98.4095 43.6624 97.2672 47.3576C96.4633 50.0768 93.9671 56.4447 93.671 56.6306C84.9977 55.6777 74.759 54.9341 65.5357 54.9341C56.4394 54.9341 44.5369 56.0137 37.9367 56.92C37.0483 55.1305 34.6928 48.5429 33.9735 45.9632C33.1274 42.8257 32.9581 36.667 33.7197 35.6676C34.8197 34.2499 46.4122 32.0886 57.074 31.3449C61.4741 31.0195 70.4436 31.066 75.309 31.4378ZM36.3961 58.8711L36.3428 75.305C36.2159 75.305 30.2334 74.2779 30.2334 74.2779L30.2334 42.4373C30.2757 42.4838 34.746 54.1301 36.3961 58.8711ZM102.132 74.2779L95.9688 75.305L95.9688 58.8711L102.131 42.4373L102.132 74.2779Z" fill="#707070"></path>' +
            //'            <path d="M22.0166 99.6374C22.0166 94.7081 27.5349 90.7121 34.342 90.7121L98.0232 90.7121C104.83 90.7121 110.349 94.7081 110.349 99.6374L110.349 232.773C110.349 237.702 104.83 241.698 98.0232 241.698L34.342 241.698C27.5349 241.698 22.0166 237.702 22.0166 232.773L22.0166 99.6374Z" fill="' + color + '"></path>' +
            //'            <line x1="35.3691" y1="209.357" x2="96.9961" y2="209.357" stroke="black"></line>' +
            //'            <line x1="35.3687" y1="165.191" x2="96.9956" y2="165.191" stroke="black"></line>' +
            //'            <line x1="35.3687" y1="121.025" x2="96.9956" y2="121.025" stroke="black"></line>' +
            //'            <rect x="49.7485" y="81.4683" width="15.4067" height="29.7864" rx="6" transform="rotate(-90 49.7485 81.4683)" fill="white"></rect>' +
            //'        </g>' +
            //'        <g id="g3206">' +
            //'            <path style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:0.999999;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" id="path1368" fill="#c5cae9" d="m 165.07489,201.77059 -8,20 -8,-20 V 93.770592 h 16 z" />' +
            //'            <path id="rect1421" style="fill:#ff0000;fill-opacity:1;stroke-width:10.5846;stop-color:#000000" d="m 142.14839,44.651549 -26.1579,25.904574 v 26.922366 l 25.6138,25.613821 h 26.9599 l 25.5951,-25.595071 V 70.537373 l -25.5951,-25.613817 z" />' +
            //'        </g>' +
            //'    </g>' +
            //'    <defs>' +
            //'        <filter id="filter0_d_78:51" x="0.880859" y="0.862305" width="138.682" height="264.836" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">' +
            //'            <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>' +
            //'            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>' +
            //'            <feOffset dx="4" dy="4"></feOffset>' +
            //'            <feGaussianBlur stdDeviation="10"></feGaussianBlur>' +
            //'            <feComposite in2="hardAlpha" operator="out"></feComposite>' +
            //'            <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.5 0"></feColorMatrix>' +
            //'            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_78:51"></feBlend>' +
            //'            <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_78:51" result="shape"></feBlend>' +
            //'        </filter>' +
            //'    </defs>' +
            //'</svg>'



            return obterIcon(svg);
        }

        this.stop = function (width, height, color) {
            if (!color) color = "#ff5722";
            if (!width) width = 20;
            if (!height) height = 20;
            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" id="svg1378" version="1.1" style="fill:#000000" viewBox="0 0 192 192" width="' + width + '" height="' + height + '" y="0px" x="0px">' +
                '  <path d="m 104,164.55952 -8,20 -8,-20 V 56.559522 h 16 z" fill="#c5cae9" id="path1368" style="font-family:none;mix-blend-mode:normal;fill-rule:nonzero;stroke:none;stroke-width:0.999999;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:10;stroke-dasharray:none;stroke-dashoffset:0" />' +
                '  <path d="M 83.0735,7.440479 56.9156,33.345053 V 60.267419 L 82.5294,85.88124 h 26.9599 L 135.0844,60.286169 V 33.326303 L 109.4893,7.712486 Z" style="fill:' + color + ';fill-opacity:1;stroke-width:10.5846;stop-color:#000000" id="rect1421" />' +
                '</svg >';
            return obterIcon(svg);
        };

        this.PinMapLocal = function (obterSvg) {
            var svg =
                '<svg width="40" height="50" viewBox="0 0 40 50" fill="none" xmlns="http://www.w3.org/2000/svg">' +
                '<path d="M20 1.25C30.3553 1.25 38.75 9.64466 38.75 20C38.75 25.1586 36.0937 31.4696 32.2529 37.0254C28.5648 42.3604 23.9615 46.742 20 48.627C16.0385 46.742 11.4352 42.3604 7.74707 37.0254C3.90633 31.4696 1.25 25.1586 1.25 20C1.25 9.64466 9.64466 1.25 20 1.25Z" fill="#38AAE1" stroke="#A2DAF6" stroke-width="2.5"/>' +
                '<g clip-path="url(#clip0_209_792)">' +
                '<path d="M26.8141 25.4516H12.7666C12.5985 25.4516 12.4609 25.5892 12.4609 25.7573L12.4571 27.5916C12.4571 27.7597 12.5946 27.8973 12.7628 27.8973H26.8141C26.9822 27.8973 27.1198 27.7597 27.1198 27.5916V25.7573C27.1198 25.5892 26.9822 25.4516 26.8141 25.4516ZM26.8141 29.1201H12.7551C12.587 29.1201 12.4494 29.2577 12.4494 29.4258L12.4456 31.2601C12.4456 31.4283 12.5832 31.5658 12.7513 31.5658H26.8141C26.9822 31.5658 27.1198 31.4283 27.1198 31.2601V29.4258C27.1198 29.2577 26.9822 29.1201 26.8141 29.1201ZM26.8141 21.783H12.7742C12.6061 21.783 12.4685 21.9206 12.4685 22.0887L12.4647 23.923C12.4647 24.0912 12.6023 24.2287 12.7704 24.2287H26.8141C26.9822 24.2287 27.1198 24.0912 27.1198 23.923V22.0887C27.1198 21.9206 26.9822 21.783 26.8141 21.783ZM30.8839 16.4713L20.4858 12.1416C20.2623 12.049 20.0227 12.0013 19.7808 12.0013C19.5388 12.0013 19.2993 12.049 19.0757 12.1416L8.68151 16.4713C8.0013 16.7579 7.5542 17.4228 7.5542 18.1642V31.2601C7.5542 31.4283 7.69177 31.5658 7.85991 31.5658H10.917C11.0852 31.5658 11.2227 31.4283 11.2227 31.2601V21.783C11.2227 21.1105 11.7807 20.5602 12.4685 20.5602H27.0969C27.7847 20.5602 28.3426 21.1105 28.3426 21.783V31.2601C28.3426 31.4283 28.4802 31.5658 28.6484 31.5658H31.7055C31.8736 31.5658 32.0112 31.4283 32.0112 31.2601V18.1642C32.0112 17.4228 31.5641 16.7579 30.8839 16.4713Z" fill="#A2DAF6"/>' +
                '</g>' +
                '<defs>' +
                '<clipPath id="clip0_209_792">' +
                '<rect width="24.457" height="19.5656" fill="white" transform="translate(7.5542 12)"/>' +
                '</clipPath>' +
                '</defs>' +
                '</svg>';

            if (obterSvg)
                return svg;

            return obterIcon(svg);
        };

        this.pinTracao = function (width, height, color, obterSvg) {
            if (!color) color = "#ff5722";
            if (!width) width = 20;
            if (!height) height = 20;

            var backgroundColor = lightenHexColor(color, 0.7);

            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" width="' + width + '" height="' + height + '" viewBox="0 0 40 40" fill="none">' +
                '  <rect x="0.5" y="0.5" width="39" height="39" rx="19.5" fill="' + color + '"/>' +
                '  <rect x="0.5" y="0.5" width="39" height="39" rx="19.5" stroke="white"/>' +
                '  <g clip-path="url(#clip0)">' +
                '    <path d="M28.0002 16H25.3335V12.6667C25.3335 12.4899 25.2633 12.3203 25.1382 12.1953C25.0132 12.0702 24.8436 12 24.6668 12H10.0002C9.82335 12 9.65378 12.0702 9.52876 12.1953C9.40373 12.3203 9.3335 12.4899 9.3335 12.6667V24.6667C9.3335 24.8435 9.40373 25.013 9.52876 25.1381C9.65378 25.2631 9.82335 25.3333 10.0002 25.3333H10.6668V13.3333H24.0002V21.1333C24.3401 21.0466 24.6894 21.0019 25.0402 21H25.3335V17.3333H28.0002C28.3538 17.3333 28.6929 17.4738 28.943 17.7239C29.193 17.9739 29.3335 18.313 29.3335 18.6667V19.3333H26.6668V20.6667H29.3335V24H27.7335C27.4996 23.4567 27.1117 22.9939 26.6176 22.6687C26.1235 22.3434 25.545 22.1701 24.9535 22.1701C24.362 22.1701 23.7835 22.3434 23.2894 22.6687C22.7953 22.9939 22.4074 23.4567 22.1735 24H17.6202C17.3907 23.4527 17.0041 22.9858 16.5093 22.6583C16.0144 22.3308 15.4336 22.1574 14.8402 22.16C14.2763 22.1724 13.7282 22.3479 13.2619 22.6651C12.7957 22.9824 12.4313 23.4279 12.2127 23.9477C11.9942 24.4676 11.9309 25.0397 12.0304 25.5948C12.13 26.1499 12.3882 26.6643 12.7738 27.0758C13.1594 27.4873 13.6559 27.7784 14.2033 27.9138C14.7508 28.0493 15.3257 28.0233 15.8587 27.839C16.3917 27.6547 16.86 27.32 17.2068 26.8754C17.5537 26.4307 17.7644 25.8951 17.8135 25.3333H22.0002C22.0849 26.0625 22.4346 26.7351 22.9828 27.2233C23.531 27.7115 24.2394 27.9812 24.9735 27.9812C25.7076 27.9812 26.416 27.7115 26.9642 27.2233C27.5124 26.7351 27.8621 26.0625 27.9468 25.3333H30.0002C30.177 25.3333 30.3465 25.2631 30.4716 25.1381C30.5966 25.013 30.6668 24.8435 30.6668 24.6667V18.6667C30.6668 17.9594 30.3859 17.2811 29.8858 16.781C29.3857 16.281 28.7074 16 28.0002 16Z" fill="white"/>' +
                '  </g>' +
                '  <defs>' +
                '    <clipPath id="clip0">' +
                '      <rect width="24" height="24" fill="white" transform="translate(8 8)"/>' +
                '    </clipPath>' +
                '  </defs>' +
                '</svg>';

            if (obterSvg)
                return svg;

            return obterIcon(svg);
        };

        this.pinTracaoDeslocamento = function (width, height, color, obterSvg) {
            if (!color) color = "#ff5722";
            if (!width) width = 20;
            if (!height) height = 20;

            var backgroundColor = lightenHexColor(color, 0.7);

            var svg =
                '<svg width="' + width + '" height="' + height + '" viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg">' +
                '  <rect x="0.5" y="0.5" width="39" height="39" rx="19.5" fill="' + color + '"/>' +
                '  <rect x="0.5" y="0.5" width="39" height="39" rx="19.5" stroke="white"/>' +
                '  <path d="M11 20H20V21.5H11V20ZM9.5 16.25H17V17.75H9.5V16.25Z" fill="white"/>' +
                '  <path d="M30.4393 20.4545L28.1893 15.2045C28.1315 15.0696 28.0354 14.9545 27.9129 14.8737C27.7903 14.793 27.6468 14.7499 27.5 14.75H25.25V13.25C25.25 13.0511 25.171 12.8603 25.0303 12.7197C24.8897 12.579 24.6989 12.5 24.5 12.5H12.5V14H23.75V23.417C23.4083 23.6154 23.1092 23.8796 22.87 24.1942C22.6308 24.5088 22.4563 24.8676 22.3565 25.25H17.6435C17.461 24.543 17.0268 23.9269 16.4225 23.517C15.8182 23.1072 15.0851 22.9319 14.3608 23.0239C13.6364 23.1159 12.9705 23.4689 12.4878 24.0168C12.0051 24.5647 11.7388 25.2698 11.7388 26C11.7388 26.7302 12.0051 27.4353 12.4878 27.9832C12.9705 28.5311 13.6364 28.8841 14.3608 28.9761C15.0851 29.0681 15.8182 28.8928 16.4225 28.483C17.0268 28.0731 17.461 27.457 17.6435 26.75H22.3565C22.5197 27.3937 22.8928 27.9646 23.4168 28.3724C23.9409 28.7802 24.586 29.0016 25.25 29.0016C25.914 29.0016 26.5591 28.7802 27.0832 28.3724C27.6072 27.9646 27.9804 27.3937 28.1435 26.75H29.75C29.9489 26.75 30.1397 26.671 30.2803 26.5303C30.421 26.3897 30.5 26.1989 30.5 26V20.75C30.5 20.6484 30.4794 20.5479 30.4393 20.4545ZM14.75 27.5C14.4533 27.5 14.1633 27.412 13.9167 27.2472C13.67 27.0824 13.4777 26.8481 13.3642 26.574C13.2507 26.2999 13.221 25.9983 13.2788 25.7074C13.3367 25.4164 13.4796 25.1491 13.6893 24.9393C13.8991 24.7296 14.1664 24.5867 14.4574 24.5288C14.7483 24.4709 15.0499 24.5006 15.324 24.6142C15.5981 24.7277 15.8324 24.92 15.9972 25.1666C16.162 25.4133 16.25 25.7033 16.25 26C16.2496 26.3977 16.0914 26.779 15.8102 27.0602C15.529 27.3414 15.1477 27.4996 14.75 27.5ZM25.25 16.25H27.005L28.613 20H25.25V16.25ZM25.25 27.5C24.9533 27.5 24.6633 27.412 24.4167 27.2472C24.17 27.0824 23.9777 26.8481 23.8642 26.574C23.7507 26.2999 23.721 25.9983 23.7788 25.7074C23.8367 25.4164 23.9796 25.1491 24.1893 24.9393C24.3991 24.7296 24.6664 24.5867 24.9574 24.5288C25.2483 24.4709 25.5499 24.5006 25.824 24.6142C26.0981 24.7277 26.3324 24.92 26.4972 25.1666C26.662 25.4133 26.75 25.7033 26.75 26C26.7496 26.3977 26.5914 26.779 26.3102 27.0602C26.029 27.3414 25.6477 27.4996 25.25 27.5ZM29 25.25H28.1435C27.9783 24.6076 27.6046 24.0381 27.0809 23.6309C26.5573 23.2238 25.9133 23.0018 25.25 23V21.5H29V25.25Z" fill="white"/>' +
                '</svg>';

            if (obterSvg)
                return svg;

            return obterIcon(svg);
        };

        this.pinReboque = function (obterSvg) {

            var svg =
                '<svg width="40" height="40" viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg">' +
                '<rect x="0.5" y="0.5" width="39" height="39" rx="19.5" fill="#38AAE1"/>' +
                '<rect x="0.5" y="0.5" width="39" height="39" rx="19.5" stroke="white"/>' +
                '<path d="M15.2818 23C16.4311 23 17.4683 24.9582 17.4683 26.056H16.7114C16.5712 23.5637 13.9643 23.267 13.1513 24.3945V24.4835H12.3945V24.3945C11.5815 23.2967 8.97454 23.5637 8.83438 26.056H8.07752C8.07752 24.9582 9.1147 23 10.264 23H15.2818Z" fill="white"/>' +
                '<path d="M13.0112 25.9966C13.0112 26.9758 13.7681 27.7769 14.6931 27.7769C15.6182 27.7769 16.375 26.9758 16.4031 25.9966C16.4311 25.0175 15.6462 24.1868 14.7212 24.1868C13.7961 24.1868 13.0112 24.9879 13.0112 25.9966Z" fill="white"/>' +
                '<path d="M9.28292 25.9966C9.28292 26.9758 10.0398 27.7769 10.9648 27.7769C11.8899 27.7769 12.6468 26.9758 12.6748 25.9966C12.6748 25.0175 11.9179 24.1868 10.9929 24.1868C10.0398 24.1868 9.28292 24.9879 9.28292 25.9966Z" fill="white"/>' +
                '<path d="M30.9983 13.0017C31.5512 13.0008 32 13.4488 32 14.0017V21.5618C32 22.1141 31.5523 22.5618 31 22.5618H9C8.44772 22.5618 8 22.1141 8 21.5618V14.0393C8 13.4877 8.44667 13.0403 8.99829 13.0393L30.9983 13.0017Z" fill="white"/>' +
                '</svg>';

            if (obterSvg)     
                return svg;

            return obterIcon(svg);
        };

        this.PinChegada = function (obterSvg) {

            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" width="38" height="48" viewBox="0 0 38 48" fill="none">' +
                '  <path d="M19 1.1875C28.8376 1.1875 36.8125 9.16243 36.8125 19C36.8125 23.9006 34.2893 29.8959 30.6406 35.1738C27.137 40.242 22.7633 44.4034 19 46.1943C15.2367 44.4034 10.863 40.242 7.35938 35.1738C3.71071 29.8959 1.1875 23.9006 1.1875 19C1.1875 9.16243 9.16243 1.1875 19 1.1875Z" fill="#FF7C7C" stroke="#FF4E4E" stroke-width="2.375"/>' +
                '  <path d="M13.7411 30.0268V7.97324L29.0089 15.6072L13.7411 23.2411" stroke="#FF4E4E" stroke-width="1.1875" stroke-linecap="round" stroke-linejoin="round"/>' +
                '</svg>';

            if (obterSvg)
                return svg;

            return obterIcon(svg);
        };

        this.PinLocal = function (obterSvg) {
            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" width="38" height="48" viewBox="0 0 38 48" fill="none">' +
                '  <path d="M19 1.1875C28.8376 1.1875 36.8125 9.16243 36.8125 19C36.8125 23.9006 34.2893 29.8959 30.6406 35.1738C27.137 40.242 22.7633 44.4034 19 46.1943C15.2367 44.4034 10.863 40.242 7.35938 35.1738C3.71071 29.8959 1.1875 23.9006 1.1875 19C1.1875 9.16243 9.16243 1.1875 19 1.1875Z" fill="#3EA7DB" stroke="#A2DAF6" stroke-width="2.375"/>' +
                '  <path d="M26.9166 15.0417H24.2777V11.7431C24.2777 11.5681 24.2082 11.4003 24.0845 11.2766C23.9608 11.1528 23.793 11.0833 23.618 11.0833H9.10413C8.92917 11.0833 8.76136 11.1528 8.63764 11.2766C8.51392 11.4003 8.44441 11.5681 8.44441 11.7431V23.6181C8.44441 23.793 8.51392 23.9608 8.63764 24.0846C8.76136 24.2083 8.92917 24.2778 9.10413 24.2778H9.76386V12.4028H22.9583V20.1215C23.2947 20.0357 23.6403 19.9914 23.9875 19.9896H24.2777V16.3611H26.9166C27.2666 16.3611 27.6022 16.5001 27.8496 16.7476C28.0971 16.995 28.2361 17.3306 28.2361 17.6806V18.3403H25.5972V19.6597H28.2361V22.9583H26.6527C26.4213 22.4207 26.0374 21.9627 25.5485 21.6409C25.0595 21.319 24.487 21.1475 23.9017 21.1475C23.3164 21.1475 22.7439 21.319 22.2549 21.6409C21.766 21.9627 21.3821 22.4207 21.1507 22.9583H16.6448C16.4177 22.4168 16.0351 21.9547 15.5454 21.6306C15.0558 21.3065 14.4809 21.1349 13.8937 21.1375C13.3358 21.1498 12.7933 21.3234 12.3319 21.6374C11.8705 21.9513 11.5099 22.3922 11.2936 22.9066C11.0774 23.4211 11.0147 23.9872 11.1133 24.5365C11.2118 25.0858 11.4673 25.5949 11.8488 26.0021C12.2304 26.4094 12.7218 26.6974 13.2635 26.8314C13.8053 26.9654 14.3742 26.9397 14.9017 26.7574C15.4291 26.575 15.8925 26.2438 16.2357 25.8038C16.579 25.3638 16.7875 24.8338 16.8361 24.2778H20.9791C21.063 24.9994 21.409 25.665 21.9515 26.1481C22.494 26.6312 23.1951 26.8981 23.9215 26.8981C24.6479 26.8981 25.349 26.6312 25.8915 26.1481C26.434 25.665 26.78 24.9994 26.8639 24.2778H28.8958C29.0708 24.2778 29.2386 24.2083 29.3623 24.0846C29.486 23.9608 29.5555 23.793 29.5555 23.6181V17.6806C29.5555 16.9807 29.2775 16.3095 28.7826 15.8146C28.2877 15.3197 27.6165 15.0417 26.9166 15.0417ZM13.8937 25.5972C13.6794 25.6111 13.4645 25.5809 13.2623 25.5085C13.0602 25.4361 12.875 25.3229 12.7183 25.176C12.5616 25.0292 12.4367 24.8517 12.3513 24.6547C12.2659 24.4576 12.2219 24.2452 12.2219 24.0304C12.2219 23.8156 12.2659 23.6032 12.3513 23.4061C12.4367 23.209 12.5616 23.0316 12.7183 22.8847C12.875 22.7379 13.0602 22.6247 13.2623 22.5523C13.4645 22.4798 13.6794 22.4496 13.8937 22.4636C14.108 22.4496 14.3229 22.4798 14.5251 22.5523C14.7273 22.6247 14.9124 22.7379 15.0691 22.8847C15.2258 23.0316 15.3507 23.209 15.4361 23.4061C15.5215 23.6032 15.5656 23.8156 15.5656 24.0304C15.5656 24.2452 15.5215 24.4576 15.4361 24.6547C15.3507 24.8517 15.2258 25.0292 15.0691 25.176C14.9124 25.3229 14.7273 25.4361 14.5251 25.5085C14.3229 25.5809 14.108 25.6111 13.8937 25.5972ZM23.9017 25.5972C23.5947 25.5817 23.2991 25.4765 23.0513 25.2945C22.8036 25.1125 22.6148 24.8619 22.5082 24.5736C22.4016 24.2853 22.382 23.972 22.4517 23.6727C22.5215 23.3734 22.6776 23.1011 22.9006 22.8896C23.1237 22.6781 23.4039 22.5368 23.7066 22.4831C24.0092 22.4294 24.321 22.4657 24.6032 22.5875C24.8854 22.7093 25.1256 22.9113 25.2941 23.1684C25.4626 23.4255 25.5519 23.7263 25.551 24.0337C25.5389 24.4595 25.3585 24.8632 25.0494 25.1562C24.7403 25.4493 24.3276 25.6079 23.9017 25.5972Z" fill="#E1F5FF"/>' +
                '</svg>';

            if (obterSvg)
                return svg;

            return obterIcon(svg);
        };

        this.wifiOn = function () {

            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">' +
                '  <path d="M2.06008 10.06C2.57008 10.57 3.38008 10.62 3.93008 10.16C8.60008 6.32002 15.3801 6.32002 20.0601 10.15C20.6201 10.61 21.4401 10.57 21.9501 10.06C22.5401 9.47002 22.5001 8.49002 21.8501 7.96002C16.1401 3.29002 7.88008 3.29002 2.16008 7.96002C1.51008 8.48002 1.46008 9.46002 2.06008 10.06ZM9.82008 17.82L11.2901 19.29C11.6801 19.68 12.3101 19.68 12.7001 19.29L14.1701 17.82C14.6401 17.35 14.5401 16.54 13.9401 16.23C13.3352 15.9194 12.665 15.7574 11.9851 15.7574C11.3051 15.7574 10.635 15.9194 10.0301 16.23C9.46008 16.54 9.35008 17.35 9.82008 17.82ZM6.09008 14.09C6.58008 14.58 7.35008 14.63 7.92008 14.22C9.11313 13.3759 10.5386 12.9226 12.0001 12.9226C13.4615 12.9226 14.887 13.3759 16.0801 14.22C16.6501 14.62 17.4201 14.58 17.9101 14.09L17.9201 14.08C18.5201 13.48 18.4801 12.46 17.7901 11.97C14.3501 9.48002 9.66008 9.48002 6.21008 11.97C5.52008 12.47 5.48008 13.48 6.09008 14.09Z" fill="#1F7C4A"/>' +
                '</svg>';
            return obterIcon(svg);
        };

        this.wifiOff = function () {

            var svg =
                '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">' +
                '<path d="M20.06 10.14C20.62 10.6 21.44 10.56 21.95 10.05C22.54 9.45999 22.5 8.47999 21.85 7.94999C18.26 5.00999 13.65 3.91999 9.29996 4.68999L11.89 7.27999C14.78 7.24999 17.69 8.19999 20.06 10.14ZM17.79 11.97C17.01 11.4 16.16 10.97 15.27 10.67L18.22 13.62C18.46 13.04 18.32 12.35 17.79 11.97ZM13.95 16.23C13.3451 15.9194 12.6749 15.7574 11.995 15.7574C11.315 15.7574 10.6448 15.9194 10.04 16.23C9.44996 16.54 9.33996 17.35 9.80996 17.82L11.28 19.29C11.67 19.68 12.3 19.68 12.69 19.29L14.16 17.82C14.65 17.35 14.55 16.54 13.95 16.23ZM19.68 17.9L4.11996 2.33999C4.02738 2.24741 3.91747 2.17397 3.79651 2.12387C3.67554 2.07376 3.54589 2.04797 3.41496 2.04797C3.28403 2.04797 3.15438 2.07376 3.03342 2.12387C2.91246 2.17397 2.80254 2.24741 2.70996 2.33999C2.61738 2.43258 2.54394 2.54249 2.49384 2.66345C2.44373 2.78442 2.41794 2.91406 2.41794 3.04499C2.41794 3.17592 2.44373 3.30557 2.49384 3.42654C2.54394 3.5475 2.61738 3.65741 2.70996 3.74999L5.04996 6.09999C4.03996 6.59999 3.05996 7.20999 2.15996 7.94999C2.00724 8.07597 1.88255 8.23249 1.7939 8.4095C1.70524 8.58652 1.65458 8.78011 1.64516 8.97786C1.63575 9.17561 1.66778 9.37314 1.73922 9.55778C1.81065 9.74242 1.9199 9.91008 2.05996 10.05C2.56996 10.56 3.37996 10.61 3.92996 10.15C4.92996 9.32999 6.02996 8.68999 7.17996 8.21999L9.40996 10.45C8.27996 10.75 7.19996 11.25 6.21996 11.96C5.52996 12.46 5.48996 13.47 6.08996 14.07L6.09996 14.08C6.58996 14.57 7.35996 14.62 7.92996 14.21C9.09511 13.3938 10.4776 12.9446 11.9 12.92L18.27 19.29C18.66 19.68 19.29 19.68 19.68 19.29C20.07 18.92 20.07 18.29 19.68 17.9Z" fill="#FF4E4E"/>' +
                '</svg>';
            return obterIcon(svg);
        };

    };

    this.setShapeDraggable = function (draggable) {
        shapeDraggable = draggable;
    };

    this.setShapeEditable = function (editable) {
        shapeEditable = editable;
    }

    this.getShapes = function () {
        return listShapes;
    }

    this.getShapesNotMarkers = function () {
        var shapes = []
        for (var i = 0; i < listShapes.length; i++) {
            if (listShapes[i].type != google.maps.drawing.OverlayType.MARKER) {
                shapes.push(listShapes[i]);
            }
        }
        return shapes;
    }
}
