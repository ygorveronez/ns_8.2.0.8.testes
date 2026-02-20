using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGT.Hangfire.Threads
{
    public static class HangfireExtensions
    {

        public const string tagRecurringJob = "recurring-job";
        public const string tagStopJob = "recurring-jobs-stop";
        public static void AgendarJob(string JobId)
        {
            using (var connection = JobStorage.Current.GetConnection())
            using (var transaction = connection.CreateWriteTransaction())
            {
                transaction.RemoveFromSet(tagStopJob, JobId);
                transaction.AddToSet($"{tagRecurringJob}s", JobId);
                transaction.Commit();
            }
        }


        public static void RemoverAgendamentoDoJob(string jobId)
        {
            using (var connection = JobStorage.Current.GetConnection())
            using (var transaction = connection.CreateWriteTransaction())
            {
                transaction.RemoveFromSet(tagStopJob, jobId);
                transaction.Commit();
            }
        }

        public static void PararJob(string JobId)
        {
            using (var connection = JobStorage.Current.GetConnection())
            using (var transaction = connection.CreateWriteTransaction())
            {
                transaction.RemoveFromSet($"{tagRecurringJob}s", JobId);
                transaction.AddToSet($"{tagStopJob}", JobId);
                transaction.Commit();
            }
        }

        public static List<string> GetAllStopedJobs()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                return connection.GetAllItemsFromSet(tagStopJob).ToList();
            }
        }

        public static List<string> GetAllRecurringJobs()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                return connection.GetAllItemsFromSet($"{tagRecurringJob}s").ToList();
            }
        }

        public static bool IsJobStopedOrDoesNotExist(string JobId)
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                return connection.GetAllItemsFromSet(tagStopJob).Contains(JobId) || !connection.GetAllItemsFromSet($"{tagRecurringJob}s").Contains(JobId);
            }
        }

        public static bool JobEstaAgendado(string JobId)
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                return connection.GetAllItemsFromSet($"{tagRecurringJob}s").Contains(JobId);
            }
        }

        public static bool JobNaoEstaAgendado(string JobId) => !JobEstaAgendado(JobId);

    }
}
