﻿using FluentValidation;
using Adnc.Usr.Application.Contracts.Dtos;

namespace Adnc.Usr.Application.Contracts.DtoValidators
{
    public class MenuUpdationDtoValidator : AbstractValidator<MenuUpdationDto>
    {
        public MenuUpdationDtoValidator()
        {
            Include(new MenuCreationDtoValidator());
        }
    }
}
