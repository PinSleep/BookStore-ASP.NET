using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BookStore.Helpers
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        public static T Get<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return json == null ? default(T) : JsonSerializer.Deserialize<T>(json);
        }
    }
}
