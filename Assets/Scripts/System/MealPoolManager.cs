using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class MealPoolManager : MonoBehaviour
{
    
    public ObjectPool<GameObject> Pool => _pool;
    public List<GameObject> Meals => _meals;

    [SerializeField] private int defaultCapacity = 2;
    [SerializeField] private int maxCapacity = 5;
    [SerializeField] private GameObject mealPrefab;
    [SerializeField] private Transform mealParent;
    
    private ObjectPool<GameObject> _pool;
    private List<GameObject> _meals = new List<GameObject>();

    private List<IEnumerator> _releaseRequest = new List<IEnumerator>();
    

    public void Initialize()
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: CreateMeal, OnGetMealFromPool, OnReleaseToPool,
            OnDestroyOrder, true, defaultCapacity, maxCapacity);
        
        List<GameObject> temp = new List<GameObject>();
        
        for (int i = 0; i < defaultCapacity; i++)
        {
            var item = _pool.Get();
            temp.Add(item);
        }

        foreach (GameObject go in temp)
        {
            _pool.Release(go);
        }
        
        for (int i = 0; i < 5; i++)
        {
            _releaseRequest.Add(null);
        }
    }
    
    private GameObject CreateMeal()
    {
        var go = Instantiate(mealPrefab, mealParent);
        //pS.OnReleaseToPool = ReleaseOrder;
        go.gameObject.SetActive(false);
        
        _meals.Add(go);
        return go;
    }

    private void OnGetMealFromPool(GameObject go)
    {
        go.gameObject.SetActive(true);
        //ingredient.transform.position = Vector3.zero; // exemplo
    }

    private void OnReleaseToPool(GameObject go)
    {
        go.transform.SetParent(mealParent);
        
        go.gameObject.SetActive(false);
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
    }

    private void OnDestroyOrder(GameObject go)
    {
        Destroy(go.gameObject);
    }

    private void ReleaseOrder(GameObject go)
    {
        _pool.Release(go);
    }
    
    public void QueueReleaseWithDelay(GameObject go, float delay)
    {
        for (int i = 0; i < _releaseRequest.Count; i++)
        {
            if (_releaseRequest[i] != null) continue;
            
            var i1 = i;
            
            _releaseRequest[i] = ReleaseAfterDelay(go, delay, () => _releaseRequest[i1] = null);
            
            StartCoroutine(_releaseRequest[i]);
            
            break;
        }
    }
    
    private IEnumerator ReleaseAfterDelay(GameObject go, float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        
        _pool.Release(go);
        callback?.Invoke();
    }
}
