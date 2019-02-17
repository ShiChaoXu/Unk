<%@ Page Title="" Language="C#" MasterPageFile="~/M1.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebAdmin.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">

        <div class="col-sm-6 col-lg-3">
            <a class="card" href="javascript:void(0)">
                <div class="card-block clearfix">
                    <div class="pull-right">
                        <p class="h6 text-muted m-t-0 m-b-xs">已发行(份)</p>
                        <p class="h3 text-blue m-t-sm m-b-0" id="g_IconTotal">0</p>
                    </div>
                    <div class="pull-left m-r">
                        <span class="img-avatar img-avatar-48 bg-blue bg-inverse"><i class="ion-ios-bell fa-1-5x"></i></span>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-sm-6 col-lg-3">
            <a class="card bg-green bg-inverse" href="javascript:void(0)">
                <div class="card-block clearfix">
                    <div class="pull-right">
                        <p class="h6 text-muted m-t-0 m-b-xs">用户总量</p>
                        <p class="h3 m-t-sm m-b-0" id="g_UserTotal">0</p>
                    </div>
                    <div class="pull-left m-r">
                        <span class="img-avatar img-avatar-48 bg-gray-light-o"><i class="ion-ios-people fa-1-5x"></i></span>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-sm-6 col-lg-3">
            <a class="card bg-blue bg-inverse" href="javascript:void(0)">
                <div class="card-block clearfix">
                    <div class="pull-right">
                        <p class="h6 text-muted m-t-0 m-b-xs">今日涨幅</p>
                        <p class="h3 m-t-sm m-b-0">￥<span id="g_CurrentPirce">0.00</span></p>
                    </div>
                    <div class="pull-left m-r">
                        <span class="img-avatar img-avatar-48 bg-gray-light-o"><i class="ion-ios-speedometer fa-1-5x"></i></span>
                    </div>
                </div>
            </a>
        </div>


        <div class="col-sm-6 col-lg-3">
            <a class="card bg-purple bg-inverse" href="javascript:void(0)">
                <div class="card-block clearfix">
                    <div class="pull-right">
                        <p class="h6 text-muted m-t-0 m-b-xs">新增会员</p>
                        <p class="h3 m-t-sm m-b-0" id="g_CurrentUser">0</p>
                    </div>
                    <div class="pull-left m-r">
                        <span class="img-avatar img-avatar-48 bg-gray-light-o"><i class="ion-ios-email fa-1-5x"></i></span>
                    </div>
                </div>
            </a>
        </div>

    </div>

    <div class="row">
        <form class="form-inline form-group ">
            <div class="col-xs-12">
                <input type="email" class="form-control" id="s_UserName" placeholder="手机或姓名">
                <button type="button" class="btn btn-primary" id="BtnSearch">搜索</button>
                <button type="button" class="btn btn-danger" id="BtnAdd">新增</button>
            </div>
        </form>
    </div>
    <div class="row">
        <table class="table table-hover" id="Demo">
            <thead>
                <tr>
                    <th>#</th>
                    <th>姓名</th>
                    <th>手机号</th>
                    <th>当前持有(UNK)</th>
                    <th>推荐人</th>
                    <th>创建日期</th>
                    <th>操作</th>
                </tr>
            </thead>
            <tbody id="UserListDom">
                
            </tbody>
        </table>
    </div>

    <script>

        var RegTemplate = `
<div class="form-horizontal" id="RegUserForm">

        <div class="form-group">
            <label class="col-sm-2 control-label">姓名</label>
            <div class="col-xs-10">
                <input class="form-control" placeholder="用户姓名" id="UserName"/>
            </div>
        </div>

         <div class="form-group">
            <label class="col-sm-2 control-label">手机</label>
            <div class="col-xs-10">
                <input class="form-control" placeholder="用户手机" id="UserPhone"/>
            </div>
        </div>

         <div class="form-group">
            <label class="col-sm-2 control-label">登录密码</label>
            <div class="col-xs-10">
                <input class="form-control" placeholder="登录密码" id="UserPwd" type="text" value="123456"/>
            </div>
        </div>

           <div class="form-group">
            <label class="col-sm-2 control-label">推荐人</label>
            <div class="col-xs-10">
                <input class="form-control" placeholder="推荐人姓名" id="UserRef" type="text"/>
            </div>
        </div>

        <div class="clearfix"></div>

    </div>
`;
        var UserListEmptyTemplate = `
<tr>
    <td colspan="7" style="text-align:center;">输入 [手机或姓名] 后显示用户信息</td>
</tr>
`;
        var EditTemplate = `
   <div class="form-horizontal" id="RegUserForm">

        <div class="form-group">
            <label class="col-sm-2 control-label">姓名</label>
            <div class="col-xs-10">
                <input class="form-control" placeholder="用户姓名" id="UserName" value="{0}"/>
            </div>
        </div>

        <div class="form-group">
            <label class="col-sm-2 control-label">手机</label>
            <div class="col-xs-10">
                <input class="form-control" placeholder="用户手机" id="UserPhone" value="{1}"/>
            </div>
        </div>

        <div class="form-group">
            <label class="col-sm-2 control-label">登录密码</label>
            <div class="col-xs-10">
                <input class="form-control" placeholder="登录密码" id="UserPwd" type="text" value="{2}" />
            </div>
        </div>

        <div class="form-group">
            <label class="col-sm-2 control-label">推荐人</label>
            <div class="col-xs-10">
                <input class="form-control" placeholder="推荐人姓名" id="UserRef" type="text" value="{3}" disabled/>
            </div>
        </div>

        <div class="form-group">
            <label class="col-sm-2 control-label">持有UNK</label>
            <div class="col-xs-10">
                <input class="form-control" placeholder="当前持有(份)" id="UserTotal" type="text" value="{4}"/>
            </div>
        </div>

        <div class="clearfix"></div>

    </div>
`;

        $(function () {

            $("#UserListDom").html(UserListEmptyTemplate);

            Utility.ajax.get(`User/DashBoard`, function (data) {
                if (data) {
                    var totalUnk = data.totalUnk;
                    totalUnk = totalUnk.substr(0, totalUnk.indexOf(".") + 3);
                    $("#g_IconTotal").html(Utility.convert.comdify(totalUnk));
                    $("#g_UserTotal").html(data.totalUser);
                    $("#g_CurrentPirce").html(data.currentPrice);
                    $("#g_CurrentUser").html(data.totalCurrentUser);
                }
            });

            $("#BtnAdd").on("click", function (data) {
                bootbox.dialog({
                    size: "large",
                    title: "新增会员",
                    message: RegTemplate,
                    buttons: {
                        "cancel": {
                            label: "取消"
                        },
                        "success": {
                            label: "创建",
                            className: "btn btn-primary",
                            callback: function (data) {
                                if (data) {
                                    var userForm = new Object();
                                    userForm.CardName = $("#UserName").val().trim();
                                    userForm.UserName = $("#UserPhone").val().trim();
                                    userForm.UserPwd = $("#UserPwd").val().trim();
                                    userForm.pUserID = $("#UserRef").val().trim();
                                    userForm.CardNo = "";

                                    if (userForm.CardName.length < 1 || userForm.pUserID.length <= 1 || userForm.UserName.length <= 1 || userForm.UserPwd.length <= 1) {
                                        bootbox.alert("表单所有项目为必填字段,请输入");
                                        return false;
                                    }

                                    Utility.Style.setLoading(true);
                                    Utility.ajax.post(`User/RegUserByAdmin`, userForm, function (data) {
                                        Utility.Style.setLoading(false);
                                        if (!data.IsExist) {
                                            bootbox.alert(data.Message, function () {
                                                $("#BtnAdd").trigger("click");
                                            });
                                        } else {
                                            bootbox.alert(`${userForm.CardName} -- 注册成功`, function () {
                                                window.location.href = "/Default.aspx";
                                            });
                                        }
                                        return false;
                                    });

                                    return false;


                                }
                            }
                        },
                    }
                });
            });
            $("#BtnSearch").on("click", function () {
                var v_UserName = $("#s_UserName").val().trim();
                if (v_UserName.length == 0) {
                    bootbox.alert("请输入手机或者姓名");
                    return;
                }
                Utility.Style.setLoading(true);
                Utility.ajax.get(`User/GetAllDetails`, function (data) {
                    Utility.Style.setLoading(false);
                    var r_List = data.filter(x => x.UserName.indexOf(v_UserName) != -1 || x.UserPhone.indexOf(v_UserName) != -1);
                    if (r_List.length == 0) {
                        $("#UserListDom").html(UserListEmptyTemplate);
                    } else {
                        var v_html = '';
                        for (var i = 0; i < r_List.length; i++) {
                            v_html += RenderUserList(r_List[i]);
                        }
                        $("#UserListDom").html(v_html);
                    }
                });
            });

            function RenderUserList(p_User) {
                var TotalUNK = p_User.TotalUNK.substr(0, p_User.TotalUNK.indexOf(".") + 3)
                var tr = `
<tr>
                    <th scope="row">${p_User.ID}</th>
                    <td>${p_User.UserName}</td>
                    <td>${p_User.UserPhone}</td>
                    <td>${Utility.convert.comdify(TotalUNK)}</td>
                    <td>${p_User.ReferrerName}</td>
                    <td>${p_User.CreateTime.replace('T', ' ')}</td>
                    <td>
                        <button type="button" class="btn btn-warning" onclick="BtnEdit(this)" data-UserName='${p_User.UserName}' data-UserID='${p_User.ID}' >修改</button>
                        <button type="button" class="btn btn-info" onclick="BtnDel(this)" data-UserName='${p_User.UserName}' data-UserID='${p_User.ID}' >销毁</button>
                    </td>
                </tr>
`;
                return tr;
            }

        });
        function BtnEdit(obj) {
            Utility.Style.setLoading(true);
            Utility.ajax.get(`User/GetAllDetails`, function (data) {
                Utility.Style.setLoading(false);
                var rList = data.filter(x => x.ID == obj.dataset.userid);
                if (rList.length > 0) {
                    var _obj = rList[0];
                    console.log(_obj);
                    bootbox.dialog({
                        size: "large",
                        title: "修改信息",
                        message: Utility.convert.Format(EditTemplate, _obj.UserName, _obj.UserPhone, _obj.UserPwd, _obj.ReferrerName, _obj.TotalUNK),
                        buttons: {
                            "cancel": {
                                label:"取消"
                            },
                            "success": {
                                label:"更新",
                                className: 'btn btn-danger',
                                callback: function (data) {
                                    var userForm = new Object();
                                    userForm.CardName = $("#UserName").val().trim();
                                    userForm.UserName = $("#UserPhone").val().trim();
                                    userForm.UserPwd = $("#UserPwd").val().trim();
                                    userForm.TotalUNK = $("#UserTotal").val().trim();
                                    userForm.ID = obj.dataset.userid;
                                    Utility.Style.setLoading(true);
                                    Utility.ajax.post("User/UpdateAccountAndAsset", userForm, function (data) {
                                        Utility.Style.setLoading(false);
                                        bootbox.alert("修改成功", function () {
                                            window.location.href = "/Default.aspx";
                                        });
                                    });
                                    return false;

                                }
                            }
                        }
                    });




                } else {
                    boolean.alert("系统繁忙");
                }
                return false;
            });
        }

        function BtnDel(obj) {
            console.log(obj.dataset);
            bootbox.confirm(`是否确认销毁[ ${obj.dataset.username} ], 销毁后<span style="color:red;font-weight:bolder;">不可恢复(包含用户资产)</span>!`, function (data) {
                if (data) {
                    Utility.Style.setLoading(true);
                    Utility.ajax.get(`User/DeleteData?p_userID=${obj.dataset.userid}`, function (data) {
                        Utility.Style.setLoading(false);
                        bootbox.alert(`${obj.dataset.username} 已销毁数据`, function (data) {
                            window.location.href = "/Default.aspx";
                        });
                    });
                    return false;
                }
            });
        }

    </script>

</asp:Content>
