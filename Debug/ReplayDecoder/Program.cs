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

            int arrayLength = (int)stream.Length;
            byte[] buffer = new byte[arrayLength];
            int file = stream.Read(buffer, 0, arrayLength);

            Replay replay = BLReplay.ReplayDecoder.Decode(buffer);

            int score1 = replay.info.score;
            int score2 = ScoreCalculator.CalculateScoreFromReplay(replay);
            ScoreStatistic statistic = ReplayStatisticUtils.ProcessReplay(replay);
            int score3 = statistic.WinTracker.TotalScore;

            // For breakpoint
            Console.WriteLine(score1 + "  " + score2 + "  " + score3);
        }
    }
}
