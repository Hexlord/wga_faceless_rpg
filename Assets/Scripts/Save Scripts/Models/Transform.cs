﻿
namespace CI.QuickSave.Core.Models
{
    [System.Serializable]
    public class Transform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public UnityEngine.Transform ToUnityType()
        {
            UnityEngine.GameObject dummy = new UnityEngine.GameObject();

            dummy.transform.position = position.ToUnityType();
            dummy.transform.rotation = rotation.ToUnityType();
            dummy.transform.localScale = scale.ToUnityType();
            UnityEngine.Transform transform = dummy.transform;
            UnityEngine.Object.Destroy(dummy);
            return dummy.transform;
        }

        public static Transform FromUnityType(UnityEngine.Transform transform)
        {
            Transform serializableTransform = new Transform();

            serializableTransform.position = new Vector3();
            serializableTransform.rotation = new Quaternion();
            serializableTransform.scale = new Vector3();

            serializableTransform.position.x = transform.position.x;
            serializableTransform.position.y = transform.position.y;
            serializableTransform.position.z = transform.position.z;

            serializableTransform.rotation.x = transform.rotation.x;
            serializableTransform.rotation.y = transform.rotation.y;
            serializableTransform.rotation.z = transform.rotation.z;
            serializableTransform.rotation.w = transform.rotation.w;

            serializableTransform.scale.x = transform.localScale.x;
            serializableTransform.scale.y = transform.localScale.y;
            serializableTransform.scale.z = transform.localScale.z;

            return serializableTransform;
        }
    }
}
