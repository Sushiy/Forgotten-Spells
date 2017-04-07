﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using gestureUtil;

namespace primitive
{
    /// <summary>
    /// Records the movements of the desired object (mouse, wand)
    /// and detects if simple primitives were drawn (big enough)
    ///
    /// TODO detect edges and count them
    /// TODO check if edges "overlapped" and see if the thing is big enough
    /// </summary>
    public class PrimitiveDetector : MonoBehaviour
    {
        [SerializeField]
        private float m_fDequeueInterval = 0.5f; // how fast are points dequeued again
        [SerializeField]
        private int m_iPointCount = 256; // how many points can the queue hold?
        [SerializeField]
        private float m_fMinimumDistance = 0.25f; // minimum distance between recorded points
        [SerializeField]
        private int m_iMinimumIndexDiff = 40; // minimum number of points of shapes (length)
        [SerializeField]
        [Range(0f, 1f)]
        private float m_fRadiusTolerance = 0.25f; // minimum radius tolerance of a circle shape


        private FixedSizeQueue<Vector3> points;
        private Vector3 lastPoint;
        private LineRenderer trail;

        //private Vector3 debug_center = Vector3.zero;

        void Awake()
        {
            points = new FixedSizeQueue<Vector3>(m_iPointCount);
            trail = GetComponent<LineRenderer>();
        }

        void Start()
        {
            StartCoroutine("ConstantlyDequeue");
        }

        void Update()
        {
            TrackPoints();
            DetectCircle();
        }

        /// <summary>
        /// Constantly dequeues points to make the trail "vanish"
        /// </summary>
        /// <returns></returns>
        IEnumerator ConstantlyDequeue()
        {
            while (true)
            {
                if (points.Count > 0)
                    points.Dequeue();
                yield return new WaitForSeconds(m_fDequeueInterval);
            }
        }

        /// <summary>
        /// Tries to detect a circle in the queue
        /// </summary>
        void DetectCircle()
        {
            // is the shape/circle "long enough" ?
            if (points.Count < m_iMinimumIndexDiff)
                return;

            Vector3[] p = points.ToArray();
            var possibleIndices = new List<int>();

            // check if lastPoint is close enough to another point to detect "crossing"
            for (int i=0; i<p.Length-1; ++i)
            {
                float distance = Vector2.Distance(p[i], lastPoint);
                if (distance < m_fMinimumDistance)
                {
                    possibleIndices.Add(i);
                }
            }

            // no crossings detected. end here.
            if (possibleIndices.Count < 1) return;
            
            // for every possible circle do the following:
            float radius = 0f;
            Vector3 center = Vector3.zero;
            bool circleFound = false;
            foreach (int index in possibleIndices)
            {
                // calc center of circle
                if (p.Length - index < m_iMinimumIndexDiff) return;
                center = Vector3.zero;
                for (int i = index; i < p.Length; ++i)
                {
                    center += p[i];
                }
                center /= p.Length - index;

                // calc radius  (point - center).Length() - radius
                radius = 0f;
                float[] radiusArr = new float[p.Length];
                for (int i = index; i < p.Length; ++i)
                {
                    radiusArr[i] = Vector3.Distance(p[i], center);
                    radius += radiusArr[i];
                }
                radius /= p.Length - index;

                // calc error
                //float avgError = 0f;
                //float radius100 = radius / 100f;
                //for (int i = index; i < p.Length; ++i)
                //{
                //    float radiusDiff = Mathf.Abs(radius - radiusArr[i]);
                //    avgError += radius100 * radiusDiff;
                //}
                //avgError /= p.Length - index;
                //avgError /= radius;
                //avgError *= 100f;

                // every radius must be roughly the same radius
                float minRadius = radius * (1 - m_fRadiusTolerance);
                float maxRadius = radius * (1 + m_fRadiusTolerance);
                circleFound = true;
                for (int i = index; i < p.Length; ++i)
                {
                    float cur = radiusArr[i];
                    if (cur > maxRadius ||
                        cur < minRadius)
                    {
                        //print("This is not a \"circle\"");
                        circleFound = false;
                        break;
                    }
                }

                if (circleFound)
                {
                    break;
                }
            }
            
            if (circleFound)
            {
                print("new circle found with radius=" + radius);
                //debug_center = center;
            }
        }

        /// <summary>
        /// Tracks the mouse and stores the points in to the queue.
        /// 
        /// TODO make it able to track a wand as well!
        /// </summary>
        void TrackPoints()
        {
            // new point
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10));

            // has the mouse moved significantly enough?
            if (Vector3.Distance(p, lastPoint) > m_fMinimumDistance)
            {
                // store the point
                lastPoint = p;
                points.Enqueue(p);
            }

            // draw the trail
            if (trail != null)
            {
                trail.positionCount = points.Count;
                trail.SetPositions(points.ToArray());
            }
        }

        //void OnDrawGizmos()
        //{
        //    Gizmos.color = new Color(1, 0, 1);
        //    Gizmos.DrawCube(debug_center, Vector3.one * 0.5f);
        //}
    }
}
