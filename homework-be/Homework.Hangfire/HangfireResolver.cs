using Hangfire;

namespace Homework.Hangfire
{
    public interface IHangfireResolver
    {
        /// <summary>
        /// Spustí metody hangfire
        /// </summary>
        void Run();
    }
    public class HangfireResolver : IHangfireResolver
    {
        public HangfireResolver(
            )
        {
        }

        public void Run()
        {
            #region Daily jobs 
             // RecurringJob.AddOrUpdate(() => CreateExcel(), "0 0 * * *");
            #endregion

            #region Hourly Jobs
            #endregion

            #region Minutely Jobs
            #endregion

            #region Weekly jobs 
            #endregion
        }
    }
}
