﻿using System;
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
        //================= AUTHOR: DANIEL BREWER ======================
        // First Name, Last Name, Department Name

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



        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            Employee employee = GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            //This piece of code keeps the type correct to pass into the edit
            EmployeeEditViewModel viewModel = new EmployeeEditViewModel
            {
                Departments = GetAllDepartments(),
                TrainingPrograms = GetAllTrainingPrograms(),
                Computers = GetAllComputers(),
                Employee = employee
            };

            return View(viewModel);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE employee,  
                                           SET firstname = @firstname, 
                                               lastname = @lastname,
                                               isSupervisor = @isSupervisor, 
                                               departmentId = @departmentId,
                                             WHERE id = @id;

                                            INSERT INTO employeeTraining
                                              VALUES(@id, @trainingProgramId)

                                            INSERT INTO computerEmployee
                                             VALUES ( @id , @computerId);";

                        cmd.Parameters.Add(new SqlParameter("@computerId", viewModel.SelectedCE));
                        cmd.Parameters.Add(new SqlParameter("@trainingProgramId", viewModel.SelectedTP));
                        cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", viewModel.Employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", viewModel.Employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

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
                                                 ce.UnassignDate AS 'Computer Retired On',
                                                 c.Id AS 'Computer Id',
                                                 c.Make AS 'Computer Make',
                                                 c.Manufacturer AS 'Computer Manufacturer',
                                                 et.Id AS 'Employee Training Id',
                                                 et.TrainingProgramId AS 'Training Program Id', 
                                                 tp.[Name] AS 'Training Program', 
                                                 tp.StartDate AS 'Training Program Start'
                                            FROM Employee AS e 
                                            LEFT JOIN Department d ON d.Id = e.DepartmentId
                                            LEFT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id 
                                            LEFT JOIN Computer c ON c.Id = ce.ComputerId
                                            LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                                            LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                            WHERE e.Id = @id AND ce.UnassignDate IS NULL";

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
                                Computer = new Computer(),
                                TrainingProgramList = new List<TrainingProgram>()
                            };
                        }


                        if (!reader.IsDBNull(reader.GetOrdinal("Computer Id")))
                        {
                            employee.Computer.Id = reader.GetInt32(reader.GetOrdinal("Computer Id"));
                            employee.Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Computer Id")),
                                Make = reader.GetString(reader.GetOrdinal("Computer Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Computer Manufacturer"))
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
                        //AssignDate = reader.GetDateTime(reader.GetOrdinal("Computer Assigned On")),
                        //DecommissionDate
                    }

                    reader.Close();
                    return View(employee);

                }
            }
        }

            //=======================================================================================
            //End HANNAH Get Details
            //=======================================================================================



            // JD created to grab individual items for editing. The edit requires ability to edit name, computer and training programs.
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
        }
    }


