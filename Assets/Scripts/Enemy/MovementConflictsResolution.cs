using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace a
{
    public class MovementConflictsResolution
    {
        private readonly NavigationSystem navigationHost;
        private readonly float checkingDistance;
        private Dictionary<BaseAgent, Vector2> AgentDirection;
        private List<BaseAgent> AgentProcessingPriority;
        //private Queue<MovementConflict> ConflictQueue;
        private BaseAgent processAgent;
        private BaseAgent meetingAgent;
        private Vector3 rayCastingPoint;
        private float checkingAngle = 0.0f;
        private float deltaCheckingAngle;
        private Ray conflictResolutionThickRay;
        private RaycastHit hit;
        private Vector3 intendedDirection;
        private Vector3 resultDirection;

        public MovementConflictsResolution(NavigationSystem ns, float dist, IEnumerable<BaseAgent> agents)
        {
            navigationHost = ns;
            checkingDistance = dist;
            AgentProcessingPriority = new List<BaseAgent>(agents);
            AgentDirection = new Dictionary<BaseAgent, Vector2>();
        }

        public void UpdateAgentList(BaseAgent agent)
        {
            AgentProcessingPriority.Add(agent);
            AgentProcessingPriority.Sort(ComparePriority);
        }
        
        private static int ComparePriority(BaseAgent x, BaseAgent y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (y == null)
                    return 1;
                else
                {
                    if (x.Priority > y.Priority)
                        return 1;
                    else if (x.Priority == y.Priority)
                        return 0;
                    else
                        return -1;
                }
            }
        }

        private void CheckConflicts()
        {
            AgentProcessingPriority.Sort(ComparePriority);
            checkingAngle = 0.0f;
            for (int i = 0; i < AgentProcessingPriority.Count; i++)
            {
                processAgent = AgentProcessingPriority[i];
                intendedDirection = navigationHost.GetIntendedDirection(processAgent.ID);
                resultDirection = intendedDirection;
                if (intendedDirection != Vector3.zero)
                {
                    rayCastingPoint = processAgent.transform.position + Vector3.up;
                    conflictResolutionThickRay = new Ray(rayCastingPoint, resultDirection);
                    if (Physics.SphereCast(conflictResolutionThickRay, processAgent.Radius, out hit,
                        checkingDistance,
                        LayerMask.GetMask("Environment", "Character"),
                        QueryTriggerInteraction.Ignore))
                    {
                        meetingAgent = null;
                        GameObject gm = hit.collider.gameObject.TraverseParent("Faceless");
                        Debug.Log(!hit.collider.CompareTag("Player"));
                        if (!hit.collider.CompareTag("Player") && gm != null) 
                            meetingAgent = gm.GetComponent<BaseAgent>();
                        if (meetingAgent != null)
                        {
                            if (meetingAgent.Priority < processAgent.Priority)
                            {
                                resultDirection = intendedDirection;
                                if (navigationHost.GetIntendedDirection(meetingAgent.ID) != Vector3.zero)
                                {
                                    var normal = Vector3.Cross(intendedDirection, Vector3.up);
                                    var normal2D = new Vector2(normal.x, normal.z);
                                    if (!AgentDirection.ContainsKey(meetingAgent))
                                    {
                                        
                                        AgentDirection.Add(meetingAgent, normal2D);
                                    }
                                    else
                                    {
                                        AgentDirection[meetingAgent] += normal2D;
                                    }
                                }
                            }
                            else
                            {
                                if (navigationHost.GetIntendedDirection(meetingAgent.ID) == Vector3.zero)
                                {
                                    resultDirection = FindDetour(processAgent);
                                }
                                else
                                {
                                    resultDirection = Vector3.zero;
                                }
                            }
                        }
                    }
                    if (!AgentDirection.ContainsKey(processAgent))
                        AgentDirection.Add(processAgent, new Vector2(resultDirection.x, resultDirection.z));
                    else
                    {
                        AgentDirection[processAgent] = new Vector2(resultDirection.x, resultDirection.z);
                    }
                }
                else
                {
                    AgentDirection.Add(processAgent, Vector2.zero);
                }
            }
            
        }

        private Vector3 FindDetour(BaseAgent agent)
        {
            //conflict.higherPriority;
            rayCastingPoint = agent.transform.position + Vector3.up;
            deltaCheckingAngle = 2 * (Mathf.Atan(processAgent.Radius / checkingDistance));
            conflictResolutionThickRay = new Ray(rayCastingPoint, resultDirection);
            
            while (Physics.SphereCast(conflictResolutionThickRay, processAgent.Radius, out hit,
                checkingDistance,
                LayerMask.GetMask("Environment", "Character"),
                QueryTriggerInteraction.Ignore))
            {
                if (checkingAngle < -180)
                {
                    resultDirection = Vector3.zero;
                    break;
                }

                checkingAngle = (Mathf.Sign(checkingAngle) >= 0)
                    ? checkingAngle + deltaCheckingAngle
                    : -checkingAngle;
                    
                resultDirection = Quaternion.AngleAxis(checkingAngle, Vector3.up) * intendedDirection;
                conflictResolutionThickRay = new Ray(rayCastingPoint, resultDirection);
            }
            
            return resultDirection;
        }
        
        public Dictionary<BaseAgent, Vector2> ResolveMovementConflicts()
        {
            AgentDirection.Clear();
            CheckConflicts();
            return AgentDirection;
        }
    }
}
