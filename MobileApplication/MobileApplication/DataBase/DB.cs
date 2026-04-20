using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApplication
{
    public class DB
    {
        string connectionString = "Host=192.168.0.114;Port=5432;Database=db_medical_institutions;Username=postgres;Password=PostgreSQL";
        //string connectionString = "Host=10.109.137.252;Port=5432;Database=db_medical_institutions;Username=postgres;Password=PostgreSQL";
        //string connectionString = "Host=192.168.0.162;Port=5432;Database=db_medical_institutions;Username=postgres;Password=PostgreSQL";


        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        public void GetUserAuthorization(string login, string password)
        {
            using (var connection = GetConnection())
            {
                string sql = @"select authorization_patient(@Login, @Password)";

                int id = connection.QueryFirstOrDefault<int>(sql, new
                {
                    Login = login,
                    Password = password,
                });

                if (id != -1)
                {
                    connection.Open();

                    sql = @"select concat(ind.last_name, ' ', ind.first_name, coalesce(' ' || ind.middle_name, '')) as fio,
                    ind.gender as gender, p.wing_id as polyclinicId from individuals ind inner join patients p
                    on ind.id = p.individual_id
                    where p.id = @UserId";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserAuthorization.id = id;
                                UserAuthorization.fio = reader.GetString(0);
                                UserAuthorization.gender = reader.GetString(1);
                                UserAuthorization.polyclinicId = reader.GetInt32(2);
                            }
                        }
                    }

                    connection.Close();

                }
                else
                {

                    UserAuthorization.id = id;

                }
            }
        }

        public string AvailablePhoneLogin(string phone, string login)
        {
            using (var connection = GetConnection())
            {
                string sql = @"select available_phone_login_patient(@Phone, @Login)";

                return connection.QueryFirstOrDefault<string>(sql, new {
                    Phone = phone,
                    Login = login
                });
            }
        }

        public string AvailableUserPhone(string phone)
        {
            using (var connection = GetConnection())
            {
                string sql = @"select available_phone_user_patient(@Phone)";

                return connection.QueryFirstOrDefault<string>(sql, new { Phone = phone});
            }
        }

        public string InsertUser(string login, string password, string phone)
        {
            using (var connection = GetConnection())
            {
                string sql = @"call insert_user_patient(@Login, @Password, @Phone, '0')";

                return connection.QueryFirstOrDefault<string>(sql, new { 
                    Login = login,
                    Password = password,
                    Phone = phone 
                });
            }
        }

        public string UpdateUser(string password, string phone)
        {
            using (var connection = GetConnection())
            {
                string sql = @"call update_user_patient(@Password, @Phone, '0')";

                return connection.QueryFirstOrDefault<string>(sql, new
                {
                    Password = password,
                    Phone = phone
                });
            }
        }

        public List<Post> GetPosts()
        {
            using(var connection = GetConnection())
            {
                string sql = @"select * from get_list_posts(@PatientId, @TypeWing, @PolyclinicId)";

                return connection.Query<Post>(sql, new { 
                    PatientId = ReceptionSetting.patientId, 
                    TypeWing = ReceptionSetting.typeWing,
                    PolyclinicId = ReceptionSetting.patientPolyclinicId,
                }).ToList();
            }
        }

        public List<Filter> GetFilters()
        {
            using (var connection = GetConnection())
            {
                string sql = @"select * from get_list_filters(@PostId)";

                return connection.Query<Filter>(sql, new {PostId = ReceptionSetting.postId}).ToList();
            }
        }

        public List<Doctor> GetDoctors()
        {
            using (var connection = GetConnection())
            {
                string sql = @"select * from get_doctors(@PatientId, @PostId, @TypeWing, @PolyclinicId)";

                return connection.Query<Doctor>(sql, new {
                    PatientId = ReceptionSetting.patientId,
                    PostId = ReceptionSetting.postId,
                    TypeWing = ReceptionSetting.typeWing,
                    PolyclinicId = ReceptionSetting.patientPolyclinicId
                }).ToList();

            }
        }

        public List<WorkSchedule> GetWorkSchedules(int employeeAssignmentId, int wingId)
        {
            using (var connection = GetConnection())
            {
                string sql = @"select sch.id as id, sch.time_start as timeStart, sch.time_end as timeEnd, sch.week_days as weekDays, c.name as cabinetName 
                                from departments d inner join cabinets c
                                on d.id = c.department_id
                                inner join employee_assignments emp_a
                                on c.id = emp_a.cabinet_id
                                inner join work_schedules sch
                                on emp_a.id = sch.employee_assignment_id
                                where d.wing_id = @WingId and sch.employee_assignment_id = @EmployeeId and emp_a.is_deleted <> true
                                order by time_start;";

                return connection.Query<WorkSchedule>(sql, new
                {
                    WingId = wingId,
                    EmployeeId = employeeAssignmentId
                }).ToList();

            }
        }

        public List<Wing> GetWings()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select * from get_wings(@PatientId, @TypeWing, @PolyclinicId)";


                return connection.Query<Wing>(sql, new
                {
                    PatientId = ReceptionSetting.patientId,
                    TypeWing = ReceptionSetting.typeWing,
                    PolyclinicId = ReceptionSetting.patientPolyclinicId
                }).ToList();
            }
        }

        public List<Children> GetChildrens()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select p.id as id, ind.last_name as lastName, ind.first_name as firstName, coalesce(' ' || ind.middle_name, '') as middleName,
                                ind.birthday as birthday, ind.gender as gender, p.wing_id as wingId, coalesce(ind.insurance_policy, '') as insurancePolicy,
                                coalesce(ind.insurance_company, '') as insuranceCompany, coalesce(ind.snils, '') as snils 
                                from individuals ind inner join patients p
                                on ind.id = p.individual_id
                                inner join parent_childrens pc
                                on p.id = pc.children_id
                                where pc.parent_id = @PatientId";

                return connection.Query<Children>(sql, new
                {
                    PatientId = UserAuthorization.id,
                }).ToList();



            }
        }

        public bool AvailableDay(DateTime date, Filter userFilter)
        {
            var russianDays = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "Понедельник" },
                { DayOfWeek.Tuesday, "Вторник" },
                { DayOfWeek.Wednesday, "Среда" },
                { DayOfWeek.Thursday, "Четверг" },
                { DayOfWeek.Friday, "Пятница" },
                { DayOfWeek.Saturday, "Суббота" },
                { DayOfWeek.Sunday, "Воскресенье" }
            };

            List<Doctor> doctors = new List<Doctor>();
            doctors = GetDoctors();

            if (userFilter.type == "Врач") doctors = doctors.Where(emp => emp.fio == userFilter.name).ToList();
            else if (userFilter.type == "Город") doctors = doctors.Where(emp => emp.city == userFilter.name).ToList();
            else if (userFilter.type == "Медицинское учреждение") doctors = doctors.Where(emp => emp.wingName == userFilter.name).ToList();

            foreach (Doctor doctor in doctors)
            {
                List<WorkSchedule> workSchedules = GetWorkSchedules(doctor.id, doctor.wingId);

                workSchedules = workSchedules.Where(sch => sch.weekDays.Any(day => day.Trim().Equals(russianDays[date.DayOfWeek], StringComparison.OrdinalIgnoreCase))).ToList();

                foreach (WorkSchedule workSchedule in workSchedules)
                {
                    TimeSpan currentTime = workSchedule.timeStart;
                    var timeSlotsToCheck = new List<DateTime>();

                    while (currentTime + doctor.timeInterval <= workSchedule.timeEnd)
                    {
                        timeSlotsToCheck.Add(date.Date + currentTime);
                        currentTime = currentTime + doctor.timeInterval;
                    }

                    if (!timeSlotsToCheck.Any())
                        continue;

                    using (var connection = GetConnection())
                    {
                        string sql = @"select count(*) from visits v inner join employee_assignments emp_a
                                        on emp_a.id = v.employee_assignment_id 
                                        inner join work_schedules sch
                                        on emp_a.id = sch.employee_assignment_id
                                        where sch.id = @ScheduleID and v.date = any(@Dates)";

                        int occupiedSlots = connection.QueryFirst<int>(sql, new
                        {
                            ScheduleID = workSchedule.id,
                            Dates = timeSlotsToCheck.ToArray()
                        });


                        if (occupiedSlots < timeSlotsToCheck.Count) return true;
                    }
                }
            }

            return false;
        }

        public List<TimeSlot> AvailableTimeSlot(DateTime date, Doctor doctor)
        {
            var russianDays = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "Понедельник" },
                { DayOfWeek.Tuesday, "Вторник" },
                { DayOfWeek.Wednesday, "Среда" },
                { DayOfWeek.Thursday, "Четверг" },
                { DayOfWeek.Friday, "Пятница" },
                { DayOfWeek.Saturday, "Суббота" },
                { DayOfWeek.Sunday, "Воскресенье" }
            };

            List<WorkSchedule> workSchedules = GetWorkSchedules(doctor.id, doctor.wingId);

            workSchedules = workSchedules.Where(sch => sch.weekDays.Any(day => day.Trim().Equals(russianDays[date.DayOfWeek], StringComparison.OrdinalIgnoreCase))).ToList();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                var timeSlotsToCheck = new List<TimeSlot>();

                foreach (WorkSchedule workSchedule in workSchedules)
                {
                    TimeSpan currentTime = workSchedule.timeStart;

                    while (currentTime + doctor.timeInterval <= workSchedule.timeEnd)
                    {
                        string sql = @"select count(*) from visits v inner join employee_assignments emp_a
                                        on emp_a.id = v.employee_assignment_id 
                                        inner join work_schedules sch
                                        on emp_a.id = sch.employee_assignment_id
                                        where sch.id = @ScheduleID and v.date = @Date";


                        int occupiedSlots = connection.QueryFirst<int>(sql, new
                        {
                            ScheduleID = workSchedule.id,
                            Date = date.Add(currentTime),
                        });


                        if (occupiedSlots == 0)
                        {
                            timeSlotsToCheck.Add(new TimeSlot
                            {
                                date = date.Date.AddHours(currentTime.Hours).AddMinutes(currentTime.Minutes),
                                scheduleId = workSchedule.id,
                            }
                            );
                        }

                        currentTime = currentTime + doctor.timeInterval;
                    }


                }

                return timeSlotsToCheck;
            }



        }

        public void InsertVisit(DateTime dateTime, Page page, Doctor doctor)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string sql = @"call insert_visit_patient(@PatientId, @PostId, @Datee, @EmployeeAssignmentId, @EmployeeId, '0')";

                    string result = connection.QueryFirstOrDefault<string>(sql, new
                    {
                        PatientId = ReceptionSetting.patientId,
                        PostId = doctor.postId,
                        Datee = dateTime,
                        EmployeeAssignmentId = doctor.assignmentId,
                        EmployeeId = doctor.id
                    });

                    if (result == "-1") page.DisplayAlert("Ошибка", "Данный пациент находится в стационаре", "ОК");
                    else if (result == "-2") page.DisplayAlert("Ошибка", $"Вы уже записаны к этому врачу или врачу со специальностью {doctor.postName}", "ОК");
                    else if (result == "-3") page.DisplayAlert("Ошибка", "В пределах 30 минут у вас уже есть запись", "ОК");
                    else if (result == "1") page.DisplayAlert("Успех", "Запись прошла успешно", "ОК");
                }
                catch (Exception ex)
                {
                    page.DisplayAlert("Ошибка", $"{ex.Message}", "ОК");
                }

               
            }
        }



        public List<Visit> GetVisits()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select v.id as id, concat(ind.last_name, ' ', ind.first_name, coalesce(' ' || ind.middle_name, '')) as fio, 
                                v.date as date, w.name as wingName, c.name as cabinet, v.status as status, p.name as postName, 
                                concat(ind_pat.last_name, ' ', ind_pat.first_name, coalesce(' ' || ind_pat.middle_name, '')) as patient
                                from wings w inner join departments d
                                on w.id = d.wing_id
                                inner join cabinets c
                                on d.id = c.department_id
                                inner join employee_assignments emp_assig
                                on c.id = emp_assig.cabinet_id
                                inner join work_schedules sch
                                on emp_assig.id = sch.employee_assignment_id
                                inner join visits v
                                on emp_assig.id = v.employee_assignment_id
                                inner join employees emp
                                on emp.id = emp_assig.employee_id
                                inner join individuals ind
                                on emp.individual_id = ind.id
                                inner join patients pat
                                on pat.id = v.patient_id
                                inner join individuals ind_pat
                                on ind_pat.id = pat.individual_id
                                inner join posts p
                                on emp.post_id = p.id
                                where v.patient_id = @PatientId";

                return connection.Query<Visit>(sql, new { PatientId = UserAuthorization.id}).ToList();
            }
        }

        public void DeleteVisit(int visitId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"delete from visits where id = @ID";

                connection.Execute(sql, new {ID = visitId});
            }
        }


    }
}
