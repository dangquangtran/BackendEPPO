using BusinessObjects.Models;
using DTOs.Contracts;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Repository.Interfaces;
using Service.Interfaces;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Service
{
    public class ContractService: IContractService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Contract>> GetListContract(int page, int size)
        {
            return await _unitOfWork.ContractRepository.GetAsync(includeProperties: "User", pageIndex: page, pageSize: size);
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
            string pdfUrl = await GenerateContractPdfAsync(contract);
            contract.ContractUrl = pdfUrl;

            var entity = new Contract
            {
                UserId = contract.UserId,
                ContractNumber = contract.ContractNumber,
                Description = contract.Description,
                CreationContractDate = contract.CreationContractDate,
                EndContractDate = contract.EndContractDate,
                TotalAmount = contract.TotalAmount,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt,
                TypeContract = contract.TypeContract,
                ContractUrl = contract.ContractUrl,
                IsActive = contract.IsActive,
                Status = 1,
            };
            _unitOfWork.ContractRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }


        private async Task<string> GenerateContractPdfAsync(CreateContractDTO contract)
        {
            string fileName = $"Contract_{contract.ContractNumber}_{DateTime.Now.Ticks}.pdf";
            string pdfPath = Path.Combine("wwwroot/contracts", fileName);
      
            Directory.CreateDirectory(Path.GetDirectoryName(pdfPath));

            using (PdfDocument pdfDoc = new PdfDocument())
            {
                pdfDoc.Info.Title = $"Contract {contract.ContractNumber}";

                PdfPage page = pdfDoc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
             //   XFont font = new XFont("Arial", 12, XFontStyle.Regular);
                XFont font = new XFont("Arial", 12);

                gfx.DrawString($"Contract Number: {contract.ContractNumber}", font, XBrushes.Black, new XPoint(40, 50));
                gfx.DrawString($"User ID: {contract.UserId}", font, XBrushes.Black, new XPoint(40, 80));
                gfx.DrawString($"Description: {contract.Description}", font, XBrushes.Black, new XPoint(40, 110));
                gfx.DrawString($"Start Date: {contract.CreationContractDate:yyyy-MM-dd}", font, XBrushes.Black, new XPoint(40, 140));
                gfx.DrawString($"End Date: {contract.EndContractDate:yyyy-MM-dd}", font, XBrushes.Black, new XPoint(40, 170));
                gfx.DrawString($"Total Amount: {contract.TotalAmount:C}", font, XBrushes.Black, new XPoint(40, 200));
                gfx.DrawString($"Type: {contract.TypeContract}", font, XBrushes.Black, new XPoint(40, 230));
              

                pdfDoc.Save(pdfPath);
            }

            return $"/contracts/{fileName}"; 
        }
    }
    
}
