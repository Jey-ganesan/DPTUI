namespace DPT_MVC.Helper
{
    public class SessionHelper
    {
        public static string GetStringValueFromSession(HttpContext context, string key)
        {
            return context.Session.GetString(key);
        }

        public static void SetStringValueInSession(HttpContext context, string key, string value)
        {
            context.Session.SetString(key, value);
        }
    }
}
