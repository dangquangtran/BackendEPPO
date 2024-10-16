﻿using BusinessObjects.Models;
using Microsoft.AspNetCore.Routing;

namespace BackendEPPO.Extenstion
{
    public class ApiEndPointConstant
    {

        public const string ApiRoleAdmin = "/Admin";
        public const string ApiRoleManager = "/Manager";
        public const string ApiRoleStaff = "/Staff";
        public const string ApiRoleOwner = "/Owner";
        public const string ApiRoleCustomer = "/Customer";
        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";

        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        public const string ApiEndpointByAdmin = RootEndPoint + ApiVersion + ApiRoleAdmin;
        public const string ApiEndpointByManager = RootEndPoint + ApiVersion + ApiRoleManager;
        public const string ApiEndpointByStaff = RootEndPoint + ApiVersion + ApiRoleStaff;
        public const string ApiEndpointByCustomer = RootEndPoint + ApiVersion + ApiRoleCustomer;
        public const string ApiEndpointByOwner = RootEndPoint + ApiVersion + ApiRoleOwner;

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

            public const string CreateAccountByCustomer = ApiEndpointByCustomer + "/CreateAccount";

            public const string CreateAccountByOwner = ApiEndpointByOwner + "/CreateAccount";

            public const string CreateAccountByAdmin = ApiEndpointByAdmin + "/CreateAccount";

            public const string UpdateAccount = GetUserEndpoint + "/UpdateAccount/Id";
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

            public const string CreateAddress = GetListAddress_Endpoint + "/CreateAddress";
            public const string UpdateAddress = GetListAddress_Endpoint + "/UpdateAddress/AddressId";

        }
        // Do Huu Thuan
        public static class Notification
        {
            public const string GetListNotification_Endpoint = ApiEndpoint + "/GetList/Notification";
            public const string GetNotificationByID = GetListNotification_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class TypeEcommerce
        {
            public const string GetListTypeEcommerce_Endpoint = ApiEndpoint + "/GetList/TypeEcommerce";
            public const string GetTypeEcommerceByID = GetListTypeEcommerce_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class Wallet
        {
            public const string GetListWallet_Endpoint = ApiEndpoint + "/GetList/Wallet";
            public const string GetWalletByID = GetListWallet_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class Blog
        {
            public const string GetListBlog_Endpoint = ApiEndpoint + "/GetList/Blog";
            public const string GetBlogByID = GetListBlog_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class Feedback
        {
            public const string GetListFeedback_Endpoint = ApiEndpoint + "/GetList/Feedback";
            public const string GetFeedbackByID = GetListFeedback_Endpoint + "/Id";
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
        // Do Huu Thuan
        public static class Rank
        {
            public const string GetListRank_Endpoint = ApiEndpoint + "/GetList/Ranks"; 
            public const string GetRoleByID = GetListRank_Endpoint + "/Id";

            public const string CreateRankByManager = ApiEndpointByManager + "/CreateRank";
            public const string UpdateRank = GetListRank_Endpoint + "/UpdateRank/RankId";
        }
        // Do Huu Thuan
        public static class SubFeedback
        {
            public const string GetListSubFeedback_Endpoint = ApiEndpoint + "/GetList/SubFeedback";
            public const string GetSubFeedbackByID = GetListSubFeedback_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class ImageFeedback
        {
            public const string GetListImageFeedback_Endpoint = ApiEndpoint + "/GetList/ImageFeedback";
            public const string GetImageFeedbackByID = GetListImageFeedback_Endpoint + "/Id";
        }
        // Do Huu Thuan
        public static class UserVoucher
        {
            public const string GetListUserVoucher_Endpoint = ApiEndpoint + "/GetList/UserVoucher";
            public const string GetImageFeedbackByID = GetListUserVoucher_Endpoint + "/Id";
        }

        public static class Conversation
        {
            public const string GetListConversation_Endpoint = ApiEndpoint + "/GetList/Conversations";
            public const string GetConversationByID = GetListConversation_Endpoint + "/Id";
        }

    }
   

}
