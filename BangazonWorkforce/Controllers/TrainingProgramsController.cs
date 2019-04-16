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

        //======= AUTHOR: ALLISON COLLINS ============
        // GET: Index of past training programs, accessible from link at bottom of index page of future training programs
        // GET: TrainingPrograms
        public ActionResult PastIndex()
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
                        if (CurrentDate > trainingProgram.EndDate)
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
        // GET: TrainingProgram/Create

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


    }
}