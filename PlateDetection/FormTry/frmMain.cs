using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using Emgu.CV;                  //
using Emgu.CV.CvEnum;           // usual Emgu CV imports
using Emgu.CV.Structure;        //
using Emgu.CV.UI;               //


namespace FormTry
{
    public partial class frmMain : Form
    {

        // member variables ///////////////////////////////////////////////////////////////////////
        VideoCapture capWebcam;

        // constructor ////////////////////////////////////////////////////////////////////////////
        public frmMain()
        {
            InitializeComponent();

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                capWebcam = new Emgu.CV.VideoCapture(0);
           
            }
            catch (Exception ex)
            {
                MessageBox.Show("unable to read from webcam, error: " + Environment.NewLine + Environment.NewLine +
                                ex.Message + Environment.NewLine + Environment.NewLine +
                                "exiting program");
                Environment.Exit(0);
                return;
            }
            Application.Idle += processFrameAndUpdateGUI;       // add process image function to the application's list of tasks
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void processFrameAndUpdateGUI(object sender, EventArgs arg)
        {
            Mat imgOriginal;

            imgOriginal = capWebcam.QueryFrame();

            if (imgOriginal == null)
            {
                MessageBox.Show("unable to read frame from webcam" + Environment.NewLine + Environment.NewLine +
                                "exiting program");
                Environment.Exit(0);
                return;
            }

            Mat imgGrayscale = new Mat(imgOriginal.Size, DepthType.Cv8U, 1);
            Mat imgBlurred = new Mat(imgOriginal.Size, DepthType.Cv8U, 1);
            Mat imgCanny = new Mat(imgOriginal.Size, DepthType.Cv8U, 1);

            CvInvoke.CvtColor(imgOriginal, imgGrayscale, ColorConversion.Bgr2Gray);

            CvInvoke.GaussianBlur(imgGrayscale, imgBlurred, new Size(5, 5), 1.5);

            CvInvoke.Canny(imgBlurred, imgCanny, 100, 200);

           // ibOriginal.Image = imgOriginal;
           // ibCanny.Image = imgCanny;
        }

    }   // end class
}
