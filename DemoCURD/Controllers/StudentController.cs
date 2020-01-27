using System;
using System.Collections.Generic;
using System.Linq;
using DemoCURD.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using DemoCURD.Data;

namespace DemoCURD.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateView()
        {
            return PartialView("_StudentCreateView");
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);

                bool isSaved = _context.SaveChanges() > 0;

                if (isSaved)
                {
                    return Json(true);
                }

                return Json(false);
            }

            return PartialView("_StudentCreateView", student);
        }

        public IActionResult EditView(int id)
        {
            var student = _context.Students.Find(id);

            return PartialView("_StudentEditView", student);
        }

        [HttpPost]
        public IActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                var previousStudent = _context.Students.Find(student.Id);

                previousStudent.Id = student.Id;
                previousStudent.Name = student.Name;

                _context.Students.Update(student);

                bool isSaved = _context.SaveChanges() > 0;

                if (isSaved)
                {
                    return Json(true);
                }

                return Json(false);
            }

            return PartialView("_StudentEditView", student);
        }

        public IActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var student = _context.Students.Find(id);

                _context.Students.Remove(student);

                bool isSaved = _context.SaveChanges() > 0;

                if (isSaved)
                {
                    return Json(true);
                }

                return Json(false);
            }

            return Json(false);
        }

        public IActionResult LoadStudent()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault().ToLower();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var students = _context.Students.ToList();

            var studentList = new List<Student>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                students = students.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                students = students.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                students = students.Where(x => x.Name.ToLower().Contains(searchValue)).ToList();
            }

            foreach (var item in students)
            {
                studentList.Add(new Student
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }

            //total number of rows count     
            recordsTotal = studentList.Count();

            //Paging     
            var data = studentList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}