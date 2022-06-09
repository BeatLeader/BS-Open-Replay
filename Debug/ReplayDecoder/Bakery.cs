using System.Linq;
using ReplayDecoder.BLReplay;

public static class Bakery {
    #region Normalization

    private const float LinearVelocityLimit = 10.0f; //Meters per second
    private const float AngularVelocityLimit = 360.0f * 14; //Degrees per second
    private const float WristRollAngularVelocityLimit = 360.0f * 5; //Degrees per second

    //private static Color NormalizeIntoColor(float value, float minLimit, float maxLimit) {
    //    var normalized = Mathf.Clamp01((value - minLimit) / (maxLimit - minLimit));
    //    return new Color(normalized, normalized, normalized, 1);
    //}

    public static float Clamp01(float value)
    {
        if (value < 0.0) return 0.0f;
        return value > 1.0f ? 1.0f : value;
    }

    private static float NormalizeIntoFloat(float value, float minLimit, float maxLimit)
    {
        return Clamp01((value - minLimit) / (maxLimit - minLimit));
    }

    #endregion

    #region BakeReplay

    private const int TextureHeight = 10;

    //public static Texture2D BakeReplay(Replay replay, int textureWidth) {
    //    var texture = new Texture2D(textureWidth, TextureHeight, TextureFormat.RGBAFloat, false) {
    //        wrapMode = TextureWrapMode.Clamp,
    //        filterMode = FilterMode.Point
    //    };

    //    var firstEventTime = replay.frames.First().time;
    //    var lastEventTime = replay.frames.Last().time;
    //    var lastFrameIndex = replay.frames.Count - 1;

    //    var frameIndex = 1;
    //    var previousMovementData = new MovementData();

    //    for (var x = -1; x < textureWidth; x++) {
    //        var normalizedSongTime = (float) (x + 1) / textureWidth;
    //        var targetSongTime = firstEventTime + (lastEventTime - firstEventTime) * normalizedSongTime;

    //        for (; frameIndex < lastFrameIndex; frameIndex++) {
    //            if (replay.frames[frameIndex].time > targetSongTime) break;
    //        }

    //        var currentMovementData = LerpFrames(replay.frames[frameIndex - 1], replay.frames[frameIndex], targetSongTime);

    //        if (x >= 0) {
    //            WritePixels(texture, x, previousMovementData, currentMovementData);
    //        }

    //        previousMovementData = currentMovementData;
    //    }

    //    texture.Apply();
    //    return texture;
    //}

    //private static void WritePixels(Texture2D texture, int x, MovementData previousMovementData, MovementData currentMovementData) {
    //    CalculateVelocities(previousMovementData, currentMovementData,
    //        out var leftHandLinearVelocity,
    //        out var leftHandAngularVelocity,
    //        out var leftHandWristRollingVelocity,
    //        out var rightHandLinearVelocity,
    //        out var rightHandAngularVelocity,
    //        out var rightHandWristRollingVelocity
    //    );

    //    texture.SetPixel(x, 0, NormalizeIntoColor(leftHandLinearVelocity.x, -LinearVelocityLimit, LinearVelocityLimit));
    //    texture.SetPixel(x, 1, NormalizeIntoColor(leftHandLinearVelocity.y, -LinearVelocityLimit, LinearVelocityLimit));
    //    texture.SetPixel(x, 2, NormalizeIntoColor(leftHandLinearVelocity.z, -LinearVelocityLimit, LinearVelocityLimit));
    //    texture.SetPixel(x, 3, NormalizeIntoColor(leftHandAngularVelocity, 0, AngularVelocityLimit));
    //    texture.SetPixel(x, 4, NormalizeIntoColor(leftHandWristRollingVelocity, 0, WristRollAngularVelocityLimit));

    //    texture.SetPixel(x, 5, NormalizeIntoColor(rightHandLinearVelocity.x, -LinearVelocityLimit, LinearVelocityLimit));
    //    texture.SetPixel(x, 6, NormalizeIntoColor(rightHandLinearVelocity.y, -LinearVelocityLimit, LinearVelocityLimit));
    //    texture.SetPixel(x, 7, NormalizeIntoColor(rightHandLinearVelocity.z, -LinearVelocityLimit, LinearVelocityLimit));
    //    texture.SetPixel(x, 8, NormalizeIntoColor(rightHandAngularVelocity, 0, AngularVelocityLimit));
    //    texture.SetPixel(x, 9, NormalizeIntoColor(rightHandWristRollingVelocity, 0, WristRollAngularVelocityLimit));
    //}

