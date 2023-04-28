using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGXUnity;

class TransformData
{
    public TransformData(AGXUnity.RigidBody rb)
    {
        body = rb;

        var b = rb.GetInitialized<RigidBody>().Native;
        transform = b.getTransform();
        linearVelocity = b.getVelocity();
        angularVelocity = b.getAngularVelocity();
    }

    public void Apply()
    {
        var b = body.GetInitialized<RigidBody>().Native;
        b.setTransform(transform);
        b.setVelocity(linearVelocity);
        b.setAngularVelocity(angularVelocity);
    }

    public AGXUnity.RigidBody body;
    public agx.AffineMatrix4x4 transform;
    public agx.Vec3 linearVelocity;
    public agx.Vec3 angularVelocity;
};

public class PositionResetter : MonoBehaviour
{
    // this restores stores the blocks and restores them on user request
    private Dictionary<string, List<TransformData>> m_body_transforms = new Dictionary<string, List<TransformData>>();
    Vector3 TCPPos;
    Quaternion TCPRot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StoreTCPPosition(Transform TCP)
    {
        TCPPos = TCP.position;
        TCPRot = TCP.rotation;
    }
    public void RestoreTCPPosition(Transform TCP)
    {
        TCP.position = TCPPos;
        TCP.rotation = TCPRot;
    }

    public void StoreTransforms(string  name, CheckPoint checkpointType)
    {
        if (!m_body_transforms.ContainsKey(name))
        {
            m_body_transforms[name] = new List<TransformData>();
        }
        m_body_transforms[name].Clear();

        if (checkpointType == CheckPoint.Blocks || checkpointType == CheckPoint.RobotLink)
        {
            string tagName;
            if (checkpointType == CheckPoint.Blocks)
            {
                tagName = "block";
            }
            else
            {
                tagName = "RobotLink";
            }
            // the name is the key in the dictionary and is what we use to restore the transforms
            // the tagName is the type of AGX RigidBodies that we want to record. either blocks or robotLink


            //var bodies = UnityEngine.Object.FindObjectsOfType<AGXUnity.RigidBody>();
            //here find objects that YOU tag or 
            var bodies = GameObject.FindGameObjectsWithTag(tagName);
            foreach (var body in bodies)
            {
                AGXUnity.RigidBody b = body.gameObject.GetComponent<AGXUnity.RigidBody>();
                if (!(b.hideFlags == HideFlags.NotEditable || b.hideFlags == HideFlags.HideAndDontSave))
                    m_body_transforms[name].Add(new TransformData(b));
            }
        }
        else if(checkpointType == CheckPoint.All)
        {
            // store everything
            var links = GameObject.FindGameObjectsWithTag("RobotLink");
            var blocks = GameObject.FindGameObjectsWithTag("block");
            //var fingers = GameObject.FindGameObjectsWithTag("finger");

            foreach (var link in links)
            {
                AGXUnity.RigidBody b = link.gameObject.GetComponent<AGXUnity.RigidBody>();
                //if (!(b.hideFlags == HideFlags.NotEditable || b.hideFlags == HideFlags.HideAndDontSave))
                    m_body_transforms[name].Add(new TransformData(b));
            }
            foreach (var block in blocks)
            {
                AGXUnity.RigidBody b = block.gameObject.GetComponent<AGXUnity.RigidBody>();
                //if (!(b.hideFlags == HideFlags.NotEditable || b.hideFlags == HideFlags.HideAndDontSave))
                    m_body_transforms[name].Add(new TransformData(b));
            }
            //foreach (var finger in fingers)
            //{
            //    AGXUnity.RigidBody b = finger.gameObject.GetComponent<AGXUnity.RigidBody>();
            //    if (!(b.hideFlags == HideFlags.NotEditable || b.hideFlags == HideFlags.HideAndDontSave))
            //        m_body_transforms[name].Add(new TransformData(b));
            //}
        }
        
    }

    public void RestoreTransforms(string name)
    {
        if (!m_body_transforms.ContainsKey(name))
            return;

        var transforms = m_body_transforms[name];
        foreach (var t in transforms)
        {
            t.Apply();
        }
    }
}
