using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPARobot.UserControls
{
    /// <summary>
    /// CornExpressionEditor.xaml 的交互逻辑
    ///                                        Allowed values       Allowed special characters      Comment

    //┌───────────── second(optional)           0-59                * , - /                      
    //│ ┌───────────── minute                   0-59                * , - /                      
    //│ │ ┌───────────── hour                   0-23                * , - /                      
    //│ │ │ ┌───────────── day of month         1-31                * , - / L W ?                
    //│ │ │ │ ┌───────────── month              1-12 or JAN-DEC     * , - /                      
    //│ │ │ │ │ ┌───────────── day of week      0-6  or SUN-SAT     * , - / # L ?                   Both 0 and 7 means SUN
    //│ │ │ │ │ │
    //* * * * * *
    /// </summary>
    public partial class CronExpressionEditor : UserControl
    {
        public CronExpressionEditor()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SecondProperty =
            DependencyProperty.Register(nameof(Secound), typeof(string),typeof(CronExpressionEditor),new PropertyMetadata("*"));
        public string Secound
        {
            get => (string)GetValue(SecondProperty);
            set => SetValue(SecondProperty, value);
        }

        public static readonly DependencyProperty MinuteProperty =
            DependencyProperty.Register(nameof(Minute), typeof(string), typeof(CronExpressionEditor), new PropertyMetadata("*"));
        public string Minute
        {
            get => (string)GetValue(MinuteProperty);
            set => SetValue(MinuteProperty, value);
        }

        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register(nameof(Hour), typeof(string), typeof(CronExpressionEditor), new PropertyMetadata("*"));
        public string Hour
        {
            get => (string)GetValue(HourProperty);
            set => SetValue(HourProperty, value);
        }

        public static readonly DependencyProperty MonthProperty =
            DependencyProperty.Register(nameof(Month), typeof(string), typeof(CronExpressionEditor), new PropertyMetadata("*"));
        public string Month
        {
            get => (string)GetValue(MonthProperty);
            set => SetValue(MonthProperty, value);
        }

        public static readonly DependencyProperty DayOfMonthProperty =
            DependencyProperty.Register(nameof(DayOfMonth), typeof(string), typeof(CronExpressionEditor), new PropertyMetadata("*"));
        public string DayOfMonth
        {
            get => (string)GetValue(DayOfMonthProperty);
            set => SetValue(DayOfMonthProperty, value);
        }

        public static readonly DependencyProperty DayOfWeekProperty =
            DependencyProperty.Register(nameof(DayOfWeek), typeof(string), typeof(CronExpressionEditor), new PropertyMetadata("?"));
        public string DayOfWeek
        {
            get => (string)GetValue(DayOfWeekProperty);
            set => SetValue(DayOfWeekProperty, value);
        }

    }
}