    public class ReplayStats
    {
        public float leftHandLinearVelocity;
        public float leftHandAngularVelocity;
        public float leftHandWristRollingVelocity;
        public float rightHandLinearVelocity;
        public float rightHandAngularVelocity;
        public float rightHandWristRollingVelocity;

        public float maxLeftHandLinearVelocity;
        public float maxLeftHandAngularVelocity;
        public float maxLeftHandWristRollingVelocity;
        public float maxRightHandLinearVelocity;
        public float maxRightHandAngularVelocity;
        public float maxRightHandWristRollingVelocity;
    }

    public static ReplayStats VelocityStats(Replay replay)
    {
        int textureWidth = (int)replay.frames.Last().time;
        var firstEventTime = replay.frames.First().time;
        var lastEventTime = replay.frames.Last().time;
        var lastFrameIndex = replay.frames.Count - 1;

        var frameIndex = 1;
        var previousMovementData = new MovementData();
        var result = new ReplayStats();

        for (var x = -1; x < textureWidth; x++)
        {
            var normalizedSongTime = (float)(x + 1) / textureWidth;
            var targetSongTime = firstEventTime + (lastEventTime - firstEventTime) * normalizedSongTime;

            for (; frameIndex < lastFrameIndex; frameIndex++)
            {
                if (replay.frames[frameIndex].time > targetSongTime) break;
            }

            var currentMovementData = LerpFrames(replay.frames[frameIndex - 1], replay.frames[frameIndex], targetSongTime);

            if (x >= 0)
            {
                WritePixels(result, previousMovementData, currentMovementData);
            }

            previousMovementData = currentMovementData;
        }

        return result;
    }

    private static void WritePixels(ReplayStats stats, MovementData previousMovementData, MovementData currentMovementData)
    {
        CalculateVelocities(previousMovementData, currentMovementData,
            out var leftHandLinearVelocity,
            out var leftHandAngularVelocity,
            out var leftHandWristRollingVelocity,
            out var rightHandLinearVelocity,
            out var rightHandAngularVelocity,
            out var rightHandWristRollingVelocity
        );

        float avgLeftHandLinearVelocity = 
            (NormalizeIntoFloat(leftHandLinearVelocity.x, -LinearVelocityLimit, LinearVelocityLimit) +
            NormalizeIntoFloat(leftHandLinearVelocity.y, -LinearVelocityLimit, LinearVelocityLimit) +
            NormalizeIntoFloat(leftHandLinearVelocity.z, -LinearVelocityLimit, LinearVelocityLimit)) / 3.0f;

        float avgLeftHandAngularVelocity = NormalizeIntoFloat(leftHandAngularVelocity, 0, AngularVelocityLimit);
        float avgLeftHandWristRollingVelocity = NormalizeIntoFloat(leftHandWristRollingVelocity, 0, WristRollAngularVelocityLimit);

        stats.leftHandLinearVelocity += avgLeftHandLinearVelocity;
        stats.leftHandAngularVelocity += avgLeftHandAngularVelocity;
        stats.leftHandWristRollingVelocity += avgLeftHandWristRollingVelocity;

        if (avgLeftHandLinearVelocity > stats.maxLeftHandLinearVelocity) { stats.maxLeftHandLinearVelocity = avgLeftHandLinearVelocity; }
        if (avgLeftHandAngularVelocity > stats.maxLeftHandAngularVelocity) { stats.maxLeftHandAngularVelocity = avgLeftHandAngularVelocity; }
        if (avgLeftHandWristRollingVelocity > stats.maxLeftHandWristRollingVelocity) { stats.maxLeftHandWristRollingVelocity = avgLeftHandWristRollingVelocity; }

        float avgRightHandLinearVelocity =
            (NormalizeIntoFloat(rightHandLinearVelocity.x, -LinearVelocityLimit, LinearVelocityLimit) +
            NormalizeIntoFloat(rightHandLinearVelocity.y, -LinearVelocityLimit, LinearVelocityLimit) +
            NormalizeIntoFloat(rightHandLinearVelocity.z, -LinearVelocityLimit, LinearVelocityLimit)) / 3.0f;
        float avgRightHandAngularVelocity = NormalizeIntoFloat(rightHandAngularVelocity, 0, AngularVelocityLimit);
        float avgRightHandWristRollingVelocity = NormalizeIntoFloat(rightHandWristRollingVelocity, 0, WristRollAngularVelocityLimit);

        stats.rightHandLinearVelocity += avgRightHandLinearVelocity;
        stats.rightHandAngularVelocity += avgRightHandAngularVelocity;
        stats.rightHandWristRollingVelocity += avgRightHandWristRollingVelocity;

        if (avgRightHandLinearVelocity > stats.maxRightHandLinearVelocity) { stats.maxRightHandLinearVelocity = avgRightHandLinearVelocity; }
        if (avgRightHandAngularVelocity > stats.maxRightHandAngularVelocity) { stats.maxRightHandAngularVelocity = avgRightHandAngularVelocity; }
        if (avgRightHandWristRollingVelocity > stats.maxRightHandWristRollingVelocity) { stats.maxRightHandWristRollingVelocity = avgRightHandWristRollingVelocity; }
    }

