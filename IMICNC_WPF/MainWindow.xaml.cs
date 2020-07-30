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

using System.ComponentModel;
using EtherCATSeries;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Diagnostics;

namespace IMICNC_WPF
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        
        BackgroundWorker bgwThred = new BackgroundWorker();

        bool ToolStatus_Picture = true;
        bool ServerStatus_Picture = true;
        bool Shift = true;
        float SpeedValue = 0;
        float FeedSpeedValue = 0;
        int feed_angle = 0;
        int feed_roll = 0;

        public static Axis<ushort> PositiveLimitSwitchStatus = new Axis<ushort>();
        public static Axis<ushort> NegativeLimitSwitchStatus = new Axis<ushort>();

        // Draw //因為使用wpf所以需要加上System.Drawing.
        System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Brushes.Black,1);
        System.Drawing.Point draw_o = new System.Drawing.Point(0, 0);
        System.Drawing.Point draw_p;
        List<System.Drawing.Point> recoder = new List<System.Drawing.Point>();
        List<double> recoder_z = new List<double>();

        Bitmap Map;
        double temp_x = 0, temp_y = 0;
        BitmapImage Map_image;

        public MainWindow()
        {
            InitializeComponent();

        }

        string Path = "D://OneDrive//OneDrive - 國立成功大學//Document//IMI//【CNC】UI_REDESIGN//IMICNC_WPF//IMICNC_WPF//Resource";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
       
            #region LOAD PICTURE
            img_name.Source = new BitmapImage(new Uri(Path + "//LIN.png", UriKind.Absolute));
            img_on.Source = new BitmapImage(new Uri(Path + "//ON_1.png", UriKind.Absolute));
            img_off.Source = new BitmapImage(new Uri(Path + "//OFF_1.png", UriKind.Absolute));
            img_power.Source = new BitmapImage(new Uri(Path + "//Power_Light_1.png", UriKind.Absolute));
            img_lightx.Source = new BitmapImage(new Uri(Path + "//Limit_Light_1.png", UriKind.Absolute));
            img_lighty.Source = new BitmapImage(new Uri(Path + "//Limit_Light_1.png", UriKind.Absolute));
            img_lightz.Source = new BitmapImage(new Uri(Path + "//Limit_Light_1.png", UriKind.Absolute));
            img_emg.Source = new BitmapImage(new Uri(Path + "//EMG.png", UriKind.Absolute));
            img_o.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//O.png", UriKind.Absolute));
            img_n.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//N.png", UriKind.Absolute));
            img_g.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//G.png", UriKind.Absolute));
            img_7.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//7.png", UriKind.Absolute));
            img_8.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//8.png", UriKind.Absolute));
            img_9.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//9.png", UriKind.Absolute));
            img_x.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//X.png", UriKind.Absolute));
            img_y.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//Y.png", UriKind.Absolute));
            img_z.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//Z.png", UriKind.Absolute));
            img_4.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//4.png", UriKind.Absolute));
            img_5.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//5.png", UriKind.Absolute));
            img_6.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//6.png", UriKind.Absolute));
            img_m.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//M.png", UriKind.Absolute));
            img_s.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//S.png", UriKind.Absolute));
            img_t.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//T.png", UriKind.Absolute));
            img_1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//1.png", UriKind.Absolute));
            img_2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//2.png", UriKind.Absolute));
            img_3.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//3.png", UriKind.Absolute));
            img_f.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//F.png", UriKind.Absolute));
            img_h.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//H.png", UriKind.Absolute));
            img_eob.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//EOB.png", UriKind.Absolute));
            img_reduce.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//-.png", UriKind.Absolute));
            img_0.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//0.png", UriKind.Absolute));
            img_dot.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//DOT.png", UriKind.Absolute));
            img_pos.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//POS.png", UriKind.Absolute));
            img_prog.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//PROG.png", UriKind.Absolute));
            img_shift.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//SHIFT.png", UriKind.Absolute));
            img_can.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//CAN.png", UriKind.Absolute));
            img_input.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//INPUT.png", UriKind.Absolute));
            img_pageup.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//PAGEUP.png", UriKind.Absolute));
            img_up.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//UP.png", UriKind.Absolute));
            img_alert.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//ALERT.png", UriKind.Absolute));
            img_insert.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//INSERT.png", UriKind.Absolute));
            img_delete.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//DELETE.png", UriKind.Absolute));
            img_pagedown.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//PAGEDOWN.png", UriKind.Absolute));
            img_left.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//LEFT.png", UriKind.Absolute));
            img_down.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//DOWN.png", UriKind.Absolute));
            img_right.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//RIGHT.png", UriKind.Absolute));
            img_help.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//HELP.png", UriKind.Absolute));
            img_reset.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//RESET.png", UriKind.Absolute));
            img_message.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//MESSAGE.png", UriKind.Absolute));
            img_system.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//SYSTEM.png", UriKind.Absolute));
            img_ccw.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//CCW.png", UriKind.Absolute));
            img_cw.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//CW.png", UriKind.Absolute));
            img_stop.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//STOP.png", UriKind.Absolute));
            img_folderon.Source = new BitmapImage(new Uri(Path + "//ON_1.png", UriKind.Absolute));
            img_folderoff.Source = new BitmapImage(new Uri(Path + "//OFF_1.png", UriKind.Absolute));
            img_x1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//+X.png", UriKind.Absolute));
            img_x2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//-X.png", UriKind.Absolute));
            img_y1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//+Y.png", UriKind.Absolute));
            img_y2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//-Y.png", UriKind.Absolute));
            img_z1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//-Z.png", UriKind.Absolute));
            img_z2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//+Z.png", UriKind.Absolute));
            img_jog.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//JOG.png", UriKind.Absolute));
            img_feedrate.Source = new BitmapImage(new Uri(Path + "//FEEDRATE.png", UriKind.Absolute));
            img_circle.Source = new BitmapImage(new Uri(Path + "//CIRCLE.png", UriKind.Absolute));
            img_feed.Source = new BitmapImage(new Uri(Path + "//FEED.png", UriKind.Absolute));
            img_feedroll.Source = new BitmapImage(new Uri(Path + "//FEEDROLL.png", UriKind.Absolute));
            img_feedrateaim.Source = new BitmapImage(new Uri(Path + "//FEEDRATEAIM.png", UriKind.Absolute));

            #endregion

            MCCL.MCC_EnableLimitSwitchCheck(0);

            //連線至EMP-Z
            MCCLManager.ConnectEMPZ();

            //開啟執行緒
            bgwThred.WorkerReportsProgress = true;
            bgwThred.WorkerSupportsCancellation = true;
            bgwThred.DoWork += new DoWorkEventHandler(bgwThred_Dowork);
            bgwThred.ProgressChanged += new ProgressChangedEventHandler(bgwThred_ProgressChanged);
            bgwThred.RunWorkerAsync();

            //Draw 
            Map = new Bitmap(Convert.ToInt16(icvsDraw.Width), Convert.ToInt16(icvsDraw.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
          
            //SQL  
            DealData();

            //機構參數load
            UpdateParameterUI();
        }

        #region KeyBoard Picture Change
        private void img_o_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_o.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//O_2.png", UriKind.Absolute));
        }

        private void img_o_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_o.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//O.png", UriKind.Absolute));
        }

        private void img_n_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_n.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//N_2.png", UriKind.Absolute));
        }
        private void img_n_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_n.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//N.png", UriKind.Absolute));
        }

        private void img_g_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_g.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//G_2.png", UriKind.Absolute));
        }

        private void img_g_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_g.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//G.png", UriKind.Absolute));
        }

        private void img_7_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_7.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//7_2.png", UriKind.Absolute));
        }

        private void img_7_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_7.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//7.png", UriKind.Absolute));
        }

        private void img_8_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_8.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//8_2.png", UriKind.Absolute));
        }

        private void img_8_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_8.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//8.png", UriKind.Absolute));
        }

        private void img_9_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_9.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//9_2.png", UriKind.Absolute));
        }

        private void img_9_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_9.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//9.png", UriKind.Absolute));
        }

        private void img_x_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_x.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//X_2.png", UriKind.Absolute));
        }

        private void img_x_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_x.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//X.png", UriKind.Absolute));
        }

        private void img_y_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_y.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//Y_2.png", UriKind.Absolute));
        }

        private void img_y_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_y.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//Y.png", UriKind.Absolute));
        }

        private void img_z_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_z.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//Z_2.png", UriKind.Absolute));
        }

        private void img_z_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_z.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//Z.png", UriKind.Absolute));
        }

        private void img_4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_4.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//4_2.png", UriKind.Absolute));
        }

        private void img_4_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_4.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//4.png", UriKind.Absolute));
        }

        private void img_5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_5.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//5_2.png", UriKind.Absolute));
        }

        private void img_5_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_5.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//5.png", UriKind.Absolute));
        }

        private void img_6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_6.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//6_2.png", UriKind.Absolute));
        }

        private void img_6_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_6.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//6.png", UriKind.Absolute));
        }

        private void img_m_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_m.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//M_2.png", UriKind.Absolute));
        }

        private void img_m_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_m.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//M.png", UriKind.Absolute));
        }

        private void img_s_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_s.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//S_2.png", UriKind.Absolute));
        }

        private void img_s_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_s.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//S.png", UriKind.Absolute));
        }

        private void img_t_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_t.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//T_2.png", UriKind.Absolute));
        }

        private void img_t_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_t.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//T.png", UriKind.Absolute));
        }

        private void img_1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//1_2.png", UriKind.Absolute));
        }

        private void img_1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//1.png", UriKind.Absolute));
        }

        private void img_2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//2_2.png", UriKind.Absolute));
        }

        private void img_2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//2.png", UriKind.Absolute));
        }

        private void img_3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_3.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//3_2.png", UriKind.Absolute));
        }

        private void img_3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_3.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//3.png", UriKind.Absolute));
        }

        private void img_f_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_f.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//F_2.png", UriKind.Absolute));
        }

        private void img_f_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_f.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//F.png", UriKind.Absolute));
        }

        private void img_h_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_h.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//H_2.png", UriKind.Absolute));
        }

        private void img_h_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_h.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//H.png", UriKind.Absolute));
        }

        private void img_eob_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_eob.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//EOB_2.png", UriKind.Absolute));
        }

        private void img_eob_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_eob.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//EOB.png", UriKind.Absolute));
        }

        private void img_reduce_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_reduce.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//-_2.png", UriKind.Absolute));
        }

        private void img_reduce_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_reduce.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//-.png", UriKind.Absolute));
        }

        private void img_0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_0.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//0_2.png", UriKind.Absolute));
        }

        private void img_0_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_0.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//0.png", UriKind.Absolute));
        }

        private void img_dot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_dot.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//DOT_2.png", UriKind.Absolute));
        }

        private void img_dot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_dot.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//DOT.png", UriKind.Absolute));
        }

        private void img_pos_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_pos.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//POS_2.png", UriKind.Absolute));
        }

        private void img_pos_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_pos.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//POS.png", UriKind.Absolute));
        }

        private void img_prog_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_prog.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//PROG_2.png", UriKind.Absolute));
        }

        private void img_prog_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_prog.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//PROG.png", UriKind.Absolute));
        }

        private void img_system_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_system.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//SYSTEM_2.png", UriKind.Absolute));
        }

        private void img_system_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_system.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//SYSTEM.png", UriKind.Absolute));
        }

        private void img_shift_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_shift.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//SHIFT_2.png", UriKind.Absolute));
        }

        private void img_shift_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_shift.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//SHIFT.png", UriKind.Absolute));
        }

        private void img_can_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_can.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//CAN_2.png", UriKind.Absolute));
        }

        private void img_can_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_can.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//CAN.png", UriKind.Absolute));
        }

        private void img_input_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_input.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//INPUT_2.png", UriKind.Absolute));
        }

        private void img_input_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_input.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//INPUT.png", UriKind.Absolute));
        }

        private void img_pageup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_pageup.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//PAGEUP_2.png", UriKind.Absolute));
        }

        private void img_pageup_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_pageup.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//PAGEUP.png", UriKind.Absolute));
        }

        private void img_message_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_message.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//MESSAGE_2.png", UriKind.Absolute));
        }

        private void img_message_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_message.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//MESSAGE.png", UriKind.Absolute));
        }

        private void img_up_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_up.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//UP_2.png", UriKind.Absolute));
        }

        private void img_up_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_up.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//UP.png", UriKind.Absolute));
        }

        private void img_alert_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_alert.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//ALERT_2.png", UriKind.Absolute));
        }

        private void img_alert_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_alert.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//ALERT.png", UriKind.Absolute));
        }

        private void img_insert_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_insert.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//INSERT_2.png", UriKind.Absolute));
        }

        private void img_insert_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_insert.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//INSERT.png", UriKind.Absolute));
        }

        private void img_delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_delete.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//DELETE_2.png", UriKind.Absolute));
        }

        private void img_delete_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_delete.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//DELETE.png", UriKind.Absolute));
        }

        private void img_pagedown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_pagedown.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//PAGEDOWN_2.png", UriKind.Absolute));
        }

        private void img_pagedown_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_pagedown.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//PAGEDOWN.png", UriKind.Absolute));
        }

        private void img_left_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_left.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//LEFT_2.png", UriKind.Absolute));
        }

        private void img_left_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_left.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//LEFT.png", UriKind.Absolute));
        }

        private void img_down_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_down.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//DOWN_2.png", UriKind.Absolute));
        }

        private void img_down_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_down.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//DOWN.png", UriKind.Absolute));
        }

        private void img_right_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_right.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//RIGHT_2.png", UriKind.Absolute));
        }

        private void img_right_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_right.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//RIGHT.png", UriKind.Absolute));
        }

        private void img_help_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_help.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//HELP_2.png", UriKind.Absolute));
        }

        private void img_help_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_help.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off/HELP.png", UriKind.Absolute));
        }

        private void img_reset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_reset.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//RESET_2.png", UriKind.Absolute));
        }

        private void img_reset_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_reset.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//RESET.png", UriKind.Absolute));
        }

        private void img_y1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_y1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//+Y_2.png", UriKind.Absolute));
        }

        private void img_y1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_y1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//+Y.png", UriKind.Absolute));
        }


        private void img_z2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_z2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//+Z_2.png", UriKind.Absolute));
        }

        private void img_z2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_z2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//+Z.png", UriKind.Absolute));
        }

        private void img_x1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_x1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//+X_2.png", UriKind.Absolute));
        }

        private void img_x1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_x1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//+X.png", UriKind.Absolute));
        }

        private void img_x2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_x2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//-X_2.png", UriKind.Absolute));
        }

        private void img_x2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_x2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//-X.png", UriKind.Absolute));
        }

        private void img_z1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_z1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//-Z_2.png", UriKind.Absolute));
        }

        private void img_z1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_z1.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//-Z.png", UriKind.Absolute));
        }

        private void img_y2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_y2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//-Y_2.png", UriKind.Absolute));
        }

        private void img_y2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_y2.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//-Y.png", UriKind.Absolute));
        }
