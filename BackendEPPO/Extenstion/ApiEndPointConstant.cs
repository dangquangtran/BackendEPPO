using Microsoft.AspNetCore.Routing;

namespace BackendEPPO.Extenstion
{
    public class ApiEndPointConstant
    {
        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        static ApiEndPointConstant()
        {
        }
               
        public static class User
        {
            // Do Huu Thuan
            public const string GetListUsers_Endpoint = ApiEndpoint + "/GetList/Users";

    


        }
        public static class Staff
        {
            public const string GetListUsers_Endpoint = ApiEndpoint + "/GetList/Users";




        }
    }
   

}
