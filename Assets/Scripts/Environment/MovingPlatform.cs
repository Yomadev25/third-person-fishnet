using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Vector3[] _waypoints;

    private int _currentWaypoint;

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, _waypoints[_currentWaypoint]) < 0.5f)
        {
            _currentWaypoint++;
            if (_currentWaypoint > _waypoints.Length - 1)
            {
                _currentWaypoint = 0;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _waypoints[_currentWaypoint], _speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
