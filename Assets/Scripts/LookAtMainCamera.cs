using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(LookAtConstraint))] //이 스크립트를 붙이면 항상 typeof안의 컴포넌트가 자동으로 붙는다.
public class LookAtMainCamera : MonoBehaviour
{
    private void Awake()
    {
        var source = new ConstraintSource();
        source.sourceTransform = Camera.main.transform;
        source.weight = 1;

        var lookAtConstraint = GetComponent<LookAtConstraint>();
        lookAtConstraint.AddSource(source);
        lookAtConstraint.constraintActive = true;

    }

}
