﻿using Adnc.Application.Shared.Dtos;

namespace Adnc.Whse.Application.Contracts.Dtos
{
    public class ProducChangePriceDto : IDto
    {
        public decimal Price { set; get; }
    }
}
