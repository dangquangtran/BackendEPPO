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
    public class ContractService: IContractService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;
        public ContractService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        private readonly FirebaseStorageService _firebaseStorageService;

        private string fileName = null;

        public ContractService(IUnitOfWork unitOfWork, FirebaseStorageService firebaseStorageService)
        {
            _unitOfWork = unitOfWork;
            _firebaseStorageService = firebaseStorageService;

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
            contract.UserId = contract.UserId;
            contract.ContractNumber = contract.ContractNumber;
            contract.Description = contract.Description;
            contract.CreationContractDate = contract.CreationContractDate;
            contract.EndContractDate = contract.EndContractDate;
            contract.TotalAmount = contract.TotalAmount;
            contract.CreatedAt = contract.CreatedAt;
            contract.UpdatedAt = contract.UpdatedAt;
            contract.TypeContract = contract.TypeContract;
            contract.ContractUrl = contract.ContractUrl;
            contract.IsActive = contract.IsActive;
            contract.Status = contract.Status;

            _unitOfWork.ContractRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task CreateContract(CreateContractDTO contract)
        {

            var entity = new Contract
            {
                UserId = contract.UserId,
                ContractNumber = contract.ContractNumber,
                Description = contract.Description,
                CreationContractDate = contract.CreationContractDate,
                EndContractDate = contract.EndContractDate,
                TotalAmount = contract.TotalAmount,
                CreatedAt = DateTime.Now,
                UpdatedAt = contract.UpdatedAt,
                TypeContract = contract.TypeContract,
                ContractUrl = contract.ContractUrl,
                IsActive = 1,
                Status =  1,  
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
                        IsActive =  true,  // Gán giá trị mặc định IsActive nếu null
                        Status =  1,  // Gán mặc định Status nếu null
                    };

                    // Lưu mỗi ContractDetail vào cơ sở dữ liệu
                    _unitOfWork.ContractDetailRepository.Insert(contractDetailEntity);
                }
                await _unitOfWork.SaveAsync();
            }





            string pdfUrl = await GenerateContractPdfAsync(contract);
            entity.ContractUrl = pdfUrl;
            entity.ContractFileName = fileName;

            _unitOfWork.ContractRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }


        public async Task<string> GenerateContractPdfAsync(CreateContractDTO contract)
        {
             //await Task.FromResult(_unitOfWork.UserRepository.GetByID(contract.UserId));



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
                gfx.DrawString($"Căn cứ vào:", titleFont, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Bộ luật Dân sự nước Cộng hòa Xã hội Chủ nghĩa Việt Nam năm 2015.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString("• Các quy định pháp luật hiện hành liên quan đến hợp đồng thuê tài sản.", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Thông tin bên A và bên B
                gfx.DrawString($"Hôm nay, ngày {contract.CreationContractDate?.ToString("dd")} tháng {contract.CreationContractDate?.ToString("MM")} năm {contract.CreationContractDate?.ToString("yyyy")}, tại {contract.UserId}", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Viết nội dung vào PDF
                gfx.DrawString("BÊN CHO THUÊ (Bên A):", titleFont, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Họ và tên: Ông Đỗ Hữu Thuận", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Địa chỉ: Quận 9, Thành phố Hồ Chí Minh", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Số điện thoại: 0333888257", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Chứng minh nhân dân/Căn cước công dân: 01213123132", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Email: EPPO.HCM@gmail.com", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;

                // Thông tin Bên B
                gfx.DrawString("BÊN THUÊ (Bên B):", titleFont, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Họ và tên: {contract.UserId}", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Địa chỉ: {contract.UserId}", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Số điện thoại: {contract.UserId}", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Chứng minh nhân dân/Căn cước công dân: {contract.UserId}", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"• Email: {contract.UserId}", font, XBrushes.Red, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                if (yPoint >= pageHeightLimit) CreateNewPage();

              
                    // Điều khoản hợp đồng
                    gfx.DrawString("Cùng nhau thỏa thuận ký kết hợp đồng thuê cây với các điều khoản như sau:", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;

                    // Điều 1: Đối tượng hợp đồng
                    gfx.DrawString("Điều 1: Đối Tượng Hợp Đồng", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"1. Mô tả cây {contract.UserId} cho thuê:", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"   • Số lượng: {contract.UserId}", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"   • Mô tả chi tiết: {contract.UserId}", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"   • Giá trị của cây (ước tính): {contract.UserId} VNĐ", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;


                    // Điều 2: Thời gian thuê
                    gfx.DrawString("Điều 2: Thời gian thuê", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"• Thời gian thuê cây mai bắt đầu từ ngày {contract.UserId:yyyy-MM-dd} đến ngày {contract.UserId:yyyy-MM-dd}.", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"• Bên B có quyền gia hạn hợp đồng thuê nếu có thỏa thuận và sự đồng ý của Bên A.", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;

                    // Điều 3: Giá thuê và phương thức thanh toán
                    gfx.DrawString("Điều 3: Giá thuê và phương thức thanh toán", titleFont, XBrushes.Red, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"1. Giá thuê: {contract.UserId:C}/tháng", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"• Tổng giá trị hợp đồng thuê trong {contract.UserId} tháng là {contract.UserId:C}.", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"2. Phương thức thanh toán:", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"• Bên B thanh toán qua chuyển khoản vào tài khoản của Bên A:", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"  - Số tài khoản: {contract.UserId}", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"  - Ngân hàng: {contract.UserId}", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"  - Chủ tài khoản: {contract.UserId}", font, XBrushes.Black, new XPoint(margin, yPoint));
                    yPoint += lineHeight;
                    gfx.DrawString($"• Thanh toán sẽ được thực hiện vào ngày {contract.UserId} hàng tháng.", font, XBrushes.Black, new XPoint(margin, yPoint));
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
                gfx.DrawString("(Ký tên)                                                                               (Ký tên)", font, XBrushes.Black, new XPoint(margin, yPoint));
                yPoint += lineHeight;
                gfx.DrawString($"Đỗ Hữu Thuận                                                                               {contract.UserId}", font, XBrushes.Red, new XPoint(margin, yPoint));

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

    }

}
