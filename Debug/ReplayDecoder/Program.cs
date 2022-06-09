using ReplayDecoder.BLReplay;
using System;
using System.IO;
using System.Windows.Forms;

namespace ReplayDecoder
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "BS Open Replay |*.bsor";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            Stream stream = File.Open(ofd.FileName, FileMode.Open);
            byte[] replayData;

            DateTime start = DateTime.Now;

            for (int i = 0; i < 10000; i++)
            {
                using (var ms = new MemoryStream(5))
                {
                    stream.CopyTo(ms);
                    long length = ms.Length;
                    replayData = ms.ToArray();
                }
            }

            DateTime end = DateTime.Now;

            DateTime start2 = DateTime.Now;

            for (int i = 0; i < 10000; i++)
            {
                using (var ms = new MemoryStream(5))
                {
                    stream.CopyTo(ms);
                    long length = ms.Length;
                    replayData = ms.ToArray();
                }
            }

            DateTime end2 = DateTime.Now;

            DateTime start3 = DateTime.Now;

            for (int i = 0; i < 10000; i++)
            {
                using (var ms = new MemoryStream(5))
                {
                    stream.CopyTo(ms);
                    long length = ms.Length;
                    replayData = ms.ToArray();
                }
            }

            DateTime end3 = DateTime.Now;
            Console.WriteLine(" {0} ms | {1} ms | {2} ms", (end - start).TotalMilliseconds, (end2 - start2).TotalMilliseconds, (end3 - start3).TotalMilliseconds);

            int arrayLength = (int)stream.Length;
            byte[] buffer = new byte[arrayLength];
            int file = stream.Read(buffer, 0, arrayLength);

            Replay replay = BLReplay.ReplayDecoder.Decode(buffer);

            int score1 = replay.info.score;
            int score2 = ScoreCalculator.CalculateScoreFromReplay(replay);
            ScoreStatistic statistic = ReplayStatisticUtils.ProcessReplay(replay);
            int score3 = statistic.WinTracker.TotalScore;

            var velocityStats = Bakery.VelocityStats(replay);

            // For breakpoint
            Console.WriteLine(score1 + "  " + score2 + "  " + score3);
        }
    }
}
