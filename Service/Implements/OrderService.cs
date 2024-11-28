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
            var orders = _unitOfWork.OrderRepository.Get(filter: c => c.Status != 0, orderBy: query => query.OrderByDescending(c => c.OrderId), pageIndex: pageIndex, pageSize: pageSize, includeProperties: "OrderDetails");
            return _mapper.Map<IEnumerable<OrderVM>>(orders);
        }

        public OrderVM GetOrderById(int id)
        {
            var order =_unitOfWork.OrderRepository.GetByID(id, includeProperties: "OrderDetails");
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
                filter: o => o.UserId == userId && o.Status == status && o.TypeEcommerceId == 1,
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "OrderDetails,OrderDetails.Plant" // Bao gồm thông tin chi tiết đơn hàng
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

            foreach (var orderDetail in order.OrderDetails)
            {
                if (orderDetail.RentalStartDate.HasValue && orderDetail.NumberMonth.HasValue)
                {
                    // Lấy giá cây từ cơ sở dữ liệu
                    var plant = _unitOfWork.PlantRepository.GetByID(orderDetail.PlantId);
                    if (plant != null)
                    {
                        //// Tính giá cho OrderDetail = giá cây * số tháng
                        //double priceForOrderDetail = plant.Price * orderDetail.NumberMonth.Value;

                        //// Cộng dồn giá này vào tổng giá của Order
                        //order.TotalPrice += priceForOrderDetail;

                        // Tính RentalEndDate
                        orderDetail.RentalEndDate = orderDetail.RentalStartDate.Value.AddMonths((int)orderDetail.NumberMonth.Value).AddDays(3);
                    }
                    else
                    {
                        throw new Exception($"Không tìm thấy cây với ID {orderDetail.PlantId}.");
                    }
                }
            }

            order.FinalPrice = order.TotalPrice + order.DeliveryFee;
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

            // Cập nhật mô tả giao hàng
            order.DeliveryDescription = "Giao hàng thất bại";
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
            _unitOfWork.Save();
        }

        public async Task UpdateReturnOrderSuccess(int orderId, List<IFormFile> imageFiles, int userId)
        {
            // Lấy thông tin đơn hàng
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            // Cập nhật mô tả giao hàng
            order.DeliveryDescription = "Thu hồi thành công";
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

            // Cập nhật thông tin đơn hàng và lưu thay đổi
            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();
        }
        public void UpdateOrderStatus(int orderId, int newStatus, int userId)
        {
            // Lấy thông tin đơn hàng từ cơ sở dữ liệu
            var order = _unitOfWork.OrderRepository.GetByID(orderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = newStatus;
            order.ModificationDate = DateTime.UtcNow.AddHours(7);
            order.ModificationBy = userId;

            _unitOfWork.OrderRepository.Update(order);

            _unitOfWork.Save();
        }

        public IEnumerable<OrderVM> GetOrdersByOwner(int userId, int pageIndex, int pageSize)
        {
            // Lấy danh sách đơn hàng liên quan đến các Plant có Code trùng với userId
            var orders = _unitOfWork.OrderRepository.Get(
                filter: o => o.OrderDetails.Any(od => od.Plant.Code == userId.ToString()),
                orderBy: o => o.OrderBy(order => order.Status),
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

    }

}
