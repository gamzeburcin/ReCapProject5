using System.Linq;
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfRentalDal : EfEntityRepositoryBase<Rental, RentACarContext>, IRentalDal
    {
        public List<RentalDetailDto> GetRentalDetails()
        {
            using (RentACarContext context = new RentACarContext())
            {
                var result = from rental in context.Rentals
                             join car in context.Cars
                             on rental.CarId equals car.CarId

                             join customer in context.Customers
                             on rental.CustomerId equals customer.UserId

                             join user in context.Users
                             on customer.UserId equals user.Id

                             join brand in context.Brands
                             on car.BrandId equals brand.BrandId

                             join color in context.Colors
                             on car.ColorId equals color.ColorId



                             select new RentalDetailDto
                             {
                                 RentalId = rental.Id,
                                 CarId = car.CarId,
                                 CustomerId = customer.UserId,
                                 Brand = brand.BrandName,
                                 Color = color.ColorName,
                                 FirstName = user.FirstName,
                                 LastName = user.LastName,
                                 CompanyName = customer.CompanyName,
                                 Email = user.Email,
                                 RentDate = rental.RentDate,
                                 

                             };
                return result.ToList();

            }
        }
    }
}
