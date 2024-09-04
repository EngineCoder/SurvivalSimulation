using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽象工厂模式
/// </summary>
public class AbstractFactoryStructrue : MonoBehaviour
{
    void Start()
    {
        Factory_AutoParts factory_AutoParts_SUV = new Factory_AutoParts_SUV();
        Client client1 = new Client(factory_AutoParts_SUV);
        client1.Run();

        Factory_AutoParts factory_AutoParts_VAN = new Factory_AutoParts_VAN();
        Client client2 = new Client(factory_AutoParts_VAN);
        client2.Run();
    }

    #region 抽象工厂
    abstract class Factory_AutoParts
    {
        public abstract Engine CreateEngine();//生产引擎
        public abstract Tires CreateTires();//生产轮胎
    }
    #endregion

    #region 具体工厂
    class Factory_AutoParts_SUV : Factory_AutoParts
    {
        public override Engine CreateEngine()
        {
            return new Engine_SUV();//越野车引擎
        }
        public override Tires CreateTires()
        {
            return new Tires_SUV();//越野车轮胎
        }
    }
    class Factory_AutoParts_VAN : Factory_AutoParts
    {
        public override Engine CreateEngine()
        {
            return new Engine_VAN();//面包车引擎
        }
        public override Tires CreateTires()
        {
            return new Tires_VAN();//面包车轮胎
        }
    }
    #endregion

    #region 抽象产品
    abstract class Engine//生产引擎
    { }
    abstract class Tires//生产轮胎
    {
        public abstract void Interact(Engine a);
    }
    #endregion

    #region 具体产品
    class Engine_SUV : Engine
    {
        public Engine_SUV()
        {
            Debug.Log("制造出： 越野车引擎");
        }

    }
    class Engine_VAN : Engine
    {
        public Engine_VAN()
        {
            Debug.Log("制造出： 面包车引擎");
        }
    }

    class Tires_SUV : Tires
    {
        public Tires_SUV()
        {
            Debug.Log("制造出：越野车轮胎");
        }
        public override void Interact(Engine a)
        {
            Debug.Log(this.GetType().Name + " interacts with " + a.GetType().Name);
        }
    }
    class Tires_VAN : Tires
    {
        public Tires_VAN()
        {
            Debug.Log("制造出：面包车轮胎");
        }
        public override void Interact(Engine a)
        {
            Debug.Log(this.GetType().Name + " interacts with " + a.GetType().Name);
        }
    }
    #endregion

    class Client
    {
        private Engine engine;
        private Tires tires;

        public Client(Factory_AutoParts factory)
        {
            engine = factory.CreateEngine();
            tires = factory.CreateTires();
        }

        public void Run()
        {
            tires.Interact(engine);
        }
    }
}

