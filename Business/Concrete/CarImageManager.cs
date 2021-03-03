using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspect.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.FileHelper;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        ICarImageDal _carImageDal;

        public CarImageManager(ICarImageDal carImageDal)
        {
            _carImageDal = carImageDal;
        }
        [ValidationAspect(typeof(CarImageValidator))]
        public IResult Add(CarImage carImage, IFormFile file)
        {
            IResult result = BusinessRules.Run(CheckIfImageLimit(carImage.CarId));

            if (result != null)
            {
                return result;
            }

            carImage.ImagePath = FileHelper.AddAsync(file);
            carImage.Date = DateTime.Now;
            _carImageDal.Add(carImage);

            return new SuccessResult(Messages.CarImageAdded);

        }

        public IResult Delete(CarImage carImage)
        {
            var oldpath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\WebAPI")) + _carImageDal.Get(I => I.Id == carImage.Id).ImagePath;

            var result = BusinessRules.Run(FileHelper.DeleteAsync(oldpath));

            if (result != null)
            {
                return result;
            }

            _carImageDal.Delete(carImage);
            return new SuccessResult(Messages.DeletedCarImage);
        }

        public IDataResult<CarImage> Get(int id)
        {
            return new SuccessDataResult<CarImage>(_carImageDal.Get(ci => ci.Id == id));
        }

        public IDataResult<List<CarImage>> GetAll(Expression<Func<CarImage, bool>> filter = null)
        {
            return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll());
        }

        public IDataResult<List<CarImage>> GetImagesByCarId(int id)
        {
            return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll(ci => ci.CarId == id));
        }

        [ValidationAspect(typeof(CarImageValidator))]
        public IResult Update(CarImage carImage, IFormFile file)
        {
            var oldpath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\WebAPI")) + _carImageDal.Get(ci => ci.Id == carImage.Id).ImagePath;
            carImage.ImagePath = FileHelper.UpdateAsync(oldpath, file);
            carImage.Date = DateTime.Now;
            _carImageDal.Update(carImage);
            return new SuccessResult();
        }

        private IResult CheckIfImageLimit(int carId)
        {
            var result = _carImageDal.GetAll(ci => ci.CarId == carId).Count;
            if (result > 5)
            {
                return new ErrorResult(Messages.FailAddedImageLimit);
            }
            return new SuccessResult();
        }
    }
}
