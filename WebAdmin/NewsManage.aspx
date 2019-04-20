<%@ Page Title="" Language="C#" MasterPageFile="~/M1.Master" AutoEventWireup="true" CodeBehind="NewsManage.aspx.cs" Inherits="WebAdmin.NewsManage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <div class="row">
       <div class="col-sm-12">
           <button class="btn btn-sm btn-success" type="button" onclick="window.location.href= 'InsertNews.aspx'">新增公告</button>
       </div>
   </div>
   <div class="row">
       <table class="table table-hover" id="Demo">
            <thead>
                <tr>
                    <th>#</th>
                    <th>标题</th>
                    <th>创建时间</th>
                    <th>操作</th>
                </tr>
            </thead>
            <tbody id="UserListDom">
                
            </tbody>
        </table>
   </div>
    <script>
        $(function () {
            Utility.ajax.get(`User/GetNewsTitle`, function (data) {
                var html = ``;
                data.List.forEach(item => {
                    html += `
<tr>
    <th scope="row">${item.ID}</th>
    <td>${item.Title}</td>
    <td>${item.CreateTime}</td>
<td>
<button type="button" onclick="newsAction(0,${item.ID})">修改</button>
<button type="button" onclick="newsAction(1,${item.ID})">删除</button>
</td>
</tr>
`;
                });
                $("#UserListDom").html(html);
            });
        });

        function newsAction(action, ID) {
            if (action == 0) {
                window.location.href = `InsertNews.aspx?ID=${ID}`;
            } else {
                Utility.ajax.get(`User/DelNewsInfo?p_ID=${ID}`, function (data) {
                    window.location.reload();
                });
            }
        }


    </script>
</asp:Content>
