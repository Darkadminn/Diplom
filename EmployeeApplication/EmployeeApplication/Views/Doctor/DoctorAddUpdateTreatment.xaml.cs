using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Xceed.Wpf.Toolkit.Calculator;

namespace EmployeeApplication
{
    /// <summary>
    /// Логика взаимодействия для DoctorAddUpdateTreatment.xaml
    /// </summary>
    public partial class DoctorAddUpdateTreatment : Window
    {
        DB dB = new DB();
        List<HospitalResearche> researches = new List<HospitalResearche>();
        List<HospitalProcedure> procedures = new List<HospitalProcedure>();
        List<HospitalProcedureHistory> procedureHistories = new List<HospitalProcedureHistory>();
        List<HospitalOperation> operations = new List<HospitalOperation>();
        List<WardTraffic> wardTraffics = new List<WardTraffic>();
        Treatment treatment0;
        public DoctorAddUpdateTreatment(Treatment treatment, bool read)
        {
            InitializeComponent();

            treatment0 = treatment;

            Patient.Text = treatment.patient;
            Diagnosis.Text = treatment.diagnosis;
            DateStart.SelectedDate = treatment.dateStart;
            DateEnd.SelectedDate = treatment.dateEnd;

            researches = dB.GetResearches(treatment.id);
            procedures = dB.GetHospitalProcedures(treatment.id);
            procedureHistories = dB.GetHospitalProcedureHistoriesTreatment(treatment.id);
            wardTraffics = dB.GetWardTraffics(treatment.id);
            operations = dB.GetOperations(treatment.id);

            DataGridResearches.ItemsSource = researches;
            DataGridProcedures.ItemsSource = procedures;
            DataGridWardTraffics.ItemsSource = wardTraffics;
            DataGridOperations.ItemsSource = operations;

            if(UserAuthorization.isOperation == true) TabOperations.Visibility = Visibility.Visible;
            else TabOperations.Visibility = Visibility.Collapsed;
        }

        private void ButtonClickBack(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите закрыть это окно? Все несохраненные данные будут утрачены.", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }

        }

        private void DataGridProcedures_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(DataGridProcedures.SelectedItem != null)
            {
                var procedure = DataGridProcedures.SelectedItem as HospitalProcedure;

                var window = new DoctorTreatmentProcedureHistories(procedure, procedureHistories);
                window.ShowDialog();

                if(window.DialogResult == true)
                {
                    procedureHistories.RemoveAll(ph => ph.hospitalProcedureId == procedure.id);
                    procedureHistories.AddRange(StaticHospitalProcedureHistory.hospitalProcedureHistories);

                    StaticHospitalProcedureHistory.hospitalProcedureHistories.Clear();

                }
            }
            
        }

