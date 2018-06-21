/**
* ASP实现web上传打印 for Windows
* 使用默认打印机打印 
* 对应的IIS应用池需要授权系统账号
* author @NewFuture
*/
using System;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading.Tasks;
using System.IO;

public partial class Print : System.Web.UI.Page
{

    /// <summary>
    /// 日志文件目录
    /// </summary>
    string LOG_DIR
    {
        get { return Server.MapPath("~/file/"); }
    }

    /// <summary>
    /// 文件目录
    /// </summary>
    string FIlE_DIR
    {
        get { return Server.MapPath("~/file/"); }
    }

    /// <summary>
    /// 上传密码
    /// </summary>
    protected string Password = System.Configuration.ConfigurationManager.AppSettings["password"];
    protected bool needPwd { get { return !String.IsNullOrEmpty(this.Password); } }
    /// <summary>
    /// 允许的文件后缀
    /// </summary>
    protected string[] AllowType = { ".pdf", ".jpg", ".png", ".tiff", ".doc", ".docx", ".rtf", ".txt" };
    protected void Page_Load()
    {
        if ("POST" == Request.HttpMethod)
        {
            //POST 上传
            Session["msg"] = this.OnPost();
            var url = Request.UrlReferrer;
            if (url == null)
            {
                See_Other(Request.RawUrl);
            }
            else
            {
                See_Other(url.ToString());
            }
        }
        else
        {
            if (!this.needPwd)
            {
                ///密码控件提示
                this.Message.Text = "<strong>注意:未设置打印密码任何人可直接打印</strong>";
            }
            this.Message.Text += Session["msg"];
            Session.Remove("msg");
            return;
        }

    }

    /// <summary>
    /// 处理POST请求的表单
    /// </summary>
    /// <returns></returns>
    protected string OnPost()
    {
        //验证密码
        if (this.needPwd)
        {
            if (this.Password.Trim() != Request.Form["password"])
            {
                return "打印密码无效";
            }
            else
            {
                //暂时保存密码
                Session["pwd"] = Request.Form["password"];
            }
        }

        if (Request.Files.Count < 0)
        {
            return "无文件( ▼-▼ )";
        }

        int copies = 0;
        if (!int.TryParse(Request.Form["copies"], out copies))
        {
            return "份数无效";
        }
        string range = this.GetRange(Request.Form["range"]);
        if (range == null)
        {
            return "打印页码范围[" + Request.Form["range"] + "] 格式错误！";
        }

        //逐个处理文件
        var files = Request.Files;
        string msg = "";
        for (int i = 0; i < files.Count; ++i)
        {
            HttpPostedFile file = files[i];
            if (this.Upload(file, copies, range))
            {
                msg += "<div style='color:green;'>" + file.FileName + "已添加到打印队列</div>";
            }
            else
            {
                msg += "<div style='color:green;'>" + file.FileName + "打印失败</div>";
            }
        }
        return msg;
    }

    /// <summary>
    /// 303重定向
    /// </summary>
    /// <param name="url"></param>
    public void See_Other(string url)
    {
        Response.RedirectLocation = url;
        Response.StatusCode = 303;
        Response.End();
    }

    /// <summary>
    /// 上传打印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected bool Upload(HttpPostedFile file, int copies, string range)
    {
        string type = GetType(file.FileName);
        if (type == null)
        {
            return false;
        }
        else
        {
            //保存并打印上传文件
            Random r = new Random();
            string path = FIlE_DIR + DateTime.UtcNow.ToString("MM-dd_HH-mm-ss_") + r.Next().ToString() + GetType(file.FileName);
            file.SaveAs(path);
            //打印
            return print(path, copies, range, type);
        }
    }

    /// <summary>
    /// 检查和获取后缀名
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    private string GetType(String filename)
    {
        string ext = System.IO.Path.GetExtension(filename).ToLower();
        return Array.IndexOf(AllowType, ext) == -1 ? null : ext;
    }

    private string GetRange(string range)
    {
        if (string.IsNullOrEmpty(range))
        {
            range = "";
        }
        else
        {
            string expr = @"^((\d+(\-\d+)?)\,)*\d+(\-\d+)?$";//^((\d+(\-\d+)?)\,)*\d+(\-\d+)?$
            range = range.Trim().Replace(' ', ',').Replace("，", ",").Trim(',');
            if (!Regex.IsMatch(range, expr))
            {
                return null;
            }
        }
        return range;
    }

    /// <summary>
    /// 打印
    /// </summary>
    /// <param name="path"></param>
    /// <param name="copy"></param>
    /// <param name="range">范围,1-5</param>
    /// <returns></returns>
    protected bool print(string path, int copies = 1, string range = null, string type = ".pdf")
    {
        string cmd, param;
        switch (type)
        {
            case ".pdf":
            case ".jpg":
            case ".png":
            case ".tiff":
                //pdf
                // https://www.sumatrapdfreader.org/docs/Command-line-arguments.html
                cmd = Server.MapPath("~/bin/") + "SumatraPDF.exe";
                param = string.Format("-print-to-default -silent -print-settings \"{0}x,{1}\"  -appdata \"{3}\" \"{2}\"", copies, range, path, FIlE_DIR);
                copies = 1;
                break;

            default:
                //word
                cmd = "write";
                param = string.Format("/p \"{0}\"", path);
                break;
        }
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = param,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            process.OutputDataReceived += async (s, e) => await log(e.Data, "log");
            process.ErrorDataReceived += async (s, e) => await log(e.Data, "error");
            while (copies-- > 0)
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            this.log(ex.ToString(), "exception");

        }
        return false;
    }

    /// <summary>
    /// 异步记录日志
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    protected async Task log(string msg, string type = "log")
    {
        if (String.IsNullOrEmpty(msg))
        {
            return;
        }
        string filePath = LOG_DIR + type + ".txt";
        byte[] encodedText = System.Text.Encoding.Unicode.GetBytes(msg + "\n\r");

        using (FileStream sourceStream = new FileStream(filePath,
            FileMode.Append, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }
}