#endregion

        private void UpdateUI()
        {
            txtTime.Text = DateTime.Now.ToString();
        }
        
        #region bgwThred執行緒
        private void bgwThred_Dowork(object sender, DoWorkEventArgs e)
        {
            //所有背景任務皆在此
            while (true)
            {
                Thread.Sleep(10);
                if (bgwThred.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    //dowork here
                    MCCLManager.GetMCCLData();
                    DateTime.Now.ToShortDateString();
                    DateTime.Now.ToString("HH:mm:ss");
                    bgwThred.ReportProgress(10); //必須存在才能運作
                }

            }
        }
        private void bgwThred_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //更新ui皆在此處理
            UpdateUI();
            CoordinateUI();
            MonitorUI();
            LimitStatus();
        }

        #endregion

        #region 功能頁面

        #region 座標顯示
        private void CoordinateUI()
        {
            lab_xpos1.Content = (MCCLManager.machanicalCoordinate.X).ToString("##00.000");
            lab_ypos1.Content = (MCCLManager.machanicalCoordinate.Y).ToString("##00.000");
            lab_zpos1.Content = (MCCLManager.machanicalCoordinate.Z).ToString("##00.000");
            lab_xpos2.Content = (MCCLManager.absoluteCoordinate.X).ToString("##00.000");
            lab_ypos2.Content = (MCCLManager.absoluteCoordinate.Y).ToString("##00.000");
            lab_zpos2.Content = (MCCLManager.absoluteCoordinate.Z).ToString("##00.000");
        }

        #endregion

        #region 程式編輯
        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                if (open.ShowDialog() == true) //跟wf不一樣
                {
                    string FilePath = open.FileName,FileName = System.IO.Path.GetFileName(FilePath);

                    StreamReader sr = new StreamReader(open.FileName);
                    string result = sr.ReadToEnd();
                    txtGcode.Text = File.ReadAllText(open.FileName); //跟wf不一樣
                    sr.Close();
                    txtFilePlace.Text = FilePath;
                    txtFileName.Text = FileName;

                }
            }
            catch { }
        }

        private void btnConfirmFile_Click(object sender, RoutedEventArgs e)
        {
            //更改跳轉至下一頁面
        }
        #endregion

        #region 加工監控
        private void MonitorUI()
        {
            Draw(Convert.ToDouble(lab_xpos1.Content), Convert.ToDouble(lab_ypos1.Content), Convert.ToDouble(lab_zpos1.Content));
        }

        private void Draw(double x,double y,double z)
        {
            Graphics g = Graphics.FromImage(Map);

            if ((temp_x != x) || (temp_y != y))
            {
                draw_p = new System.Drawing.Point((int)x * 3, (int)y * 3);
                recoder.Add(draw_p);
                recoder_z.Add(z);

                if (z <= 0)
                {
                    pen.Color = System.Drawing.Color.Red;
                    g.DrawLine(pen, draw_o, draw_p);
                }
                else
                {
                    pen.Color = System.Drawing.Color.Black;
                    g.DrawLine(pen, draw_o, draw_p);
                }

                draw_o = draw_p;
                temp_x = x;
                temp_y = y;


                using (MemoryStream memory = new MemoryStream())
                {
                    Map.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage Map_imag = new BitmapImage();
                    Map_imag.BeginInit();
                    Map_imag.StreamSource = memory;
                    Map_imag.CacheOption = BitmapCacheOption.OnLoad;
                    Map_imag.EndInit();
                }

                img_Draw.Source = Map_image;
                img_Draw.InvalidateVisual(); //更新
            }
        }


        private void btnStartCode_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter SaveFile = new StreamWriter(@"C:\Users\Lin\Desktop\GG.txt");
            foreach (var item in txtGcode.Text)
            {
                SaveFile.WriteLine(item.ToString());
            }
            SaveFile.Close();

            string TxtPath = @"C:\Users\Lin\Desktop\GG.txt";
            String FilePath = TxtPath;

            StringBuilder InFilePath = new StringBuilder(FilePath);

            CompilerAndInterpreter.Compile(InFilePath);

            int strIndex = FilePath.Trim().LastIndexOf('.');
            StringBuilder OutFilePath = new StringBuilder(FilePath.Substring(0, strIndex) + "Intermediate.txt");
            CompilerAndInterpreter.readFile(OutFilePath/*中間語言的路徑*/);
            //key
            CompilerAndInterpreter.InterpretAll(0, 0, 0);

            MessageBox.Show("Done");
        }

        #endregion

        #region 刀具管理
        DataTable ToolDataTable;
        string Toolcnstr = @"Data Source = (LocalDB)\MSSQLLocalDB;" +
                    " AttachDbFilename = |DataDirectory|ToolDB.mdf;" +
                    "Integrated Security = True";
        void DealData()
        {
            SqlConnection ToolDBcn = new SqlConnection(Toolcnstr);
            try
            {
                ToolDBcn.Open();
                string Query = "SELECT* FROM ToolManager";
                SqlCommand Cmd = new SqlCommand(Query, ToolDBcn);
                SqlDataReader dr = Cmd.ExecuteReader();
                cbToolSelect.Items.Clear();
                while (dr.Read())
                {
                    string ID = dr.GetString(0);
                    cbToolSelect.Items.Add(ID);
                }

                ToolDBcn.Close();
            }
            catch (Exception)
            {
                throw;
            }

            //建立datast物件
            DataSet ToolDataSet = new DataSet();

            //建立sqldataadapter物件並取出資料表
            SqlDataAdapter ToolData = new SqlDataAdapter
                ("SELECT * FROM ToolManager", ToolDBcn);

            //將資料表所有資料填入ToolDataSet物件
            ToolData.Fill(ToolDataSet, "ToolManager");

            //宣告DataTable物件，並讓該物件存放DataSet中的資料表的DataTable
            ToolDataTable = ToolDataSet.Tables["ToolManager"];
        }
        void SaveData()
        {
            using (SqlConnection ToolDBcn = new SqlConnection())
            {
                //連接在地資料庫
                ToolDBcn.ConnectionString = Toolcnstr;
                ToolDBcn.Open();
                string sqlStr = "INSERT INTO ToolManager(編號,刀具名稱,刀具型別," +
                    "累積使用時間,預測換刀時間)VALUES('" + txtID.Text.Replace("'", "''") + "','" +
                    txtName.Text.Replace("'", "''") + "','" + txtType.Text.Replace("'", "''") +
                    "','" + txtUsedTime.Text.Replace("'", "''") + "'," + txtPreTime.Text.Replace("'", "''") + ")";

                //發送sql指令
                SqlCommand Cmd = new SqlCommand(sqlStr, ToolDBcn);
                //Cmd.ExecuteNonQuery();
                MessageBox.Show("Done");
                DealData();
            }
        }

        void DeleteData()
        {
            using (SqlConnection ToolDBcn = new SqlConnection())
            {
                ToolDBcn.ConnectionString = Toolcnstr;
                ToolDBcn.Open();
                string sqlStr = "DELETE FROM ToolManager WHERE 編號 = '" +
                    txtID.Text.Replace("'", "''") + "'";

                SqlCommand Cmd = new SqlCommand(sqlStr, ToolDBcn);
                //Cmd.ExecuteNonQuery();
                MessageBox.Show("Done");
                DealData();
            }
        }

        void ShowData()
        {
            txtID.Text = ToolDataTable.Rows[cbToolSelect.SelectedIndex]["編號"].ToString();
            txtName.Text = ToolDataTable.Rows[cbToolSelect.SelectedIndex]["刀具名稱"].ToString();
            txtType.Text = ToolDataTable.Rows[cbToolSelect.SelectedIndex]["刀具型別"].ToString();
            txtUsedTime.Text = ToolDataTable.Rows[cbToolSelect.SelectedIndex]["累積使用時間"].ToString();
            txtPreTime.Text = ToolDataTable.Rows[cbToolSelect.SelectedIndex]["預測換刀時間"].ToString();
        }


        private void btnToolAdd_Click(object sender, RoutedEventArgs e)
        {
            ClearData();
        }

        private void btnToolSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void btnToolClear_Click(object sender, RoutedEventArgs e)
        {
            ClearData();
        }

        private void btnToolDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteData();
        }

        private void cbToolSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowData();
        }

        void ClearData()
        {
            txtID.Text = "";
            txtName.Text = "";
            txtPreTime.Text = "";
            txtType.Text = "";
            txtUsedTime.Text = "";
        }


        #endregion

        #region 參數管理
        public void UpdateParameterUI()
        {
            txtPosToEncoder.Text = "0";
            txtRPM.Text = "3000";
            txtPPR.Text = "1048576";
            txtPitch.Text = "8.23";
            txtGearRatio.Text = "1";
            txtHightLimit.Text = "50000";
            txtLowLimit.Text = "50000";
            txtPulseMode.Text = "DAA";
            txtPulseWidth.Text = "100";
            txtCommand.Text = "0";
            txtHighSpeed.Text = "0.1";
            txtLowSpeed.Text = "0.0001";
        }
        #endregion

        #endregion

                
        #region 開關
        private void Img_on_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MCCLManager.ConnectEMPZ();
            MCCLManager.SetMachineParameter();
            MCCLManager.StartEMPZ();
            MCCLManager.ServoOn();
            ServerStatus_Picture = true;
            ServerStatus();
            img_power.Source = new BitmapImage(new Uri(Path + "//Power_Light_2.png", UriKind.Absolute));
            img_folderoff.Source = new BitmapImage(new Uri(Path + "//OFF_2.png", UriKind.Absolute));
        }

        private void Img_off_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MCCLManager.ServoOff();
            MCCLManager.SpindleOff();
            ServerStatus_Picture = false;
            ServerStatus();
        }

        private void Img_emg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MCCLManager.ServoOff();
            MCCLManager.EndEMPZ();
            MCCLManager.DisConnectEMPZ();
            MCCLManager.SpindleOff();
            ServerStatus_Picture = false;
            ServerStatus();
            img_power.Source = new BitmapImage(new Uri(Path + "//Power_Light_1.png", UriKind.Absolute));
        }


        private void ServerStatus()
        {
            if (ServerStatus_Picture == true)
            {
                img_on.Source = new BitmapImage(new Uri(Path + "//ON_2.png", UriKind.Absolute));
                img_off.Source = new BitmapImage(new Uri(Path + "//OFF_1.png", UriKind.Absolute));
            }
            else
            {
                img_on.Source = new BitmapImage(new Uri(Path + "//ON_1.png", UriKind.Absolute));
                img_off.Source = new BitmapImage(new Uri(Path + "//OFF_2.png", UriKind.Absolute));
            }
        }
        #endregion

        #region 主軸
        private void Img_cw_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MCCLManager.isSpindleOn == false)
            {
                MCCLManager.SpindleOn();
                SpeedValue = 2000;
            }
            else
            {
                MessageBox.Show("Have on");
            }
        }

        private void Img_stop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MCCLManager.isSpindleOn == true)
            {
                MCCLManager.SpindleOff();
                SpeedValue = 0;
            }
            else
            {
                MessageBox.Show("Have off");
            }
        }

        #endregion

        #region JOG Contiune
        private void Img_x1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sbyte axis = 0, direction = 1;
            MCCLManager.JogConti(axis, direction);
        }

        private void Img_x2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sbyte axis = 0, direction = -1;
            MCCLManager.JogConti(axis, direction);
        }

        private void Img_y1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sbyte axis = 1, direction = 1;
            MCCLManager.JogConti(axis, direction);
        }

        private void Img_y2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sbyte axis = 1, direction = -1;
            MCCLManager.JogConti(axis, direction);
        }
        private void Img_z2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sbyte axis = 2, direction = 1;
            MCCLManager.JogConti(axis, direction);
        }

        private void Img_z1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sbyte axis = 2, direction = -1;
            MCCLManager.JogConti(axis, direction);
        }

        private void JogMouseUp(object sender, MouseButtonEventArgs e)
        {
            MCCLManager.MCC_AbortMotionEx();
        }

        #endregion

        #region Machining
        private void Btn_original_Click(object sender, RoutedEventArgs e)
        {
            MCCLManager.MCC_GoHome();
        }

        private void Btn_mechpoint_Click(object sender, RoutedEventArgs e)
        {
            MCCLManager.MCC_DefineMachining();
        }

        #endregion

        #region 刀具夾緊
        private void Img_folderon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MCCLManager.ClampTool();
            //夾緊刀具
            ToolStatus_Picture = true;
            StatusDisplay();
        }

        private void Img_folderoff_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MCCLManager.UnClampTool();
            //鬆開刀具
            ToolStatus_Picture = false;
            StatusDisplay();
        }
        
        private void StatusDisplay()
        {
            if (ToolStatus_Picture)
            {
                img_folderon.Source = new BitmapImage(new Uri(Path + "//ON_2.png", UriKind.Absolute));
                img_folderoff.Source = new BitmapImage(new Uri(Path + "//OFF_1.png", UriKind.Absolute));
            }
            else
            {
                img_folderon.Source = new BitmapImage(new Uri(Path + "//ON_1.png", UriKind.Absolute));
                img_folderoff.Source = new BitmapImage(new Uri(Path + "//OFF_2.png", UriKind.Absolute));
            }
        }
        #endregion

        #region LimitStatus
        private void LimitStatus()
        {
            //MCCL.MCC_EnableLimitSwitchCheck();
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.X, 0, 0, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.Y, 0, 1, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref NegativeLimitSwitchStatus.Z, 0, 2, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.X, 1, 0, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.Y, 1, 1, MCCL.CARDINDEX0);
            MCCL.MCC_GetLimitSwitchStatus(ref PositiveLimitSwitchStatus.Z, 1, 2, MCCL.CARDINDEX0);
            Debug.Print("limit-:" + PositiveLimitSwitchStatus.X.ToString());

            if (PositiveLimitSwitchStatus.X != 0)
            {
                img_lightx.Source = new BitmapImage(new Uri(Path + "//Limit_Light_2.png", UriKind.Absolute));
                Debug.Print("Y");
            }
            else
            {
                img_lightx.Source = new BitmapImage(new Uri(Path + "//Limit_Light_1.png", UriKind.Absolute));
                Debug.Print("N");
            }

        }

        #endregion

        #region 虛擬鍵盤
        private void Img_shift_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                img_shift.Source = new BitmapImage(new Uri(Path + "//KeyBoard//On//SHIFT_2.png", UriKind.Absolute));
                Shift = false;
            }
            else
            {
                img_shift.Source = new BitmapImage(new Uri(Path + "//KeyBoard//Off//SHIFT.png", UriKind.Absolute));
                Shift = true;
            }
        }

        private void Img_o_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("O");
            }
            else
            {
                Insert("P");
            }
            
        }

        private void Img_n_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("N");
            }
            else
            {
                Insert("Q");
            }
            
        }

        private void Img_g_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("G");
            }
            else
            {
                Insert("R");
            }
        }

        private void Img_7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("7");
            }
            else
            {
                Insert("A");
            }
        }
        private void Img_8_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("8");
            }
            else
            {
                Insert("B");
            }
        }
        private void Img_9_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("9");
            }
            else
            {
                Insert("C");
            }
        }

        private void Img_x_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("X");
            }
            else
            {
                Insert("U");
            }
        }

        private void Img_y_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("Y");
            }
            else
            {
                Insert("V");
            }
        }

        private void Img_z_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("Z");
            }
            else
            {
                Insert("W");
            }
        }

        private void Img_4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("4");
            }
            else
            {
                Insert("[");
            }
        }

        private void Img_5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("5");
            }
            else
            {
                Insert("]");
            }
        }

        private void Img_6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("6");
            }
            else
            {
                Insert("SP");
            }
        }

        private void Img_m_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("M");
            }
            else
            {
                Insert("I");
            }

        }

        private void Img_s_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("S");
            }
            else
            {
                Insert("J");
            }
        }

        private void Img_t_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("T");
            }
            else
            {
                Insert("K");
            }
        }

        private void Img_1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("1");
            }
            else
            {
                Insert(",");
            }
        }

        private void Img_2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("2");
            }
            else
            {
                Insert("#");
            }
        }

        private void Img_3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("3");
            }
            else
            {
                Insert("=");
            }
        }

        private void Img_f_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("F");
            }
            else
            {
                Insert("L");
            }
        }

        private void Img_h_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("H");
            }
            else
            {
                Insert("D");
            }
        }

        private void Img_eob_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert(" ");
            }
            else
            {
                Insert("E");
            }
        }

        private void Img_reduce_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("-");
            }
            else
            {
                Insert("+");
            }
        }

        private void Img_0_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert("0");
            }
            else
            {
                Insert("*");
            }
        }

        private void Img_dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Shift == true)
            {
                Insert(".");
            }
            else
            {
                Insert("/");
            }
        }

        //從游標處加入文字的方法
        void Insert(String txtstring)
        {
            int start = txtGcode.SelectionStart;
            txtGcode.Text = txtGcode.Text.Insert(start, txtstring);
            txtGcode.Focus();
            txtGcode.SelectionStart = start;
            txtGcode.SelectionLength = txtstring.Length;
        }
        #endregion

        #region 旋鈕事件處理

        //處理圖片旋轉.
        //進給率
        private void Img_feedrateaim_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (feed_angle > 0)
            {
                feed_angle -= 15;
                img_feedrateaim.RenderTransform = new RotateTransform(feed_angle); //Xaml 部分必須撰寫RenderTransformOrigin ="0.5,0.5"
                FeedSpeedValue += 10;
            }
            else if (feed_angle >= 210)
            {
                Debug.Print("limit");
            }

            feed_angle = feed_angle % 360;
            
            //MCCLManager.SetFeedSpeed(FeedSpeedValue / 60);
        }

       
        //處理圖片旋轉
        private void Img_feedrateaim_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (feed_angle < 210 && feed_angle >= 0)
            {
                feed_angle += 15;
                img_feedrateaim.RenderTransform = new RotateTransform(feed_angle); //Xaml 部分必須撰寫RenderTransformOrigin ="0.5,0.5"
                FeedSpeedValue -= 10;
            }
            else
            {
                Debug.Print("limit");
            }

            feed_angle = feed_angle % 360;
            
            //MCCLManager.SetFeedSpeed(FeedSpeedValue / 60);
        }

        //處理"主軸速度"圖片旋轉
        private void Img_feedroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (feed_roll < 324)
            {
                feed_roll += 36;
                img_feedroll.RenderTransform = new RotateTransform(feed_roll);
                SpeedValue += 1000;
            }

            feed_roll = feed_roll % 360;        
            MCCLManager.ToolRPMChange(SpeedValue / 5000);

        }

        private void Img_feedroll_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (feed_roll > 0)
            {
                feed_roll -= 36;
                img_feedroll.RenderTransform = new RotateTransform(feed_roll);
                SpeedValue -= 1000;
            }

            feed_roll = feed_roll % 360;
            
            MCCLManager.ToolRPMChange(SpeedValue / 5000);
        }
        #endregion

    }
}
