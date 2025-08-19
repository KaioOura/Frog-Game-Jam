using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class VfxPoolManager : MonoBehaviour
{
    public ObjectPool<ParticleSystem> Pool => _pool;
    public List<ParticleSystem> Vfxs => _vfxs;

    [SerializeField] private int defaultCapacity = 7;
    [SerializeField] private int maxCapacity = 10;
    [SerializeField] private ParticleSystem orderPrefab;
    [SerializeField] private Transform ordersParent;
    
    private ObjectPool<ParticleSystem> _pool;
    private List<ParticleSystem> _vfxs = new List<ParticleSystem>();
    

    public void Initialize()
    {
        _pool = new ObjectPool<ParticleSystem>(
            createFunc: CreateOrder, OnGetOrderFromPool, OnReleaseToPool,
            OnDestroyOrder, true, defaultCapacity, maxCapacity);
        
        List<ParticleSystem> temp = new List<ParticleSystem>();
        
        for (int i = 0; i < defaultCapacity; i++)
        {
            var item = _pool.Get();
            temp.Add(item);
        }

        foreach (ParticleSystem order in temp)
        {
            _pool.Release(order);
        }
    }
    
    private ParticleSystem CreateOrder()
    {
        var pS = Instantiate(orderPrefab, ordersParent);
        //pS.OnReleaseToPool = ReleaseOrder;
        pS.gameObject.SetActive(false);
        
        _vfxs.Add(pS);
        return pS;
    }

    private void OnGetOrderFromPool(ParticleSystem pS)
    {
        pS.gameObject.SetActive(true);
        //ingredient.transform.position = Vector3.zero; // exemplo
    }

    private void OnReleaseToPool(ParticleSystem pS)
    {
        pS.transform.SetParent(ordersParent);
        
        pS.gameObject.SetActive(false);
        pS.transform.rotation = Quaternion.identity;
        pS.transform.localScale = Vector3.one;
    }

    private void OnDestroyOrder(ParticleSystem pS)
    {
        Destroy(pS.gameObject);
    }

    private void ReleaseOrder(ParticleSystem pS)
    {
        _pool.Release(pS);
    }
}
