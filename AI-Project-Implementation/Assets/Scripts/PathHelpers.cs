using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHelpers : MonoBehaviour
{
    public static List<Vector3> CondensePathPoints(List<Vector3Int> argPath, float argMaxJumpDistance, float argCollisionRadiusCheck, LayerMask argCollisionMask)
    {
        if (argPath == null) return null;
        if (argPath.Count == 0) return null;
        List<Vector3> condensedPath = new List<Vector3>();
        float maxJumpDistanceSqrd = argMaxJumpDistance * argMaxJumpDistance;

        condensedPath.Add(argPath[0]);

        // Start from index 2, this is because 0 has already been added
        // The test work one point ahead meaning if the path to index 2 is blocked
        // it will add 2 - 1
        int currentIndex = 2;
        while (currentIndex < argPath.Count)
        {
            Vector3Int nextPoint = argPath[currentIndex];
            Vector3Int prevPoint = argPath[currentIndex - 1];
            if (Vector3.SqrMagnitude(condensedPath[condensedPath.Count -1] - nextPoint) > maxJumpDistanceSqrd || PathBlocked(condensedPath[condensedPath.Count - 1], nextPoint, argCollisionRadiusCheck, argCollisionMask)) condensedPath.Add(prevPoint);
            else currentIndex++;

            if (currentIndex == argPath.Count - 1)
                condensedPath.Add(nextPoint);
        }

        return condensedPath;
    }

    public static bool PathBlocked(Vector3 argFrom, Vector3 argTo, float argCollisionRadiusCheck, LayerMask argCollisionMask)
    {
        return Physics.SphereCast(new Ray(argFrom, (argTo - argFrom).normalized), argCollisionRadiusCheck, (argTo - argFrom).magnitude, argCollisionMask);
    }

    public static int FindFurthestVisiblePointIndex(List<Vector3> argPoints, Vector3 argFrom, LayerMask argCollisionMask)
    {
        int furthestVisibleIndex = 0;
        for (int i = 0; i < argPoints.Count; i++)
        {
            if (Physics.Raycast(new Ray(argFrom, (argPoints[i] - argFrom).normalized), (argPoints[i] - argFrom).magnitude, argCollisionMask)) continue;
            furthestVisibleIndex = i;
        }
        return furthestVisibleIndex;
    }
}
