<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebAdmin.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="assets/css/bootstrap.css" rel="stylesheet" />
</head>
<body class="container">
   <form>
       <div class="row">UserName:<input type="text" id="username"/>

       </div>
       <div class="row">UserPwd:<input type="password" id="userpwd"/>

       </div>
       <div class="row">
           <button type="button" id="BtnLogin">登录</button>

       </div>
   </form>
</body>
</html>
<script src="assets/js/core/jquery.min.js"></script>
<script src="assets/js/Domain.Utility.js"></script>
<script src="assets/js/bootbox.js"></script>
<script src="assets/js/core/bootstrap.min.js"></script>
<script>
    $(function () {
        $("#BtnLogin").on("click", function () {
            var username = $("#username").val().trim();
            var userpwd = $("#userpwd").val().trim();
            var users = [
                "13681525763", "17710328707", "13484001955"
            ];
            if (users.indexOf(username) != -1) {
                Utility.Style.setLoading(true);

                var _sendPost = new Object();
                _sendPost.p_UserName = username;
                _sendPost.p_UserPwd = userpwd;

                Utility.ajax.post(`User/Exist`, _sendPost, function (data) {
                    Utility.Style.setLoading(false);
                    if (data.IsExist) {
                        window.localStorage.setItem("User", data.User.UserPhone);
                        window.location.href = "/Default.aspx";

                    } else {
                        bootbox.alert("密码错误,请重新输入");
                    }
                });

                //bootbox.alert("登录成功");
            } else {
                bootbox.alert("该用户没有管理权限");
            }
        });
    })
</script>