    #endregion

    #region CalculateVelocities

    private static void CalculateVelocities(MovementData from, MovementData to,
        out Vector3 leftHandLinearVelocity,
        out float leftHandAngularVelocity,
        out float leftHandWristRollingVelocity,
        out Vector3 rightHandLinearVelocity,
        out float rightHandAngularVelocity,
        out float rightHandWristRollingVelocity
    ) {
        var songDeltaTime = to.songTime - from.songTime;

        leftHandLinearVelocity = CalculateLinearVelocity(from.leftPosition, to.leftPosition, songDeltaTime);
        leftHandAngularVelocity = CalculateAngularVelocity(from.leftRotation, to.leftRotation, songDeltaTime);
        leftHandWristRollingVelocity = CalculateWristRollingVelocity(from.leftRotation, to.leftRotation, songDeltaTime);

        rightHandLinearVelocity = CalculateLinearVelocity(from.rightPosition, to.rightPosition, songDeltaTime);
        rightHandAngularVelocity = CalculateAngularVelocity(from.rightRotation, to.rightRotation, songDeltaTime);
        rightHandWristRollingVelocity = CalculateWristRollingVelocity(from.rightRotation, to.rightRotation, songDeltaTime);
    }

    private static Vector3 CalculateLinearVelocity(Vector3 from, Vector3 to, float deltaTime) {
        return (to - from) / deltaTime;
    }

    private static float CalculateAngularVelocity(Quaternion from, Quaternion to, float deltaTime) {
        var a = from * Vector3.forward;
        var b = to * Vector3.forward;
        return Vector3.Angle(a, b) / deltaTime; //UnityEngine.Vector3.Angle returns DEGREES! may differ from other libs
    }

    private static float CalculateWristRollingVelocity(Quaternion from, Quaternion to, float deltaTime) {
        var fromForward = from * Vector3.forward;
        var toForward = to * Vector3.forward;

        var alignRotation = Quaternion.FromToRotation(fromForward, toForward);
        var fromAligned = alignRotation * from;

        var a = fromAligned * Vector3.left;
        var b = to * Vector3.left;
        return Vector3.Angle(a, b) / deltaTime; //UnityEngine.Vector3.Angle returns DEGREES! may differ from other libs
    }

    #endregion

    #region MovementData -- UTILITY

    private struct MovementData {
        public Vector3 leftPosition;
        public Quaternion leftRotation;
        public Vector3 rightPosition;
        public Quaternion rightRotation;
        public float songTime;
    }

    #endregion

    #region LerpFrames -- UTILITY

    private static MovementData LerpFrames(Frame from, Frame to, float songTime) {
        var t = (songTime - from.time) / (to.time - from.time);

        return new MovementData {
            leftPosition = LerpGolovaVector3(from.leftHand.position, to.leftHand.position, t),
            leftRotation = LerpGolovaQuaternion(from.leftHand.rotation, to.leftHand.rotation, t),
            rightPosition = LerpGolovaVector3(from.rightHand.position, to.rightHand.position, t),
            rightRotation = LerpGolovaQuaternion(from.rightHand.rotation, to.rightHand.rotation, t),
            songTime = songTime
        };
    }

    private static Vector3 LerpGolovaVector3(Vector3 from, Vector3 to, float t) {
        return Vector3.Lerp(from, to, t);
    }

    private static Quaternion LerpGolovaQuaternion(Quaternion from, Quaternion to, float t) {
        return Quaternion.Lerp(from, to, t);
    }

    #endregion
}