﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CarStore.Data;
using CarStore.Models;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;

namespace CarStore.Pages.CarPages
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly CarStore.Data.CarStoreContext _context;
        private readonly IWebHostEnvironment _env;

        // Objects
        [BindProperty]
        public Car Car { get; set; } = default!;

        [BindProperty]
        public IFormFile ImageUpload { get; set; } = default!;

        // List of car types
        public List<SelectListItem> CarTypeList { get; set; } = new List<SelectListItem>();

        public CreateModel(CarStore.Data.CarStoreContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;

            // Populating the type list 
            List<CarType> carTypes = _context.CarType.ToList();

            foreach (CarType carType in carTypes)
            {
                CarTypeList.Add(new SelectListItem()
                {
                    Value = carType.CarTypeID.ToString(),
                    Text = carType.TypeName
                });
            }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            // Set filename for the photo
            string imageFileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss_") + ImageUpload.FileName;
            Car.ImageName = imageFileName;

            // Get and set TypeID
            int typeID = Car.CarType.CarTypeID;
            CarType carType = _context.CarType.Single(m => m.CarTypeID == typeID);
            Car.CarType = carType;

            // Validate
            if (!ModelState.IsValid || _context.Car == null || Car == null)
            {
                return Page();
            }

            _context.Car.Add(Car);
            await _context.SaveChangesAsync();

            // Save image to filesystem
            string filePath = Path.Combine(_env.ContentRootPath, "wwwroot/photos", imageFileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                ImageUpload.CopyTo(fileStream);
            }

            return RedirectToPage("./CarList");
        }
    }
}
