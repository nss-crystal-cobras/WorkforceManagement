using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _configuration;

        public DepartmentsController(IConfiguration configuration)
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
        //================= AUTHOR: ALLISON COLLINS ======================
        // GET: Departments
        // dept name, dept budget, size of dept (number of employees assigned)
        public ActionResult Index()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                //join tables by department id within employee object to id within department object
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id AS DepartmentId, 
                                        d.[Name], 
                                        d.Budget, 
                                        e.Id AS EmployeeId, 
                                        e.FirstName, 
                                        e.LastName
                                      FROM Department d 
                                      LEFT JOIN Employee e on e.DepartmentId = DepartmentId;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    //dictionary lets us add department id to newly created department
                    Dictionary<int, Department> departments = new Dictionary<int, Department>();

                    while (reader.Read())
                    {
                        int DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));

                        //why does this work? -> won't each department contain an Id no matter what?
                        if (!departments.ContainsKey(DepartmentId))
                        {
                            Department newDepartment = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };

                            departments.Add(DepartmentId, newDepartment);
                        }

                        //logic for if DB doesn't include any employees; execute logic if DB is not null
                        //add employee to EmployeeIdList within Department class
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Department currentDepartment = departments[DepartmentId];
                            currentDepartment.EmployeeList.Add(
                                new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                });
                        }
                    }

                    reader.Close();
                    //return department dictionary, which holds a list
                    return View(departments.Values.ToList());
                }
            }
        }
        // GET: Departments/Details/5
        // header: dept name
        // list of employees
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT d.Id AS DepartmentId, 
                            d.[Name], 
                            e.Id AS EmployeeId, 
                            e.FirstName, 
                            e.LastName
                        FROM Department d 
                        LEFT JOIN Employee e on e.DepartmentId = DepartmentId
                        WHERE DepartmentId = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Department department = null;

                    //have declared object to be null; as long as this is the case, execute following logic
                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            //instance of dept object
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };
                        }

                        //if DB is not null, fetch employee info
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            int employeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));
                            //Any() method determines if a matching element exists in a collection
                            //EmployeeList defined in Department.cs
                            if (!department.EmployeeList.Any(e => e.Id == employeeId))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                };
                                department.EmployeeList.Add(employee);
                            }
                        }
                    }
                    reader.Close();
                    return View(department);
                }
            }
            //========== END A.C. CODE ==============



            // GET: Instructors/Create
            /*
            public ActionResult Create()
            {
                DepartmentCreateViewModel viewModel =
            new DepartmentCreateViewModel(_configuration.GetConnectionString("DefaultConnection"));
                return View(viewModel);
            }

            // POST: Instructors/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create(DepartmentCreateViewModel viewModel)
            {
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"INSERT INTO instructor (firstname, lastname, slackhandle, cohortid)
                                                 VALUES (@firstname, @lastname, @slackhandle, @cohortid)";
                            cmd.Parameters.Add(new SqlParameter("@name", viewModel.Department.Name));
                            cmd.Parameters.Add(new SqlParameter("@budget", viewModel.Department.Budget));

                            cmd.ExecuteNonQuery();

                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                catch
                {
                    return View(viewModel);
                }
            }
            /*
                    // GET: Instructors/Edit/5
                    public ActionResult Edit(int id)
                    {

                        Instructor instructor = GetInstructorById(id);
                        if (instructor == null)
                        {
                            return NotFound();
                        }

                        InstructorEditViewModel viewModel = new InstructorEditViewModel
                        {
                            Cohorts = GetAllCohorts(),
                            Instructor = instructor
                        };

                        return View(viewModel);
                    }

                    // POST: Instructors/Edit/5
                    [HttpPost]
                    [ValidateAntiForgeryToken]
                    public ActionResult Edit(int id, InstructorEditViewModel viewModel)
                    {
                        try
                        {
                            using (SqlConnection conn = Connection)
                            {
                                conn.Open();
                                using (SqlCommand cmd = conn.CreateCommand())
                                {
                                    cmd.CommandText = @"UPDATE instructor 
                                                       SET firstname = @firstname, 
                                                           lastname = @lastname,
                                                           slackhandle = @slackhandle, 
                                                           cohortid = @cohortid
                                                           WHERE id = @id;";
                                    cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Instructor.FirstName));
                                    cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Instructor.LastName));
                                    cmd.Parameters.Add(new SqlParameter("@slackhandle", viewModel.Instructor.SlackHandle));
                                    cmd.Parameters.Add(new SqlParameter("@cohortid", viewModel.Instructor.CohortId));
                                    cmd.Parameters.Add(new SqlParameter("@id", id));

                                    cmd.ExecuteNonQuery();

                                    return RedirectToAction(nameof(Index));
                                }
                            }
                        }
                        catch
                        {
                            viewModel.Cohorts = GetAllCohorts();
                            return View(viewModel);
                        }
                    }

                    // GET: Instructors/Delete/5
                    public ActionResult Delete(int id)
                    {
                        return View();
                    }

                    // POST: Instructors/Delete/5
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

                    private Instructor GetInstructorById(int id)
                    {
                        using (SqlConnection conn = Connection)
                        {
                            conn.Open();
                            using (SqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = @"SELECT i.Id AS InstructorId,
                                                           i.FirstName, i.LastName, 
                                                           i.SlackHandle, i.CohortId,
                                                           c.Name AS CohortName
                                                      FROM Instructor i LEFT JOIN Cohort c on i.cohortid = c.id
                                                     WHERE  i.Id = @id";
                                cmd.Parameters.Add(new SqlParameter("@id", id));
                                SqlDataReader reader = cmd.ExecuteReader();

                                Instructor instructor = null;

                                if (reader.Read())
                                {
                                    instructor = new Instructor
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                        SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                        CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                        Cohort = new Cohort
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                            Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                        }
                                    };
                                }

                                reader.Close();

                                return instructor;
                            }
                        }

                    }

                    private List<Cohort> GetAllCohorts()
                    {
                        using (SqlConnection conn = Connection)
                        {
                            conn.Open();
                            using (SqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = @"SELECT id, name from Cohort;";
                                SqlDataReader reader = cmd.ExecuteReader();

                                List<Cohort> cohorts = new List<Cohort>();

                                while (reader.Read())
                                {
                                    cohorts.Add(new Cohort
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                        Name = reader.GetString(reader.GetOrdinal("name"))
                                    });
                                }
                                reader.Close();

                                return cohorts;
                            }
                        }

                    }

                }
               */
        }
        //========== END A.C. CODE ==============


//================================= AUTHOR: DANIEL BREWER ========================================= 
        // GET: Departments/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Department
                    ([Name], Budget)
                    VALUES
                    (@name, @budget)";
                        cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));
                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {

                return View(department);
            }
        }
        //============================= End of DB Code =======================================
    }
}