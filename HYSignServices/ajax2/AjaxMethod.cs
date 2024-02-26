using System;

namespace blqw
{
    /// <summary> 指定一方法在前端Js中可直接调用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AjaxMethodAttribute : Attribute
    {
        /// <summary> 指定一方法在前端Js中可直接调用
        /// </summary>
        public AjaxMethodAttribute() { }
        /// <summary> 指定一方法在前端Js中可直接调用,并指定前端调用中的方法名
        /// </summary>
        /// <param name="functionName"></param>
        public AjaxMethodAttribute(string functionName) { FunctionName = functionName; }
        public string FunctionName { get;private set; }
    }
}