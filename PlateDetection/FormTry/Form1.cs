using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using openalprnet;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Emgu.CV.CvEnum;

namespace FormTry
{
    public partial class Form1 : Form
    {
        VideoCapture capture,capture1;
      
        string[,] arrPlate = new string[5, 2] {{"06DF144","Oguzhan Ozer"},
                                      {"34BGH75","Mehmet Korkmaz"},
                                      {"06ZGV89","Veli Kavlak"},
                                      {"06TL002","Cem Gok"},
                                      {"06GKN62","Deniz KIPCA"} };

        public Form1()
        {
            InitializeComponent();
           
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        
        private  Mutex m = new Mutex(false);
        private Semaphore s = new Semaphore(0,1);

        void detectPlate(Image img)
        {
            string person = "";
            string detectPlate = "";
            float maxConf = 0.0f;
            
            try
            {
                //mre.WaitOne();
               // m.WaitOne();
                s.WaitOne();

                var alpr = new AlprNet("eu", "C:\\Users\\oguzhan\\source\\repos\\ConsoleApp1\\ConsoleApp1\\bin\\x64\\openalpr.conf", "C:\\Users\\oguzhan\\source\\repos\\ConsoleApp1\\ConsoleApp1\\bin\\x64\\runtime_data");

                if (alpr.IsLoaded())
                {
                    textBox1.Text = "loaad";
                }
                else
                {
                    textBox1.Text = "OpenAlpr failed to load!";
                    return;
                }
                alpr.DefaultRegion = "md";

                //Image img;
                //img = (Image)target;

                var results = alpr.Recognize(ImageToByte(img));

                string ss = "";

                foreach (var result in results.Plates)
                {
                    ss += "" + result.TopNPlates.Count + "\n";

                    foreach (var plate in result.TopNPlates)
                    {
                        ss += plate.Characters + "\n";

                        richTextBox1.Text = "" + ss;

                        if (plate.OverallConfidence > maxConf)
                        {
                            maxConf = plate.OverallConfidence;
                            detectPlate = plate.Characters;
                        }
                    }

                }

                for (int i = 0; i < 5; i++)
                {
                    if (detectPlate == arrPlate[i, 0])
                    {
                        person = arrPlate[i, 1];
                    }
                }

                textBox1.Text = "  Matches Template :  " + detectPlate + " -- Person : " + person;
            }
            catch
            {
                m.ReleaseMutex();
            
            }
                

        }

         void processFrameAndUpdateGUIAsync(object sender, EventArgs arg)
         {
            Mat[] imgOriginal = new Mat[2];

            Task  xx =  Task.Run(()  => 
               {
                   imgOriginal[0] = capture.QueryFrame();

                   if (imgOriginal == null)
                   {
                       MessageBox.Show("unable to read frame from webcam" + Environment.NewLine + Environment.NewLine + "exiting program");
                       Environment.Exit(0);
                       return;
                   }
                   
                   ımageBox1.Image = imgOriginal[0];
                   detectPlate(imgOriginal[0].Bitmap);
                   
               });

              Task    yy = Task.Run(() =>
              {
                   imgOriginal[1] = capture1.QueryFrame();
                   ımageBox2.Image = imgOriginal[1];
                   detectPlate(imgOriginal[1].Bitmap);
                   
              });

            //imgOriginal[0] = capture.QueryFrame();
            //ımageBox1.Image = imgOriginal[0];

            //imgOriginal[1] = capture1.QueryFrame();
            //ımageBox2.Image = imgOriginal[1];
            
            //ThreadPool.QueueUserWorkItem(
            //    new WaitCallback(detectPlate) , imgOriginal[0].Bitmap);


            //ThreadPool.QueueUserWorkItem(
            //    new WaitCallback(detectPlate), imgOriginal[1].Bitmap);


        }

      
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                capture = new Emgu.CV.VideoCapture(0);
                capture1 = new Emgu.CV.VideoCapture(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("unable to read from webcam, error: " + Environment.NewLine + Environment.NewLine +
                                ex.Message + Environment.NewLine + Environment.NewLine +
                                "exiting program");
                Environment.Exit(0);
                return;
            }
    
         Application.Idle += processFrameAndUpdateGUIAsync;       // add process image function to the application's list of tasks
            
        }

    }

}

