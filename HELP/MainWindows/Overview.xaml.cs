#region References
using System;
using System.Collections.Generic;
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
using HELP.Resources.UserControls;
#endregion

namespace HELP.MainWindows
{
    public partial class Overview : Window
    {
        #region Variables
        private BackgroundWorker reevaluationWorker;
        private bool continueReevaluation;
        #endregion

        #region Constructors
        public Overview()
        {
            InitializeComponent();

            continueReevaluation = false;

            //InitDummyData();
            LoadData();
            InitFilters();

            this.reevaluationWorker = new BackgroundWorker();

            if (App.Role == 1)
            {
                continueReevaluation = true;

                InitReevaluationCheck();
            } else if (App.Role == 2)
            {
                this.btnNewCase.IsEnabled = false;
            }

            Closing += (sender, e) =>
            {
                if (App.CloseState == 0) Application.Current.Shutdown();
            };

            Closed += (sender, e) => App.CloseState = 0;
        }
        #endregion

        #region Methods

        #region Events
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Case caseClone = ((Case)((ListViewItem)sender).Content).Clone();
            //Case caseClone = ((Case)listViewMain.SelectedItem).Clone();
            CaseWindow selectedCase = new CaseWindow((Case)((ListViewItem)sender).Content);

            if (selectedCase.ShowDialog() == false)
            {
                for (int i = 0; i < DynamicData.Cases.Count; i++)
                {
                    if (caseClone.CaseNr.Equals(DynamicData.Cases[i].CaseNr))
                    {
                        DynamicData.Cases[i] = caseClone;

                        break;
                    }
                }
            }
        }

        private void GridViewColumnHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SortDescriptionCollection sortDescriptions = View.SortDescriptions;
                Image columnSortImage = ((Image)((StackPanel)((GridViewColumnHeader)sender).Content).Children[0]);
                object columnContent = ((GridViewColumnHeader)sender).Content;

