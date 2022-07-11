using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ReplayDecoder.BLReplay;
using System;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using NumSharp;
using System.Collections.Generic;

namespace ReplayDecoder
{
    static class Program
    {
        [STAThread]
        async static Task Main(string[] args)
        {

            string[] files =
            Directory.GetFiles("\\replays", "*.bsor", SearchOption.AllDirectories);

            (List<List<List<float>>> xs, List<float> ys) = (new List<List<List<float>>>(), new List<float>());
            

            for (int f = 0; f < files.Length; f++)
            {
                try {
                Stream stream = File.Open(files[f], FileMode.Open);
                byte[] replayData;

                int arrayLength = (int)stream.Length;
                byte[] buffer = new byte[arrayLength];
                int file = stream.Read(buffer, 0, arrayLength);

                Replay replay = BLReplay.ReplayDecoder.Decode(buffer);
                ScoreStatistic statistic = ReplayStatisticUtils.ProcessReplay(replay);

                

                (var xxs, var yys) = statistic.Groups;

                xs.AddRange(xxs);
                ys.AddRange(yys);
                } catch { }

                
            }

            var x = np.zeros(new Shape(xs.Count, 13, 13, 1));
            var y = np.zeros(new Shape(xs.Count, 1));

            for (int i = 0; i < xs.Count; i++)
            {
                for (int j = 0; j < 13; j++)
                {

                    for (int z = 0; z < 13; z++)
                    {
                        if (j < xs[i].Count) {

                            x[i][j][z][0] = xs[i][j][z];
                        } else {
                            x[i][j][z][0] = 0;
                        }
                    }
                }

                y[i][0] = ys[i];
            }

            xs = null;
            ys = null;

            np.save("\\replays\\xs.npy", x);
            np.save("\\replays\\ys.npy", y);
        }
    }
}
