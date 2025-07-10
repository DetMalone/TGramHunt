namespace TGramHunt.Services.Helpers
{
    public static class LinkHelper
    {
        public const string rootLink = "/";
        public const string accauntHandlerLink = "/api/auth/tgcallback";
        public const string emptyLink = "/empty";
        public const string tgIframeCssPath = "/dist/tgiframe.css";
        public const string authLogout = "/api/auth/logout";
        public const string notFound = "/notfound";
        public const string profileIndex = "/profile/indexProfile";

        public static string? CreateLink(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return null;
            }

            return "https://telegram.me/" + tag.Replace("@", "");
        }

        public static string? ImageRelativLink(string id, int cache, string? picture = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return picture;
            }

            return string.Format("/api/Image/{0}?cache={1}", id, cache);
        }
    }
}