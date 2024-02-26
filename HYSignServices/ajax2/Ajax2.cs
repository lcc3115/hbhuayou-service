using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;

namespace blqw
{
    /// <summary> 超级方便的Asp.Net Ajax解决方案
    /// </summary>
    public static class Ajax2
    {
        /// <summary>
        /// 缓存
        /// </summary>
        static class Cache
        {
            static Dictionary<string, object> _Items = new Dictionary<string, object>(255);

            public static object Get(string key, Converter<string, object> get)
            {
                object obj;
                if (_Items.TryGetValue(key, out obj) == false)
                {
                    if (get == null)
                    {
                        return null;
                    }
                    _Items[key] = obj = get(key);
                }
                return obj;
            }

            private static void Set(string key, object cache)
            {
                _Items[key] = cache;
            }

            public static void Remove(string key)
            {
                _Items.Remove(key);
            }
        }

        #region javascript min
#if !DEBUG
        const string JAVASCRIPT = @"window.blqw=window.blqw||{};blqw.Ajax=blqw.Ajax||{};blqw.Ajax.GetRequest=function(){if(window.ActiveXObject)try{return new ActiveXObject(""Msxml2.XMLHTTP"")}catch(c){return new ActiveXObject(""Microsoft.XMLHTTP"")}else if(window.XMLHttpRequest)return new XMLHttpRequest};blqw.Ajax.Throw=function(c){for(var e=function(a,b,c){this.name=""AjaxError"";this.type=c;this.message=a;this.stack=b;this.innerError=null;this.toString=function(){return'message:'+this.message+'\r\n'+'stack:'+e.stack}},a=new e(c.message,c.stack,c.type),d=a;c.innerError;)c=c.innerError,d.innerError=new e(c.message,c.stack,c.type),d=d.innerError;return a};blqw.Ajax.Exec=function(c,e){var a=function(b){if(null==b)return"""";var a;switch(typeof b){case ""number"":case ""boolean"":return b.toString();case ""string"":return encodeURIComponent(b.replace(""\0"",""\0\0""));case ""undefined"":return"""";case ""function"":try{return arguments.callee(b())}catch(c){return""""}case ""object"":switch(a=Object.prototype.toString.apply(b),a){case ""[object Date]"":return encodeURIComponent(b.getFullYear()+""-""+(b.getMonth()+1)+""-""+b.getDate()+"" ""+b.getHours()+"":""+b.getMinutes()+"":""+b.getSeconds()+"".""+b.getMilliseconds());case ""[object RegExp]"":return arguments.callee(b.toString());case ""[object Array]"":a=[];for(var d in b)a.push(arguments.callee(b[d]));return""[""+a.join("","")+""]"";case ""[object Object]"":a=[];for(d in b)a.push(d+':""'+arguments.callee(b[d]).replace(""%22"",""%5C%22"")+'""');return 0===a.length?""{}"":""{""+a.join("","")+""}""}}},d="""";if(0<e.length){for(var d=[],f=0;f<e.length;f++)d.push(a(e[f]));d=""blqw.ajaxdata=""+d.join(""\x00"")}url=window.location.href;a=blqw.Ajax.GetRequest();a.open(""POST"",url,!1);a.setRequestHeader(""Content-Type"",""application/x-www-form-urlencoded; charset=utf-8"");a.send(d+""&blqw.ajaxmethod=""+c);if(200==a.status){a=eval(""(""+a.responseText+"")"");""v""in a&&eval(a.v);if(""e""in a)throw blqw.Ajax.Throw(a.e);return a.d}alert(""出现错误"")};";
#endif
        #endregion

        #region javascript full
#if DEBUG
        const string JAVASCRIPT = @"
	window.blqw = window.blqw || {};
    blqw.Ajax = blqw.Ajax || {};
    blqw.Ajax.GetRequest = function () {
        if (window.ActiveXObject) {
            try {
                return new ActiveXObject('Msxml2.XMLHTTP');
            } catch (e) {
                return new ActiveXObject('Microsoft.XMLHTTP');
            }
        }
        else if (window.XMLHttpRequest) {
            return new XMLHttpRequest();
        }
    }

