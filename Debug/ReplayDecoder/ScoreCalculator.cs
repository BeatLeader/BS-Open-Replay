using ReplayDecoder.BLReplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayDecoder
{
    internal class ScoreCalculator
    {

        public static int CalculateScoreFromReplay(Replay replay)
        {
            var scoreCalculator = new ScoreCalculator();
            scoreCalculator.ProcessReplay(replay);
            return scoreCalculator.TotalScore;
        }

        private readonly MultiplierCounter _multiplierCounter = new MultiplierCounter();
        public int TotalScore { get; private set; }

        private ScoreCalculator() { }

        private void ProcessReplay(Replay replay)
        {
            var nextNoteIndex = 0;
            var nextWallIndex = 0;

            do
            {
                var hasNextNote = nextNoteIndex < replay.notes.Count;
                var hasNextWall = nextWallIndex < replay.walls.Count;
                if (!hasNextNote && !hasNextWall) return;

                var nextNote = hasNextNote ? replay.notes[nextNoteIndex] : null;
                var nextWall = hasNextWall ? replay.walls[nextWallIndex] : null;

                var notePriority = hasNextNote ? nextNote.eventTime : float.MaxValue;
                var wallPriority = hasNextWall ? nextWall.time : float.MaxValue;

                if (notePriority <= wallPriority)
                {
                    ProcessNoteEvent(nextNote);
                    nextNoteIndex += 1;
                }
                else
                {
                    ProcessWallEvent(nextWall);
                    nextWallIndex += 1;
                }
            } while (true);
        }

        private void ProcessWallEvent(WallEvent wallEvent)
        {
            _multiplierCounter.Decrease();
        }

        private void ProcessNoteEvent(NoteEvent noteEvent)
        {
            switch (noteEvent.eventType)
            {
                case NoteEventType.good:
                    ProcessGoodCut(noteEvent);
                    break;
                case NoteEventType.bad:
                case NoteEventType.miss:
                case NoteEventType.bomb:
                    ProcessBadCut(noteEvent);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessBadCut(NoteEvent noteEvent)
        {
            _multiplierCounter.Decrease();
            // Plugin.Log.Notice($"reeBad - m: {_multiplierCounter.Multiplier}");
        }

        private void ProcessGoodCut(NoteEvent noteEvent)
        {
            _multiplierCounter.Increase();
            var before = GetBeforeCutScore(noteEvent.noteCutInfo.beforeCutRating);
            var after = GetAfterCutScore(noteEvent.noteCutInfo.afterCutRating);
            var acc = GetCutDistanceScore(noteEvent.noteCutInfo.cutDistanceToCenter);
            var total = acc + before + after;
            // Plugin.Log.Notice($"reeGood - b: {before} a: {after} a: {acc} t: {total} m: {_multiplierCounter.Multiplier}");
            TotalScore += total * _multiplierCounter.Multiplier;
        }

        private class MultiplierCounter
        {
            public int Multiplier { get; private set; } = 1;

            private int _multiplierIncreaseProgress;
            private int _multiplierIncreaseMaxProgress = 2;

            public void Reset()
            {
                Multiplier = 1;
                _multiplierIncreaseProgress = 0;
                _multiplierIncreaseMaxProgress = 2;
            }

            public void Increase()
            {
                if (Multiplier >= 8) return;

                if (_multiplierIncreaseProgress < _multiplierIncreaseMaxProgress)
                {
                    ++_multiplierIncreaseProgress;
                }

                if (_multiplierIncreaseProgress >= _multiplierIncreaseMaxProgress)
                {
                    Multiplier *= 2;
                    _multiplierIncreaseProgress = 0;
                    _multiplierIncreaseMaxProgress = Multiplier * 2;
                }
            }

            public void Decrease()
            {
                if (_multiplierIncreaseProgress > 0)
                {
                    _multiplierIncreaseProgress = 0;
                }

                if (Multiplier > 1)
                {
                    Multiplier /= 2;
                    _multiplierIncreaseMaxProgress = Multiplier * 2;
                }
            }
        }

        private const float MinBeforeCutScore = 0.0f;
        private const float MinAfterCutScore = 0.0f;
        private const float MaxBeforeCutScore = 70.0f;
        private const float MaxAfterCutScore = 30.0f;
        private const float MaxCenterDistanceCutScore = 15.0f;

        public static int RoundToInt(float f) { return (int)Math.Round(f); }

        public static float Clamp01(float value)
        {
            if (value < 0F)
                return 0F;
            else if (value > 1F)
                return 1F;
            else
                return value;
        }

        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        private static int GetCutDistanceScore(float cutDistanceToCenter)
        {
            return RoundToInt(MaxCenterDistanceCutScore * (1f - Clamp01(cutDistanceToCenter / 0.3f)));
        }

        private static int GetBeforeCutScore(float beforeCutRating)
        {
            var rating = Clamp01(beforeCutRating);
            return RoundToInt(LerpUnclamped(MinBeforeCutScore, MaxBeforeCutScore, rating));
        }

        private static int GetAfterCutScore(float afterCutRating)
        {
            var rating = Clamp01(afterCutRating);
            return RoundToInt(LerpUnclamped(MinAfterCutScore, MaxAfterCutScore, rating));
        }
    }
}
