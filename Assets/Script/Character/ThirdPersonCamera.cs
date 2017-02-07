using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    private Transform m_CameraTransform;
    private Transform m_Target;
    private float m_fDistance = 7.0f;
    private float m_fHeight = 3.0f;
    private float m_fAngularSmoothLag = 0.3f;
    private float m_fAngularMaxSpeed = 15.0f;
    private float m_fHeightSmoothLag = 0.3f;
    private float m_fSnapSmoothLag = 0.2f;
    private float m_fSnapMaxSpeed = 720.0f;
    private float m_fClampHeadPositionScreenSpace = 0.75f;
    private float m_fLockCameraTimeout = 0.2f;  
    private float m_fHeightVelocity = 0.0f;
    private float m_fAngleVelocity = 0.0f;    
    private float m_fTargetHeight = 100000.0f;
    private Vector3 m_vHeadOffset = Vector3.zero;
    private Vector3 m_vCenterOffset = Vector3.zero;

    private bool m_bSnap = false;
    public CharacterControl m_Controller;

    void Awake()
    {
        if (!m_CameraTransform && Camera.main)
        {
            m_CameraTransform = Camera.main.transform;
        }
        if (!m_CameraTransform)
        {
             enabled = false;
        }

        m_Target = transform;
        if (m_Target)
        {
            m_Controller = m_Target.GetComponent<CharacterControl>();
        }

        if (m_Controller)
        {
            Collider characterController = m_Target.GetComponent<Collider>();
            m_vCenterOffset = characterController.bounds.center - m_Target.position;
            m_vHeadOffset = m_vCenterOffset;
            m_vHeadOffset.y = characterController.bounds.max.y - m_Target.position.y;
        }
    
        Cut(m_Target, m_vCenterOffset);
    }

    void DebugDrawStuff()
    {
        Debug.DrawLine(m_Target.position, m_Target.position + m_vHeadOffset);

    }
    // a 에서 b 까지의 각도를 계산한다.
    float AngleDistance(float a, float b)
    {
        a = Mathf.Repeat(a, 360);
        b = Mathf.Repeat(b, 360);

        return Mathf.Abs(b - a);
    }

    void Apply(Transform dummyTarget, Vector3 dummyCenter)
    {
        if (!m_Controller)
            return;

        var targetCenter = m_Target.position + m_vCenterOffset;
        var targetHead = m_Target.position + m_vHeadOffset;

        var originalTargetAngle = m_Target.eulerAngles.y;
        var currentAngle = m_CameraTransform.eulerAngles.y;

        var targetAngle = originalTargetAngle;

        if (Input.GetButton("Fire2"))
            m_bSnap = true;

        if (m_bSnap)
        {
            if (AngleDistance(currentAngle, originalTargetAngle) < 3.0)
                m_bSnap = false;

            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref m_fAngleVelocity, m_fSnapSmoothLag, m_fSnapMaxSpeed);
        }
        else
        {
            if (m_Controller.GetLockCameraTimer() < m_fLockCameraTimeout)
            {
                targetAngle = currentAngle;
            }

            if (AngleDistance(currentAngle, targetAngle) > 160 && m_Controller.IsMoveBackward())
                targetAngle += 180;

            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref m_fAngleVelocity, m_fAngularSmoothLag, m_fAngularMaxSpeed);
        }


        if (m_Controller.IsJumping())
        {
            var newTargetHeight = targetCenter.y + m_fHeight;
            if (newTargetHeight < m_fTargetHeight || newTargetHeight - m_fTargetHeight > 5)
                m_fTargetHeight = targetCenter.y + m_fHeight;
        }
        else
        {
            m_fTargetHeight = targetCenter.y + m_fHeight;
        }

        var currentHeight = m_CameraTransform.position.y;
        currentHeight = Mathf.SmoothDamp(currentHeight, m_fTargetHeight, ref m_fHeightVelocity, m_fHeightSmoothLag);

        var currentRotation = Quaternion.Euler(0, currentAngle, 0);

        m_CameraTransform.position = targetCenter;
        m_CameraTransform.position += currentRotation * Vector3.back * m_fDistance;

        m_CameraTransform.position = new Vector3(m_CameraTransform.position.x, currentHeight, m_CameraTransform.position.z);
        SetUpRotation(targetCenter, targetHead);
    }

    void LateUpdate()
    {
        Apply(transform, Vector3.zero);
    }

    void Cut(Transform dummyTarget, Vector3 dummyCenter)
    {
        var oldHeightSmooth = m_fHeightSmoothLag;
        var oldSnapMaxSpeed = m_fSnapMaxSpeed;
        var oldSnapSmooth = m_fSnapSmoothLag;

        m_fSnapMaxSpeed = 10000;
        m_fSnapSmoothLag = 0.001f;
        m_fHeightSmoothLag = 0.001f;

        m_bSnap = true;
        Apply(transform, Vector3.zero);

        m_fHeightSmoothLag = oldHeightSmooth;
        m_fSnapMaxSpeed = oldSnapMaxSpeed;
        m_fSnapSmoothLag = oldSnapSmooth;
    }

    void SetUpRotation(Vector3 centerPos, Vector3 headPos)
    {
        Vector3 cameraPos = m_CameraTransform.position;
        Vector3 offsetToCenter = centerPos - cameraPos;
        var yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));

        var relativeOffset = Vector3.forward * m_fDistance + Vector3.down * m_fHeight;
        m_CameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);

        Camera camera = m_CameraTransform.GetComponent<Camera>();
        var centerRay = camera.ViewportPointToRay(new Vector3(.5f, 0.5f, 1));
        var topRay = camera.ViewportPointToRay(new Vector3(.5f, m_fClampHeadPositionScreenSpace, 1));

        var centerRayPos = centerRay.GetPoint(m_fDistance);
        var topRayPos = topRay.GetPoint(m_fDistance);

        var centerToTopAngle = Vector3.Angle(centerRay.direction, topRay.direction);

        var heightToAngle = centerToTopAngle / (centerRayPos.y - topRayPos.y);

        var extraLookAngle = heightToAngle * (centerRayPos.y - centerPos.y);
        if (extraLookAngle < centerToTopAngle)
        {
            extraLookAngle = 0;
        }
        else
        {
            extraLookAngle = extraLookAngle - centerToTopAngle;
            m_CameraTransform.rotation *= Quaternion.Euler(-extraLookAngle, 0, 0);
        }
    }

    public Vector3 GetCenterOffset()
    {
        return m_vCenterOffset;
    }
}
