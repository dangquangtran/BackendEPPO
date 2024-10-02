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

            public const string CreateUserAccount = ApiEndpoint + "/UsersAccount/CreateAccount";
        }
        // Do Huu Thuan
        public static class Room
        {
            public const string GetListRoom_Endpoint = ApiEndpoint + "/GetList/Rooms";
            public const string GetRoomByID = GetListRoom_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class Contract
        {
            public const string GetListContract_Endpoint = ApiEndpoint + "/GetList/Contracts";
            public const string GetContractByID = GetListContract_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class RoomParticipant
        {
            public const string GetRoomParticipant_Endpoint = ApiEndpoint + "/GetList/RoomParticipant";
            public const string GetRoomParticipantByID = GetRoomParticipant_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class ContractDetails
        {
            public const string GetListContractDetails_Endpoint = ApiEndpoint + "/GetList/ContractDetails";
            public const string GetContractDetailsByID = GetListContractDetails_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class Address
        {
            public const string GetListAddress_Endpoint = ApiEndpoint + "/GetList/Address";
            public const string GetAddressByID = GetListAddress_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class Notification
        {
            public const string GetListNotification_Endpoint = ApiEndpoint + "/GetList/Notification";
            public const string GetNotificationByID = GetListNotification_Endpoint + "/Id";
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
            public const string GetPlantByCategory = GetListPlants_Endpoint + "/CategoryId";


        }
        // Do Huu Thuan
        public static class Categories
        {
            public const string GetListCategory_Endpoint = ApiEndpoint + "/GetList/Categories";
            public const string GetCategoriesByID = GetListCategory_Endpoint + "/Id";
        }



    }
   

}
