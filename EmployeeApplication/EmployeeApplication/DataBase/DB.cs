using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplication
{
    internal class DB
    {
        string connectionString = "Host=localhost;Port=5432;Database=db_medical_institutions;Username=postgres;Password=PostgreSQL";

        public void GetUserAuthorization(string login, string password)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select authorization_employee(@Login, @Password);";

                int id = connection.QueryFirstOrDefault<int>(sql, new
                {
                    Login = login,
                    Password = password
                });

                if (id != -1)
                {
                    connection.Open();

                    sql = @"select ue.role as role, w.type as wingType, p.is_operation as isOperation,
                        concat(ind.last_name, ' ', ind.first_name, coalesce(' ' || ind.middle_name, '')) as fio, emp.id as employeeId, 
					    p.name as postName from posts p inner join employees emp
                        on p.id = emp.post_id
                        inner join individuals ind
                        on ind.id = emp.individual_id
                        inner join employee_assignments ea
                        on emp.id = ea.employee_id
                        inner join user_employees ue
                        on ea.id = ue.employee_assignment_id
                        inner join cabinets c
                        on c.id = ea.cabinet_id
					    inner join departments d
					    on d.id = c.department_id
					    inner join wings w
					    on w.id = d.wing_id
                        where ea.id = @UserId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserAuthorization.id = id;
                                UserAuthorization.role = reader.GetString(0);
                                UserAuthorization.wingType = reader.GetString(1);
                                UserAuthorization.isOperation = reader.GetBoolean(2);
                                UserAuthorization.fio = reader.GetString(3);
                                UserAuthorization.employeeId = reader.GetInt32(4);
                                UserAuthorization.postName = reader.GetString(5);
                            }
                        }
                    }

                    connection.Close();
                }
                else
                {
                    UserAuthorization.id = id;
                    UserAuthorization.role = "";
                }




            }

        }

        public List<Patient> GetPatients()
        {
            using(var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select * from patients_view;";

                return connection.Query<Patient>(sql).ToList();
            }
        }

        public List<Wing> GetPoliclinicWings()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select w.id as id, w.name as name, w.address_city as city, w.address_street as street, w.address_home as home, m.id as medicalInstitutionId,
                                m.name as medicalInstitution, w.type as type, w.type_individual as typeIndividual from medical_institutions m inner join wings w
                                on m.id = w.medical_institution_id where w.type = 'Амбулатория'";

                return connection.Query<Wing>(sql).ToList();
            }
        }

        public List<Wing> GetWings()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select w.id as id, w.name as name, w.address_city as city, w.address_street as street, w.address_home as home, m.id as medicalInstitutionId,
                                m.name as medicalInstitution, w.type as type, w.type_individual as typeIndividual from medical_institutions m inner join wings w
                                on m.id = w.medical_institution_id";

                return connection.Query<Wing>(sql).ToList();
            }
        }

        public List<Employee> GetEmployees()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select * from employees_view;";

                return connection.Query<Employee>(sql).ToList();
            }
        }

        public List<EmployeeAssignment> GetEmployeeAssignments()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select * from employee_assignments_view";

                return connection.Query<EmployeeAssignment>(sql).ToList();
            }
        }

        public List<User> GetUsers()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select * from users_view;";

                return connection.Query<User>(sql).ToList();
            }
        }
    }
}
