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
        // GET: Departments
        //================= AUTHOR: ALLISON COLLINS ======================
        // dept name, dept budget, size of dept (number of employees assigned)
        public ActionResult Index()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                //join tables by department id within employee object to id within department object
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id AS DepartmentId, d.[Name], d.Budget, e.Id AS EmployeeId, e.FirstName, e.LastName
                                      FROM Department d LEFT JOIN Employee e on e.DepartmentId = DepartmentId;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    //dictionary lets us add department id to newly created department
                    Dictionary<int, Department> departments = new Dictionary<int, Department>();

                    while (reader.Read())
                    {
                        int DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));

                        //why does this work?
                        if(!departments.ContainsKey(DepartmentId))
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
                            currentDepartment.EmployeeIdList.Add(
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