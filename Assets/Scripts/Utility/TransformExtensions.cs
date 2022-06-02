using UnityEngine;

// Class that adds some extension methods to the Transform class
public static class TransformExtensions {
    // Allows setting the global scale of the Transform
    public static void SetGlobalScale (this Transform t, Vector3 globalScale) {
        t.localScale = Vector3.one;
        t.localScale = new Vector3(globalScale.x / t.lossyScale.x, globalScale.y / t.lossyScale.y, globalScale.z / t.lossyScale.z);
    }

    // Converts a Transform into a Pose
    public static void GetPose(this Transform t, out Pose p) {
        p.position = t.position;
        p.rotation = t.rotation;
    }
    public static Pose GetPose(this Transform t) {
        t.GetPose(out Pose p); return p;
    }

    // Converts a Pose into a Transform
    public static void CopyFrom(this Transform t, Pose p) {
        t.position = p.position;
        t.rotation = p.rotation;
    }
}