    blqw.Ajax.Throw = function(e){{
            function AjaxError(message,stack,type){{
                this.name = 'AjaxError';
                this.type = type;
                this.message = message;
                this.stack = stack;
                this.innerError = null;
                this.toString = function () {{
                                    return 'message:' + this.message + '\r\n' + 'stack:' + e.stack;
                                }};
            }};
            var err = new AjaxError(e.message,e.stack,e.type);
            var e1 = err;
            while(e.innerError){{
                e = e.innerError;
                e1.innerError = new AjaxError(e.message,e.stack,e.type);
                e1 = e1.innerError;
            }}
            return err;
        }}

    blqw.Ajax.Exec = function (method, args) {
            var getStr = function (obj) {
                if (obj == null) return '';
                var type = typeof (obj);
                switch (type) {
                    case 'number':
                    case 'boolean':
                        return obj.toString();
                    case 'string':
                        return encodeURIComponent(obj.replace('\0', '\0\0'));
                    case 'undefined':
                        return '';
                    case 'function':
                        try {
                            return arguments.callee(obj());
                        } catch (e) {
                            return '';
                        }
                    case 'object':
                        type = Object.prototype.toString.apply(obj);
                        switch (type) {
                            case '[object Date]':
                                return encodeURIComponent(obj.getFullYear() + '-' +
                            (obj.getMonth() + 1) + '-' +
                            obj.getDate() + ' ' +
                            obj.getHours() + ':' +
                            obj.getMinutes() + ':' +
                            obj.getSeconds() + '.' +
                            obj.getMilliseconds());
                            case '[object RegExp]':
                                return arguments.callee(obj.toString());
                            case '[object Array]':
                                var arr = [];
                                for (var i in obj) {
                                    arr.push(arguments.callee(obj[i]));
                                }
                                return '[' + arr.join(',') + ']';
                            case '[object Object]':
                                var arr = [];
                                for (var i in obj) {
                                    arr.push(i + ':""' + arguments.callee(obj[i]).replace('%22','%5C%22') + '""');
                                }
                                if (arr.length === 0) {
                                    return '{}';
                                }
                                return '{' + arr.join(',') + '}';
                        }
                        break;
                }
            }
            var ajaxdata = '';
            if (args.length > 0) {
                var arr = [];
                for (var i = 0; i < args.length; i++) {
                    arr.push(getStr(args[i]));
                }
                ajaxdata = 'blqw.ajaxdata=' + arr.join('\0');
            }
            url = window.location.href;
            var req = blqw.Ajax.GetRequest();
            req.open('POST', url, false);
            req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8');
            var ret = req.send(ajaxdata + '&blqw.ajaxmethod=' + method);
            if (req.status == 200) {
                var html = req.responseText;
                var data = eval('(' + html+ ')');
                if ('v' in data) {
                    eval(data.v);
                } 
                if ('e' in data) {
                    throw blqw.Ajax.Throw(data.e);
                } else {
                    return data.d;
                }
            } else {
                alert('出现错误');
            }
        } 
";

        private static void Encode()
        {
            //var aaaa = new Yahoo.Yui.Compressor.JavaScriptCompressor(JAVASCRIPT, false, Encoding.UTF8, System.Globalization.CultureInfo.CurrentCulture).Compress().Replace("\"", "\"\"");
        }

#endif
        #endregion

