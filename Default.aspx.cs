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

public partial class Default : System.Web.UI.Page
{
    /// <summary>
    /// 上传密码
    /// </summary>
    protected string Password = "打印密码";
    /// <summary>
    /// 允许的文件后缀
    /// </summary>
    protected string[] AllowType = { ".pdf", ".jpg", ".png", ".tiff" };

    /// <summary>
    /// 上传打印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void upButton_Click(object sender, EventArgs e)
    {
        if (FileUpload.HasFile)
        {
            LabelMsg.ForeColor = System.Drawing.Color.Red;
            if (pwdTextBox.Text != Password)
            {
                LabelMsg.Text = "上传密码无效!";
            }
            else if (GetType(FileUpload.FileName) == null)
            {
                LabelMsg.Text = "上传失败，不是PDF或者指定的图片文件：" + FileUpload.FileName;
                return;
            }
            else
            {
                //打印范围
                string range = GetRange();
                if (range == null)
                {
                    LabelMsg.Text = FileUpload.FileName + "打印页码范围 "+rangeInput.Text+" 格式错误！";
                    return;
                }
                //保存并打印上传文件
                Random r = new Random();
                string path = Server.MapPath("~/file/") + DateTime.UtcNow.ToString("MM-dd_HH-mm-ss_") + r.Next().ToString() + GetType(FileUpload.FileName);
                FileUpload.SaveAs(path);
                int copies = 1;
                int.TryParse(copy.Text, out copies);

                //打印
                if (print(path, copies, range))
                {
                    LabelMsg.Text = FileUpload.FileName + "[" + copies + "份 " + range + "]已经添加至打印队列~";
                    LabelMsg.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    LabelMsg.Text = FileUpload.FileName + "上传完成，但是打印出错！";
                }
            }
        }
        else
        {
            LabelMsg.Text = "无文件( ▼-▼ )";
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
    private string GetRange()
    {
        string range = rangeInput.Text ?? "";
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
    /// https://github.com/sumatrapdfreader/sumatrapdf/wiki/Command-line-arguments
    /// </summary>
    /// <param name="path"></param>
    /// <param name="copy"></param>
    /// <param name="range">范围,1-5</param>
    /// <returns></returns>
    protected bool print(string path, int copy = 1, string range = null)
    {
        try
        {
            var cmd = Server.MapPath("~/bin/") + "SumatraPDF.exe";
            var param = string.Format("-print-to-default -print-settings \"{0}x,{1}\" \"{2}\"", copy, range, path);
            var p = Process.Start(cmd, param);
            return true;
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
        return false;
    }
}