                foreach (GridViewColumn column in ((GridView)listViewMain.View).Columns)
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
                        }
                        else
                        {
                            if (sortDescriptions[position].Direction == ListSortDirection.Ascending)
                            {
                                sortDescriptions.RemoveAt(position);
                                sortDescriptions.Insert(position, new SortDescription(propertyName, ListSortDirection.Descending));
                                columnSortImage.Source = new BitmapImage(new Uri(@"/Resources/Images/SortedDescending.png", UriKind.Relative));
                            }
                            else
                            {
                                sortDescriptions.RemoveAt(position);
                                columnSortImage.Source = new BitmapImage(new Uri(@"/Resources/Images/Unsorted.png", UriKind.Relative));
                            }
                        }
                    }
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            continueReevaluation = false;

            App.CloseState = 1;
            App.Role = 0;
            App.FullNameUser = "";

            App.DBConnection.StopDynamicDataLoading();
            DynamicData.Patients.Clear();
            DynamicData.Cases.Clear();

            new Login(false).Show();

            this.Close();
        }

        private void BtnResetList_Click(object sender, RoutedEventArgs e)
        {
            string[] sortedColumns = { "PriorityInt", "ArrivalFormatted", "Age", "Name" };
            List<string> filters = new List<string>(DynamicData.FilterValues.Keys);

            foreach (string key in filters)
            {
                DynamicData.FilterValues[key] = true;
            }

            // Besser Lösung?
            priorityHeader.ContextMenu = new CheckBoxFilterMenu(DynamicData.PrioritiesAsList);
            statusHeader.ContextMenu = new CheckBoxFilterMenu(DynamicData.Statuses);
            locationHeader.ContextMenu = new CheckBoxFilterMenu(DynamicData.Locations);

            caseNrHeader.ContextMenu = new SearchBoxFilterMenu("Durchgangsnummer");
            nameHeader.ContextMenu = new SearchBoxFilterMenu("Patientenname");

            DynamicData.CaseNrFilter = "";
            DynamicData.NameFilter = "";

            // ----------------------------------------------------------

            SetDefaultListSorting();

            foreach (GridViewColumn column in ((GridView)listViewMain.View).Columns)
            {
                Image sortIcon = ((Image)((StackPanel)((GridViewColumnHeader)column.Header).Content).Children[0]);

                if (column.DisplayMemberBinding == null ? true : sortedColumns.Contains(((Binding)column.DisplayMemberBinding).Path.Path))
                {
                    sortIcon.Source = new BitmapImage(new Uri(@"/Resources/Images/SortedAscending.png", UriKind.Relative));
                }
                else
                {
                    sortIcon.Source = new BitmapImage(new Uri(@"/Resources/Images/Unsorted.png", UriKind.Relative));
                }
            }
        }

        private void BtnNewCase_Click(object sender, RoutedEventArgs e)
        {
            new CaseWindow().ShowDialog();
        }
        #endregion

        private void InitFilters()
        {
            this.priorityHeader.ContextMenu = new CheckBoxFilterMenu(DynamicData.PrioritiesAsList);
            this.statusHeader.ContextMenu = new CheckBoxFilterMenu(DynamicData.Statuses);
            this.locationHeader.ContextMenu = new CheckBoxFilterMenu(DynamicData.Locations);

            this.caseNrHeader.ContextMenu = new SearchBoxFilterMenu("Durchgangsnummer");
            this.nameHeader.ContextMenu = new SearchBoxFilterMenu("Patientenname");

            View.Filter = ListFilter;
        }

        private void InitReevaluationCheck()
        {
            this.reevaluationWorker.DoWork += ReevaluationTask;
            this.reevaluationWorker.RunWorkerCompleted += ReevaluationTaskCompleted;

            this.reevaluationWorker.RunWorkerAsync();
        }

        private void LoadData()
        {
            this.listViewMain.ItemsSource = DynamicData.Cases;

            View = (CollectionView)CollectionViewSource.GetDefaultView(listViewMain.ItemsSource);

            SetDefaultListSorting();
        }

        private void SetDefaultListSorting()
        {
            string[] sortedColumns = { "PriorityInt", "ArrivalFormatted", "Age", "Name" };

            foreach (string column in sortedColumns)
            {
                View.SortDescriptions.Add(new SortDescription(column, ListSortDirection.Ascending));
            }
        }

        private bool ListFilter(object item)
        {
            if (
                DynamicData.FilterValues[((Case)item).Priority] &&
                DynamicData.FilterValues[((Case)item).Status] &&
                DynamicData.FilterValues[((Case)item).Location] &&
                ((Case)item).CaseNr.ToString().Contains(DynamicData.CaseNrFilter) &&
                ((Case)item).Name.ToLower().Contains(DynamicData.NameFilter.ToLower())
                )
            {
                return true;
            }

            return false;
        }

        private void ReevaluationTask(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                foreach (Case item in listViewMain.Items)
                {
                    if (DateTime.Compare(DateTime.Now, item.Reevaluation) == 0 || DateTime.Compare(DateTime.Now, item.Reevaluation) > 0)
                    {
                        e.Result = item;

                        return;
                    }
                }

                Thread.Sleep(500);
            }
        }

        private void ReevaluationTaskCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!continueReevaluation) return;

            Case result = (Case)e.Result;

            MessageBox.Show("Es sollte nach dem Patienten " + result.Name + " (" + result.CaseNr + ") geschaut werden!", "Erinnerung");

            for (int i = 0; i < listViewMain.Items.Count; i++)
            {
                if (DynamicData.Cases[i].CaseNr.Equals(result.CaseNr)) DynamicData.Cases[i].Reevaluation = DateTime.Now.AddMinutes(result.PriorityInt);
            }

            App.DBConnection.UpdateReevaluationTimer(result.CaseNr, DateTime.Now.AddMinutes(result.PriorityInt));

            listViewMain.Items.Refresh();

            this.reevaluationWorker.RunWorkerAsync();
        }
        #endregion

        #region Properties
        public static CollectionView View { get; private set; }
        #endregion
    }
}
