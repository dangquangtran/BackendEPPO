﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Conversation
{
    public class CreateConversationDTO
    {
        public int? UserOne { get; set; }
        public int? UserTwo { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? Status { get; set; }
    }
}
