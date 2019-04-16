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
                    cmd.CommandText = @"SELECT Id, name, startdate,
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
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
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

                    //make list of past training programs
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

                        //if current date is later than end of training program, add to list of trainingPrograms
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
        // GET: Instructor/Delete/5
        public ActionResult Delete(int id)
        {
            TrainingProgram trainingProgram = GetTrainingProgramById(id);
            if (trainingProgram == null)
            {
                return NotFound();
            }
            else
            {
                return View(trainingProgram);
            }
        }

        // POST: Instructor/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE From EmployeeTraining Where TrainingProgramId = @id;
                                        DELETE FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    return RedirectToAction(nameof(Index));

                }
            }
        }

        private TrainingProgram GetTrainingProgramById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id AS TrainingProgramId,
                                               t.Name, t.StartDate, 
                                               t.EndDate, t.MaxAttendees
                                               FROM TrainingProgram t 
                                               WHERE  Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;

                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                        };
                    }

                    reader.Close();

                    return trainingProgram;
                }
            }

        }

        //============================= End of DB Code =======================================

        //============================= Start HN Code: Ticket #10 ============================
        // Training Programs: Details and Edit

        /*
                 Given user is viewing the list of training programs
                When the user clicks on a training program
                Then the user should see all details of that training program
                And any employees that are currently attending the program --> For this, you will need to access TrainingPrograms via EmployeeTraining

                Given user is viewing the details of a training program
                When the user clicks on the edit link
                Then the user should be presented with a form that allows the user to edit any property of the training program unless the training program has already taken place
         */
        // NOTE: Get Individual Employees' Training Programs (via EmployeeTraining join table)

        // Get EmployeeTraining + TrainingProgram + Employee
        // Join Employee to TrainingProgram on  EmployeeTraining


        //--------------------------------------------------------------------------------------------------------------------
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                            et.TrainingProgramId AS 'JoinTrainingProgramId',
                                            tp.Id AS 'Training Program Id',
                                            tp.[Name] AS 'Training Program Name',
                                            tp.StartDate AS 'Training Program Start',
                                            tp.EndDate AS 'Training Program End',
                                            tp.MaxAttendees AS 'Max Attendees',
                                            e.Id AS 'Employee Id', 
                                            e.FirstName AS 'Employee First Name',
                                            e.LastName AS 'Employee Last Name',
                                            et.Id AS 'Employee-Training-Id',
                                            et.EmployeeId AS 'JoinEmployeeId'
                                        FROM EmployeeTraining et
                                        LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                        LEFT JOIN Employee e ON e.Id = et.EmployeeId
                                        WHERE tp.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram singleTrainingProgram = null;

                    while (reader.Read())
                    {
                        if (singleTrainingProgram == null)
                        {
                            singleTrainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Training Program Id")),
                                Name = reader.GetString(reader.GetOrdinal("Training Program Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("Training Program Start")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("Training Program End")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("Max Attendees")),
                                Employees = new List<Employee>()    //Employees is the name of the List<Employee> prop in the TrainingProgram model
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("Employee Id")))
                        {
                            int employeeId = reader.GetInt32(reader.GetOrdinal("Employee Id"));

                            if (!singleTrainingProgram.Employees.Any(e => e.Id == employeeId))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("Employee First Name")),
                                    LastName = reader.GetString(reader.GetOrdinal("Employee Last Name"))
                                };

                                singleTrainingProgram.Employees.Add(employee);
                            }
                        }
                    }
                    reader.Close();
                    return View(singleTrainingProgram);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------

        public ActionResult Edit(int id)
        {
            TrainingProgram trainingProgram = GetTrainingProgramByIdToEdit(id);
            if (trainingProgram == null)
            {
                return NotFound();
            }

            return View(trainingProgram);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TrainingProgram singleTrainingProgram)
        {
            //try
            //{
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE TrainingProgram tp
                                            SET tp.[Name] = @Name,
                                                tp.StartDate = @StartDate,
                                                tp.EndDate = @EndDate,
                                                tp.MaxAttendees = @MaxAttendees
                                            WHERE tp.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@Name", singleTrainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", singleTrainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", singleTrainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", singleTrainingProgram.MaxAttendees));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
    //    }
    //        catch
    //        {
    //            return View(trainingProgram);
    //}
}



        // ============================= End HN Code ============================================

        //=============================== Helper Functions =======================================


        // NOTE: This grabs ALL the Training Programs with ALL OF their respective data from the database:
        //private List<TrainingProgram> GetAllTrainingProgramsById(int id)
        private TrainingProgram GetTrainingProgramByIdToEdit(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                            tp.Id AS 'Training Program Id'
                                            tp.[Name] AS 'Training Program Name', 
                                            tp.StartDate AS 'Training Program Start',
                                            tp.EndDate AS 'Training Program End', 
                                            tp.MaxAttendees AS 'Max Attendees'
                                      FROM TrainingProgram tp
                                      WHERE tp.Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    //List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    TrainingProgram singleTrainingProgram = null;

                    if (reader.Read())
                    {
                        //trainingProgram.Add(new TrainingProgram

                        singleTrainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Training Program Id")),
                            Name = reader.GetString(reader.GetOrdinal("Training Program Name ")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("Training Program Start")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("Training Program End")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("Max Attendees"))
                        };
                    }
                    reader.Close();
                    return singleTrainingProgram;
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------

        //HN: This query will provide an Employee and the list of training programs he/she is registered for.
        /*
        @"SELECT
            e.Id AS 'Employee Id', 
            e.FirstName AS 'Employee First Name',
            e.LastName AS 'Employee Last Name',
            tp.Id AS 'Training Program Id',
            tp.[Name] AS 'Training Program Name',
            tp.StartDate AS 'Training Program Start',
            tp.EndDate AS 'Training Program End',
            tp.MaxAttendees AS 'Max Attendees',
            et.Id AS 'Employee-Training-Id',
            et.TrainingProgramId AS 'TrainingProgramId',
            et.EmployeeId AS 'EmployeeId'
        FROM Employee e
        LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
        LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
        WHERE e.Id = @id";
        */

        //====================================== End Helper Functions ======================================
    }
}