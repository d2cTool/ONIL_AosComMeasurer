using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfMeasurerView
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class c4352m1_window : Window
    {
        private MeasurerViewModel viewModel;

        public event EventHandler SelectorNextPosition;
        public event EventHandler SelectorPrevPosition;
        public event EventHandler AcBtnClick;
        public event EventHandler DcBtnClick;
        public event EventHandler OhmBtnClick;
        public event EventHandler NoneBtnClick;
        public event EventHandler GuardOnBtnClick;
        public event EventHandler GuardOffBtnClick;

        public bool isActive { get; set; }
        public c4352m1_window()
        {
            InitializeComponent();

            isActive = false;

            UiGuardOnBtn.MouseLeftButtonUp += UiGuardOnBtn_MouseLeftButtonUp;
            UiGuardOffBtn.MouseLeftButtonUp += UiGuardOffBtn_MouseLeftButtonUp;
            UiSelector.MouseRightButtonUp += UiSelector_MouseRightButtonUp;
            UiSelector.MouseLeftButtonUp += UiSelector_MouseLeftButtonUp;
            UiTypeDcBtn.MouseLeftButtonUp += UiTypeDcBtn_MouseLeftButtonUp;
            UiTypeAcBtn.MouseLeftButtonUp += UiTypeAcBtn_MouseLeftButtonUp;
            UiTypeOhmBtn.MouseLeftButtonUp += UiTypeOhmBtn_MouseLeftButtonUp;
        }

        private void UiTypeOhmBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isActive)
            {
                if (UiTypeOhmBtn.Opacity == 0)
                {
                    UiTypeOhmBtn.Opacity = 1;

                    NoneBtnClick?.Invoke(this, new EventArgs());
                }
                else
                {
                    UiTypeOhmBtn.Opacity = 0;
                    UiTypeAcBtn.Opacity = 1;
                    UiTypeDcBtn.Opacity = 1;

                    OhmBtnClick?.Invoke(this, new EventArgs());
                }
            }
        }

        private void UiTypeAcBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isActive)
            {
                if (UiTypeAcBtn.Opacity == 0)
                {
                    UiTypeAcBtn.Opacity = 1;

                    NoneBtnClick?.Invoke(this, new EventArgs());
                }
                else
                {
                    UiTypeAcBtn.Opacity = 0;
                    UiTypeOhmBtn.Opacity = 1;
                    UiTypeDcBtn.Opacity = 1;

                    AcBtnClick?.Invoke(this, new EventArgs());
                }
            }
        }

        private void UiTypeDcBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isActive)
            {
                if (UiTypeDcBtn.Opacity == 0)
                {
                    UiTypeDcBtn.Opacity = 1;

                    NoneBtnClick?.Invoke(this, new EventArgs());
                }
                else
                {
                    UiTypeDcBtn.Opacity = 0;
                    UiTypeOhmBtn.Opacity = 1;
                    UiTypeAcBtn.Opacity = 1;

                    DcBtnClick?.Invoke(this, new EventArgs());
                }
            }
        }

        private void UiSelector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isActive)
            {
                SelectorNextPosition?.Invoke(this, new EventArgs());
            }
        }

        private void UiSelector_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isActive)
            {
                SelectorPrevPosition?.Invoke(this, new EventArgs());
            }
        }

        private void UiGuardOffBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isActive)
            {
                if (UiGuardOffBtn.Opacity == 1)
                {
                    UiGuardOffBtn.Opacity = 0;
                    GuardOffBtnClick?.Invoke(this, new EventArgs());
                }
            }
        }

        private void UiGuardOnBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isActive)
            {
                UiGuardOffBtn.Opacity = 1;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
        }

        private void MoveSelector(double angle)
        {
            RotateTransform rotateTransform = new RotateTransform(angle);
            UiSelector.RenderTransform = rotateTransform;
        }

        public void MoveArrow(double angle)
        {
            RotateTransform rotateTransform = new RotateTransform(angle);
            UiArrow.RenderTransform = rotateTransform;
        }

        private void SetType(string type)
        {
            switch (type)
            {
                case "0":
                    UiTypeAcBtn.Opacity = 1;
                    UiTypeDcBtn.Opacity = 1;
                    UiTypeOhmBtn.Opacity = 1;
                    break;
                case "1":
                    UiTypeAcBtn.Opacity = 1;
                    UiTypeDcBtn.Opacity = 0;
                    UiTypeOhmBtn.Opacity = 1;
                    break;
                case "2":
                    UiTypeAcBtn.Opacity = 1;
                    UiTypeDcBtn.Opacity = 1;
                    UiTypeOhmBtn.Opacity = 0;
                    break;
                case "3":
                    UiTypeAcBtn.Opacity = 0;
                    UiTypeDcBtn.Opacity = 1;
                    UiTypeOhmBtn.Opacity = 1;
                    break;
                default:
                    break;
            }
        }

        private void SetGuard(string btn)
        {
            switch (btn)
            {
                case "0":
                    UiGuardOffBtn.Opacity = 1;
                    UiGuardOnBtn.Opacity = 1;
                    break;
                case "1":
                    UiGuardOffBtn.Opacity = 0;
                    UiGuardOnBtn.Opacity = 1;
                    break;
                case "2":
                    UiGuardOffBtn.Opacity = 1;
                    UiGuardOnBtn.Opacity = 0;
                    break;
                default:
                    break;
            }
        }

        public void UpdateView(string type, double limit, string btn)
        {
            SetType(type);
            MoveSelector(limit);
            SetGuard(btn);
        }
    }
}
