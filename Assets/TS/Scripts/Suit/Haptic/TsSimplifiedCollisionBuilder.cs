using System;
using System.Collections.Generic;
using TsApi;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[ExecuteInEditMode]
public class TsSimplifiedCollisionBuilder : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 0.99f)]
    private float m_boneWeightThreshold = 0.45f;

    private SkinnedMeshRenderer meshRenderer;

    [SerializeField]
    private TsAvatarSettings m_avatarSettings;

    [SerializeField]
    private TsHapticSimplifiedChannel[] channels;

    [SerializeField]
    private TsHapticPlayer m_hapticPlayer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();

    }

    private bool HasActiveBoneWeight(BoneWeight bw, int transformIndex)
    {
        float threshold = m_boneWeightThreshold;
        if (bw.boneIndex0 == transformIndex && bw.weight0 > threshold)
        {
            return true;
        }

        if (bw.boneIndex1 == transformIndex && bw.weight1 > threshold)
        {
            return true;
        }

        if (bw.boneIndex2 == transformIndex && bw.weight2 > threshold)
        {
            return true;
        }

        if (bw.boneIndex3 == transformIndex && bw.weight3 > threshold)
        {
            return true;
        }
        return false;
    }

    

    void Build()
    {
        var boneWeights = meshRenderer.sharedMesh.boneWeights;
        var vertices = meshRenderer.sharedMesh.vertices;

        var existing = meshRenderer.rootBone.GetComponentsInChildren<TsHapticCollisionHandler>();

        foreach (var channel in existing)
        {
            DestroyImmediate(channel.gameObject);
        }

        foreach(var channel in channels)
        {
            var transformName = m_avatarSettings.GetTransformName(channel.BoneIndex);
            var transformIndex = GetTransformIndex(transformName);
            if (transformIndex == -1)
            {
                Debug.LogWarning($"Failed to get transform index with name: {transformName}");
                continue;
            }
            var boneTransform = meshRenderer.bones[transformIndex];
            var child = boneTransform.GetChild(0);

            Vector3 pointA = boneTransform.position;
            Vector3 pointB = child.position;
            
            Vector3 pointC = (pointA + pointB)/2 + TsAnimatorBones.GetSidePlanePoint(channel.BoneIndex);

            Plane plane = new Plane(pointA, pointB, pointC);

            List<Vector3> points = new List<Vector3>();
            var poseRotation = m_avatarSettings.GetIPoseRotation(channel.BoneIndex);
            for(int i=0; i<boneWeights.Length; ++i)
            {
                var bw = boneWeights[i];
                var v = vertices[i];

                var delta = Quaternion.Inverse(boneTransform.rotation) * poseRotation;
                var globalV = delta * boneTransform.InverseTransformPoint(v);
                if(HasActiveBoneWeight(bw, transformIndex))
                {
                    if (plane.GetSide(v) == (channel.BoneSide == TsBone2dSide.Front))
                    {
                        points.Add(globalV);
                    }
                }
            }

            if (points.Count < 4)
            {
                continue;
            }
            var calculator = new GK.ConvexHullCalculator();
            List<Vector3> newVerts = new List<Vector3>();
            List<int> newTriangles = new List<int>();
            List<Vector3> newNormals = new List<Vector3>();
            calculator.GenerateHull(points, true, ref newVerts, ref newTriangles, ref newNormals);

            var newMesh = new Mesh();
            newMesh.SetVertices(newVerts);
            newMesh.SetTriangles(newTriangles, 0);
            newMesh.SetNormals(newNormals);
            
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals();
            var channeObj = new GameObject(channel.name);
            channeObj.transform.position = boneTransform.position;
            channeObj.transform.rotation = boneTransform.rotation;
            channeObj.transform.SetParent(boneTransform);
            var channelCollider = channeObj.AddComponent<MeshCollider>();
            channelCollider.sharedMesh = newMesh;
            channelCollider.convex = true;
            var handler = channeObj.AddComponent<TsHapticCollisionHandler>();
            handler.Channel = channel;
            handler.HapticPlayer = m_hapticPlayer;
        }
    }

    private int GetTransformIndex(string transformName)
    {
        for (int i = 0; i < meshRenderer.bones.Length; ++i)
        {
            if (meshRenderer.bones[i].name == transformName)
            {
                return i;
            }
        }
        return -1;
    }

    public bool build = false;

    // Update is called once per frame
    void Update()
    {
        if(build)
        {
            try
            {
                Build();
            }
            catch (Exception)
            {
            }
            
            build = false;
        }
    }
}
