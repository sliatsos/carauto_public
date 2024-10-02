namespace CarAuto.MAUI.Shared.Configuration
{
    // All the code in this file is included in all platforms.
    public class KeycloakConfiguration
    {
        public const string ClientId = "carauto-client";

        //public const string OrganizationUrl = "http://keycloak:7070";

        public const string OrganizationUrl = "https://carauto.loca.lt";

        public const string Callback = "carauto://login-callback";

        public const string CallbackScheme = "carauto";

        public const string Realm = "carauto";

        public const string TokenStorageName = "token";
    }
}