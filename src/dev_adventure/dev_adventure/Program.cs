using System;

namespace dev_adventure
{
#if WINDOWS || XBOX
    static class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
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

