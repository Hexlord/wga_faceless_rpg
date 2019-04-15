using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Tools
{
    class AIPathFindingVoids
    {
        public static void FindSinglePath(Vector3 start, Vector3 finish, NavMeshPath path)
        {
            NavMesh.CalculatePath(start, finish, NavMesh.AllAreas, path);
        }

        public static void FindAllPaths(Vector3[] starts, Vector3[] finishes, NavMeshPath[] paths)
        {
            if (starts.Length != finishes.Length || finishes.Length != paths.Length || paths.Length != starts.Length) return; //Error
            for (int i = 0; i < starts.Length; i++)
            {
                NavMesh.CalculatePath(starts[i], finishes[i], NavMesh.AllAreas, paths[i]);
            }
        }
    }
}
