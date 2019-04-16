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

                    //key: integer (DepartmentId), value: Department object
                    Dictionary<int, Department> departments = new Dictionary<int, Department>();

                    while (reader.Read())
                    {
                        //reader looks for column name "DepartmentId" in database
                        int DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));

                        //DepartmentId has already been established as the key in the "departments" dictionary
                        //following logic adds new department to dictionary if the dictionary doesn't yet contain the particular DepartmentId
                        if (!departments.ContainsKey(DepartmentId))
                        {
                            Department newDepartment = new Department
                            {
                                //finding columns with following titles
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };

                            //build list of employees along with dept id
                            List<Employee> employees = GetEmployeesByDepartmentId(reader.GetInt32(reader.GetOrdinal("DepartmentId")));

                            //EmployeeList defined in Department model; new department object given list of employees with same Id as long as at least 1 employee exists
                            if(employees.Count() > 0)
                            {
                                newDepartment.EmployeeList = GetEmployeesByDepartmentId(reader.GetInt32(reader.GetOrdinal("DepartmentId")));
                            }

                            //add key (DepartmentId) and value (newDepartment) to departments dictionary
                            departments.Add(DepartmentId, newDepartment);
                        }
                    }

                    reader.Close();
                    //return department dictionary, which holds a list
                    return View(departments.Values.ToList());
                }
            }
        }
        // =================== GET: Departments/Details/5 ================================
        // header: dept name
        // list of employees, budget
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
                            d.Budget,
                            e.Id AS EmployeeId, 
                            e.FirstName, 
                            e.LastName
                        FROM Department d 
                        LEFT JOIN Employee e on e.DepartmentId = d.Id
                        WHERE d.Id = @id";
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
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };
                        }

                        //if DB is not null, fetch employee info
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            int employeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));
                            //Any() method determines if a matching element exists in a collection
                            //EmployeeList defined in Department.cs
                            //looking for existing employee in list; if no instance of current employee is in list, create it here
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
                        } else
                        {
                            return View(department);
                        }
                    }
                    reader.Close();
                    return View(department);
                }
            }
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

        //ALLISON COLLINS - use logic for Dept index
        // method gets employee Id, first and last name, connects DepartmentId in employee with corresponding Id in department
        // returns list of employees
        private List<Employee> GetEmployeesByDepartmentId(int DepartmentId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT e.Id AS EmployeeId,
                            e.FirstName, 
                            e.LastName
                        FROM Employee e
                        LEFT JOIN Department d on e.DepartmentId = d.id
                        WHERE e.DepartmentId = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", DepartmentId));
                    //execute a command that returns rows
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        });
                    }

                    reader.Close();

                    return employees;
                }
            }
        }
    }
}