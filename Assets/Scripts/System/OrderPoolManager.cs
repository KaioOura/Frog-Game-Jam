using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class OrderPoolManager : MonoBehaviour
{
    public ObjectPool<Order> Pool => _pool;
    public List<Order> Orders => _orders;

    [SerializeField] private int defaultCapacity = 7;
    [SerializeField] private int maxCapacity = 10;
    [SerializeField] private Order orderPrefab;
    [SerializeField] private Transform ordersParent;
    
    private ObjectPool<Order> _pool;
    private List<Order> _orders = new List<Order>();
    

    public void Initialize()
    {
        _pool = new ObjectPool<Order>(
            createFunc: CreateOrder, OnGetOrderFromPool, OnReleaseToPool,
            OnDestroyOrder, true, defaultCapacity, maxCapacity);
        
        List<Order> temp = new List<Order>();
        
        for (int i = 0; i < defaultCapacity; i++)
        {
            var item = _pool.Get();
            temp.Add(item);
        }

        foreach (Order order in temp)
        {
            _pool.Release(order);
        }
    }
    
    private Order CreateOrder()
    {
        var order = Instantiate(orderPrefab, ordersParent);
        order.OnReleaseToPool = ReleaseOrder;
        order.gameObject.SetActive(false);
        
        _orders.Add(order);
        return order;
    }

    private void OnGetOrderFromPool(Order order)
    {
        order.gameObject.SetActive(true);
        //ingredient.transform.position = Vector3.zero; // exemplo
    }

    private void OnReleaseToPool(Order order)
    {
        order.transform.SetParent(ordersParent);
        
        order.gameObject.SetActive(false);
        order.transform.rotation = Quaternion.identity;
        order.transform.localScale = Vector3.one;
    }

    private void OnDestroyOrder(Order order)
    {
        Destroy(order.gameObject);
    }

    private void ReleaseOrder(Order order)
    {
        _pool.Release(order);
    }
}
