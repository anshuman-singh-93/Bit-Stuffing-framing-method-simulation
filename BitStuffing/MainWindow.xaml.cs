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
using System.Windows.Threading;

namespace BitStuffing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StackPanel sp = new StackPanel();
        StackPanel sp_child1 = new StackPanel();
        StackPanel sp_child2 = new StackPanel();
        StackPanel sp_child3 = new StackPanel();
        TextBlock tb1 = new TextBlock();
        TextBlock tb2 = new TextBlock();
        TextBlock tb3 = new TextBlock();
        bool flag_found_in_data = false;
        bool returning = false;
        bool reached_at_dll = false;
        bool exec = true;
        DispatcherTimer timer = new DispatcherTimer();
        StringBuilder sb = new StringBuilder();
        string msg, flag;
        bool is_paused = false;

        public MainWindow()
        {
            InitializeComponent();
                reset_btn.IsEnabled = false;
            flag_box.IsEnabled = false;
            timer.Tick += new EventHandler(start_animation);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }


        public void start_animation(object sender,EventArgs e)
        {   double top=Canvas.GetTop(sp);
            double left=Canvas.GetLeft(sp);
            if(top<300)
            Canvas.SetTop(sp, top + 10);
            this.Title = top + "-" + left;
            if(top>=290 && left<=350)
            {
                Canvas.SetLeft(sp, left + 10);
                returning = true;
                reached_at_dll = true;
            }
            if(reached_at_dll && exec)
            {
                bit_stuff_process_start();
            }

            if (left > 350)
                Canvas.SetTop(sp,top-10);
            if(returning && top<=30)
            {
                //packet is reached at destination
                timer.Stop();
                log.Items.Add("messaged received at destination");
                tb2.Text = "";
                tb2.Text = msg;
            }





        }
     
        public void bit_stuff_process_start()
        {
            for(int i=0;i<sb.Length;i++)
            {
                int count = 0;
                for (int j=i;j<sb.Length;j++)
                {
                    if (sb[j] == '1')
                        count++;
                    else
                        break;
                    if (count == 5)
                    {
                        sb.Insert(j + 1, "0");
                        log.Items.Add("flag found in data,bit 0 is stuffed");
                        flag_found_in_data = true;
                    }
                    if (j > i + 4)
                        break;
                }
            }

            if (flag_found_in_data)
                log.Items.Add("data with stuffing-" + "\n" + sb);
            else
                log.Items.Add("flag not found in data");
            tb2.Text = "";
            tb2.Text = sb.ToString();
            reached_at_dll = false;
            exec = false;
        }
        public void init()
        {
            sp.Orientation = Orientation.Horizontal;
            sp.Width = 200;
            sp.Height = 40;
            sp.Background = Brushes.Blue;
            

            sp_child1.Width = 50;
            sp_child1.Height = 40;
            sp_child1.Background = Brushes.Red;


            sp_child2.Width = 100;
            sp_child2.Height = 40;
            sp_child2.Background = Brushes.AntiqueWhite;

            sp_child3.Width = 50;
            sp_child3.Height = 40;
            sp_child3.Background = Brushes.Gray;

            sp.Children.Add(sp_child1);
            sp.Children.Add(sp_child2);
            sp.Children.Add(sp_child3);

            Canvas.SetLeft(sp,50);
            Canvas.SetTop(sp, 10);

            bit_canvas.Children.Add(sp);
            add_data_in_frame();

        }

        private void add_data_in_frame()
        {
            tb1.Text = flag;
            tb1.FontSize = 11;

            tb2.Text = msg;
            tb2.FontSize = 11;

            tb3.Text =flag;
            tb3.FontSize = 11;
            tb1.HorizontalAlignment = HorizontalAlignment.Center;
            tb2.HorizontalAlignment = HorizontalAlignment.Center;
            tb3.HorizontalAlignment = HorizontalAlignment.Center;

            sp_child1.Children.Add(tb1);
            sp_child2.Children.Add(tb2);
            sp_child3.Children.Add(tb3);
            timer.Start();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            msg = msg_box.Text;
            flag = flag_box.Text;
            if(msg.Length==0 || flag.Length==0)
            {
                MessageBox.Show("some of the field are blank");
                return;
            }
            if(flag.Length<8)
            {
                MessageBox.Show("minumum length of flag is 8 bit");
                return;
            }
            foreach(char ch in msg)
            {
                if(ch!='0'&& ch!='1')
                {
                    MessageBox.Show("message takes only 1 and 0");
                    return;

                }
            }

            foreach (char ch in flag)
            {
                if (ch != '0' && ch != '1')
                {
                    MessageBox.Show("flag takes only 1 and 0");
                    return;

                }
            }

            reset_btn.IsEnabled = true;
            start_btn.IsEnabled = false;
            log.Items.Add("started with message bit- "+ msg);
            sb.Append(msg);
            init();

            


        }


        private void reset_Click(object sender, RoutedEventArgs e)
        {
            sp_child1.Children.Remove(tb1);
            sp_child2.Children.Remove(tb2);
            sp_child3.Children.Remove(tb3);

            sp.Children.Remove(sp_child1);
            sp.Children.Remove(sp_child2);
            sp.Children.Remove(sp_child3);

            bit_canvas.Children.Remove(sp);
            msg = string.Empty;
            flag = string.Empty;

            log.Items.Clear();
            returning = false;
            exec = true;
            sb.Clear();
            flag_found_in_data = false;

            start_btn.IsEnabled = true;
            reset_btn.IsEnabled = false;

        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            if(!is_paused)
            {
                is_paused = true;
                timer.Stop();
            }
            else
            {
                is_paused = false;
                timer.Start();
            }

        }
    }
}
