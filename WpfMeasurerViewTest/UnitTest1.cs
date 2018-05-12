using System;
using System.Threading;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfMeasurerView;

namespace WpfMeasurerViewTest
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        public void WpfMeasurerViewSimpleTest()
        {
            c4352m1_window win = null; // = new c4352m1_window();
            Thread thread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                c4352m1_window window = new c4352m1_window();

                win = window;

                window.Closed += (s, e) => Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                //window.MoveSelector(34.4);

                window.Show();
                Dispatcher.Run();

            }));

            thread.Name = "Ui_Wpf_Thread";
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;

            thread.Start();

            Thread.Sleep(1000);

            Dispatcher.FromThread(thread).Invoke(new Action(() =>
            {
                //win?.MoveSelector(64.4);
            }));

            Dispatcher.FromThread(thread).Invoke(new Action(() =>
            {
                win?.MoveArrow(55);
                //win?.SetType("1");
            }));


            //win.MoveSelector(34.4);

            while (true)
            {
                Thread.Sleep(1);
            }
        }
    }
}
