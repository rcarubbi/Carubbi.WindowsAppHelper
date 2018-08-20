using System.ComponentModel;
using System.Threading;

namespace Carubbi.WindowsAppHelper
{
    /// <summary>
    /// BackgroundWorker com capacidade de abortar a thread no caso de uma ThreadAbortException
    /// </summary>
    public class AbortableBackgroundWorker : BackgroundWorker
    {
        private Thread _workerThread;

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            _workerThread = Thread.CurrentThread;
            try
            {
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                e.Cancel = true; //We must set Cancel property to true!
                Thread.ResetAbort(); //Prevents ThreadAbortException propagation
            }
        }

        /// <summary>
        /// Aborta a thread interna
        /// </summary>
        public void Abort()
        {
            if (_workerThread == null) return;

            _workerThread.Abort();
            _workerThread = null;
        }
    }
}
