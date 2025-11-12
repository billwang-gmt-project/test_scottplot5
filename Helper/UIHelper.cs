using System.Windows.Controls;

namespace Helper
{
    public class UIHelper
    {
        public static void InvokeOnUIThread(Control control, Action action)
        {
            if (control == null)
                return;

            try
            {
                if (control.Dispatcher.CheckAccess())
                {
                    control.Dispatcher.Invoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (Exception ex)
            {
                // log with exception here
                LogHelper.AddInfoLog("===========exception error================");
                LogHelper.AddInfoLog(ex.ToString());
                LogHelper.AddInfoLog("==========================================");
            }
        }
    }
}
