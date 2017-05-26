<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html>
<html>

<head>
    <meta charset="UTF-8">
    <title>在线打印共享</title>
    <link rel="stylesheet" href="style.css">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
</head>

<body>
    <h1>打印文件</h1>
    <main class="agile-its">
        <h2>上传文件</h2>
        <div class="print">
            <div class="header">
                <div class="tips">
                    <span>格式:</span>
                    <ul class="listtype">
                        <li>PDF(.pdf)</li>
                        <li>图片(.png .jpg .tiff)</li>
                        <li>文本(.txt)</li>
                        <li>Word(.doc .docx .rtf)</li>
                    </ul>
                </div>
                <div class="warn">WORD文档暂不支持<code>"选页"</code>设置,可能存在排版兼容问题</div>
                <div id="messages">
                    <asp:Label ID="Message" runat="server"></asp:Label></div>
            </div>
            
            <form id="upload" method="POST" enctype="multipart/form-data">
                <div class="agileinfo">
                    <div id="filedrag">
                        <span class="uploadtip">点击上传文件<br />
                            或者拖拽至此</span>
                    </div>
                    <input type="file" id="files" name="files[]" multiple="multiple" required="required" accept=".pdf,.doc,.docx,.rtf,.txt,image/*" />
                </div>
                <div class="agileinfo inputbox">
                    <%--<input name="password" type="password" placeholder="打印密码" required />--%>
                    <asp:TextBox runat="server" TextMode="Password" name="password" ID="password" placeholder="打印密码" required="required"></asp:TextBox>
                </div>
                <div class="agileinfo inputbox" id="copies">
                    <input name="copies" type="number" value="1" placeholder="设置份数" required />
                </div>
                <div class="agileinfo inputbox">
                    <input name="range" type="text" title="页码范围如:2-8 或1,3,5" placeholder="PDF页码:2-5或3,5 (默认所有页)" />
                </div>
                <button type="submit" id="submit" disabled>提交</button>
            </form>
        </div>
    </main>
    <footer>
        <p>© New Future | <a href="https://github.com/NewFuture/WebPrint">Source Code</a></p>
    </footer>
    <script src="file.js"></script>

</body>

</html>
