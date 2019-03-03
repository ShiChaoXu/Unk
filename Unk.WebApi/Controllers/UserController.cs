using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unk.Biz;
using Unk.Biz.Entity;
using Unk.Core;
using Unk.Core.ViewModel;

namespace Unk.WebApi.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly UserInfoBiz g_UserInfoBiz = new UserInfoBiz();
        private readonly GainsInHistoryBiz g_GainsInHistoryBiz = new GainsInHistoryBiz();
        private readonly GlobalBiz g_GlobalBiz = new GlobalBiz();

        [HttpGet]
        public object GetAll()
        {
            var v_rList = g_UserInfoBiz.GetUserList();
            return new
            {
                Data = v_rList
            };
        }

        [HttpGet]
        public object GetAllDetails()
        {
            var v_rList = g_UserInfoBiz.GetUserListDetails();
            return new
            {
                Data = v_rList
            };
        }

        [HttpPost]
        public object Exist([FromBody] UserViewModels p_UserView)
        {
            UserInfoEntity v_user = CacheHelper.GetCache(CacheHelper.USER + p_UserView.p_UserName) as UserInfoEntity;
            List<TokenDetailsEntity> tokenList = new List<TokenDetailsEntity>();
            if (v_user == null)
            {
                v_user = g_UserInfoBiz.UserLogin(p_UserView.p_UserName, p_UserView.p_UserPwd);
                if (v_user != null)
                {
                    tokenList = g_GainsInHistoryBiz.GetUserTokenList(v_user.ID);
                }
            }
            return new {
                Data = new
                {
                    IsExist = v_user != null,
                    User = v_user,
                    TokenList = tokenList
                }
            };
        }

        [HttpGet]
        public object GetUserByPhone(string p_phone)
        {
            return new
            {
                Data = g_UserInfoBiz.GetUserByPhone(p_phone)
            };
        }

        [HttpGet]
        public object GetUserSingle(string p_id)
        {
            return new
            {
                Data = g_UserInfoBiz.GetUserSingle(p_id)
            };
        }

        [HttpGet]
        public object GetTokenSummary() {
            return new
            {
                Data = g_GainsInHistoryBiz.GetEveryDataPrice()
            };
        }

        [HttpGet]
        public object GetTokenTotalCount(string p_type, string p_userid)
        {
            return new {
                Data = new
                {
                    TotalCount = g_GainsInHistoryBiz.GetTokenCount(p_type, p_userid),
                    HasSignIn = !g_GainsInHistoryBiz.CheckHasSign(p_type, p_userid)
                }
            };
        }

        [HttpGet]
        public object GetGainsInHisotry(string p_type) {
            return new
            {
                Data = g_GainsInHistoryBiz.GetGainsInHisotryList(p_type)
            };
        }

        [HttpPost]
        public object UpdateAccountCoin([FromBody] UpdateAccountModels p_UserView)
        {
            return new
            {
                Data = g_GainsInHistoryBiz.UpdateAccountCoin(p_UserView.p_type, p_UserView.p_userid, p_UserView.p_total, p_UserView.p_desc)
            };
        }

        [HttpPost]
        public object UpdateUserIDInfo([FromBody] UserInfoEntity p_UserView) {

            return new
            {
                Data = g_UserInfoBiz.UpdateUserIDInfo(p_UserView)
            };
        }
        [HttpPost]
        public object UpdateUserKeyWord([FromBody] UserInfoEntity p_UserView) {
            return new
            {
                Data = g_UserInfoBiz.UpdateUserKeyWords(p_UserView)
            };
        }

        public object UpdateUserPwd([FromBody] UserInfoEntity p_UserView) {
            return new
            {
                Data = g_UserInfoBiz.UpdateUserPwd(p_UserView, p_UserView.PayPassWord.Length > 0)
            };
        }


        [HttpPost]
        public object RegNewUser([FromBody] RegNewUserModels p_UserView)
        {
            bool status = false;
            string message = "";
            if (g_UserInfoBiz.GetUserSingle(p_UserView.pUserID) == null)
            {
                status = false;
                message = "二维码已失效,请重新联系推荐人";
            }
            else
            {
                if (g_UserInfoBiz.GetUserByPhone(p_UserView.UserName) == null)
                {
                    if (g_UserInfoBiz.RegUser(p_UserView) == false)
                    {
                        status = false;
                        message = "系统故障, 请联系管理员";
                    }
                    else
                    {
                        status = true;
                        message = $"{p_UserView.UserName} 注册成功, 请登录系统!";
                    }
                }
                else {
                    status = false;
                    message = $"{p_UserView.UserName} 该用户已存在, 请输入新手机号.";
                }
            }
            return new
            {
                Data = status,
                Message = message
            };
        }

        [HttpPost]
        public object RegUserByAdmin([FromBody] RegNewUserModels p_UserView)
        {
            // 先判断推荐人是否存在.
            // 在判断当前注册人是否存在
            UserInfoEntity v_User = g_UserInfoBiz.GetUserByName(p_UserView.pUserID);
            if (v_User != null)
            {
                var v_regUser = g_UserInfoBiz.GetUserByName(p_UserView.CardName);
                if (v_regUser == null)
                {

                    if (g_UserInfoBiz.GetUserByPhone(p_UserView.UserName) == null)
                    {

                        p_UserView.pUserID = v_User.ID.ToString();
                        var v_Status = false;
                        if (g_UserInfoBiz.RegUser(p_UserView))
                        {
                            v_Status = true;
                        }
                        return new
                        {
                            Data = new
                            {
                                IsExist = v_Status,
                                Message = v_Status ? "" : "系统异常,请联系系统管理员"
                            }
                        };
                    }
                    else {
                        return new
                        {
                            Data = new
                            {
                                IsExist = false,
                                Message = $"注册人-[{p_UserView.UserName}] 手机号已存在!"
                            }
                        };
                    }
                }
                else {
                    return new
                    {
                        Data = new
                        {
                            IsExist = false,
                            Message = $"注册人-[{p_UserView.CardName}] 已存在!"
                        }
                    };
                }
            }
            else {
                return new
                {
                    Data = new
                    {
                        IsExits = false,
                        Message = $"推荐人-[{p_UserView.pUserID}] 不存在,请联系人"
                    }
                };
            }
            
        }

        [HttpGet]
        public object DashBoard() {
            var results = g_GlobalBiz.GetDashBoard();
            return new
            {
                Data = new {
                    totalUnk = results.Item1,
                    totalUser = results.Item2,
                    currentPrice = results.Item3,
                    totalCurrentUser = results.Item4
                }
            };
        }

        [HttpGet]
        public object DeleteData(string p_userID) {
            return new
            {
                Data = new
                {
                    Success = g_UserInfoBiz.DeleteUserData(p_userID)
                }
            };
        }

        [HttpPost]
        public object UpdateAccountAndAsset([FromBody] RegNewUserModels p_UserView) {
            return new
            {
                Data = new
                {
                    Success = g_UserInfoBiz.UpdateAccountAndAsset(p_UserView)
                }
            };
        }

    }
}