using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGXUnity;

namespace AGXUnity_UR_Robot.Script
{
  public static class TransformDeepChildExtension
  {
    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
      Queue<Transform> queue = new Queue<Transform>();
      queue.Enqueue(aParent);
      while (queue.Count > 0)
      {
        var c = queue.Dequeue();
        if (c.name == aName)
          return c;
        foreach (Transform t in c)
          queue.Enqueue(t);
      }
      return null;
    }
  }

  public class UR_Robot : ScriptComponent
  {
    public enum Actuator
    {
      Base,
      Shoulder,
      Elbow,
      Wrist1,
      Wrist2,
      Wrist3,
      Hand,
      NumActuators,
      Finger1,
      Finger2,
      NumConstraints,
      Fingers
    };


    /// <summary>
    /// Locate a named ScriptComponent in any child or sub-children of the component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    private T FindChild<T>(string name)
      where T : ScriptComponent
    {
      var t = TransformDeepChildExtension.FindDeepChild(transform, name);
      return t.GetComponentInChildren<T>();
    }

    public Constraint GetActuator(Actuator actuator)
    {
      if (m_constraints[(int)actuator] == null)
        m_constraints[(int)actuator] = FindChild<Constraint>(actuator.ToString());
      return m_constraints[(int)actuator];
    }

    Constraint[] m_constraints;

    protected override bool Initialize()
    {
      m_constraints = new Constraint[(int)Actuator.NumConstraints];

      return true;
    }

    // Update is called once per frame
    void Update()
    {

    }
  }
}

