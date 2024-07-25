namespace Mango.Web.Configuration
{
    public class MangoConfig
    {
        public static string CouponUrlBase { get; set; }
        public static string OrderUrlBase { get; set; }
        public static string CartUrlBase { get; set; }
        public static string ProductUrlBase { get; set; }
        public static string AuthUrlBase { get; set; }

        public const string TokenName = "JwtAuthToken";
    }
}
