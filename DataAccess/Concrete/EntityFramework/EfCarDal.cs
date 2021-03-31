using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfCarDal : EfEntityRepositoryBase<Car,RentACarContext>,ICarDal //Bu bir EntityFramework
    {
        public CarDetailDto GetCarDetail(int carId)
        {
            using (RentACarContext context = new RentACarContext())
            { 
                var result = from c in context.Cars.Where(p => p.CarId == carId)
                             join b in context.Brands
                                 on c.BrandId equals b.BrandId
                             join d in context.Colors
                                 on c.ColorId equals d.ColorId
                             select new CarDetailDto()
                             {
                                 CarId = c.CarId,
                                 Description = c.Description,
                                 BrandName = b.BrandName,
                                 ColorName = d.ColorName,
                                 DailyPrice = c.DailyPrice,
                                 ImagePath = context.CarImages.Where(p => p.CarId == c.CarId).Select(p => p.ImagePath).ToList()
                             };

                return result.SingleOrDefault();
            }
        }

        public List<CarDetailDto> GetCarDetails()
        {
            using (RentACarContext context = new RentACarContext())
            {
                var result = from c in context.Cars 
                             join b in context.Brands 
                                 on c.BrandId equals b.BrandId
                             join d in context.Colors 
                                 on c.ColorId equals d.ColorId
                             select new CarDetailDto() 
                             {
                                 CarId = c.CarId,
                                 Description = c.Description,
                                 BrandName = b.BrandName,
                                 ColorName = d.ColorName,
                                 DailyPrice = c.DailyPrice,
                                 ImagePath = context.CarImages.Where(p => p.CarId == c.CarId).Select(p => p.ImagePath).ToList()
                             };

                return result.ToList();
            }
        }

    }
}
