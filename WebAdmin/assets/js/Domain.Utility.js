var g_s = {
    // "http://unk-token.com:8091/api/" : 'http://localhost:8091/api/'
    apiroot: 'http://unk-token.com:9051/api/',
    cookie: 'GPMP.User',
    user: null,
    cover: {
        show: function () {
            $('.gsbi_cover').show();
        },
        hide: function () {
            $('.gsbi_cover').hide();
        }
    }
};

var Utility;

(function (Utility, $) {
    "use strict";

    Utility.msg = {
        info: function (msg) {
            $.gritter.add({
                title: 'Info notification',
                text: msg,
                time: 2000,
                class_name: 'gritter-info gritter-center'
            });
        },
        warning: function (msg, callback, title) {
            var tit = 'Warning Notification';
            if (title) {
                tit = title;
            }
            $.gritter.add({
                title: tit,
                text: msg,
                time: 2000,
                class_name: 'gritter-warning gritter-center'
            });
            if (typeof callback == 'function') {
                return callback();
            }

        },
        alert: function (msg, callback, autoCallback) {
            msg = autoCallback ? msg + " page will be auto closed after 10s." : msg;
            bootbox.alert(msg, function (result) {
                callback();
            });
            if (autoCallback) {
                window.setTimeout(function () { callback(); }, 10000);
            }
        },
        confirm: function (msg, callback,cancelCallBack) {
            bootbox.confirm({
                message: msg,
                size: 'small',
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-success'
                    },
                    cancel: {
                        label: 'No',
                        className: 'btn-danger'
                    }
                },
                callback: function (result) {

                    if (result) {
                        return callback();
                    } else {
                        if (typeof cancelCallBack == "function") {
                            return cancelCallBack();
                        } else {
                            return true;
                        }
                    }
                }
            });
        }
    };

    Utility.ajax = {
        blockGet: function (url, callback, btn_selector) {
            var xhr = $.ajax({
                url: g_s.apiroot + url,
                //data: { token: g_s.user.Token },
                type: 'GET',
                async: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Token', g_s.user.Token);
                    if (btn_selector) {
                        $(btn_selector).css({ 'position': 'relative' })
                        .attr('disabled', 'disabled')
                        .append('<div class="loading-layout"><i class="ace-icon loading-icon fa fa-spinner fa-spin fa-2x white"></i></div>');
                    }
                }
            });
            this.done(xhr, callback, btn_selector);
        },
        DeferredPost: function (url, postData, dataType, btn_selector) {
            var options = {
                url: g_s.apiroot + url,
                data: postData,
                type: 'POST',
                timeout: 3600000,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Token', g_s.user.Token);
                    if (btn_selector) {
                        $(btn_selector).css({ 'position': 'relative' })
                        .attr('disabled', 'disabled')
                        .append('<div class="loading-layout"><i class="ace-icon loading-icon fa fa-spinner fa-spin fa-2x white"></i></div>');
                    }
                },
                complete: function (data) {
                    if (btn_selector) {
                        $(btn_selector).removeAttr('disabled').removeAttr('style');
                        $(btn_selector).find('.loading-layout').remove();
                    }
                }
            };
            if (dataType) {
                dataType = dataType.toLowerCase();
                options.dataType = dataType;
            }

            var xhr = $.ajax(options);
            if (dataType && dataType === 'json') {
                var deferred = $.Deferred();
                xhr.then(function (data) {
                    if (data.Results) {
                        deferred.resolve(data.Results);
                    }
                }).fail(function (args1, args2, args3) {
                    deferred.reject(args1, args2, args3);
                });
                return deferred;
            }

            return xhr;
        },
        blockPost: function (url, postData, callback, btn_selector) {
            var xhr = $.ajax({
                url: g_s.apiroot + url,
                data: postData,
                type: 'POST',
                async: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Token', g_s.user.Token);
                    if (btn_selector) {
                        $(btn_selector).css({ 'position': 'relative' })
                        .attr('disabled', 'disabled')
                        .append('<div class="loading-layout"><i class="ace-icon loading-icon fa fa-spinner fa-spin fa-2x white"></i></div>');
                    }
                }
            });
            this.done(xhr, callback, btn_selector);
        },
        get: function (url, callback, btn_selector) {
            var config = {
                url: g_s.apiroot + url,
                //data: { token: g_s.user.Token },
                cache: false,
                type: 'GET',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Token', g_s.user.Token);
                    if (btn_selector) {
                        $(btn_selector).css({ 'position': 'relative' })
                        .attr('disabled', 'disabled')
                        .append('<div class="loading-layout"><i class="ace-icon loading-icon fa fa-spinner fa-spin fa-2x white"></i></div>');
                    }
                }
            };

            if (url.indexOf('.html') > -1) {
                //if(url.indexOf("?")>-1){
                //    config.url = url + "&v=" + Math.random();
                //} else {
                //    config.url = url + '?v=' + Math.random();
                //}
                var regex = new RegExp(g_s.apiroot);
                config.url = url.replace(regex,'');
            }


            var xhr = $.ajax(config);
            if (callback == null) {
                return xhr;
            }
            return this.done(xhr, callback, btn_selector);
        },
        getDashboard: function (url, callback, btn_selector) {
            var xhr = $.ajax({
                url: g_s.apiroot + url,
                //data: { token: g_s.user.Token },
                type: 'GET',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Token', g_s.user.Token);
                    if (btn_selector) {
                        $(btn_selector).css({ 'position': 'relative' })
                        .attr('disabled', 'disabled')
                        .append('<div class="loading-layout"><i class="ace-icon loading-icon fa fa-spinner fa-spin fa-2x white"></i></div>');
                    }
                }
            });
            this.doneDashboard(xhr, callback, btn_selector);
        },
        post: function (url, postData, callback, btn_selector) {
            if (typeof (postData) == 'object') {
                postData = $.param(postData);
            }

            var xhr = $.ajax({
                url: g_s.apiroot + url,
                data: postData,
                type: 'POST',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Token', g_s.user.Token);
                    if (btn_selector) {
                        $(btn_selector).css({ 'position': 'relative' })
                        .attr('disabled', 'disabled')
                        .append('<div class="loading-layout"><i class="ace-icon loading-icon fa fa-spinner fa-spin fa-2x white"></i></div>');
                    }

                }
            });
            this.done(xhr, callback, btn_selector);
        },
        postDashboard: function (url, postData, callback, btn_selector) {
            if (typeof (postData) == 'object') {
                postData = $.param(postData);
            }

            var xhr = $.ajax({
                url: g_s.apiroot + url,
                data: postData,
                type: 'POST',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Token', g_s.user.Token);
                    if (btn_selector) {
                        $(btn_selector).css({ 'position': 'relative' })
                        .attr('disabled', 'disabled')
                        .append('<div class="loading-layout"><i class="ace-icon loading-icon fa fa-spinner fa-spin fa-2x white"></i></div>');
                    }

                }
            });
            this.doneDashboard(xhr, callback, btn_selector);
        },
        save: function (formid, callback, btn_selector) {

            var f = $('#' + formid);
            this.post(f.attr('action'), f.serialize(), callback, btn_selector);

        },
        done: function (xhr, callback, btn_selector) {
            return xhr.done(function (data) {
                if (data.Data) {
                    if (typeof callback=='function') {
                        callback(data.Data);
                    }
                }
                if (btn_selector) {
                    var isVisible = $(btn_selector).css('display') != 'none' && $(btn_selector).css('visibility') == 'visible';
                    $(btn_selector).removeAttr('disabled').removeAttr('style')[isVisible ? 'show' : 'hide']();
                    $(btn_selector).find('.loading-layout').remove();
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (btn_selector) {
                    var isVisible = $(btn_selector).css('display') != 'none' && $(btn_selector).css('visibility') == 'visible';
                    $(btn_selector).removeAttr('disabled').removeAttr('style')[isVisible ? 'show' : 'hide']();
                    $(btn_selector).find('.loading-layout').remove();
                }
                Utility.localStore.saveData('err_msg', jqXHR.responseText);
                //window.location.href = '/error.html?c=' + jqXHR.status + '&err=' + encodeURIComponent('Request Fails : ' + errorThrown);
                
                if (jqXHR.status == 401) {
                    location.href = "/login.html?d=" + new Date();
                } else {
                    Utility.msg.warning('Request Fails : ' + errorThrown, 'body');
                }
            });
        },
        doneDashboard: function (xhr, callback, btn_selector) {
            return xhr.done(function (data) {
                if (data.Results) {
                    callback(data.Results);
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (btn_selector) {
                    $(btn_selector).removeAttr('disabled').removeAttr('style');
                    $(btn_selector).find('.loading-layout').remove();
                }
                Utility.localStore.saveData('err_msg', jqXHR.responseText);
                //window.location.href = '/error.html?c=' + jqXHR.status + '&err=' + encodeURIComponent('Request Fails : ' + errorThrown);
                Utility.msg.warning('Request Fails : ' + errorThrown, 'body');
            });
        }
    };

    Utility.localStore = {
        saveData: function (key, data) {
            if (window.localStorage) {
                if (typeof (data) != 'string') {
                    data = JSON.stringify(data);
                }
                window.localStorage.setItem(key, data);

            }
            else {
                Utility.msg.alert('not support localStorage');
            }

        },
        removeData: function (key) {
            if (window.localStorage) {

                window.localStorage.removeItem(key);

            }
            else {
                Utility.msg.alert('not support localStorage');
            }

        },
        getData: function (key) {
            if (window.localStorage) {
                var data = window.localStorage.getItem(key) ? Utility.convert.toJson(window.localStorage.getItem(key)) : null;
                return data;
                //return data.map(function (item) {
                //    return item.replace(/['")-><&\\\/\.]/g, '');
                //});
            }
            else {
                Utility.msg.alert('not support localStorage');
                return null;
            }
        }
    };

    Utility.convert = {
        toDate: function (val, format) {
            if (val == null) {
                return '';
            }
            val = val instanceof Date ? val : new Date((val.indexOf('GMT') >= 0 ? val : val + 'Z'));

            format = !!format ? format : 'MM/dd/yyyy';
            var o = {
                "M+": val.getMonth() + 1, //month
                "d+": val.getDate(), //day
                "h+": val.getHours(), //hour
                "m+": val.getMinutes(), //minute
                "s+": val.getSeconds(), //second
                "q+": Math.floor((val.getMonth() + 3) / 3), //quarter
                "S": val.getMilliseconds() //millisecond
            }
            if (/(y+)/.test(format)) {
                format = format.replace(RegExp.$1, (val.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            for (var k in o) if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1,
                RegExp.$1.length == 1 ? o[k] :
                ("00" + o[k]).substr(("" + o[k]).length));
            }
            return format;

        },
        toUtcDate: function (val, format) {
            if (val == null) {
                return '';
            }
            
            val = val.Unix ? val : new Date(val);
            format = !!format ? format : 'MM/dd/yyyy';
            var o = {
                "M+": val.getUTCMonth() + 1, //month
                "d+": val.getUTCDate(), //day
                "h+": val.getUTCHours(), //hour
                "m+": val.getUTCMinutes(), //minute
                "s+": val.getUTCSeconds(), //second
                "q+": Math.floor((val.getUTCMonth() + 3) / 3), //quarter
                "S": val.getUTCMilliseconds() //millisecond
            }
            if (/(y+)/.test(format)) {
                format = format.replace(RegExp.$1, (val.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            for (var k in o) if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1,
                RegExp.$1.length == 1 ? o[k] :
                ("00" + o[k]).substr(("" + o[k]).length));
            }
            return format;
        },
        toInt: function (val, def) {
        },
        toDecimal: function (val, def) {
        },
        toJson: function (val, def) {
            try {
                if (val != null && typeof (val) == 'object') {
                    return val;
                }
                var rv = JSON.parse(val);
                return rv == null ? '' : rv;
            }
            catch (ex) {
                return !def ? '' : def;
            }
        },
        safeInput: function (val) {
            val = val.replace(/[^\w\.\/]/ig, '');
            return val;
        },
        CNY2USD: function (amount, rate) {
            return Utility.convert.Return2Float((amount / rate / 1000) * 100 / 100);
        },
        Return2Float: function (amount) {
            //var re = /([0-9]+\.[0-9]{2})[0-9]*/;
            //var aNew = amount.toString().replace(re, "$1");
            //return aNew;
            var f = parseFloat(amount);
            if (isNaN(f)) {
                return amount;
                //return false;
            }
            var f = Math.round(amount * 100) / 100;
            var s = f.toString();
            var rs = s.indexOf('.');
            if (rs < 0) {
                rs = s.length;
                s += '.';
            }
            while (s.length <= rs + 2) {
                s += '0';
            }
            return s;             
        },
        formatNumber: function (val, precision) {
            try {
                var c = val.replace(/[^0-9|\.]/g, '');

                if (/^0+/.test(c))
                    c = c.replace(/^0+/, '');
                if (!/\./.test(c))
                    c += '.00';
                if (/^\./.test(c))
                    c = '0' + c;
                c += '00';
                c = c.match(/\d+\.\d{2}/)[0];
                return c;
            } catch (e) {
                if (console && console.error) {
                    console.error(e);
                }
            }

            return '';
        },
        comdify: function (n) {
            var re = /\d{1,3}(?=(\d{3})+$)/g;
            var n1 = n.replace(/^(\d+)((\.\d+)?)$/, function (s, s1, s2) { return s1.replace(re, "$&,") + s2; });
            return n1;
        },
        DateFtt: function (fmt, date) {
            var o = {
                "M+": date.getMonth() + 1,                 //月份   
                "d+": date.getDate(),                    //日   
                "h+": date.getHours(),                   //小时   
                "m+": date.getMinutes(),                 //分   
                "s+": date.getSeconds(),                 //秒   
                "q+": Math.floor((date.getMonth() + 3) / 3), //季度   
                "S": date.getMilliseconds()             //毫秒   
            };
            if (/(y+)/.test(fmt))
                fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt))
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            return fmt;
        },
        Sum: function (list) {
            if (list != undefined || list != null) {
                return Utility.convert.Return2Float(eval(list.join("+")));
            }
        },
        Format: function (source, params) {
            if (arguments.length === 1) {
                return function () {
                    var args = $.makeArray(arguments);
                    args.unshift(source);
                    return $.validator.format.apply(this, args);
                };
            }
            if (arguments.length > 2 && params.constructor !== Array) {
                params = $.makeArray(arguments).slice(1);
            }
            if (params.constructor !== Array) {
                params = [params];
            }
            $.each(params, function (i, n) {
                source = source.replace(new RegExp("\\{" + i + "\\}", "g"), function () {
                    return n;
                });
            });
            return source;
        }
    };

    Utility.selectElement = {
        bindDataSource: function (selector, dataSource, defaultValue) {
            var $dataSourceTarget = $(selector);
            $dataSourceTarget.find('option:gt(0)').remove();

            var optionHtml = '';
            for (var itemIndex in dataSource) {
                var item = dataSource[itemIndex];
                optionHtml += '<option value="' + item.Value + '">' + item.Text + '</option>';
            }
            if (optionHtml.length>0) {
                $dataSourceTarget.append(optionHtml);
            }

            if (defaultValue) {
                $dataSourceTarget.val(defaultValue);
            }
        }
    };

    Utility.Roles = {
        FA: "FA",
        CFO: "CFO",
        T2F: "T2F",
        T2: "T2",
        CIO: "CIO",
        BU: "BU",
        PMO: "PMO"
    };

    Utility.Url = {
        getParam: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);

            if (r != null) return (decodeURIComponent(r[2]).replace(/['")><&\\\/\.]/g, '')); return null;
        },
        getRefUrl: function () {
            var url = window.location.href;
            url = url.substr(0, url.lastIndexOf('?'));
            var last = url.lastIndexOf('/');
            var page = url.substr(last);
            if (page.indexOf('.') > 0) {
                //  e/abc/index.html
                url = url.substr(0, last);

            }
            else if (last + 1 == url.length) {
                //  e/abc/
                url = url.substr(0, url.lastIndexOf('/'));
            }

            // e
            url = url.substr(0, url.lastIndexOf('/'));

            return url;
        },
        setCookie: function (name, value) {
            var Days = 0.5;
            var exp = new Date();
            exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
            document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
        },
        getCookie: function (name) {
            var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

            if (arr = document.cookie.match(reg)) {
                return unescape(arr[2]);
            }
            else {
                return null;
            }
        },
        delCookie: function (name) {
            var exp = new Date();

            exp.setTime(exp.getTime() - (24 * 60 * 60 * 1000));
            var cval = this.getCookie(name);
            if (cval != null) {
                document.cookie = name + "=;path=/;expires=" + exp.toGMTString();

            }
        }
    };

    Utility.Style = {
        setLoading: function (isShow, msg, options) {
            if (isShow) {

                var defaultOptions = { message: '<div class="text-center" id="FormLoading"><i class="fa fa-spin fa-spinner"></i> ' + (msg || 'Loading...') + '</div>' };
                if (options) {
                    var customOptions = $.extend(defaultOptions, options);
                    bootbox.dialog(customOptions);
                } else {
                    bootbox.dialog(defaultOptions);
                }

            } else {
                bootbox.hideAll();
            }
        }
    };

   

}(Utility || (Utility = {}), jQuery));

g_s.user = JSON.parse(Utility.Url.getCookie(g_s.cookie));
g_s.user = g_s.user == null ? {} : g_s.user;

$(function () {

    $('body').delegate('.user-logout', 'click', function () {

        Utility.Url.delCookie(g_s.cookie);
        window.location.href = '/login.html?d=' + new Date();


        return false;
    });
});

/* Polyfill goes here */
// Production steps of ECMA-262, Edition 5, 15.4.4.17
// Reference: http://es5.github.io/#x15.4.4.17
if (!Array.prototype.some) {
    Array.prototype.some = function (fun/*, thisArg*/) {
        'use strict';

        if (this == null) {
            throw new TypeError('Array.prototype.some called on null or undefined');
        }

        if (typeof fun !== 'function') {
            throw new TypeError();
        }

        var t = Object(this);
        var len = t.length >>> 0;

        var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
        for (var i = 0; i < len; i++) {
            if (i in t && fun.call(thisArg, t[i], i, t)) {
                return true;
            }
        }

        return false;
    };
}