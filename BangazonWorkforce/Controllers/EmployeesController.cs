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
using Remotion.Linq.Clauses;

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
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(connectionString);
            }
        }

        // GET: Employees
        //================= AUTHOR: DANIEL BREWER ======================
        // First Name, Last Name, Department Name

        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"   SELECT e.Id, e.departmentId,
          e.FirstName, e.LastName, d.name AS DepartmentName
          FROM Employee e LEFT JOIN  Department d on d.Id = e.DepartmentId;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")), 
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                            }
                        };

                        employees.Add(employee);
                    }

                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        
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

      
        //+++++++++++++ AUTHOR: JD Wheeler +++++++++++++++
        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            Employee employee = GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }


            var tempTP = GetEmpTPbyId(id);

            //This piece of code will set the values needed for edit. It calls on the methods written at the bottom of this controller.
            EmployeeEditViewModel viewModel = new EmployeeEditViewModel
            {
                Departments = GetAllDepartments(),
                TrainingPrograms = GetAllTrainingPrograms(),
                CurrentEmpTP = GetEmpTPbyId(id),
                SelectedTPs = tempTP.Select(tp => tp.Id).ToList(),
                Computers = GetAllComputers(),
                Employee = employee
            };

            return View(viewModel);
        }
        //++++++++++++++++++End of JD' Code+++++++++++++++++++++++

        //+++++++++++++ AUTHOR: JD Wheeler +++++++++++++++
        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel viewModel)
        {


            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE employee 
                                            SET    firstname = @firstname, 
                                                   lastname = @lastname,
                                                   isSupervisor = @isSupervisor, 
                                                   departmentId = @departmentId
                                            WHERE  id = @id;

                                            UPDATE computerEmployee
                                            SET    computerId = @computerId
                                            WHERE  employeeId = @id;

                                            DELETE FROM EmployeeTraining
                                            WHERE employeeId = @id;";


                    cmd.Parameters.Add(new SqlParameter("@computerId", viewModel.SelectedCE));
                    cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@isSupervisor", viewModel.Employee.IsSupervisor));
                    cmd.Parameters.Add(new SqlParameter("@departmentId", viewModel.Employee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();


                    cmd.CommandText = @"INSERT INTO employeeTraining (employeeId, trainingProgramId)
                                            VALUES ( @id, @trainingId );";

                    foreach (var tpID in viewModel.SelectedTPs)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@trainingId", tpID));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                    }


                    return RedirectToAction(nameof(Index));
                }
            }
        }
        //++++++++++++++++++End of JD' Code+++++++++++++++++++++++

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
        //++++++++++++++++++End of JD' Code+++++++++++++++++++++++

        //+++++++++++++ AUTHOR: JD Wheeler +++++++++++++++
        // JD created to grab individual items for editing. The edit requires ability to edit name, computer and training programs.
        private List<TrainingProgram> GetEmpTPbyId(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.id,
                                               tp.[name],
                                               tp.startDate    

                                               FROM trainingProgram tp 
                                               LEFT JOIN employeeTraining et ON tp.id = et.id
                                               LEFT JOIN employee e ON et.id = e.id 
                                               
                                               WHERE  e.Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> CurrentEmpTP = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        CurrentEmpTP.Add (new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                         
                        });


                    }

                    reader.Close();

                    return (CurrentEmpTP);

                }
            }
        }
        //++++++++++++++++++End of JD' Code+++++++++++++++++++++++

        //+++++++++++++ AUTHOR: JD Wheeler +++++++++++++++
        private Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.id, 
                                               e.firstname, 
                                               e.lastname,
                                               e.issupervisor,
                                               e.departmentid,
											   c.make,
											   c.manufacturer,
                                               tp.[name],
                                               tp.startDate,
                                               c.id AS computerId,
                                               tp.id AS trainingProgramId,
                                               d.[name] AS departmentname
                                        FROM Employee e INNER JOIN Department d ON e.departmentid = d.id
                                                        LEFT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id
														LEFT JOIN Computer c ON c.Id = ce.ComputerId
                                                        LEFT JOIN EmployeeTraining et ON e.Id = et.EmployeeId
                                                        LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                         WHERE  e.Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    while (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("departmentid")),
                                Name = reader.GetString(reader.GetOrdinal("departmentname")),
                            }
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("computerId")))
                        {
                            employee.Computer.Id = reader.GetInt32(reader.GetOrdinal("computerId"));
                            employee.Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Make = reader.GetString(reader.GetOrdinal("make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("manufacturer"))
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("trainingProgramId")))
                        {
                            employee.TrainingProgram.Id = reader.GetInt32(reader.GetOrdinal("trainingProgramId"));
                            employee.TrainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("startDate"))
                            };
                        }

                    }

                    reader.Close();

                    return (employee);

                }
            }
        }
        //++++++++++++++++++End of JD' Code+++++++++++++++++++++++

        //+++++++++++++ AUTHOR: JD Wheeler +++++++++++++++
        // JD - Wrote this for grabbing all instances of departments for employee views.
        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, name from Department;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();

                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("name"))
                        });
                    }

                    reader.Close();

                    return departments;
                }
            }
        }
        //++++++++++++++++++End of JD' Code+++++++++++++++++++++++

        //+++++++++++++ AUTHOR: JD Wheeler +++++++++++++++
        private List<TrainingProgram> GetAllTrainingPrograms()
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT id, name from TrainingProgram;";
                        SqlDataReader reader = cmd.ExecuteReader();

                        List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                        while (reader.Read())
                        {
                            trainingPrograms.Add(new TrainingProgram()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            });
                        }

                        reader.Close();

                        return trainingPrograms;
                    }
                }

            }
        //++++++++++++++++++End of JD' Code+++++++++++++++++++++++



        //+++++++++++++ AUTHOR: JD Wheeler +++++++++++++++
        private List<Computer> GetAllComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, make from Computer;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {
                        computers.Add(new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("make"))
                        });
                    }

                    reader.Close();

                    return computers;
                }
            }

        }
        //++++++++++++++++++End of JD' Code+++++++++++++++++++++++
    }
}

