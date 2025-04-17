using System.Numerics;
using System.Text.Json;


namespace TestRunner
{
    public class JobManager
    {
        public class JobCheckpoint
        {
            public Guid JobId { get; set; }
            public int ThreadIndex { get; set; }
            public int JobIndex { get; set; }
            public string Start { get; set; } = "0";
            public string End { get; set; } = "0";
            public string LastProcessed { get; set; } = "0"; // last completed prime to allow resumption of jobs.
            public bool Completed { get; set; }
            public DateTime? JobStartDate { get; internal set; }
            public DateTime? LastRunDate { get; internal set; }
            public DateTime? CompletionDate { get; internal set; }
            public DateTime LastUpdated { get; internal set; }
            public JobCheckpoint()
            {
                JobId = Guid.NewGuid();
            }
        }


        public static BigInteger GetJobStart(int j, int thread, BigInteger jobStartPrime, BigInteger jobEndPrime, string checkpointFile)
        {
            var checkpoints = LoadCheckpoint(checkpointFile);

            var checkpoint = checkpoints.FirstOrDefault();
            if (checkpoint == null)
            {
                checkpoint = new JobCheckpoint
                {
                    ThreadIndex = thread,
                    JobIndex = j,
                    Start = jobStartPrime.ToString(),
                    End = jobEndPrime.ToString(),
                    LastProcessed = "0",
                    Completed = false,
                    JobStartDate = DateTime.Now,
                    LastRunDate = DateTime.Now,
                };
                checkpoints.Add(checkpoint);

            }


            if (checkpoint.Completed == true)
            {
                Console.WriteLine($"Skipping completed job {j} ({jobStartPrime} - {jobEndPrime})");
                return BigInteger.Parse(checkpoint.End);
            }
            if (checkpoint.JobStartDate == null || checkpoint.JobStartDate==DateTime.MinValue)
            {
                checkpoint.JobStartDate = DateTime.Now;
            }

            checkpoint.LastRunDate = DateTime.Now;
            SaveCheckpoint(checkpointFile, checkpoints);

            var lastProcessed = BigInteger.Parse(checkpoint.LastProcessed);
            var resumeFrom = lastProcessed > 0
                ? lastProcessed + 1
                : (ulong)jobStartPrime;
            return resumeFrom;
        }

        static void SaveCheckpoint(string fileName, List<JobCheckpoint> jobs)
        {
            var di = Directory.CreateDirectory(Path.Combine(".", "checkpoints"));
            File.WriteAllText(Path.Combine(di.FullName, fileName), JsonSerializer.Serialize(jobs, new JsonSerializerOptions { WriteIndented = true }));
        }

        static List<JobCheckpoint> LoadCheckpoint(string fileName)
        {
            var di = Directory.CreateDirectory(Path.Combine(".", "checkpoints"));
            var path = Path.Combine(di.FullName, fileName);
            if (!File.Exists(path)) return new List<JobCheckpoint>();
            return JsonSerializer.Deserialize<List<JobCheckpoint>>(File.ReadAllText(path))!;
        }

        internal static void Update(string checkpointFile, BigInteger lastPrime, bool completed = false)
        {
            var checkpoints = LoadCheckpoint(checkpointFile);
            var entry = checkpoints.FirstOrDefault();
        
            if (entry != null)
            {
                if(entry.JobStartDate == null || entry.JobStartDate == DateTime.MinValue)
                {
                    entry.JobStartDate = DateTime.Now;
                }
                entry.LastRunDate = DateTime.Now;
                entry.LastProcessed = lastPrime.ToString();
                entry.LastUpdated = DateTime.Now;
                if (completed)
                {
                    entry.CompletionDate = DateTime.Now;
                    entry.Completed = true;
                }
            }
            SaveCheckpoint(checkpointFile, checkpoints);
        }


    }
}
