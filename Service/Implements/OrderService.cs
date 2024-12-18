using AutoMapper;
using BusinessObjects.Models;
using DTOs.Order;
using Microsoft.AspNetCore.Http;
using Repository.Implements;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly FirebaseStorageService _firebaseStorageService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, FirebaseStorageService firebaseStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _firebaseStorageService = firebaseStorageService;
        }

        public IEnumerable<OrderVM> GetAllOrders(int pageIndex, int pageSize)
        {
            var orders = _unitOfWork.OrderRepository.Get(filter: c => c.Status != 0, orderBy: query => query.OrderByDescending(c => c.OrderId), pageIndex: pageIndex, pageSize: pageSize, includeProperties: "OrderDetails,ImageDeliveryOrders,ImageReturnOrders");
            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }

        public OrderVM GetOrderById(int id)
        {
            var order =_unitOfWork.OrderRepository.GetByID(id, includeProperties: "OrderDetails,ImageDeliveryOrders,ImageReturnOrders");
            return _mapper.Map<OrderVM>(order);
        }

        public void CreateOrder(CreateOrderDTO createOrderDTO, int userId)
        {
            var user = _unitOfWork.UserRepository.GetByID(userId);
            var walletId = user.WalletId;
            var wallet = _unitOfWork.WalletRepository.GetByID(walletId);

            if (wallet == null)
            {
                throw new Exception("Không tìm thấy ví của người dùng.");
            }

            double finalPrice = createOrderDTO.TotalPrice + createOrderDTO.DeliveryFee;

            if (createOrderDTO.PaymentId == 2)
            {
                if (wallet.NumberBalance < finalPrice)
                {
                    throw new Exception("Số dư trong ví không đủ để thanh toán.");
                }

                wallet.NumberBalance -= finalPrice;
                _unitOfWork.WalletRepository.Update(wallet);

                Transaction transaction = new Transaction
                {
                    WalletId = walletId,
                    Description = "Thanh toán đơn hàng",
                    WithdrawNumber = finalPrice,
                    RechargeNumber = null,
                    WithdrawDate = DateTime.UtcNow.AddHours(7),
                    CreationDate = DateTime.UtcNow.AddHours(7),
                    PaymentId = 2,
                    Status = 1,
                    IsActive = true
                };
                _unitOfWork.TransactionRepository.Insert(transaction);
            }

            Order order = _mapper.Map<Order>(createOrderDTO);
            order.CreationDate = DateTime.UtcNow.AddHours(7);
            order.Status = createOrderDTO.PaymentId == 2 ? 2 : 1;
            order.UserId = userId;
            order.FinalPrice = order.TotalPrice + order.DeliveryFee;
            order.PaymentStatus = createOrderDTO.PaymentId == 2 ? "Đã thanh toán" : "Chưa thanh toán";
            order.TypeEcommerceId = 1;

            foreach (var orderDetailDTO in createOrderDTO.OrderDetails)
            {
                if (orderDetailDTO.PlantId.HasValue)
                {
                    var plant = _unitOfWork.PlantRepository.GetByID(orderDetailDTO.PlantId.Value);
                    if (plant != null)
                    {
                        // Kiểm tra trạng thái isActive của cây
                        if ((bool)!plant.IsActive)
                        {
                            throw new Exception($"Cây với ID {plant.PlantId} không thể được đặt vì đã ngừng hoạt động.");
                        }

                        // Cập nhật trạng thái của cây
                        plant.IsActive = false;
                        _unitOfWork.PlantRepository.Update(plant);
                        var owner = _unitOfWork.UserRepository.GetByID(plant.Code);
                        if (owner != null)
                        {
                            CreateNotification(owner.UserId, "Thông báo", "Đơn hàng " + order.OrderId + " đã được tạo thành công");
                        }
                    }
                }
            }
            _unitOfWork.OrderRepository.Insert(order);
            _unitOfWork.Save();
        }

        public void UpdateOrder(UpdateOrderDTO updateOrder)
        {
            Order order = _mapper.Map<Order>(updateOrder);
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }
        public IEnumerable<OrderVM> GetOrdersBuyByUserId(int userId, int pageIndex, int pageSize, int status)
        {
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.UserId == userId && o.Status == status && (o.TypeEcommerceId == 1 || o.TypeEcommerceId == 3),
                orderBy: q => q.OrderByDescending(o => o.CreationDate),
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails,OrderDetails.Plant" // Bao gồm thông tin chi tiết đơn hàng
            );

            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }

        public IEnumerable<OrderVM> GetOrdersAuctionByUserId(int userId, int pageIndex, int pageSize, int status)
        {
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.UserId == userId && o.Status == status && o.TypeEcommerceId == 3,
                orderBy: q => q.OrderByDescending(o => o.CreationDate),
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails,OrderDetails.Plant"
            );

            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }
        public async Task<double> CountOrderPrice(int status, int? month = null, int? year = null)
        {
            var query = _unitOfWork.OrderRepository.Get(
                filter: o => o.FinalPrice.HasValue && o.Status == status // status = 4 là giao hàng thành công
            );
            if (month.HasValue && year.HasValue)
            {
                query = query.Where(o => o.CreationDate.HasValue
                                         && o.CreationDate.Value.Month == month
                                         && o.CreationDate.Value.Year == year);
            }
            var totalRevenue = query.Any() ? query.Sum(o => o.FinalPrice.Value) : 0;

            return totalRevenue;
        }
        public async Task<List<double>> CountOrderPriceForYear(int status, int year)
        {
            var monthlyRevenue = new List<double>(new double[12]); // Khởi tạo 12 tháng với giá trị mặc định là 0

            var query = _unitOfWork.OrderRepository.Get(
                filter: o => o.FinalPrice.HasValue && o.Status == status && o.CreationDate.HasValue && o.CreationDate.Value.Year == year
            );

            // Nhóm doanh thu theo tháng
            var groupedByMonth = query.GroupBy(o => o.CreationDate.Value.Month)
                                       .Select(g => new
                                       {
                                           Month = g.Key,
                                           TotalRevenue = g.Sum(o => o.FinalPrice.Value)
                                       })
                                       .ToList();

            // Gán doanh thu vào danh sách tương ứng với từng tháng
            foreach (var item in groupedByMonth)
            {
                monthlyRevenue[item.Month - 1] = item.TotalRevenue; // Month - 1 để phù hợp với index của danh sách
            }

            return monthlyRevenue;
        }

        public async Task<List<double>> CountOrderPriceByTypeEcom(int status, int year, int typeEcommerceId)
        {
            var monthlyRevenue = new List<double>(new double[12]); // Khởi tạo 12 tháng với giá trị mặc định là 0

            var query = _unitOfWork.OrderRepository.Get(
                filter: o => o.FinalPrice.HasValue && o.Status == status && o.CreationDate.HasValue && o.CreationDate.Value.Year == year && o.TypeEcommerceId == typeEcommerceId
            );

            // Nhóm doanh thu theo tháng
            var groupedByMonth = query.GroupBy(o => o.CreationDate.Value.Month)
                                       .Select(g => new
                                       {
                                           Month = g.Key,
                                           TotalRevenue = g.Sum(o => o.FinalPrice.Value)
                                       })
                                       .ToList();

            // Gán doanh thu vào danh sách tương ứng với từng tháng
            foreach (var item in groupedByMonth)
            {
                monthlyRevenue[item.Month - 1] = item.TotalRevenue; // Month - 1 để phù hợp với index của danh sách
            }

            return monthlyRevenue;
        }


        public async Task<int> CountOrderByStatus(int userId, int status)
        {
            var orderCount = await Task.FromResult(_unitOfWork.OrderRepository.Get(
                filter: o => o.UserId == userId && o.Status == status
            ).Count());

            return orderCount;
        }

        public IEnumerable<OrderRentalVM> GetOrdersRentalByUserId(int userId, int pageIndex, int pageSize, int status)
        {
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.UserId == userId && o.Status == status && o.TypeEcommerceId == 2,
                orderBy: q => q.OrderByDescending(o => o.CreationDate),
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails,OrderDetails.Plant" // Bao gồm thông tin chi tiết đơn hàng
            );

            return _mapper.Map<IEnumerable<OrderRentalVM>>(orders);
        }


        //private double CalculateTotalPrice(CreateOrderDTO createOrderDTO)
        //{
        //    double totalPrice = 0;

        //    // Tính tổng giá của tất cả các OrderDetails
        //    foreach (var orderDetail in createOrderDTO.OrderDetails)
        //    {
        //        totalPrice += orderDetail.TotalPrice ?? 0;
        //    }

        //    return totalPrice;
        //}

        //private double ApplyVoucher(double? totalPrice, int? userVoucherId, int? plantVoucherId)
        //{
        //    double finalPrice = totalPrice ?? 0;

        //    // Giả sử hàm ApplyUserVoucher và ApplyPlantVoucher sẽ trả về số tiền giảm giá
        //    if (userVoucherId.HasValue)
        //    {
        //        finalPrice -= ApplyUserVoucher(userVoucherId.Value);
        //    }

        //    if (plantVoucherId.HasValue)
        //    {
        //        finalPrice -= ApplyPlantVoucher(plantVoucherId.Value);
        //    }

        //    return finalPrice;
        //}
        public void UpdatePaymentStatus(int orderId, string paymentStatus)
        {
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order != null)
            {
                order.PaymentStatus = paymentStatus; // Giả sử bạn có thuộc tính PaymentStatus trong Order
                order.ModificationDate = DateTime.UtcNow.AddHours(7); // Cập nhật ngày sửa đổi
                _unitOfWork.OrderRepository.Update(order);
                _unitOfWork.Save();
            }
        }
        public OrderVM CreateRentalOrder(CreateOrderRentalDTO createOrderDTO, int userId)
        {
            var unpaidOrdersCount = _unitOfWork.OrderRepository
                .Get(o => o.UserId == userId && o.PaymentStatus == "Chưa thanh toán" && o.PaymentId == 2 && o.Status == 1)
                .Count();

            if (unpaidOrdersCount > 3)
            {
                throw new Exception("Người dùng có hơn 3 đơn hàng thuê chưa thanh toán. Không thể tạo đơn hàng mới.");
            }

            // Lấy danh sách các đơn hàng có status = 1 và paymentId = 2
            var activeOrders = _unitOfWork.OrderRepository
                .Get(o => o.UserId == userId && o.Status == 1, includeProperties: "OrderDetails")
                .ToList();

            // Lấy danh sách PlantId trong các đơn hàng này
            var rentedPlantIds = activeOrders
                .SelectMany(o => o.OrderDetails)
                .Select(od => od.PlantId)
                .Distinct()
                .ToHashSet();

            // Kiểm tra từng PlantId trong đơn hàng mới
            foreach (var orderDetailDTO in createOrderDTO.OrderDetails)
            {
                if (orderDetailDTO.PlantId.HasValue && rentedPlantIds.Contains(orderDetailDTO.PlantId.Value))
                {
                    throw new Exception($"Cây với ID {orderDetailDTO.PlantId.Value} đã được thuê trong một đơn hàng khác. Không thể tạo đơn hàng mới với cây này.");
                }
            }

            // Tạo order mới
            Order order = _mapper.Map<Order>(createOrderDTO);
            order.CreationDate = DateTime.UtcNow.AddHours(7);
            order.TypeEcommerceId = 2;
            order.Status = 1;
            order.UserId = userId;
            double totalDeposit = 0;
            foreach (var orderDetail in order.OrderDetails)
            {
                if (orderDetail.RentalStartDate.HasValue && orderDetail.NumberMonth.HasValue)
                {
                    // Lấy giá cây từ cơ sở dữ liệu
                    var plant = _unitOfWork.PlantRepository.GetByID(orderDetail.PlantId);
                    if (plant != null)
                    {
                        totalDeposit += orderDetail.Deposit ?? 0;
                        orderDetail.RentalEndDate = orderDetail.RentalStartDate.Value.AddMonths((int)orderDetail.NumberMonth.Value);
                    }
                    else
                    {
                        throw new Exception($"Không tìm thấy cây với ID {orderDetail.PlantId}.");
                    }
                }
            }

            order.FinalPrice = order.TotalPrice + order.DeliveryFee+totalDeposit;
            order.PaymentStatus = "Chưa thanh toán";

            // Cập nhật trạng thái của cây và lưu đơn hàng
            foreach (var orderDetailDTO in createOrderDTO.OrderDetails)
            {
                if (orderDetailDTO.PlantId.HasValue)
                {
                    var plant = _unitOfWork.PlantRepository.GetByID(orderDetailDTO.PlantId.Value);
                    if (plant != null)
                    {
                        if ((bool)!plant.IsActive)
                        {
                            throw new Exception($"Cây với ID {plant.PlantId} không thể được thuê vì đã ngừng hoạt động.");
                        }
                        plant.IsActive = false;
                        _unitOfWork.PlantRepository.Update(plant);
                        var owner = _unitOfWork.UserRepository.GetByID(plant.Code);
                        if (owner != null)
                        {
                            CreateNotification(owner.UserId, "Thông báo", "Đơn hàng " + order.OrderId + " đã được tạo thành công");
                        }
                    }
                }
            }
            _unitOfWork.OrderRepository.Insert(order);
            _unitOfWork.Save();
            return _mapper.Map<OrderVM>(order);
        }


        public void UpdatePaymentOrderRental(int orderId, int contractId, int userId, int paymentId)
        {
            // Lấy thông tin hợp đồng và cập nhật
            var contract = _unitOfWork.ContractRepository.GetByID(contractId);
            if (contract != null)
            {
                contract.IsActive = 1;
                _unitOfWork.ContractRepository.Update(contract);
            }

            // Lấy thông tin người dùng và ví
            var user = _unitOfWork.UserRepository.GetByID(userId);
            var walletId = user?.WalletId;
            var wallet = _unitOfWork.WalletRepository.GetByID(walletId);

            if (wallet == null)
            {
                throw new Exception("Không tìm thấy ví của người dùng.");
            }

            // Lấy thông tin đơn hàng
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }
            if (order.Status == 5)
            {
                throw new Exception("Không thể thanh toán cho đơn hàng này vì trạng thái đơn hàng là đã huỷ.");
            }

            // Kiểm tra paymentId để quyết định logic cần thực hiện
            if (paymentId == 2)
            {
                // Kiểm tra số dư ví
                if (wallet.NumberBalance < order.FinalPrice)
                {
                    throw new Exception("Số dư trong ví không đủ để thanh toán.");
                }

                // Trừ tiền từ ví và cập nhật ví
                wallet.NumberBalance -= order.FinalPrice;
                _unitOfWork.WalletRepository.Update(wallet);

                // Tạo và thêm giao dịch mới
                Transaction transaction = new Transaction
                {
                    WalletId = walletId,
                    Description = "Thanh toán đơn hàng",
                    WithdrawNumber = order.FinalPrice,
                    RechargeNumber = null,
                    WithdrawDate = DateTime.UtcNow.AddHours(7),
                    CreationDate = DateTime.UtcNow.AddHours(7),
                    PaymentId = 2,
                    Status = 1,
                    IsActive = true
                };
                _unitOfWork.TransactionRepository.Insert(transaction);

                // Cập nhật trạng thái thanh toán đơn hàng
                order.PaymentStatus = "Đã thanh toán";
                order.Status = 2;
            }
            
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }

        public void CancelOrder(int orderId, int userId)
        {
            // Lấy thông tin đơn hàng từ cơ sở dữ liệu
            var order = _unitOfWork.OrderRepository.GetByID(orderId, includeProperties: "OrderDetails");
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            // Kiểm tra trạng thái đơn hàng (chỉ hủy nếu chưa hoàn thành)
            if (order.PaymentStatus == "Đã thanh toán")
            {
                // Lấy thông tin ví của người dùng
                var user = _unitOfWork.UserRepository.GetByID(userId);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy thông tin người dùng.");
                }

                var walletId = user.WalletId;
                var wallet = _unitOfWork.WalletRepository.GetByID(walletId);
                if (wallet == null)
                {
                    throw new Exception("Không tìm thấy ví của người dùng.");
                }

                // Hoàn tiền vào ví
                wallet.NumberBalance += order.FinalPrice ?? 0;
                _unitOfWork.WalletRepository.Update(wallet);

                // Tạo giao dịch hoàn tiền
                Transaction transaction = new Transaction
                {
                    WalletId = walletId,
                    Description = "Hoàn tiền hủy đơn hàng",
                    RechargeNumber = order.FinalPrice,
                    WithdrawNumber = null,
                    RechargeDate = DateTime.UtcNow.AddHours(7),
                    CreationDate = DateTime.UtcNow.AddHours(7),
                    PaymentId = order.PaymentId,
                    Status = 1,
                    IsActive = true
                };
                _unitOfWork.TransactionRepository.Insert(transaction);
            }

            order.Status = 5;
            order.ModificationDate = DateTime.UtcNow.AddHours(7);

            foreach (var orderDetail in order.OrderDetails)
            {
                // Cập nhật trạng thái của cây nếu cần
                if (orderDetail.PlantId.HasValue)
                {
                    var plant = _unitOfWork.PlantRepository.GetByID(orderDetail.PlantId.Value);
                    if (plant != null)
                    {
                        plant.IsActive = true; // Kích hoạt lại cây nếu đơn hàng bị hủy
                        _unitOfWork.PlantRepository.Update(plant);
                    }
                }
            }

            // Cập nhật đơn hàng
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }

        public async Task UpdatePreparedOrderSuccess(int orderId, int userId)
        {
            // Lấy thông tin đơn hàng
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            // Cập nhật mô tả giao hàng
            order.DeliveryDescription = "Chuẩn bị hàng thành công";
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            order.ModificationBy = userId;
            order.Status = 3;

            // Cập nhật thông tin đơn hàng và lưu thay đổi
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }

        public async Task UpdateDeliverOrderSuccess(int orderId, List<IFormFile> imageFiles, int userId)
        {
            // Lấy thông tin đơn hàng
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            // Kiểm tra nếu đơn hàng đã giao hàng thành công
            if (order.Status == 3 && order.DeliveryDescription == "Giao hàng thành công")
            {
                throw new InvalidOperationException("Không thể cập nhật đơn hàng đã giao hàng thành công.");
            }

            // Cập nhật mô tả giao hàng
            order.DeliveryDescription = "Giao hàng thành công";
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            order.ModificationBy = userId;

            // Kiểm tra danh sách file
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    // Mở stream từ file
                    using var stream = imageFile.OpenReadStream();
                    string fileName = imageFile.FileName;

                    // Upload file lên Firebase và lấy URL
                    string imageUrl = await _firebaseStorageService.UploadOrderDeliveryImageAsync(stream, fileName);

                    // Tạo đối tượng ImageDeliveryOrder và thêm vào cơ sở dữ liệu
                    var imageDeliveryOrder = new ImageDeliveryOrder
                    {
                        OrderId = orderId,
                        ImageUrl = imageUrl,
                        UploadDate = DateTime.UtcNow.AddHours(7)
                    };

                    order.ImageDeliveryOrders.Add(imageDeliveryOrder);
                }
            }

            // Cập nhật thông tin đơn hàng và lưu thay đổi
            _unitOfWork.OrderRepository.Update(order);

            CreateNotification(order.UserId ?? 0, "Thông báo", "Đơn hàng " + order.OrderId + " đã được giao thành công");
            _unitOfWork.Save();
        }
        public async Task UpdateDeliverOrderFail(int orderId, List<IFormFile> imageFiles, int userId)
        {
            // Lấy thông tin đơn hàng
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }
            if (order.DeliveryDescription == "Giao hàng thành công")
            {
                throw new Exception("Đơn hàng đã được giao thành công, không thể cập nhật giao hàng thất bại.");
            }
            // Cập nhật mô tả giao hàng
            order.DeliveryDescription = "Giao hàng thất bại";
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            order.ModificationBy = userId;
            order.NumberOfDeliveries += 1;
            // Kiểm tra danh sách file
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    // Mở stream từ file
                    using var stream = imageFile.OpenReadStream();
                    string fileName = imageFile.FileName;

                    // Upload file lên Firebase và lấy URL
                    string imageUrl = await _firebaseStorageService.UploadOrderDeliveryImageAsync(stream, fileName);

                    // Tạo đối tượng ImageDeliveryOrder và thêm vào cơ sở dữ liệu
                    var imageDeliveryOrder = new ImageDeliveryOrder
                    {
                        OrderId = orderId,
                        ImageUrl = imageUrl,
                        UploadDate = DateTime.UtcNow.AddHours(7)
                    };

                    order.ImageDeliveryOrders.Add(imageDeliveryOrder);
                }
            }
            if (order.NumberOfDeliveries == 3)
            {
                CancelOrder(orderId, userId);
                return;
            }
            // Cập nhật thông tin đơn hàng và lưu thay đổi
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }

        public async Task UpdateReturnOrderSuccess(int orderId, List<IFormFile> imageFiles, int userId , string depositDescription, double depositReturnOwner)
        {
            double depositReturnCustomer = 0;
            // Lấy thông tin đơn hàng
            var order = _unitOfWork.OrderRepository.GetByID(orderId, includeProperties: "OrderDetails");
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }
            bool isExpired = order.OrderDetails.Any(orderDetail =>
                             orderDetail.RentalEndDate == null || orderDetail.RentalEndDate > DateTime.UtcNow.AddHours(7));

            if (isExpired)
            {
                throw new Exception("Đơn hàng chưa hết hạn thuê, không thể thu hồi.");
            }
            // Cập nhật mô tả giao hàng
            order.DeliveryDescription = "Thu hồi thành công";
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            order.ModificationBy = userId;
            order.Status = 6;
            foreach (var orderDetail in order.OrderDetails)
            {
                if (depositReturnOwner > orderDetail.Deposit)
                {
                    throw new Exception($"Số tiền trả lại cho chủ cây không được lớn hơn tiền cọc ({orderDetail.Deposit}).");
                }
                orderDetail.DepositDescription = depositDescription;
                orderDetail.DepositReturnOwner = depositReturnOwner;
                orderDetail.DepositReturnCustomer = orderDetail.Deposit - depositReturnOwner;
                depositReturnCustomer = orderDetail.Deposit - depositReturnOwner ?? 0;
            }
            // Kiểm tra danh sách file
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    // Mở stream từ file
                    using var stream = imageFile.OpenReadStream();
                    string fileName = imageFile.FileName;

                    // Upload file lên Firebase và lấy URL
                    string imageUrl = await _firebaseStorageService.UploadOrderReturnImageAsync(stream, fileName);

                    // Tạo đối tượng ImageDeliveryOrder và thêm vào cơ sở dữ liệu
                    var imageReturnOrder = new ImageReturnOrder
                    {
                        OrderId = orderId,
                        ImageUrl = imageUrl,
                        UploadDate = DateTime.UtcNow.AddHours(7)
                    };

                    order.ImageReturnOrders.Add(imageReturnOrder);
                }
            }

            var owner = await _unitOfWork.UserRepository.GetByIDAsync(userId); // Giả sử userId là của owner
            if (owner == null)
            {
                throw new Exception("Không tìm thấy thông tin người dùng chủ cây.");
            }

            var ownerWallet = await _unitOfWork.WalletRepository.GetByIDAsync(owner.WalletId);
            if (ownerWallet == null)
            {
                throw new Exception("Không tìm thấy ví của người dùng chủ cây.");
            }

            // Cộng tiền vào ví của chủ cây
            ownerWallet.NumberBalance += depositReturnOwner;
            _unitOfWork.WalletRepository.Update(ownerWallet);

            // Tạo giao dịch cộng tiền cho chủ cây (owner)
            Transaction ownerTransaction = new Transaction
            {
                WalletId = ownerWallet.WalletId,
                Description = "Trả tiền cọc từ đơn hàng " + order.OrderId,
                RechargeNumber = depositReturnOwner,
                WithdrawNumber = null,
                RechargeDate = DateTime.UtcNow.AddHours(7),
                CreationDate = DateTime.UtcNow.AddHours(7),
                PaymentId = 2,  
                Status = 1,
                IsActive = true
            };
            _unitOfWork.TransactionRepository.Insert(ownerTransaction);

            // Trả tiền cọc cho người dùng (user)
            var customer = await _unitOfWork.UserRepository.GetByIDAsync(order.UserId); // Giả sử order.UserId là của khách hàng
            if (customer == null)
            {
                throw new Exception("Không tìm thấy thông tin người dùng.");
            }

            var customerWallet = await _unitOfWork.WalletRepository.GetByIDAsync(customer.WalletId);
            if (customerWallet == null)
            {
                throw new Exception("Không tìm thấy ví của người dùng.");
            }

            customerWallet.NumberBalance += depositReturnCustomer;
            _unitOfWork.WalletRepository.Update(customerWallet);

            Transaction customerTransaction = new Transaction
            {
                WalletId = customerWallet.WalletId,
                Description = "Trả tiền cọc từ đơn hàng " + order.OrderId,
                RechargeNumber = depositReturnCustomer,
                WithdrawNumber = null,
                RechargeDate = DateTime.UtcNow.AddHours(7),
                CreationDate = DateTime.UtcNow.AddHours(7),
                PaymentId = 2, 
                Status = 1,
                IsActive = true
            };
            _unitOfWork.TransactionRepository.Insert(customerTransaction);
            CreateNotification(order.UserId ?? 0, "Thông báo", "Đơn hàng " + order.OrderId + " đã được thu hồi thành công");
            // Cập nhật thông tin đơn hàng và lưu thay đổi
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }

        public async Task UpdateReturnOrderFail(int orderId, int userId)
        {
            // Lấy thông tin đơn hàng
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }
            bool isExpired = order.OrderDetails.Any(orderDetail =>
                 orderDetail.RentalEndDate == null || orderDetail.RentalEndDate > DateTime.UtcNow.AddHours(7));

            if (isExpired)
            {
                throw new Exception("Đơn hàng chưa hết hạn thuê, không thể thu hồi.");
            }
            // Cập nhật mô tả giao hàng
            order.DeliveryDescription = "Thu hồi thất bại";
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            order.ModificationBy = userId;
            
            // Cập nhật thông tin đơn hàng và lưu thay đổi
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }
        public void UpdateOrderStatus(int orderId, int newStatus, int userId)
        {
            // Lấy thông tin đơn hàng từ cơ sở dữ liệu
            var order = _unitOfWork.OrderRepository.GetByID(orderId, includeProperties: "OrderDetails");
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = newStatus;
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            order.ModificationBy = userId;

            if (newStatus == 4 && order.PaymentStatus == "Đã thanh toán")
            {
                // Tính tổng số tiền từ tất cả các OrderDetail
                double totalAmount = order.TotalPrice + order.DeliveryFee ?? 0;

                // Kiểm tra thông tin ví của người dùng chủ cây
                User owner = null;
                Wallet wallet = null;

                foreach (var orderDetail in order.OrderDetails)
                {
                    if (orderDetail.PlantId.HasValue)
                    {
                        var plant = _unitOfWork.PlantRepository.GetByID(orderDetail.PlantId.Value);
                        if (plant != null)
                        {
                            owner = _unitOfWork.UserRepository.GetByID(plant.Code);
                            if (owner != null)
                            {
                                wallet = _unitOfWork.WalletRepository.GetByID(owner.WalletId);
                                break; 
                            }
                        }
                    }
                }

                if (owner == null)
                {
                    throw new Exception("Không tìm thấy thông tin người dùng chủ cây.");
                }

                if (wallet == null)
                {
                    throw new Exception("Không tìm thấy ví của người dùng chủ cây.");
                }

                // Cộng tiền vào ví
                wallet.NumberBalance += (int)(totalAmount * 80 / 100);
                _unitOfWork.WalletRepository.Update(wallet);

                // Tạo giao dịch cộng tiền
                Transaction transaction = new Transaction
                {
                    WalletId = wallet.WalletId,
                    Description = "Nhận tiền từ đơn hàng " +order.OrderId,
                    RechargeNumber = (int)(totalAmount * 80 / 100),
                    WithdrawNumber = null,
                    RechargeDate = DateTime.UtcNow.AddHours(7),
                    CreationDate = DateTime.UtcNow.AddHours(7),
                    PaymentId = 2,
                    Status = 1,
                    IsActive = true
                };
                _unitOfWork.TransactionRepository.Insert(transaction);
                // Cộng 20% vào ví manager có WalletId = 5
                Wallet managerWallet = _unitOfWork.WalletRepository.GetByID(5);
                if (managerWallet == null)
                {
                    throw new Exception("Không tìm thấy ví của manager.");
                }

                int managerAmount = (int)(totalAmount * 20 / 100);
                managerWallet.NumberBalance += managerAmount;
                _unitOfWork.WalletRepository.Update(managerWallet);

                // Tạo giao dịch cộng tiền cho manager
                _unitOfWork.TransactionRepository.Insert(new Transaction
                {
                    WalletId = managerWallet.WalletId,
                    Description = "Nhận tiền hoa hồng từ đơn hàng " + order.OrderId,
                    RechargeNumber = managerAmount,
                    WithdrawNumber = null,
                    RechargeDate = DateTime.UtcNow.AddHours(7),
                    CreationDate = DateTime.UtcNow.AddHours(7),
                    PaymentId = 2,
                    Status = 1,
                    IsActive = true
                });
            
        }

            CreateNotification(order.UserId ?? 0, "Thông báo", "Đơn hàng " + order.OrderId + " đã được cập nhật");
            _unitOfWork.OrderRepository.Update(order);

            _unitOfWork.Save();
        }

        public IEnumerable<OrderVM> GetOrdersByOwner(int userId, int pageIndex, int pageSize)
        {
            // Lấy danh sách đơn hàng liên quan đến các Plant có Code trùng với userId
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.OrderDetails.Any(od => od.Plant.Code == userId.ToString()),
                orderBy: o => o.OrderBy(order => order.Status).ThenByDescending(order => order.CreationDate),
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails,OrderDetails.Plant"
            );

            // Ánh xạ sang OrderVM
            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }

        public IEnumerable<OrderVM> GetOrdersByTypeEcommerceId(int typeEcommerceId, DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize)
        {
            // Lấy danh sách đơn hàng theo typeEcommerceId và lọc theo ngày nếu có
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.TypeEcommerceId == typeEcommerceId &&
                            (!startDate.HasValue || o.CreationDate >= startDate.Value) &&
                            (!endDate.HasValue || o.CreationDate <= endDate.Value),
                orderBy: o => o.OrderByDescending(order => order.OrderId), // Sắp xếp theo OrderId giảm dần
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails,OrderDetails.Plant"
            );

            // Ánh xạ sang OrderVM
            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }

        public async Task<int> CountOrderByStatus(int status)
        {
            var orderCount = await Task.FromResult(_unitOfWork.OrderRepository.Get(
                filter: o => o.Status != status
            ).Count());

            return orderCount;
        }
        public async Task<double> CountOrderPrice(int status)
        {
            var query = _unitOfWork.OrderRepository.Get(
                filter: o => o.FinalPrice.HasValue && o.Status != status 
            );
       
            var totalRevenue = query.Any() ? query.Sum(o => o.FinalPrice.Value) : 0;

            return totalRevenue;
        }
        public async Task<double> CountOrderPriceDateNow(int status)
        {
            var todayStart = DateTime.Today; // 00:00:00 của ngày hiện tại
            var todayEnd = todayStart.AddDays(1).AddTicks(-1); // 23:59:59.9999999 của ngày hiện tại

            var query = _unitOfWork.OrderRepository.Get(
                filter: o =>
                    o.FinalPrice.HasValue &&
                    o.Status != status &&
                    o.CreationDate >= todayStart && // Từ đầu ngày
                    o.CreationDate <= todayEnd // Đến cuối ngày
            );

            var totalRevenue = query.Any() ? query.Sum(o => o.FinalPrice.Value) : 0;

            return totalRevenue;
        }
        public void UpdateOrderDetailDeposit(int orderDetailId, string depositDescription, double? depositReturnCustomer, double? depositReturnOwner)
        {
            // Lấy OrderDetail từ cơ sở dữ liệu
            var orderDetail = _unitOfWork.OrderDetailRepository.GetByID(orderDetailId);

            if (orderDetail == null)
            {
                throw new Exception($"Không tìm thấy OrderDetail với ID {orderDetailId}.");
            }

            // Cập nhật các trường nếu có giá trị
            if (!string.IsNullOrWhiteSpace(depositDescription))
            {
                orderDetail.DepositDescription = depositDescription;
            }

            if (depositReturnCustomer.HasValue)
            {
                orderDetail.DepositReturnCustomer = depositReturnCustomer.Value;
            }

            if (depositReturnOwner.HasValue)
            {
                orderDetail.DepositReturnOwner = depositReturnOwner.Value;
            }
            var order = _unitOfWork.OrderRepository.GetByID(orderDetail.OrderId);
            order.Status = 6;
            order.DeliveryDescription = "Thu hồi thành công";
            // Lưu thay đổi vào cơ sở dữ liệu
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.OrderDetailRepository.Update(orderDetail);
            _unitOfWork.Save();
        }

        public void CreateNotification(int userId, string title, string description)
        {
            // Tạo đối tượng thông báo mới
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Description = description,
                CreatedDate = DateTime.UtcNow.AddHours(7),
                UpdatedDate = DateTime.UtcNow.AddHours(7),
                IsRead = false,
                IsNotifications = false,
                Status = 1
            };

            // Thêm vào cơ sở dữ liệu
            _unitOfWork.NotificationRepository.Insert(notification);
            _unitOfWork.Save();
        }
        public void CustomerNotReceivedOrder(int orderId, int userId)
        {
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            if (order.UserId != userId)
            {
                throw new Exception("Người dùng không có quyền thao tác trên đơn hàng này.");
            }

            if (order.Status != 3) // Giả định trạng thái 3 là "Đang giao hàng"
            {
                throw new Exception("Đơn hàng không ở trạng thái có thể xác nhận chưa nhận hàng.");
            }
            order.DeliveryDescription = "Khách hàng chưa nhận được đơn hàng";
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            order.ModificationBy = userId;

            _unitOfWork.OrderRepository.Update(order);

            _unitOfWork.Save();
        }
    }

}
