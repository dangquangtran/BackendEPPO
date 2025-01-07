using BusinessObjects.Models;
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
        public static class Count
        {
            public const string CountOrder_Endpoint = ApiEndpoint + "/Count/OrderStatus/ByUserId";
            public const string CountOrderByToken_Endpoint = ApiEndpoint + "/Count/OrderStatus/ByToken";
            public const string CountAccountByStatus_Endpoint = ApiEndpoint + "/Count/User/Status";
            public const string CountOrderPriceRevenue_Endpoint = ApiEndpoint + "/Count/Order/Revenue";
            public const string CountAccountResgiter_Endpoint = ApiEndpoint + "/Count/User/Registered/RoomId";
            public const string CountShipByPlant = ApiEndpoint + "/Count/FreeShip/PlantId";
            public const string CountOrderPriceRevenue12M_Endpoint = ApiEndpoint + "/Count/Order/Revenue/Month";
            public const string CountOrderTypeEcommerceId_Endpoint = ApiEndpoint + "/Count/Order/Revenue/TypeEcommerceId";

            public const string CountCustomerByStatus_Endpoint = ApiEndpoint + "/Count/Customer/Status";
            public const string CountOrderStatus_Endpoint = ApiEndpoint + "/Count/Order/Status";
            public const string CountOrderTotalRevenue_Endpoint = ApiEndpoint + "/Count/Order/Total/Revenue";
            public const string CountOrderTodayRevenue_Endpoint = ApiEndpoint + "/Count/Order/Today/Revenue";
        }

        // Do Huu Thuan     
        public static class User
        {
            public const string GetUserEndpoint = ApiEndpoint + "/GetUser/Users";
            public const string GetListUsers_Endpoint = ApiEndpoint + "/GetList/Users";

            public const string GetListFilterByRole_Endpoint = ApiEndpoint + "/GetList/FilterByRole";
            public const string SearchAccountByKey_Endpoint = ApiEndpoint + "/GetList/SearchAccountByKey";

            public const string Login_Endpoint = ApiEndpoint + "/Users/Login";
            public const string Login_FaceID_Endpoint = ApiEndpoint + "/Users/Login/FaceID";
            public const string GetUserByID = GetUserEndpoint + "/Id";

            public const string GetInformationByID = GetUserEndpoint + "/Information/UserID";

            public const string CreateUserAccount = ApiEndpoint + "/UsersAccount/CreateAccount/Manager";

            public const string CreateAccountByCustomer = ApiEndpointByCustomer + "/CreateAccount/Customer";

            public const string CreateAccountByOwner = ApiEndpointByOwner + "/CreateAccount/Owner";

            public const string CreateAccountByAdmin = ApiEndpointByAdmin + "/CreateAccount";

            public const string UpdateAccount = GetUserEndpoint + "/UpdateAccount/Id";

            public const string UpdateInformationAccount = GetUserEndpoint + "/Update/Information/Id";

            public const string ChangePassword = GetUserEndpoint + "/UpdateAccount/ChangePassword/Id";
            public const string ChangePasswordByToken = GetUserEndpoint + "/UpdateAccount/ChangePassword/ByToken/Id";
            public const string ChangeStatus = GetUserEndpoint + "/UpdateAccount/ChangeStatus/Id";

            public const string GetListTopCustomers = GetUserEndpoint + "/TopCustomers";
            public const string SearchAccountID = GetUserEndpoint + "/Search/AccountID";

        }
        // Do Huu Thuan
        public static class Room
        {
            public const string GetListRoom_Endpoint = ApiEndpoint + "/GetList/Rooms";
            public const string GetListRoomByDateNow_Endpoint = ApiEndpoint + "/GetList/Rooms/FilterDate";
            public const string GetRoomByID = GetListRoom_Endpoint + "/Id";
            public const string CreateRoom = GetListRoom_Endpoint + "/Create/Room";
            public const string UpdateRoomByID = GetListRoom_Endpoint + "/Update/Room/Id";

            public const string DeleteRoomByID = GetListRoom_Endpoint + "/Delete/Room/Id";
            public const string UpdateStatusRoomByID = GetListRoom_Endpoint + "/Update/Status/Room/Id";
            public const string SearchListRoomByDate_Endpoint = ApiEndpoint + "/GetList/Rooms/SearchRoomByDate";
            public const string FilterListRoomByPrice_Endpoint = ApiEndpoint + "/GetList/Rooms/FilterListRoomByPrice";
            public const string GetListRoomIsActive_Endpoint = ApiEndpoint + "/GetList/Rooms/Aucting";
            public const string GetListRoomStatus_Endpoint = ApiEndpoint + "/GetList/Rooms/Status";
            public const string GetListRoomActive_Endpoint = ApiEndpoint + "/GetList/Rooms/Active";
            public const string GetListRoomManager_Endpoint = ApiEndpoint + "/GetList/AllRoom";

            public const string GetListHistoryRoom_Endpoint = ApiEndpoint + "/GetList/History/AllRoom";

            public const string GetRoomIDByCustomer = GetListRoom_Endpoint + "/Check/Id";

            public const string SearchRoomByID = GetListRoom_Endpoint + "/Search/Id";
        }
        // Do Huu Thuan
        public static class UserRoom
        {
            public const string GetListUserRoom_Endpoint = ApiEndpoint + "/GetList/UserRoom";
            public const string GetListUserRoomByToken_Endpoint = ApiEndpoint + "/GetList/UserRoom/Registered/ByToken";
            public const string GetUserRoomByID = GetListUserRoom_Endpoint + "/RoomId";
            public const string GetUserRoomByRoomID = GetListUserRoom_Endpoint + "/UerRoom/RoomId";
            public const string CreateUserRoom = GetListUserRoom_Endpoint + "/Create/UserRoom";
            public const string UpdateUserRoomByID = GetListUserRoom_Endpoint + "/Update/UserRoom/Id";
            public const string DelteUserRoomByID = GetListUserRoom_Endpoint + "/Delete/UserRoom/Id";
        }
        // Do Huu Thuan
        public static class Contract
        {
            public const string GetListContract_Endpoint = ApiEndpoint + "/GetList/Contracts";
            public const string GetContractByID = GetListContract_Endpoint + "/Id";
            public const string CreateContract = GetListContract_Endpoint + "/Create/Contract";
            public const string UpdateContractID = GetListContract_Endpoint + "/Update/Contract/Id";

            public const string UpdateStatusContractID = GetListContract_Endpoint + "/Update/Status/Contract/ContractId";

            public const string GetContractOfCustomer_Endpoint = ApiEndpoint + "/GetList/Contracts/ByToken/UserId";
            public const string GetContractOfUser_Endpoint = ApiEndpoint + "/GetList/Contracts/UserId";

            public const string CreatePartnershipContract = GetListContract_Endpoint + "/Create/Contract/Ownership";

            public const string DownLoadContract = ApiEndpoint + "/Download/Contract/{FileName}";
            public const string GetListContractStatus_Endpoint = ApiEndpoint + "/GetList/Contracts/Status";


            public const string ConfirmContractID = GetListContract_Endpoint + "/IsSigned/Contract/Id"; 
            public const string CreateContractAddendum = GetListContract_Endpoint + "/Create/Contract/Addendum";
            public const string SearchContract = ApiEndpoint + "/Search/Contract";
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
            public const string CreateContractDetails = GetListContractDetails_Endpoint + "/Create/ContractDetail";
            public const string UpdateContractDetailsID = GetListContractDetails_Endpoint + "/Update/ContractDetail/Id";
        }
        // Do Huu Thuan
        public static class Address
        {
            public const string GetListAddress_Endpoint = ApiEndpoint + "/GetList/Address";
            public const string GetAddressByID = GetListAddress_Endpoint + "/Id";

            public const string CreateAddress = GetListAddress_Endpoint + "/CreateAddress";
            public const string UpdateAddress = GetListAddress_Endpoint + "/UpdateAddress/Address/Id";

            public const string UpdateAddressByToken = GetListAddress_Endpoint + "/UpdateAddress/AddressByToken/Id";

            public const string DeleteAddress = GetListAddress_Endpoint + "/Delete/Address/ID";

            public const string DeleteAddressByToken = GetListAddress_Endpoint + "/Delete/AddressByToken/Id";

            public const string GetListAddressByUserID_Endpoint = ApiEndpoint + "/GetList/Address/OfByUserID";

            public const string GetListAddressByToken_Endpoint = ApiEndpoint + "/GetList/Address/OfByUserID/ByToken";

        }
        public static class Transaction
        {
            public const string GetListTransaction_Endpoint = ApiEndpoint + "/GetList/Transaction";
        }
        // Do Huu Thuan
        public static class Notification
        {
            public const string GetListNotification_Endpoint = ApiEndpoint + "/GetList/Notification";
            public const string GetNotificationByID = GetListNotification_Endpoint + "/Id";
            public const string CreateNotificationByID = GetListNotification_Endpoint + "/Create/Notification";
            public const string UpdateNotificationByID = GetListNotification_Endpoint + "/Update/Notification/Id";
        }
        // Do Huu Thuan
        public static class TypeEcommerce
        {
            public const string GetListTypeEcommerce_Endpoint = ApiEndpoint + "/GetList/TypeEcommerce";
            public const string GetTypeEcommerceByID = GetListTypeEcommerce_Endpoint + "/Id";
            public const string CreateTypeEcommerce = GetListTypeEcommerce_Endpoint + "/Create/TypeEcommerce";
            public const string UpdateTypeEcommerceID = GetListTypeEcommerce_Endpoint + "/Update/TypeEcommerce/Id";
        }
        // Do Huu Thuan
        public static class Wallet
        {
            public const string GetListWallet_Endpoint = ApiEndpoint + "/GetList/Wallet";
            public const string GetWalletByID = GetListWallet_Endpoint + "/Id";

            public const string GetListTransaction_Endpoint = GetListWallet_Endpoint + "/Transaction/Id";

            public const string CreateWallet = GetListWallet_Endpoint + "/Create/Wallet";
            public const string UpdateWalletByID = GetListWallet_Endpoint + "/Update/Wallet/Id";
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
            public const string CreateFeedback = GetListFeedback_Endpoint + "/Create/Feedback";
            public const string UpdateFeedbackID = GetListFeedback_Endpoint + "/Update/Feedback/Id";
            public const string DeleteFeedbackID = GetListFeedback_Endpoint + "/Delete/Feedback/Id";

            public const string GetListFeedbackByPlant = ApiEndpoint + "/GetList/Feedback/ByPlant";

            public const string GetListFeedbackOrderStatus = ApiEndpoint + "/GetList/Feedback/OrderStatus";
            public const string GetListFeedbackOrderStatusDelivered = ApiEndpoint + "/GetList/Feedback/Order/Delivered";

            public const string GetListFeedbackOrderDelivered = ApiEndpoint + "/GetList/Feedback/Order/Delivered/Plant";
            public const string GetListFeedbackOrderDeliveredRenting = ApiEndpoint + "/GetList/Feedback/Order/Delivered/Plant/Renting";
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

            public const string GetListPlantsByTypeEcommerceId = GetListPlants_Endpoint + "/Filter/ByTypeEcommerceId";

            public const string GetListPlantsByTypeEcommerceAndCategory = GetListPlants_Endpoint + "/Filter/TypeEcommerceIdAndCategoryId";

            public const string SearchPlantByKeyWord = GetListPlants_Endpoint + "/Search/Keyword";

            public const string CheckPlantInCart = GetListPlants_Endpoint + "/Cart/Check/PlantId";

            public const string CreatePlantByOwner_Endpoint = ApiEndpoint + "/GetList/Plants/CreatePlant/ByToken";
            public const string CreatePlantByOwnerToken_Endpoint = ApiEndpoint + "/Plants/CreatePlant/ByToken";

            public const string GetListPlantsRegister_Endpoint = ApiEndpoint + "/GetList/Plants/Register";

            public const string UpdatePlantByManager = ApiEndpoint + "/GetList/Plants/UpdatePlant/PlantId";

            public const string GetListPlantsOwnerByTypeEcommerceId = GetListPlants_Endpoint + "/PlantOwner/ByTypeEcommerceId";


            public const string UpdateStatusPlant = ApiEndpoint + "/GetList/Plants/Update/ByStatus"; 
            public const string CancelContractPlant = ApiEndpoint + "/GetList/Plants/CancelContractPlant"; 

            public const string UpdatePlantIdByManager = ApiEndpoint + "/GetList/Plants/Update/PlantId";


            public const string GetListPlantsByTypeEcommerceIdManager = GetListPlants_Endpoint + "/Filter/TypeEcommerceId";

            public const string ViewPlantsToAccept = ApiEndpoint + "/GetList/Plants/Accept";
            public const string ViewPlantsWaitAccept = ApiEndpoint + "/GetList/Plants/WaitToAccept"; 
            public const string ViewPlantsUnAccept = ApiEndpoint + "/GetList/Plants/UnAccept";
            public const string SearchPlantID = GetListPlants_Endpoint + "/Search/PlantID";

            public const string GetAllPlantsSaleOfOwner = ApiEndpoint + "/GetList/Plants/Sale/OfOwner/ByCode";

            public const string GetAllPlantsRentalOfOwner = ApiEndpoint + "/GetList/Plants/Rental/OfOwner/ByCode";
            public const string CalculateDepositRental = ApiEndpoint + "/GetList/Plants/DepositRental"; 
        }
        // Do Huu Thuan
        public static class Categories
        {
            public const string GetListCategory_Endpoint = ApiEndpoint + "/GetList/Categories";
            public const string GetCategoriesByID = GetListCategory_Endpoint + "/Id";
            public const string CreateCategories = GetListCategory_Endpoint + "/Create/Categories";
            public const string UpdateCategoriesID = GetListCategory_Endpoint + "/Update/Categories/Id";
            public const string DeleteCategoriesID = GetListCategory_Endpoint + "/Delete/Categories/Id";
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
        public static class Payment
        {
            public const string PaymentRecharge_Endpoint = ApiEndpoint + "/Payment/ZaloPay/Recharge";
        }
        public static class OrderBy
        {
            public const string CreateOrderBy = ApiEndpoint + "/GetList/Plants/Create/OrderBuy";
            public const string GetOrderByID = ApiEndpoint + "/GetList/Order/By/OrderId";
        }
        public static class OrderRental
        {
            public const string CreateOrderRental = ApiEndpoint + "/GetList/Plants/Create/OrderRental";
            public const string ViewReturnOrderRental = ApiEndpoint + "/GetList/OrderRental/View/Return/Id";
        }

    }
   

}
