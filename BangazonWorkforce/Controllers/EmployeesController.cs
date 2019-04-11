using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
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
            this._configuration = configuration;
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

        //==================================================================================================
        //Hannah

        // Ticket Instructions:
//         1. First name and last name (of Employee)
//        2. Department
//        3. Currently assigned computer
//        4. Training programs they have attended, or plan on attending (access the list of training programs associated with the employee)

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                COALESCE(ce.EmployeeId, et.EmployeeId, e.Id) AS            'Employee Id',
	                e.FirstName,
	                e.LastName,
	                d.[Name] AS 'Department Name',
	                ce.AssignDate AS 'Computer Assigned On',
	                ce.ComputerId AS 'Computer Id',
	                et.TrainingProgramId, 
	                tp.[Name] AS 'Training Program', 
	                tp.StartDate AS 'Training Program Start'
                FROM Employee AS e
                FULL OUTER JOIN EmployeeTraining AS et ON e.Id = et.EmployeeId
                INNER JOIN TrainingProgram AS tp ON tp.Id = et.TrainingProgramId
                FULL OUTER JOIN ComputerEmployee AS ce ON e.Id = ce.EmployeeId
                LEFT JOIN Department AS d ON d.Id = e.DepartmentId
                WHERE e.Id = @id";

                    //NOTE: HMN: This query was tested in SQL and produced, overall, the desired results (based on issue ticket specifications) The List of training programs for employees (past and future) may need to be tweaked to show end date or past date, however.






                    return View();
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