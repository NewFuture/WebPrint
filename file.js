//上传控件处理
(function () {
    function $id(id) {
        return document.getElementById(id);
    }
    var ALLOWED_EXT = new Array(".png", ".jpg", ".jpeg", ".tiff", ".pdf", ".doc", ".docx", ".rtf", ".txt");
    // output information
    function Output(msg) {
        $id("filedrag").innerHTML += msg;
    }

    function CleanMsg() {
        $id("filedrag").innerHTML = '';
        $id("messages").innerHTML = '';
    }

    function Msg(msg) {
        $id("messages").innerHTML += '<small>' + msg + '</small><br/>';
    }
    // file drag hover
    function FileDragHover(e) {
        e.stopPropagation();
        e.preventDefault();
        e.target.className = (e.type == "dragover" ? "hover" : "");
    }

    // file selection
    function FileSelectHandler(e) {

        // cancel event and hover styling
        FileDragHover(e);

        // fetch FileList object
        var files = e.target.files || e.dataTransfer.files;

        // process all File objects
        CleanMsg();
        var upfiles = [];
        //var delfiles = [];
        var updated = false;
        for (var i = 0; i < files.length; i++) {
            if (ParseFile(files[i])) {
                upfiles.push(i);
            } else {
                Msg("无效文件:" + files[i].name);
                alert(files[i].name + "\n 文件格式不支持");
                upfiles = [];
                break;
            }
        }
        upfiles.forEach(function (i) {
            var file = files[i];
            Output(
				"<p>文件: <strong>" + file.name + "</strong><br/>" +
				" [<samll>" + file.size + "</samll> bytes]" +
				" 类型: <samll>" + (file.type || "未知") + "</samll></p>"
			);
        });
        if (upfiles.length > 0) {
            Msg("总计上传" + upfiles.length + "个文件");
            $id("submit").disabled = false;
        } else {
            $id("files").value = "";
            $id("submit").disabled = true;
            Output('<span class="uploadtip">点击上传文件<br/>或者拖拽至此<br/>支持多个文件</span>');
        }
    }

    function ParseFile(file) {
        var name = file.name;
        var ext = name.substring(name.lastIndexOf('.'));
        for (var i = 0; i < ALLOWED_EXT.length; ++i) {
            if (ALLOWED_EXT[i] == ext) {
                return true;
            }
        }
        return false;
    }

    // initialize
    function Init() {
        var fileselect = $id("files"),
			filedrag = $id("filedrag");
        // file select
        fileselect.addEventListener("change", FileSelectHandler, false);

        // file drop
        filedrag.addEventListener("dragover", FileDragHover, false);
        filedrag.addEventListener("dragleave", FileDragHover, false);
        filedrag.addEventListener("drop", FileSelectHandler, false);
        filedrag.style.display = "block";
    }

    // call initialization file
    if (window.File && window.FileList && window.FileReader) {
        Init();
    }
})();