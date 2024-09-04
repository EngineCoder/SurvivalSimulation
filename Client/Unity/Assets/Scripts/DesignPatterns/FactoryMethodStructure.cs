using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 工厂方法模式
/// </summary>
public class FactoryMethodStructure : MonoBehaviour
{
    void Start()
    {
        List<Factory> factories = new List<Factory>();
        factories.Add(new ProductAFactory());
        factories.Add(new ProductBFactory());
        foreach (Factory factory in factories)
        {
            Product product = factory.CreateProduct();

            Debug.Log("Created " + product.GetType().Name);
        }
    }

    #region 抽象工厂
    abstract class Factory
    {
        public abstract Product CreateProduct();
    }
    #endregion

    #region 具体工厂
    class ProductAFactory : Factory
    {
        public override Product CreateProduct()
        {
            return new ProductA();
        }
    }
    class ProductBFactory : Factory
    {
        public override Product CreateProduct()
        {
            return new ProductB();
        }
    }
    #endregion

    #region 抽象产品
    abstract class Product { }
    #endregion

    #region 具体产品
    class ProductA : Product { }
    class ProductB : Product { }
    #endregion

}
