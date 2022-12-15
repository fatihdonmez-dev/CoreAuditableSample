using System.Globalization;

namespace CoreAuditableSample.Utils
{
    public static class Helper
    {
        private static HttpClient _client;
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> target)
        {
            return !(target != null && target.Count() > 0);
        }
        public static bool IsNullOrEmpty(this string target)
        {
            return string.IsNullOrWhiteSpace(target);
        }
        public static bool IsNullOrEmpty(this Guid? target)
        {
            return target.GetValueOrDefault(Guid.Empty) == Guid.Empty;
        }
        public static bool IsNullOrEmpty(this Guid target)
        {
            return target == null || target == Guid.Empty;
        }
        public static string GenerateKeyPlus(params object[] list)
        {
            return list.Aggregate(string.Empty, (current, t) => current + (t + "_")).TrimEnd('_');
        }
        public static string IsNull(this string target, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(target) ? defaultValue : target;
        }
        public static string ToLowerEn(this string target, string defaultValue = null)
        {
            return target.IsNull(defaultValue.IsNull(string.Empty)).ToLower(new CultureInfo("en-US"));
        }
    }
}
