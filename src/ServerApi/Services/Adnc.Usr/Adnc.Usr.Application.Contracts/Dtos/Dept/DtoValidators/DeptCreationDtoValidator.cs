﻿using FluentValidation;
using Adnc.Usr.Application.Contracts.Dtos;

namespace Adnc.Usr.Application.Contracts.DtoValidators
{
    /// <summary>
    /// DeptCreationDto
    /// </summary>
    public class DeptCreationDtoValidator : AbstractValidator<DeptCreationDto>
    {
        /// <summary>
        /// DeptCreationDtoValidator
        /// </summary>
        public DeptCreationDtoValidator()
        {
            RuleFor(x => x.SimpleName).NotEmpty().Length(2, 16);
            RuleFor(x => x.FullName).NotEmpty().Length(2, 32);
            //RuleFor(x => x.Tips).NotEmpty().MaximumLength(64);
            //RuleFor(x => x.Pid).GreaterThan(1).WithMessage("{PropertyName} 不能为空")
            //                                  .NotEqual(x => x.Id).When(x => x.Id > 0);

            RuleFor(x => x.Pid).GreaterThan(1).WithMessage("{PropertyName} 不能为空");
        }
    }
}