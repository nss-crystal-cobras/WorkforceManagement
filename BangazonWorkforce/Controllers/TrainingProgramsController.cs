using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
   
        // GET: TrainingPrograms
        public class TrainingProgramsController : Controller
        {
                private readonly IConfiguration _configuration;

                public TrainingProgramsController(IConfiguration configuration)
                {
                    this._configuration = configuration;
                }

                public SqlConnection Connection
                {
                    get
                    {
                        string connectionString = _configuration.GetConnectionString("DefaultConnection");
                        return new SqlConnection(connectionString);
                    }
                }
                // GET: TrainingPrograms
                public ActionResult Index()
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"SELECT name, startdate,
                                      enddate, maxattendees
                                      FROM TrainingProgram;";
                            SqlDataReader reader = cmd.ExecuteReader();

                            List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                            while (reader.Read())
                            {
                                TrainingProgram trainingProgram = new TrainingProgram
                                {
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                    MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                                };

                                trainingPrograms.Add(trainingProgram);
                            }

                            reader.Close();
                            return View(trainingPrograms);
                        }
                    }
                }
/*
                // GET: TrainingPrograms/Details/5
                public ActionResult Details(int id)
        {
            return View();
        }
*/
        // GET: TrainingPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingPrograms/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}