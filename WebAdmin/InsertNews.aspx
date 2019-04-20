<%@ Page Title="" Language="C#" MasterPageFile="~/M1.Master" AutoEventWireup="true" CodeBehind="InsertNews.aspx.cs" Inherits="WebAdmin.InsertNews" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="xheditor-1.2.2.min.js"></script>
    <script src="xheditor_lang/zh-cn.js"></script>
    <div class="row">
        <div class="form-group">
            <div class="col-sm-1">标题</div>
            <div class="col-sm-9">
                <input type="text" id="Title" />
            </div>
        </div>

    </div>
    <div class="row" style="margin-top:30px;">
        <div class="form-group">
            <div class="col-sm-1">内容</div>
            <div class="col-sm-9">
                <textarea id="elm4" name="elm4" class="xheditor-mini" rows="12" cols="80" style="width: 80%"></textarea>
            </div>
        </div>
    </div>

     <div class="row" style="margin-top:30px;">
        <div class="form-group">
            <div class="col-sm-1"></div>
            <div class="col-sm-9"><button type="button" id="Btn_submit" class="btn btn-sm btn-success">提交</button></div>
            <input  type="hidden" id="ID" value="0"/>
        </div>
    </div>


    <script type="text/javascript">
        $(function () {

            function getUrlParam(name) {
                var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
                var r = window.location.search.substr(1).match(reg);  //匹配目标参数
                if (r != null) return unescape(r[2]); return null; //返回参数值
            }

            var id = getUrlParam("ID");
            if (id != null) {
                $("#ID").val(id);
                Utility.ajax.get(`User/GetNewsInfo?p_ID=${id}`, function (data) {
                    data = data.data;
                    $("#Title").val(data.Title);
                    $("#elm4").val(data.Description);    
                });
            }

            $("#Btn_submit").click(function () {
                var title = $("#Title").val().trim();
                var desc = $("#elm4").val().trim();
                var sendObject = new Object();
                sendObject.ID = $("#ID").val();
                sendObject.Title = title;
                sendObject.Description = desc;
                Utility.ajax.post(`User/InsertNews`, sendObject, function (data) {
                    alert("成功");
                    window.location.href = "NewsManage.aspx";
                });
            });
        });
    </script>
</asp:Content>
