﻿using SmartUp.DataAccess.SQLServer.Dao;
using SmartUp.DataAccess.SQLServer.Model;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using SmartUp.Core.Constants;
using System.Text;

namespace SmartUp.UI
{
    public partial class SemesterStudent : Page
    {
        private int CreditsFromP = StudentDao.GetInstance().GetCreditsFromPByStudentID(Constants.STUDENT_ID);
        private static Semester SelectedSemester {  get; set; }

        public SemesterStudent()
        {
            InitializeComponent();
            foreach (Semester semester in SemesterDao.GetInstance().GetAllSemesters())
            {
                AddSemesterBlock(semester);
            }
        }

        public void AddSemesterBlock(Semester semester)
        {
            Border card = new Border();
            card.CornerRadius = new CornerRadius(20, 20, 20, 20);
            card.Background = Brushes.Gray;
            card.Margin = new Thickness(5);
            card.Width = 150;
            card.Height = 80;

            Grid cardGrid = new Grid();
            RowDefinition rowDefinition0 = new RowDefinition();
            RowDefinition rowDefinition1 = new RowDefinition();
            cardGrid.RowDefinitions.Add(rowDefinition0);
            cardGrid.RowDefinitions.Add(rowDefinition1);

            TextBlock semesterName = new TextBlock();
            semesterName.Text = semester.Abbreviation;
            semesterName.VerticalAlignment = VerticalAlignment.Center;
            semesterName.HorizontalAlignment = HorizontalAlignment.Center;
            semesterName.FontSize = 20;
            semesterName.FontWeight = FontWeights.Bold;
            semesterName.Foreground = Brushes.White;
            Grid.SetRow(semesterName, 0);

            Ellipse circle = new Ellipse();
            circle.Height = 30;
            circle.Width = 30;
            circle.Fill = Brushes.White;
            circle.HorizontalAlignment = HorizontalAlignment.Right;
            circle.Margin = new Thickness(0, 0, 10, 0);
            Grid.SetRow(circle, 1);

            TextBlock informationI = new TextBlock();
            informationI.Padding = new Thickness(0, 0, 22, 0);

            informationI.Text = "i";
            informationI.FontWeight = FontWeights.Bold;
            informationI.Foreground = Brushes.Black;
            informationI.FontSize = 15;
            informationI.HorizontalAlignment = HorizontalAlignment.Right;
            informationI.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(informationI, 1);

            cardGrid.Children.Add(circle);
            cardGrid.Children.Add(informationI);
            cardGrid.Children.Add(semesterName);

            card.Child = cardGrid;

            SemesterWrap.Children.Add(card);

            card.PreviewMouseDown += (sender, e) => CardMouseDown(semester, card);
            SemesterWrap.MouseDown += (sender, e) => SemesterWrapMouseDown(card);
        }

        private static void SemesterWrapMouseDown(Border card)
        {
            if (!card.IsMouseOver)
            {
                card.Background = Brushes.Gray;
   
            }
        }

        private void CardMouseDown(Semester semester, Border card)
        {
            SelectedSemester = semester;
            SemesterName.Text = semester.Name;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"EC nodig van propedeuse: {semester.RequiredCreditsFromP}\n\n");
            stringBuilder.Append($"Vakken in dit semester:\n");
            foreach (string courseName in SemesterCourseDao.GetInstance().GetSemesterCoursesBySemesterAbbreviation(semester.Abbreviation))
            {
                stringBuilder.Append($"  - {courseName}\n");
            }
            stringBuilder.Append($"\n\n{semester.Description}");
            SemesterDescription.Text = stringBuilder.ToString();
            card.Background = Brushes.DarkGray;
            if (semester.RequiredCreditsFromP > CreditsFromP)
            {
                EnrollButton.IsEnabled = false;
            }
            else
            {
                EnrollButton.IsEnabled = true;
            }
        }

        private void EnrollForSemester(object sender, RoutedEventArgs eventArgs)
        {
            if(SelectedSemester.Abbreviation != null )
            {
                SemesterRegistrationDao.CreateRegistrationByStudentIdBasedOnSemester(Constants.STUDENT_ID, SelectedSemester.Abbreviation);
            }
        }
    }
}
