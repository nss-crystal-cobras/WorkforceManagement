using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;

//ALLISON COLLINS - this view model is necessary to make a drop-down menu to select departments on the create form
//hold data for a Razor template that will list all employees and all departments

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeCreateViewModel
    {
        public EmployeeCreateViewModel()
        {
            Departments = new List<Department>();
        }

        public EmployeeCreateViewModel(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, [Name] from Department;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Departments = new List<Department>();

                    while (reader.Read())
                    {
                        Departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            EmployeeList = new List<Employee>()
                        });
                    }
                    reader.Close();
                }
            }
        }


        public Employee Employee { get; set; }
        public List<Department> Departments { get; set; }

        public List<SelectListItem> DepartmentOptions
        {
            get
            {
                return Departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
            }
        }
    }
}