        private void Departments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Departments.SelectedItem != null)
            {
                Department department = Departments.SelectedItem as Department;
                Wards.ItemsSource = dB.GetWards(department.id).Where(w => w.gender == treatment0.patientGender);
                Wards.IsEnabled = true;
            }
        }

        private void DataGridResearches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridResearches.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var treatmentResearches = (HospitalResearche)DataGridResearches.SelectedItem;

                DateResearche.SelectedDate = treatmentResearches.date;
                TimeResearche.Text = treatmentResearches.date.ToString("HH mm").Replace(" ", "");
                NameResearche.Text = treatmentResearches.name;
                ResultResearche.Text = treatmentResearches.result;

            }
        }

        private void ButtonClickAddResearche(object sender, RoutedEventArgs e)
        {
            if (DateResearche.SelectedDate == null || TimeResearche.Text.Contains(TimeResearche.PromptChar.ToString()) || string.IsNullOrWhiteSpace(NameResearche.Text))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (!TimeSpan.TryParse(TimeResearche.Text, out TimeSpan timeResearches))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DateResearche.SelectedDate > DateTime.Now)
                {
                    MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DataGridWardTraffics.Items.Count == 0)
                {
                    MessageBox.Show("Пациент не находится в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime? dateTime = DateResearche.SelectedDate?.Date.Add(timeResearches);

                WardTraffic wardTraffic = wardTraffics.OrderBy(tr => tr.dateArrival).FirstOrDefault();

                if ((DateTime)dateTime < wardTraffic.dateArrival)
                {
                    MessageBox.Show("В это время пациента не было в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string result0 = null;

                if (!string.IsNullOrWhiteSpace(ResultResearche.Text)) result0 = ResultResearche.Text;

                HospitalResearche treatmentResearches = new HospitalResearche
                {
                    date = (DateTime)dateTime,
                    name = NameResearche.Text,
                    result = result0
                };

                researches.Add(treatmentResearches);

                DataGridResearches.Items.Refresh();
            }
        }

        private void ButtonClickUpdateResearche(object sender, RoutedEventArgs e)
        {
            if (DataGridResearches.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (DateResearche.SelectedDate == null || TimeResearche.Text.Contains(TimeResearche.PromptChar.ToString()) || string.IsNullOrWhiteSpace(NameResearche.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (!TimeSpan.TryParse(TimeResearche.Text, out TimeSpan timeResearches))
                    {
                        MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (DateResearche.SelectedDate > DateTime.Now)
                    {
                        MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (DataGridWardTraffics.Items.Count == 0)
                    {
                        MessageBox.Show("Пациент не находится в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    DateTime? dateTime = DateResearche.SelectedDate?.Date.Add(timeResearches);

                    WardTraffic wardTraffic = wardTraffics.OrderBy(tr => tr.dateArrival).FirstOrDefault();

                    if ((DateTime)dateTime < wardTraffic.dateArrival)
                    {
                        MessageBox.Show("В это время пациента не было в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string result0 = null;

                    if (!string.IsNullOrWhiteSpace(ResultResearche.Text)) result0 = ResultResearche.Text;

                    var treatmentResearches = (HospitalResearche)DataGridResearches.SelectedItem;

                    int index = DataGridResearches.SelectedIndex;

                    HospitalResearche treatmentResearchesNew = new HospitalResearche
                    {
                        date = (DateTime)dateTime,
                        name = NameResearche.Text,
                        result = result0
                    };

                    researches.Remove(treatmentResearches);

                    researches.Insert(index, treatmentResearchesNew);

                    DataGridResearches.Items.Refresh();
                }
            }
        }

        private void ButtonClickDeleteResearche(object sender, RoutedEventArgs e)
        {
            var treatmentResearches = (HospitalResearche)DataGridResearches.SelectedItem;

            researches.Remove(treatmentResearches);

            DataGridResearches.Items.Refresh();
        }

        private void DataGridProcedures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridProcedures.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var treatmentProcedure = (HospitalProcedure)DataGridProcedures.SelectedItem;

                DateProcedure.SelectedDate = treatmentProcedure.date;
                TimeProcedure.Text = treatmentProcedure.date.ToString("HH mm").Replace(" ", "");
                NameProcedure.Text = treatmentProcedure.name;
                CountProcedure.Text = treatmentProcedure.count.ToString();
                DescriptionProcedure.Text = treatmentProcedure.description;

            }
        }

        private void ButtonClickAddProcedure(object sender, RoutedEventArgs e)
        {
            if (DateProcedure.SelectedDate == null || TimeProcedure.Text.Contains(TimeResearche.PromptChar.ToString()) || string.IsNullOrWhiteSpace(NameProcedure.Text))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (!TimeSpan.TryParse(TimeProcedure.Text, out TimeSpan timeProcedure))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DateProcedure.SelectedDate > DateTime.Now)
                {
                    MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DataGridWardTraffics.Items.Count == 0)
                {
                    MessageBox.Show("Пациент не находится в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime? dateTime = DateProcedure.SelectedDate?.Date.Add(timeProcedure);

                WardTraffic wardTraffic = wardTraffics.OrderBy(tr => tr.dateArrival).FirstOrDefault();

                if ((DateTime)dateTime < wardTraffic.dateArrival)
                {
                    MessageBox.Show("В это время пациента не было в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string description0 = "";

                if (!string.IsNullOrWhiteSpace(DescriptionProcedure.Text)) description0 = DescriptionProcedure.Text;

                HospitalProcedure treatmentProcedure = new HospitalProcedure
                {
                    date = (DateTime)dateTime,
                    name = NameProcedure.Text,
                    description = description0
                };

                procedures.Add(treatmentProcedure);

                DataGridProcedures.Items.Refresh();
            }
        }

        private void ButtonClickUpdateProcedure(object sender, RoutedEventArgs e)
        {
            if (DataGridProcedures.SelectedItem != null)
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (DateProcedure.SelectedDate == null || TimeProcedure.Text.Contains(TimeResearche.PromptChar.ToString()) || string.IsNullOrWhiteSpace(NameProcedure.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (!TimeSpan.TryParse(TimeProcedure.Text, out TimeSpan timeProcedure))
                    {
                        MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (DateProcedure.SelectedDate > DateTime.Now)
                    {
                        MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (DataGridWardTraffics.Items.Count == 0)
                    {
                        MessageBox.Show("Пациент не находится в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    DateTime? dateTime = DateProcedure.SelectedDate?.Date.Add(timeProcedure);

                    WardTraffic wardTraffic = wardTraffics.OrderBy(tr => tr.dateArrival).FirstOrDefault();

                    if ((DateTime)dateTime < wardTraffic.dateArrival)
                    {
                        MessageBox.Show("В это время пациента не было в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string description0 = "";

                    if (!string.IsNullOrWhiteSpace(DescriptionProcedure.Text)) description0 = DescriptionProcedure.Text;

                    var treatmentProcedure = (HospitalProcedure)DataGridProcedures.SelectedItem;

                    HospitalProcedure treatmentProcedureNew = new HospitalProcedure
                    {
                        date = (DateTime)dateTime,
                        name = NameProcedure.Text,
                        description = description0
                    };

                    int index = DataGridProcedures.SelectedIndex;

                    procedures.Remove(treatmentProcedure);
                    procedures.Insert(index, treatmentProcedureNew);

                    DataGridProcedures.Items.Refresh();
                }
            }
        }

        private void ButtonClickDeleteProcedure(object sender, RoutedEventArgs e)
        {
            HospitalProcedure treatmentProcedure = (HospitalProcedure)DataGridProcedures.SelectedItem;

            procedures.Remove(treatmentProcedure);
            var procedureHistoriesRemove = procedureHistories.Where(ph => ph.hospitalProcedureId == treatmentProcedure.id).ToList();

            foreach (var procedureHistory in procedureHistoriesRemove)
            {
                procedureHistories.Remove(procedureHistory);
            }

            DataGridProcedures.Items.Refresh();
        }

        private void ButtonClickAddWardTraffic(object sender, RoutedEventArgs e)
        {
            if (DateWardTraffic.SelectedDate == null || TimeWardTraffic.Text.Contains(TimeWardTraffic.PromptChar.ToString()) || Departments.SelectedItem == null || Wards.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (!TimeSpan.TryParse(TimeWardTraffic.Text, out TimeSpan timeWardTraffic))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DateWardTraffic.SelectedDate > DateTime.Now)
                {
                    MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Department department = Departments.SelectedItem as Department;
                Ward ward = Wards.SelectedItem as Ward;
                DateTime? dateTime = DateWardTraffic.SelectedDate?.Date.Add(timeWardTraffic);

                if (wardTraffics.Count == 0)
                {
                    WardTraffic wardTraffic = new WardTraffic
                    {
                        departmentId = department.id,
                        department = department.name,
                        wardId = ward.id,
                        ward = ward.name,
                        dateArrival = (DateTime)dateTime
                    };

                    wardTraffics.Add(wardTraffic);

                    DataGridWardTraffics.Items.Refresh();
                }
                else
                {
                    WardTraffic wardTrafficOld = wardTraffics.Where(tr => tr.dateDeparture == null).FirstOrDefault();

                    if (wardTrafficOld.wardId == ward.id)
                    {
                        MessageBox.Show("Пациент уже находится в этой палате", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    wardTrafficOld.dateDeparture = dateTime;

                    WardTraffic wardTraffic = new WardTraffic
                    {
                        departmentId = department.id,
                        department = department.name,
                        wardId = ward.id,
                        ward = ward.name,
                        dateArrival = (DateTime)dateTime
                    };

                    wardTraffics.Add(wardTraffic);

                    DataGridWardTraffics.Items.Refresh();
                }

            }
        }

        private void ButtonClickDeleteWardTraffic(object sender, RoutedEventArgs e)
        {
            WardTraffic wardTrafficRemove = DataGridWardTraffics.SelectedItem as WardTraffic;

            wardTraffics.Remove(wardTrafficRemove);

            List<WardTraffic> selectedTraffics = wardTraffics.OrderBy(t => t.dateArrival).ToList();

            WardTraffic wardTraffic = selectedTraffics.LastOrDefault();
            if (wardTraffic != null)
            {
                wardTraffic.dateDeparture = null;
            }

            DataGridWardTraffics.Items.Refresh();
        }

        private void DataGridOperations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridOperations.SelectedItem == null)
            {
                MessageBox.Show("Выберите элемент", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                HospitalOperation treatmentOperation = (HospitalOperation)DataGridOperations.SelectedItem;

                DateOperation.SelectedDate = treatmentOperation.date;
                TimeOperation.Text = treatmentOperation.date.ToString("HH mm").Replace(" ", "");
                NameOperation.Text = treatmentOperation.name;

                if (treatmentOperation.result == true) ResultOperation.SelectedIndex = 0;
                else ResultOperation.SelectedIndex = 1;
            }
        }

        private void ButtonClickAddOperation(object sender, RoutedEventArgs e)
        {
            if (DateOperation.SelectedDate == null || TimeOperation.Text.Contains(TimeResearche.PromptChar.ToString()) || string.IsNullOrWhiteSpace(NameOperation.Text) || ResultOperation.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (!TimeSpan.TryParse(TimeOperation.Text, out TimeSpan timeOperation))
                {
                    MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DateOperation.SelectedDate > DateTime.Now)
                {
                    MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DataGridWardTraffics.Items.Count == 0)
                {
                    MessageBox.Show("Пациент не находится в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime? dateTime = DateOperation.SelectedDate?.Date.Add(timeOperation);

                WardTraffic wardTraffic = wardTraffics.OrderBy(tr => tr.dateArrival).FirstOrDefault();

                if ((DateTime)dateTime < wardTraffic.dateArrival)
                {
                    MessageBox.Show("В это время пациента не было в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool result0;

                if (ResultOperation.SelectedIndex == 0) result0 = true;
                else result0 = false;

                HospitalOperation operation = new HospitalOperation
                {
                    date = (DateTime)dateTime,
                    name = NameOperation.Text,
                    result = result0
                };

                operations.Add(operation);

                DataGridOperations.Items.Refresh();
            }
        }

        private void ButtonClickUpdateOperation(object sender, RoutedEventArgs e)
        {
            if (DataGridOperations.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (DateOperation.SelectedDate == null || TimeOperation.Text.Contains(TimeResearche.PromptChar.ToString()) || string.IsNullOrWhiteSpace(NameOperation.Text) || ResultOperation.SelectedIndex == -1)
                {
                    MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (!TimeSpan.TryParse(TimeOperation.Text, out TimeSpan timeOperation))
                    {
                        MessageBox.Show("Неверный формат времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (DateOperation.SelectedDate > DateTime.Now)
                    {
                        MessageBox.Show("Дата не может быть больше текущей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (DataGridWardTraffics.Items.Count == 0)
                    {
                        MessageBox.Show("Пациент не находится в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    DateTime? dateTime = DateOperation.SelectedDate?.Date.Add(timeOperation);

                    WardTraffic wardTraffic = wardTraffics.OrderBy(tr => tr.dateArrival).FirstOrDefault();

                    if ((DateTime)dateTime < wardTraffic.dateArrival)
                    {
                        MessageBox.Show("В это время пациента не было в стационаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    bool result0;

                    if (ResultOperation.SelectedIndex == 0) result0 = true;
                    else result0 = false;

                    var operation = (HospitalOperation)DataGridOperations.SelectedItem;

                    int index = DataGridOperations.SelectedIndex;

                    operations.Remove(operation);

                    HospitalOperation operationNew = new HospitalOperation
                    {
                        date = (DateTime)dateTime,
                        name = NameOperation.Text,
                        result = result0
                    };

                    operations.Insert(index, operationNew);

                    DataGridOperations.Items.Refresh();
                }
            }
        }

        private void ButtonClickDeleteOperation(object sender, RoutedEventArgs e)
        {
            HospitalOperation operation = DataGridOperations.SelectedItem as HospitalOperation;

            operations.Remove(operation);

            DataGridOperations.Items.Refresh();
        }

        private void ButtonClickSave(object sender, RoutedEventArgs e)
        {
            bool result = dB.SaveOrEndTreatment(treatment0, researches, procedures, procedureHistories, wardTraffics, operations, false);

            if (result == true)
            {
                MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK);
                this.DialogResult = true;
            }

        }

        private void ButtonClickPrint(object sender, RoutedEventArgs e)
        {


        }

        private void ButtonClickEnd(object sender, RoutedEventArgs e)
        {
            bool result = dB.SaveOrEndTreatment(treatment0, researches, procedures, procedureHistories, wardTraffics, operations, true);

            if (result == true)
            {
                MessageBox.Show("Курс лечения завершен", "Успех", MessageBoxButton.OK);
                this.DialogResult = true;
                this.Close();
            }
        }

        
    }
}