        /// <summary> 当前是否是Ajax回传状态
        /// </summary>
        public static bool IsAjaxing
        {
            get
            {
                return HttpContext.Current.Request != null && HttpContext.Current.Request.Form["blqw.ajaxmethod"] != null;
            }
        }
        /// <summary> 将Js变量名转成js代码可识别的形式
        /// </summary>
        private static string ConvertVarName(string name)
        {
            if (name.Contains("."))
            {
                var arr = name.Split('.');
                name = "window";
                StringBuilder sb = new StringBuilder();

                foreach (var item in arr)
                {
                    name += string.Concat("[", Json.ToJsonString(item), "]");
                    sb.AppendFormat("{0}={0}", name);
                    sb.Append("||{};");
                }
                sb.Append(name);
                return sb.ToString();
            }
            else
            {
                return "window[" + Json.ToJsonString(name) + "]";
            }
        }

        /// <summary> 注册变量
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="value">变量值</param>
        public static void RegisterVar(string name, object value)
        {
            var js = ConvertVarName(name) + "=" + Json.ToJsonString(value) + ";";
            RegisterScript(js);
        }
        /// <summary> 注册js脚本
        /// </summary>
        /// <param name="javascript"></param>
        public static void RegisterScript(string javascript)
        {
            var page = (Page)HttpContext.Current.Handler;
            if (page == null)
            {
                return;
            }
            if (IsAjaxing)
            {
                if (page.Items.Contains("blqw.Ajax2.Js"))
                {
                    page.Items["blqw.Ajax2.Js"] += javascript;
                }
            }
            else
            {
                page.ClientScript.RegisterStartupScript(typeof(void), Guid.NewGuid().ToString("N"), javascript, true);
            }
        }
        /// <summary> 在页面中注册Ajax脚本
        /// </summary>
        /// <param name="page"></param>
        public static void Register(Page page)
        {
            if (page == null)
            {
                return;
            }
            if (page.Items.Contains("blqw.Ajax2"))
            {
                return;
            }
            page.Items.Add("blqw.Ajax2", true);
            page.Items.Add("blqw.Ajax2.Js", "");
            if (page.Request.Form["blqw.ajaxmethod"] != null)
            {
                string str;
                try
                {
                    var name = page.Request.Form["blqw.ajaxmethod"];
                    var method = page.GetType().GetMethod(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                    var met = Literacy.Cache(method);
                    var data = page.Request.Form["blqw.ajaxdata"];
                    var p = method.GetParameters();
                    object[] args = null;
                    if (data != null)
                    {
                        var d = data.Split('\0');
                        args = new object[p.Length];

                        if (args.Length != d.Length)
                        {
                            throw new ArgumentException("该方法需要提供 " + p.Length + " 个参数!");
                        }
                        for (int i = 0; i < p.Length; i++)
                        {
                            var val = d[i].Replace("\0\0", "\0");
                            var pit = p[i].ParameterType;
                            try
                            {
                                if (Type.GetTypeCode(pit) == TypeCode.String)
                                {
                                    args[i] = val;
                                }
                                else if (val.Length > 2 && (val[0] == '[' || val[0] == '{'))
                                {
                                    args[i] = Json.ToObject(pit, val);
                                }
                                else
                                {
                                    if (pit.IsValueType)
                                    {
                                        var type = Nullable.GetUnderlyingType(pit);

                                        if (val.Length == 0)
                                        {
                                            if (type == null)
                                            {
                                                args[i] = Activator.CreateInstance(pit);
                                            }
                                        }
                                        else
                                        {
                                            args[i] = Convert.ChangeType(val, type ?? pit);
                                        }
                                    }
                                    else if (val.Length != 0)
                                    {
                                        args[i] = Convert.ChangeType(val, pit);
                                    }
                                }
                            }
                            catch (Exception mex)
                            {
                                throw new ArgumentException("<参数 " + (i + 1) + ">转换失败!无法将 {" + val + "} 转换为 " + p[i].ParameterType.Name + " 类型", mex);
                            }
                        }

                    }
                    else if (p.Length > 0)
                    {
                        throw new ArgumentException("该方法需要提供 " + p.Length + " 个参数!");
                    }

                    var obj = met(page, args);
                    if (method.ReturnType.Namespace == "System")
                    {
                        if (method.ReturnType == typeof(void))
                        {
                            str = "";
                        }
                        else if (method.ReturnType == typeof(string))
                        {
                            str = "d:" + Json.ToJsonString(obj.ToString());
                        }
                        else if (method.ReturnType.IsValueType)
                        {
                            str = "d:" + Json.ToJsonString(obj.ToString());
                        }
                        else
                        {
                            str = "d:" + Json.ToJsonString(obj);
                        }
                    }
                    else
                    {
                        str = "d:" + Json.ToJsonString(obj);
                    }
                    var ext = page.Items["blqw.Ajax2.Js"].ToString();
                    if (ext.Length > 0)
                    {
                        ext = "v:" + Json.ToJsonString(ext);
                        if (str.Length > 0)
                        {
                            str = ext + "," + str;
                        }
                        else
                        {
                            str = ext;
                        }
                    }
                }
                catch (Exception ex)
                {
                    str = "e:" + new JsError(ex).ToString();
                }
                page.Response.Clear();
                page.Response.Write("{" + str + "}");
                page.Response.End();
            }
            else
            {

                string js = (string)Cache.Get(page.GetType().FullName + "->js", key =>
                {
                    var t = page.GetType();
                    StringBuilder sb = new StringBuilder();
                    foreach (var m in t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public))
                    {
                        var attr = (AjaxMethodAttribute)Attribute.GetCustomAttribute(m, typeof(AjaxMethodAttribute));
                        if (attr != null)
                        {
                            var ps = m.GetParameters();
                            var funcName = attr.FunctionName ?? m.Name;
                            if (funcName.IndexOf('.') > -1)
                            {
                                sb.Append(ConvertVarName(funcName));
                                sb.Append("=function(){return blqw.Ajax.Exec('");
                            }
                            else
                            {
                                sb.Append("function ");
                                sb.Append(funcName);
                                sb.Append("(){return blqw.Ajax.Exec('");
                            }
                            sb.Append(m.Name);
                            sb.Append("',arguments);}");
                            sb.AppendLine();
                        }
                    }
                    return sb.ToString();
                });

                if (page.Form == null)
                {
                    page.Response.Write(
                        string.Concat(@"<script type=""text/javascript"">
//<![CDATA[
", JAVASCRIPT, js, @"
//]]>
</script>"));
                }
                else
                {
                    page.ClientScript.RegisterClientScriptBlock(typeof(void), Guid.NewGuid().ToString("N"), JAVASCRIPT + js, true);
                }
            }
        }

        /// <summary> 在js中注册pager对象
        /// </summary>
        /// <param name="pager"></param>
        public static void RegisterPager(Pager pager)
        {
            if (pager != null)
            {
                RegisterVar(pager.Name, pager);
            }
        }

        /// <summary> 注册Alert消息框
        /// </summary>
        /// <param name="message"></param>
        public static void Alert(string message)
        {
            var js = "window.alert({0});";
            js = string.Format(js, Json.ToJsonString(message));
            RegisterScript(js);
        }

        /// <summary> 全局标识,是否抛出详细异常
        /// </summary>
        public static bool ThrowStack { get; set; }


        /// <summary> 在C#中模拟JsError对象,可以将Exception转为JsError
        /// </summary>
        class JsError
        {
            public JsError(Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    ex = ex.InnerException;
                }
                this.message = ex.Message;
                if (Ajax2.ThrowStack)
                {
                    this.type = ex.GetType().Name;
                    this.stack = ex.StackTrace;
                }

                if (ex.InnerException != null)
                {
                    this.innerError = new JsError(ex.InnerException);
                }
            }
            public string message
            {
                get;
                set;
            }
            public string stack
            {
                get;
                set;
            }
            public string type
            {
                get;
                set;
            }
            public JsError innerError
            {
                get;
                set;
            }
            public override string ToString()
            {
                return Json.ToJsonString(this);
            }
        }

    }
}