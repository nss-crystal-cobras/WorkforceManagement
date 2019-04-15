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
                    
                           /* set variable to timestamp here */
                           DateTime CurrentDate = DateTime.Now;

                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                        };

                        /* add if statement to check for enddate */
                        if (CurrentDate < trainingProgram.EndDate)
                        {
                            trainingPrograms.Add(trainingProgram);
                        }
                    }

                            reader.Close();
                            return View(trainingPrograms);
                        }
                    }
                }

        //================================= AUTHOR: DANIEL BREWER ========================================= 
        // GET: Departments/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingProgram/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees)
                                          VALUES (@name, @startdate, @enddate, @maxattendees)";

                        cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@startdate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@enddate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxattendees", trainingProgram.MaxAttendees));
                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {

                return View(trainingProgram);
            }
        }
        //============================= End of DB Code =======================================

        //============================= Start HN Code: Ticket #10 ============================
        // Training Programs: Details and Edit

        /*
                 Given user is viewing the list of training programs
                When the user clicks on a training program
                Then the user should see all details of that training program
                And any employees that are currently attending the program

                Given user is viewing the details of a training program
                When the user clicks on the edit link
                Then the user should be presented with a form that allows the user to edit any property of the training program unless the training program has already taken place
         */




        // ============================= End HN Code ========================================
    }
}