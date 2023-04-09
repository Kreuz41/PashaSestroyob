using Microsoft.AspNetCore.Authorization;

namespace PashaSestroyob.Data.Models
{
    public class CustomAuthorizationPolicy
    {
        public const string PolicyName = "CustomAuthorizationPolicy";

        public static void Build(AuthorizationPolicyBuilder builder)
        {
            builder.RequireAuthenticatedUser();
        }
    }
}
