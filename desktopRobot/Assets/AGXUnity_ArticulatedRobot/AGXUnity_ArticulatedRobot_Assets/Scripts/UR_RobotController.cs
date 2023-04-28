using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGXUnity;
using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace AGXUnity_UR_Robot.Script
{

  public class UR_RobotController : ScriptComponent
  {

    public enum Action
    {
      Base,
      Shoulder,
      Elbow,
      Wrist1,
      Wrist2,
      Wrist3,
      Hand,
      Fingers
    };



#if ENABLE_INPUT_SYSTEM
    [SerializeField]
    private InputActionAsset m_inputAsset = null;

    public InputActionAsset InputAsset
    {
      get
      {
        return m_inputAsset;
      }
      set
      {
        m_inputAsset = value;
        InputMap = m_inputAsset?.FindActionMap("ArticulatedRobot");

        if (InputMap != null && IsSynchronizingProperties)
        {
          m_hasValidInputActionMap = true;
          foreach (var actionName in System.Enum.GetNames(typeof(Action)))
          {
            if (InputMap.FindAction(actionName) == null)
            {
              Debug.LogWarning($"Unable to find Input Action: ArticulatedRobot.{actionName}");
              m_hasValidInputActionMap = false;
            }
          }

          if (m_hasValidInputActionMap)
            InputMap.Enable();
          else
            Debug.LogWarning("ArticulatedRobot input disabled due to missing action(s) in the action map.");
        }

        if (m_inputAsset != null && InputMap == null)
          Debug.LogWarning("InputActionAsset doesn't contain an ActionMap named \"ArticulatedRobot\".");
      }
    }

    private InputAction SaveAction;
    private InputAction ResetAction;

    public InputActionMap InputMap = null;
#endif

    [HideInInspector]

    public float GetInputValue(Action action)
    {
#if ENABLE_INPUT_SYSTEM
      return m_hasValidInputActionMap ? InputMap[action.ToString()].ReadValue<float>() : 0.0f;
#else
      var name = action.ToString();
      var jAction = Input.GetAxis( 'j' + name );
      return jAction != 0.0f ? jAction : Input.GetAxis( 'k' + name );
#endif
    }

#if ENABLE_INPUT_SYSTEM
    private bool m_hasValidInputActionMap = false;
#endif

    private UR_Robot m_robot;

    UR_Robot Robot
    {
      get
      {
        if (m_robot == null)
          m_robot = GetComponent<UR_Robot>();
        return m_robot;
      }
    }



    protected override bool Initialize()
    {

      SaveAction = new InputAction("Save", binding: "<Keyboard>/o");
      SaveAction.Enable();

      ResetAction = new InputAction("Reset", binding: "<Keyboard>/r");
      ResetAction.Enable();


      StoreTransforms("InitialState");

      return true;
    }

    private void ControlFingers(float input, float threshold)
    {
      Constraint[] constraints = {Robot.GetActuator(UR_Robot.Actuator.Finger1),
                                  Robot.GetActuator(UR_Robot.Actuator.Finger2)};
      foreach (var c in constraints)
      {
        if (Mathf.Abs(input) > threshold)
        {
          var lockController = c.GetController<LockController>();
          lockController.Enable = true;
          var range = c.GetController<RangeController>().Range;
          var curr_pos = c.GetCurrentAngle();
          lockController.Position = Mathf.Clamp(curr_pos + input * Time.fixedDeltaTime * 0.07f, range.Min, range.Max);
        }
      }
    }


    public void StoreTransforms(string name)
    {

      if (!m_body_transforms.ContainsKey(name))
      {
        m_body_transforms[name] = new List<TransformData>();
      }
      m_body_transforms[name].Clear();

      var bodies = UnityEngine.Object.FindObjectsOfType<AGXUnity.RigidBody>();
      foreach (var b in bodies)
      {
        if (!(b.hideFlags == HideFlags.NotEditable || b.hideFlags == HideFlags.HideAndDontSave))
          m_body_transforms[name].Add(new TransformData(b));
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

    private Dictionary<string, List<TransformData>> m_body_transforms = new Dictionary<string, List<TransformData>>();


    private void Reset()
    {
      RestoreTransforms("InitialState");
    }

    // Update is called once per frame
    void Update()
    {

      const float threshold = 0.01f;

#if ENABLE_INPUT_SYSTEM
      if (SaveAction.triggered)
      {
        GetSimulation().write("agxunity_simulation.agx");
      }

      if (ResetAction.triggered)
      {
        Reset();
      }

#endif


      foreach (Action action in System.Enum.GetValues(typeof(Action)))
      {
        // This one will be handled separately
        if (action == Action.Fingers)
          continue;

        float inputVal = GetInputValue(action);
        var constraint = Robot.GetActuator((UR_Robot.Actuator)action);
        var speedController = constraint.GetController<TargetSpeedController>();
        var lockController = constraint.GetController<LockController>();
        if (Mathf.Abs(inputVal) > threshold)
        {
          speedController.Speed = inputVal;
          lockController.Enable = false;
          speedController.Enable = true;
        }
        else
        {
          speedController.Speed = 0;
          lockController.Enable = true;
          lockController.Position = constraint.GetCurrentAngle();
        }
      }

      float input = GetInputValue(Action.Fingers);
      ControlFingers(input, threshold);
    }
  }
}