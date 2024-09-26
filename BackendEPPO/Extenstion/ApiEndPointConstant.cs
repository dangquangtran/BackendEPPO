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

        // Do Huu Thuan     
        public static class User
        {
        
            public const string GetListUsers_Endpoint = ApiEndpoint + "/GetList/Users";

        }
        // Do Huu Thuan
        public static class Contract
        {
            public const string GetListContract_Endpoint = ApiEndpoint + "/GetList/Contracts";

        }
        // Do Huu Thuan
        public static class Accessory
        {
            public const string GetListAccessory_Endpoint = ApiEndpoint + "/GetList/Accessories";

        }
        // Do Huu Thuan
        public static class Plants
        {
            public const string GetListPlants_Endpoint = ApiEndpoint + "/GetList/Plants";




        }




    }
   

}
