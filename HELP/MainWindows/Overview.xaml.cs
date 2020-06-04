using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using HELP.DataModels;
using HELP.Forms;

namespace HELP.MainWindows
{
    public partial class Overview : Window
    {
        public static ObservableCollection<string> filterItems = new ObservableCollection<string>();
        public static int roleFlag = -1;

        private ObservableCollection<Case> cases;

        private Filters selectedFilter;

        private Dictionary<string, bool> filterValues;
        private string caseFilter;
        private string patientFilter;

        private BackgroundWorker reevaluationWorker;

        public Overview()
        {
            cases = new ObservableCollection<Case>();
            filterValues = new Dictionary<string, bool>();
            caseFilter = "";
            patientFilter = "";

            reevaluationWorker = new BackgroundWorker();

            InitializeComponent();

            InitDummyData();
            InitListFunctions();
            InitReevaluationChecking();
        }

        #region Methods
        private void InitDummyData()
        {
            DynamicData.priorities.Add("Akut", 0);
            DynamicData.priorities.Add("Sehr Dringend", 10);
            DynamicData.priorities.Add("Dringend", 30);
            DynamicData.priorities.Add("Normal", 90);
            DynamicData.priorities.Add("Nicht Dringend", 120);

            DynamicData.statuses.Add("In Behandlung");
            DynamicData.statuses.Add("In Beobachtung");
            DynamicData.statuses.Add("Wartet");
            DynamicData.statuses.Add("Entlassen");

            DynamicData.treatmentRooms.Add("Chirurgie");
            DynamicData.treatmentRooms.Add("Orthopädie");
            DynamicData.treatmentRooms.Add("Kardiologie");
            DynamicData.treatmentRooms.Add("Gynäkologie");
            DynamicData.treatmentRooms.Add("Pädiatrie");
            DynamicData.treatmentRooms.Add("Wartezimmer");

            cases.Add(new Case(PatientList.patients[0])
            { Priority = Category.Akut, Reevaluation = DateTime.Now.AddMinutes(10), Status = "In Behandlung", TreatmentRoom = "Kardiologie", Complaint = "Fieber, Husten", Diagnosis = "COVID-19" });

            cases.Add(new Case(PatientList.patients[1])
            { Priority = Category.Sehr_Dringend, Reevaluation = DateTime.Now.AddMinutes(10), Status = "In Behandlung", TreatmentRoom = "Chirurgie", Complaint = "Schmerzen im Bein", Diagnosis = "Fibulafraktur" });

            cases.Add(new Case(PatientList.patients[2])
            { Priority = Category.Dringend, Reevaluation = DateTime.Now.AddMinutes(15), Status = "Wartet", TreatmentRoom = "Wartezimmer", Complaint = "Kopfschmerzen, Übelkeit", Diagnosis = "Magen-Darm Infektion" });

            cases.Add(new Case(PatientList.patients[3])
            { Priority = Category.Normal, Reevaluation = DateTime.Now.AddMinutes(20), Status = "In Beobachtung", TreatmentRoom = "Wartezimmer", Complaint = "Daumen blutet", Diagnosis = "Schnittwunde" });

            cases.Add(new Case(PatientList.patients[4])
            { Priority = Category.Nicht_Dringend, Reevaluation = DateTime.Now.AddMinutes(25), Status = "Wartet", TreatmentRoom = "Wartezimmer", Complaint = "Nase läuft", Diagnosis = "Grippaler Infekt" });

            ListViewMain.ItemsSource = cases;
        }

        private void InitListFunctions()
        {
            foreach (string item in DynamicData.PrioritiesAsList)
            {
                filterItems.Add(item);
                filterValues.Add(item, true);
            }

            foreach (string item in DynamicData.statuses)
            {
                filterValues.Add(item, true);
            }

            foreach (string item in DynamicData.treatmentRooms)
            {
                filterValues.Add(item, true);
            }

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewMain.ItemsSource);

            view.Filter = Filter;

            selectedFilter = Filters.PRIORITY;

            string[] sortedColumns = { "PriorityInt", "ArrivalFormatted", "Age", "Name" };

            foreach (string binding in sortedColumns)
            {
                view.SortDescriptions.Add(new SortDescription(binding, ListSortDirection.Ascending));
            }

            foreach (GridViewColumn column in ((GridView)ListViewMain.View).Columns)
            {
                if (column.DisplayMemberBinding == null ? true : sortedColumns.Contains(((Binding)column.DisplayMemberBinding).Path.Path))
                {
                    ((Image)((StackPanel)((GridViewColumnHeader)column.Header).Content).Children[0]).Source = new BitmapImage(new Uri(@"/Resources/Images/SortedAscending.png", UriKind.Relative));
                }
            }
        }

        private void InitReevaluationChecking()
        {
            reevaluationWorker.DoWork += ReevaluationTask;
            reevaluationWorker.RunWorkerCompleted += ReevaluationTaskCompleted;

            reevaluationWorker.RunWorkerAsync();
        }

