using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建造者模式
/// </summary>
public class BuilderStructure : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Builder builder = new MobikeBuilder();
        Director director = new Director(builder);
        director.Construct();
    }

    /// <summary>
    /// 抽象构建
    /// </summary>
    public abstract class Builder
    {
        public abstract void BuildBikeFrame();
        public abstract void BuildSeat();
        public abstract void BuildTire();

        public abstract Bike CreateBike();
    }

    #region 部件接口
    public interface IFrame { }
    public interface ISeat { }
    public interface ITire { }

    //MobikeBuilder
    public class AlloyFrame : IFrame
    {
        public AlloyFrame()
        {
            Debug.Log("创建合金骨架");
        }
    }//合金骨架
    public class DermisSeat : ISeat
    {
        public DermisSeat()
        {
            Debug.Log("创建真皮座椅");
        }
    }//真皮座椅
    public class SolidTire : ITire
    {
        public SolidTire()
        {
            Debug.Log("创建实心轮胎");
        }
    }//实心轮胎

    //OfoBuilder
    public class CarbonFrame : IFrame
    {
        public CarbonFrame()
        {
            Debug.Log("碳纤维车架");
        }
    }//碳纤维车架
    public class RubberSeat : ISeat {
        public RubberSeat() { Debug.Log("橡胶座"); }
    }//橡胶座
    public class InflateTire : ITire {
        public InflateTire()
        {
            Debug.Log("充气轮胎");
        }
    }//充气轮胎
    #endregion

    public class Bike
    {
        public IFrame Frame { get; set; }
        public ISeat Seat { get; set; }
        public ITire Tire { get; set; }
    }

    public class MobikeBuilder : Builder
    {
        Bike mobike = new Bike();

        public override void BuildBikeFrame()
        {
            mobike.Frame = new AlloyFrame();
        }
        public override void BuildSeat()
        {
            mobike.Seat = new DermisSeat();
        }
        public override void BuildTire()
        {
            mobike.Tire = new SolidTire();
        }

        public override Bike CreateBike()
        {
            return mobike;
        }
    }
    public class OfoBuilder : Builder
    {
        Bike Ofo = new Bike();

        public override void BuildBikeFrame()
        {
            Ofo.Frame = new CarbonFrame();
        }
        public override void BuildSeat()
        {
            Ofo.Seat = new RubberSeat();
        }
        public override void BuildTire()
        {
            Ofo.Tire = new InflateTire();
        }
        public override Bike CreateBike()
        {
            return Ofo;
        }
    }

    public class Director
    {
        private Builder builder = null;
        public Director(Builder builder)
        {
            this.builder = builder;
        }

        public Bike Construct()
        {
            builder.BuildBikeFrame();
            builder.BuildSeat();
            builder.BuildTire();

            return builder.CreateBike();
        }
    }
}


