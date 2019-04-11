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

        // GET: Employees/Details/{id}
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
	                                        d.[Name] AS 'Department Name',
	                                        ce.AssignDate AS 'Computer Assigned On',
	                                        c.Id AS 'Computer Id',
	                                        c.Make AS 'Computer Make',
	                                        c.Manufacturer AS 'Computer Manufacturer',
	                                        et.TrainingProgramId AS 'Training Program Id', 
	                                        tp.[Name] AS 'Training Program', 
	                                        tp.StartDate AS 'Training Program Start'
                                    FROM Employee AS e 
                                    LEFT JOIN Department d ON d.Id = e.DepartmentId
                                    RIGHT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id 
                                    INNER JOIN Computer c ON c.Id = ce.ComputerId
                                    RIGHT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                                    INNER JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                    WHERE e.Id = @id";

                    //NOTE: HMN: This query was tested in SQL and produced, overall, the desired results (based on issue ticket specifications) The List of training programs for employees (past and future) may need to be tweaked to show end date or past date, however.

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();


                    //If employee = null, it doesn't exist; create an object for it.
                    //An employee must have a department they work in (i.e., dept cannot be null).
                    //An employee does not have to be enrolled in a training program but if they are, the training program type needs to show all past and future training programs for the employee.
                    //An employee must have a computer (i.e., computer !== null).

                    Dictionary<int, Employee> employees = new Dictionary<int, Employee>();

                        while (reader.Read())
                        {
                            int employeeid = reader.GetInt32(reader.GetOrdinal("Id"));

                            if (!employees.ContainsKey(employeeid))
                            {

                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("Employee First Name")),
                                    LastName = reader.GetString(reader.GetOrdinal("Employee Last Name")),
                                    TrainingProgramList = new List<TrainingProgram>(),
                                    Computer = new Computer(),
                                    Department = new Department
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Department Id")),
                                        Name = reader.GetString(reader.GetOrdinal("Department Name")),
                                    },
                                };
                                employees.Add(employeeid, employee);
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("ecId")))
                            {
                                Employee currentemployee = employees[employeeid];
                                currentemployee.Computer = new Computer
                                {
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                };
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("etId")))
                            {
                                Employee currentemployee = employees[employeeid];

                                if (!currentemployee.TrainingProgramList.Any(x => x.Id == reader.GetInt32(reader.GetOrdinal("tpId"))))
                                {
                                    currentemployee.TrainingProgramList.Add(new TrainingProgram
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("tpId")),
                                        Name = reader.GetString(reader.GetOrdinal("tpName")),
                                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                        EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
                                    });
                                }
                            }
                        }

                        reader.Close();

                        List<Employee> employeeDetail = employees.Values.ToList();

                        foreach (Employee e in employeeDetail)
                        {
                            return View(e);
                        }
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