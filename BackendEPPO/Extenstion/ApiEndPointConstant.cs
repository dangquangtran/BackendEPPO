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
            public const string GetUserEndpoint = ApiEndpoint + "/GetUser/Users";

            public const string GetListUsers_Endpoint = ApiEndpoint + "/GetList/Users";
            public const string Login_Endpoint = ApiEndpoint + "/Users/Login";
            public const string GetUserByID = GetUserEndpoint + "/Id";
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
            public const string GetAccessoryByID = GetListAccessory_Endpoint + "/Id";

        }
        // Do Huu Thuan
        public static class Services
        {
            public const string GetListServices_Endpoint = ApiEndpoint + "/GetList/Services";

        }
        // Do Huu Thuan
        public static class Plants
        {
            public const string GetListPlants_Endpoint = ApiEndpoint + "/GetList/Plants";
            public const string GetPlantByID = GetListPlants_Endpoint + "/Id";



        }




    }
   

}
