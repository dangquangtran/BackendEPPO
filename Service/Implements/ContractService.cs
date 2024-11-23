using AutoMapper;
using BusinessObjects.Models;
using DTOs.Contracts;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Repository.Interfaces;
using Service.Implements;
using Service.Interfaces;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Service
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        private readonly FirebaseStorageService _firebaseStorageService;

        private string fileName = null;

        public ContractService(IUnitOfWork unitOfWork, IMapper mapper, FirebaseStorageService firebaseStorageService)
        {
            _unitOfWork = unitOfWork;
            _firebaseStorageService = firebaseStorageService;
            _mapper = mapper;

        }
        public async Task UpdateStatusContractID(int contractId, int status)
        {
            var entity = await Task.FromResult(_unitOfWork.ContractRepository.GetByID(contractId));

            if (entity == null)
            {
                throw new Exception($"Contract with ID {contractId} not found.");
            }

            entity.IsActive = 1;
            entity.Status = status;

            _unitOfWork.ContractRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }



        public async Task<IEnumerable<Contract>> GetListContract(int page, int size)
        {
            return await _unitOfWork.ContractRepository.GetAsync(filter: c => c.Status != 0, includeProperties: "User", pageIndex: page, pageSize: size);
        }

        public async Task<IEnumerable<Contract>> GetContractOfUser(int userID)
        {
            var contract = _unitOfWork.ContractRepository.Get(
                filter: c => c.UserId == userID && c.Status != 0
            );
            return _mapper.Map<IEnumerable<Contract>>(contract);
            //    return await _unitOfWork.ContractRepository.GetAsync(filter: c => c.Status != 0, includeProperties: "User", pageIndex: page, pageSize: size);
        }

        public async Task<Contract> GetContractByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.ContractRepository.GetByID(Id));
        }

        public async Task UpdateContract(UpdateContractDTO contract)
        {
            var entity = await Task.FromResult(_unitOfWork.ContractRepository.GetByID(contract.ContractId));

            if (entity == null)
            {
                throw new Exception($"Contract with ID {contract.ContractId} not found.");
            }
            entity.UserId = contract.UserId;
            entity.ContractNumber = contract.ContractNumber;
            entity.Description = contract.Description;
            entity.CreationContractDate = contract.CreationContractDate;
            entity.EndContractDate = contract.EndContractDate;
            entity.TotalAmount = contract.TotalAmount;
            entity.CreatedAt = contract.CreatedAt;
            entity.UpdatedAt = contract.UpdatedAt;
            entity.TypeContract = contract.TypeContract;
            entity.ContractUrl = contract.ContractUrl;
            entity.IsActive = contract.IsActive;
            entity.Status = contract.Status;

            _unitOfWork.ContractRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }


        public async Task<int> CreateContract(CreateContractDTO contract , int userId)
        {

            var entity = new Contract
            {
                UserId = userId,
                ContractNumber = contract.ContractNumber,
                Description = contract.Description,
                CreationContractDate = contract.CreationContractDate,
                EndContractDate = contract.EndContractDate,
                TotalAmount = contract.TotalAmount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                TypeContract = "Thuê Cây",
                ContractUrl = contract.ContractUrl,
                IsActive = 1,
                Status = 1,
            };

            _unitOfWork.ContractRepository.Insert(entity);
            await _unitOfWork.SaveAsync();

            if (contract.ContractDetails != null && contract.ContractDetails.Any())
            {
                foreach (var contractDetail in contract.ContractDetails)
                {
                    var contractDetailEntity = new ContractDetail
                    {
                        ContractId = entity.ContractId,  // Gán ContractId từ hợp đồng đã tạo
                        PlantId = contractDetail.PlantId,
                        Quantity = 1,
                        TotalPrice = contractDetail.TotalPrice,
                        IsActive = true,  // Gán giá trị mặc định IsActive nếu null
                        Status = 1,  // Gán mặc định Status nếu null
                    };

                    // Lưu mỗi ContractDetail vào cơ sở dữ liệu
                    _unitOfWork.ContractDetailRepository.Insert(contractDetailEntity);
                }
                await _unitOfWork.SaveAsync();
            }



            string pdfUrl = await GenerateContractPdfAsync(contract, userId);
            entity.ContractUrl = pdfUrl;
            entity.ContractFileName = fileName;

            _unitOfWork.ContractRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return entity.ContractId;
        }


        public async Task CreatePartnershipContract(CreateContractPartnershipDTO contract, int userID)
        {



            var entity = new Contract
            {
                UserId = userID,
                ContractNumber = userID,
                Description = "Hợp Tác Kinh Doanh",
                CreationContractDate = DateTime.UtcNow,
                EndContractDate = DateTime.UtcNow.AddYears(2),
                TotalAmount = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.UtcNow,
                TypeContract = "Hợp Tác Kinh Doanh",
                ContractUrl = contract.ContractUrl,
                IsActive = 1,
                Status = 1,
            };

            _unitOfWork.ContractRepository.Insert(entity);
            await _unitOfWork.SaveAsync();

            if (contract.ContractDetails != null && contract.ContractDetails.Any())
            {
                foreach (var contractDetail in contract.ContractDetails)
                {
                    var contractDetailEntity = new ContractDetail
                    {
                        ContractId = entity.ContractId,
                        PlantId = 1,
                        Quantity = 1,
                        TotalPrice = 1,
                        IsActive = true,  // Gán giá trị mặc định IsActive nếu null
                        Status = 1,  // Gán mặc định Status nếu null
                    };

                    // Lưu mỗi ContractDetail vào cơ sở dữ liệu
                    _unitOfWork.ContractDetailRepository.Insert(contractDetailEntity);
                }
                await _unitOfWork.SaveAsync();
            }



            string pdfUrl = await GenerateBusinessPartnershipContractPdfAsync(contract, userID);
            entity.ContractUrl = pdfUrl;
            entity.ContractFileName = fileName;

            _unitOfWork.ContractRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }




        public async Task<string> GenerateContractPdfAsync(CreateContractDTO contract , int userId)
        {
            var user = await Task.FromResult(_unitOfWork.UserRepository.GetByID(userId));
            if (user == null)
            {
                throw new Exception("No data user.");
            }



            fileName = $"Contract_{contract.ContractNumber}_{DateTime.Now.Ticks}.pdf";

            string pdfPath = Path.Combine("wwwroot", "contracts", fileName);

            // Tạo thư mục nếu chưa có
            Directory.CreateDirectory(Path.GetDirectoryName(pdfPath));

            using (PdfDocument pdfDoc = new PdfDocument())
            {
                pdfDoc.Info.Title = $"Contract {contract.ContractNumber}";

                // Tạo trang PDF
                PdfPage page = pdfDoc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 10);
                XFont titleFont = new XFont("Arial", 14, XFontStyleEx.Bold);

                double margin = 50;
                int lineHeight = 20;
                double yPoint = margin;
                double pageHeightLimit = page.Height - margin;

                double pageWidth = page.Width;
                double textWidth = gfx.MeasureString("HỢP ĐỒNG THUÊ CÂY", titleFont).Width;
                double centerX = (pageWidth - textWidth) / 2;

                void CreateNewPage()
                {
                    page = pdfDoc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPoint = margin;
                }
                if (yPoint >= pageHeightLimit) CreateNewPage();

                // Tiêu đề hợp đồng
                gfx.DrawString($"HỢP ĐỒNG THUÊ CÂY", titleFont, XBrushes.Black, new XPoint(centerX, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"Số: {contract.ContractNumber}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"Ngày ký: {contract.CreationContractDate?.ToString("dd/MM/yyyy")}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Căn cứ vào các quy định pháp luật
                gfx.DrawString($"Căn cứ vào:", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Bộ luật Dân sự nước Cộng hòa Xã hội Chủ nghĩa Việt Nam năm 2015.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Các quy định pháp luật hiện hành liên quan đến hợp đồng thuê tài sản.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Thông tin bên A và bên B
                gfx.DrawString($"Hôm nay, ngày {contract.CreationContractDate?.ToString("dd")} tháng {contract.CreationContractDate?.ToString("MM")} năm {contract.CreationContractDate?.ToString("yyyy")}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Viết nội dung vào PDF
                gfx.DrawString("BÊN CHO THUÊ (Bên A):", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Họ và tên: Ông Đỗ Hữu Thuận", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Địa chỉ: Quận 9, Thành phố Hồ Chí Minh", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Số điện thoại: 0333888257", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Chứng minh nhân dân/Căn cước công dân: 074202112390", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Email: EPPO.HCM@gmail.com", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Thông tin Bên B
                gfx.DrawString("BÊN THUÊ (Bên B):", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Họ và tên: {user.FullName}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Giới tính: {user.Gender}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Ngày sinh: {user.DateOfBirth}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Số điện thoại: {user.PhoneNumber}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Chứng minh nhân dân/Căn cước công dân: {user.IdentificationCard}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Email: {user.Email}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();


                // Điều khoản hợp đồng
                gfx.DrawString("Cùng nhau thỏa thuận ký kết hợp đồng thuê cây với các điều khoản như sau:", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Điều 1: Đối tượng hợp đồng
                gfx.DrawString("Điều 1: Đối Tượng Hợp Đồng", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (contract.ContractDetails != null && contract.ContractDetails.Any())
                {
                    foreach (var contractDetail in contract.ContractDetails)
                    {


                        var plant = _unitOfWork.PlantRepository.GetByID(contractDetail.PlantId);

                  
                gfx.DrawString($"1. Mô tả cây {plant.PlantName} cho thuê:", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                        gfx.DrawString($"•  Mô tả cây {plant.Title} cho thuê:", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"•  Mô tả cây {plant.Description} cho thuê:", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"   • Số lượng: 1 ", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
    
                gfx.DrawString($"   • Giá trị của cây (ước tính): {plant.Price} VNĐ", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                        if (yPoint >= pageHeightLimit) CreateNewPage();
                    }
                }

                // Điều 2: Thời gian thuê
                gfx.DrawString("Điều 2: Thời gian thuê", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"• Thời gian thuê cây mai bắt đầu từ ngày {contract.CreationContractDate:yyyy-MM-dd} đến ngày {contract.EndContractDate:yyyy-MM-dd}.", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"• Bên B có quyền gia hạn hợp đồng thuê nếu có thỏa thuận và sự đồng ý của Bên A.", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;

                        // Điều 3: Giá thuê và phương thức thanh toán
                        gfx.DrawString("Điều 3: Giá thuê và phương thức thanh toán", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"1. Giá thuê: {contract.TotalAmount} VND/tháng", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"• Tổng giá trị hợp đồng thuê trong {contract.TotalAmount} tháng là  {contract.CreationContractDate:MM-dd}.", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"2. Phương thức thanh toán:", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"• Bên B thanh toán qua chuyển khoản vào tài khoản của Bên A:", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"  - Số tài khoản: 040704070013100", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"  - Ngân hàng: HDBank", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"  - Chủ tài khoản: ĐỖ HỮU THUẬN", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;
                        gfx.DrawString($"• Thanh toán sẽ được thực hiện vào ngày  {contract.CreationContractDate:dd} hàng tháng.", font, XBrushes.Black, new XPoint(margin, yPoint));
                        yPoint += lineHeight;

                if (yPoint >= pageHeightLimit) CreateNewPage();
                // Điều 4: Quyền và nghĩa vụ của Bên A
                gfx.DrawString("Điều 4: Quyền và nghĩa vụ của Bên A", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"1. Bên A có trách nhiệm cung cấp cây mai đúng mô tả như trong hợp đồng.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"2. Bên A bảo đảm cây mai không bị nhiễm bệnh, hư hỏng nghiêm trọng trong suốt thời gian thuê.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"3. Bên A có quyền kiểm tra cây mai định kỳ để đảm bảo cây được chăm sóc đúng cách.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Điều 5: Quyền và nghĩa vụ của Bên B
                gfx.DrawString("Điều 5: Quyền và nghĩa vụ của Bên B", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"1. Bên B có trách nhiệm chăm sóc, bảo quản cây mai trong suốt thời gian thuê.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"2. Bên B không được phép chuyển nhượng quyền thuê cây cho bên thứ ba mà không có sự đồng ý của Bên A.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"3. Nếu cây mai bị hư hỏng hoặc chết do lỗi của Bên B, Bên B có trách nhiệm bồi thường hoặc thay thế cây.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Điều 6: Vi phạm hợp đồng
                gfx.DrawString("Điều 6: Vi phạm hợp đồng", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"1. Nếu một bên vi phạm các điều khoản của hợp đồng, bên còn lại có quyền yêu cầu chấm dứt hợp đồng và yêu", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"cầu bồi thường thiệt hại.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"2. Trường hợp không thanh toán tiền thuê đúng hạn, Bên A có quyền yêu cầu Bên B thanh toán lãi suất theo", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"quy định của pháp luật.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();
                // Điều 7: Điều khoản chung
                gfx.DrawString("Điều 7: Điều khoản chung", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"1. Hợp đồng có hiệu lực kể từ ngày ký.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"2. Hai bên cam kết thực hiện đầy đủ các điều khoản trong hợp đồng. Mọi thay đổi, bổ sung hợp đồng phải có sự", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"đồng ý bằng văn bản của cả hai bên.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"3. Mọi tranh chấp phát sinh trong quá trình thực hiện hợp đồng sẽ được giải quyết thông qua thương lượng,", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"nếu không giải quyết được, sẽ đưa ra tòa án có thẩm quyền.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;


                // Phần ký tên
                gfx.DrawString("ĐẠI DIỆN BÊN A                                          ĐẠI DIỆN BÊN B", titleFont, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"Đỗ Hữu Thuận                                                                               {user.FullName}", font, XBrushes.Red, new XPoint(margin, yPoint));

                // Lưu tài liệu PDF
                pdfDoc.Save(pdfPath);
            }
            using (var pdfStream = new FileStream(pdfPath, FileMode.Open))
            {
                string fileUrl = await _firebaseStorageService.UploadContractPdfAsync(pdfStream, fileName);
                return fileUrl;
            }

            //return $"/contracts/{fileName}";
        }


        public async Task<string> GenerateBusinessPartnershipContractPdfAsync(CreateContractPartnershipDTO contract, int userID)
        {
            var user = await Task.FromResult(_unitOfWork.UserRepository.GetByID(userID));
            if (user == null)
            {
                throw new Exception("No data user.");
            }


            // Define file path and name
            string fileName = $"BusinessPartnershipContract_{DateTime.Now.Ticks}.pdf";
            string pdfPath = Path.Combine("wwwroot", "contracts", fileName);

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(pdfPath));

            // Initialize PDF document and page
            using (PdfDocument pdfDoc = new PdfDocument())
            {
                pdfDoc.Info.Title = $"Business Partnership Contract";
                PdfPage page = pdfDoc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 10);
                XFont titleFont = new XFont("Arial", 14);

                double margin = 50;
                int lineHeight = 20;
                double yPoint = margin;
                double pageHeightLimit = page.Height - margin;
                double pageWidth = page.Width;

                // Function to create a new page if content overflows
                void CreateNewPage()
                {
                    page = pdfDoc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPoint = margin;
                }

                // Title of the contract
                double textWidth = gfx.MeasureString("HỢP ĐỒNG HỢP TÁC KINH DOANH VỀ CHO THUÊ CÂY", titleFont).Width;
                double centerX = (pageWidth - textWidth) / 2;
                gfx.DrawString("HỢP ĐỒNG HỢP TÁC KINH DOANH VỀ CHO THUÊ CÂY", titleFont, XBrushes.Black, new XPoint(centerX, yPoint));
                yPoint += lineHeight;

                // Legal basis
                gfx.DrawString("Căn cứ vào:", titleFont, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Luật Dân sự Việt Nam năm 2015", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Các quy định pháp lý khác có liên quan", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Mong muốn hợp tác kinh doanh giữa " + user.FullName + " và Hệ Thống EPPO", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();
                // Date and location
                gfx.DrawString($"Hôm nay, ngày {DateTime.Now.ToString("dd")} tháng {DateTime.Now.ToString("MM")} năm {DateTime.Now.ToString("yyyy")}, tại {userID}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;


                // Party B (Bên Thuê)
                gfx.DrawString("BÊN THUÊ A (CÔNG TY HOẶC CÁ NHÂN THUÊ CÂY):", titleFont, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Tên: EPPO", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Địa chỉ: 299 Đ. Liên Phường, Phường Phú Hữu, Thủ Đức, Hồ Chí Minh ", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Mã số thuế: 483874098128723", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Điện thoại: 0333888257", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Đại diện: Ông Đỗ Hữu Thuận (CEO)", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;


                // Party A (Bên Cho Thuê)
                gfx.DrawString("BÊN CHO THUÊ B (CHỦ SỞ HỮU):", titleFont, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Tên: {user.FullName}", font, XBrushes.Black, new XPoint(margin, yPoint)); // Replace contract.UserId with userID
                yPoint += lineHeight;
                gfx.DrawString($"• Địa chỉ: Thủ Đức, Hồ Chí Minh", font, XBrushes.Black, new XPoint(margin, yPoint)); // Replace contract.UserId with actual user data
                yPoint += lineHeight;
                gfx.DrawString($"• CCCD/CMND: {user.IdentificationCard}", font, XBrushes.Black, new XPoint(margin, yPoint)); // Replace contract.UserId with actual user data
                yPoint += lineHeight;
                gfx.DrawString($"• Điện thoại: {user.PhoneNumber}", font, XBrushes.Black, new XPoint(margin, yPoint)); // Replace contract.UserId with actual user data
                yPoint += lineHeight;
                gfx.DrawString($"• Đại diện: {user.UserName}", font, XBrushes.Black, new XPoint(margin, yPoint)); // Replace contract.UserId with actual user data
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();

                gfx.DrawString("Bên A (Chủ Shop) cam kết hợp tác với Bên B (Nhà Vườn) trong việc đăng tải các sản phẩm cây cối, hoa, cảnh lên hệ thống của Bên A, ", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("nhằm phục vụ cho nhu cầu mua bán, cho thuê, và đấu giá các sản phẩm cây cối, hoa, cảnh của Bên B.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;


                gfx.DrawString("Quyền và nghĩa vụ của Bên A (Chủ Shop)", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Cung cấp nền tảng trực tuyến cho phép Bên B đăng tải thông tin về sản phẩm cây cối, hoa, cảnh lên hệ thống.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Đảm bảo hệ thống vận hành ổn định và bảo mật thông tin giao dịch của Bên B.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Cung cấp công cụ quản lý sản phẩm, kiểm tra đơn hàng, và hỗ trợ xử lý giao dịch.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Cung cấp hỗ trợ kỹ thuật khi Bên B gặp vấn đề với việc đăng tải sản phẩm trên hệ thống.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Thực hiện các chiến dịch quảng bá và tiếp thị sản phẩm của Bên B trên nền tảng của Bên A.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();

                // Điều 3: Quyền và nghĩa vụ của Bên B
                gfx.DrawString("Quyền và nghĩa vụ của Bên B (Nhà Vườn)", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Cung cấp thông tin chính xác, đầy đủ về các sản phẩm cây cối, hoa, cảnh mà Bên B muốn đăng lên hệ thống, bao gồm mô tả chi tiết, hình ảnh và giá bán.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Đảm bảo các sản phẩm cây cối, hoa, cảnh đáp ứng các tiêu chuẩn chất lượng và có nguồn gốc hợp pháp.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Thanh toán các khoản phí dịch vụ cho Bên A theo tỷ lệ đã thỏa thuận trong hợp đồng.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Cung cấp dịch vụ chăm sóc, bảo dưỡng cho cây cối, hoa, cảnh nếu có yêu cầu từ khách hàng thông qua hệ thống.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Chịu trách nhiệm về mọi vấn đề phát sinh liên quan đến chất lượng sản phẩm, giao dịch với khách hàng.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();
                gfx.DrawString("Phương thức thanh toán", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Các khoản phí dịch vụ, hoa hồng và thanh toán sẽ được thực hiện qua chuyển khoản ngân hàng hoặc các", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("•phương thức thanh toán trực tuyến mà hai bên đồng thuận.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Bên B sẽ thanh toán phí dịch vụ cho Bên A vào mỗi kỳ thanh toán (theo tháng/quý/năm) hoặc ngay sau khi hoàn tất ", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("•mỗi giao dịch, tùy theo thỏa thuận giữa hai bên.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();

                // Article 1: Purpose of the contract
                if (yPoint >= pageHeightLimit) CreateNewPage();
                gfx.DrawString("Điều 1: Mục đích hợp tác", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"Bên cho thuê cam kết cho bên thuê mượn cây cối, hoa, cảnh (các loại cây) để phục vụ cho nhu cầu kinh doanh, trang trí, sự kiện, hoặc các ", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"mục đích hợp pháp khác của bên thuê.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"Bên thuê cam kết sử dụng cây cối đúng mục đích và chịu trách nhiệm bảo vệ cây trong suốt thời gian thuê.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Article 2: Rental time and location
                gfx.DrawString("Điều 2: Thời gian và địa điểm cho thuê", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"Địa điểm giao nhận cây: 299 Đ. Liên Phường, Phường Phú Hữu, Thủ Đức, Hồ Chí Minh.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Article 3: Rights and obligations of Party A (Bên Cho Thuê)
                gfx.DrawString("Điều 3: Quyền và nghĩa vụ của bên cho thuê", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Cung cấp cây cối đúng chất lượng, số lượng như đã thỏa thuận.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Đảm bảo cây cối được chăm sóc, bảo dưỡng tốt trước khi giao cho bên thuê.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Cung cấp dịch vụ bảo trì, sửa chữa cây cối khi có sự cố xảy ra (nếu có thỏa thuận).", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();

                // Điều 4: Quyền và nghĩa vụ của bên thuê
                gfx.DrawString("Điều 4: Quyền và nghĩa vụ của bên thuê", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Cam kết thanh toán đầy đủ số tiền thuê theo thỏa thuận trong hợp đồng.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Đảm bảo sử dụng cây cối đúng mục đích, không thay đổi cấu trúc cây hoặc làm tổn hại đến chất lượng cây.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Chịu trách nhiệm sửa chữa hoặc bồi thường nếu cây bị hư hỏng trong quá trình thuê.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();
                // Điều 4: Phí dịch vụ và hoa hồng
                gfx.DrawString("Điều 5: Phí dịch vụ và hoa hồng", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Hệ thống sẽ thu phí dịch vụ hoặc hoa hồng từ các giao dịch mua bán, cho thuê và đấu giá theo tỷ lệ đã thỏa thuận trước.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Phí dịch vụ sẽ được tính dựa trên giá trị giao dịch, bao gồm cả chi phí giao hàng, xử lý và quản lý.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                // Điều 6: Giải quyết tranh chấp
                gfx.DrawString("Điều 6: Giải quyết tranh chấp", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Mọi tranh chấp phát sinh trong quá trình hợp tác sẽ được giải quyết thông qua hòa giải, nếu không thành công sẽ đưa", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• ra tòa án có thẩm quyền", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();
                gfx.DrawString("Điều 7: Quyền và nghĩa vụ bảo mật", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Hệ thống cam kết bảo mật tất cả các thông tin giao dịch và dữ liệu của đối tác và khách hàng.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Các bên đối tác cam kết không tiết lộ thông tin hoặc dữ liệu nhạy cảm liên quan đến hệ thống và các đối tác khác.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();
                // Điều 8: Chấm dứt hợp đồng
                
                if (yPoint >= pageHeightLimit) CreateNewPage();
                gfx.DrawString("Điều 8: Điều khoản chấm dứt hợp đồng", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Hợp đồng có thể được chấm dứt trong các trường hợp sau:", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("  - Một bên không thực hiện đúng nghĩa vụ trong hợp đồng.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("  - Hai bên đồng thuận chấm dứt hợp đồng.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Các bên có nghĩa vụ thông báo trước ít nhất 30 ngày khi có ý định chấm dứt hợp đồng.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();
                // Phần ký tên
                gfx.DrawString("ĐẠI DIỆN BÊN A                                          ĐẠI DIỆN BÊN B", titleFont, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("(Ký tên)                                                                               (Ký tên)", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"Đỗ Hữu Thuận                                                                               {user.FullName}", font, XBrushes.Red, new XPoint(margin, yPoint));

                // Lưu tài liệu PDF
                // Saving the PDF document
                pdfDoc.Save(pdfPath);
            }

            using (var pdfStream = new FileStream(pdfPath, FileMode.Open))
            {
                string fileUrl = await _firebaseStorageService.UploadContractPdfAsync(pdfStream, fileName);
                return fileUrl;
            }
        }

    }

}