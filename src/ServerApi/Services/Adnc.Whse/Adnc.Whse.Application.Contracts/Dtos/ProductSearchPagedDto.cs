﻿using Adnc.Application.Shared.Dtos;

namespace Adnc.Whse.Application.Contracts.Dtos
{
    public class ProductSearchPagedDto : SearchPagedDto
    {
        public string Id { get; set; }

        public string[] Ids { get; set; }

        public int StatusCode { get; set; }
    }
}
