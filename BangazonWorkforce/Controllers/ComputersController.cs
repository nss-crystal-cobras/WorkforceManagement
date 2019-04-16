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



namespace BangazonWorkforce.Controllers
{

    public class ComputersController : Controller
    {
        private readonly IConfiguration _configuration;

        public ComputersController(IConfiguration configuration)
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



        // GET: Computers
        public ActionResult Index()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, purchaseDate,
                                        make, manufacturer
                                        FROM Computer  ;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("manufacturer"))

                        };

                        computers.Add(computer);
                    }

                    reader.Close();
                    return View(computers);
                }
            }
        }

        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, Make, Manufacturer, PurchaseDate 
                                        FROM computer
                                        WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;
                    while (reader.Read())
                    {
                        if (computer == null)
                        {
                            computer = new Computer()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Make = reader.GetString(reader.GetOrdinal("make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("manufacturer")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("purchaseDate"))

                            };
                        
                        }
                    }
                reader.Close();
                return View(computer);
                }
            }

        }
    



// GET: Computers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO computer (Make, Manufacturer, PurchaseDate)
                                            VALUES (@make, @manufacturer, @purchasedate)";

                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@purchasedate", computer.PurchaseDate));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {

                return View(computer);
            }
        }

        // GET: Computers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computers/Edit/5
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

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {

            ComputerDeleteViewModel viewModel = new ComputerDeleteViewModel
            {
                Computer = GetComputerById(id)
            };

            return View(viewModel);
        }

        // POST: Computers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ComputerDeleteViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                     cmd.CommandText = @"DELETE FROM Computer
                                        WHERE Id = @id
                                        AND NOT EXISTS (SELECT * FROM [ComputerEmployee]
                                        WHERE ComputerId = @id)";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(viewModel);
            }
        }




        private Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                        c.Id,
                                        c.Make,
                                        c.Manufacturer,
                                        c.PurchaseDate,
										e.Id AS EmployeeId,
										e.FirstName,
                                        e.LastName
                                        FROM Computer c
                                        LEFT JOIN (SELECT * 
										FROM ComputerEmployee
										WHERE UnassignDate IS NULL)
										ce ON c.Id = ce.ComputerId
                                        LEFT JOIN Employee e ON ce.EmployeeId = e.Id
                                        WHERE c.Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Computer computer = null;

                    while (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Employee = new Employee()
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            computer.Employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                        };
                    }
                    reader.Close();
                    return computer;
                }
            }
        }





    }
}