using System;

namespace DevAdventure
{
#if WINDOWS || XBOX
    static class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        [STAThread]
        static void Main()
        {
            try
            {
                using (DevAdventure game = new DevAdventure())
                {
                    game.Run();
                }
            }
            catch(Exception ex)
            {
                logger.Fatal("Uncaught exception: {0}", ex.Message);
                System.Windows.Forms.MessageBox.Show("Uncaught exception. Please check log file.", "Fatal error",  System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
#endif
}

