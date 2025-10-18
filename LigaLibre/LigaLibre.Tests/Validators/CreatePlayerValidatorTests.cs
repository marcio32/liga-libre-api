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
    public class CreatePlayerValidatorTests
    {
        private readonly CreatePlayerValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var dto = new CreatePlayerDto { FirstName = "" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(c => c.FirstName);
        }
        
        [Fact]
        public void Should_Have_Error_When_Age_Is_Too_Young()
        {
            var dto = new CreatePlayerDto { Age = 15 };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(c => c.Age);
        }
        
        [Fact]
        public void Should_Have_Error_When_Position_Is_Invalid()
        {
            var dto = new CreatePlayerDto { Position = "InvalidPosition" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(c => c.Position);
        }
        
        [Fact]
        public void Should_Not_Have_Error_When_Valid()
        {
            var dto = new CreatePlayerDto { 
                FirstName = "Lionel",
                LastName = "Messi",
                Age = 35,
                Position = "Delantero",
                JerseyNumber = 10,
                Height = 1.70m,
                Weight = 72.0m,
                Nationality = "Argentina",
                DateOfBirth = new DateTime(1987, 6, 24),
                ClubId = 1
            };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
