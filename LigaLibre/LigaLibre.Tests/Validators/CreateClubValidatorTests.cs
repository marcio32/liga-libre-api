using FluentValidation.TestHelper;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigaLibre.Tests.Validators
{
    public class CreateClubValidatorTests
    {
        private readonly CreateClubValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var dto = new CreateClubDto { Name = "" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(c => c.Name);
        }
       
        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var dto = new CreateClubDto { Email = "invalid-email" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public void Should_Have_Error_When_NumberOfPartners_Is_Zero()
        {
            var dto = new CreateClubDto { NumberOfPartners = 0 };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(c => c.NumberOfPartners);
        }
        
        [Fact]
        public void SHould_Not_Have_Error_When_Valid()
        {
            var dto = new CreateClubDto { 
                Name = "Boca Juniors",
                City = "Buenos Aires",
                Email = "infor@boca.com",
                StadiumName = "La Bombonera",
                NumberOfPartners = 500000

            };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
