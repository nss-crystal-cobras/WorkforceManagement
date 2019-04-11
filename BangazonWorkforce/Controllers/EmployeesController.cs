using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace BangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmployeesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Employees
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id AS EmployeeId,
                                      e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor
                                      FROM Employee e LEFT JOIN  Department d on d.id = e.DepartmentId;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                        };

                        employees.Add(employee);
                    }

                    reader.Close();
                    return View(employees);
                }
            }
        }

        //===== AUTHOR: ALLISON COLLINS =========
        // GET: Employees/Create
        public ActionResult Create()
        {
            EmployeeCreateViewModel viewModel =
                new EmployeeCreateViewModel(_configuration.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeCreateViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee
                    ( FirstName, LastName, IsSupervisor, DepartmentId )
                    VALUES
                    ( @firstName, @lastName, @isSupervisor, @departmentId )";
                        cmd.Parameters.Add(new SqlParameter("@firstName", model.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", model.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", model.Employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", model.Employee.DepartmentId));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                model.Departments = GetAllDepartments();
                return View(model);
            }
        }
        //========== END A.C. CODE ====================

        //=======================================================================================
        //Begin HANNAH Get Details
        //=======================================================================================


        // Ticket Instructions:
        //         1. First name and last name (of Employee)
        //        2. Department
        //        3. Currently assigned computer
        //        4. Training programs they have attended, or plan on attending (access the list of training programs associated with the employee)

        //            //If employee = null, it doesn't exist; create an object for it.
        //            //An employee must have a department they work in (i.e., dept cannot be null).
        //            //An employee must have a computer (i.e., computer !== null).
        //            //An employee does not have to be enrolled in a training program but if they are, the training program type needs to show all past and future training programs for the employee.



        //GET: Employees/Details/{id
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                                 e.Id AS 'Employee Id',
                                                 e.FirstName AS 'Employee First Name',
                                                 e.LastName AS 'Employee Last Name',
                                                    d.Id AS 'Department Id',
                                                 d.[Name] AS 'Department Name',
                                                 ce.AssignDate AS 'Computer Assigned On',
                                                 c.Id AS 'Computer Id',
                                                 c.Make AS 'Computer Make',
                                                 c.Manufacturer AS 'Computer Manufacturer',
                                                 et.Id AS 'Employee Training Id',
                                                 et.TrainingProgramId AS 'Training Program Id', 
                                                 tp.[Name] AS 'Training Program', 
                                                 tp.StartDate AS 'Training Program Start'
                                            FROM Employee AS e 
                                            LEFT JOIN Department d ON d.Id = e.DepartmentId
                                            RIGHT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id 
                                            INNER JOIN Computer c ON c.Id = ce.ComputerId
                                            RIGHT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                                            INNER JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                            WHERE e.Id = @id AND ce.UnAssignDate IS NULL";

                    //NOTE: HMN: This query was tested in SQL and produced, overall, the desired results (based on issue ticket specifications) The List of training programs for employees (past and future) may need to be tweaked to show end date or past date, however.

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Employee employee = null;


                    while (reader.Read())
                    {
                        if (employee == null)
                        {
                            employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("Employee First Name")),
                                LastName = reader.GetString(reader.GetOrdinal("Employee Last Name")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("Department Id")),
                                Department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Department Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Department Name")),
                                },
                                Computer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Computer Id")),
                                    Make = reader.GetString(reader.GetOrdinal("Computer Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Computer Manufacturer"))
                                },
                                TrainingProgramList = new List<TrainingProgram>()
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("Employee Id")))
                        {

                            if (!reader.IsDBNull(reader.GetOrdinal("Employee Training Id")))
                            {
                                if (!employee.TrainingProgramList.Exists(tp => tp.Id == reader.GetInt32(reader.GetOrdinal("Training Program Id"))))
                                {
                                        TrainingProgram trainingProgram = new TrainingProgram
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Training Program Id")),
                                        Name = reader.GetString(reader.GetOrdinal("Training Program")),
                                        StartDate = reader.GetDateTime(reader.GetOrdinal("Training Program Start"))
                                    };
                                    employee.TrainingProgramList.Add(trainingProgram);
                                }
                            }
                        }

                    };

                    reader.Close();
                    return View(employee);
                }
            }
        }

        //=======================================================================================
                                //End HANNAH Get Details
        //=======================================================================================


        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, [Name] from Department;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> Departments = new List<Department>();

                    while (reader.Read())
                    {
                        Departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                    reader.Close();

                    return Departments;
                }
            }
        }


        //==================================================================================================
        /*

                // GET: Employees/Create
                public ActionResult Create()
                {
                    return View();
                }

                // POST: Employees/Create
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

                // GET: Employees/Edit/5
                public ActionResult Edit(int id)
                {
                    return View();
                }

                // POST: Employees/Edit/5
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

                // GET: Employees/Delete/5
                public ActionResult Delete(int id)
                {
                    return View();
                }

                // POST: Employees/Delete/5
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
                */
    }
}
