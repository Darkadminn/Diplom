using Dapper;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using static Xceed.Wpf.Toolkit.Calculator;

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

        public List<Individual> GetIndividuals()
        {

            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select id, last_name as lastName, first_name as firstName, coalesce(middle_name, '') as middleName, coalesce(phone, '') as phone, birthday, 
                                coalesce(snils, '') as snils, coalesce(data_passport->>'series', '') as passportSeries, coalesce(data_passport->>'number', '') as passportNumber, 
                                data_passport->>'issue_date' as passportIssuedDate, coalesce(data_passport->>'issued_by', '') as passportIssuedBy, gender, 
                                coalesce(insurance_policy, '') as insurancePolicy, coalesce(insurance_company, '') as insuranceCompany,
                                birth_certificate as birthCertificate from individuals;";



                return connection.Query<Individual>(sql).ToList();
            }
        }

        public List<Children> GetChildrens()
        {

            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select id, last_name as lastName, first_name as firstName, coalesce(middle_name, '') as middleName, birthday,
                                birth_certificate as birthCertificate from individuals where extract(year from age(current_date, birthday)) < 14;";



                return connection.Query<Children>(sql).ToList();
            }
        }

        public List<Children> GetChildrensIndividual(int id)
        {

            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select ind.id as id, ind.last_name as lastName, ind.first_name as firstName, coalesce(ind.middle_name, '') as middleName, 
                                ind.birthday as birthday, ind.birth_certificate as birthCertificate from individuals ind inner join parent_childrens pc
                                on ind.id = pc.children_id
                                where pc.parent_id = @ID;";



                return connection.Query<Children>(sql, new {ID = id}).ToList();
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

        public List<Department> GetDepartments()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select id, name, wing_id as wingId from departments";

                return connection.Query<Department>(sql).ToList();
            }
        }

        public List<Cabinet> GetCabinets()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select id, name, department_id as departmentId from cabinets";

                return connection.Query<Cabinet>(sql).ToList();
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

        public List<WorkSchedule> GetWorkSchedules(int employeeAssignmentId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                
                string sql = @"select id, time_start as timeStart, time_end as timeEnd, week_days as weekDays
                            from work_schedules
                            where employee_assignment_id = @ID";

                return connection.Query<WorkSchedule>(sql, new { ID = employeeAssignmentId }).ToList();
                

            }
        }

        public List<Visit> GetVisits()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select v.id as id, v.date as date, v.patient_condition->>'diagnosis' as diagnosis, 
                                        v.patient_condition->>'objective' as objective, v.status as status,
                                        v.patient_condition->>'subjective' as subjective, v.recommendation as recommendation, 
                                        v.employee_assignment_id as employeeAssignmentId, v.patient_id as patientId,
                                        concat(ind.last_name, ' ', ind.first_name, coalesce(' ' || ind.middle_name, '')) as patient,
                                        (select count(*) > 0 from hospital_treatments where visit_id = v.id) as isTreatment
                                        from visits v inner join patients p
                                        on p.id = v.patient_id
                                        inner join individuals ind
                                        on ind.id = p.individual_id
                                        where v.employee_assignment_id = @ID";

                return connection.Query<Visit>(sql, new { ID = UserAuthorization.id }).ToList();


            }
        }

        public List<Treatment> GetTreatments()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select t.id as id, t.date_start as dateStart, t.date_end as dateEnd, v.id as visitId, v.patient_id as patientId,
                                        concat(ind.last_name, ' ', ind.first_name, coalesce(' ' || ind.middle_name, '')) as patient, v.date as dateVisit,
                                        v.patient_condition->>'diagnosis' as diagnosis, ind.gender as patientGender
                                        from hospital_treatments t inner join visits v 
                                        on v.id = t.visit_id
                                        inner join patients p
                                        on p.id = v.patient_id
                                        inner join individuals ind
                                        on ind.id = p.individual_id
                                        where v.employee_assignment_id = @ID";

                return connection.Query<Treatment>(sql, new { ID = UserAuthorization.id }).ToList();


            }
        }

        public List<Ward> GetWards(int departmentId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select w.id as id, w.name as name, w.count_bed - (select count(*) from ward_traffics where ward_id = w.id and date_departure is null) as countBed, 
                                w.department_id as departmentId, w.gender as gender from wards w
                                where department_id = @DepartmentId;";

                return connection.Query<Ward>(sql, new { DepartmentId = departmentId}).ToList();


            }
        }

        public List<HospitalResearche> GetResearches(int treatmentId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select id, name, date, result from hospital_treatment_researches 
                                where hospital_treatment_id = @TreatmentId;";

                return connection.Query<HospitalResearche>(sql, new { TreatmentId = treatmentId }).ToList();


            }
        }

        public List<HospitalProcedure> GetHospitalProcedures(int treatmentId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select hp.id as id, hp.date as date, hp.name as name, hp.count as count, hp.description as description, 
                                (select count(*) from hospital_treatment_procedures_histories where hospital_treatment_procedure_id = hp.id) as countСompleted 
                                from hospital_treatment_procedures hp
                                where hp.hospital_treatment_id = @TreatmentId;";

                return connection.Query<HospitalProcedure>(sql, new { TreatmentId = treatmentId }).ToList();


            }
        }

        public List<HospitalProcedureHistory> GetHospitalProcedureHistories(int hospitalProcedureId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select id, date, hospital_treatment_procedure_id as hospitalProcedureId from hospital_treatment_procedures_histories 
                                where hospital_treatment_procedure_id = @HospitalProcedureId;";

                return connection.Query<HospitalProcedureHistory>(sql, new { HospitalProcedureId = hospitalProcedureId }).ToList();


            }
        }

        public List<HospitalProcedureHistory> GetHospitalProcedureHistoriesTreatment(int treatmentId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select ph.id as id, ph.date as date, ph.hospital_treatment_procedure_id as hospitalProcedureId 
                                from hospital_treatment_procedures_histories ph inner join hospital_treatment_procedures p
                                on p.id = ph.hospital_treatment_procedure_id
                                where p.hospital_treatment_id = @TreatmentId;";

                return connection.Query<HospitalProcedureHistory>(sql, new { TreatmentId = treatmentId }).ToList();


            }
        }

        public List<VisitProcedureHistory> GetVisitProcedureHistories(int procedureId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select id, date, visit_procedure_id as visitProcedureId from visit_procedures_histories
                                where visit_procedure_id = @ProcedureId;";

                return connection.Query<VisitProcedureHistory>(sql, new { ProcedureId = procedureId }).ToList();


            }
        }

        public List<WardTraffic> GetWardTraffics(int treatmentId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select d.id as departmentId, d.name as department, w.id as wardId, w.name as ward, wt.date_arrival as dateArrival, 
                                wt.date_departure as dateDeparture  from departments d
                                inner join wards w
                                on d.id = w.department_id
                                inner join ward_traffics wt
                                on w.id = wt.ward_id
                                where wt.hospital_treatment_id = @TreatmentId;";


                return connection.Query<WardTraffic>(sql, new { TreatmentId = treatmentId}).ToList();
            }
        }

        public List<HospitalOperation> GetOperations(int treatmentId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select id, date, name, result from operations
                                where hospital_treatment_id = @TreatmentId;";

                return connection.Query<HospitalOperation>(sql, new { TreatmentId = treatmentId}).ToList();
            }
        }

        public List<VisitProcedure> GetVisitProcedures()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select vp.id as id, mp.name as medicalProcedure, mp.id as medicalProcedureId, 
                                        vp.comment as comment, vp.visit_id as visitId, vp.count as count, v.date as visitDate,
                                        concat(ind.last_name, ' ', ind.first_name, coalesce(' ' || ind.middle_name, '')) as patient,
                                        (select count(*) from visit_procedures_histories where visit_procedure_id = vp.id) as countСompleted
                                        from medical_procedures mp inner join visit_procedures vp
                                        on mp.id = vp.medical_procedure_id
                                        inner join visits v
                                        on v.id = vp.visit_id
                                        inner join patients p
                                        on p.id = v.patient_id
                                        inner join individuals ind
                                        on ind.id = p.individual_id
                                        where v.employee_assignment_id = @ID";

                return connection.Query<VisitProcedure>(sql, new { ID = UserAuthorization.id }).ToList();


            }
        }

        public List<VisitProcedure> GetCurrentVisitProcedures(int visitId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select vp.id as id, mp.name as medicalProcedure, mp.id as medicalProcedureId, 
                                        vp.comment as comment, vp.visit_id as visitId, vp.count as count
                                        from medical_procedures mp inner join visit_procedures vp
                                        on mp.id = vp.medical_procedure_id
                                        where vp.visit_id = @VisitId";

                return connection.Query<VisitProcedure>(sql, new { VisitId = visitId }).ToList();


            }
        }

        public List<MedicalProcedure> GetMedicalProcedures()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {

                string sql = @"select id, name, code, description from medical_procedures;";

                return connection.Query<MedicalProcedure>(sql).ToList();


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

        public List<User> GetNoUsers()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select * from no_users_view;";

                return connection.Query<User>(sql).ToList();
            }
        }

        public List<Post> GetPosts()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select id, name, type from posts;";

                return connection.Query<Post>(sql).ToList();
            }
        }

        public void DeletePatient(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string sql = @"update patients
                               set is_deleted = true
                               where id = @ID";

                    connection.Execute(sql, new { ID = id });

                    MessageBox.Show($"Пациент успешно удален", "Успех", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        public void DeleteEmployee(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string sql = @"call delete_employee(@ID);";

                    connection.Execute(sql, new { ID = id });

                    MessageBox.Show($"Сотрудник успешно удален", "Успех", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

        public void DeleteEmployeeAssignment(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string sql = @"update employee_assignments
                               set is_deleted = true
                               where id = @ID";

                    connection.Execute(sql, new { ID = id });

                    MessageBox.Show($"Назначение успешно удалено", "Успех", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

        public void DeleteUser(int id, string role)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string sql = @"";

                    if (role == "Пациент")
                    {
                        sql = @"delete from user_patients where id = @ID";
                    }
                    else
                    {
                        sql = @"delete from user_employees where id = @ID";
                    }

                    connection.Execute(sql, new { ID = id });

                    MessageBox.Show($"Пользователь успешно удален", "Успех", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                                
            }
        }

        public bool AddPatient(Patient patient, Individual individual, List<Children> childrens, bool full)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var childrensId = childrens.Select(c => c.id).Distinct().ToArray();

                    string sql = "";
                    string result = "";

                    if (full)
                    {
                        sql = @"call insert_patient_full(@LastName, @FirstName, @MiddleName, @Phone, @Birthday::date,
                                                        @Snils, @Series, @Number, @IssuedBy, @IssueDate::date,
                                                        @Gender, @InsurancePolicy, @InsuranceCompany, @BirthCertificate,
                                                        @WingId, @Childrens, '0')";

                        result = connection.QueryFirstOrDefault<string>(sql, new
                        {
                            LastName = individual.lastName,
                            FirstName = individual.firstName,
                            MiddleName = individual.middleName,
                            Phone = individual.phone,
                            Birthday = individual.birthday.Date,
                            Snils = individual.snils,
                            Series = individual.passportSeries,
                            Number = individual.passportNumber,
                            IssuedBy = individual.passportIssuedBy,
                            IssueDate = individual.passportIssuedDate.Date,
                            Gender = individual.gender,
                            InsurancePolicy = individual.insurancePolicy,
                            InsuranceCompany = individual.insuranceCompany,
                            BirthCertificate = individual.birthCertificate,
                            WingId = patient.wingId,
                            Childrens = childrensId,
                        });

                        
                    }
                    else
                    {
                        sql = @"call insert_patient(@IndividualId, @WingId, '0')";

                        result = connection.QueryFirstOrDefault<string>(sql, new
                        {
                            IndividualId = patient.individualId,
                            WingId = patient.wingId
                        });
                    }

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {
                        switch (result)
                        {
                            case "-1": 

                                MessageBox.Show("Значение поля телефон неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-2":

                                MessageBox.Show("Значение поля СНИЛС неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-3":

                                MessageBox.Show("Значение полей серия и номер паспорта неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-4":

                                MessageBox.Show("Значение поля полис ОМС неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-5":

                                MessageBox.Show("Значение поля свидетельство о рождении неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-11":

                                MessageBox.Show("Уже существует такой пациент", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                        }
                            
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
            }
        }

        public bool UpdatePatient(Patient patient, Individual individual, List<Children> childrens)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var childrensId = childrens.Select(c => c.id).Distinct().ToArray();


                    string sql = @"call update_patient(@LastName, @FirstName, @MiddleName, @Phone, @Birthday::date,
                                                    @Snils, @Series, @Number, @IssuedBy, @IssueDate::date,
                                                    @Gender, @InsurancePolicy, @InsuranceCompany, @BirthCertificate,
                                                    @PatientId, @WingId, @IndividualId, @Childrens, '0')";

                    string result = connection.QueryFirstOrDefault<string>(sql, new
                    {
                        LastName = individual.lastName,
                        FirstName = individual.firstName,
                        MiddleName = individual.middleName,
                        Phone = individual.phone,
                        Birthday = individual.birthday,
                        Snils = individual.snils,
                        Series = individual.passportSeries,
                        Number = individual.passportNumber,
                        IssuedBy = individual.passportIssuedBy,
                        IssueDate = individual.passportIssuedDate,
                        Gender = individual.gender,
                        InsurancePolicy = individual.insurancePolicy,
                        InsuranceCompany = individual.insuranceCompany,
                        BirthCertificate = individual.birthCertificate,
                        PatientId = patient.id,
                        WingId = patient.wingId,
                        IndividualId = patient.individualId,
                        Childrens = childrensId,
                    });

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {
                        switch (result)
                        {
                            case "-1":

                                MessageBox.Show("Значение поля телефон неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-2":

                                MessageBox.Show("Значение поля СНИЛС неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-3":

                                MessageBox.Show("Значение полей серия и номер паспорта неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-4":

                                MessageBox.Show("Значение поля полис ОМС неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-5":

                                MessageBox.Show("Значение поля свидетельство о рождении неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
            }
        }

        public bool AddEmployee(Employee employee, Individual individual, List<Children> childrens, bool full)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var childrensId = childrens.Select(c => c.id).Distinct().ToArray();

                    string sql = "";
                    string result = "";

                    if (full)
                    {
                        sql = @"call insert_employee_full(@LastName, @FirstName, @MiddleName, @Phone, @Birthday::date,
                                                        @Snils, @Series, @Number, @IssuedBy, @IssueDate::date,
                                                        @Gender, @InsurancePolicy, @InsuranceCompany, @BirthCertificate,
                                                        @PostId, @DateAdmission::date, @DateDismissal::date, @Childrens, '0')";

                        result = connection.QueryFirstOrDefault<string>(sql, new
                        {
                            LastName = individual.lastName,
                            FirstName = individual.firstName,
                            MiddleName = individual.middleName,
                            Phone = individual.phone,
                            Birthday = individual.birthday.Date,
                            Snils = individual.snils,
                            Series = individual.passportSeries,
                            Number = individual.passportNumber,
                            IssuedBy = individual.passportIssuedBy,
                            IssueDate = individual.passportIssuedDate.Date,
                            Gender = individual.gender,
                            InsurancePolicy = individual.insurancePolicy,
                            InsuranceCompany = individual.insuranceCompany,
                            BirthCertificate = individual.birthCertificate,
                            PostId = employee.postId,
                            DateAdmission = employee.dateAdmission.Date,
                            DateDismissal = employee.dateDismissal?.Date,
                            Childrens = childrensId,
                        });


                    }
                    else
                    {
                        sql = @"call insert_employee(@IndividualId, @PostId, @DateAdmission::date, @DateDismissal::date, '0')";

                        result = connection.QueryFirstOrDefault<string>(sql, new
                        {
                            IndividualId = employee.individualId,
                            PostId = employee.postId,
                            DateAdmission = employee.dateAdmission.Date,
                            DateDismissal = employee.dateDismissal?.Date
                        });
                    }

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {
                        switch (result)
                        {
                            case "-1":

                                MessageBox.Show("Значение поля телефон неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-2":

                                MessageBox.Show("Значение поля СНИЛС неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-3":

                                MessageBox.Show("Значение полей серия и номер паспорта неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-4":

                                MessageBox.Show("Значение поля полис ОМС неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-5":

                                MessageBox.Show("Значение поля свидетельство о рождении неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-11":

                                MessageBox.Show("Уже существует такой сотрудник", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
        }

        public bool UpdateEmployee(Employee employee, Individual individual, List<Children> childrens)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var childrensId = childrens.Select(c => c.id).Distinct().ToArray();


                    string sql = @"call update_employee(@LastName, @FirstName, @MiddleName, @Phone, @Birthday::date,
                                                    @Snils, @Series, @Number, @IssuedBy, @IssueDate::date,
                                                    @Gender, @InsurancePolicy, @InsuranceCompany, @BirthCertificate,
                                                    @EmployeeId, @PostId, @DateAdmission::date, @DateDismissal::date, @IndividualId, @Childrens, '0')";

                    string result = connection.QueryFirstOrDefault<string>(sql, new
                    {
                        LastName = individual.lastName,
                        FirstName = individual.firstName,
                        MiddleName = individual.middleName,
                        Phone = individual.phone,
                        Birthday = individual.birthday,
                        Snils = individual.snils,
                        Series = individual.passportSeries,
                        Number = individual.passportNumber,
                        IssuedBy = individual.passportIssuedBy,
                        IssueDate = individual.passportIssuedDate,
                        Gender = individual.gender,
                        InsurancePolicy = individual.insurancePolicy,
                        InsuranceCompany = individual.insuranceCompany,
                        BirthCertificate = individual.birthCertificate,
                        EmployeeId = employee.id,
                        PostId = employee.postId,
                        DateAdmission = employee.dateAdmission.Date,
                        DateDismissal = employee.dateDismissal?.Date,
                        IndividualId = employee.individualId,
                        Childrens = childrensId,
                    });

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {
                        switch (result)
                        {
                            case "-1":

                                MessageBox.Show("Значение поля телефон неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-2":

                                MessageBox.Show("Значение поля СНИЛС неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-3":

                                MessageBox.Show("Значение полей серия и номер паспорта неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-4":

                                MessageBox.Show("Значение поля полис ОМС неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-5":

                                MessageBox.Show("Значение поля свидетельство о рождении неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
        }

        public bool AddUser(User user)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string sql = @"call insert_user(@Login, @Password, @Role, @UserId, '0')";

                    string result = connection.QueryFirstOrDefault<string>(sql, new
                    {
                        Login = user.login,
                        Password = user.password,
                        Role = user.role,
                        UserId = user.id,
                    });

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {
                        switch (result)
                        {
                            case "-1":

                                MessageBox.Show("У этого пользователя уже есть учетная запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                            case "-2":

                                MessageBox.Show("Значение поля логин неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;

                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
        }

        public bool UpdateUser(User user)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string sql = @"call update_user(@Login, @Password, @Role, @ID, '0')";

                    string result = connection.QueryFirstOrDefault<string>(sql, new
                    {
                        Login = user.login,
                        Password = user.password,
                        Role = user.role,
                        ID = user.userId,
                    });

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {
                        if(result == "-1")
                        {
                            MessageBox.Show("Значение поля логин неуникально", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
        }

        public bool AddEmployeeAssignment(EmployeeAssignment employeeAssignment, List<WorkSchedule> workSchedules)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string[] jsonWorkSchedules = workSchedules.Select(ws =>
                    {
                        var jsonObject = new
                        {
                            timeStart = ws.timeStart,
                            timeEnd = ws.timeEnd,
                            weekDays = ws.weekDays
                        };
                        return JsonSerializer.Serialize(jsonObject);
                    }).ToArray();

                    string sql = @"call insert_employee_assignment(@EmployeeId, @EmploymentType, @DateFrom::date, @DateTo::date, @CabinetId, 
                                                                @WorkSchedules::jsonb[], '0')";

                    string result = connection.QueryFirstOrDefault<string>(sql, new
                    {
                        EmployeeId = employeeAssignment.employeeId,
                        EmploymentType = employeeAssignment.type,
                        DateFrom = employeeAssignment.dateFrom,
                        DateTo = employeeAssignment.dateTo,
                        CabinetId = employeeAssignment.cabinetId,
                        WorkSchedules = jsonWorkSchedules
                    });

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {
                        switch (result)
                        {
                            case "-1":
                                {
                                    MessageBox.Show($"У этого сотрудника уже есть основное назначение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    break;
                                }

                            case "-2":
                                {
                                    MessageBox.Show($"У этого сотрудника нету основного назначения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    break;
                                }

                            case "-3":
                                {
                                    MessageBox.Show($"Расписания этого сотрудника пересекаюся", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    break;
                                }
                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        public bool UpdateEmployeeAssignment(EmployeeAssignment employeeAssignment, List<WorkSchedule> workSchedules)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string[] jsonWorkSchedules = workSchedules.Select(ws =>
                    {
                        var jsonObject = new
                        {
                            timeStart = ws.timeStart,
                            timeEnd = ws.timeEnd,
                            weekDays = ws.weekDays
                        };
                        return JsonSerializer.Serialize(jsonObject);
                    }).ToArray();

                    string sql = @"call update_employee_assignment(@ID, @EmployeeId, @DateFrom::date, @DateTo::date, @CabinetId, 
                                                                @WorkSchedules::jsonb[], '0')";

                    string result = connection.QueryFirstOrDefault<string>(sql, new
                    {
                        ID = employeeAssignment.id,
                        EmployeeId = employeeAssignment.employeeId,
                        DateFrom = employeeAssignment.dateFrom,
                        DateTo = employeeAssignment.dateTo,
                        CabinetId = employeeAssignment.cabinetId,
                        WorkSchedules = jsonWorkSchedules
                    });

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {

                        if(result == "-1")
                        {
                            MessageBox.Show($"Расписания этого сотрудника пересекаюся", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
            }
        }

        public bool SaveVisitProcedure(VisitProcedure visitProcedure, List<VisitProcedureHistory> visitProcedureHistories)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string[] jsonVisitProcedureHistories = visitProcedureHistories.Select(vph =>
                    {
                        var jsonObject = new
                        {
                            date = vph.date
                        };
                        return JsonSerializer.Serialize(jsonObject);
                    }).ToArray();

                    
                    string sql = @"call save_visit_procedure_histories(@ID, @VisitProcedureHistories::jsonb, '0')";

                    string result = connection.QueryFirstOrDefault<string>(sql, new
                    {
                        ID = visitProcedure.id,
                        VisitProcedureHistories = jsonVisitProcedureHistories
                    });

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
        }

        public bool UpdateVisit(Visit visit, List<VisitProcedure> visitProcedures, bool treatment)
        {

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string[] jsonVisitProcedures = visitProcedures.Select(vp =>
                    {
                        var jsonObject = new
                        {
                            medicalProcedureId = vp.medicalProcedureId,
                            comment = vp.comment,
                            count = vp.count
                        };
                        return JsonSerializer.Serialize(jsonObject);
                    }).ToArray();

                    string jsonPatientCondition = JsonSerializer.Serialize(new
                    {
                        subjective = visit.subjective,
                        objective = visit.objective,
                        diagnosis = visit.diagnosis,
                    });

                    string sql = @"call update_visit(@ID, @PacientCondition::jsonb, @Recommendation, @VisitProcedures::jsonb[],
                                                        @Treatment, @DateVisit::date, '0')";

                    string result = connection.QueryFirstOrDefault<string>(sql, new
                    {
                        ID = visit.id,
                        PacientCondition = jsonPatientCondition,
                        Recommendation = visit.recommendation,
                        VisitProcedures = jsonVisitProcedures,
                        Treatment = treatment,
                        DateVisit = visit.date.Date
                    });

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
            
        }

        public bool SaveOrEndTreatment(Treatment treatment, List<HospitalResearche> researches, List<HospitalProcedure> procedures,
                                        List<HospitalProcedureHistory> procedureHistories, List<WardTraffic> wardTraffics, List<HospitalOperation> operations,
                                        bool end)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string[] jsonResearches = researches.Select(r =>
                    {
                        var jsonObject = new
                        {
                            date = r.date,
                            name = r.name,
                            result = r.result
                        };
                        return JsonSerializer.Serialize(jsonObject);
                    }).ToArray();

                    string[] jsonWardTraffics = wardTraffics.Select(w =>
                    {
                        var jsonObject = new
                        {
                            dateArrival = w.dateArrival,
                            dateDeparture = w.dateDeparture,
                            wardId = w.wardId
                        };
                        return JsonSerializer.Serialize(jsonObject);
                    }).ToArray();

                    string[] jsonOperations = operations.Select(o =>
                    {
                        var jsonObject = new
                        {
                            date = o.date,
                            name = o.name,
                            result = o.result
                        };
                        return JsonSerializer.Serialize(jsonObject);
                    }).ToArray();

                    string[] jsonProcedures = procedures.Select(p =>
                    {
                        string[] jsonProcedureHistories = procedureHistories.Where(ph => ph.hospitalProcedureId == p.id).Select(ph =>
                        {
                            var jsonObjectHistories = new
                            {
                                date = ph.date
                            };
                            return JsonSerializer.Serialize(jsonObjectHistories);

                        }).ToArray();

                        var jsonObject = new
                        {
                            date = p.date,
                            name = p.name,
                            count = p.count,
                            description = p.description,
                            procedureHistories = jsonProcedureHistories
                        };
                        return JsonSerializer.Serialize(jsonObject);
                    }).ToArray();

                    string sql = @"call save_treatment(@TreatmentId, @Researches::jsonb[], @Procedures::jsonb[], @WardTraffics::jsonb[], 
                                                        @Operations::jsonb[], @DateEnd::date, @End, '0')";

                    string result = connection.QueryFirstOrDefault(sql, new
                    {
                        TreatmentId = treatment.id,
                        Researches = jsonResearches,
                        Procedures = jsonProcedures,
                        WardTraffics = jsonWardTraffics,
                        Operations = jsonOperations,
                        DateEnd = DateTime.Now.Date,
                        End = end
                    });

                    if (result == "1")
                    {
                        return true;
                    }
                    else
                    {                        

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
        }


        
    }
}
