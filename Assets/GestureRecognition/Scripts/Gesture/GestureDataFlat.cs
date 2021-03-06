﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace gesture
{
    public struct GestureSpellObject
    {
        public SpellType type;
        public Vector2[] points;
    }

    [CreateAssetMenu(fileName ="dataset_", menuName = "Gesture/DataSet Flat", order = 2)]
    public class GestureDataFlat : ScriptableObject
    {
        public SpellType[] spelltypes;
        public Vector2[] gestures;

        public int samplesPerGesture = 4;
        public int pointsPerSample = 4;
        public int numberOfGestures = 10;
        public bool initialise = true;

        public void Init()
        {
            if (!initialise)
                return;

            // init the arrays
            gestures = new Vector2[samplesPerGesture * pointsPerSample * numberOfGestures];
            spelltypes = new SpellType[numberOfGestures];
            for (int i=0; i< numberOfGestures; ++i)
            {
                spelltypes[i] = SpellType.NONE;
            }

            initialise = false;
        }

        /// <summary>
        /// Creates a dataset of the struct GestureSpellObject
        /// </summary>
        /// <returns>returns true if the dataset is valid and returns false if something goes wrong</returns>
        public bool createGestureDataset(out GestureSpellObject[] dataset)
        {
            dataset = new GestureSpellObject[numberOfGestures * samplesPerGesture];

            try
            {
                // go through every sample
                int numberOfSamples = samplesPerGesture * numberOfGestures;
                int pointIndex = 0;
                int dataIndex = 0;
                for (int sampleIndex = 0; sampleIndex < numberOfSamples; ++sampleIndex)
                {
                    GestureSpellObject g = new GestureSpellObject();
                    g.type = spelltypes[sampleIndex / samplesPerGesture];
                    g.points = new Vector2[pointsPerSample];

                    for (int i = 0; i<pointsPerSample; ++i)
                    {
                        g.points[i] = gestures[pointIndex++]; // copy next point
                    }

                    dataset[dataIndex++] = g;
                }
            }
            // in case there is not every point recorded in this dataset
            // the method returns false and stops immedieatly
            // to flag the dataset that is returned not valid
            catch (System.IndexOutOfRangeException e)
            {
                Debug.LogError(e.StackTrace);
                return false;
            }

            return true;
        }

        public Vector2[] getPointsFromSample(int gestureIndex, int sampleIndex)
        {
            // check for traps
            if (gestureIndex < 0 || gestureIndex >= numberOfGestures)
            {
                Debug.LogError("The Gesture Index in this call is invalid!");
                return new Vector2[0];
            }
            if (sampleIndex < 0 || sampleIndex >= samplesPerGesture)
            {
                Debug.LogError("The Sample Index in this call is invalid!");
                return new Vector2[0];
            }

            Vector2[] result = new Vector2[pointsPerSample];
            for (int i=0; i<pointsPerSample; ++i)
            {
                result[i] = gestures[(gestureIndex * samplesPerGesture * pointsPerSample) +
                    (sampleIndex * pointsPerSample) + i];
            }
            return result;
        }
    }
}
