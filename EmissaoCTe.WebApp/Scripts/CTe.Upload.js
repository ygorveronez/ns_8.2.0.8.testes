function AbrirUploadPadrao(config) {
    var upload = PrepararUploadPadrao();
    var errors = [];
    var files  = [];

    config = $.extend({
        title: "Importação de arquivos",
        url: "",
        filter: [],
        multiple: true,
        max_file_size: '2000kb',
        onFinish: function(arquivos, erros){}
    }, config);
    
    var $upload = $("<div></div>", {class: "upload-container"});
    
    upload.$modal.modal("hide");

    upload.$title.text(config.title);
    upload.$body.append($upload);

    $upload.pluploadQueue({
        runtimes: 'html5,flash,gears,silverlight,browserplus',
        url: ObterPath() + config.url,
        max_file_size: config.max_file_size,
        unique_names: true,
        multi_selection: config.multiple,
        filters: {
            mime_types: config.filter
        },
        silverlight_xap_url: 'Scripts/plupload/Moxie.xap',
        flash_swf_url: 'Scripts/plupload/Moxie.swf',
        init: {
            FileUploaded: function (up, file, info) {
                try {
                    var raw = info.response.trim().replace(/^\?\(/, "").replace(/\);\s*$/, "");
                    var retorno = JSON.parse(raw);

                    if (retorno.Sucesso)
                        files.push(retorno.Objeto);
                    else
                        errors.push(retorno.Erro);
                } catch (e) {
                    errors.push("Erro ao processar " + file.name + ": " + e.message);
                }
            },
            StateChanged: function (up) {
                if (up.state != plupload.STARTED) {
                    config.onFinish(files, errors);
                    upload.$modal.modal("hide");
                }
            },
            FilesAdded: function (up, files) {
                if (config.multiple == false && up.files.length > 1)
                    up.splice(1, up.files.length);
            }
        }
    });

    upload.$modal.modal("show");

    return upload;
}

function PrepararUploadPadrao() {
    if("___UploadPadrao" in window)
        return window.___UploadPadrao;

    var $modal = $([
        '<div class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">',
            '<div class="modal-dialog modal-lg">',
                '<div class="modal-content">',
                    '<div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><h4 class="modal-title"></h4></div>',
                    '<div class="modal-body"></div>',
                '</div>',
            '</div>',
        '</div>'
    ].join(""));

    var upload = {}

    upload.$modal = $modal;
    upload.$title = $modal.find(".modal-title");
    upload.$body  = $modal.find(".modal-body");
    upload.arquivos = [];

    $("body").append($modal);

    window.___UploadPadrao = upload;

    $modal.on('hidden.bs.modal', function () {
        window.___UploadPadrao.arquivos = [];
        upload.$title.text("");
        upload.$body.find(".upload-container").remove();
    });

    return upload;
}