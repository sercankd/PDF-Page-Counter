using System.Windows.Forms;

namespace PDF_Page_Counter
{
    public partial class WorkingOverlay : Form
    {
        public WorkingOverlay()
        {
            InitializeComponent();
            Application.EnableVisualStyles();
            Opacity = 0.4;
        }
    }
}
