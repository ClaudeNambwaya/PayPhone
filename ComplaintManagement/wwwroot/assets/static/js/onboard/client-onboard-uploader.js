$(function () {
    /*
     * For the sake keeping the code clean, this file
     * contains only the plugin configuration & callbacks.
     * 
     * UI functions ui_* can be located in: debbuger.js
     */
    $('#drag-and-drop-zone').dmUploader({ //
        url: '/ClientManagement/Upload',
        extFilter: ["doc", "docx", "pdf", "jpg", "jpeg", "png", "gif"],
        fieldName: 'postedFiles',
        maxFileSize: 3000000, // 3 Megs 
        onDragEnter: function () {
            // Happens when dragging something over the DnD area
            this.addClass('active');
        },
        onDragLeave: function () {
            // Happens when dragging something OUT of the DnD area
            this.removeClass('active');
        },
        onInit: function () {
            // Plugin is ready to use
            ui_add_log('Ready to upload', 'info');
        },
        onComplete: function () {
            // All files in the queue are processed (success or error)
            //ui_add_log('All pending tranfers finished');
        },
        onNewFile: function (id, file) {
            // When a new file is added using the file selector or the DnD area
            //ui_add_log('New file added ' + file.name);
            ui_multi_add_file(id, file, 'uploaderFile', 'files');
            //$('#uploadedFiles').append(file.name + '|<br />');
        },
        onBeforeUpload: function (id) {
            // about to start uploading a file
            //ui_add_log('Starting the upload of #' + id);
            ui_multi_update_file_status(id, 'uploading', 'Uploading...', 'uploaderFile');
            ui_multi_update_file_progress(id, 0, '', true, 'uploaderFile');
        },
        onUploadCanceled: function (id) {
            // Happens when a file is directly canceled by the user.
            ui_multi_update_file_status(id, 'warning', 'Canceled by User', 'uploaderFile');
            ui_multi_update_file_progress(id, 0, 'warning', false, 'uploaderFile');
        },
        onUploadProgress: function (id, percent) {
            // Updating file progress
            ui_multi_update_file_progress(id, percent, '', true, 'uploaderFile');
        },
        onUploadSuccess: function (id, data) {
            console.log(data);
            // A file was successfully uploaded
            var json = $.parseJSON(JSON.stringify(data));
            for (var i = 0; i < json.length; i++) {
                var item = json[i];
                var li = '<li>' + item.new_file_name + '</li>';
                $("#uploadedFilesList ul").append(li);

                if (i === 0)
                    $('#uploadedFiles').append(item.new_file_name.trim() + '|<br />');
                else
                    $('#uploadedFiles').append('|' + item.new_file_name);

                //ui_add_log('Server Response for file #' + item.new_file_name);
                ui_add_log('Upload of file <b>' + item.original_file_name + '</b> COMPLETED', 'success');
                ui_multi_update_file_status(id, 'success', 'Upload Complete', 'uploaderFile');
                ui_multi_update_file_progress(id, 100, 'success', false, 'uploaderFile');



                //var item = json[i];
                //$('#uploadedFiles').append(item.new_file_name.trim() + '|<br />');
                //document.getElementById("label_national_id").innerHTML = item.original_file_name;
                //document.getElementById("label_text_national_id").value = item.new_file_name;
                //ui_add_log('Server Response for file #' + id + ': ' + item.new_file_name);
                //ui_add_log('Upload of file #' + id + ' COMPLETED', 'success');
                //ui_multi_update_file_status(id, 'success', 'Upload Complete', 'uploaderFile-ids');
                //ui_multi_update_file_progress(id, 100, 'success', false, 'uploaderFile-ids');








            }
            //console.log(document.getElementById("uploadedFiles").innerHTML.trim());
        },
        onUploadError: function (id, xhr, status, message) {
            ui_multi_update_file_status(id, 'danger', message, 'uploaderFile');
            ui_multi_update_file_progress(id, 0, 'danger', false, 'uploaderFile');
        },
        onFallbackMode: function () {
            // When the browser doesn't support this plugin :(
            ui_add_log('Plugin cant be used here, running Fallback callback', 'danger');
        },
        onFileTypeError: function (file) {
            ui_add_log('File \'' + file.name + '\' cannot be added: invalid file type', 'danger');

            RemoveFromUploadedFiles(file.name, "File " + file.name + " cannot be added: invalid file type");
        },
        onFileExtError: function (file) {
            ui_add_log('File \'' + file.name + '\' cannot be added: invalid file extension', 'danger');

            RemoveFromUploadedFiles(file.name, "File " + file.name + " cannot be added; invalid file extension, allowed extensions are doc, docx, pdf, jpg, jpeg, png and gif");
        },
        onFileSizeError: function (file) {
            ui_add_log('File \'' + file.name + '\' cannot be added: size excess limit', 'danger');

            RemoveFromUploadedFiles(file.name, "File " + file.name + " cannot be added; size limit exceeded, file size is " + (file.size / 1000000).toFixed(2) + " MB while maximum allowed is 3 MB");
        }
    });

    function RemoveFromUploadedFiles(file_to_replace, message) {
        var container = document.getElementById('uploadedFiles');
        var client_files = container.textContent.trim();
        client_files = client_files.replace(file_to_replace + '|', '');
        document.getElementById('uploadedFiles').innerHTML = client_files;

        Swal.fire({
            title: "Information",
            text: message,
            icon: "warning",
            confirmButtonText: "Ok"
        });
    }
});