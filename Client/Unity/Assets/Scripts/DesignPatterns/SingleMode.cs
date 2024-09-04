using UnityEngine;

/// <summary>  
/// 功能:添加单例模板类,在想要创建单例的脚本中后面继承SingleMode<T>并添加下面的两行代码,
/// 就可以实现单例功能  例子(继承): public class GameManager :    SingleMode<GameManager>  
///private void Init(){}  
///void Awake(){ this.Init();}  
/// </summary>  
public class SingleMode<T> : MonoBehaviour where T : MonoBehaviour
{
    //脚本单例后接泛型<T> 继承 MonoBehaviour, 约束泛型<T>(也就是你要定义单例的脚本) : 继承MonoBehaviour      where后面接约束条件
    //这里的泛型<T> 就是代表要定义的单例脚本

    //定义静态的泛型<T>的类型的变量 _instance
    private static T _instance;

    //定义一个静态的objec类型的_lock并实例化
    private static object _lock = new object ();

    //定义bool类型的变量applicationIsQuitting 判断程序是否关闭
    private static bool applicationIsQuitting = false;

    void Awake(){ this.Init();}
    private void Init() { }

    public static T Instance//定义静态的泛型<T>类型的属性  
    {
        get//get方法  
        {
            //if (applicationIsQuitting)//如果applicationIsQuitting为真 则返回null  以下的代码不执行  
            //{
            //    return null;
            //}
            lock (_lock)//lock 关键字可以用来确保代码块完成运行，而不会被其他线程中断。  
            {
                if (null == _instance)//如果_instance等于空 执行一下代码  
                {
                    _instance = (T)FindObjectOfType(typeof(T));//_instance找到类型为泛型<T>(也就是想要定义为单例的脚本)  
                    if (FindObjectsOfType(typeof(T)).Length > 1) //如果_instance找到的长度大于  
                    {
                        return _instance;//返回_instance   保证单例的唯一性  
                    }

                    if (null == _instance) //如果_instance等于空  
                    {
                        GameObject singleton = new GameObject();//实例化出一个GameObject类型的singleton(单例)  
                        _instance = singleton.AddComponent<T>();//把GameObject类型的对象singleton添加上泛型<T>(想要定义为单例的脚本)组件(脚本)  
                        singleton.name = "[Single]" + typeof(T).ToString();//名字为......  

                        DontDestroyOnLoad(singleton);//切换场景后保留singleton  保持单例在场景中的传递,用于存储一些小型的数据  
                    }
                }
                return _instance;//返回出_instance单例  
            }
        }
    }

    public virtual void OnDestroy ()//定义一个无参无返回值的方法OnDestroy 用于清除定义的单例  
	{  
		//applicationIsQuitting = true;//设置applicationIsQuitting 为真  
	}
} 