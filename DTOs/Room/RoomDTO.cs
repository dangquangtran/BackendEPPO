﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Room
{
    public class RoomDTO
    {
        public int RoomId { get; set; }
        public int? PlantId { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public double? RegistrationFee { get; set; }
        public double? PriceStep { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public int? Status { get; set; }
    }
    public class CreateRoomDTO
    {
        public int? PlantId { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public double? RegistrationFee { get; set; }
        public double? PriceStep { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
    }
    public class UpdateRoomDTO
    {
        public int RoomId { get; set; }
        public int? PlantId { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public double? RegistrationFee { get; set; }
        public double? PriceStep { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public int? Status { get; set; }
    }

    public class DeleteRoomDTO
    {
        public int RoomId { get; set; }
    }
    public class UpdateStatusRoomDTO
    {
        public int? Status { get; set; }
    }
}