        private void ReevaluationTask(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                foreach (Case item in ListViewMain.Items)
                {
                    if (DateTime.Compare(DateTime.Now, item.Reevaluation) == 0 || DateTime.Compare(DateTime.Now, item.Reevaluation) > 0)
                    {
                        e.Result = item;

                        return;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void ReevaluationTaskCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Case result = (Case)e.Result;

            MessageBox.Show("Es sollte nach dem Patienten " + result.Name + " (" + result.CaseNr +") geschaut werden!", "Erinnerung");

            for (int i = 0; i < ListViewMain.Items.Count; i++)
            {
                if (cases[i].CaseNr.Equals(result.CaseNr))
                {
                    cases[i].Reevaluation = DateTime.Now.AddMinutes(result.PriorityInt);
                }
            }

            ListViewMain.Items.Refresh();

            reevaluationWorker.RunWorkerAsync();
        }

        private bool Filter(object item)
        {
            if (
                filterValues[((Case)item).Priority.ToString().Replace('_', ' ')] &&
                filterValues[((Case)item).Status] &&
                filterValues[((Case)item).TreatmentRoom] &&
                ((Case)item).CaseNr.ToString().Contains(caseFilter) &&
                ((Case)item).Name.ToLower().Contains(patientFilter.ToLower())
                )
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Events
        private void NewCaseBtn_Click(object sender, RoutedEventArgs e)
        {
            new CaseWindow().Show();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            new CaseWindow((Case)ListViewMain.SelectedItem).ShowDialog();
        }

        private void GridViewColumnHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                switch (((Label)((StackPanel)((GridViewColumnHeader)sender).Content).Children[1]).Content)
                {
                    case "Priorität":
                        if (selectedFilter != Filters.PRIORITY)
                        {
                            filterItems.Clear();

                            foreach (string item in DynamicData.PrioritiesAsList)
                            {
                                filterItems.Add(item);
                            }

                            selectedFilter = Filters.PRIORITY;
                        }

                        break;
                    case "Status":
                        if (selectedFilter != Filters.STATUS)
                        {
                            filterItems.Clear();

                            foreach (string item in DynamicData.statuses)
                            {
                                filterItems.Add(item);
                            }

                            selectedFilter = Filters.STATUS;
                        }

                        break;
                    case "Aufenthaltsort":
                        if (selectedFilter != Filters.TREATMENTROOM)
                        {
                            filterItems.Clear();

                            foreach (string item in DynamicData.treatmentRooms)
                            {
                                filterItems.Add(item);
                            }

                            selectedFilter = Filters.TREATMENTROOM;
                        }

                        break;
                }
            } else if (e.LeftButton == MouseButtonState.Pressed)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewMain.ItemsSource);
                SortDescriptionCollection sortDescriptions = view.SortDescriptions;
                Image columnSortImage = ((Image)((StackPanel)((GridViewColumnHeader)sender).Content).Children[0]);
                object columnContent = ((GridViewColumnHeader)sender).Content;               

                foreach (GridViewColumn column in ((GridView)ListViewMain.View).Columns)
                {
                    if (((GridViewColumnHeader)column.Header).Content.Equals(columnContent))
                    {
                        string propertyName = column.DisplayMemberBinding == null ? "PriorityInt" : ((Binding)column.DisplayMemberBinding).Path.Path;
                        bool contained = false;
                        int position = 0;

                        for (int i = 0; i < sortDescriptions.Count; i++)
                        {
                            if (sortDescriptions[i].PropertyName.Equals(propertyName))
                            {
                                contained = true;
                                position = i;

                                break;
                            }
                        }

                        if (!contained)
                        {
                            sortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
                            columnSortImage.Source = new BitmapImage(new Uri(@"/Resources/Images/SortedAscending.png", UriKind.Relative));
                        } else
                        {
                            if (sortDescriptions[position].Direction == ListSortDirection.Ascending)
                            {
                                sortDescriptions.RemoveAt(position);
                                sortDescriptions.Insert(position, new SortDescription(propertyName, ListSortDirection.Descending));
                                columnSortImage.Source = new BitmapImage(new Uri(@"/Resources/Images/SortedDescending.png", UriKind.Relative));
                            } else
                            {
                                sortDescriptions.RemoveAt(position);
                                columnSortImage.Source = new BitmapImage(new Uri(@"/Resources/Images/Unsorted.png", UriKind.Relative));
                            }
                        }
                    }
                }
            }
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            //string key = ((ContentPresenter)((CheckBox)sender).Content).Content.ToString();
            //bool state = (bool)((CheckBox)sender).IsChecked;

            filterValues[((ContentPresenter)((CheckBox)sender).Content).Content.ToString()] = (bool)((CheckBox)sender).IsChecked;

            CollectionViewSource.GetDefaultView(ListViewMain.ItemsSource).Refresh();
        }

        private void CaseSearchFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            caseFilter = ((TextBox)sender).Text;

            CollectionViewSource.GetDefaultView(ListViewMain.ItemsSource).Refresh();
        }

        private void PatientSearchFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            patientFilter = ((TextBox)sender).Text;

            CollectionViewSource.GetDefaultView(ListViewMain.ItemsSource).Refresh();
        }
        #endregion

        private enum Filters
        {
            PRIORITY,
            STATUS,
            TREATMENTROOM
        }
    }
}