using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.User
{
    public class ResponseUserDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public string? IdentificationCard { get; set; }
        public int? WalletId { get; set; }
        public int? RoleId { get; set; }
        public string? RankLevel { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsSigned { get; set; }
        public bool? IsUpdated { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public int? Status { get; set; }
    }
    public class CreateAccountByCustomerDTO
    {
        public string? UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AddressDescription { get; set; }
        //public string? ImageUrl { get; set; }
        public string? IdentificationCard { get; set; }
        //public int? WalletId { get; set; }
        //public int? RoleId { get; set; }
        //public string? RankLevel { get; set; }
        //public bool? IsActive { get; set; }
        //public DateTime? CreationDate { get; set; }
        //public int? CreationBy { get; set; }
        //public DateTime? ModificationDate { get; set; }
        //public int? ModificationBy { get; set; }
        //public int? Status { get; set; }
    }
    public class CreateAccountByOwnerDTO
    {
        public string? UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AddressDescription { get; set; }
        //public string? ImageUrl { get; set; }
        public string? IdentificationCard { get; set; }
        //public int? WalletId { get; set; }
        //public int? RoleId { get; set; }
        //public string? RankLevel { get; set; }
        //public bool? IsActive { get; set; }
        //public DateTime? CreationDate { get; set; }
        //public int? CreationBy { get; set; }
        //public DateTime? ModificationDate { get; set; }
        //public int? ModificationBy { get; set; }
        //public int? Status { get; set; }
    }
    public class CreateAccountByAdminDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public string? IdentificationCard { get; set; }
        public int? WalletId { get; set; }
        public int? RoleId { get; set; }
        public string RankLevel { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public int? Status { get; set; }
    }

    public class UpdateAccount
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthInput { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? ImageUrl { get; set; }
        public string? IdentificationCard { get; set; }
        public int? WalletId { get; set; }
        public string? RankLevel { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public int? Status { get; set; }
        public bool? IsUpdated { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
    public class ChangePassword
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }
    public class ChangeStatus
    {
        public int? Status { get; set; }
    }
    public class UpdateInformation
    {
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        //public string DateOfBirthInput { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? ImageUrl { get; set; }
        public string? IdentificationCard { get; set; }
        public int? ModificationBy { get; set; }
        public int? WalletId { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationBy { get; set; }
        public string? RankLevel { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool? IsSigned { get; set; }//dang ki ve hop dong
        public bool? IsUpdated { get; set; }// check coi no lan dau login
    }
    public class UpdateRankVler
    {
        public int UserId { get; set; }
        public string? RankLevel { get; set; }
    }
}
