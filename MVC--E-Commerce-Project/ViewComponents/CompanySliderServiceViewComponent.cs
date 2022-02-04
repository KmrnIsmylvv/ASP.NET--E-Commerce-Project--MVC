using Microsoft.AspNetCore.Mvc;
using MVC__E_Commerce_Project.DAL;
using MVC__E_Commerce_Project.Models;
using MVC__E_Commerce_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC__E_Commerce_Project.ViewComponents
{
    public class CompanySliderServiceViewComponent:ViewComponent
    {
        private readonly Context _context;

        public CompanySliderServiceViewComponent(Context context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<CompanySlider> companySliders = _context.CompanySliders.ToList();
            List<Service> services = _context.Services.ToList();

            CompanySliderServiceVM companySliderServiceVM = new CompanySliderServiceVM();
            companySliderServiceVM.CompanySliders = companySliders;
            companySliderServiceVM.Services = services;

            return View(await Task.FromResult(companySliderServiceVM));
        }

    }
}
